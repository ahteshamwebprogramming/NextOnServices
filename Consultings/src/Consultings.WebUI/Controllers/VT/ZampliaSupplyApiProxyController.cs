using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Consultings.WebUI.Controllers.VT;

[ApiController]
[Route("VT/ZampliaSupplyProxy")]
[Produces("application/json")]
public class ZampliaSupplyApiProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ZampliaSupplyApiProxyController> _logger;

    public ZampliaSupplyApiProxyController(
        IHttpClientFactory httpClientFactory,
        ILogger<ZampliaSupplyApiProxyController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpPost("ValidateSettings")]
    public IActionResult ValidateSettings([FromBody] ZampliaProxyRequest? request)
    {
        var errors = GetValidationErrors(request?.Setting);
        return Ok(new ZampliaProxyResponse
        {
            Result = errors.Count == 0,
            StatusCode = errors.Count == 0 ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest,
            Message = errors.Count == 0 ? "Zamplia settings payload is valid for proxy calls." : string.Join(" ", errors)
        });
    }

    [HttpPost("TestConnectivity")]
    public Task<IActionResult> TestConnectivity([FromBody] ZampliaProxyRequest? request) =>
        ExecuteAsync(request, new[] { "/Surveys/GetAllocatedSurveys" }, "Zamplia connectivity test completed.");

    [HttpPost("GetAllocatedSurveys")]
    public Task<IActionResult> GetAllocatedSurveys([FromBody] ZampliaProxyRequest? request) =>
        ExecuteAsync(request, new[] { "/Surveys/GetAllocatedSurveys" }, "Allocated surveys loaded.");

    [HttpPost("GetSurveyById")]
    public Task<IActionResult> GetSurveyById([FromBody] ZampliaProxyRequest? request) =>
        ExecuteAsync(
            request,
            new[]
            {
                QueryPath("/Surveys/GetSurveyById", ("SurveyId", request?.SurveyId?.ToString())),
                QueryPath("/Surveys/GetSurveyById", ("surveyId", request?.SurveyId?.ToString())),
                QueryPath("/Surveys/GetSurveyById", ("Id", request?.SurveyId?.ToString()))
            },
            "Survey details loaded.");

    [HttpPost("GetSurveyQualifications")]
    public Task<IActionResult> GetSurveyQualifications([FromBody] ZampliaProxyRequest? request) =>
        ExecuteAsync(
            request,
            new[]
            {
                QueryPath("/Surveys/GetSurveyQualifications", ("SurveyId", request?.SurveyId?.ToString())),
                QueryPath("/Surveys/GetSurveyQualifications", ("surveyId", request?.SurveyId?.ToString())),
                QueryPath("/Surveys/GetSurveyQualifications", ("Id", request?.SurveyId?.ToString()))
            },
            "Survey qualifications loaded.");

    [HttpPost("GetSurveyQuotas")]
    public Task<IActionResult> GetSurveyQuotas([FromBody] ZampliaProxyRequest? request) =>
        ExecuteAsync(
            request,
            new[]
            {
                QueryPath("/Surveys/GetSurveyQuotas", ("SurveyId", request?.SurveyId?.ToString())),
                QueryPath("/Surveys/GetSurveyQuotas", ("surveyId", request?.SurveyId?.ToString())),
                QueryPath("/Surveys/GetSurveyQuotas", ("Id", request?.SurveyId?.ToString()))
            },
            "Survey quotas loaded.");

    [HttpPost("GetDemoGraphics")]
    public Task<IActionResult> GetDemoGraphics([FromBody] ZampliaProxyRequest? request) =>
        ExecuteAsync(request, new[] { QueryPath("/Attributes/GetDemoGraphics", ("LanguageId", request?.LanguageId?.ToString())) }, "Demographics loaded.");

    [HttpPost("GenerateLink")]
    public Task<IActionResult> GenerateLink([FromBody] ZampliaProxyRequest? request) =>
        ExecuteAsync(
            request,
            new[]
            {
                QueryPath("/Surveys/GenerateLink",
                    ("SurveyId", request?.SurveyId?.ToString()),
                    ("IpAddress", request?.IpAddress),
                    ("TransactionId", request?.TransactionId))
            },
            "Launch link generated.");

    [HttpPost("GetReconciliation")]
    public Task<IActionResult> GetReconciliation([FromBody] ZampliaProxyRequest? request) =>
        ExecuteAsync(request, new[] { QueryPath("/Reconciliation/GetReconciliation", request?.QueryParameters) }, "Reconciliation payload loaded.");

    [HttpPost("GetUsersSessionEvents")]
    public Task<IActionResult> GetUsersSessionEvents([FromBody] ZampliaProxyRequest? request) =>
        ExecuteAsync(
            request,
            new[]
            {
                QueryPath("/Surveys/getUsersSessionEvents",
                    ("SurveyId", request?.SurveyId?.ToString()),
                    ("TransactionId", request?.TransactionId))
            },
            "Session events loaded.");

    private async Task<IActionResult> ExecuteAsync(ZampliaProxyRequest? request, IEnumerable<string> relativePaths, string successMessage)
    {
        var errors = GetValidationErrors(request?.Setting);
        if (errors.Count > 0)
        {
            return Ok(new ZampliaProxyResponse
            {
                Result = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = string.Join(" ", errors)
            });
        }

        ZampliaProxyResponse? lastResponse = null;
        foreach (var relativePath in ExpandRelativePaths(request!.Setting!, relativePaths).Where(path => !string.IsNullOrWhiteSpace(path)))
        {
            lastResponse = await SendAsync(request!.Setting!, relativePath);
            if (lastResponse.Result)
            {
                lastResponse.Message ??= successMessage;
                return Ok(lastResponse);
            }
        }

        lastResponse ??= new ZampliaProxyResponse
        {
            Result = false,
            StatusCode = StatusCodes.Status400BadRequest,
            Message = "Unable to build a valid Zamplia request."
        };

        lastResponse.Message ??= "Zamplia proxy request failed.";
        return Ok(lastResponse);
    }

    private async Task<ZampliaProxyResponse> SendAsync(ZampliaProxySetting setting, string relativePath)
    {
        var url = BuildAbsoluteUrl(setting.BaseUrl, relativePath);
        try
        {
            var client = _httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.TryAddWithoutValidation("ZAMP-KEY", setting.ApiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.CacheControl = CacheControlHeaderValue.Parse("no-cache");

            _logger.LogInformation("Zamplia proxy sending request. Url={Url}", url);
            var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Zamplia proxy received response. Url={Url}, StatusCode={StatusCode}, Body={Body}", url, (int)response.StatusCode, body);

            return new ZampliaProxyResponse
            {
                Result = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                RequestUrl = url,
                ResponseBody = body,
                Message = response.IsSuccessStatusCode ? null : $"Zamplia request failed with status {(int)response.StatusCode}."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Zamplia proxy call failed. Url={Url}", url);
            return new ZampliaProxyResponse
            {
                Result = false,
                StatusCode = StatusCodes.Status500InternalServerError,
                RequestUrl = url,
                ResponseBody = ex.ToString(),
                Message = ex.Message
            };
        }
    }

    private static List<string> GetValidationErrors(ZampliaProxySetting? setting)
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

        return errors;
    }

    private static string QueryPath(string path, params (string Key, string? Value)[] queryPairs)
    {
        return QueryPath(path, queryPairs.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase));
    }

    private static string QueryPath(string path, Dictionary<string, string?>? queryParameters)
    {
        var filtered = (queryParameters ?? new Dictionary<string, string?>())
            .Where(item => !string.IsNullOrWhiteSpace(item.Key) && !string.IsNullOrWhiteSpace(item.Value))
            .ToDictionary(item => item.Key, item => item.Value!, StringComparer.OrdinalIgnoreCase);

        return filtered.Count == 0 ? path : QueryHelpers.AddQueryString(path, filtered);
    }

    private static IEnumerable<string> ExpandRelativePaths(ZampliaProxySetting setting, IEnumerable<string> relativePaths)
    {
        var yielded = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var relativePath in relativePaths.Where(path => !string.IsNullOrWhiteSpace(path)))
        {
            var normalizedPath = NormalizeRelativePath(relativePath);
            foreach (var candidatePath in GetCandidatePaths(setting.BaseUrl, normalizedPath))
            {
                if (yielded.Add(candidatePath))
                {
                    yield return candidatePath;
                }
            }
        }
    }

    private static IEnumerable<string> GetCandidatePaths(string? baseUrl, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            yield break;
        }

        var normalizedPath = NormalizeRelativePath(relativePath);
        var trimmedPath = normalizedPath.TrimStart('/');
        if (trimmedPath.StartsWith("api/", StringComparison.OrdinalIgnoreCase))
        {
            yield return normalizedPath;
            yield break;
        }

        if (!Uri.TryCreate((baseUrl ?? string.Empty).Trim(), UriKind.Absolute, out var baseUri))
        {
            yield return NormalizeRelativePath($"/api/v1/{trimmedPath}");
            yield return NormalizeRelativePath($"/api/{trimmedPath}");
            yield return normalizedPath;
            yield break;
        }

        var basePath = (baseUri.AbsolutePath ?? string.Empty).Trim('/');
        if (string.IsNullOrWhiteSpace(basePath))
        {
            yield return NormalizeRelativePath($"/api/v1/{trimmedPath}");
            yield return NormalizeRelativePath($"/api/{trimmedPath}");
            yield return normalizedPath;
            yield break;
        }

        yield return normalizedPath;
    }

    private static string NormalizeRelativePath(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return "/";
        }

        return relativePath.StartsWith("/", StringComparison.Ordinal) ? relativePath : $"/{relativePath}";
    }

    private static string BuildAbsoluteUrl(string? baseUrl, string relativePath)
    {
        var normalizedBaseUrl = (baseUrl ?? string.Empty).Trim();
        var normalizedRelativePath = NormalizeRelativePath(relativePath).TrimStart('/');
        if (!Uri.TryCreate($"{normalizedBaseUrl.TrimEnd('/')}/", UriKind.Absolute, out var baseUri))
        {
            return $"{normalizedBaseUrl.TrimEnd('/')}/{normalizedRelativePath}";
        }

        return new Uri(baseUri, normalizedRelativePath).ToString();
    }

    public class ZampliaProxyRequest
    {
        public ZampliaProxySetting? Setting { get; set; }
        public long? SurveyId { get; set; }
        public int? LanguageId { get; set; }
        public string? IpAddress { get; set; }
        public string? TransactionId { get; set; }
        public Dictionary<string, string?> QueryParameters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }

    public class ZampliaProxySetting
    {
        public string? BaseUrl { get; set; }
        public string? ApiKey { get; set; }
    }

    public class ZampliaProxyResponse
    {
        public bool Result { get; set; }
        public bool IsStub { get; set; }
        public int? StatusCode { get; set; }
        public string? Message { get; set; }
        public string? RequestUrl { get; set; }
        public string? ResponseBody { get; set; }
    }
}
