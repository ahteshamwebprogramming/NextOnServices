using Microsoft.AspNetCore.Mvc;

namespace Consultings.WebUI.Controllers.VT;

/// <summary>
/// Proxies Sago (Sample Cube) API calls for NextOnServices Surveys Sago module.
/// NextOnServices calls this when ConsultingApiBaseUrl is set; otherwise it calls Sago directly.
/// </summary>
[ApiController]
[Route("VT/ApiProjects")]
[Produces("application/json")]
public class SagoApiProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SagoApiProxyController> _logger;

    public SagoApiProxyController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<SagoApiProxyController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    private string BaseUrl => _configuration["SagoApi:BaseUrl"] ?? "https://api.sample-cube.com/api";
    private string SupplierId => _configuration["SagoApi:SupplierId"] ?? "";
    private string ApiKey => _configuration["SagoApi:ApiKey"] ?? "";

    private async Task<IActionResult> CallLucidAsync(string pathSuffix, string context)
    {
        var url = $"{BaseUrl.TrimEnd('/')}/{pathSuffix}";
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return Content(content, "application/json");
            }
            _logger.LogError("Sago API failed. Context={Context}, StatusCode={StatusCode}", context, (int)response.StatusCode);
            return StatusCode((int)response.StatusCode, new { error = true, context, message = "Failed to fetch from Sago", statusCode = (int)response.StatusCode });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sago API call failed. Context={Context}", context);
            return StatusCode(500, new { error = true, context, message = ex.Message });
        }
    }

    [HttpGet("GetLucidSurveys")]
    public Task<IActionResult> GetLucidSurveys() =>
        CallLucidAsync($"Survey/GetSupplierAllocatedSurveys/{SupplierId}/{ApiKey}", "GetLucidSurveys");

    [HttpGet("GetLucidLanguages")]
    public Task<IActionResult> GetLucidLanguages() =>
        CallLucidAsync($"Definition/GetLanguages/{SupplierId}/{ApiKey}", "GetLucidLanguages");

    [HttpGet("GetLucidIndustries")]
    public Task<IActionResult> GetLucidIndustries() =>
        CallLucidAsync($"Definition/GetIndustries/{SupplierId}/{ApiKey}", "GetLucidIndustries");

    [HttpGet("GetLucidStudyTypes")]
    public Task<IActionResult> GetLucidStudyTypes() =>
        CallLucidAsync($"Definition/GetStudyTypes/{SupplierId}/{ApiKey}", "GetLucidStudyTypes");

    [HttpGet("GetLucidSurveyStatuses")]
    public Task<IActionResult> GetLucidSurveyStatuses() =>
        CallLucidAsync($"Definition/GetSurveyStatuses/{SupplierId}/{ApiKey}", "GetLucidSurveyStatuses");

    [HttpGet("GetLucidRedirectTypes")]
    public Task<IActionResult> GetLucidRedirectTypes() =>
        CallLucidAsync($"Definition/GetRedirectTypes/{SupplierId}/{ApiKey}", "GetLucidRedirectTypes");

    [HttpGet("GetLucidQualificationTypes")]
    public Task<IActionResult> GetLucidQualificationTypes() =>
        CallLucidAsync($"Definition/GetQualificationTypes/{SupplierId}/{ApiKey}", "GetLucidQualificationTypes");

    [HttpGet("GetLucidQualifications/{surveyId}")]
    public Task<IActionResult> GetLucidQualifications(string surveyId) =>
        CallLucidAsync($"Survey/GetSupplierSurveyQualifications/{SupplierId}/{ApiKey}/{surveyId}", "GetLucidQualifications");

    [HttpGet("GetLucidBundledQualifications/{languageId}")]
    public Task<IActionResult> GetLucidBundledQualifications(string languageId) =>
        CallLucidAsync($"Definition/GetBundledQualificationAnswers/{SupplierId}/{ApiKey}/{languageId}", "GetLucidBundledQualifications");
}
