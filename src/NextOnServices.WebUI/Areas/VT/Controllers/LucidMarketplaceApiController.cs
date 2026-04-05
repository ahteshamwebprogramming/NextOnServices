using System.Net.Http.Json;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.APIProjects;
using NextOnServices.WebUI.VT.Services;

namespace NextOnServices.WebUI.VT.Controllers;

[Area("VT")]
[Authorize]
[Route("VT/[controller]")]
public class LucidMarketplaceApiController : Controller
{
    private readonly ILogger<LucidMarketplaceApiController> _logger;
    private readonly LucidMarketplaceAPIController _lucidMarketplaceAPIController;
    private readonly SurveyAPIController _surveyAPIController;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILegacyProjectStatusService _legacyProjectStatusService;
    private readonly IConfiguration _configuration;

    public LucidMarketplaceApiController(
        ILogger<LucidMarketplaceApiController> logger,
        LucidMarketplaceAPIController lucidMarketplaceAPIController,
        SurveyAPIController surveyAPIController,
        IHttpClientFactory httpClientFactory,
        ILegacyProjectStatusService legacyProjectStatusService,
        IConfiguration configuration)
    {
        _logger = logger;
        _lucidMarketplaceAPIController = lucidMarketplaceAPIController;
        _surveyAPIController = surveyAPIController;
        _httpClientFactory = httpClientFactory;
        _legacyProjectStatusService = legacyProjectStatusService;
        _configuration = configuration;
    }

    [HttpPost("SaveSettings")]
    public async Task<IActionResult> SaveSettings([FromBody] LucidMarketplaceSettingDTO inputData)
    {
        if (inputData == null)
        {
            return BadRequest(new { result = false, message = "Settings payload is required." });
        }

        var userId = GetCurrentUserId();
        inputData.CreatedBy ??= userId;
        inputData.ModifiedBy = userId;

        var apiResult = await _lucidMarketplaceAPIController.SaveLucidMarketplaceSetting(inputData);
        await AppendLogAsync(
            actionName: "SaveSettings",
            requestUrl: "/VT/LucidMarketplaceApi/SaveSettings",
            requestBody: JsonConvert.SerializeObject(inputData),
            source: "manual",
            supplierCode: inputData.SupplierCode,
            apiResult: apiResult);

        return ConvertApiResult(apiResult);
    }

    [HttpPost("TestConnectivity")]
    public async Task<IActionResult> TestConnectivity([FromBody] LucidMarketplaceConnectivityRequest inputData)
    {
        if (inputData == null)
        {
            return BadRequest(new { result = false, message = "Connectivity payload is required." });
        }

        var setting = new LucidMarketplaceSettingDTO
        {
            BaseUrl = inputData.BaseUrl,
            ApiKey = inputData.ApiKey,
            SupplierCode = inputData.SupplierCode,
            UseConsultingsBridge = inputData.UseConsultingsBridge
        };

        var proxyResponse = inputData.UseConsultingsBridge
            ? await CallConsultingsProxyAsync("TestConnectivity", new LucidMarketplaceProxyRequest { Setting = setting })
            : new LucidMarketplaceProxyResponse
            {
                Result = false,
                IsStub = true,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Enable the Consultings bridge to validate Lucid Marketplace connectivity."
            };

        await AppendLogAsync(
            actionName: "TestConnectivity",
            requestUrl: proxyResponse.RequestUrl ?? "/VT/LucidMarketplaceApi/TestConnectivity",
            requestBody: JsonConvert.SerializeObject(setting),
            source: "manual",
            supplierCode: setting.SupplierCode,
            proxyResponse: proxyResponse);

        return Json(new
        {
            result = proxyResponse.Result,
            isStub = proxyResponse.IsStub,
            statusCode = proxyResponse.StatusCode,
            message = proxyResponse.Message
        });
    }

    [HttpPost("CreateSubscription")]
    public async Task<IActionResult> CreateSubscription([FromBody] LucidMarketplaceSubscriptionActionRequest inputData)
    {
        if (inputData == null || string.IsNullOrWhiteSpace(inputData.SubscriptionType))
        {
            return BadRequest(new { result = false, message = "Subscription type is required." });
        }

        var normalizedSubscriptionType = NormalizeSubscriptionType(inputData.SubscriptionType);
        if (string.IsNullOrWhiteSpace(normalizedSubscriptionType))
        {
            return BadRequest(new { result = false, message = "Unsupported Lucid Marketplace subscription type." });
        }

        var currentSetting = await GetCurrentSettingAsync();
        if (currentSetting == null || string.IsNullOrWhiteSpace(currentSetting.SupplierCode))
        {
            return BadRequest(new { result = false, message = "Save Lucid Marketplace settings before creating subscriptions." });
        }

        var supplierCode = string.IsNullOrWhiteSpace(inputData.SupplierCode)
            ? currentSetting.SupplierCode?.Trim()
            : inputData.SupplierCode.Trim();
        LucidMarketplaceSubscriptionDTO subscription;

        try
        {
            subscription = normalizedSubscriptionType == "RespondentOutcomes"
                ? BuildOutcomeSubscription(currentSetting, inputData, supplierCode, GetCurrentUserId())
                : BuildOpportunitySubscription(currentSetting, inputData, supplierCode, GetCurrentUserId());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { result = false, message = ex.Message });
        }
        catch (Exception ex) when (ex is System.Text.Json.JsonException || ex is Newtonsoft.Json.JsonException)
        {
            return BadRequest(new { result = false, message = ex.Message });
        }

        var proxyResponse = currentSetting.UseConsultingsBridge
            ? await CallConsultingsProxyAsync("CreateSubscription", new LucidMarketplaceProxyRequest
            {
                Setting = currentSetting,
                Subscription = subscription
            })
            : new LucidMarketplaceProxyResponse
            {
                Result = false,
                IsStub = true,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Direct Lucid Marketplace calls are not enabled for this module. Use the Consultings bridge."
            };

        subscription.ResponsePayloadSnapshot = proxyResponse.ResponseBody;
        subscription.LastStatus = proxyResponse.Message;
        subscription.IsActive = proxyResponse.Result && (proxyResponse.StatusCode ?? 0) < 300;
        subscription.LastValidatedOn = DateTime.Now;

        var saveResult = await _lucidMarketplaceAPIController.SaveLucidMarketplaceSubscription(subscription);

        await AppendLogAsync(
            actionName: $"CreateSubscription:{normalizedSubscriptionType}",
            requestUrl: proxyResponse.RequestUrl ?? "/VT/LucidMarketplaceApi/CreateSubscription",
            requestBody: subscription.RequestPayloadSnapshot,
            source: "manual",
            supplierCode: subscription.SupplierCode,
            proxyResponse: proxyResponse,
            apiResult: saveResult);

        if (saveResult is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
        {
            var saved = objectResult.Value as LucidMarketplaceSubscriptionDTO;
            return Json(new
            {
                result = true,
                remoteResult = proxyResponse.Result,
                isStub = proxyResponse.IsStub,
                message = proxyResponse.Message ?? "Subscription request recorded.",
                subscriptionId = saved?.LucidMarketplaceSubscriptionId ?? 0
            });
        }

        return ConvertApiResult(saveResult);
    }

