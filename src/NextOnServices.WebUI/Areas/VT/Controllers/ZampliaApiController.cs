using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.APIProjects;
using NextOnServices.WebUI.VT.Services;

namespace NextOnServices.WebUI.VT.Controllers;

[Area("VT")]
[Authorize]
[Route("VT/[controller]")]
public class ZampliaApiController : Controller
{
    private readonly ILogger<ZampliaApiController> _logger;
    private readonly ZampliaAPIController _zampliaAPIController;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IZampliaLaunchService _zampliaLaunchService;
    private readonly ILegacyProjectStatusService _legacyProjectStatusService;

    public ZampliaApiController(
        ILogger<ZampliaApiController> logger,
        ZampliaAPIController zampliaAPIController,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IZampliaLaunchService zampliaLaunchService,
        ILegacyProjectStatusService legacyProjectStatusService)
    {
        _logger = logger;
        _zampliaAPIController = zampliaAPIController;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _zampliaLaunchService = zampliaLaunchService;
        _legacyProjectStatusService = legacyProjectStatusService;
    }

    [HttpPost("SaveSettings")]
    public async Task<IActionResult> SaveSettings([FromBody] ZampliaSettingDTO inputData)
    {
        if (inputData == null)
        {
            return BadRequest(new { result = false, message = "Settings payload is required." });
        }

        var userId = GetCurrentUserId();
        inputData.CreatedBy ??= userId;
        inputData.ModifiedBy = userId;
        var apiResult = await _zampliaAPIController.SaveZampliaSetting(inputData);
        await AppendLogAsync("SaveSettings", "/VT/ZampliaApi/SaveSettings", JsonConvert.SerializeObject(inputData), "manual", null, null, apiResult: apiResult);
        return ConvertApiResult(apiResult);
    }

    [HttpPost("TestConnectivity")]
    public async Task<IActionResult> TestConnectivity([FromBody] ZampliaConnectivityRequest inputData)
    {
        if (inputData == null)
        {
            return BadRequest(new { result = false, message = "Connectivity payload is required." });
        }

        var proxyResponse = await CallConsultingsProxyAsync("TestConnectivity", new ZampliaProxyRequest
        {
            Setting = new ZampliaSettingDTO
            {
                BaseUrl = inputData.BaseUrl,
                ApiKey = inputData.ApiKey,
                UseConsultingsBridge = inputData.UseConsultingsBridge
            }
        });

        await AppendLogAsync("TestConnectivity", proxyResponse.RequestUrl ?? "/VT/ZampliaApi/TestConnectivity", JsonConvert.SerializeObject(inputData), "manual", null, null, proxyResponse: proxyResponse);
        return Json(new { result = proxyResponse.Result, statusCode = proxyResponse.StatusCode, message = proxyResponse.Message });
    }

    [HttpPost("SyncAllocatedSurveys")]
    public async Task<IActionResult> SyncAllocatedSurveys()
    {
        var setting = await GetCurrentSettingAsync();
        if (setting == null || string.IsNullOrWhiteSpace(setting.ApiKey))
        {
            return Json(new { result = false, message = "Save Zamplia settings before syncing surveys." });
        }

        var userId = GetCurrentUserId();
        var allocatedResponse = await CallConsultingsProxyAsync("GetAllocatedSurveys", new ZampliaProxyRequest { Setting = setting });
        await AppendLogAsync("SyncAllocatedSurveys", allocatedResponse.RequestUrl ?? "/VT/ZampliaApi/SyncAllocatedSurveys", null, "manual", null, null, proxyResponse: allocatedResponse);
        if (!allocatedResponse.Result || string.IsNullOrWhiteSpace(allocatedResponse.ResponseBody))
        {
            return Json(new { result = false, message = allocatedResponse.Message ?? "Unable to load allocated surveys." });
        }

        var processResult = await _zampliaAPIController.ProcessAllocatedSurveysPayloadAsync(allocatedResponse.ResponseBody, "manual", userId);
        var detailErrors = new List<string>();
        foreach (var surveyId in processResult.SurveyIds.Distinct())
        {
            var detailResult = await SyncSurveyDetailsInternalAsync(surveyId, userId, "manual");
            var detailOutcome = ReadOperationOutcome(detailResult);
            if (!detailOutcome.Success)
            {
                detailErrors.Add($"Survey {surveyId}: {detailOutcome.Message ?? "detail sync failed."}");
            }
        }

        return Json(new
        {
            result = processResult.Errors.Count == 0 && detailErrors.Count == 0,
            processed = processResult.ProcessedCount,
            inactivated = processResult.InactivatedCount,
            skipped = processResult.SkippedCount,
            syncedSurveyIds = processResult.SurveyIds.Distinct().ToList(),
            inactivatedSurveyIds = processResult.InactivatedSurveyIds.Distinct().ToList(),
            errors = processResult.Errors.Concat(detailErrors).ToList(),
            message = $"Processed {processResult.ProcessedCount} survey row(s), marked {processResult.InactivatedCount} inactive, skipped {processResult.SkippedCount}, and attempted detail hydration for {processResult.SurveyIds.Distinct().Count()} survey id(s)."
        });
    }

    [HttpPost("RemoveInactiveSurveys")]
    public async Task<IActionResult> RemoveInactiveSurveys()
    {
        var cleanupResult = await _zampliaAPIController.RemoveInactiveSurveysAsync(GetCurrentUserId(), "manual");
        return Json(new
        {
            result = true,
            removed = cleanupResult.RemovedCount,
            removedSurveyIds = cleanupResult.RemovedSurveyIds,
            message = cleanupResult.RemovedCount > 0
                ? $"Removed {cleanupResult.RemovedCount} inactive survey(s) permanently from Zamplia."
                : "No inactive Zamplia surveys were found to remove."
        });
    }

    [HttpPost("SyncSurveyDetails")]
    public async Task<IActionResult> SyncSurveyDetails([FromBody] ZampliaAddProjectRequest inputData)
    {
        if (inputData == null || inputData.ZampliaSurveyId <= 0)
        {
            return BadRequest(new { result = false, message = "A valid Zamplia survey id is required." });
        }

        var survey = await GetSurveyAsync(inputData.ZampliaSurveyId);
        if (survey == null || survey.ZampliaSurveyId <= 0 || survey.SurveyId <= 0)
        {
            return Json(new { result = false, message = "Survey not found." });
        }

        var detailResult = await SyncSurveyDetailsInternalAsync(survey.SurveyId, GetCurrentUserId(), "manual");
        return Json(detailResult);
    }

