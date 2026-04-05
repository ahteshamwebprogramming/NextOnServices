using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Consultings.WebUI.Controllers.VT;

[ApiController]
[Route("VT/LucidMarketplaceProxy")]
[Produces("application/json")]
public class LucidMarketplaceApiProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LucidMarketplaceApiProxyController> _logger;

    public LucidMarketplaceApiProxyController(
        IHttpClientFactory httpClientFactory,
        ILogger<LucidMarketplaceApiProxyController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpPost("ValidateSettings")]
    public IActionResult ValidateSettings([FromBody] LucidMarketplaceProxyRequest? request)
    {
        var validationErrors = GetValidationErrors(request?.Setting);
        if (validationErrors.Any())
        {
            return Ok(new LucidMarketplaceProxyResponse
            {
                Result = false,
                IsStub = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = string.Join(" ", validationErrors)
            });
        }

        return Ok(new LucidMarketplaceProxyResponse
        {
            Result = true,
            IsStub = false,
            StatusCode = StatusCodes.Status200OK,
            Message = "Lucid Marketplace settings payload is valid for Consultings proxy calls."
        });
    }

    [HttpPost("TestConnectivity")]
    public async Task<IActionResult> TestConnectivity([FromBody] LucidMarketplaceProxyRequest? request)
    {
        var validationErrors = GetValidationErrors(request?.Setting);
        if (validationErrors.Any())
        {
            return Ok(new LucidMarketplaceProxyResponse
            {
                Result = false,
                IsStub = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = string.Join(" ", validationErrors)
            });
        }

        var url = BuildSubscriptionUrl(request!.Setting!, null, includeSurveys: true);
        var response = await SendRequestAsync(HttpMethod.Get, url, null, request.Setting!.ApiKey, treatNotFoundAsSuccess: true);
        response.Message ??= response.Result
            ? "Lucid Marketplace opportunities endpoint is reachable."
            : "Lucid Marketplace connectivity test failed.";

        return Ok(response);
    }

    [HttpPost("CreateSubscription")]
    public async Task<IActionResult> CreateSubscription([FromBody] LucidMarketplaceProxyRequest? request)
    {
        var validationErrors = GetValidationErrors(request?.Setting);
        if (validationErrors.Any())
        {
            return Ok(new LucidMarketplaceProxyResponse
            {
                Result = false,
                IsStub = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = string.Join(" ", validationErrors)
            });
        }

        if (request?.Subscription == null || string.IsNullOrWhiteSpace(request.Subscription.SubscriptionType))
        {
            return Ok(new LucidMarketplaceProxyResponse
            {
                Result = false,
                IsStub = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "SubscriptionType is required."
            });
        }

        var subscriptionType = NormalizeSubscriptionType(request.Subscription.SubscriptionType);
        if (string.IsNullOrWhiteSpace(subscriptionType))
        {
            return Ok(new LucidMarketplaceProxyResponse
            {
                Result = false,
                IsStub = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Unsupported Lucid Marketplace subscription type."
            });
        }

        var payload = BuildCreatePayload(request.Subscription, subscriptionType);
        if (payload == null)
        {
            return Ok(new LucidMarketplaceProxyResponse
            {
                Result = false,
                IsStub = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "A valid subscription payload is required."
            });
        }

        var url = BuildSubscriptionUrl(request.Setting!, request.Subscription, includeSurveys: false);
        var response = await SendRequestAsync(HttpMethod.Post, url, payload, request.Setting!.ApiKey);
        response.RemoteSubscriptionId ??= request.Setting.SupplierCode;
        response.Message ??= response.Result
            ? subscriptionType == "RespondentOutcomes"
                ? "Lucid Marketplace respondent outcomes subscription request completed."
                : "Lucid Marketplace opportunities subscription request completed."
            : subscriptionType == "RespondentOutcomes"
                ? "Lucid Marketplace respondent outcomes subscription request failed."
                : "Lucid Marketplace opportunities subscription request failed.";

        return Ok(response);
    }

    [HttpPost("GetSubscriptions")]
    public async Task<IActionResult> GetSubscriptions([FromBody] LucidMarketplaceProxyRequest? request)
    {
        var validationErrors = GetValidationErrors(request?.Setting);
        if (validationErrors.Any())
        {
            return Ok(new LucidMarketplaceProxyResponse
            {
                Result = false,
                IsStub = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = string.Join(" ", validationErrors)
            });
        }

        var subscriptionType = NormalizeSubscriptionType(request?.Subscription?.SubscriptionType) ?? "Opportunities";
        var url = BuildSubscriptionUrl(request!.Setting!, request.Subscription, includeSurveys: subscriptionType == "Opportunities");
        var response = await SendRequestAsync(HttpMethod.Get, url, null, request.Setting!.ApiKey, treatNotFoundAsSuccess: true);
        response.RemoteSubscriptionId ??= request.Setting.SupplierCode;
        response.Message ??= response.Result
            ? subscriptionType == "RespondentOutcomes"
                ? "Lucid Marketplace respondent outcomes subscription details loaded."
                : "Lucid Marketplace opportunities subscription details loaded."
            : subscriptionType == "RespondentOutcomes"
                ? "Lucid Marketplace respondent outcomes subscription lookup failed."
                : "Lucid Marketplace opportunities subscription lookup failed.";

        return Ok(response);
    }

    [HttpPost("DeleteSubscription")]
    public async Task<IActionResult> DeleteSubscription([FromBody] LucidMarketplaceProxyRequest? request)
    {
        var validationErrors = GetValidationErrors(request?.Setting);
        if (validationErrors.Any())
        {
            return Ok(new LucidMarketplaceProxyResponse
            {
                Result = false,
                IsStub = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = string.Join(" ", validationErrors)
            });
        }

        var subscriptionType = NormalizeSubscriptionType(request?.Subscription?.SubscriptionType) ?? "Opportunities";
        var url = BuildSubscriptionUrl(request!.Setting!, request.Subscription, includeSurveys: false);
        var response = await SendRequestAsync(HttpMethod.Delete, url, null, request.Setting!.ApiKey, treatNotFoundAsSuccess: true);
        response.RemoteSubscriptionId ??= request.Setting.SupplierCode;
        response.Message ??= response.Result
            ? subscriptionType == "RespondentOutcomes"
                ? "Lucid Marketplace respondent outcomes subscription removed."
                : "Lucid Marketplace opportunities subscription removed."
            : subscriptionType == "RespondentOutcomes"
                ? "Lucid Marketplace respondent outcomes subscription delete failed."
                : "Lucid Marketplace opportunities subscription delete failed.";

        return Ok(response);
    }

    [HttpPost("CreateSupplierLink")]
    public async Task<IActionResult> CreateSupplierLink([FromBody] LucidMarketplaceProxyRequest? request)
    {
        var validationErrors = GetValidationErrors(request?.Setting);
        validationErrors.AddRange(GetEntryLinkValidationErrors(request?.EntryLink));
        if (validationErrors.Any())
        {
            return Ok(new LucidMarketplaceProxyResponse
            {
                Result = false,
                IsStub = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = string.Join(" ", validationErrors)
            });
        }

        var url = BuildSupplierLinkUrl(request!.Setting!, request.EntryLink!, "Create");
        var payload = BuildSupplierLinkPayload(request.EntryLink!);
        var response = await SendRequestAsync(HttpMethod.Post, url, payload, request.Setting!.ApiKey);
        response.Message ??= response.Result
            ? "Lucid Marketplace supplier link created."
            : "Lucid Marketplace supplier link create failed.";

        return Ok(response);
    }

    [HttpPost("GetSupplierLink")]
    public async Task<IActionResult> GetSupplierLink([FromBody] LucidMarketplaceProxyRequest? request)
    {
        var validationErrors = GetValidationErrors(request?.Setting);
        validationErrors.AddRange(GetEntryLinkValidationErrors(request?.EntryLink));
        if (validationErrors.Any())
        {
            return Ok(new LucidMarketplaceProxyResponse
            {
                Result = false,
                IsStub = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = string.Join(" ", validationErrors)
            });
        }

        var url = BuildSupplierShowLinkUrl(request!.Setting!, request.EntryLink!);
        var response = await SendRequestAsync(HttpMethod.Get, url, null, request.Setting!.ApiKey, treatNotFoundAsSuccess: false);
        response.Message ??= response.Result
            ? "Lucid Marketplace supplier link details loaded."
            : "Lucid Marketplace supplier link lookup failed.";

        return Ok(response);
    }

    [HttpPost("UpdateSupplierLink")]
    public async Task<IActionResult> UpdateSupplierLink([FromBody] LucidMarketplaceProxyRequest? request)
    {
        var validationErrors = GetValidationErrors(request?.Setting);
        validationErrors.AddRange(GetEntryLinkValidationErrors(request?.EntryLink));
        if (validationErrors.Any())
        {
            return Ok(new LucidMarketplaceProxyResponse
            {
                Result = false,
                IsStub = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = string.Join(" ", validationErrors)
            });
        }

        var url = BuildSupplierLinkUrl(request!.Setting!, request.EntryLink!, "Update");
        var payload = BuildSupplierLinkPayload(request.EntryLink!);
        var response = await SendRequestAsync(HttpMethod.Put, url, payload, request.Setting!.ApiKey);
        response.Message ??= response.Result
            ? "Lucid Marketplace supplier link updated."
            : "Lucid Marketplace supplier link update failed.";

        return Ok(response);
    }

    private async Task<LucidMarketplaceProxyResponse> SendRequestAsync(
        HttpMethod method,
        string url,
        string? body,
        string? apiKey,
        bool treatNotFoundAsSuccess = false)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(method, url);
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                request.Headers.TryAddWithoutValidation("Authorization", apiKey);
            }
            request.Headers.CacheControl = CacheControlHeaderValue.Parse("no-cache");

            if (!string.IsNullOrWhiteSpace(body))
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var httpSuccess = response.IsSuccessStatusCode || (treatNotFoundAsSuccess && response.StatusCode == HttpStatusCode.NotFound);
            var success = httpSuccess && !HasBusinessLevelFailure(content);

            return new LucidMarketplaceProxyResponse
            {
                Result = success,
                IsStub = false,
                StatusCode = (int)response.StatusCode,
                RequestUrl = url,
                ResponseBody = content,
                Message = success
                    ? null
                    : ExtractErrorMessage(content, response.StatusCode)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lucid Marketplace proxy call failed. Method={Method}, Url={Url}", method, url);
            return new LucidMarketplaceProxyResponse
            {
                Result = false,
                IsStub = false,
                StatusCode = StatusCodes.Status500InternalServerError,
                RequestUrl = url,
                Message = ex.Message,
                ResponseBody = ex.ToString()
            };
        }
    }

    private static List<string> GetValidationErrors(LucidMarketplaceProxySetting? setting)
    {
        var errors = new List<string>();
        if (setting == null)
        {
            errors.Add("Settings payload is required.");
            return errors;
        }

        if (string.IsNullOrWhiteSpace(setting.BaseUrl))
        {
            errors.Add("Base URL is required.");
        }

        if (string.IsNullOrWhiteSpace(setting.ApiKey))
        {
            errors.Add("API Key is required.");
        }

        if (string.IsNullOrWhiteSpace(setting.SupplierCode))
        {
            errors.Add("Supplier Code is required.");
        }

        return errors;
    }

    private static List<string> GetEntryLinkValidationErrors(LucidMarketplaceProxyEntryLink? entryLink)
    {
        var errors = new List<string>();
        if (entryLink == null)
        {
            errors.Add("Entry link payload is required.");
            return errors;
        }

        if (entryLink.SurveyNumber <= 0)
        {
            errors.Add("SurveyNumber is required.");
        }

        if (string.IsNullOrWhiteSpace(entryLink.SupplierCode))
        {
            errors.Add("SupplierCode is required for supplier-link requests.");
        }

        return errors;
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

    private static string BuildSubscriptionUrl(
        LucidMarketplaceProxySetting setting,
        LucidMarketplaceProxySubscription? subscription,
        bool includeSurveys)
    {
        var subscriptionType = NormalizeSubscriptionType(subscription?.SubscriptionType) ?? "Opportunities";
        var baseUrl = setting.BaseUrl?.Trim().TrimEnd('/') ?? string.Empty;
        if (subscriptionType == "RespondentOutcomes")
        {
            if (!baseUrl.Contains("/supply/respondent-outcomes/v2", StringComparison.OrdinalIgnoreCase))
            {
                baseUrl = $"{GetApiRoot(baseUrl)}/supply/respondent-outcomes/v2";
            }
        }
        else if (!baseUrl.Contains("/supply/opportunities/v1", StringComparison.OrdinalIgnoreCase))
        {
            baseUrl = $"{baseUrl}/supply/opportunities/v1";
        }

        var url = $"{baseUrl}/subscriptions/{Uri.EscapeDataString(setting.SupplierCode ?? string.Empty)}";
        if (subscriptionType == "Opportunities" && includeSurveys)
        {
            url += "?surveys=true";
        }

        return url;
    }

    private static string BuildSupplierLinkUrl(
        LucidMarketplaceProxySetting setting,
        LucidMarketplaceProxyEntryLink entryLink,
        string action)
    {
        var apiRoot = GetApiRoot(setting.BaseUrl);
        var supplierCode = Uri.EscapeDataString(entryLink.SupplierCode ?? setting.SupplierCode ?? string.Empty);
        var surveyNumber = entryLink.SurveyNumber.ToString();

        return action switch
        {
            "Create" => $"{apiRoot}/Supply/v1/SupplierLinks/Create/{surveyNumber}/{supplierCode}",
            "Update" => $"{apiRoot}/Supply/v1/SupplierLinks/Update/{surveyNumber}/{supplierCode}",
            _ => $"{apiRoot}/Supply/v1/SupplierLinks/Delete/{surveyNumber}/{supplierCode}"
        };
    }

    private static string BuildSupplierShowLinkUrl(
        LucidMarketplaceProxySetting setting,
        LucidMarketplaceProxyEntryLink entryLink)
    {
        var apiRoot = GetApiRoot(setting.BaseUrl);
        var supplierCode = Uri.EscapeDataString(entryLink.SupplierCode ?? setting.SupplierCode ?? string.Empty);
        var surveyNumber = entryLink.SurveyNumber.ToString();
        return $"{apiRoot}/Supply/v1/SupplierLinks/BySurveyNumber/{surveyNumber}/{supplierCode}";
    }

    private static string GetApiRoot(string? configuredBaseUrl)
    {
        var baseUrl = configuredBaseUrl?.Trim().TrimEnd('/') ?? string.Empty;
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return string.Empty;
        }

        var supplyIndex = baseUrl.IndexOf("/supply/", StringComparison.OrdinalIgnoreCase);
        if (supplyIndex >= 0)
        {
            return baseUrl.Substring(0, supplyIndex);
        }

        return baseUrl;
    }

    private static string? BuildCreatePayload(LucidMarketplaceProxySubscription subscription, string subscriptionType)
    {
        if (!string.IsNullOrWhiteSpace(subscription.RequestPayloadSnapshot))
        {
            try
            {
                using var _ = JsonDocument.Parse(subscription.RequestPayloadSnapshot);
                return subscription.RequestPayloadSnapshot;
            }
            catch
            {
                return null;
            }
        }

        if (string.IsNullOrWhiteSpace(subscription.CallbackUrl))
        {
            return null;
        }

        if (subscriptionType == "RespondentOutcomes")
        {
            return JsonSerializer.Serialize(new
            {
                callback = subscription.CallbackUrl,
                outcomes = new object[] { new { } }
            });
        }

        return JsonSerializer.Serialize(new
        {
            callback = subscription.CallbackUrl,
            include_quotas = subscription.IncludeQuotas
        });
    }

    private static string BuildSupplierLinkPayload(LucidMarketplaceProxyEntryLink entryLink)
    {
        return JsonSerializer.Serialize(new
        {
            SupplierLinkTypeCode = string.IsNullOrWhiteSpace(entryLink.SupplierLinkTypeCode) ? "TS" : entryLink.SupplierLinkTypeCode,
            TrackingTypeCode = string.IsNullOrWhiteSpace(entryLink.TrackingTypeCode) ? "NONE" : entryLink.TrackingTypeCode,
            DefaultLink = entryLink.DefaultLink,
            SuccessLink = entryLink.SuccessLink,
            FailureLink = entryLink.FailureLink,
            OverQuotaLink = entryLink.OverQuotaLink,
            QualityTerminationLink = entryLink.QualityTerminationLink
        });
    }

    private static bool TryGetJsonProperty(JsonElement element, out JsonElement value, params string[] propertyNames)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (propertyNames.Any(name => string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase)))
                {
                    value = property.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    private static string? GetJsonString(JsonElement element, params string[] propertyNames)
    {
        return TryGetJsonProperty(element, out var value, propertyNames) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
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
            int.TryParse(value.GetString(), out intValue))
        {
            return intValue;
        }

        return null;
    }

    private static string ExtractErrorMessage(string? responseBody, HttpStatusCode statusCode)
    {
        if (!string.IsNullOrWhiteSpace(responseBody))
        {
            var apiSummary = TryExtractApiMessageSummary(responseBody);
            if (!string.IsNullOrWhiteSpace(apiSummary))
            {
                return apiSummary;
            }

            return responseBody;
        }

        return $"Lucid Marketplace request failed with status {(int)statusCode}.";
    }

    private static bool HasBusinessLevelFailure(string? responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;
            var apiResult = GetJsonInt(root, "ApiResult");
            var apiResultCode = GetJsonInt(root, "ApiResultCode");

            return (apiResult.HasValue && apiResult.Value != 0) ||
                   (apiResultCode.HasValue && apiResultCode.Value != 0);
        }
        catch
        {
            return false;
        }
    }

    private static string? TryExtractApiMessageSummary(string responseBody)
    {
        try
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;
            if (!TryGetJsonProperty(root, out var apiMessagesElement, "ApiMessages") ||
                apiMessagesElement.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            var messages = new List<string>();
            foreach (var item in apiMessagesElement.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.String)
                {
                    continue;
                }

                var message = item.GetString();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    messages.Add(message);
                }

                if (messages.Count >= 3)
                {
                    break;
                }
            }

            return messages.Count == 0 ? null : string.Join(" | ", messages);
        }
        catch
        {
            return null;
        }
    }

    public class LucidMarketplaceProxyRequest
    {
        public LucidMarketplaceProxySetting? Setting { get; set; }

        public LucidMarketplaceProxySubscription? Subscription { get; set; }

        public LucidMarketplaceProxyEntryLink? EntryLink { get; set; }
    }

    public class LucidMarketplaceProxySetting
    {
        public string? BaseUrl { get; set; }

        public string? ApiKey { get; set; }

        public string? SupplierCode { get; set; }

        public bool UseConsultingsBridge { get; set; }
    }

    public class LucidMarketplaceProxySubscription
    {
        public string? SubscriptionType { get; set; }

        public string? CallbackUrl { get; set; }

        public string? RemoteSubscriptionId { get; set; }

        public bool IncludeQuotas { get; set; }

        public string? RequestPayloadSnapshot { get; set; }
    }

    public class LucidMarketplaceProxyEntryLink
    {
        public int SurveyNumber { get; set; }

        public string? SupplierCode { get; set; }

        public string? SupplierLinkTypeCode { get; set; }

        public string? TrackingTypeCode { get; set; }

        public string? DefaultLink { get; set; }

        public string? SuccessLink { get; set; }

        public string? FailureLink { get; set; }

        public string? OverQuotaLink { get; set; }

        public string? QualityTerminationLink { get; set; }
    }

    public class LucidMarketplaceProxyResponse
    {
        public bool Result { get; set; }

        public bool IsStub { get; set; }

        public int? StatusCode { get; set; }

        public string? Message { get; set; }

        public string? RemoteSubscriptionId { get; set; }

        public string? RequestUrl { get; set; }

        public string? ResponseBody { get; set; }
    }
}
