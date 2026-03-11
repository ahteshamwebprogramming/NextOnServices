using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace Consultings.WebUI.Controllers.VT;

/// <summary>
/// Proxies Spectrum Surveys API calls for NextOnServices SurveysSpectrum module.
/// NextOnServices calls this when ConsultingApiBaseUrl is set; otherwise it calls Spectrum directly.
/// </summary>
[ApiController]
[Route("VT/ApiProjects")]
[Produces("application/json")]
public class SpectrumApiProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SpectrumApiProxyController> _logger;

    public SpectrumApiProxyController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<SpectrumApiProxyController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    private string BaseUrl => _configuration["SpectrumApi:BaseUrl"] ?? "https://api.spectrumsurveys.com/suppliers/v2";
    private string AccessToken => _configuration["SpectrumApi:AccessToken"] ?? "";

    [HttpGet("GetSpectrumSurveys")]
    [HttpPost("GetSpectrumSurveys")]
    public async Task<IActionResult> GetSpectrumSurveys()
    {
        var url = $"{BaseUrl.TrimEnd('/')}/surveys";
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("access-token", AccessToken);
            client.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse("no-cache");
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Version = HttpVersion.Version11;
            req.VersionPolicy = HttpVersionPolicy.RequestVersionExact;
            var response = await client.SendAsync(req);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return Content(content, "application/json");
            _logger.LogError("Spectrum API failed. GetSpectrumSurveys StatusCode={StatusCode}", (int)response.StatusCode);
            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Spectrum API call failed. GetSpectrumSurveys");
            return StatusCode(500, new { error = true, context = "GetSpectrumSurveys", message = ex.Message });
        }
    }

    /// <summary>
    /// Registers a survey with Spectrum and returns the API response (including survey_entry_url).
    /// VT AddProjectFromSpectrum calls this when ConsultingApiBaseUrl is set.
    /// </summary>
    [HttpPost("RegisterSpectrumSurvey")]
    public async Task<IActionResult> RegisterSpectrumSurvey([FromBody] RegisterSpectrumSurveyRequest request)
    {
        var projectId = request?.ProjectId?.Trim();
        if (string.IsNullOrEmpty(projectId))
            return BadRequest(new { error = true, message = "ProjectId is required" });
        var url = $"{BaseUrl.TrimEnd('/')}/surveys/register/{Uri.EscapeDataString(projectId)}";
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("access-token", AccessToken);
            client.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse("no-cache");
            var response = await client.PostAsync(url, new StringContent(""));
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return Content(content, "application/json");
            _logger.LogError("Spectrum register API failed. StatusCode={StatusCode}, Response={Response}", (int)response.StatusCode, content);
            return StatusCode((int)response.StatusCode, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Spectrum register API call failed. ProjectId={ProjectId}", projectId);
            return StatusCode(500, new { error = true, context = "RegisterSpectrumSurvey", message = ex.Message });
        }
    }

    public class RegisterSpectrumSurveyRequest
    {
        public string? ProjectId { get; set; }
    }
}
