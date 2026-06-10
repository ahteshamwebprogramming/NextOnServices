using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Infrastructure.Models.APIProjects;

namespace NextOnServices.WebUI.VT.Controllers;

[Area("VT")]
[Authorize]
[Route("VT/[controller]")]
public class TorfacMarketplaceApiController : Controller
{
    private const int MaxResponsePreviewLength = 200000;
    private const string TorfacVendorKey = "Torfac";

    private readonly ILogger<TorfacMarketplaceApiController> _logger;
    private readonly TorfacMarketplaceAPIController _torfacMarketplaceAPIController;
    private readonly ProjectsAPIController _projectsAPIController;
    private readonly IHttpClientFactory _httpClientFactory;

    public TorfacMarketplaceApiController(
        ILogger<TorfacMarketplaceApiController> logger,
        TorfacMarketplaceAPIController torfacMarketplaceAPIController,
        ProjectsAPIController projectsAPIController,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _torfacMarketplaceAPIController = torfacMarketplaceAPIController;
        _projectsAPIController = projectsAPIController;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("SaveSettings")]
    public async Task<IActionResult> SaveSettings([FromBody] TorfacMarketplaceSettingDTO inputData)
    {
        if (inputData == null)
        {
            return BadRequest(new { result = false, message = "Settings payload is required." });
        }

        var userId = GetCurrentUserId();
        inputData.CreatedBy ??= userId;
        inputData.ModifiedBy = userId;

        var apiResult = await _torfacMarketplaceAPIController.SaveTorfacMarketplaceSetting(inputData);
        return ConvertApiResult(apiResult);
    }