    [HttpPost("RefreshSubscriptions")]
    public async Task<IActionResult> RefreshSubscriptions()
    {
        var currentSetting = await GetCurrentSettingAsync();
        var existingSubscriptions = await GetSubscriptionsAsync();
        var refreshMessages = new List<string>();

        if (currentSetting != null && currentSetting.UseConsultingsBridge)
        {
            foreach (var subscriptionType in new[] { "Opportunities", "RespondentOutcomes" })
            {
                var proxyResponse = await CallConsultingsProxyAsync("GetSubscriptions", new LucidMarketplaceProxyRequest
                {
                    Setting = currentSetting,
                    Subscription = new LucidMarketplaceSubscriptionDTO
                    {
                        SubscriptionType = subscriptionType,
                        SupplierCode = currentSetting.SupplierCode
                    }
                });

                var existingSubscription = existingSubscriptions
                    .Where(subscription =>
                        string.Equals(NormalizeSubscriptionType(subscription.SubscriptionType), subscriptionType, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(subscription.SupplierCode, currentSetting.SupplierCode, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(subscription => subscription.IsActive)
                    .ThenByDescending(subscription => subscription.LastValidatedOn ?? subscription.ModifiedDate ?? subscription.CreatedDate)
                    .FirstOrDefault();

                var syncedSubscription = TryBuildSubscriptionFromRemoteResponse(currentSetting, subscriptionType, proxyResponse, existingSubscription);
                if (syncedSubscription != null)
                {
                    syncedSubscription.CreatedBy = GetCurrentUserId();
                    syncedSubscription.ModifiedBy = GetCurrentUserId();
                    await _lucidMarketplaceAPIController.SaveLucidMarketplaceSubscription(syncedSubscription);
                }

                refreshMessages.Add(proxyResponse.Message ?? $"{subscriptionType} subscription refreshed.");

                await AppendLogAsync(
                    actionName: $"RefreshSubscriptions:{subscriptionType}",
                    requestUrl: proxyResponse.RequestUrl ?? "/VT/LucidMarketplaceApi/RefreshSubscriptions",
                    requestBody: JsonConvert.SerializeObject(new
                    {
                        currentSetting.SupplierCode,
                        subscriptionType
                    }),
                    source: "manual",
                    supplierCode: currentSetting.SupplierCode,
                    proxyResponse: proxyResponse);
            }
        }

        var apiResult = await _lucidMarketplaceAPIController.GetLucidMarketplaceSubscriptions();
        if (apiResult is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
        {
            var data = objectResult.Value as List<LucidMarketplaceSubscriptionDTO> ?? new List<LucidMarketplaceSubscriptionDTO>();
            return Json(new
            {
                result = true,
                message = refreshMessages.Count > 0
                    ? string.Join(" ", refreshMessages.Where(message => !string.IsNullOrWhiteSpace(message)))
                    : "Local subscriptions loaded successfully.",
                data
            });
        }

        return ConvertApiResult(apiResult);
    }

    [HttpPost("DeactivateSubscription")]
    public async Task<IActionResult> DeactivateSubscription([FromBody] LucidMarketplaceDeactivateSubscriptionRequest inputData)
    {
        if (inputData == null || inputData.LucidMarketplaceSubscriptionId <= 0)
        {
            return BadRequest(new { result = false, message = "A valid subscription id is required." });
        }

        var subscriptions = await GetSubscriptionsAsync();
        var currentSubscription = subscriptions.FirstOrDefault(x => x.LucidMarketplaceSubscriptionId == inputData.LucidMarketplaceSubscriptionId);
        var currentSetting = await GetCurrentSettingAsync();

        LucidMarketplaceProxyResponse? proxyResponse = null;
        if (currentSubscription != null &&
            currentSetting != null &&
            currentSetting.UseConsultingsBridge &&
            !string.IsNullOrWhiteSpace(NormalizeSubscriptionType(currentSubscription.SubscriptionType)))
        {
            proxyResponse = await CallConsultingsProxyAsync("DeleteSubscription", new LucidMarketplaceProxyRequest
            {
                Setting = currentSetting,
                Subscription = currentSubscription
            });
        }

        var apiResult = await _lucidMarketplaceAPIController.DeactivateLucidMarketplaceSubscription(inputData.LucidMarketplaceSubscriptionId, GetCurrentUserId());

        await AppendLogAsync(
            actionName: $"DeactivateSubscription:{NormalizeSubscriptionType(currentSubscription?.SubscriptionType) ?? "Unknown"}",
            requestUrl: proxyResponse?.RequestUrl ?? "/VT/LucidMarketplaceApi/DeactivateSubscription",
            requestBody: JsonConvert.SerializeObject(new
            {
                inputData.LucidMarketplaceSubscriptionId,
                currentSubscription?.RemoteSubscriptionId
            }),
            source: "manual",
            supplierCode: currentSubscription?.SupplierCode,
            proxyResponse: proxyResponse,
            apiResult: apiResult);

        return ConvertApiResult(apiResult);
    }

    [HttpPost("UpdateOpportunityState")]
    public async Task<IActionResult> UpdateOpportunityState([FromBody] LucidMarketplaceOpportunityStateRequest inputData)
    {
        var apiResult = await _lucidMarketplaceAPIController.UpdateLucidMarketplaceOpportunityState(inputData, GetCurrentUserId());

        await AppendLogAsync(
            actionName: "UpdateOpportunityState",
            requestUrl: "/VT/LucidMarketplaceApi/UpdateOpportunityState",
            requestBody: JsonConvert.SerializeObject(inputData),
            source: "manual",
            apiResult: apiResult);

        return ConvertApiResult(apiResult);
    }

    [HttpPost("RunReconciliation")]
    public async Task<IActionResult> RunReconciliation([FromBody] LucidMarketplaceReconciliationRunRequest? inputData)
    {
        var apiResult = await _lucidMarketplaceAPIController.RunLucidMarketplaceReconciliation(inputData, GetCurrentUserId());
        if (apiResult is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
        {
            var result = objectResult.Value as LucidMarketplaceReconciliationRunResult ?? new LucidMarketplaceReconciliationRunResult();
            return Json(new
            {
                result = result.Success,
                runId = result.ReconciliationRunId,
                totalReviewed = result.TotalReviewed,
                totalMismatched = result.TotalMismatched,
                message = result.Message ?? "Lucid Marketplace reconciliation completed."
            });
        }

        return ConvertApiResult(apiResult);
    }

    [HttpPost("AddProjectFromLucidMarketplace")]
    public async Task<IActionResult> AddProjectFromLucidMarketplace([FromBody] LucidMarketplaceAddProjectRequest inputData)
    {
        if (inputData == null || inputData.LucidMarketplaceOpportunityId <= 0)
        {
            return BadRequest(new { result = false, message = "A valid Lucid Marketplace opportunity is required." });
        }

        var userId = GetCurrentUserId();
        if (!userId.HasValue || userId.Value <= 0)
        {
            return Json(new { result = false, message = "Session expired. Please login again." });
        }

        var opportunity = await GetOpportunityAsync(inputData.LucidMarketplaceOpportunityId);
        if (opportunity == null || opportunity.LucidMarketplaceOpportunityId <= 0)
        {
            await AppendLogAsync(
                actionName: "AddProject",
                requestUrl: "/VT/LucidMarketplaceApi/AddProjectFromLucidMarketplace",
                requestBody: JsonConvert.SerializeObject(inputData),
                source: "manual",
                relatedEntityId: inputData.LucidMarketplaceOpportunityId,
                responseBodyOverride: "Invalid Opportunity",
                responseStatusCodeOverride: StatusCodes.Status400BadRequest,
                forceSuccess: false);

            return Json(new { result = false, message = "Invalid Opportunity" });
        }

        if (opportunity.IsMapped)
        {
            await AppendLogAsync(
                actionName: "AddProject",
                requestUrl: "/VT/LucidMarketplaceApi/AddProjectFromLucidMarketplace",
                requestBody: JsonConvert.SerializeObject(inputData),
                source: "manual",
                supplierCode: opportunity.SupplierCode,
                relatedEntityId: opportunity.LucidMarketplaceOpportunityId,
                relatedSurveyId: opportunity.SurveyId,
                responseBodyOverride: "Already Added",
                responseStatusCodeOverride: StatusCodes.Status200OK,
                forceSuccess: false);

            return Json(new { result = false, message = "Already Added" });
        }

        if (opportunity.SurveyId <= 0)
        {
            await AppendLogAsync(
                actionName: "AddProject",
                requestUrl: "/VT/LucidMarketplaceApi/AddProjectFromLucidMarketplace",
                requestBody: JsonConvert.SerializeObject(inputData),
                source: "manual",
                supplierCode: opportunity.SupplierCode,
                relatedEntityId: opportunity.LucidMarketplaceOpportunityId,
                responseBodyOverride: "Missing Required Data",
                responseStatusCodeOverride: StatusCodes.Status400BadRequest,
                forceSuccess: false);

            return Json(new { result = false, message = "Missing Required Data" });
        }

        var currentSetting = await GetCurrentSettingAsync();
        var existingEntryLink = await GetEntryLinkAsync(opportunity.LucidMarketplaceOpportunityId);
        var reusableProjectLevelLink = string.IsNullOrWhiteSpace(existingEntryLink?.LiveLink)
            ? existingEntryLink?.TestLink
            : existingEntryLink.LiveLink;
        var addRequest = new AddProjectFromLucidRequest
        {
            ProjectId = opportunity.SurveyId.ToString(CultureInfo.InvariantCulture),
            ProjectName = string.IsNullOrWhiteSpace(opportunity.SurveyName)
                ? (opportunity.SurveyNumber ?? opportunity.SurveyId.ToString(CultureInfo.InvariantCulture))
                : opportunity.SurveyName.Trim(),
            CPI = opportunity.RevenuePerInterview?.ToString("0.####", CultureInfo.InvariantCulture),
            LOI = ConvertToWholeNumberString(opportunity.BidLengthOfInterview),
            IR = ConvertToWholeNumberString(opportunity.BidIncidence),
            TotalRemaining = opportunity.TotalRemaining?.ToString(CultureInfo.InvariantCulture),
            LiveLink = reusableProjectLevelLink ?? string.Empty,
            ClientId = currentSetting?.DefaultClientId,
            CountryId = currentSetting?.DefaultCountryId,
            SupplierId = currentSetting?.DefaultSupplierId
        };

        var apiProjectsController = ActivatorUtilities.CreateInstance<ApiProjectsController>(HttpContext.RequestServices);
        apiProjectsController.ControllerContext = ControllerContext;

        var addProjectAction = await apiProjectsController.AddProjectFromLucidMarketplace(addRequest);
        var addProjectResult = ExtractVendorAddProjectResult(addProjectAction);
        var addProjectStatusCode = GetStatusCode(addProjectAction);

        await AppendLogAsync(
            actionName: "AddProjectCreate",
            requestUrl: "/VT/LucidMarketplaceApi/AddProjectFromLucidMarketplace",
            requestBody: JsonConvert.SerializeObject(new
            {
                inputData.LucidMarketplaceOpportunityId,
                addRequest.ProjectId,
                addRequest.ProjectName,
                addRequest.CPI,
                addRequest.LOI,
                addRequest.IR,
                addRequest.TotalRemaining,
                addRequest.ClientId,
                addRequest.CountryId,
                addRequest.SupplierId
            }),
            source: "manual",
            supplierCode: opportunity.SupplierCode,
            relatedEntityId: opportunity.LucidMarketplaceOpportunityId,
            relatedSurveyId: opportunity.SurveyId,
            responseBodyOverride: JsonConvert.SerializeObject(addProjectResult ?? new VendorAddProjectResult
            {
                Result = false,
                Message = "Project Creation Failed"
            }),
            responseStatusCodeOverride: addProjectStatusCode,
            forceSuccess: addProjectResult?.Result);

        if (addProjectResult == null)
        {
            await AppendLogAsync(
                actionName: "AddProject",
                requestUrl: "/VT/LucidMarketplaceApi/AddProjectFromLucidMarketplace",
                requestBody: JsonConvert.SerializeObject(inputData),
                source: "manual",
                supplierCode: opportunity.SupplierCode,
                relatedEntityId: opportunity.LucidMarketplaceOpportunityId,
                relatedSurveyId: opportunity.SurveyId,
                responseBodyOverride: "Project Creation Failed",
                responseStatusCodeOverride: StatusCodes.Status500InternalServerError,
                forceSuccess: false);

            return Json(new { result = false, message = "Project Creation Failed" });
        }

        if (!addProjectResult.Result)
        {
            await AppendLogAsync(
                actionName: "AddProject",
                requestUrl: "/VT/LucidMarketplaceApi/AddProjectFromLucidMarketplace",
                requestBody: JsonConvert.SerializeObject(inputData),
                source: "manual",
                supplierCode: opportunity.SupplierCode,
                relatedEntityId: opportunity.LucidMarketplaceOpportunityId,
                relatedSurveyId: opportunity.SurveyId,
                responseBodyOverride: addProjectResult.Message ?? "Project Creation Failed",
                responseStatusCodeOverride: addProjectStatusCode,
                forceSuccess: false);

            return Json(new { result = false, message = addProjectResult.Message ?? "Project Creation Failed" });
        }

        var internalLaunchUrl = BuildInternalLaunchUrl(
            opportunity.LucidMarketplaceOpportunityId,
            addProjectResult.ProjectId,
            addProjectResult.ProjectUrlId,
            addProjectResult.ProjectMappingId,
            "Live",
            "[respondentID]");

        var syncedCoreLaunchUrl = await _lucidMarketplaceAPIController.SyncLucidMarketplaceProjectLaunchUrlByIdsAsync(
            opportunity.LucidMarketplaceOpportunityId,
            addProjectResult.ProjectId,
            addProjectResult.ProjectUrlId,
            addProjectResult.ProjectMappingId,
            addRequest.CountryId,
            internalLaunchUrl);

        if (!syncedCoreLaunchUrl)
        {
            await AppendLogAsync(
                actionName: "AddProjectCoreLinkSync",
                requestUrl: "/VT/LucidMarketplaceApi/AddProjectFromLucidMarketplace",
                requestBody: JsonConvert.SerializeObject(new
                {
                    inputData.LucidMarketplaceOpportunityId,
                    addProjectResult.ProjectId,
                    addProjectResult.ProjectUrlId,
                    addProjectResult.ProjectMappingId,
                    internalLaunchUrl
                }),
                source: "manual",
                supplierCode: opportunity.SupplierCode,
                relatedEntityId: opportunity.LucidMarketplaceOpportunityId,
                relatedSurveyId: opportunity.SurveyId,
                responseBodyOverride: "Project created but core launch URL sync failed.",
                responseStatusCodeOverride: StatusCodes.Status500InternalServerError,
                forceSuccess: false);

            return Json(new { result = false, message = "Project created but the Lucid launch URL could not be stored in the normal project tables." });
        }

        var mapResult = await _lucidMarketplaceAPIController.SaveLucidMarketplaceProjectMap(new LucidMarketplaceProjectMapDTO
        {
            LucidMarketplaceOpportunityId = opportunity.LucidMarketplaceOpportunityId,
            LucidSurveyId = opportunity.SurveyId,
            InternalProjectId = addProjectResult.ProjectId,
            InternalProjectUrlId = addProjectResult.ProjectUrlId,
            InternalProjectMappingId = addProjectResult.ProjectMappingId,
            SupplierCode = opportunity.SupplierCode,
            AddedBy = userId,
            AddedOn = DateTime.Now,
            IsActive = true,
            RawJson = null
        });

        var supportMapSaved = mapResult is ObjectResult mapObjectResult && mapObjectResult.StatusCode == StatusCodes.Status200OK;
        if (!supportMapSaved)
        {
            await AppendLogAsync(
                actionName: "AddProjectSupportMap",
                requestUrl: "/VT/LucidMarketplaceApi/AddProjectFromLucidMarketplace",
                requestBody: JsonConvert.SerializeObject(inputData),
                source: "manual",
                supplierCode: opportunity.SupplierCode,
                relatedEntityId: opportunity.LucidMarketplaceOpportunityId,
                relatedSurveyId: opportunity.SurveyId,
                responseBodyOverride: "Project created in the core tables, but the Lucid support mapping row could not be saved.",
                responseStatusCodeOverride: StatusCodes.Status500InternalServerError,
                forceSuccess: false);
        }

        await _lucidMarketplaceAPIController.UpdateLucidMarketplaceOpportunityState(
            new LucidMarketplaceOpportunityStateRequest
            {
                LucidMarketplaceOpportunityId = opportunity.LucidMarketplaceOpportunityId,
                IsActive = true,
                LocalState = "Mapped"
            },
            userId);

        await AppendLogAsync(
            actionName: "AddProject",
            requestUrl: "/VT/LucidMarketplaceApi/AddProjectFromLucidMarketplace",
            requestBody: JsonConvert.SerializeObject(inputData),
            source: "manual",
            supplierCode: opportunity.SupplierCode,
            relatedEntityId: opportunity.LucidMarketplaceOpportunityId,
            relatedSurveyId: opportunity.SurveyId,
            responseBodyOverride: JsonConvert.SerializeObject(new
            {
                addProjectResult.ProjectId,
                addProjectResult.ProjectUrlId,
                addProjectResult.ProjectMappingId,
                state = "Mapped",
                supportMapSaved,
                internalLaunchUrl
            }),
            responseStatusCodeOverride: StatusCodes.Status200OK,
            forceSuccess: true);

        return Json(new
        {
            result = true,
            message = supportMapSaved
                ? addProjectResult.Message ?? "Project added successfully."
                : "Project added successfully. Lucid support mapping could not be saved, but the core project tables were updated.",
            projectId = addProjectResult.ProjectId,
            projectUrlId = addProjectResult.ProjectUrlId,
            projectMappingId = addProjectResult.ProjectMappingId,
            supportMapSaved
        });
    }

    [AllowAnonymous]
    [HttpPost("OpportunitiesCallback")]
    [HttpPost("OpportunitiesCallback/{supplierCode?}")]
    public async Task<IActionResult> OpportunitiesCallback(string? supplierCode = null)
    {
        var requestBody = await ReadRequestBodyAsync();
        var signatureHeader = Request.Headers["X-Lucid-Signature"].ToString();

        await AppendLogAsync(
            actionName: "CallbackReceived",
            requestUrl: $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
            requestBody: requestBody,
            source: "callback",
            supplierCode: supplierCode,
            responseBodyOverride: string.IsNullOrWhiteSpace(signatureHeader) ? null : $"X-Lucid-Signature: {signatureHeader}",
            responseStatusCodeOverride: StatusCodes.Status200OK,
            forceSuccess: true);

        if (string.IsNullOrWhiteSpace(requestBody))
        {
            await AppendLogAsync(
                actionName: "CallbackRejected",
                requestUrl: $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
                requestBody: requestBody,
                source: "callback",
                supplierCode: supplierCode,
                responseBodyOverride: "Request body is required.",
                responseStatusCodeOverride: StatusCodes.Status400BadRequest,
                forceSuccess: false);
            return BadRequest("Request body is required.");
        }

        try
        {
            var validation = await ValidateCallbackSignatureAsync("Opportunities", supplierCode, requestBody);
            if (!validation.IsValid)
            {
                await AppendLogAsync(
                    actionName: "CallbackRejected",
                    requestUrl: $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
                    requestBody: requestBody,
                    source: "callback",
                    supplierCode: validation.EffectiveSupplierCode ?? supplierCode,
                    responseBodyOverride: BuildCallbackValidationLogPayload(validation, signatureHeader, "Rejected"),
                    responseStatusCodeOverride: validation.StatusCode,
                    forceSuccess: false);

                return StatusCode(validation.StatusCode, validation.Message);
            }

            await AppendLogAsync(
                actionName: "CallbackValidated",
                requestUrl: $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
                requestBody: requestBody,
                source: "callback",
                supplierCode: validation.EffectiveSupplierCode ?? supplierCode,
                responseBodyOverride: BuildCallbackValidationLogPayload(validation, signatureHeader, "Accepted"),
                responseStatusCodeOverride: StatusCodes.Status200OK,
                forceSuccess: true);

            var processResult = await _lucidMarketplaceAPIController.ProcessOpportunitiesPayloadAsync(
                requestBody,
                "callback",
                validation.EffectiveSupplierCode,
                userId: null);

            await AppendLogAsync(
                actionName: "CallbackProcessed",
                requestUrl: $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
                requestBody: requestBody,
                source: "callback",
                supplierCode: validation.EffectiveSupplierCode,
                relatedSurveyId: processResult.SurveyIds.LastOrDefault(),
                responseBodyOverride: JsonConvert.SerializeObject(processResult),
                responseStatusCodeOverride: StatusCodes.Status200OK,
                forceSuccess: true);

            return Content("OK", "text/plain");
        }
        catch (System.Text.Json.JsonException ex)
        {
            _logger.LogWarning(ex, "Lucid Marketplace callback payload could not be parsed.");
            return BadRequest("Invalid JSON payload.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lucid Marketplace callback processing failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, "Unable to process callback.");
        }
    }

    [AllowAnonymous]
    [HttpPost("OutcomesCallback")]
    [HttpPost("OutcomesCallback/{supplierCode?}")]
    public async Task<IActionResult> OutcomesCallback(string? supplierCode = null)
    {
        var requestBody = await ReadRequestBodyAsync();
        var signatureHeader = Request.Headers["X-Lucid-Signature"].ToString();

        await AppendLogAsync(
            actionName: "OutcomesCallbackReceived",
            requestUrl: $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
            requestBody: requestBody,
            source: "callback",
            supplierCode: supplierCode,
            responseBodyOverride: string.IsNullOrWhiteSpace(signatureHeader) ? null : $"X-Lucid-Signature: {signatureHeader}",
            responseStatusCodeOverride: StatusCodes.Status200OK,
            forceSuccess: true);

        if (string.IsNullOrWhiteSpace(requestBody))
        {
            await AppendLogAsync(
                actionName: "OutcomesCallbackRejected",
                requestUrl: $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
                requestBody: requestBody,
                source: "callback",
                supplierCode: supplierCode,
                responseBodyOverride: "Request body is required.",
                responseStatusCodeOverride: StatusCodes.Status400BadRequest,
                forceSuccess: false);
            return BadRequest("Request body is required.");
        }

        try
        {
            var validation = await ValidateCallbackSignatureAsync("RespondentOutcomes", supplierCode, requestBody);
            if (!validation.IsValid)
            {
                await AppendLogAsync(
                    actionName: "OutcomesCallbackRejected",
                    requestUrl: $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
                    requestBody: requestBody,
                    source: "callback",
                    supplierCode: validation.EffectiveSupplierCode ?? supplierCode,
                    responseBodyOverride: BuildCallbackValidationLogPayload(validation, signatureHeader, "Rejected"),
                    responseStatusCodeOverride: validation.StatusCode,
                    forceSuccess: false);

                return StatusCode(validation.StatusCode, validation.Message);
            }

            await AppendLogAsync(
                actionName: "OutcomesCallbackValidated",
                requestUrl: $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
                requestBody: requestBody,
                source: "callback",
                supplierCode: validation.EffectiveSupplierCode ?? supplierCode,
                responseBodyOverride: BuildCallbackValidationLogPayload(validation, signatureHeader, "Accepted"),
                responseStatusCodeOverride: StatusCodes.Status200OK,
                forceSuccess: true);

            var processResult = await _lucidMarketplaceAPIController.ProcessRespondentOutcomesPayloadAsync(
                requestBody,
                "callback",
                validation.EffectiveSupplierCode,
                userId: null);

            foreach (var candidate in processResult.LegacyCompletionCandidates)
            {
                var applied = await TryApplyLegacyCompletionFromOutcomeAsync(candidate);
                if (applied)
                {
                    processResult.LegacyCompletionAppliedCount++;
                }
                else
                {
                    processResult.LegacyCompletionSkippedCount++;
                }
            }

            await AppendLogAsync(
                actionName: "OutcomesCallbackProcessed",
                requestUrl: $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
                requestBody: requestBody,
                source: "callback",
                supplierCode: validation.EffectiveSupplierCode,
                relatedSurveyId: processResult.SurveyIds.LastOrDefault(),
                responseBodyOverride: JsonConvert.SerializeObject(new
                {
                    processResult.ProcessedCount,
                    processResult.SkippedCount,
                    processResult.MatchedAttemptCount,
                    processResult.UpdatedAttemptCount,
                    processResult.LegacyCompletionAppliedCount,
                    processResult.LegacyCompletionSkippedCount,
                    processResult.OutcomeIds,
                    processResult.AttemptIds,
                    processResult.SurveyIds,
                    processResult.Errors
                }),
                responseStatusCodeOverride: StatusCodes.Status200OK,
                forceSuccess: true);

            return Content("OK", "text/plain");
        }
        catch (System.Text.Json.JsonException ex)
        {
            _logger.LogWarning(ex, "Lucid Marketplace outcomes callback payload could not be parsed.");
            return BadRequest("Invalid JSON payload.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lucid Marketplace outcomes callback processing failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, "Unable to process callback.");
        }
    }

    [HttpPost("GenerateEntryLinkFromOpportunity")]
    public async Task<IActionResult> GenerateEntryLinkFromOpportunity([FromBody] LucidMarketplaceGenerateEntryLinkRequest inputData)
    {
        if (inputData == null || !inputData.LucidMarketplaceOpportunityId.HasValue || inputData.LucidMarketplaceOpportunityId.Value <= 0)
        {
            return BadRequest(new { result = false, message = "A valid Lucid Marketplace opportunity is required." });
        }

        var resolution = await EnsureEntryLinkAsync(inputData, "manual");
        if (!resolution.Success || resolution.EntryLink == null)
        {
            return Json(new
            {
                result = false,
                message = resolution.Message ?? "Unable to generate the Lucid Marketplace entry link."
            });
        }

        return Json(new
        {
            result = true,
            message = resolution.Message ?? "Lucid Marketplace entry link generated successfully.",
            entryLinkId = resolution.EntryLink.Id,
            liveLink = resolution.EntryLink.LiveLink,
            testLink = resolution.EntryLink.TestLink,
            internalLaunchUrl = resolution.InternalLaunchUrl,
            usedFallback = resolution.UsedFallback
        });
    }

    [HttpPost("GenerateEntryLinkFromProject")]
    public async Task<IActionResult> GenerateEntryLinkFromProject([FromBody] LucidMarketplaceGenerateEntryLinkRequest inputData)
    {
        if (inputData == null ||
            (!inputData.InternalProjectId.HasValue || inputData.InternalProjectId.Value <= 0) &&
            (!inputData.InternalProjectMappingId.HasValue || inputData.InternalProjectMappingId.Value <= 0))
        {
            return BadRequest(new { result = false, message = "A valid internal Lucid Marketplace project or project mapping is required." });
        }

        var resolution = await EnsureEntryLinkAsync(inputData, "manual");
        if (!resolution.Success || resolution.EntryLink == null)
        {
            return Json(new
            {
                result = false,
                message = resolution.Message ?? "Unable to generate the Lucid Marketplace entry link."
            });
        }

        return Json(new
        {
            result = true,
            message = resolution.Message ?? "Lucid Marketplace entry link generated successfully.",
            entryLinkId = resolution.EntryLink.Id,
            liveLink = resolution.EntryLink.LiveLink,
            testLink = resolution.EntryLink.TestLink,
            internalLaunchUrl = resolution.InternalLaunchUrl,
            usedFallback = resolution.UsedFallback
        });
    }

    [HttpGet("EntryLinkDetails/{opportunityId:int}")]
    public async Task<IActionResult> EntryLinkDetails(int opportunityId)
    {
        var apiResult = await _lucidMarketplaceAPIController.GetLucidMarketplaceEntryLink(opportunityId);
        return ConvertApiResult(apiResult);
    }

    [AllowAnonymous]
    [HttpGet("/VT/LucidMarketplace/LaunchRespondent")]
    public async Task<IActionResult> LaunchRespondent(
        int lucidMarketplaceOpportunityId,
        int internalProjectId,
        int? internalProjectUrlId = null,
        int? internalProjectMappingId = null,
        string? respondentId = null,
        string? attemptType = "Live")
    {
        var normalizedAttemptType = NormalizeAttemptType(attemptType);
        if (lucidMarketplaceOpportunityId <= 0 || internalProjectId <= 0)
        {
            return BadRequest("Invalid Lucid Marketplace launch context.");
        }

        if (string.IsNullOrWhiteSpace(respondentId))
        {
            return BadRequest("Respondent id is required.");
        }

        var resolution = await EnsureEntryLinkAsync(new LucidMarketplaceGenerateEntryLinkRequest
        {
            LucidMarketplaceOpportunityId = lucidMarketplaceOpportunityId,
            InternalProjectId = internalProjectId,
            InternalProjectUrlId = internalProjectUrlId,
            InternalProjectMappingId = internalProjectMappingId,
            AttemptType = normalizedAttemptType,
            ForceRefresh = true
        }, "launch");

        if (!resolution.Success || resolution.EntryLink == null || resolution.Context == null)
        {
            return Content(resolution.Message ?? "Unable to prepare the Lucid Marketplace launch link.", "text/plain");
        }

        var supplierProjectResolution = await EnsureSupplierProjectUidAsync(resolution.Context, respondentId);
        if (!supplierProjectResolution.Success || string.IsNullOrWhiteSpace(supplierProjectResolution.SupplierProjectUid))
        {
            await AppendLogAsync(
                actionName: "LaunchRespondent",
                requestUrl: $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
                requestBody: JsonConvert.SerializeObject(new
                {
                    lucidMarketplaceOpportunityId,
                    internalProjectId,
                    internalProjectUrlId,
                    internalProjectMappingId,
                    respondentId,
                    normalizedAttemptType
                }),
                source: "launch",
                supplierCode: resolution.Context.SupplierCode,
                relatedEntityId: resolution.Context.LucidMarketplaceOpportunityId,
                relatedSurveyId: resolution.Context.LucidSurveyId,
                responseBodyOverride: supplierProjectResolution.Message ?? "Unable to create the legacy SupplierProjects tracking row.",
                responseStatusCodeOverride: StatusCodes.Status500InternalServerError,
                forceSuccess: false);

            return Content(supplierProjectResolution.Message ?? "Unable to create the legacy SupplierProjects tracking row.", "text/plain");
        }

        var supplierProjectUid = supplierProjectResolution.SupplierProjectUid;
        var vendorLaunchUrl = normalizedAttemptType == "Test"
            ? resolution.EntryLink.TestLink
            : BuildLucidRespondentLaunchUrl(resolution.EntryLink.LiveLink, supplierProjectUid);

        if (string.IsNullOrWhiteSpace(vendorLaunchUrl))
        {
            await AppendLogAsync(
                actionName: "LaunchRespondent",
                requestUrl: $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
                requestBody: JsonConvert.SerializeObject(new
                {
                    lucidMarketplaceOpportunityId,
                    internalProjectId,
                    internalProjectUrlId,
                    internalProjectMappingId,
                    respondentId,
                    normalizedAttemptType
                }),
                source: "launch",
                supplierCode: resolution.Context.SupplierCode,
                relatedEntityId: resolution.Context.LucidMarketplaceOpportunityId,
                relatedSurveyId: resolution.Context.LucidSurveyId,
                responseBodyOverride: "Vendor launch URL is unavailable.",
                responseStatusCodeOverride: StatusCodes.Status500InternalServerError,
                forceSuccess: false);

            return Content("Vendor launch URL is unavailable.", "text/plain");
        }

        var attemptPayload = new LucidMarketplaceRespondentAttemptDTO
        {
            LucidMarketplaceOpportunityId = resolution.Context.LucidMarketplaceOpportunityId,
            InternalProjectId = resolution.Context.InternalProjectId,
            InternalProjectUrlId = resolution.Context.InternalProjectUrlId,
            InternalProjectMappingId = resolution.Context.InternalProjectMappingId,
            LucidSurveyId = resolution.Context.LucidSurveyId,
            SupplierCode = resolution.Context.SupplierCode,
            RespondentId = supplierProjectUid,
            LaunchUrl = vendorLaunchUrl,
            AttemptType = normalizedAttemptType,
            AttemptedOn = DateTime.Now,
            RawJson = JsonConvert.SerializeObject(new
            {
                resolution.Context,
                sourceRespondentId = supplierProjectResolution.SourceRespondentId,
                supplierProjectUid,
                supplierProjectCreated = supplierProjectResolution.CreatedSupplierProject,
                entryLinkId = resolution.EntryLink.Id,
                resolution.InternalLaunchUrl
            })
        };

        var saveAttemptResult = await _lucidMarketplaceAPIController.SaveLucidMarketplaceRespondentAttempt(attemptPayload);
        LucidMarketplaceRespondentAttemptDTO? savedAttempt = null;
        if (saveAttemptResult is ObjectResult saveAttemptObject && saveAttemptObject.StatusCode == StatusCodes.Status200OK)
        {
            savedAttempt = saveAttemptObject.Value as LucidMarketplaceRespondentAttemptDTO;
        }

        if (savedAttempt == null || savedAttempt.Id <= 0)
        {
            await AppendLogAsync(
                actionName: "LaunchRespondent",
                requestUrl: $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
                requestBody: JsonConvert.SerializeObject(attemptPayload),
                source: "launch",
                supplierCode: resolution.Context.SupplierCode,
                relatedEntityId: resolution.Context.LucidMarketplaceOpportunityId,
                relatedSurveyId: resolution.Context.LucidSurveyId,
                apiResult: saveAttemptResult,
                responseBodyOverride: "Unable to track the Lucid Marketplace launch attempt.",
                responseStatusCodeOverride: StatusCodes.Status500InternalServerError,
                forceSuccess: false);

            return Content("Unable to track the Lucid Marketplace launch attempt.", "text/plain");
        }

        Response.Cookies.Append(
            GetAttemptCookieName(resolution.Context.InternalProjectMappingId, resolution.Context.LucidMarketplaceOpportunityId),
            savedAttempt.Id.ToString(CultureInfo.InvariantCulture),
            new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps,
                Expires = DateTimeOffset.UtcNow.AddHours(4)
            });

        await AppendLogAsync(
            actionName: "LaunchRespondent",
            requestUrl: $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
            requestBody: JsonConvert.SerializeObject(new
            {
                attemptId = savedAttempt.Id,
                vendorLaunchUrl,
                normalizedAttemptType,
                supplierProjectUid,
                supplierProjectResolution.CreatedSupplierProject
            }),
            source: "launch",
            supplierCode: resolution.Context.SupplierCode,
            relatedEntityId: resolution.Context.LucidMarketplaceOpportunityId,
            relatedSurveyId: resolution.Context.LucidSurveyId,
            responseBodyOverride: vendorLaunchUrl,
            responseStatusCodeOverride: StatusCodes.Status302Found,
            forceSuccess: true);

        return Redirect(vendorLaunchUrl);
    }

    [AllowAnonymous]
    [HttpGet("/VT/LucidMarketplace/RedirectReturn")]
    public async Task<IActionResult> RedirectReturn(
        int lucidMarketplaceOpportunityId,
        int internalProjectId,
        int? internalProjectUrlId = null,
        int? internalProjectMappingId = null,
        string? status = null,
        string? mid = null)
    {
        var rawQuery = Request.QueryString.HasValue ? Request.QueryString.Value : string.Empty;
        var attemptCookieName = GetAttemptCookieName(internalProjectMappingId, lucidMarketplaceOpportunityId);
        LucidMarketplaceRespondentAttemptDTO? attempt = null;

        if (Request.Cookies.TryGetValue(attemptCookieName, out var attemptIdValue) &&
            int.TryParse(attemptIdValue, out var attemptId) &&
            attemptId > 0)
        {
            var attemptResult = await _lucidMarketplaceAPIController.GetLucidMarketplaceRespondentAttempt(attemptId);
            if (attemptResult is ObjectResult attemptObject && attemptObject.StatusCode == StatusCodes.Status200OK)
            {
                attempt = attemptObject.Value as LucidMarketplaceRespondentAttemptDTO;
            }
        }

        attempt ??= await _lucidMarketplaceAPIController.GetLatestLucidMarketplaceRespondentAttemptAsync(
            internalProjectMappingId,
            lucidMarketplaceOpportunityId,
            string.IsNullOrWhiteSpace(mid) ? null : mid.Trim());

        if (attempt == null || attempt.Id <= 0)
        {
            await AppendLogAsync(
                actionName: "RedirectReturn",
                requestUrl: $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
                requestBody: rawQuery,
                source: "redirect",
                relatedEntityId: lucidMarketplaceOpportunityId > 0 ? lucidMarketplaceOpportunityId : null,
                responseBodyOverride: "Redirect return missing required attempt identifier.",
                responseStatusCodeOverride: StatusCodes.Status400BadRequest,
                forceSuccess: false);

            return Content("Redirect return missing required attempt identifier.", "text/plain");
        }

        var returnCode = ExtractReturnCode();
        var normalizedReturn = LucidMarketplaceReconciliationHelper.NormalizeRedirectStatus(status);
        var redirectStatusInfo = LucidMarketplaceReconciliationHelper.ResolveRedirectStatusInfo(normalizedReturn);
        attempt.InternalProjectId ??= internalProjectId > 0 ? internalProjectId : null;
        attempt.InternalProjectUrlId ??= internalProjectUrlId;
        attempt.InternalProjectMappingId ??= internalProjectMappingId;
        attempt.SessionId = string.IsNullOrWhiteSpace(mid) ? attempt.SessionId : mid.Trim();
        attempt.ReturnStatus = normalizedReturn;
        attempt.ReturnCode = string.IsNullOrWhiteSpace(returnCode) ? normalizedReturn ?? "Default" : returnCode;
        attempt.ReturnRawQuery = rawQuery;

        if (!string.Equals(attempt.FinalStatusSource, "OutcomesFeed", StringComparison.OrdinalIgnoreCase))
        {
            attempt.FinalStatus = redirectStatusInfo.FinalStatus;
            attempt.FinalStatusSource = "RedirectReturn";
            attempt.IsCompleted = redirectStatusInfo.IsCompleted;
            attempt.IsTerminated = redirectStatusInfo.IsTerminated;
            attempt.IsOverQuota = redirectStatusInfo.IsOverQuota;
            attempt.IsQualityTermination = redirectStatusInfo.IsQualityTermination;
            attempt.IsDuplicate = redirectStatusInfo.IsDuplicate;
            attempt.IsSecurityTermination = redirectStatusInfo.IsSecurityTermination;
        }

        attempt.ModifiedDate = DateTime.Now;

        var updateAttemptResult = await _lucidMarketplaceAPIController.SaveLucidMarketplaceRespondentAttempt(attempt);
        Response.Cookies.Delete(attemptCookieName);

        await AppendLogAsync(
            actionName: "RedirectReturn",
            requestUrl: $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}",
            requestBody: rawQuery,
            source: "redirect",
            supplierCode: attempt.SupplierCode,
            relatedEntityId: attempt.LucidMarketplaceOpportunityId,
            relatedSurveyId: attempt.LucidSurveyId,
            apiResult: updateAttemptResult,
            responseBodyOverride: JsonConvert.SerializeObject(new
            {
                attempt.Id,
                attempt.ReturnStatus,
                attempt.ReturnCode,
                attempt.SessionId
            }),
            responseStatusCodeOverride: StatusCodes.Status200OK);

        var projectStatus = string.Equals(attempt.FinalStatusSource, "OutcomesFeed", StringComparison.OrdinalIgnoreCase)
            ? LucidMarketplaceReconciliationHelper.MapLucidFinalStatusToInternalStatus(attempt.FinalStatus)
                ?? LucidMarketplaceReconciliationHelper.MapRedirectStatusToProjectStatus(attempt.ReturnStatus)
            : LucidMarketplaceReconciliationHelper.MapRedirectStatusToProjectStatus(attempt.ReturnStatus);
        if (string.IsNullOrWhiteSpace(attempt.RespondentId))
        {
            return Content(projectStatus, "text/plain");
        }

        return Redirect($"/VT/ProjectStatus.aspx?ID={Uri.EscapeDataString(attempt.RespondentId)}&Status={Uri.EscapeDataString(projectStatus)}&RC=0");
    }

    private async Task<bool> TryApplyLegacyCompletionFromOutcomeAsync(LucidMarketplaceLegacyCompletionCandidate candidate)
    {
        if (candidate == null ||
            string.IsNullOrWhiteSpace(candidate.InternalRespondentUid) ||
            string.IsNullOrWhiteSpace(candidate.LegacyProjectStatus))
        {
            return false;
        }

        var supplierProject = await _surveyAPIController.GetSupplierProjectByUidAsync(candidate.InternalRespondentUid);
        if (supplierProject == null || string.IsNullOrWhiteSpace(supplierProject.UID))
        {
            _logger.LogWarning(
                "Lucid Marketplace outcome {OutcomeId} could not resolve SupplierProjects UID {Uid}.",
                candidate.OutcomeId,
                candidate.InternalRespondentUid);
            return false;
        }

        if (string.Equals(supplierProject.Status?.Trim(), candidate.LegacyProjectStatus, StringComparison.OrdinalIgnoreCase) &&
            supplierProject.IsSent == 1)
        {
            return false;
        }

        var updateResult = await _legacyProjectStatusService.ApplyAsync(
            supplierProject.UID.Trim(),
            candidate.LegacyProjectStatus);

        if (updateResult.Success)
        {
            return true;
        }

        _logger.LogWarning(
            "Lucid Marketplace outcome {OutcomeId} failed to update legacy status for UID {Uid}. Status={Status}, Response={Response}",
            candidate.OutcomeId,
            supplierProject.UID,
            candidate.LegacyProjectStatus,
            updateResult.UpdateResponse);
        return false;
    }

    [HttpGet("LogDetails/{id}")]
    public async Task<IActionResult> LogDetails(int id)
    {
        var apiResult = await _lucidMarketplaceAPIController.GetLucidMarketplaceLog(id);
        return ConvertApiResult(apiResult);
    }

    private async Task<LucidMarketplaceSettingDTO?> GetCurrentSettingAsync()
    {
        var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceSetting();
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as LucidMarketplaceSettingDTO
            : null;
    }

    private async Task<List<LucidMarketplaceSubscriptionDTO>> GetSubscriptionsAsync()
    {
        var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceSubscriptions();
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as List<LucidMarketplaceSubscriptionDTO> ?? new List<LucidMarketplaceSubscriptionDTO>()
            : new List<LucidMarketplaceSubscriptionDTO>();
    }

    private async Task<LucidMarketplaceCallbackValidationResult> ValidateCallbackSignatureAsync(
        string subscriptionType,
        string? supplierCode,
        string requestBody)
    {
        var subscriptions = await GetSubscriptionsAsync();
        var verification = LucidMarketplaceCallbackSignatureHelper.Verify(
            Request.Headers["X-Lucid-Signature"].ToString(),
            requestBody,
            subscriptions,
            subscriptionType,
            supplierCode);

        var effectiveSupplierCode = string.IsNullOrWhiteSpace(supplierCode)
            ? verification.SupplierCode
            : supplierCode;

        if (verification.IsValid)
        {
            _logger.LogInformation(
                "Lucid Marketplace callback signature validated. SubscriptionType={SubscriptionType}, SupplierCode={SupplierCode}, SubscriptionId={SubscriptionId}, KeyId={KeyId}, Timestamp={Timestamp}, Details={Details}",
                subscriptionType,
                effectiveSupplierCode,
                verification.SubscriptionId,
                verification.KeyId,
                verification.Timestamp,
                verification.DiagnosticMessage);
        }
        else
        {
            _logger.LogWarning(
                "Lucid Marketplace callback signature rejected. SubscriptionType={SubscriptionType}, SupplierCode={SupplierCode}, ReasonCode={ReasonCode}, SubscriptionId={SubscriptionId}, KeyId={KeyId}, Timestamp={Timestamp}, Message={Message}, Details={Details}",
                subscriptionType,
                effectiveSupplierCode ?? supplierCode,
                verification.ReasonCode,
                verification.SubscriptionId,
                verification.KeyId,
                verification.Timestamp,
                verification.Message,
                verification.DiagnosticMessage);
        }

        return new LucidMarketplaceCallbackValidationResult
        {
            IsValid = verification.IsValid,
            StatusCode = verification.StatusCode,
            Message = verification.Message,
            EffectiveSupplierCode = effectiveSupplierCode,
            ReasonCode = verification.ReasonCode,
            DiagnosticMessage = verification.DiagnosticMessage,
            SubscriptionId = verification.SubscriptionId,
            KeyId = verification.KeyId,
            Timestamp = verification.Timestamp
        };
    }

    private static string BuildCallbackValidationLogPayload(
        LucidMarketplaceCallbackValidationResult validation,
        string? signatureHeader,
        string status)
    {
        return JsonConvert.SerializeObject(new
        {
            status,
            validation.Message,
            validation.ReasonCode,
            validation.DiagnosticMessage,
            validation.EffectiveSupplierCode,
            validation.SubscriptionId,
            validation.KeyId,
            validation.Timestamp,
            SignatureHeader = string.IsNullOrWhiteSpace(signatureHeader) ? null : signatureHeader
        });
    }

    private async Task<LucidMarketplaceOpportunityDTO?> GetOpportunityAsync(int opportunityId)
    {
        var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceOpportunity(opportunityId);
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as LucidMarketplaceOpportunityDTO
            : null;
    }

    private async Task<LucidMarketplaceProxyResponse> CallConsultingsProxyAsync(string actionPath, LucidMarketplaceProxyRequest payload)
    {
        var baseUrl = _configuration.GetValue<string>("ConsultingApiBaseUrl");
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return new LucidMarketplaceProxyResponse
            {
                Result = false,
                IsStub = true,
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Consultings proxy base URL is not configured."
            };
        }

        var url = $"{baseUrl.TrimEnd('/')}/VT/LucidMarketplaceProxy/{actionPath}";

        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync(url, payload);
            var content = await response.Content.ReadAsStringAsync();

            var proxyResponse = string.IsNullOrWhiteSpace(content)
                ? new LucidMarketplaceProxyResponse()
                : JsonConvert.DeserializeObject<LucidMarketplaceProxyResponse>(content) ?? new LucidMarketplaceProxyResponse();

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
            _logger.LogError(ex, "Consultings Lucid Marketplace proxy call failed. ActionPath={ActionPath}", actionPath);
            return new LucidMarketplaceProxyResponse
            {
                Result = false,
                IsStub = true,
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                RequestUrl = url
            };
        }
    }

    private async Task AppendLogAsync(
        string actionName,
        string? requestUrl,
        string? requestBody,
        string source,
        string? supplierCode = null,
        int? relatedEntityId = null,
        int? relatedSurveyId = null,
        IActionResult? apiResult = null,
        LucidMarketplaceProxyResponse? proxyResponse = null,
        string? responseBodyOverride = null,
        int? responseStatusCodeOverride = null,
        bool? forceSuccess = null)
    {
        var log = new LucidMarketplaceSyncLogDTO
        {
            ModuleName = "Lucid Marketplace",
            ActionName = actionName,
            RequestUrl = requestUrl,
            RequestBodySnapshot = requestBody,
            Source = source,
            SupplierCode = supplierCode,
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
            log.ErrorText = log.IsSuccess ? null : (proxyResponse.Message ?? responseBodyOverride);
        }
        else if (apiResult != null)
        {
            switch (apiResult)
            {
                case ObjectResult objectResult:
                    log.ResponseStatusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;
                    log.ResponseBodySnapshot = responseBodyOverride ?? JsonConvert.SerializeObject(objectResult.Value);
                    log.IsSuccess = forceSuccess ?? ((objectResult.StatusCode ?? StatusCodes.Status200OK) < 400);
                    log.ErrorText = log.IsSuccess ? null : JsonConvert.SerializeObject(objectResult.Value);
                    break;
                case StatusCodeResult statusCodeResult:
                    log.ResponseStatusCode = responseStatusCodeOverride ?? statusCodeResult.StatusCode;
                    log.ResponseBodySnapshot = responseBodyOverride;
                    log.IsSuccess = forceSuccess ?? (statusCodeResult.StatusCode < 400);
                    log.ErrorText = log.IsSuccess ? null : $"Status code {statusCodeResult.StatusCode}";
                    break;
            }
        }
        else
        {
            log.ResponseStatusCode = responseStatusCodeOverride;
            log.ResponseBodySnapshot = responseBodyOverride;
            log.IsSuccess = forceSuccess ?? true;
            log.ErrorText = log.IsSuccess ? null : responseBodyOverride;
        }

        await _lucidMarketplaceAPIController.AddLucidMarketplaceLog(log);
    }

    private int? GetCurrentUserId()
    {
        return HttpContext.Session.GetInt32("UserId");
    }

    private static string? ConvertToWholeNumberString(decimal? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        return decimal.ToInt32(decimal.Round(value.Value, 0, MidpointRounding.AwayFromZero))
            .ToString(CultureInfo.InvariantCulture);
    }

    private static VendorAddProjectResult? ExtractVendorAddProjectResult(IActionResult actionResult)
    {
        object? value = actionResult switch
        {
            ObjectResult objectResult => objectResult.Value,
            JsonResult jsonResult => jsonResult.Value,
            _ => null
        };

        if (value == null)
        {
            return null;
        }

        if (value is VendorAddProjectResult typedResult)
        {
            return typedResult;
        }

        try
        {
            return JsonConvert.DeserializeObject<VendorAddProjectResult>(JsonConvert.SerializeObject(value));
        }
        catch
        {
            return null;
        }
    }

    private static int GetStatusCode(IActionResult actionResult)
    {
        return actionResult switch
        {
            ObjectResult objectResult => objectResult.StatusCode ?? StatusCodes.Status200OK,
            JsonResult jsonResult => jsonResult.StatusCode ?? StatusCodes.Status200OK,
            StatusCodeResult statusCodeResult => statusCodeResult.StatusCode,
            IStatusCodeActionResult statusCodeActionResult when statusCodeActionResult.StatusCode.HasValue => statusCodeActionResult.StatusCode.Value,
            _ => StatusCodes.Status200OK
        };
    }

    private IActionResult ConvertApiResult(IActionResult apiResult)
    {
        switch (apiResult)
        {
            case JsonResult jsonResult:
                return jsonResult;
            case ObjectResult objectResult:
                return new JsonResult(objectResult.Value)
                {
                    StatusCode = objectResult.StatusCode
                };
            case ContentResult contentResult:
                return contentResult;
            case StatusCodeResult statusCodeResult:
                return StatusCode(statusCodeResult.StatusCode);
            case IStatusCodeActionResult status when status.StatusCode.HasValue:
                return StatusCode(status.StatusCode.Value);
            default:
                return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    private async Task<LucidMarketplaceEntryLinkResolution> EnsureEntryLinkAsync(
        LucidMarketplaceGenerateEntryLinkRequest inputData,
        string source)
    {
        var context = await _lucidMarketplaceAPIController.GetLucidMarketplaceLaunchContextAsync(
            inputData.LucidMarketplaceOpportunityId,
            inputData.InternalProjectId,
            inputData.InternalProjectMappingId);

        if (context == null || context.LucidMarketplaceOpportunityId <= 0)
        {
            return LucidMarketplaceEntryLinkResolution.Fail("Invalid Opportunity");
        }

        if (context.InternalProjectId.GetValueOrDefault() <= 0 || context.InternalProjectMappingId.GetValueOrDefault() <= 0)
        {
            return LucidMarketplaceEntryLinkResolution.Fail("Project not mapped");
        }

        if (!string.Equals(context.ProjectFrom, "LucidMarketplace", StringComparison.OrdinalIgnoreCase))
        {
            return LucidMarketplaceEntryLinkResolution.Fail("Project is not mapped from Lucid Marketplace.");
        }

        if (context.LucidSurveyId <= 0)
        {
            return LucidMarketplaceEntryLinkResolution.Fail("Missing Required Data");
        }

        if (string.IsNullOrWhiteSpace(context.SupplierCode))
        {
            return LucidMarketplaceEntryLinkResolution.Fail("Supplier code is required before generating entry links.");
        }

        if (!TryResolveStoredMarketplaceSurveyNumber(context, out var resolvedSurveyNumber, out var surveyNumberError))
        {
            var validationDiagnostics = BuildEntryLinkRequestDiagnostics(
                context,
                null,
                surveyNumberPathValue: null,
                createAttempted: false,
                fallbackAttempted: false,
                finalReason: surveyNumberError);

            await AppendLogAsync(
                actionName: "GenerateEntryLinkValidation",
                requestUrl: "/VT/LucidMarketplaceApi/GenerateEntryLinkFromOpportunity",
                requestBody: JsonConvert.SerializeObject(validationDiagnostics),
                source: source,
                supplierCode: context.SupplierCode,
                relatedEntityId: context.LucidMarketplaceOpportunityId,
                relatedSurveyId: context.LucidSurveyId,
                responseBodyOverride: surveyNumberError,
                responseStatusCodeOverride: StatusCodes.Status400BadRequest,
                forceSuccess: false);

            return LucidMarketplaceEntryLinkResolution.Fail(surveyNumberError ?? "Cannot generate Lucid Marketplace target link because SurveyNumber is not stored for this opportunity.");
        }

        var existingEntryLink = await GetEntryLinkAsync(context.LucidMarketplaceOpportunityId);
        var internalLaunchUrl = BuildInternalLaunchUrl(context, NormalizeAttemptType(inputData.AttemptType), "[respondentID]");

        if (string.IsNullOrWhiteSpace(context.BaseUrl) ||
            string.IsNullOrWhiteSpace(context.ApiKey) ||
            !context.UseConsultingsBridge)
        {
            if (existingEntryLink?.Id > 0 && !string.IsNullOrWhiteSpace(existingEntryLink.LiveLink))
            {
                await _lucidMarketplaceAPIController.SyncLucidMarketplaceProjectLaunchUrlAsync(context.LucidMarketplaceOpportunityId, internalLaunchUrl);
                return LucidMarketplaceEntryLinkResolution.SuccessResult(existingEntryLink, context, internalLaunchUrl, true, "Stored Lucid Marketplace link loaded.");
            }

            return LucidMarketplaceEntryLinkResolution.Fail("Save active Lucid Marketplace settings and enable the Consultings bridge before generating entry links.");
        }

        var proxyRequest = BuildEntryLinkProxyRequest(context, resolvedSurveyNumber);
        var currentUserId = GetCurrentUserId();
        var createRequestDiagnostics = BuildEntryLinkRequestDiagnostics(
            context,
            proxyRequest.EntryLink,
            resolvedSurveyNumber,
            createAttempted: true,
            fallbackAttempted: false,
            finalReason: null);

        var createResponse = await CallConsultingsProxyAsync("CreateSupplierLink", proxyRequest);
        await AppendLogAsync(
            actionName: "GenerateEntryLinkCreate",
            requestUrl: createResponse.RequestUrl ?? "/VT/LucidMarketplaceApi/GenerateEntryLinkFromOpportunity",
            requestBody: JsonConvert.SerializeObject(createRequestDiagnostics),
            source: source,
            supplierCode: context.SupplierCode,
            relatedEntityId: context.LucidMarketplaceOpportunityId,
            relatedSurveyId: context.LucidSurveyId,
            responseBodyOverride: BuildProxyResponseLogSnapshot(createResponse),
            proxyResponse: createResponse);

        LucidMarketplaceEntryLinkDTO? resolvedEntryLink = TryBuildEntryLinkFromProxyResponse(context, createResponse, currentUserId);
        var usedFallback = false;
        LucidMarketplaceProxyResponse? getResponse = null;

        if (resolvedEntryLink == null && ShouldAttemptEntryLinkFallback(createResponse))
        {
            getResponse = await CallConsultingsProxyAsync("GetSupplierLink", proxyRequest);
            var fallbackRequestDiagnostics = BuildEntryLinkRequestDiagnostics(
                context,
                proxyRequest.EntryLink,
                resolvedSurveyNumber,
                createAttempted: true,
                fallbackAttempted: true,
                finalReason: "Create-link did not produce a usable target. Running show-link fallback.");

            await AppendLogAsync(
                actionName: "GenerateEntryLinkShow",
                requestUrl: getResponse.RequestUrl ?? "/VT/LucidMarketplaceApi/GenerateEntryLinkFromOpportunity",
                requestBody: JsonConvert.SerializeObject(fallbackRequestDiagnostics),
                source: source,
                supplierCode: context.SupplierCode,
                relatedEntityId: context.LucidMarketplaceOpportunityId,
                relatedSurveyId: context.LucidSurveyId,
                responseBodyOverride: BuildProxyResponseLogSnapshot(getResponse),
                proxyResponse: getResponse);

            resolvedEntryLink = TryBuildEntryLinkFromProxyResponse(context, getResponse, currentUserId);
            usedFallback = resolvedEntryLink != null;

            if (resolvedEntryLink != null && ShouldUpdateEntryLink(resolvedEntryLink, proxyRequest.EntryLink))
            {
                var updateResponse = await CallConsultingsProxyAsync("UpdateSupplierLink", proxyRequest);
                await AppendLogAsync(
                    actionName: "GenerateEntryLinkUpdate",
                    requestUrl: updateResponse.RequestUrl ?? "/VT/LucidMarketplaceApi/GenerateEntryLinkFromOpportunity",
                    requestBody: JsonConvert.SerializeObject(fallbackRequestDiagnostics),
                    source: source,
                    supplierCode: context.SupplierCode,
                    relatedEntityId: context.LucidMarketplaceOpportunityId,
                    relatedSurveyId: context.LucidSurveyId,
                    responseBodyOverride: BuildProxyResponseLogSnapshot(updateResponse),
                    proxyResponse: updateResponse);

                    var updatedEntryLink = TryBuildEntryLinkFromProxyResponse(context, updateResponse, currentUserId);
                    if (updatedEntryLink != null)
                    {
                    resolvedEntryLink = updatedEntryLink;
                }
            }
        }

        if (resolvedEntryLink == null && existingEntryLink?.Id > 0 && !string.IsNullOrWhiteSpace(existingEntryLink.LiveLink))
        {
            resolvedEntryLink = existingEntryLink;
            usedFallback = true;
        }

        if (resolvedEntryLink == null)
        {
            var finalFailureMessage = BuildEntryLinkFailureMessage(context, resolvedSurveyNumber, createResponse, getResponse);
            await AppendLogAsync(
                actionName: "GenerateEntryLinkFailed",
                requestUrl: "/VT/LucidMarketplaceApi/GenerateEntryLinkFromOpportunity",
                requestBody: JsonConvert.SerializeObject(BuildEntryLinkRequestDiagnostics(
                    context,
                    proxyRequest.EntryLink,
                    resolvedSurveyNumber,
                    createAttempted: true,
                    fallbackAttempted: getResponse != null,
                    finalReason: finalFailureMessage)),
                source: source,
                supplierCode: context.SupplierCode,
                relatedEntityId: context.LucidMarketplaceOpportunityId,
                relatedSurveyId: context.LucidSurveyId,
                responseBodyOverride: JsonConvert.SerializeObject(new
                {
                    Create = BuildProxyResponseSummary(createResponse),
                    Fallback = BuildProxyResponseSummary(getResponse)
                }),
                responseStatusCodeOverride: getResponse?.StatusCode ?? createResponse.StatusCode ?? StatusCodes.Status500InternalServerError,
                forceSuccess: false);

            return LucidMarketplaceEntryLinkResolution.Fail(finalFailureMessage);
        }

        var saveResult = await _lucidMarketplaceAPIController.SaveLucidMarketplaceEntryLink(resolvedEntryLink);
        if (saveResult is ObjectResult saveObjectResult && saveObjectResult.StatusCode == StatusCodes.Status200OK)
        {
            resolvedEntryLink = saveObjectResult.Value as LucidMarketplaceEntryLinkDTO ?? resolvedEntryLink;
        }

        await _lucidMarketplaceAPIController.SyncLucidMarketplaceProjectLaunchUrlAsync(context.LucidMarketplaceOpportunityId, internalLaunchUrl);
        return LucidMarketplaceEntryLinkResolution.SuccessResult(
            resolvedEntryLink,
            context,
            internalLaunchUrl,
            usedFallback,
            usedFallback ? "Lucid Marketplace supplier link loaded using fallback retrieval." : "Lucid Marketplace supplier link generated successfully.");
    }

    private async Task<string?> SyncInternalLaunchUrlAsync(int lucidMarketplaceOpportunityId)
    {
        var context = await _lucidMarketplaceAPIController.GetLucidMarketplaceLaunchContextAsync(lucidMarketplaceOpportunityId: lucidMarketplaceOpportunityId);
        if (context == null || context.LucidMarketplaceOpportunityId <= 0)
        {
            return null;
        }

        var internalLaunchUrl = BuildInternalLaunchUrl(context, "Live", "[respondentID]");
        await _lucidMarketplaceAPIController.SyncLucidMarketplaceProjectLaunchUrlAsync(context.LucidMarketplaceOpportunityId, internalLaunchUrl);
        return internalLaunchUrl;
    }

    private LucidMarketplaceProxyRequest BuildEntryLinkProxyRequest(LucidMarketplaceLaunchContextDTO context, int resolvedSurveyNumber)
    {
        return new LucidMarketplaceProxyRequest
        {
            Setting = new LucidMarketplaceSettingDTO
            {
                BaseUrl = context.BaseUrl,
                ApiKey = context.ApiKey,
                SupplierCode = context.SupplierCode,
                UseConsultingsBridge = context.UseConsultingsBridge
            },
            EntryLink = new LucidMarketplaceEntryLinkProxyRequest
            {
                SurveyNumber = resolvedSurveyNumber,
                SupplierCode = context.SupplierCode,
                SupplierLinkTypeCode = string.IsNullOrWhiteSpace(context.SupplierLinkTypeCode)
                    ? "OWS"
                    : context.SupplierLinkTypeCode.Trim(),
                TrackingTypeCode = string.IsNullOrWhiteSpace(context.TrackingTypeCode)
                    ? "NONE"
                    : context.TrackingTypeCode.Trim(),
                DefaultLink = BuildRedirectReturnUrl(context, "default"),
                SuccessLink = BuildRedirectReturnUrl(context, "complete"),
                FailureLink = BuildRedirectReturnUrl(context, "failure"),
                OverQuotaLink = BuildRedirectReturnUrl(context, "overquota"),
                QualityTerminationLink = BuildRedirectReturnUrl(context, "quality")
            }
        };
    }

    private string BuildInternalLaunchUrl(LucidMarketplaceLaunchContextDTO context, string attemptType, string respondentIdToken)
    {
        return BuildInternalLaunchUrl(
            context.LucidMarketplaceOpportunityId,
            context.InternalProjectId,
            context.InternalProjectUrlId,
            context.InternalProjectMappingId,
            attemptType,
            respondentIdToken);
    }

    private string BuildInternalLaunchUrl(
        int lucidMarketplaceOpportunityId,
        int? internalProjectId,
        int? internalProjectUrlId,
        int? internalProjectMappingId,
        string attemptType,
        string respondentIdToken)
    {
        var respondentValue = respondentIdToken == "[respondentID]"
            ? respondentIdToken
            : Uri.EscapeDataString(respondentIdToken ?? string.Empty);

        var relativeLaunchUrl = $"/VT/LucidMarketplace/LaunchRespondent?lucidMarketplaceOpportunityId={lucidMarketplaceOpportunityId}" +
                                $"&internalProjectId={internalProjectId.GetValueOrDefault()}" +
                                $"&internalProjectUrlId={internalProjectUrlId.GetValueOrDefault()}" +
                                $"&internalProjectMappingId={internalProjectMappingId.GetValueOrDefault()}" +
                                $"&attemptType={Uri.EscapeDataString(attemptType)}" +
                                $"&respondentId={respondentValue}";

        return BuildAbsoluteLucidMarketplaceUrl(relativeLaunchUrl);
    }

    private string BuildAbsoluteLucidMarketplaceUrl(string relativePathAndQuery)
    {
        var normalizedRelativePath = string.IsNullOrWhiteSpace(relativePathAndQuery)
            ? "/"
            : relativePathAndQuery.StartsWith("/", StringComparison.Ordinal)
                ? relativePathAndQuery
                : $"/{relativePathAndQuery}";

        var publicBaseUrl = ResolveLucidMarketplacePublicBaseUrl();
        if (string.IsNullOrWhiteSpace(publicBaseUrl))
        {
            return normalizedRelativePath;
        }

        return $"{publicBaseUrl.TrimEnd('/')}{normalizedRelativePath}";
    }

    private string? ResolveLucidMarketplacePublicBaseUrl()
    {
        if (TryResolveConfiguredPublicBaseUrl(_configuration.GetValue<string>("PublicWebBaseUrl"), null, out var configuredBaseUrl))
        {
            return configuredBaseUrl;
        }

        if (TryResolveConfiguredPublicBaseUrl(_configuration.GetValue<string>("MaskingUrl"), "/VT/MaskingUrl.aspx", out var maskingBaseUrl))
        {
            return maskingBaseUrl;
        }

        if (Request?.Host.HasValue == true)
        {
            var pathBase = Request.PathBase.HasValue ? Request.PathBase.Value : string.Empty;
            return $"{Request.Scheme}://{Request.Host}{pathBase}".TrimEnd('/');
        }

        return null;
    }

    private static bool TryResolveConfiguredPublicBaseUrl(string? configuredUrl, string? knownPathSuffix, out string? resolvedBaseUrl)
    {
        resolvedBaseUrl = null;
        if (string.IsNullOrWhiteSpace(configuredUrl) ||
            !Uri.TryCreate(configuredUrl.Trim(), UriKind.Absolute, out var configuredUri))
        {
            return false;
        }

        var root = configuredUri.GetLeftPart(UriPartial.Authority).TrimEnd('/');
        var absolutePath = configuredUri.AbsolutePath ?? string.Empty;
        string pathBase;

        if (!string.IsNullOrWhiteSpace(knownPathSuffix) &&
            absolutePath.EndsWith(knownPathSuffix, StringComparison.OrdinalIgnoreCase))
        {
            pathBase = absolutePath[..^knownPathSuffix.Length].TrimEnd('/');
        }
        else
        {
            pathBase = absolutePath == "/" ? string.Empty : absolutePath.TrimEnd('/');
        }

        resolvedBaseUrl = string.IsNullOrWhiteSpace(pathBase)
            ? root
            : $"{root}{pathBase}";

        return true;
    }

    private string BuildRedirectReturnUrl(LucidMarketplaceLaunchContextDTO context, string returnStatus)
    {
        var statusValue = returnStatus.ToLowerInvariant();
        var revenueSegment = statusValue == "complete" ? "&rev=[%REVENUE%]" : string.Empty;
        var relativeRedirectUrl = "/VT/LucidMarketplace/RedirectReturn" +
                                  $"?lucidMarketplaceOpportunityId={context.LucidMarketplaceOpportunityId}" +
                                  $"&internalProjectId={context.InternalProjectId.GetValueOrDefault()}" +
                                  $"&internalProjectUrlId={context.InternalProjectUrlId.GetValueOrDefault()}" +
                                  $"&internalProjectMappingId={context.InternalProjectMappingId.GetValueOrDefault()}" +
                                  $"&status={statusValue}" +
                                  $"&mid=[%MID%]{revenueSegment}";
        return BuildAbsoluteLucidMarketplaceUrl(relativeRedirectUrl);
    }

    private async Task<LucidMarketplaceEntryLinkDTO?> GetEntryLinkAsync(int opportunityId)
    {
        var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceEntryLink(opportunityId);
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as LucidMarketplaceEntryLinkDTO
            : null;
    }

    private static LucidMarketplaceEntryLinkDTO? TryBuildEntryLinkFromProxyResponse(
        LucidMarketplaceLaunchContextDTO context,
        LucidMarketplaceProxyResponse proxyResponse,
        int? currentUserId)
    {
        if (!proxyResponse.Result || string.IsNullOrWhiteSpace(proxyResponse.ResponseBody))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(proxyResponse.ResponseBody);
            var root = document.RootElement;
            if (!TryResolveMarketplaceTargetElement(root, context.SupplierCode, out var supplierLinkElement))
            {
                return null;
            }

            if (supplierLinkElement.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            var liveLink = GetJsonString(supplierLinkElement, "LiveLink", "LiveSupplierLink");
            var testLink = GetJsonString(supplierLinkElement, "TestLink", "TestSupplierLink");

            decimal? rpiValue = null;
            string? rpiCurrencyCode = null;
            if (TryGetJsonProperty(supplierLinkElement, out var rpiElement, "RPI"))
            {
                rpiValue = GetJsonDecimal(rpiElement, "Value", "value");
                rpiCurrencyCode = GetJsonString(rpiElement, "CurrencyCode", "currency_code");
            }

            return new LucidMarketplaceEntryLinkDTO
            {
                LucidMarketplaceOpportunityId = context.LucidMarketplaceOpportunityId,
                LucidSurveyId = context.LucidSurveyId,
                SupplierCode = context.SupplierCode,
                InternalProjectId = context.InternalProjectId,
                InternalProjectUrlId = context.InternalProjectUrlId,
                InternalProjectMappingId = context.InternalProjectMappingId,
                SupplierLinkTypeCode = GetJsonString(supplierLinkElement, "SupplierLinkTypeCode", "SupplierLinkType") ?? "OWS",
                TrackingTypeCode = GetJsonString(supplierLinkElement, "TrackingTypeCode") ?? "NONE",
                DefaultLink = GetJsonString(supplierLinkElement, "DefaultLink"),
                SuccessLink = GetJsonString(supplierLinkElement, "SuccessLink"),
                FailureLink = GetJsonString(supplierLinkElement, "FailureLink"),
                OverQuotaLink = GetJsonString(supplierLinkElement, "OverQuotaLink"),
                QualityTerminationLink = GetJsonString(supplierLinkElement, "QualityTerminationLink"),
                LiveLink = liveLink,
                TestLink = testLink,
                SupplierLinkSid = GetJsonString(supplierLinkElement, "SupplierLinkSid", "SupplierLinkSID", "SurveySid", "SurveySID") ??
                                  ExtractQueryParameter(liveLink, "SID") ??
                                  ExtractQueryParameter(testLink, "SID"),
                RPIValue = rpiValue,
                RPICurrencyCode = rpiCurrencyCode,
                RawJson = proxyResponse.ResponseBody,
                IsActive = true,
                CreatedBy = currentUserId,
                ModifiedBy = currentUserId
            };
        }
        catch
        {
            return null;
        }
    }

    private static bool TryResolveStoredMarketplaceSurveyNumber(
        LucidMarketplaceLaunchContextDTO context,
        out int resolvedSurveyNumber,
        out string? errorMessage)
    {
        resolvedSurveyNumber = 0;
        errorMessage = null;

        if (!string.IsNullOrWhiteSpace(context.SurveyNumber) &&
            int.TryParse(context.SurveyNumber.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var surveyNumber) &&
            surveyNumber > 0)
        {
            resolvedSurveyNumber = surveyNumber;
            return true;
        }

        if (TryResolveSurveyNumberFromOpportunityRawJson(context.OpportunityRawJson, out surveyNumber))
        {
            resolvedSurveyNumber = surveyNumber;
            return true;
        }

        if (context.LucidSurveyId > 0)
        {
            resolvedSurveyNumber = context.LucidSurveyId;
            return true;
        }

        errorMessage = "Cannot generate Lucid Marketplace target link because SurveyNumber is not stored for this opportunity.";
        return false;
    }

    private static bool TryResolveSurveyNumberFromOpportunityRawJson(string? rawJson, out int surveyNumber)
    {
        surveyNumber = 0;
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(rawJson);
            var root = document.RootElement;

            if (TryResolveSurveyNumberFromJsonElement(root, out surveyNumber))
            {
                return true;
            }

            if (TryGetJsonProperty(root, out var surveyElement, "survey", "Survey") &&
                TryResolveSurveyNumberFromJsonElement(surveyElement, out surveyNumber))
            {
                return true;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    private static bool TryResolveSurveyNumberFromJsonElement(JsonElement element, out int surveyNumber)
    {
        surveyNumber = 0;
        var value = GetJsonString(element, "survey_number", "surveyNumber", "survey_id", "surveyId");
        return !string.IsNullOrWhiteSpace(value) &&
               int.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out surveyNumber) &&
               surveyNumber > 0;
    }

    private static object BuildEntryLinkRequestDiagnostics(
        LucidMarketplaceLaunchContextDTO context,
        LucidMarketplaceEntryLinkProxyRequest? request,
        int? surveyNumberPathValue,
        bool createAttempted,
        bool fallbackAttempted,
        string? finalReason)
    {
        return new
        {
            lucidMarketplaceOpportunityId = context.LucidMarketplaceOpportunityId,
            storedSurveyId = context.LucidSurveyId,
            storedSurveyNumber = context.SurveyNumber,
            surveyNumberPathArgument = surveyNumberPathValue,
            supplierCode = context.SupplierCode,
            createAttempted,
            fallbackAttempted,
            supplierLinkTypeCode = request?.SupplierLinkTypeCode,
            trackingTypeCode = request?.TrackingTypeCode,
            finalReason
        };
    }

    private static bool ShouldAttemptEntryLinkFallback(LucidMarketplaceProxyResponse? response)
    {
        if (response == null)
        {
            return false;
        }

        var responseSummary = BuildProxyResponseSummary(response);
        if ((responseSummary?.ApiResult.HasValue == true && responseSummary.ApiResult.Value != 0) ||
            (responseSummary?.ApiResultCode.HasValue == true && responseSummary.ApiResultCode.Value != 0))
        {
            return true;
        }

        if (response.StatusCode == StatusCodes.Status404NotFound)
        {
            return true;
        }

        var combinedText = string.Join(" ",
            response.Message ?? string.Empty,
            response.ResponseBody ?? string.Empty,
            responseSummary?.ApiMessagesPreview ?? string.Empty);

        return combinedText.Contains("does not exist", StringComparison.OrdinalIgnoreCase) ||
               combinedText.Contains("not found", StringComparison.OrdinalIgnoreCase) ||
               combinedText.Contains("CreateSupplierAllocationTargetFromModel failed", StringComparison.OrdinalIgnoreCase) ||
               combinedText.Contains("CreateSupplierLinkFromModel failed", StringComparison.OrdinalIgnoreCase) ||
               combinedText.Contains("targeted", StringComparison.OrdinalIgnoreCase) ||
               combinedText.Contains("otc", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildEntryLinkFailureMessage(
        LucidMarketplaceLaunchContextDTO context,
        int resolvedSurveyNumber,
        LucidMarketplaceProxyResponse createResponse,
        LucidMarketplaceProxyResponse? fallbackResponse)
    {
        if (fallbackResponse != null)
        {
            return $"Lucid Marketplace target link could not be loaded for SurveyNumber {resolvedSurveyNumber}. Create-link failed and show-link fallback also failed.";
        }

        var createSummary = BuildProxyResponseSummary(createResponse);
        if (createSummary != null && !string.IsNullOrWhiteSpace(createSummary.ApiMessagesPreview))
        {
            return $"Lucid Marketplace target link could not be created for SurveyNumber {resolvedSurveyNumber}. {createSummary.ApiMessagesPreview}";
        }

        return $"Lucid Marketplace target link could not be created for SurveyNumber {resolvedSurveyNumber}.";
    }

    private static string BuildProxyResponseLogSnapshot(LucidMarketplaceProxyResponse? proxyResponse)
    {
        return JsonConvert.SerializeObject(new
        {
            Summary = BuildProxyResponseSummary(proxyResponse),
            RawResponse = proxyResponse?.ResponseBody
        });
    }

    private static EntryLinkProxyResponseSummary? BuildProxyResponseSummary(LucidMarketplaceProxyResponse? proxyResponse)
    {
        if (proxyResponse == null)
        {
            return null;
        }

        var summary = new EntryLinkProxyResponseSummary
        {
            HttpStatusCode = proxyResponse.StatusCode,
            Message = proxyResponse.Message
        };

        if (string.IsNullOrWhiteSpace(proxyResponse.ResponseBody))
        {
            return summary;
        }

        try
        {
            using var document = JsonDocument.Parse(proxyResponse.ResponseBody);
            var root = document.RootElement;
            summary.ApiResult = GetJsonInt(root, "ApiResult");
            summary.ApiResultCode = GetJsonInt(root, "ApiResultCode");

            if (TryGetJsonProperty(root, out var apiMessagesElement, "ApiMessages") &&
                apiMessagesElement.ValueKind == JsonValueKind.Array)
            {
                summary.ApiMessages = apiMessagesElement
                    .EnumerateArray()
                    .Where(item => item.ValueKind == JsonValueKind.String)
                    .Select(item => item.GetString())
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Take(3)
                    .Cast<string>()
                    .ToList();

                summary.ApiMessagesPreview = string.Join(" | ", summary.ApiMessages);
            }
        }
        catch
        {
            summary.ApiMessagesPreview = null;
        }

        return summary;
    }

    private static bool TryResolveMarketplaceTargetElement(JsonElement root, string? supplierCode, out JsonElement supplierLinkElement)
    {
        supplierLinkElement = default;

        if (TryGetJsonProperty(root, out supplierLinkElement, "SupplierLink", "Target", "TargetModel") &&
            supplierLinkElement.ValueKind == JsonValueKind.Object)
        {
            return true;
        }

        if (TryGetJsonProperty(root, out var singleAllocationElement, "SupplierAllocation") &&
            TryGetMarketplaceTargetFromAllocation(singleAllocationElement, out supplierLinkElement))
        {
            return true;
        }

        if (TryGetJsonProperty(root, out var allocationsElement, "SupplierAllocations") &&
            allocationsElement.ValueKind == JsonValueKind.Array)
        {
            JsonElement? firstTarget = null;
            foreach (var allocationElement in allocationsElement.EnumerateArray())
            {
                if (allocationElement.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(supplierCode))
                {
                    var allocationSupplierCode = GetJsonString(allocationElement, "SupplierCode");
                    if (!string.Equals(allocationSupplierCode, supplierCode, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }

                if (TryGetMarketplaceTargetFromAllocation(allocationElement, out supplierLinkElement))
                {
                    return true;
                }
            }

            foreach (var allocationElement in allocationsElement.EnumerateArray())
            {
                if (allocationElement.ValueKind == JsonValueKind.Object &&
                    TryGetMarketplaceTargetFromAllocation(allocationElement, out var targetElement))
                {
                    firstTarget = targetElement;
                    break;
                }
            }

            if (firstTarget.HasValue)
            {
                supplierLinkElement = firstTarget.Value;
                return true;
            }
        }

        if (TryGetJsonProperty(root, out var surveyElement, "Survey") &&
            surveyElement.ValueKind == JsonValueKind.Object)
        {
            if (TryGetJsonProperty(surveyElement, out supplierLinkElement, "SupplierLink", "Target", "TargetModel") &&
                supplierLinkElement.ValueKind == JsonValueKind.Object)
            {
                return true;
            }

            if (TryGetJsonProperty(surveyElement, out var surveyAllocationsElement, "SupplierAllocations", "OfferwallAllocations") &&
                surveyAllocationsElement.ValueKind == JsonValueKind.Array)
            {
                JsonElement? firstSurveyTarget = null;
                foreach (var allocationElement in surveyAllocationsElement.EnumerateArray())
                {
                    if (allocationElement.ValueKind != JsonValueKind.Object)
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(supplierCode))
                    {
                        var allocationSupplierCode = GetJsonString(allocationElement, "SupplierCode");
                        if (!string.Equals(allocationSupplierCode, supplierCode, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }

                    if (TryGetMarketplaceTargetFromAllocation(allocationElement, out supplierLinkElement))
                    {
                        return true;
                    }
                }

                foreach (var allocationElement in surveyAllocationsElement.EnumerateArray())
                {
                    if (allocationElement.ValueKind == JsonValueKind.Object &&
                        TryGetMarketplaceTargetFromAllocation(allocationElement, out var targetElement))
                    {
                        firstSurveyTarget = targetElement;
                        break;
                    }
                }

                if (firstSurveyTarget.HasValue)
                {
                    supplierLinkElement = firstSurveyTarget.Value;
                    return true;
                }
            }
        }

        return false;
    }

    private static bool TryGetMarketplaceTargetFromAllocation(JsonElement allocationElement, out JsonElement supplierLinkElement)
    {
        supplierLinkElement = default;
        return allocationElement.ValueKind == JsonValueKind.Object &&
               TryGetJsonProperty(allocationElement, out supplierLinkElement, "Target", "TargetModel") &&
               supplierLinkElement.ValueKind == JsonValueKind.Object;
    }

    private static bool ShouldUpdateEntryLink(
        LucidMarketplaceEntryLinkDTO currentEntryLink,
        LucidMarketplaceEntryLinkProxyRequest? requestedEntryLink)
    {
        if (requestedEntryLink == null)
        {
            return false;
        }

        return !string.Equals(currentEntryLink.DefaultLink, requestedEntryLink.DefaultLink, StringComparison.OrdinalIgnoreCase) ||
               !string.Equals(currentEntryLink.SuccessLink, requestedEntryLink.SuccessLink, StringComparison.OrdinalIgnoreCase) ||
               !string.Equals(currentEntryLink.FailureLink, requestedEntryLink.FailureLink, StringComparison.OrdinalIgnoreCase) ||
               !string.Equals(currentEntryLink.OverQuotaLink, requestedEntryLink.OverQuotaLink, StringComparison.OrdinalIgnoreCase) ||
               !string.Equals(currentEntryLink.QualityTerminationLink, requestedEntryLink.QualityTerminationLink, StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildLucidRespondentLaunchUrl(string? liveLink, string respondentId)
    {
        if (string.IsNullOrWhiteSpace(liveLink) || string.IsNullOrWhiteSpace(respondentId))
        {
            return string.Empty;
        }

        var encodedRespondentId = Uri.EscapeDataString(respondentId);
        if (liveLink.Contains("[respondentID]", StringComparison.OrdinalIgnoreCase))
        {
            return liveLink.Replace("[respondentID]", encodedRespondentId, StringComparison.OrdinalIgnoreCase);
        }

        var updated = new Regex("(?i)([?&]PID=)([^&]*)").Replace(
            liveLink,
            match => $"{match.Groups[1].Value}{encodedRespondentId}",
            1);

        if (!string.Equals(updated, liveLink, StringComparison.Ordinal))
        {
            return updated;
        }

        return liveLink.Contains("?", StringComparison.Ordinal)
            ? $"{liveLink}&PID={encodedRespondentId}"
            : $"{liveLink}?PID={encodedRespondentId}";
    }

    private string ExtractReturnCode()
    {
        foreach (var key in new[] { "code", "sumstat", "client_status", "marketplace_status", "rev" })
        {
            var value = Request.Query[key].ToString();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return string.Empty;
    }

    private static string NormalizeAttemptType(string? attemptType)
    {
        return string.Equals(attemptType, "Test", StringComparison.OrdinalIgnoreCase) ? "Test" : "Live";
    }

    private async Task<(bool Success, string? SupplierProjectUid, string? SourceRespondentId, bool CreatedSupplierProject, string? Message)> EnsureSupplierProjectUidAsync(
        LucidMarketplaceLaunchContextDTO context,
        string respondentId)
    {
        var normalizedRespondentId = respondentId?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedRespondentId))
        {
            return (false, null, null, false, "Respondent id is required.");
        }

        var stableSupplierProjectUid = BuildStableSupplierProjectUid(context.ProjectMappingSid, normalizedRespondentId);
        var existingSupplierProject = await _surveyAPIController.GetSupplierProjectByUidAsync(
            stableSupplierProjectUid,
            context.ProjectMappingSid);

        if (existingSupplierProject != null && !string.IsNullOrWhiteSpace(existingSupplierProject.UID))
        {
            return (true, existingSupplierProject.UID.Trim(), normalizedRespondentId, false, null);
        }

        if (string.IsNullOrWhiteSpace(context.ProjectMappingSid))
        {
            return (false, null, normalizedRespondentId, false, "Lucid Marketplace project mapping SID is missing.");
        }

        var clientIp = HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty;
        var browser = CommonHelper.ParseBrowser(Request);
        var clientDevice = CommonHelper.Device(Request);

        var saveResult = await _surveyAPIController.SaveSupplierProject(
            clientIp,
            browser,
            "InComplete",
            context.ProjectMappingSid,
            context.ProjectMappingCode ?? string.Empty,
            stableSupplierProjectUid,
            normalizedRespondentId,
            clientDevice,
            0,
            string.Empty);

        if (string.IsNullOrWhiteSpace(saveResult) || string.Equals(saveResult, "3", StringComparison.Ordinal))
        {
            return (false, null, normalizedRespondentId, false, "Unable to create the legacy SupplierProjects tracking row.");
        }

        return (true, stableSupplierProjectUid, normalizedRespondentId, true, null);
    }

    private static string BuildStableSupplierProjectUid(string? sid, string respondentId)
    {
        var raw = $"{sid?.Trim() ?? string.Empty}|{respondentId.Trim()}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(hash)[..32];
    }

    private static string GetAttemptCookieName(int? internalProjectMappingId, int lucidMarketplaceOpportunityId)
    {
        return $"lm_attempt_{internalProjectMappingId.GetValueOrDefault()}_{lucidMarketplaceOpportunityId}";
    }

    private static string? ExtractQueryParameter(string? url, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(parameterName))
        {
            return null;
        }

        var match = Regex.Match(url, $"(?i)(?:[?&]){Regex.Escape(parameterName)}=([^&]+)");
        return match.Success ? Uri.UnescapeDataString(match.Groups[1].Value) : null;
    }

    private static bool TryGetJsonProperty(JsonElement element, out JsonElement value, params string[] propertyNames)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            value = default;
            return false;
        }

        foreach (var property in element.EnumerateObject())
        {
            if (propertyNames.Any(name => string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    private static string? GetJsonString(JsonElement element, params string[] propertyNames)
    {
        if (!TryGetJsonProperty(element, out var value, propertyNames))
        {
            return null;
        }

        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Number => value.ToString(),
            JsonValueKind.True => bool.TrueString,
            JsonValueKind.False => bool.FalseString,
            JsonValueKind.Null => null,
            _ => value.GetRawText()
        };
    }

    private static decimal? GetJsonDecimal(JsonElement element, params string[] propertyNames)
    {
        if (!TryGetJsonProperty(element, out var value, propertyNames))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out var decimalValue))
        {
            return decimalValue;
        }

        if (value.ValueKind == JsonValueKind.String &&
            decimal.TryParse(value.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimalValue))
        {
            return decimalValue;
        }

        return null;
    }

    private static int? GetJsonInt(JsonElement element, params string[] propertyNames)
    {
        if (!TryGetJsonProperty(element, out var value, propertyNames))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var intValue))
        {
            return intValue;
        }

        if (value.ValueKind == JsonValueKind.String &&
            int.TryParse(value.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue))
        {
            return intValue;
        }

        return null;
    }

    private static bool? GetJsonBoolean(JsonElement element, params string[] propertyNames)
    {
        if (!TryGetJsonProperty(element, out var value, propertyNames))
        {
            return null;
        }

        return value.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String when bool.TryParse(value.GetString(), out var boolValue) => boolValue,
            JsonValueKind.Number when value.TryGetInt32(out var intValue) => intValue != 0,
            _ => null
        };
    }

    private static string? NormalizeSubscriptionType(string? subscriptionType)
    {
        var normalized = (subscriptionType ?? string.Empty)
            .Trim()
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .ToLowerInvariant();

        return normalized switch
        {
            "opportunities" => "Opportunities",
            "outcomes" or "respondentoutcomes" => "RespondentOutcomes",
            _ => null
        };
    }

    private static LucidMarketplaceSubscriptionDTO BuildOpportunitySubscription(
        LucidMarketplaceSettingDTO currentSetting,
        LucidMarketplaceSubscriptionActionRequest inputData,
        string? supplierCode,
        int? currentUserId)
    {
        var callbackUrl = string.IsNullOrWhiteSpace(inputData.CallbackUrl)
            ? currentSetting.OpportunitiesCallbackUrl
            : inputData.CallbackUrl;

        if (string.IsNullOrWhiteSpace(callbackUrl))
        {
            throw new InvalidOperationException("An opportunities callback URL is required before creating the subscription.");
        }

        var countryLanguageCodes = NormalizeCountryLanguageCodes(inputData.CountryLanguageCodes);
        if (countryLanguageCodes.Count == 0)
        {
            throw new InvalidOperationException("Enter one or more Country-Language codes, for example eng_us, before creating the opportunities subscription.");
        }

        return new LucidMarketplaceSubscriptionDTO
        {
            SubscriptionType = "Opportunities",
            SupplierCode = supplierCode,
            CallbackUrl = callbackUrl.Trim(),
            IncludeQuotas = inputData.IncludeQuotas,
            RemoteSubscriptionId = supplierCode,
            RequestPayloadSnapshot = BuildOpportunitySubscriptionPayload(callbackUrl.Trim(), inputData.IncludeQuotas, countryLanguageCodes, inputData),
            CreatedBy = currentUserId,
            ModifiedBy = currentUserId
        };
    }

    private static LucidMarketplaceSubscriptionDTO BuildOutcomeSubscription(
        LucidMarketplaceSettingDTO currentSetting,
        LucidMarketplaceSubscriptionActionRequest inputData,
        string? supplierCode,
        int? currentUserId)
    {
        var callbackUrl = string.IsNullOrWhiteSpace(inputData.CallbackUrl)
            ? currentSetting.OutcomesCallbackUrl
            : inputData.CallbackUrl;

        if (string.IsNullOrWhiteSpace(callbackUrl))
        {
            throw new InvalidOperationException("A respondent outcomes callback URL is required before creating the subscription.");
        }

        return new LucidMarketplaceSubscriptionDTO
        {
            SubscriptionType = "RespondentOutcomes",
            SupplierCode = supplierCode,
            CallbackUrl = callbackUrl.Trim(),
            IncludeQuotas = false,
            RemoteSubscriptionId = supplierCode,
            RequestPayloadSnapshot = BuildOutcomesSubscriptionPayload(callbackUrl.Trim(), inputData.OutcomeFiltersJson),
            CreatedBy = currentUserId,
            ModifiedBy = currentUserId
        };
    }

    private static string BuildOpportunitySubscriptionPayload(
        string callbackUrl,
        bool includeQuotas,
        List<string> countryLanguageCodes,
        LucidMarketplaceSubscriptionActionRequest inputData)
    {
        return BuildOpportunitySubscriptionPayload(
            callbackUrl,
            includeQuotas,
            BuildOpportunityFiltersToken(countryLanguageCodes),
            inputData.PayloadMaxSizeMb,
            inputData.PayloadMaxSurveyCount,
            inputData.SendIntervalSeconds);
    }

    private static string BuildOpportunitySubscriptionPayload(
        string callbackUrl,
        bool includeQuotas,
        JToken? opportunitiesToken,
        int? payloadMaxSizeMb,
        int? payloadMaxSurveyCount,
        int? sendIntervalSeconds)
    {
        var payload = new JObject
        {
            ["callback"] = callbackUrl,
            ["include_quotas"] = includeQuotas,
            ["opportunities"] = opportunitiesToken?.DeepClone() ?? new JArray(new JObject())
        };

        if (payloadMaxSizeMb.HasValue)
        {
            payload["payload_max_size_mb"] = payloadMaxSizeMb.Value;
        }

        if (payloadMaxSurveyCount.HasValue)
        {
            payload["payload_max_survey_count"] = payloadMaxSurveyCount.Value;
        }

        if (sendIntervalSeconds.HasValue)
        {
            payload["send_interval_seconds"] = sendIntervalSeconds.Value;
        }

        return payload.ToString(Newtonsoft.Json.Formatting.None);
    }

    private static JToken BuildOpportunityFiltersToken(IEnumerable<string>? countryLanguageCodes)
    {
        var normalizedCodes = NormalizeCountryLanguageCodes(countryLanguageCodes).ToList();
        if (normalizedCodes.Count == 0)
        {
            return new JArray(new JObject());
        }

        return new JArray(
            new JObject(
                new JProperty("country_language",
                    new JObject(
                        new JProperty("in", new JArray(normalizedCodes))))));
    }

    private static string MergeOpportunitySubscriptionPayload(
        string callbackUrl,
        bool includeQuotas,
        JsonElement remoteRoot,
        string? existingRequestPayload)
    {
        var mergedPayload = TryParseJsonObject(existingRequestPayload) ?? new JObject();

        mergedPayload["callback"] = callbackUrl;
        mergedPayload["include_quotas"] = includeQuotas;

        if (TryGetJsonProperty(remoteRoot, out var remoteOpportunities, "opportunities") &&
            remoteOpportunities.ValueKind != JsonValueKind.Null &&
            remoteOpportunities.ValueKind != JsonValueKind.Undefined)
        {
            mergedPayload["opportunities"] = JToken.Parse(remoteOpportunities.GetRawText());
        }
        else if (mergedPayload["opportunities"] == null)
        {
            mergedPayload["opportunities"] = BuildOpportunityFiltersToken(Array.Empty<string>());
        }

        CopyJsonPropertyIfPresent(remoteRoot, mergedPayload, "payload_max_size_mb");
        CopyJsonPropertyIfPresent(remoteRoot, mergedPayload, "payload_max_survey_count");
        CopyJsonPropertyIfPresent(remoteRoot, mergedPayload, "send_interval_seconds");

        return mergedPayload.ToString(Newtonsoft.Json.Formatting.None);
    }

    private static JObject? TryParseJsonObject(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JObject.Parse(json);
        }
        catch
        {
            return null;
        }
    }

    private static void CopyJsonPropertyIfPresent(JsonElement source, JObject target, string propertyName)
    {
        if (!TryGetJsonProperty(source, out var value, propertyName))
        {
            return;
        }

        target[propertyName] = JToken.Parse(value.GetRawText());
    }

    private static string BuildOutcomesSubscriptionPayload(string callbackUrl, string? outcomeFiltersJson)
    {
        object outcomesPayload = new[] { new Dictionary<string, object?>() };

        if (!string.IsNullOrWhiteSpace(outcomeFiltersJson))
        {
            var token = JToken.Parse(outcomeFiltersJson);
            if (token.Type == JTokenType.Array)
            {
                outcomesPayload = token;
            }
            else if (token.Type == JTokenType.Object && token["outcomes"] is JArray nestedOutcomes)
            {
                outcomesPayload = nestedOutcomes;
            }
            else if (token.Type == JTokenType.Object)
            {
                outcomesPayload = new JArray(token);
            }
            else
            {
                throw new System.Text.Json.JsonException("Outcome filters must be a JSON object or array.");
            }
        }

        return JsonConvert.SerializeObject(new Dictionary<string, object?>
        {
            ["callback"] = callbackUrl,
            ["outcomes"] = outcomesPayload
        });
    }

    private static List<string> NormalizeCountryLanguageCodes(IEnumerable<string>? rawCodes)
    {
        if (rawCodes == null)
        {
            return new List<string>();
        }

        return rawCodes
            .SelectMany(code => (code ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Select(code => code.Trim().ToLowerInvariant())
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Distinct()
            .ToList();
    }

    private static LucidMarketplaceSubscriptionDTO? TryBuildSubscriptionFromRemoteResponse(
        LucidMarketplaceSettingDTO currentSetting,
        string subscriptionType,
        LucidMarketplaceProxyResponse proxyResponse,
        LucidMarketplaceSubscriptionDTO? existingSubscription)
    {
        var normalizedSubscriptionType = NormalizeSubscriptionType(subscriptionType) ?? "Opportunities";
        if (proxyResponse.StatusCode == StatusCodes.Status404NotFound)
        {
            return new LucidMarketplaceSubscriptionDTO
            {
                SubscriptionType = normalizedSubscriptionType,
                SupplierCode = existingSubscription?.SupplierCode ?? currentSetting.SupplierCode,
                CallbackUrl = normalizedSubscriptionType == "RespondentOutcomes"
                    ? existingSubscription?.CallbackUrl ?? currentSetting.OutcomesCallbackUrl
                    : existingSubscription?.CallbackUrl ?? currentSetting.OpportunitiesCallbackUrl,
                IncludeQuotas = normalizedSubscriptionType == "RespondentOutcomes"
                    ? false
                    : existingSubscription?.IncludeQuotas ?? false,
                RemoteSubscriptionId = existingSubscription?.RemoteSubscriptionId ?? currentSetting.SupplierCode,
                LastStatus = normalizedSubscriptionType == "RespondentOutcomes"
                    ? "Remote respondent outcomes subscription not found."
                    : "Remote opportunities subscription not found.",
                RequestPayloadSnapshot = existingSubscription?.RequestPayloadSnapshot,
                ResponsePayloadSnapshot = proxyResponse.ResponseBody,
                IsActive = false,
                LastValidatedOn = DateTime.Now
            };
        }

        if (string.IsNullOrWhiteSpace(proxyResponse.ResponseBody))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(proxyResponse.ResponseBody);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            if (normalizedSubscriptionType == "RespondentOutcomes")
            {
                var outcomesCallbackUrl = root.TryGetProperty("callback", out var outcomesCallbackElement) && outcomesCallbackElement.ValueKind == JsonValueKind.String
                    ? outcomesCallbackElement.GetString()
                    : existingSubscription?.CallbackUrl ?? currentSetting.OutcomesCallbackUrl;

                if (string.IsNullOrWhiteSpace(outcomesCallbackUrl))
                {
                    outcomesCallbackUrl = existingSubscription?.CallbackUrl ?? currentSetting.OutcomesCallbackUrl;
                }

                var outcomesSupplierCode = root.TryGetProperty("name", out var outcomesNameElement) && outcomesNameElement.ValueKind == JsonValueKind.String
                    ? outcomesNameElement.GetString()
                    : existingSubscription?.SupplierCode ?? currentSetting.SupplierCode;

                if (string.IsNullOrWhiteSpace(outcomesSupplierCode))
                {
                    outcomesSupplierCode = existingSubscription?.SupplierCode ?? currentSetting.SupplierCode;
                }

                var filtersJson = root.TryGetProperty("outcomes", out var outcomesElement)
                    ? outcomesElement.GetRawText()
                    : TryParseJsonObject(existingSubscription?.RequestPayloadSnapshot)?["outcomes"]?.ToString(Newtonsoft.Json.Formatting.None) ?? "[{}]";
                var filtersCount = root.TryGetProperty("outcomes", out outcomesElement) && outcomesElement.ValueKind == JsonValueKind.Array
                    ? outcomesElement.GetArrayLength()
                    : 0;

                return new LucidMarketplaceSubscriptionDTO
                {
                    SubscriptionType = "RespondentOutcomes",
                    SupplierCode = outcomesSupplierCode,
                    CallbackUrl = outcomesCallbackUrl,
                    IncludeQuotas = false,
                    RemoteSubscriptionId = outcomesSupplierCode,
                    RequestPayloadSnapshot = BuildOutcomesSubscriptionPayload(outcomesCallbackUrl ?? currentSetting.OutcomesCallbackUrl ?? string.Empty, filtersJson),
                    LastStatus = filtersCount > 0
                        ? $"Remote respondent outcomes subscription loaded with {filtersCount} filter rule(s)."
                        : "Remote respondent outcomes subscription loaded.",
                    ResponsePayloadSnapshot = proxyResponse.ResponseBody,
                    IsActive = proxyResponse.StatusCode.HasValue && proxyResponse.StatusCode.Value < 300,
                    LastValidatedOn = DateTime.Now
                };
            }

            var callbackUrl = root.TryGetProperty("callback", out var callbackElement) && callbackElement.ValueKind == JsonValueKind.String
                ? callbackElement.GetString()
                : existingSubscription?.CallbackUrl ?? currentSetting.OpportunitiesCallbackUrl;

            if (string.IsNullOrWhiteSpace(callbackUrl))
            {
                callbackUrl = existingSubscription?.CallbackUrl ?? currentSetting.OpportunitiesCallbackUrl;
            }

            var includeQuotas = GetJsonBoolean(root, "include_quotas") ?? existingSubscription?.IncludeQuotas ?? false;

            var surveysCount = root.TryGetProperty("surveys", out var surveysElement) && surveysElement.ValueKind == JsonValueKind.Array
                ? surveysElement.GetArrayLength()
                : 0;

            var supplierCode = root.TryGetProperty("name", out var nameElement) && nameElement.ValueKind == JsonValueKind.String
                ? nameElement.GetString()
                : existingSubscription?.SupplierCode ?? currentSetting.SupplierCode;

            if (string.IsNullOrWhiteSpace(supplierCode))
            {
                supplierCode = existingSubscription?.SupplierCode ?? currentSetting.SupplierCode;
            }

            var requestPayloadSnapshot = MergeOpportunitySubscriptionPayload(
                callbackUrl ?? existingSubscription?.CallbackUrl ?? currentSetting.OpportunitiesCallbackUrl ?? string.Empty,
                includeQuotas,
                root,
                existingSubscription?.RequestPayloadSnapshot);

            return new LucidMarketplaceSubscriptionDTO
            {
                SubscriptionType = "Opportunities",
                SupplierCode = supplierCode,
                CallbackUrl = callbackUrl,
                IncludeQuotas = includeQuotas,
                RemoteSubscriptionId = supplierCode,
                RequestPayloadSnapshot = requestPayloadSnapshot,
                LastStatus = surveysCount > 0
                    ? $"Remote subscription loaded with {surveysCount} active survey reference(s)."
                    : "Remote subscription loaded.",
                ResponsePayloadSnapshot = proxyResponse.ResponseBody,
                IsActive = proxyResponse.StatusCode.HasValue && proxyResponse.StatusCode.Value < 300,
                LastValidatedOn = DateTime.Now
            };
        }
        catch
        {
            return null;
        }
    }

    private async Task<string> ReadRequestBodyAsync()
    {
        Request.EnableBuffering();
        Request.Body.Position = 0;
        using var memoryStream = new MemoryStream();
        await Request.Body.CopyToAsync(memoryStream);
        var rawBytes = memoryStream.ToArray();
        Request.Body.Position = 0;
        return Encoding.UTF8.GetString(rawBytes);
    }

    private sealed class LucidMarketplaceEntryLinkResolution
    {
        public bool Success { get; init; }

        public string? Message { get; init; }

        public bool UsedFallback { get; init; }

        public string? InternalLaunchUrl { get; init; }

        public LucidMarketplaceLaunchContextDTO? Context { get; init; }

        public LucidMarketplaceEntryLinkDTO? EntryLink { get; init; }

        public static LucidMarketplaceEntryLinkResolution Fail(string message)
        {
            return new LucidMarketplaceEntryLinkResolution
            {
                Success = false,
                Message = message
            };
        }

        public static LucidMarketplaceEntryLinkResolution SuccessResult(
            LucidMarketplaceEntryLinkDTO entryLink,
            LucidMarketplaceLaunchContextDTO context,
            string internalLaunchUrl,
            bool usedFallback,
            string message)
        {
            return new LucidMarketplaceEntryLinkResolution
            {
                Success = true,
                Message = message,
                UsedFallback = usedFallback,
                InternalLaunchUrl = internalLaunchUrl,
                Context = context,
                EntryLink = entryLink
            };
        }
    }

    private sealed class EntryLinkProxyResponseSummary
    {
        public int? HttpStatusCode { get; set; }

        public string? Message { get; set; }

        public int? ApiResult { get; set; }

        public int? ApiResultCode { get; set; }

        public List<string> ApiMessages { get; set; } = new();

        public string? ApiMessagesPreview { get; set; }
    }
}
