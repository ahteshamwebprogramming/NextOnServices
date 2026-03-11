using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Collections.Generic;

namespace Consultings.WebUI.Controllers.VT;

/// <summary>
/// Proxies real Lucid (Samplicio.us) Offerwall API calls for NextOnServices Surveys (Lucid) module.
/// NextOnServices calls this when ConsultingApiBaseUrl is set; otherwise it may call Lucid directly.
/// </summary>
[ApiController]
[Route("VT/ApiProjects")]
[Produces("application/json")]
public class LucidOfferwallApiProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LucidOfferwallApiProxyController> _logger;

    public LucidOfferwallApiProxyController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<LucidOfferwallApiProxyController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    private string BaseUrl => _configuration["LucidApi:BaseUrl"] ?? "https://api.samplicio.us/Supply/v1";
    private string LookupBaseUrl => _configuration["LucidApi:LookupBaseUrl"] ?? "https://api.samplicio.us/Lookup/v1";
    private string SupplierCode => _configuration["LucidApi:SupplierCode"] ?? "";
    private string ApiKey => _configuration["LucidApi:ApiKey"] ?? "";

    [HttpGet("GetLucidIndustriesLookup")]
    public async Task<IActionResult> GetLucidIndustriesLookup()
    {
        var url = $"{LookupBaseUrl.TrimEnd('/')}/BasicLookups/BundledLookups/Industries";
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", ApiKey);
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Content(content, "application/json");
            }

            _logger.LogError("Lucid Industries lookup API failed. StatusCode={StatusCode}, Url={Url}", (int)response.StatusCode, url);
            return StatusCode((int)response.StatusCode, new
            {
                error = true,
                context = "GetLucidIndustriesLookup",
                message = "Failed to fetch industries from Lucid",
                statusCode = (int)response.StatusCode
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lucid Industries lookup API call failed. Url={Url}", url);
            return StatusCode(500, new
            {
                error = true,
                context = "GetLucidIndustriesLookup",
                message = ex.Message
            });
        }
    }

    [HttpGet("GetLucidBundledLookups")]
    public async Task<IActionResult> GetLucidBundledLookups()
    {
        var url = $"{LookupBaseUrl.TrimEnd('/')}/BasicLookups/BundledLookups/CountryLanguages";
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", ApiKey);
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Content(content, "application/json");
            }

            _logger.LogError("Lucid BundledLookups API failed. StatusCode={StatusCode}, Url={Url}", (int)response.StatusCode, url);
            return StatusCode((int)response.StatusCode, new
            {
                error = true,
                context = "GetLucidBundledLookups",
                message = "Failed to fetch bundled lookups from Lucid",
                statusCode = (int)response.StatusCode
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lucid BundledLookups API call failed. Url={Url}", url);
            return StatusCode(500, new
            {
                error = true,
                context = "GetLucidBundledLookups",
                message = ex.Message
            });
        }
    }

    [HttpGet("GetLucidStudyTypesLookup")]
    public async Task<IActionResult> GetLucidStudyTypesLookup()
    {
        var url = $"{LookupBaseUrl.TrimEnd('/')}/BasicLookups/BundledLookups/StudyTypes";
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", ApiKey);
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Content(content, "application/json");
            }

            _logger.LogError("Lucid StudyTypes lookup API failed. StatusCode={StatusCode}, Url={Url}", (int)response.StatusCode, url);
            return StatusCode((int)response.StatusCode, new
            {
                error = true,
                context = "GetLucidStudyTypesLookup",
                message = "Failed to fetch study types from Lucid",
                statusCode = (int)response.StatusCode
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lucid StudyTypes lookup API call failed. Url={Url}", url);
            return StatusCode(500, new
            {
                error = true,
                context = "GetLucidStudyTypesLookup",
                message = ex.Message
            });
        }
    }

    [HttpGet("GetRealLucidSurveys")]
    public async Task<IActionResult> GetRealLucidSurveys()
    {
        var url = $"{BaseUrl.TrimEnd('/')}/Surveys/AllOfferwall{Request.QueryString}";
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", ApiKey);
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Content(content, "application/json");
            }

            _logger.LogError("Lucid Offerwall API failed. StatusCode={StatusCode}, Url={Url}", (int)response.StatusCode, url);
            return StatusCode((int)response.StatusCode, new
            {
                error = true,
                context = "GetRealLucidSurveys",
                message = "Failed to fetch surveys from Lucid Offerwall",
                statusCode = (int)response.StatusCode
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lucid Offerwall API call failed. Url={Url}", url);
            return StatusCode(500, new
            {
                error = true,
                context = "GetRealLucidSurveys",
                message = ex.Message
            });
        }
    }

    [HttpGet("GetLucidQualificationsForOfferwall")]
    public async Task<IActionResult> GetLucidQualificationsForOfferwall(int surveyNumber, int countryLanguageId)
    {
        var questionsResult = new List<LucidQualificationQuestionDto>();

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", ApiKey);
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

            // 1) Get survey qualifications (question IDs + precodes)
            var qualUrl = $"{BaseUrl.TrimEnd('/')}/SurveyQualifications/BySurveyNumberForOfferwall/{surveyNumber}";
            var qualResponse = await client.GetAsync(qualUrl);
            var qualContent = await qualResponse.Content.ReadAsStringAsync();
            if (!qualResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)qualResponse.StatusCode, qualContent);
            }

            using var qualDoc = JsonDocument.Parse(qualContent);
            if (!qualDoc.RootElement.TryGetProperty("SurveyQualification", out var surveyQual) ||
                !surveyQual.TryGetProperty("Questions", out var questionsEl) ||
                questionsEl.ValueKind != JsonValueKind.Array)
            {
                return Ok(questionsResult);
            }

            foreach (var qEl in questionsEl.EnumerateArray())
            {
                if (!qEl.TryGetProperty("QuestionID", out var qIdProp) || qIdProp.ValueKind != JsonValueKind.Number)
                    continue;

                int questionId = qIdProp.GetInt32();
                var preCodes = new HashSet<string>();
                if (qEl.TryGetProperty("PreCodes", out var preCodesEl) && preCodesEl.ValueKind == JsonValueKind.Array)
                {
                    foreach (var pc in preCodesEl.EnumerateArray())
                    {
                        if (pc.ValueKind == JsonValueKind.String)
                            preCodes.Add(pc.GetString() ?? "");
                    }
                }

                // 2) Get question text/type
                string questionText = "";
                string questionType = "";
                var qInfoUrl = $"{LookupBaseUrl.TrimEnd('/')}/QuestionLibrary/QuestionById/{countryLanguageId}/{questionId}";
                var qInfoResp = await client.GetAsync(qInfoUrl);
                var qInfoContent = await qInfoResp.Content.ReadAsStringAsync();
                if (qInfoResp.IsSuccessStatusCode)
                {
                    using var qInfoDoc = JsonDocument.Parse(qInfoContent);
                    if (qInfoDoc.RootElement.TryGetProperty("Question", out var qObj))
                    {
                        if (qObj.TryGetProperty("QuestionText", out var qtProp) && qtProp.ValueKind == JsonValueKind.String)
                            questionText = qtProp.GetString() ?? "";
                        if (qObj.TryGetProperty("QuestionType", out var qt2Prop) && qt2Prop.ValueKind == JsonValueKind.String)
                            questionType = qt2Prop.GetString() ?? "";
                    }
                }

                // 3) Get all options and filter by pre-codes
                var allowedOptions = new List<string>();
                var optUrl = $"{LookupBaseUrl.TrimEnd('/')}/QuestionLibrary/AllQuestionOptions/{countryLanguageId}/{questionId}";
                var optResp = await client.GetAsync(optUrl);
                var optContent = await optResp.Content.ReadAsStringAsync();
                if (optResp.IsSuccessStatusCode)
                {
                    using var optDoc = JsonDocument.Parse(optContent);
                    if (optDoc.RootElement.TryGetProperty("QuestionOptions", out var optsEl) &&
                        optsEl.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var o in optsEl.EnumerateArray())
                        {
                            string pre = "";
                            string text = "";
                            if (o.TryGetProperty("Precode", out var preProp) && preProp.ValueKind == JsonValueKind.String)
                                pre = preProp.GetString() ?? "";
                            if (o.TryGetProperty("OptionText", out var txtProp) && txtProp.ValueKind == JsonValueKind.String)
                                text = txtProp.GetString() ?? "";

                            if (preCodes.Count == 0 || preCodes.Contains(pre))
                            {
                                if (!string.IsNullOrWhiteSpace(text))
                                    allowedOptions.Add(text);
                            }
                        }
                    }
                }

                questionsResult.Add(new LucidQualificationQuestionDto
                {
                    QuestionId = questionId,
                    QuestionText = questionText,
                    QuestionType = questionType,
                    AllowedOptions = allowedOptions
                });
            }

            return Ok(questionsResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Lucid qualifications. SurveyNumber={SurveyNumber}, CountryLanguageId={CountryLanguageId}", surveyNumber, countryLanguageId);
            return StatusCode(500, new { error = true, message = ex.Message });
        }
    }
}

public class LucidQualificationQuestionDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = "";
    public string QuestionType { get; set; } = "";
    public List<string> AllowedOptions { get; set; } = new List<string>();
}