    [HttpPost("AddProjectFromZamplia")]
    public async Task<IActionResult> AddProjectFromZamplia([FromBody] ZampliaAddProjectRequest inputData)
    {
        if (inputData == null || inputData.ZampliaSurveyId <= 0)
        {
            return BadRequest(new { result = false, message = "A valid Zamplia survey is required." });
        }

        var survey = await GetSurveyAsync(inputData.ZampliaSurveyId);
        if (survey == null || survey.ZampliaSurveyId <= 0)
        {
            return Json(new { result = false, message = "Invalid survey." });
        }

        if (survey.IsMapped)
        {
            return Json(new { result = false, message = "Already mapped." });
        }

        var surveyLinkResult = await EnsureSurveyLinkAsync(survey.ZampliaSurveyId, GetCurrentUserId());
        if (!surveyLinkResult.Success || string.IsNullOrWhiteSpace(surveyLinkResult.VendorLink))
        {
            return Json(new { result = false, message = surveyLinkResult.Message ?? "Generate the Zamplia survey link before adding the project." });
        }

        var currentSetting = await GetCurrentSettingAsync();
        var addRequest = new AddProjectFromLucidRequest
        {
            ProjectId = survey.SurveyId.ToString(CultureInfo.InvariantCulture),
            ProjectName = string.IsNullOrWhiteSpace(survey.SurveyName) ? survey.SurveyId.ToString(CultureInfo.InvariantCulture) : survey.SurveyName,
            CPI = survey.CPI?.ToString("0.####", CultureInfo.InvariantCulture),
            LOI = survey.LOI?.ToString(CultureInfo.InvariantCulture),
            IR = survey.IR?.ToString("0", CultureInfo.InvariantCulture),
            TotalRemaining = survey.TotalCompleteRequired?.ToString(CultureInfo.InvariantCulture),
            LiveLink = surveyLinkResult.VendorLink,
            ClientId = currentSetting?.DefaultClientId,
            CountryId = currentSetting?.DefaultCountryId,
            SupplierId = currentSetting?.DefaultSupplierId
        };

        var apiProjectsController = ActivatorUtilities.CreateInstance<ApiProjectsController>(HttpContext.RequestServices);
        apiProjectsController.ControllerContext = ControllerContext;
        var addProjectAction = await apiProjectsController.AddProjectFromZamplia(addRequest);
        var addProjectResult = ExtractVendorAddProjectResult(addProjectAction);

        if (addProjectResult == null || !addProjectResult.Result)
        {
            return Json(new { result = false, message = addProjectResult?.Message ?? "Project creation failed." });
        }

        await _zampliaAPIController.SaveZampliaProjectMap(new ZampliaProjectMapDTO
        {
            ZampliaSurveyId = survey.ZampliaSurveyId,
            SurveyId = survey.SurveyId,
            InternalProjectId = addProjectResult.ProjectId,
            InternalProjectUrlId = addProjectResult.ProjectUrlId,
            InternalProjectMappingId = addProjectResult.ProjectMappingId,
            AddedBy = GetCurrentUserId(),
            AddedOn = DateTime.Now,
            IsActive = true,
            RawJson = null
        });

        survey.LocalState = "Mapped";
        survey.ModifiedBy = GetCurrentUserId();
        await _zampliaAPIController.SaveZampliaSurvey(survey);
        await GenerateEntryLinkInternalAsync(survey.ZampliaSurveyId, GetCurrentUserId(), false);

        return Json(new
        {
            result = true,
            message = addProjectResult.Message ?? "Project added successfully.",
            projectId = addProjectResult.ProjectId,
            projectUrlId = addProjectResult.ProjectUrlId,
            projectMappingId = addProjectResult.ProjectMappingId
        });
    }

    [HttpPost("GenerateEntryLinkFromSurvey")]
    public async Task<IActionResult> GenerateEntryLinkFromSurvey([FromBody] ZampliaAddProjectRequest inputData)
    {
        if (inputData == null || inputData.ZampliaSurveyId <= 0)
        {
            return BadRequest(new { result = false, message = "A valid Zamplia survey is required." });
        }

        var result = await GenerateEntryLinkInternalAsync(inputData.ZampliaSurveyId, GetCurrentUserId(), false);
        return Json(result);
    }

    [HttpPost("GenerateSurveyLink")]
    public async Task<IActionResult> GenerateSurveyLink([FromBody] ZampliaAddProjectRequest inputData)
    {
        if (inputData == null || inputData.ZampliaSurveyId <= 0)
        {
            return BadRequest(new { result = false, message = "A valid Zamplia survey is required." });
        }

        var result = await EnsureSurveyLinkAsync(inputData.ZampliaSurveyId, GetCurrentUserId(), true);
        return Json(new
        {
            result = result.Success,
            vendorLink = result.VendorLink,
            transactionId = result.TransactionId,
            ipAddress = result.IpAddress,
            internalLaunchUrl = result.InternalLaunchUrl,
            message = result.Message
        });
    }

    [AllowAnonymous]
    [HttpGet("/VT/Zamplia/LaunchRespondent")]
    public async Task<IActionResult> LaunchRespondent(int zampliaSurveyId, int internalProjectId, int? internalProjectUrlId = null, int? internalProjectMappingId = null, string? respondentId = null)
    {
        var resolvedRespondentId = ResolveLaunchRespondentId(respondentId);
        if (string.IsNullOrWhiteSpace(resolvedRespondentId))
        {
            return Content("Replace [respondentID] in the Zamplia launch URL with a real respondent id before opening it.", "text/plain");
        }

        var context = await _zampliaAPIController.GetZampliaLaunchContextAsync(zampliaSurveyId, internalProjectId, internalProjectMappingId);
        if (context == null || context.ZampliaSurveyId <= 0 || context.SurveyId <= 0)
        {
            return Content("Invalid Zamplia launch context.", "text/plain");
        }

        var resolution = await _zampliaLaunchService.TryResolveVendorLaunchAsync(new ZampliaLaunchServiceRequest
        {
            ZampliaSurveyId = context.ZampliaSurveyId,
            ProjectMappingSid = context.ProjectMappingSid,
            ProjectMappingCode = context.ProjectMappingCode,
            SourceRespondentId = resolvedRespondentId,
            ClientIp = GetClientIpAddress(),
            ClientBrowser = CommonHelper.ParseBrowser(Request),
            ClientDevice = CommonHelper.Device(Request),
            Enc = GetQueryValue("ENC", "enc"),
            LaunchRequestUrl = Request.GetDisplayUrl(),
            UserId = GetCurrentUserId()
        });

        if (!resolution.Success || string.IsNullOrWhiteSpace(resolution.VendorLaunchUrl))
        {
            return Content(resolution.Message ?? "Unable to resolve the Zamplia launch URL.", "text/plain");
        }

        if (resolution.AttemptId.HasValue && resolution.AttemptId.Value > 0 && resolution.ZampliaSurveyId.HasValue && resolution.ZampliaSurveyId.Value > 0)
        {
            Response.Cookies.Append(GetAttemptCookieName(resolution.ZampliaSurveyId.Value), resolution.AttemptId.Value.ToString(CultureInfo.InvariantCulture), new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps,
                Expires = DateTimeOffset.UtcNow.AddHours(4)
            });
        }

