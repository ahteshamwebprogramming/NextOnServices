using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Infrastructure.Models.APIProjects;

namespace NextOnServices.WebUI.VT.Services;

public sealed class ZampliaLaunchService : IZampliaLaunchService
{
    private const int MaxLogSnapshotLength = 4000;
    private const int MaxAttemptAuditLength = 2000;

    private readonly ILogger<ZampliaLaunchService> _logger;
    private readonly ZampliaAPIController _zampliaAPIController;
    private readonly SurveyAPIController _surveyAPIController;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ZampliaLaunchService(
        ILogger<ZampliaLaunchService> logger,
        ZampliaAPIController zampliaAPIController,
        SurveyAPIController surveyAPIController,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _zampliaAPIController = zampliaAPIController;
        _surveyAPIController = surveyAPIController;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<ZampliaLaunchResolution> TryResolveVendorLaunchAsync(ZampliaLaunchServiceRequest request)
    {
        if (request == null)
        {
            return BuildFailure("Zamplia launch request is required.");
        }

        var context = await _zampliaAPIController.GetZampliaLaunchContextAsync(
            zampliaSurveyId: request.ZampliaSurveyId,
            projectMappingSid: request.ProjectMappingSid);

        if (context == null || context.ZampliaSurveyId <= 0 || context.SurveyId <= 0)
        {
            return BuildFailure("Mapped Zamplia project context was not found.");
        }

        if (!string.Equals(context.ProjectFrom, "Zamplia", StringComparison.OrdinalIgnoreCase))
        {
            return new ZampliaLaunchResolution
            {
                IsZampliaProject = false,
                Success = false,
                Message = "The requested project is not a Zamplia project.",
                ZampliaSurveyId = context.ZampliaSurveyId,
                Context = context
            };
        }

        var supplierResolution = await ResolveSupplierProjectUidAsync(context, request);
        if (!supplierResolution.Success || string.IsNullOrWhiteSpace(supplierResolution.SupplierProjectUid))
        {
            await InsertFailureLogAsync(
                context,
                request,
                request.LaunchRequestUrl,
                JsonConvert.SerializeObject(new
                {
                    request.ZampliaSurveyId,
                    request.ProjectMappingSid,
                    request.SourceRespondentId,
                    request.SupplierProjectUid
                }),
                StatusCodes.Status500InternalServerError,
                supplierResolution.Message,
                supplierResolution.Message,
                null);

            return new ZampliaLaunchResolution
            {
                IsZampliaProject = true,
                Success = false,
                Message = supplierResolution.Message,
                ZampliaSurveyId = context.ZampliaSurveyId,
                SourceRespondentId = supplierResolution.SourceRespondentId,
                SupplierProjectUid = supplierResolution.SupplierProjectUid,
                CreatedSupplierProject = supplierResolution.CreatedSupplierProject,
                Context = context
            };
        }

        if (string.IsNullOrWhiteSpace(context.ApiKey) || string.IsNullOrWhiteSpace(context.BaseUrl))
        {
            const string message = "Zamplia settings are incomplete for launch generation.";
            await InsertFailureLogAsync(
                context,
                request,
                request.LaunchRequestUrl,
                JsonConvert.SerializeObject(new
                {
                    request.ZampliaSurveyId,
                    request.ProjectMappingSid,
                    supplierResolution.SupplierProjectUid
                }),
                StatusCodes.Status500InternalServerError,
                message,
                message,
                null);

            return new ZampliaLaunchResolution
            {
                IsZampliaProject = true,
                Success = false,
                Message = message,
                ZampliaSurveyId = context.ZampliaSurveyId,
                SourceRespondentId = supplierResolution.SourceRespondentId,
                SupplierProjectUid = supplierResolution.SupplierProjectUid,
                CreatedSupplierProject = supplierResolution.CreatedSupplierProject,
                Context = context
            };
        }

        var transactionId = supplierResolution.SupplierProjectUid.Trim();
        var savedAttempt = await UpsertAttemptAsync(context, request, supplierResolution, transactionId);
        if (savedAttempt == null || savedAttempt.Id <= 0)
        {
            const string message = "Unable to create the compact Zamplia launch audit row.";
            await InsertFailureLogAsync(
                context,
                request,
                request.LaunchRequestUrl,
                JsonConvert.SerializeObject(new
                {
                    context.ZampliaSurveyId,
                    context.SurveyId,
                    supplierResolution.SupplierProjectUid,
                    transactionId
                }),
                StatusCodes.Status500InternalServerError,
                message,
                message,
                null);

            return new ZampliaLaunchResolution
            {
                IsZampliaProject = true,
                Success = false,
                Message = message,
                TransactionId = transactionId,
                ZampliaSurveyId = context.ZampliaSurveyId,
                SupplierProjectUid = supplierResolution.SupplierProjectUid,
                SourceRespondentId = supplierResolution.SourceRespondentId,
                CreatedSupplierProject = supplierResolution.CreatedSupplierProject,
                Context = context
            };
        }

        var proxyRequest = new ZampliaProxyRequest
        {
            Setting = BuildSetting(context),
            SurveyId = context.SurveyId,
            IpAddress = request.ClientIp,
            TransactionId = transactionId
        };

        var proxyResponse = await CallConsultingsProxyAsync("GenerateLink", proxyRequest);
        if (!proxyResponse.Result || string.IsNullOrWhiteSpace(proxyResponse.ResponseBody) || !TryExtractVendorLink(proxyResponse.ResponseBody, out var vendorLaunchUrl))
        {
            savedAttempt.FinalStatus = "LaunchError";
            savedAttempt.FinalStatusSource = "ZampliaGenerateLink";
            savedAttempt.VendorLaunchUrl = null;
            savedAttempt.ModifiedBy = request.UserId;
            savedAttempt.RawJson = TrimForStorage(proxyResponse.Message ?? proxyResponse.ResponseBody, MaxAttemptAuditLength);
            await _zampliaAPIController.SaveZampliaRespondentAttempt(savedAttempt);

            var message = string.IsNullOrWhiteSpace(proxyResponse.Message)
                ? "Launch link response was successful, but no redirect URL could be extracted from the Zamplia payload."
                : proxyResponse.Message;

            await InsertFailureLogAsync(
                context,
                request,
                proxyResponse.RequestUrl ?? request.LaunchRequestUrl,
                JsonConvert.SerializeObject(new
                {
                    context.SurveyId,
                    supplierResolution.SupplierProjectUid,
                    transactionId,
                    attemptId = savedAttempt.Id
                }),
                proxyResponse.StatusCode ?? StatusCodes.Status500InternalServerError,
                message,
                proxyResponse.ResponseBody,
                savedAttempt.Id);

            return new ZampliaLaunchResolution
            {
                IsZampliaProject = true,
                Success = false,
                Message = message,
                TransactionId = transactionId,
                AttemptId = savedAttempt.Id,
                ZampliaSurveyId = context.ZampliaSurveyId,
                SupplierProjectUid = supplierResolution.SupplierProjectUid,
                SourceRespondentId = supplierResolution.SourceRespondentId,
                CreatedSupplierProject = supplierResolution.CreatedSupplierProject,
                Context = context
            };
        }

        return new ZampliaLaunchResolution
        {
            IsZampliaProject = true,
            Success = true,
            Message = "Zamplia launch link resolved.",
            VendorLaunchUrl = vendorLaunchUrl,
            TransactionId = transactionId,
            AttemptId = savedAttempt.Id,
            ZampliaSurveyId = context.ZampliaSurveyId,
            SupplierProjectUid = supplierResolution.SupplierProjectUid,
            SourceRespondentId = supplierResolution.SourceRespondentId,
            CreatedSupplierProject = supplierResolution.CreatedSupplierProject,
            Context = context
        };
    }

    private async Task<(bool Success, string? SupplierProjectUid, string? SourceRespondentId, bool CreatedSupplierProject, string? Message)> ResolveSupplierProjectUidAsync(
        ZampliaLaunchContextDTO context,
        ZampliaLaunchServiceRequest request)
    {
        var sourceRespondentId = request.SourceRespondentId?.Trim();
        var suppliedUid = request.SupplierProjectUid?.Trim();

        if (!string.IsNullOrWhiteSpace(suppliedUid))
        {
            var existingByUid = await _surveyAPIController.GetSupplierProjectByUidAsync(suppliedUid, context.ProjectMappingSid);
            if (existingByUid != null && !string.IsNullOrWhiteSpace(existingByUid.UID))
            {
                return (true, existingByUid.UID.Trim(), sourceRespondentId, false, null);
            }

            return (true, suppliedUid, sourceRespondentId, false, null);
        }

        if (!string.IsNullOrWhiteSpace(sourceRespondentId))
        {
            var existingBySource = await _surveyAPIController.GetSupplierProjectByUidAsync(sourceRespondentId, context.ProjectMappingSid);
            if (existingBySource != null && !string.IsNullOrWhiteSpace(existingBySource.UID))
            {
                return (true, existingBySource.UID.Trim(), sourceRespondentId, false, null);
            }
        }

        if (string.IsNullOrWhiteSpace(sourceRespondentId))
        {
            return (false, null, null, false, "A respondent identifier is required for Zamplia launch.");
        }

        if (string.IsNullOrWhiteSpace(context.ProjectMappingSid))
        {
            return (false, null, sourceRespondentId, false, "Project mapping SID is missing for Zamplia launch.");
        }

        var stableUid = BuildStableSupplierProjectUid(context.ProjectMappingSid, sourceRespondentId);
        var existingStable = await _surveyAPIController.GetSupplierProjectByUidAsync(stableUid, context.ProjectMappingSid);
        if (existingStable != null && !string.IsNullOrWhiteSpace(existingStable.UID))
        {
            return (true, existingStable.UID.Trim(), sourceRespondentId, false, null);
        }

        var projectMappingCode = string.IsNullOrWhiteSpace(context.ProjectMappingCode)
            ? request.ProjectMappingCode?.Trim()
            : context.ProjectMappingCode?.Trim();
        if (string.IsNullOrWhiteSpace(projectMappingCode))
        {
            return (false, null, sourceRespondentId, false, "Project mapping code is missing for Zamplia launch.");
        }

        var saveResult = await _surveyAPIController.SaveSupplierProject(
            request.ClientIp ?? string.Empty,
            request.ClientBrowser ?? string.Empty,
            "InComplete",
            context.ProjectMappingSid,
            projectMappingCode,
            stableUid,
            sourceRespondentId,
            request.ClientDevice ?? string.Empty,
            0,
            request.Enc ?? string.Empty);

        if (string.IsNullOrWhiteSpace(saveResult) || string.Equals(saveResult, "3", StringComparison.Ordinal))
        {
            return (false, null, sourceRespondentId, false, "Unable to create the legacy SupplierProjects tracking row.");
        }

        return (true, stableUid, sourceRespondentId, true, null);
    }

    private async Task<ZampliaRespondentAttemptDTO?> UpsertAttemptAsync(
        ZampliaLaunchContextDTO context,
        ZampliaLaunchServiceRequest request,
        (bool Success, string? SupplierProjectUid, string? SourceRespondentId, bool CreatedSupplierProject, string? Message) supplierResolution,
        string transactionId)
    {
        var current = await _zampliaAPIController.GetLatestZampliaRespondentAttemptAsync(
            transactionId,
            supplierResolution.SupplierProjectUid,
            context.ZampliaSurveyId);

        var attempt = current ?? new ZampliaRespondentAttemptDTO
        {
            ZampliaSurveyId = context.ZampliaSurveyId,
            SurveyId = context.SurveyId,
            CreatedBy = request.UserId
        };

        attempt.InternalProjectId = context.InternalProjectId;
        attempt.InternalProjectUrlId = context.InternalProjectUrlId;
        attempt.InternalProjectMappingId = context.InternalProjectMappingId;
        attempt.RespondentId = supplierResolution.SupplierProjectUid;
        attempt.TransactionId = transactionId;
        attempt.IpAddress = TrimForStorage(request.ClientIp, 100);
        attempt.LaunchUrl = TrimForStorage(request.LaunchRequestUrl, MaxLogSnapshotLength);
        attempt.AttemptedOn = DateTime.Now;
        attempt.VendorLaunchUrl = null;
        attempt.ReturnUrl = null;
        attempt.ReturnRawQuery = null;
        attempt.ReturnCode = null;
        attempt.ReturnStatus = null;
        attempt.FinalStatus = null;
        attempt.FinalStatusSource = null;
        attempt.IsCompleted = false;
        attempt.IsTerminated = false;
        attempt.IsOverQuota = false;
        attempt.IsQualityTermination = false;
        attempt.IsSecurityTermination = false;
        attempt.IsDuplicate = false;
        attempt.CompletedOn = null;
        attempt.RawJson = BuildCompactAttemptAudit(context, supplierResolution.SourceRespondentId, supplierResolution.CreatedSupplierProject);
        attempt.ModifiedBy = request.UserId;

        var saveAttemptResult = await _zampliaAPIController.SaveZampliaRespondentAttempt(attempt);
        if (saveAttemptResult is ObjectResult saveAttemptObject && saveAttemptObject.StatusCode == StatusCodes.Status200OK)
        {
            return saveAttemptObject.Value as ZampliaRespondentAttemptDTO;
        }

        return null;
    }

    private async Task<ZampliaProxyResponse> CallConsultingsProxyAsync(string actionPath, ZampliaProxyRequest payload)
    {
        var baseUrl = _configuration.GetValue<string>("ConsultingApiBaseUrl");
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return new ZampliaProxyResponse
            {
                Result = false,
                IsStub = true,
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Consultings proxy base URL is not configured."
            };
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
            return new ZampliaProxyResponse
            {
                Result = false,
                IsStub = true,
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                RequestUrl = url
            };
        }
    }