    [HttpPost("GetSurveys")]
    public async Task<IActionResult> GetSurveys()
    {
        var setting = await GetCurrentSettingAsync();
        if (setting == null || string.IsNullOrWhiteSpace(setting.SurveysUrl))
        {
            return Json(new TorfacMarketplaceSurveyFetchResultDTO
            {
                Result = false,
                Message = "Save the Torfac Marketplace survey URL before fetching surveys."
            });
        }

        if (string.IsNullOrWhiteSpace(setting.SecretKey))
        {
            return Json(new TorfacMarketplaceSurveyFetchResultDTO
            {
                Result = false,
                SourceUrl = setting.SurveysUrl,
                Message = "Save the Torfac Marketplace secret key before fetching surveys."
            });
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(60);
            using var request = BuildTorfacRequest(HttpMethod.Get, setting.SurveysUrl.Trim(), setting.SecretKey.Trim());

            using var response = await client.SendAsync(request);
            var rawResponse = await response.Content.ReadAsStringAsync();
            BuildSurveyFetchArtifacts(
                rawResponse,
                out var surveyCount,
                out var collectionPath,
                out var columns,
                out var rows,
                out var preview,
                out var responseTruncated);
            await EnrichRowsWithProjectStateAsync(rows);

            return Json(new TorfacMarketplaceSurveyFetchResultDTO
            {
                Result = response.IsSuccessStatusCode,
                Message = response.IsSuccessStatusCode
                    ? rows.Count > 0
                        ? $"Fetched Torfac Marketplace surveys successfully. Loaded {rows.Count} survey record(s) into the table."
                        : surveyCount.HasValue
                            ? "Fetched Torfac Marketplace successfully, but the survey collection is empty."
                            : "Fetched Torfac Marketplace response successfully, but no survey list was detected."
                    : $"Remote endpoint returned HTTP {(int)response.StatusCode}.",
                SourceUrl = setting.SurveysUrl,
                StatusCode = (int)response.StatusCode,
                ContentType = response.Content.Headers.ContentType?.MediaType,
                SurveyCount = surveyCount,
                CollectionPath = collectionPath,
                RawResponse = preview,
                ResponseTruncated = responseTruncated,
                Columns = columns,
                Rows = rows
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Torfac Marketplace survey fetch failed.");
            return Json(new TorfacMarketplaceSurveyFetchResultDTO
            {
                Result = false,
                SourceUrl = setting.SurveysUrl,
                Message = ex.Message
            });
        }
    }

    [HttpGet("GetQuotaList")]
    public async Task<IActionResult> GetQuotaList(string surveyId)
    {
        if (string.IsNullOrWhiteSpace(surveyId))
        {
            return BadRequest(new { result = false, message = "Survey ID is required." });
        }

        var setting = await GetCurrentSettingAsync();
        if (setting == null || string.IsNullOrWhiteSpace(setting.SurveysUrl))
        {
            return Json(new TorfacMarketplaceQuotaFetchResultDTO
            {
                Result = false,
                Message = "Save the Torfac Marketplace survey URL before fetching quota list."
            });
        }

        if (string.IsNullOrWhiteSpace(setting.SecretKey))
        {
            return Json(new TorfacMarketplaceQuotaFetchResultDTO
            {
                Result = false,
                Message = "Save the Torfac Marketplace secret key before fetching quota list."
            });
        }

        var quotaUrl = BuildQuotaListUrl(setting.SurveysUrl.Trim(), surveyId.Trim());

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(60);
            using var request = BuildTorfacRequest(HttpMethod.Get, quotaUrl, setting.SecretKey.Trim());

            using var response = await client.SendAsync(request);
            var rawResponse = await response.Content.ReadAsStringAsync();
            BuildQuotaFetchArtifacts(
                rawResponse,
                out var quotaCount,
                out var rows,
                out var preview,
                out var responseTruncated);

            return Json(new TorfacMarketplaceQuotaFetchResultDTO
            {
                Result = response.IsSuccessStatusCode,
                Message = response.IsSuccessStatusCode
                    ? rows.Count > 0
                        ? $"Fetched Torfac quota list successfully. Loaded {rows.Count} quota record(s)."
                        : quotaCount.HasValue
                            ? "Fetched Torfac quota list successfully, but no quota rows were returned."
                            : "Fetched Torfac quota list successfully, but no quota collection was detected."
                    : $"Remote endpoint returned HTTP {(int)response.StatusCode}.",
                SourceUrl = quotaUrl,
                StatusCode = (int)response.StatusCode,
                ContentType = response.Content.Headers.ContentType?.MediaType,
                QuotaCount = quotaCount,
                RawResponse = preview,
                ResponseTruncated = responseTruncated,
                Rows = rows
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Torfac Marketplace quota fetch failed for survey {SurveyId}.", surveyId);
            return Json(new TorfacMarketplaceQuotaFetchResultDTO
            {
                Result = false,
                SourceUrl = quotaUrl,
                Message = ex.Message
            });
        }
    }

    [HttpGet("ResolveProject")]
    public async Task<IActionResult> ResolveProject(string surveyId)
    {
        if (string.IsNullOrWhiteSpace(surveyId))
        {
            return BadRequest(new { result = false, message = "Survey ID is required." });
        }

        var internalProjectId = await GetInternalProjectIdAsync(surveyId.Trim());
        if (internalProjectId <= 0)
        {
            return Json(new
            {
                result = false,
                message = "No VT project has been added for this Torfac survey yet."
            });
        }

        return Json(new
        {
            result = true,
            projectId = internalProjectId,
            redirectUrl = $"/VT/Home/ProjectPage?projectId={internalProjectId}"
        });
    }

    private async Task<TorfacMarketplaceSettingDTO?> GetCurrentSettingAsync()
    {
        var result = await _torfacMarketplaceAPIController.GetTorfacMarketplaceSetting();
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as TorfacMarketplaceSettingDTO
            : null;
    }

    private static void ApplySecretHeaders(HttpRequestMessage request, string secretKey)
    {
        request.Headers.TryAddWithoutValidation("x-api-key", secretKey);
    }

    private static HttpRequestMessage BuildTorfacRequest(HttpMethod method, string url, string secretKey)
    {
        var request = new HttpRequestMessage(method, url);
        request.Version = new Version(2, 0);
        request.VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
        ApplySecretHeaders(request, secretKey);
        return request;
    }

    private static string BuildQuotaListUrl(string surveysUrl, string surveyId)
    {
        const string surveysEndpoint = "/getallocatedsurveys";
        var normalizedSurveyId = Uri.EscapeDataString(surveyId.Trim());

        if (surveysUrl.EndsWith(surveysEndpoint, StringComparison.OrdinalIgnoreCase))
        {
            return surveysUrl[..^surveysEndpoint.Length] + $"/quota-list/{normalizedSurveyId}";
        }

        var configuredUri = new Uri(surveysUrl, UriKind.Absolute);
        return new Uri(configuredUri, $"/api/v1/supplier-api/quota-list/{normalizedSurveyId}").ToString();
    }

    private static void BuildSurveyFetchArtifacts(
        string? rawResponse,
        out int? surveyCount,
        out string? collectionPath,
        out List<string> columns,
        out List<Dictionary<string, string?>> rows,
        out string preview,
        out bool responseTruncated)
    {
        surveyCount = null;
        collectionPath = null;
        columns = new List<string>();
        rows = new List<Dictionary<string, string?>>();
        preview = string.Empty;
        responseTruncated = false;

        if (string.IsNullOrWhiteSpace(rawResponse))
        {
            return;
        }

        try
        {
            using var document = JsonDocument.Parse(rawResponse);
            var collectionLookup = TryLocateSurveyCollection(document.RootElement, "$", 0);
            if (collectionLookup.Collection.HasValue)
            {
                surveyCount = collectionLookup.Collection.Value.GetArrayLength();
                collectionPath = collectionLookup.CollectionPath;
                (columns, rows) = BuildSurveyTable(collectionLookup.Collection.Value);
            }

            var prettyJson = JsonSerializer.Serialize(document.RootElement, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            preview = TrimResponse(prettyJson, out responseTruncated);
        }
        catch
        {
            preview = TrimResponse(rawResponse, out responseTruncated);
        }
    }

    private static void BuildQuotaFetchArtifacts(
        string? rawResponse,
        out int? quotaCount,
        out List<Dictionary<string, string?>> rows,
        out string preview,
        out bool responseTruncated)
    {
        quotaCount = null;
        rows = new List<Dictionary<string, string?>>();
        preview = string.Empty;
        responseTruncated = false;

        if (string.IsNullOrWhiteSpace(rawResponse))
        {
            return;
        }

        try
        {
            using var document = JsonDocument.Parse(rawResponse);
            var collectionLookup = TryLocateSurveyCollection(document.RootElement, "$", 0);
            if (collectionLookup.Collection.HasValue)
            {
                quotaCount = collectionLookup.Collection.Value.GetArrayLength();
                rows = BuildQuotaRows(collectionLookup.Collection.Value);
            }

            var prettyJson = JsonSerializer.Serialize(document.RootElement, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            preview = TrimResponse(prettyJson, out responseTruncated);
        }
        catch
        {
            preview = TrimResponse(rawResponse, out responseTruncated);
        }
    }

    private static (JsonElement? Collection, string? CollectionPath) TryLocateSurveyCollection(JsonElement element, string path, int depth)
    {
        if (depth > 4)
        {
            return (null, null);
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            return (element.Clone(), path);
        }

        if (element.ValueKind != JsonValueKind.Object)
        {
            return (null, null);
        }

        foreach (var propertyName in new[] { "surveys", "data", "items", "results" })
        {
            if (TryGetPropertyIgnoreCase(element, propertyName, out var arrayElement) &&
                arrayElement.ValueKind == JsonValueKind.Array)
            {
                return (arrayElement.Clone(), $"{path}.{propertyName}");
            }
        }

        foreach (var property in element.EnumerateObject())
        {
            if (property.Value.ValueKind != JsonValueKind.Object && property.Value.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            var nested = TryLocateSurveyCollection(property.Value, $"{path}.{property.Name}", depth + 1);
            if (nested.Collection.HasValue)
            {
                return nested;
            }
        }

        return (null, null);
    }

    private static (List<string> Columns, List<Dictionary<string, string?>> Rows) BuildSurveyTable(JsonElement collection)
    {
        var columns = new List<string>
        {
            "surveyId",
            "name",
            "loi",
            "ir",
            "cpi",
            "countryName",
            "deviceType",
            "actions"
        };
        var rows = new List<Dictionary<string, string?>>();

        if (collection.ValueKind != JsonValueKind.Array)
        {
            return (columns, rows);
        }

        foreach (var item in collection.EnumerateArray())
        {
            rows.Add(BuildNormalizedSurveyRow(item));
        }

        return (columns, rows);
    }

    private static List<Dictionary<string, string?>> BuildQuotaRows(JsonElement collection)
    {
        var rows = new List<Dictionary<string, string?>>();
        if (collection.ValueKind != JsonValueKind.Array)
        {
            return rows;
        }

        foreach (var item in collection.EnumerateArray())
        {
            rows.Add(BuildQuotaRow(item));
        }

        return rows;
    }

    private static Dictionary<string, string?> BuildQuotaRow(JsonElement item)
    {
        var row = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        if (item.ValueKind != JsonValueKind.Object)
        {
            var fallbackValue = ConvertElementToCellValue(item);
            row["quotaId"] = fallbackValue;
            row["target"] = string.Empty;
            row["details"] = fallbackValue;
            row["rawPayload"] = item.GetRawText();
            return row;
        }

        row["quotaId"] = FindPreferredValue(item, "id", "quotaid", "quota_id");
        row["target"] = FindPreferredValue(item, "target");
        row["details"] = SummarizeQuotaDetails(item);
        row["rawPayload"] = item.GetRawText();
        return row;
    }

    private static Dictionary<string, string?> BuildNormalizedSurveyRow(JsonElement item)
    {
        var row = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        if (item.ValueKind != JsonValueKind.Object)
        {
            var fallbackValue = ConvertElementToCellValue(item);
            row["surveyId"] = fallbackValue;
            row["name"] = fallbackValue;
            row["loi"] = string.Empty;
            row["ir"] = string.Empty;
            row["cpi"] = string.Empty;
            row["countryName"] = string.Empty;
            row["deviceType"] = string.Empty;
            row["liveLink"] = string.Empty;
            row["totalRemaining"] = string.Empty;
            row["internalProjectId"] = string.Empty;
            row["hasProject"] = "false";
            row["rawPayload"] = item.GetRawText();
            return row;
        }

        row["surveyId"] = FindPreferredValue(item, "surveyid", "survey_id", "id", "projectid", "project_id", "surveycode", "survey_code");
        row["name"] = FindPreferredValue(item, "name", "surveyname", "survey_name", "title", "surveytitle", "survey_title", "projectname", "project_name");
        row["loi"] = FindPreferredValue(item, "loi", "lengthofinterview", "length_of_interview", "estimatedloi", "estimated_loi");
        row["ir"] = FindPreferredValue(item, "ir", "incidencerate", "incidence_rate", "incidence");
        row["cpi"] = FindPreferredValue(item, "cpi", "costperinterview", "cost_per_interview", "payout", "reward");
        row["countryName"] = FindPreferredValue(item, "countryname", "country_name", "country", "countries", "market", "geo");
        row["deviceType"] = FindPreferredValue(item, "devicetype", "device_type", "device", "devicetypes", "device_types", "platform");
        row["liveLink"] = FindPreferredValue(item, "livelink", "live_link", "surveyurl", "survey_url", "url", "link", "entrylink", "entry_link");
        row["totalRemaining"] = FindPreferredValue(item, "totalremaining", "total_remaining", "remaining", "quota", "needed", "remainingcompletes", "remaining_completes");
        row["internalProjectId"] = string.Empty;
        row["hasProject"] = "false";
        row["rawPayload"] = item.GetRawText();

        if (string.IsNullOrWhiteSpace(row["name"]))
        {
            row["name"] = row["surveyId"];
        }

        return row;
    }

    private static bool TryGetPropertyIgnoreCase(JsonElement element, string name, out JsonElement value)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    private static string TrimResponse(string rawResponse, out bool responseTruncated)
    {
        if (rawResponse.Length <= MaxResponsePreviewLength)
        {
            responseTruncated = false;
            return rawResponse;
        }

        responseTruncated = true;
        return rawResponse[..MaxResponsePreviewLength] + Environment.NewLine + Environment.NewLine + "...response truncated...";
    }

    private static string? ConvertElementToCellValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Null => string.Empty,
            JsonValueKind.Undefined => string.Empty,
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Object => ExtractObjectSummary(value),
            JsonValueKind.Array => ExtractArraySummary(value),
            _ => value.ToString()
        };
    }

    private static string TrimCellValue(string value)
    {
        const int maxCellLength = 300;
        return value.Length <= maxCellLength ? value : value[..maxCellLength] + "...";
    }

    private static string? FindPreferredValue(JsonElement item, params string[] aliases)
    {
        return FindPreferredValue(item, aliases, 0);
    }

    private static string? FindPreferredValue(JsonElement item, string[] aliases, int depth)
    {
        if (depth > 5)
        {
            return null;
        }

        if (item.ValueKind == JsonValueKind.Object)
        {
            foreach (var alias in aliases)
            {
                if (TryGetPropertyIgnoreCase(item, alias, out var directValue))
                {
                    var candidate = ConvertElementToCellValue(directValue);
                    if (!string.IsNullOrWhiteSpace(candidate))
                    {
                        return candidate;
                    }
                }
            }

            foreach (var property in item.EnumerateObject())
            {
                if (property.Value.ValueKind != JsonValueKind.Object && property.Value.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                var nestedValue = FindPreferredValue(property.Value, aliases, depth + 1);
                if (!string.IsNullOrWhiteSpace(nestedValue))
                {
                    return nestedValue;
                }
            }
        }
        else if (item.ValueKind == JsonValueKind.Array)
        {
            foreach (var arrayItem in item.EnumerateArray())
            {
                var nestedValue = FindPreferredValue(arrayItem, aliases, depth + 1);
                if (!string.IsNullOrWhiteSpace(nestedValue))
                {
                    return nestedValue;
                }
            }
        }

        return null;
    }

    private static string SummarizeQuotaDetails(JsonElement value)
    {
        if (value.ValueKind != JsonValueKind.Object)
        {
            return ConvertElementToCellValue(value) ?? string.Empty;
        }

        var parts = new List<string>();
        foreach (var property in value.EnumerateObject())
        {
            if (string.Equals(property.Name, "id", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var candidate = ConvertElementToCellValue(property.Value);
            if (!string.IsNullOrWhiteSpace(candidate))
            {
                parts.Add($"{property.Name}: {candidate}");
            }
        }

        return parts.Count > 0
            ? TrimCellValue(string.Join(Environment.NewLine, parts))
            : TrimCellValue(value.GetRawText());
    }

    private static string ExtractArraySummary(JsonElement value)
    {
        var parts = new List<string>();
        foreach (var item in value.EnumerateArray())
        {
            var candidate = ConvertElementToCellValue(item);
            if (!string.IsNullOrWhiteSpace(candidate) && !parts.Contains(candidate))
            {
                parts.Add(candidate);
            }
        }

        if (parts.Count == 0)
        {
            return TrimCellValue(value.GetRawText());
        }

        return TrimCellValue(string.Join(", ", parts));
    }

    private static string ExtractObjectSummary(JsonElement value)
    {
        foreach (var preferredProperty in new[] { "name", "label", "value", "country", "countryname", "type", "devicetype", "device_type", "title" })
        {
            if (TryGetPropertyIgnoreCase(value, preferredProperty, out var propertyValue))
            {
                var candidate = ConvertElementToCellValue(propertyValue);
                if (!string.IsNullOrWhiteSpace(candidate))
                {
                    return candidate;
                }
            }
        }

        return TrimCellValue(value.GetRawText());
    }

    private async Task EnrichRowsWithProjectStateAsync(List<Dictionary<string, string?>> rows)
    {
        foreach (var row in rows)
        {
            var surveyId = GetRowValue(row, "surveyId");
            if (string.IsNullOrWhiteSpace(surveyId))
            {
                row["internalProjectId"] = string.Empty;
                row["hasProject"] = "false";
                continue;
            }

            var internalProjectId = await GetInternalProjectIdAsync(surveyId.Trim());
            row["internalProjectId"] = internalProjectId > 0 ? internalProjectId.ToString() : string.Empty;
            row["hasProject"] = internalProjectId > 0 ? "true" : "false";
        }
    }

    private async Task<int> GetInternalProjectIdAsync(string surveyId)
    {
        var result = await _projectsAPIController.GetProjectByProjectIdFromAPI(surveyId, TorfacVendorKey);
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK && objectResult.Value != null
            ? Convert.ToInt32(objectResult.Value)
            : 0;
    }

    private static string? GetRowValue(IReadOnlyDictionary<string, string?> row, string key)
    {
        foreach (var pair in row)
        {
            if (string.Equals(pair.Key, key, StringComparison.OrdinalIgnoreCase))
            {
                return pair.Value;
            }
        }

        return null;
    }

    private int? GetCurrentUserId() => HttpContext.Session.GetInt32("UserId");

    private IActionResult ConvertApiResult(IActionResult apiResult) => apiResult switch
    {
        ObjectResult objectResult => StatusCode(objectResult.StatusCode ?? StatusCodes.Status200OK, objectResult.Value),
        StatusCodeResult statusCodeResult => StatusCode(statusCodeResult.StatusCode),
        JsonResult jsonResult => jsonResult,
        ContentResult contentResult => Content(contentResult.Content ?? string.Empty, contentResult.ContentType ?? "text/plain"),
        _ => apiResult
    };
}
