using System.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using NextOnServices.Infrastructure.Models.APIProjects;

namespace NextOnServices.WebUI.VT.Services;

/// <summary>
/// Calls Zamplia Supply APIs directly from NextOn (no Consultings bridge).
/// </summary>
public class ZampliaClient : IZampliaClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ZampliaClient> _logger;

    public ZampliaClient(IHttpClientFactory httpClientFactory, ILogger<ZampliaClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public Task<ZampliaProxyResponse> ExecuteAsync(string actionPath, ZampliaProxyRequest request)
    {
        return actionPath switch
        {
            "ValidateSettings" => Task.FromResult(ValidateSettings(request)),
            "TestConnectivity" => ExecutePathsAsync(request, new[] { "/Surveys/GetAllocatedSurveys" }, "Zamplia connectivity test completed."),
            "GetAllocatedSurveys" => ExecutePathsAsync(request, new[] { "/Surveys/GetAllocatedSurveys" }, "Allocated surveys loaded."),
            "GetSurveyById" => ExecutePathsAsync(
                request,
                new[]
                {
                    QueryPath("/Surveys/GetSurveyById", ("SurveyId", request?.SurveyId?.ToString())),
                    QueryPath("/Surveys/GetSurveyById", ("surveyId", request?.SurveyId?.ToString())),
                    QueryPath("/Surveys/GetSurveyById", ("Id", request?.SurveyId?.ToString()))
                },
                "Survey details loaded."),
            "GetSurveyQualifications" => ExecutePathsAsync(
                request,
                new[]
                {
                    QueryPath("/Surveys/GetSurveyQualifications", ("SurveyId", request?.SurveyId?.ToString())),
                    QueryPath("/Surveys/GetSurveyQualifications", ("surveyId", request?.SurveyId?.ToString())),
                    QueryPath("/Surveys/GetSurveyQualifications", ("Id", request?.SurveyId?.ToString()))
                },
                "Survey qualifications loaded."),
            "GetSurveyQuotas" => ExecutePathsAsync(
                request,
                new[]
                {
                    QueryPath("/Surveys/GetSurveyQuotas", ("SurveyId", request?.SurveyId?.ToString())),
                    QueryPath("/Surveys/GetSurveyQuotas", ("surveyId", request?.SurveyId?.ToString())),
                    QueryPath("/Surveys/GetSurveyQuotas", ("Id", request?.SurveyId?.ToString()))
                },
                "Survey quotas loaded."),
            "GetDemoGraphics" => ExecutePathsAsync(
                request,
                new[] { QueryPath("/Attributes/GetDemoGraphics", ("LanguageId", request?.LanguageId?.ToString())) },
                "Demographics loaded."),
            "GenerateLink" => ExecutePathsAsync(
                request,
                new[]
                {
                    QueryPath("/Surveys/GenerateLink",
                        ("SurveyId", request?.SurveyId?.ToString()),
                        ("IpAddress", request?.IpAddress),
                        ("TransactionId", request?.TransactionId))
                },
                "Launch link generated."),
            "GetReconciliation" => ExecutePathsAsync(
                request,
                new[] { QueryPath("/Reconciliation/GetReconciliation", request?.QueryParameters) },
                "Reconciliation payload loaded."),
            "GetUsersSessionEvents" => ExecutePathsAsync(
                request,
                new[]
                {
                    QueryPath("/Surveys/getUsersSessionEvents",
                        ("SurveyId", request?.SurveyId?.ToString()),
                        ("TransactionId", request?.TransactionId))
                },
                "Session events loaded."),
            _ => Task.FromResult(new ZampliaProxyResponse
            {
                Result = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = $"Unsupported Zamplia action: {actionPath}"
            })
        };
    }

    private static ZampliaProxyResponse ValidateSettings(ZampliaProxyRequest? request)
    {
        var errors = GetValidationErrors(request?.Setting);
        return new ZampliaProxyResponse
        {
            Result = errors.Count == 0,
            StatusCode = errors.Count == 0 ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest,
            Message = errors.Count == 0 ? "Zamplia settings payload is valid." : string.Join(" ", errors)
        };
    }

    private async Task<ZampliaProxyResponse> ExecutePathsAsync(
        ZampliaProxyRequest? request,
        IEnumerable<string> relativePaths,
        string successMessage)
    {
        var errors = GetValidationErrors(request?.Setting);
        if (errors.Count > 0)
        {
            return new ZampliaProxyResponse
            {
                Result = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = string.Join(" ", errors)
            };
        }

        ZampliaProxyResponse? lastResponse = null;
        foreach (var relativePath in ExpandRelativePaths(request!.Setting!, relativePaths).Where(path => !string.IsNullOrWhiteSpace(path)))
        {
            lastResponse = await SendAsync(request.Setting!, relativePath);
            if (lastResponse.Result)
            {
                lastResponse.Message ??= successMessage;
                return lastResponse;
            }
        }

        lastResponse ??= new ZampliaProxyResponse
        {
            Result = false,
            StatusCode = StatusCodes.Status400BadRequest,
            Message = "Unable to build a valid Zamplia request."
        };
        lastResponse.Message ??= "Zamplia request failed.";
        return lastResponse;
    }

    private async Task<ZampliaProxyResponse> SendAsync(ZampliaSettingDTO setting, string relativePath)
    {
        var url = BuildAbsoluteUrl(setting.BaseUrl, relativePath);
        try
        {
            var client = _httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.TryAddWithoutValidation("ZAMP-KEY", setting.ApiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.CacheControl = CacheControlHeaderValue.Parse("no-cache");

            _logger.LogInformation("Zamplia direct request. Url={Url}", url);
            var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogInformation(
                "Zamplia direct response. Url={Url}, StatusCode={StatusCode}, BodyLength={BodyLength}",
                url,
                (int)response.StatusCode,
                body?.Length ?? 0);

            return new ZampliaProxyResponse
            {
                Result = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                RequestUrl = url,
                ResponseBody = body,
                Message = response.IsSuccessStatusCode
                    ? null
                    : $"Zamplia request failed with status {(int)response.StatusCode}."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Zamplia direct call failed. Url={Url}", url);
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

    private static List<string> GetValidationErrors(ZampliaSettingDTO? setting)
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

    private static IEnumerable<string> ExpandRelativePaths(ZampliaSettingDTO setting, IEnumerable<string> relativePaths)
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
}