    private async Task InsertFailureLogAsync(
        ZampliaLaunchContextDTO context,
        ZampliaLaunchServiceRequest request,
        string? requestUrl,
        string? requestBody,
        int statusCode,
        string? errorText,
        string? responseBody,
        int? relatedEntityId)
    {
        await _zampliaAPIController.InsertLogAsync(new ZampliaSyncLogDTO
        {
            ModuleName = "Zamplia",
            ActionName = "LaunchRespondent",
            RequestUrl = TrimForStorage(requestUrl, 1000),
            RequestBodySnapshot = TrimForStorage(requestBody, MaxLogSnapshotLength),
            ResponseStatusCode = statusCode,
            ResponseBodySnapshot = TrimForStorage(responseBody, MaxLogSnapshotLength),
            Source = "launch",
            RelatedEntityId = relatedEntityId,
            RelatedSurveyId = context.SurveyId,
            IsSuccess = false,
            ErrorText = TrimForStorage(errorText, MaxLogSnapshotLength),
            StartedOn = DateTime.Now,
            CompletedOn = DateTime.Now,
            CreatedBy = request.UserId
        });
    }

    private static ZampliaSettingDTO BuildSetting(ZampliaLaunchContextDTO context) => new()
    {
        BaseUrl = context.BaseUrl,
        ApiKey = context.ApiKey,
        ExitHmacKey = context.ExitHmacKey,
        UseConsultingsBridge = context.UseConsultingsBridge,
        IsActive = true
    };