        return Redirect(resolution.VendorLaunchUrl);
    }

    [AllowAnonymous]
    [HttpGet("/VT/Zamplia/Return/Complete")]
    public Task<IActionResult> ReturnComplete() => HandleReturnAsync("Complete");

    [AllowAnonymous]
    [HttpGet("/VT/Zamplia/Return/Terminate")]
    public Task<IActionResult> ReturnTerminate() => HandleReturnAsync("Terminate");

    [AllowAnonymous]
    [HttpGet("/VT/Zamplia/Return/OverQuota")]
    public Task<IActionResult> ReturnOverQuota() => HandleReturnAsync("OverQuota");

    [AllowAnonymous]
    [HttpGet("/VT/Zamplia/Return/Quality")]
    public Task<IActionResult> ReturnQuality() => HandleReturnAsync("QualityTermination");

    [AllowAnonymous]
    [HttpGet("/VT/Zamplia/Return/Security")]
    public Task<IActionResult> ReturnSecurity() => HandleReturnAsync("SecurityTermination");

    [HttpPost("RunReconciliation")]
    public async Task<IActionResult> RunReconciliation([FromBody] ZampliaReconciliationRunRequest? inputData)
    {
        var setting = await GetCurrentSettingAsync();
        if (setting == null)
        {
            return Json(new { result = false, message = "Save Zamplia settings before reconciliation." });
        }

        var proxyResponse = await CallConsultingsProxyAsync("GetReconciliation", new ZampliaProxyRequest
        {
            Setting = setting,
            SurveyId = inputData?.SurveyId,
            TransactionId = inputData?.TransactionId,
            QueryParameters = BuildReconciliationQueryParameters(inputData)
        });

        await AppendLogAsync("RunReconciliation", proxyResponse.RequestUrl ?? "/VT/ZampliaApi/RunReconciliation", JsonConvert.SerializeObject(inputData), "manual", null, inputData?.SurveyId, proxyResponse: proxyResponse);
        if (!proxyResponse.Result || string.IsNullOrWhiteSpace(proxyResponse.ResponseBody))
        {
            return Json(new { result = false, message = proxyResponse.Message ?? "Unable to fetch reconciliation data." });
        }

        var build = await BuildReconciliationAsync(proxyResponse.ResponseBody, inputData, GetCurrentUserId());
        var runId = await _zampliaAPIController.CreateReconciliationRunAsync(build.Run, build.Items);
        return Json(new { result = true, runId, totalReviewed = build.Run.TotalReviewed, totalMismatched = build.Run.TotalMismatched, message = $"Reconciliation run #{runId} completed." });
    }

    [HttpGet("LogDetails/{id}")]
    public async Task<IActionResult> LogDetails(int id)
    {
        var apiResult = await _zampliaAPIController.GetZampliaLog(id);
        return ConvertApiResult(apiResult);
    }

    [HttpPost("GetSessionEvents")]
    public async Task<IActionResult> GetSessionEvents([FromBody] ZampliaSessionEventsRequest inputData)
    {
        if (inputData == null || inputData.SurveyId <= 0 || string.IsNullOrWhiteSpace(inputData.TransactionId))
        {
            return BadRequest(new { result = false, message = "SurveyId and TransactionId are required." });
        }

        var setting = await GetCurrentSettingAsync();
        var proxyResponse = await CallConsultingsProxyAsync("GetUsersSessionEvents", new ZampliaProxyRequest
        {
            Setting = setting,
            SurveyId = inputData.SurveyId,
            TransactionId = inputData.TransactionId
        });

        await AppendLogAsync("GetSessionEvents", proxyResponse.RequestUrl ?? "/VT/ZampliaApi/GetSessionEvents", JsonConvert.SerializeObject(inputData), "manual", null, inputData.SurveyId, proxyResponse: proxyResponse);
        return Json(new { result = proxyResponse.Result, message = proxyResponse.Message, rawJson = proxyResponse.ResponseBody });
    }

    private async Task<IActionResult> HandleReturnAsync(string status)
    {
        var rawUrl = Request.GetDisplayUrl();
        var setting = await GetCurrentSettingAsync();
        var transactionId = GetQueryValue(true, "TransactionId", "transactionId", "tid");
        var respondentId = GetQueryValue(true, "uid", "UID", "respondentId", "RespondentId", "ID", "id");
        var hash = GetQueryValue(true, "hash", "Hash");
        var zampliaSurveyId = ParseNullableInt(GetQueryValue(true, "zampliaSurveyId", "ZampliaSurveyId", "surveyId", "SurveyId"));

        var attempt = !string.IsNullOrWhiteSpace(transactionId)
            ? await _zampliaAPIController.GetLatestZampliaRespondentAttemptAsync(transactionId, null, zampliaSurveyId)
            : null;
        attempt ??= !string.IsNullOrWhiteSpace(respondentId)
            ? await _zampliaAPIController.GetLatestZampliaRespondentAttemptAsync(null, respondentId, zampliaSurveyId)
            : null;
        attempt ??= await LoadAttemptFromCookieAsync();

        var supplierProjectUid = FirstNonEmpty(transactionId, attempt?.TransactionId, respondentId, attempt?.RespondentId);

        ZampliaHmacValidationResult? hmacValidation = null;
        if (!string.IsNullOrWhiteSpace(setting?.ExitHmacKey))
        {
            hmacValidation = ZampliaHmacHelper.ValidateIncomingReturnUrl(setting.ExitHmacKey!, rawUrl, hash ?? string.Empty);
            if (!hmacValidation.IsValid)
            {
                if (attempt != null)
                {
                    attempt.HmacReceived = hash;
                    attempt.HmacCalculated = hmacValidation.CalculatedHash;
                    attempt.HmacValid = false;
                    attempt.ReturnUrl = rawUrl;
                    attempt.ReturnRawQuery = Request.QueryString.Value;
                    attempt.ModifiedBy = GetCurrentUserId();
                    await _zampliaAPIController.SaveZampliaRespondentAttempt(attempt);
                }

                await AppendLogAsync("ReturnRejected", rawUrl, Request.QueryString.Value, "return", attempt?.Id, attempt?.SurveyId ?? zampliaSurveyId, responseBodyOverride: "Invalid HMAC hash.", responseStatusCodeOverride: StatusCodes.Status400BadRequest, forceSuccess: false);
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Content("Invalid hash.", "text/plain");
            }
        }

        var statusInfo = ZampliaReconciliationHelper.ResolveStatusInfo(status);
        if (attempt == null)
        {
            if (string.IsNullOrWhiteSpace(supplierProjectUid))
            {
                await AppendLogAsync("ReturnWithoutCorrelation", rawUrl, Request.QueryString.Value, "return", null, zampliaSurveyId, responseBodyOverride: "Return callback could not be correlated to a SupplierProjects UID.", responseStatusCodeOverride: StatusCodes.Status404NotFound, forceSuccess: false);
                return Content(statusInfo.ProjectStatus, "text/plain");
            }

            return await FinalizeLegacyProjectStatusResponseAsync(supplierProjectUid, statusInfo.ProjectStatus);
        }

        attempt.ReturnUrl = rawUrl;
        attempt.ReturnRawQuery = Request.QueryString.Value;
        attempt.ReturnStatus = statusInfo.NormalizedStatus;
        attempt.ReturnCode = GetQueryValue(true, "code", "Code", "status", "Status", "rc", "RC");
        attempt.FinalStatus = statusInfo.NormalizedStatus;
        attempt.FinalStatusSource = "ZampliaReturn";
        attempt.IsCompleted = statusInfo.IsCompleted;
        attempt.IsTerminated = statusInfo.IsTerminated;
        attempt.IsOverQuota = statusInfo.IsOverQuota;
        attempt.IsQualityTermination = statusInfo.IsQualityTermination;
        attempt.IsSecurityTermination = statusInfo.IsSecurityTermination;
        attempt.IsDuplicate = attempt.IsDuplicate || statusInfo.IsDuplicate;
        attempt.SessionId ??= GetQueryValue(true, "sessionId", "SessionId");
        attempt.CompletedOn ??= DateTime.Now;
        attempt.HmacReceived = hash;
        attempt.HmacCalculated = hmacValidation?.CalculatedHash;
        attempt.HmacValid = string.IsNullOrWhiteSpace(setting?.ExitHmacKey) ? null : hmacValidation?.IsValid ?? false;
        attempt.TransactionId ??= supplierProjectUid;
        attempt.RespondentId = FirstNonEmpty(attempt.RespondentId, supplierProjectUid);
        attempt.ModifiedBy = GetCurrentUserId();

        await _zampliaAPIController.SaveZampliaRespondentAttempt(attempt);
        Response.Cookies.Delete(GetAttemptCookieName(attempt.ZampliaSurveyId));

        supplierProjectUid = FirstNonEmpty(supplierProjectUid, attempt.TransactionId, attempt.RespondentId);
        return string.IsNullOrWhiteSpace(supplierProjectUid)
            ? Content(statusInfo.ProjectStatus, "text/plain")
            : await FinalizeLegacyProjectStatusResponseAsync(supplierProjectUid, statusInfo.ProjectStatus);
    }

    private async Task<IActionResult> FinalizeLegacyProjectStatusResponseAsync(string supplierProjectUid, string projectStatus)
    {
        var legacyStatusResult = await _legacyProjectStatusService.ApplyAsync(supplierProjectUid, projectStatus, 0, null, false);
        if (!string.Equals(legacyStatusResult.UpdateResponse, "1", StringComparison.OrdinalIgnoreCase))
        {
            await AppendLogAsync(
                "ReturnStatusUpdateFailed",
                Request.GetDisplayUrl(),
                Request.QueryString.Value,
                "return",
                null,
                null,
                responseBodyOverride: $"Failed to update SupplierProjects status for UID {supplierProjectUid}.",
                responseStatusCodeOverride: StatusCodes.Status500InternalServerError,
                forceSuccess: false);

            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return Content(projectStatus, "text/plain");
        }

        if (string.IsNullOrWhiteSpace(legacyStatusResult.RedirectUrl))
        {
            return Content(projectStatus, "text/plain");
        }

        ViewBag.RedirectUrl = legacyStatusResult.RedirectUrl;
        ViewBag.Message = BuildProjectStatusMessage(legacyStatusResult.NormalizedStatus);
        ViewBag.Title = string.Empty;
        return View("~/Areas/VT/Views/Survey/ProjectStatus.cshtml");
    }

    private static string BuildProjectStatusMessage(string? normalizedStatus)
    {
        var status = normalizedStatus?.Trim().ToUpperInvariant() ?? string.Empty;
        return status switch
        {
            "COMPLETE" => "Congratulations! You have successfully completed the survey.",
            "TERMINATE" => "Thank you very much for your participation. <br> Unfortunately, at the moment we are looking for a different profile to match survey's conditions.",
            "QUOTAFULL" => "Thank you very much for your participation, but at this time we have received specific numbers of completes.",
            "SCREENED" => "Thank you for your interest in this survey. <br> Unfortunately, you did not match the criteria required for participation.",
            "OVERQUOTA" => "Thank you for your interest in this survey. <br> Unfortunately, the quota was reached for your demographic group.",
            "SEC_TERM" => "Thank you very much for your participation. <br> Unfortunately, at the moment we are lookng for a different profile to match survey's conditions.",
            "F_ERROR" => "Thank you very much for your participation. <br> Unfortunately, at the moment we are lookng for a different profile to match survey's conditions.",
            _ => "Thank you for your interest in this survey."
        };
    }

    private async Task<object> SyncSurveyDetailsInternalAsync(long surveyId, int? userId, string source)
    {
        var setting = await GetCurrentSettingAsync();
        if (setting == null || string.IsNullOrWhiteSpace(setting.ApiKey))
        {
            return new { result = false, message = "Zamplia settings are not configured." };
        }

        var detailResponse = await CallConsultingsProxyAsync("GetSurveyById", new ZampliaProxyRequest { Setting = setting, SurveyId = surveyId });
        await AppendLogAsync("SyncSurveyDetails", detailResponse.RequestUrl ?? "/VT/ZampliaApi/SyncSurveyDetails", JsonConvert.SerializeObject(new { surveyId }), source, null, surveyId, proxyResponse: detailResponse);
        if (!detailResponse.Result || string.IsNullOrWhiteSpace(detailResponse.ResponseBody))
        {
            return new
            {
                result = false,
                message = detailResponse.Message ?? $"Survey detail sync failed for survey {surveyId}.",
                requestUrl = detailResponse.RequestUrl,
                statusCode = detailResponse.StatusCode
            };
        }

        var entityId = await _zampliaAPIController.UpsertSurveyAsync(detailResponse.ResponseBody, source, userId);
        if (entityId <= 0)
        {
            return new
            {
                result = false,
                message = $"Survey detail payload for survey {surveyId} did not contain a usable survey object.",
                requestUrl = detailResponse.RequestUrl,
                statusCode = detailResponse.StatusCode
            };
        }

        var qualificationsResponse = await CallConsultingsProxyAsync("GetSurveyQualifications", new ZampliaProxyRequest { Setting = setting, SurveyId = surveyId });
        var qualificationCount = await _zampliaAPIController.UpsertQualificationsAsync(entityId, surveyId, qualificationsResponse.ResponseBody ?? "[]", userId);
        await AppendLogAsync("SyncSurveyQualifications", qualificationsResponse.RequestUrl ?? string.Empty, JsonConvert.SerializeObject(new { surveyId }), source, entityId, surveyId, proxyResponse: qualificationsResponse);

        var quotasResponse = await CallConsultingsProxyAsync("GetSurveyQuotas", new ZampliaProxyRequest { Setting = setting, SurveyId = surveyId });
        var quotaCount = await _zampliaAPIController.UpsertQuotasAsync(entityId, surveyId, quotasResponse.ResponseBody ?? "[]", userId);
        await AppendLogAsync("SyncSurveyQuotas", quotasResponse.RequestUrl ?? string.Empty, JsonConvert.SerializeObject(new { surveyId }), source, entityId, surveyId, proxyResponse: quotasResponse);

        return new
        {
            result = true,
            surveyId,
            zampliaSurveyId = entityId,
            qualificationCount,
            quotaCount,
            message = "Survey details synced successfully."
        };
    }

    private static (bool Success, string? Message) ReadOperationOutcome(object? payload)
    {
        if (payload == null)
        {
            return (false, null);
        }

        using var document = JsonDocument.Parse(JsonConvert.SerializeObject(payload));
        var success = ZampliaJsonHelper.GetJsonBoolean(document.RootElement, "result", "Result") ?? false;
        var message = ZampliaJsonHelper.GetJsonString(document.RootElement, "message", "Message");
        return (success, message);
    }

    private async Task<object> GenerateEntryLinkInternalAsync(int zampliaSurveyId, int? userId, bool syncProjectUrl)
    {
        var context = await _zampliaAPIController.GetZampliaLaunchContextAsync(zampliaSurveyId);
        if (context == null || context.ZampliaSurveyId <= 0 || context.SurveyId <= 0)
        {
            return new { result = false, message = "Mapped Zamplia project context was not found." };
        }

        var existingEntryLink = await GetEntryLinkAsync(zampliaSurveyId);
        var maskingLaunchUrl = BuildMaskingLaunchUrl(context, "[respondentID]");
        var runtimePlaceholderUrl = BuildRuntimePlaceholderUrl(context.SurveyId);
        var saveResult = await _zampliaAPIController.SaveZampliaEntryLink(new ZampliaEntryLinkDTO
        {
            Id = existingEntryLink?.Id ?? 0,
            ZampliaSurveyId = context.ZampliaSurveyId,
            SurveyId = context.SurveyId,
            TransactionId = existingEntryLink?.TransactionId,
            VendorLink = existingEntryLink?.VendorLink,
            InternalLaunchUrl = maskingLaunchUrl,
            HashApplied = existingEntryLink?.HashApplied ?? false,
            IsActive = true,
            CreatedBy = existingEntryLink?.CreatedBy ?? userId,
            ModifiedBy = userId,
            RawJson = JsonConvert.SerializeObject(new
            {
                runtimeMode = "MaskingUrlThenGenerateLink",
                runtimePlaceholderUrl,
                preservedVendorLink = !string.IsNullOrWhiteSpace(existingEntryLink?.VendorLink)
            })
        });

        if (syncProjectUrl)
        {
            await _zampliaAPIController.SyncZampliaProjectLaunchUrlAsync(context.ZampliaSurveyId, runtimePlaceholderUrl);
        }

        await AppendLogAsync("GenerateEntryLink", maskingLaunchUrl, JsonConvert.SerializeObject(new { context.ZampliaSurveyId, context.SurveyId, runtimePlaceholderUrl }), "manual", context.ZampliaSurveyId, context.SurveyId, apiResult: saveResult);
        return new
        {
            result = true,
            internalLaunchUrl = maskingLaunchUrl,
            vendorLink = existingEntryLink?.VendorLink,
            message = "Internal launch template refreshed."
        };
    }

    private async Task<ZampliaSurveyLinkGenerationResult> EnsureSurveyLinkAsync(int zampliaSurveyId, int? userId, bool forceRefresh = false)
    {
        var survey = await GetSurveyAsync(zampliaSurveyId);
        if (survey == null || survey.ZampliaSurveyId <= 0 || survey.SurveyId <= 0)
        {
            return new ZampliaSurveyLinkGenerationResult { Success = false, Message = "Survey not found." };
        }

        var existingEntryLink = await GetEntryLinkAsync(zampliaSurveyId);
        if (!forceRefresh && !string.IsNullOrWhiteSpace(existingEntryLink?.VendorLink))
        {
            return new ZampliaSurveyLinkGenerationResult
            {
                Success = true,
                VendorLink = existingEntryLink.VendorLink,
                TransactionId = existingEntryLink.TransactionId,
                InternalLaunchUrl = existingEntryLink.InternalLaunchUrl,
                Message = "Stored survey link loaded."
            };
        }

        var setting = await GetCurrentSettingAsync();
        if (setting == null || string.IsNullOrWhiteSpace(setting.ApiKey))
        {
            return new ZampliaSurveyLinkGenerationResult
            {
                Success = false,
                Message = "Save Zamplia settings before generating the survey link."
            };
        }

        var transactionId = BuildSurveyLinkTransactionId(survey.SurveyId);
        var ipAddress = ResolveSurveyLinkIpAddress();
        var proxyRequest = new ZampliaProxyRequest
        {
            Setting = new ZampliaSettingDTO
            {
                BaseUrl = string.IsNullOrWhiteSpace(setting.BaseUrl) ? "https://surveysupplysandbox.zamplia.com" : setting.BaseUrl.Trim(),
                ApiKey = setting.ApiKey,
                ExitHmacKey = setting.ExitHmacKey,
                UseConsultingsBridge = setting.UseConsultingsBridge,
                IsActive = setting.IsActive
            },
            SurveyId = survey.SurveyId,
            IpAddress = ipAddress,
            TransactionId = transactionId
        };

        var proxyResponse = await CallConsultingsProxyAsync("GenerateLink", proxyRequest);
        await AppendLogAsync(
            "GenerateSurveyLink",
            proxyResponse.RequestUrl ?? "/VT/ZampliaApi/GenerateSurveyLink",
            JsonConvert.SerializeObject(new { survey.SurveyId, ipAddress, transactionId }),
            "manual",
            survey.ZampliaSurveyId,
            survey.SurveyId,
            proxyResponse: proxyResponse);

        if (!proxyResponse.Result || string.IsNullOrWhiteSpace(proxyResponse.ResponseBody) || !TryExtractVendorLink(proxyResponse.ResponseBody, out var vendorLink))
        {
            return new ZampliaSurveyLinkGenerationResult
            {
                Success = false,
                Message = string.IsNullOrWhiteSpace(proxyResponse.Message)
                    ? "Zamplia returned a response, but no survey link could be extracted."
                    : proxyResponse.Message,
                InternalLaunchUrl = existingEntryLink?.InternalLaunchUrl
            };
        }

        var saveEntryLinkResult = await _zampliaAPIController.SaveZampliaEntryLink(new ZampliaEntryLinkDTO
        {
            Id = existingEntryLink?.Id ?? 0,
            ZampliaSurveyId = survey.ZampliaSurveyId,
            SurveyId = survey.SurveyId,
            TransactionId = transactionId,
            VendorLink = vendorLink,
            InternalLaunchUrl = existingEntryLink?.InternalLaunchUrl,
            HashApplied = existingEntryLink?.HashApplied ?? false,
            IsActive = true,
            CreatedBy = existingEntryLink?.CreatedBy ?? userId,
            ModifiedBy = userId,
            RawJson = proxyResponse.ResponseBody
        });

        var saveSucceeded = saveEntryLinkResult is ObjectResult saveObject && (saveObject.StatusCode ?? StatusCodes.Status200OK) < 400;
        return new ZampliaSurveyLinkGenerationResult
        {
            Success = true,
            VendorLink = vendorLink,
            TransactionId = transactionId,
            IpAddress = ipAddress,
            InternalLaunchUrl = existingEntryLink?.InternalLaunchUrl,
            SaveSucceeded = saveSucceeded,
            Message = saveSucceeded
                ? "Survey link generated successfully."
                : "Survey link generated successfully, but it could not be saved locally."
        };
    }

    private async Task<ZampliaProxyResponse> CallConsultingsProxyAsync(string actionPath, ZampliaProxyRequest payload)
    {
        var baseUrl = _configuration.GetValue<string>("ConsultingApiBaseUrl");
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return new ZampliaProxyResponse { Result = false, IsStub = true, StatusCode = StatusCodes.Status500InternalServerError, Message = "Consultings proxy base URL is not configured." };
        }

        var url = $"{baseUrl.TrimEnd('/')}/VT/ZampliaSupplyProxy/{actionPath}";
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync(url, payload);
            var content = await response.Content.ReadAsStringAsync();
            var proxyResponse = string.IsNullOrWhiteSpace(content)
                ? new ZampliaProxyResponse()
                : JsonConvert.DeserializeObject<ZampliaProxyResponse>(content) ?? new ZampliaProxyResponse();

            proxyResponse.StatusCode ??= (int)response.StatusCode;
            proxyResponse.RequestUrl ??= url;
            proxyResponse.ResponseBody ??= content;
            if (!response.IsSuccessStatusCode && string.IsNullOrWhiteSpace(proxyResponse.Message))
            {
                proxyResponse.Message = "Consultings proxy request failed.";
            }

            return proxyResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Consultings Zamplia proxy call failed. ActionPath={ActionPath}", actionPath);
            return new ZampliaProxyResponse { Result = false, IsStub = true, StatusCode = StatusCodes.Status500InternalServerError, Message = ex.Message, RequestUrl = url };
        }
    }

    private async Task AppendLogAsync(string actionName, string? requestUrl, string? requestBody, string source, int? relatedEntityId, long? relatedSurveyId, IActionResult? apiResult = null, ZampliaProxyResponse? proxyResponse = null, string? responseBodyOverride = null, int? responseStatusCodeOverride = null, bool? forceSuccess = null)
    {
        var log = new ZampliaSyncLogDTO
        {
            ModuleName = "Zamplia",
            ActionName = actionName,
            RequestUrl = requestUrl,
            RequestBodySnapshot = requestBody,
            Source = source,
            RelatedEntityId = relatedEntityId,
            RelatedSurveyId = relatedSurveyId,
            StartedOn = DateTime.Now,
            CompletedOn = DateTime.Now,
            CreatedBy = GetCurrentUserId()
        };

        if (proxyResponse != null)
        {
            log.ResponseStatusCode = proxyResponse.StatusCode;
            log.ResponseBodySnapshot = responseBodyOverride ?? proxyResponse.ResponseBody ?? proxyResponse.Message;
            log.IsSuccess = forceSuccess ?? proxyResponse.Result;
            log.ErrorText = log.IsSuccess ? null : proxyResponse.Message;
        }
        else if (apiResult is ObjectResult objectResult)
        {
            log.ResponseStatusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;
            log.ResponseBodySnapshot = responseBodyOverride ?? JsonConvert.SerializeObject(objectResult.Value);
            log.IsSuccess = forceSuccess ?? ((objectResult.StatusCode ?? StatusCodes.Status200OK) < 400);
            log.ErrorText = log.IsSuccess ? null : JsonConvert.SerializeObject(objectResult.Value);
        }
        else
        {
            log.ResponseStatusCode = responseStatusCodeOverride ?? StatusCodes.Status200OK;
            log.ResponseBodySnapshot = responseBodyOverride;
            log.IsSuccess = forceSuccess ?? ((responseStatusCodeOverride ?? StatusCodes.Status200OK) < 400);
            log.ErrorText = log.IsSuccess ? null : responseBodyOverride;
        }

        await _zampliaAPIController.AddZampliaLog(log);
    }

    private async Task<ZampliaSettingDTO?> GetCurrentSettingAsync()
    {
        var result = await _zampliaAPIController.GetZampliaSetting();
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as ZampliaSettingDTO
            : null;
    }

    private async Task<ZampliaSurveyDTO?> GetSurveyAsync(int id)
    {
        var result = await _zampliaAPIController.GetZampliaSurvey(id);
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as ZampliaSurveyDTO
            : null;
    }

    private async Task<ZampliaEntryLinkDTO?> GetEntryLinkAsync(int id)
    {
        var result = await _zampliaAPIController.GetZampliaEntryLink(id);
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as ZampliaEntryLinkDTO
            : null;
    }

    private int? GetCurrentUserId() => HttpContext.Session.GetInt32("UserId");

    private IActionResult ConvertApiResult(IActionResult apiResult) => apiResult switch
    {
        JsonResult jsonResult => jsonResult,
        ObjectResult objectResult => new JsonResult(objectResult.Value) { StatusCode = objectResult.StatusCode },
        ContentResult contentResult => contentResult,
        StatusCodeResult statusCodeResult => StatusCode(statusCodeResult.StatusCode),
        IStatusCodeActionResult status when status.StatusCode.HasValue => StatusCode(status.StatusCode.Value),
        _ => StatusCode(StatusCodes.Status500InternalServerError)
    };

    private static VendorAddProjectResult? ExtractVendorAddProjectResult(IActionResult actionResult)
    {
        object? value = actionResult switch
        {
            ObjectResult objectResult => objectResult.Value,
            JsonResult jsonResult => jsonResult.Value,
            _ => null
        };

        return value == null ? null : JsonConvert.DeserializeObject<VendorAddProjectResult>(JsonConvert.SerializeObject(value));
    }

    private async Task<ZampliaRespondentAttemptDTO?> LoadAttemptFromCookieAsync()
    {
        if (int.TryParse(GetQueryValue("zampliaSurveyId", "ZampliaSurveyId"), out var surveyId) && surveyId > 0)
        {
            if (Request.Cookies.TryGetValue(GetAttemptCookieName(surveyId), out var attemptIdValue) &&
                int.TryParse(attemptIdValue, out var attemptId) &&
                attemptId > 0)
            {
                return await LoadAttemptByIdAsync(attemptId);
            }
        }

        var matchingCookies = Request.Cookies
            .Where(item => item.Key.StartsWith("zamplia_attempt_", StringComparison.OrdinalIgnoreCase))
            .Select(item => item.Value)
            .Where(value => int.TryParse(value, out var parsed) && parsed > 0)
            .Distinct()
            .ToList();

        if (matchingCookies.Count != 1 || !int.TryParse(matchingCookies[0], out var fallbackAttemptId) || fallbackAttemptId <= 0)
        {
            return null;
        }

        return await LoadAttemptByIdAsync(fallbackAttemptId);
    }

    private async Task<ZampliaRespondentAttemptDTO?> LoadAttemptByIdAsync(int attemptId)
    {
        var result = await _zampliaAPIController.GetZampliaRespondentAttempt(attemptId);
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as ZampliaRespondentAttemptDTO
            : null;
    }

    private static string GetAttemptCookieName(int zampliaSurveyId) => $"zamplia_attempt_{zampliaSurveyId}";
    private string BuildMaskingLaunchUrl(ZampliaLaunchContextDTO context, string respondentIdToken)

    {
        var respondentValue = respondentIdToken == "[respondentID]" ? respondentIdToken : Uri.EscapeDataString(respondentIdToken);
        var maskingUrl = _configuration.GetValue<string>("MaskingUrl");
        if (string.IsNullOrWhiteSpace(maskingUrl))
        {
            maskingUrl = $"{Request.Scheme}://{Request.Host}/VT/MaskingUrl.aspx";
        }

        var separator = maskingUrl.Contains('?') ? '&' : '?';
        return $"{maskingUrl}{separator}SID={Uri.EscapeDataString(context.ProjectMappingSid ?? string.Empty)}&ID={respondentValue}";
    }

    private static string BuildRuntimePlaceholderUrl(long surveyId)
    {
        return $"https://zamplia-runtime.invalid/launch?surveyId={surveyId.ToString(CultureInfo.InvariantCulture)}&uid=[respondentID]&token=[tokenID]";
    }

    private string? GetClientIpAddress() => HttpContext.Connection.RemoteIpAddress?.ToString();

    private string? GetQueryValue(params string[] keys) => GetQueryValue(false, keys);

    private string? GetQueryValue(bool preferLast, params string[] keys)
    {
        foreach (var key in keys)
        {
            var values = Request.Query[key];
            if (values.Count == 0)
            {
                continue;
            }

            var orderedValues = preferLast ? values.Reverse() : values.AsEnumerable();
            foreach (var value in orderedValues)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            var parts = values.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length > 0)
            {
                return preferLast ? parts[^1] : parts[0];
            }
        }

        return null;
    }

    private static string? FirstNonEmpty(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
        }

        return null;
    }

    private Dictionary<string, string?> BuildReconciliationQueryParameters(ZampliaReconciliationRunRequest? request)
    {
        var query = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        if (request?.SurveyId > 0) query["SurveyId"] = request.SurveyId.Value.ToString(CultureInfo.InvariantCulture);
        if (request?.InternalProjectId > 0) query["InternalProjectId"] = request.InternalProjectId.Value.ToString(CultureInfo.InvariantCulture);
        if (request?.TransactionId?.Length > 0) query["TransactionId"] = request.TransactionId;
        if (request?.DateFrom.HasValue == true) query["DateFrom"] = request.DateFrom.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        if (request?.DateTo.HasValue == true) query["DateTo"] = request.DateTo.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        return query;
    }

    private async Task<ReconciliationBuildResult> BuildReconciliationAsync(string rawJson, ZampliaReconciliationRunRequest? request, int? userId)
    {
        using var document = JsonDocument.Parse(rawJson);
        var vendorItems = ZampliaJsonHelper.ExtractArrayCandidates(document.RootElement, "items", "Items", "reconciliation", "Reconciliation");
        if (vendorItems.Count == 0 && document.RootElement.ValueKind == JsonValueKind.Object)
        {
            vendorItems.Add(document.RootElement.Clone());
        }

        var items = new List<ZampliaReconciliationItemDTO>();
        var reviewed = 0;
        foreach (var vendorItem in vendorItems)
        {
            reviewed++;
            var transactionId = ZampliaJsonHelper.GetJsonString(vendorItem, "TransactionId", "transactionId");
            var respondentId = ZampliaJsonHelper.GetJsonString(vendorItem, "uid", "UID", "RespondentId", "respondentId");
            var surveyId = ZampliaJsonHelper.GetJsonLong(vendorItem, "SurveyId", "surveyId");
            var vendorStatus = ZampliaReconciliationHelper.NormalizeReturnStatus(ZampliaJsonHelper.GetJsonString(vendorItem, "Status", "status", "FinalStatus", "finalStatus"));
            var attempt = await _zampliaAPIController.GetLatestZampliaRespondentAttemptAsync(transactionId, respondentId);
            var localStatus = ZampliaReconciliationHelper.NormalizeReturnStatus(attempt?.FinalStatus ?? attempt?.ReturnStatus);
            var finalStatus = vendorStatus == "Open" ? localStatus : vendorStatus;
            var isMismatch = attempt == null ||
                             (!string.IsNullOrWhiteSpace(localStatus) &&
                              !string.IsNullOrWhiteSpace(vendorStatus) &&
                              !string.Equals(localStatus, vendorStatus, StringComparison.OrdinalIgnoreCase));
            items.Add(new ZampliaReconciliationItemDTO
            {
                ZampliaRespondentAttemptId = attempt?.Id,
                SurveyId = surveyId ?? attempt?.SurveyId,
                InternalProjectId = attempt?.InternalProjectId,
                TransactionId = transactionId ?? attempt?.TransactionId,
                RespondentId = respondentId ?? attempt?.RespondentId,
                SessionId = attempt?.SessionId,
                LocalStatus = localStatus,
                VendorStatus = vendorStatus,
                FinalStatus = finalStatus,
                FinalStatusSource = vendorStatus != "Open" ? "VendorReconciliation" : "LocalAttempt",
                IsMismatch = isMismatch,
                MismatchType = attempt == null ? "MissingAttempt" : isMismatch ? "StatusMismatch" : "None",
                Notes = attempt == null ? "Local attempt not found." : null,
                RawSnapshotJson = vendorItem.GetRawText(),
                CreatedDate = DateTime.Now
            });
        }

        var run = new ZampliaReconciliationRunDTO
        {
            RunType = request?.RunType ?? "Manual",
            SurveyId = request?.SurveyId,
            InternalProjectId = request?.InternalProjectId,
            TransactionId = request?.TransactionId,
            RunScopeJson = JsonConvert.SerializeObject(request),
            StartedOn = DateTime.Now,
            CompletedOn = DateTime.Now,
            Success = true,
            Notes = $"Reviewed {reviewed} reconciliation item(s).",
            TotalReviewed = reviewed,
            TotalMatched = items.Count(item => !item.IsMismatch),
            TotalMismatched = items.Count(item => item.IsMismatch),
            CompleteCount = items.Count(item => item.FinalStatus == "Complete"),
            TerminateCount = items.Count(item => item.FinalStatus == "Terminate"),
            OverQuotaCount = items.Count(item => item.FinalStatus == "QuotaFull" || item.FinalStatus == "OverQuota"),
            QualityTerminationCount = items.Count(item => item.FinalStatus == "QualityTermination"),
            SecurityTerminationCount = items.Count(item => item.FinalStatus == "SecurityTermination"),
            OpenCount = items.Count(item => item.FinalStatus == "Open"),
            UnknownCount = items.Count(item => string.IsNullOrWhiteSpace(item.FinalStatus)),
            CreatedBy = userId,
            CreatedDate = DateTime.Now
        };

        return new ReconciliationBuildResult { Run = run, Items = items };
    }

    private static int? ParseNullableInt(string? value)
    {
        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) || parsed <= 0)
        {
            return null;
        }

        return parsed;
    }

    private string? ResolveLaunchRespondentId(string? respondentId)
    {
        var candidate = string.IsNullOrWhiteSpace(respondentId)
            ? GetQueryValue(true, "uid", "UID", "id", "ID", "rid", "RID")
            : respondentId.Trim();

        if (IsRespondentPlaceholder(candidate))
        {
            candidate = GetQueryValue(true, "uid", "UID", "id", "ID", "rid", "RID");
        }

        return IsRespondentPlaceholder(candidate) ? null : candidate?.Trim();
    }

    private static bool IsRespondentPlaceholder(string? respondentId)
    {
        if (string.IsNullOrWhiteSpace(respondentId))
        {
            return true;
        }

        var normalized = respondentId.Trim().Trim('[', ']').Replace(" ", string.Empty);
        return string.Equals(normalized, "respondentID", StringComparison.OrdinalIgnoreCase);
    }

    private string ResolveSurveyLinkIpAddress()
    {
        if (!string.IsNullOrWhiteSpace(Request.Host.Host) &&
            !string.Equals(Request.Host.Host, "localhost", StringComparison.OrdinalIgnoreCase))
        {
            return Request.Host.Host.Trim();
        }

        var clientIp = GetClientIpAddress();
        return string.IsNullOrWhiteSpace(clientIp) ? "127.0.0.1" : clientIp.Trim();
    }

    private static string BuildSurveyLinkTransactionId(long surveyId)
    {
        return $"preview-{surveyId.ToString(CultureInfo.InvariantCulture)}-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }

    private static bool TryExtractVendorLink(string rawJson, out string vendorLink)
    {
        vendorLink = string.Empty;
        try
        {
            using var document = JsonDocument.Parse(rawJson);
            return TryExtractVendorLinkFromElement(document.RootElement, out vendorLink);
        }
        catch
        {
            vendorLink = string.Empty;
            return false;
        }
    }

    private static bool TryExtractVendorLinkFromElement(JsonElement element, out string vendorLink)
    {
        vendorLink = string.Empty;
        if (TryGetAbsoluteUrlFromElement(element, out vendorLink))
        {
            return true;
        }

        foreach (var propertyName in new[] { "link", "Link", "url", "Url", "launchLink", "LaunchLink", "surveyLink", "SurveyLink", "generatedLink", "GeneratedLink", "liveLink", "LiveLink" })
        {
            if (element.ValueKind == JsonValueKind.Object &&
                ZampliaJsonHelper.TryGetJsonPropertyIgnoreCase(element, out var linkElement, propertyName) &&
                TryGetAbsoluteUrlFromElement(linkElement, out vendorLink))
            {
                return true;
            }
        }

        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (TryGetAbsoluteUrlFromElement(property.Value, out vendorLink))
                {
                    return true;
                }

                if ((property.Value.ValueKind == JsonValueKind.Object || property.Value.ValueKind == JsonValueKind.Array) &&
                    TryExtractVendorLinkFromElement(property.Value, out vendorLink))
                {
                    return true;
                }
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                if (TryExtractVendorLinkFromElement(item, out vendorLink))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool TryGetAbsoluteUrlFromElement(JsonElement element, out string vendorLink)
    {
        vendorLink = string.Empty;
        if (element.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        var candidate = element.GetString()?.Trim();
        if (string.IsNullOrWhiteSpace(candidate) || !Uri.TryCreate(candidate, UriKind.Absolute, out var uri))
        {
            return false;
        }

        if (!string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        vendorLink = candidate;
        return true;
    }

    private sealed class ReconciliationBuildResult
    {
        public ZampliaReconciliationRunDTO Run { get; set; } = new();
        public List<ZampliaReconciliationItemDTO> Items { get; set; } = new();
    }

    private sealed class ZampliaSurveyLinkGenerationResult
    {
        public bool Success { get; set; }
        public bool SaveSucceeded { get; set; }
        public string? Message { get; set; }
        public string? VendorLink { get; set; }
        public string? TransactionId { get; set; }
        public string? IpAddress { get; set; }
        public string? InternalLaunchUrl { get; set; }
    }
}