    private static ZampliaLaunchResolution BuildFailure(string message) => new()
    {
        IsZampliaProject = false,
        Success = false,
        Message = message
    };

    private static string BuildStableSupplierProjectUid(string sid, string respondentId)
    {
        var raw = $"{sid.Trim()}|{respondentId.Trim()}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(hash)[..32];
    }

    private static string? BuildCompactAttemptAudit(ZampliaLaunchContextDTO context, string? sourceRespondentId, bool createdSupplierProject)
    {
        var payload = JsonConvert.SerializeObject(new
        {
            context.ProjectFrom,
            context.ProjectMappingSid,
            sourceRespondentId,
            createdSupplierProject,
            transactionIdSource = "SupplierProjects.UID"
        });

        return TrimForStorage(payload, MaxAttemptAuditLength);
    }

    private static string? TrimForStorage(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value) || maxLength <= 0)
        {
            return string.IsNullOrWhiteSpace(value) ? value : string.Empty;
        }

        var trimmed = value.Trim();
        if (trimmed.Length <= maxLength)
        {
            return trimmed;
        }

        return trimmed[..(maxLength - 3)] + "...";
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

        foreach (var propertyName in new[] { "link", "Link", "url", "Url", "launchLink", "LaunchLink", "surveyLink", "SurveyLink", "generatedLink", "GeneratedLink" })
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
        if (string.IsNullOrWhiteSpace(candidate))
        {
            return false;
        }

        if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri))
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
}

