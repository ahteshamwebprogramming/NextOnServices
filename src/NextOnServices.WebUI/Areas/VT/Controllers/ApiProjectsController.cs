using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.Helper;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace NextOnServices.WebUI.VT.Controllers;

[Area("VT")]
public class ApiProjectsController : Controller
{
    private readonly ILogger<ApiProjectsController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ProjectsAPIController _projectsAPIController;
    private readonly ProjectURLAPIController _projectURLAPIController;
    private readonly ProjectMappingAPIController _projectMappingAPIController;
    private readonly IConfiguration _configuration;
    private const string LUCID_API_BASE = "https://api.sample-cube.com/api";
    private const string LUCID_SUPPLIER_ID = "1595";
    private const string LUCID_API_KEY = "084853e8-1b98-4828-9af8-15332e5fe165";
    private const string SPECTRUM_API_BASE = "https://api.spectrumsurveys.com/suppliers/v2";
    private const string SPECTRUM_ACCESS_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjY1NmY4YmY4YzIwZjM0NGM3YjYyZGI4ZCIsInVzcl9pZCI6IjE5NzAxIiwiaWF0IjoxNzAxODA5MTQ0fQ.1rzvWDMOvFBqzw8U2k8jFpOFOglgNWIXTrWykub-USc";
    
    // Default values from old implementation
    private const int DEFAULT_CLIENT_ID = 53; // ForSago
    private const int DEFAULT_COUNTRY_ID = 235;
    private const int DEFAULT_SUPPLIER_ID = 1064; // Arete Research
    private const int DEFAULT_STATUS = 5;

    public ApiProjectsController(
        ILogger<ApiProjectsController> logger, 
        IHttpClientFactory httpClientFactory, 
        ProjectsAPIController projectsAPIController,
        ProjectURLAPIController projectURLAPIController,
        ProjectMappingAPIController projectMappingAPIController,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _projectsAPIController = projectsAPIController;
        _projectURLAPIController = projectURLAPIController;
        _projectMappingAPIController = projectMappingAPIController;
        _configuration = configuration;
    }

    /// <summary>Masks API keys/tokens in URL for safe logging.</summary>
    private static string MaskUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return url;
        var masked = url;
        if (!string.IsNullOrEmpty(LUCID_API_KEY) && masked.Contains(LUCID_API_KEY))
            masked = masked.Replace(LUCID_API_KEY, "***");
        if (!string.IsNullOrEmpty(SPECTRUM_ACCESS_TOKEN) && masked.Contains(SPECTRUM_ACCESS_TOKEN))
            masked = masked.Replace(SPECTRUM_ACCESS_TOKEN, "***");
        return masked;
    }

    /// <summary>Builds a multi-line string with exception chain (types, messages, stack traces).</summary>
    private static string FlattenException(Exception ex)
    {
        if (ex == null) return string.Empty;
        var sb = new System.Text.StringBuilder();
        var current = ex;
        int level = 0;
        while (current != null && level <= 2)
        {
            sb.Append("Level ").Append(level).Append(": ")
                .Append(current.GetType().FullName).Append(" - ").Append(current.Message ?? "").AppendLine();
            if (!string.IsNullOrEmpty(current.StackTrace))
                sb.AppendLine(current.StackTrace);
            current = current.InnerException;
            level++;
        }
        return sb.ToString();
    }

    /// <summary>Builds a safe JSON-serializable error object (no secrets).</summary>
    private object BuildSafeError(Exception ex, string context, string url = null, int? statusCode = null)
    {
        var obj = new
        {
            error = true,
            context,
            url = url != null ? MaskUrl(url) : null,
            message = ex?.Message,
            inner = ex?.InnerException?.Message,
            inner2 = ex?.InnerException?.InnerException?.Message,
            traceId = HttpContext?.TraceIdentifier,
            statusCode
        };
        return obj;
    }

    [Route("/VT/ApiProjects/SurveysLucid")]
    [Route("/VT/ProjectsListSago.aspx")]
    public IActionResult SurveysLucid()
    {
        ViewData["Title"] = "Surveys (Lucid)";
        return View();
    }

    [Route("/VT/ApiProjects/SurveysSpectrum")]
    public IActionResult SurveysSpectrum()
    {
        ViewData["Title"] = "Surveys (Spectrum)";
        return View();
    }

    // Lucid API Endpoints
    [HttpGet]
    [Route("/VT/ApiProjects/GetLucidSurveys")]
    public async Task<IActionResult> GetLucidSurveys()
    {
        const string context = "GetLucidSurveys";
        var url = $"{LUCID_API_BASE}/Survey/GetSupplierAllocatedSurveys/{LUCID_SUPPLIER_ID}/{LUCID_API_KEY}";
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Lucid Surveys API Response length: {Length}", content?.Length ?? 0);
                return Content(content, "application/json");
            }
            _logger.LogError("External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}, StatusCode={StatusCode}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, (int)response.StatusCode);
            return StatusCode((int)response.StatusCode, new { error = true, context, url = MaskUrl(url), message = "Failed to fetch surveys", statusCode = (int)response.StatusCode, traceId = HttpContext?.TraceIdentifier });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
    }

    [HttpGet]
    [Route("/VT/ApiProjects/GetLucidLanguages")]
    public async Task<IActionResult> GetLucidLanguages()
    {
        const string context = "GetLucidLanguages";
        var url = $"{LUCID_API_BASE}/Definition/GetLanguages/{LUCID_SUPPLIER_ID}/{LUCID_API_KEY}";
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            _logger.LogError("External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}, StatusCode={StatusCode}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, (int)response.StatusCode);
            return StatusCode((int)response.StatusCode, new { error = true, context, url = MaskUrl(url), message = "Failed to fetch languages", statusCode = (int)response.StatusCode, traceId = HttpContext?.TraceIdentifier });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
    }

    [HttpGet]
    [Route("/VT/ApiProjects/GetLucidIndustries")]
    public async Task<IActionResult> GetLucidIndustries()
    {
        const string context = "GetLucidIndustries";
        var url = $"{LUCID_API_BASE}/Definition/GetIndustries/{LUCID_SUPPLIER_ID}/{LUCID_API_KEY}";
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            _logger.LogError("External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}, StatusCode={StatusCode}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, (int)response.StatusCode);
            return StatusCode((int)response.StatusCode, new { error = true, context, url = MaskUrl(url), message = "Failed to fetch industries", statusCode = (int)response.StatusCode, traceId = HttpContext?.TraceIdentifier });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
    }

    [HttpGet]
    [Route("/VT/ApiProjects/GetLucidStudyTypes")]
    public async Task<IActionResult> GetLucidStudyTypes()
    {
        const string context = "GetLucidStudyTypes";
        var url = $"{LUCID_API_BASE}/Definition/GetStudyTypes/{LUCID_SUPPLIER_ID}/{LUCID_API_KEY}";
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            _logger.LogError("External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}, StatusCode={StatusCode}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, (int)response.StatusCode);
            return StatusCode((int)response.StatusCode, new { error = true, context, url = MaskUrl(url), message = "Failed to fetch study types", statusCode = (int)response.StatusCode, traceId = HttpContext?.TraceIdentifier });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
    }

    [HttpGet]
    [Route("/VT/ApiProjects/GetLucidSurveyStatuses")]
    public async Task<IActionResult> GetLucidSurveyStatuses()
    {
        const string context = "GetLucidSurveyStatuses";
        var url = $"{LUCID_API_BASE}/Definition/GetSurveyStatuses/{LUCID_SUPPLIER_ID}/{LUCID_API_KEY}";
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            _logger.LogError("External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}, StatusCode={StatusCode}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, (int)response.StatusCode);
            return StatusCode((int)response.StatusCode, new { error = true, context, url = MaskUrl(url), message = "Failed to fetch survey statuses", statusCode = (int)response.StatusCode, traceId = HttpContext?.TraceIdentifier });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
    }

    [HttpGet]
    [Route("/VT/ApiProjects/GetLucidRedirectTypes")]
    public async Task<IActionResult> GetLucidRedirectTypes()
    {
        const string context = "GetLucidRedirectTypes";
        var url = $"{LUCID_API_BASE}/Definition/GetRedirectTypes/{LUCID_SUPPLIER_ID}/{LUCID_API_KEY}";
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            _logger.LogError("External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}, StatusCode={StatusCode}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, (int)response.StatusCode);
            return StatusCode((int)response.StatusCode, new { error = true, context, url = MaskUrl(url), message = "Failed to fetch redirect types", statusCode = (int)response.StatusCode, traceId = HttpContext?.TraceIdentifier });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
    }

    [HttpGet]
    [Route("/VT/ApiProjects/GetLucidQualificationTypes")]
    public async Task<IActionResult> GetLucidQualificationTypes()
    {
        const string context = "GetLucidQualificationTypes";
        var url = $"{LUCID_API_BASE}/Definition/GetQualificationTypes/{LUCID_SUPPLIER_ID}/{LUCID_API_KEY}";
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            _logger.LogError("External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}, StatusCode={StatusCode}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, (int)response.StatusCode);
            return StatusCode((int)response.StatusCode, new { error = true, context, url = MaskUrl(url), message = "Failed to fetch qualification types", statusCode = (int)response.StatusCode, traceId = HttpContext?.TraceIdentifier });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
    }

    [HttpGet]
    [Route("/VT/ApiProjects/GetLucidQualifications/{surveyId}")]
    public async Task<IActionResult> GetLucidQualifications(string surveyId)
    {
        const string context = "GetLucidQualifications";
        var url = $"{LUCID_API_BASE}/Survey/GetSupplierSurveyQualifications/{LUCID_SUPPLIER_ID}/{LUCID_API_KEY}/{surveyId}";
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            _logger.LogError("External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}, StatusCode={StatusCode}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, (int)response.StatusCode);
            return StatusCode((int)response.StatusCode, new { error = true, context, url = MaskUrl(url), message = "Failed to fetch qualifications", statusCode = (int)response.StatusCode, traceId = HttpContext?.TraceIdentifier });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
    }

    [HttpGet]
    [Route("/VT/ApiProjects/GetLucidBundledQualifications/{languageId}")]
    public async Task<IActionResult> GetLucidBundledQualifications(string languageId)
    {
        const string context = "GetLucidBundledQualifications";
        var url = $"{LUCID_API_BASE}/Definition/GetBundledQualificationAnswers/{LUCID_SUPPLIER_ID}/{LUCID_API_KEY}/{languageId}";
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            _logger.LogError("External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}, StatusCode={StatusCode}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, (int)response.StatusCode);
            return StatusCode((int)response.StatusCode, new { error = true, context, url = MaskUrl(url), message = "Failed to fetch bundled qualifications", statusCode = (int)response.StatusCode, traceId = HttpContext?.TraceIdentifier });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
    }

    // Spectrum API Endpoints
    [HttpPost]
    [HttpGet]
    [Route("/VT/ApiProjects/GetSpectrumSurveys")]
    public async Task<IActionResult> GetSpectrumSurveys()
    {
        const string context = "GetSpectrumSurveys";
        var url = $"{SPECTRUM_API_BASE}/surveys";
        _logger.LogInformation("GetSpectrumSurveys endpoint called - Method: {Method}", HttpContext.Request.Method);

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("access-token", SPECTRUM_ACCESS_TOKEN);
            client.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse("no-cache");
            _logger.LogInformation("Calling Spectrum API: {Url}", MaskUrl(url));

            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Version = HttpVersion.Version11;
            req.VersionPolicy = HttpVersionPolicy.RequestVersionExact;

            var response = await client.SendAsync(req);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Spectrum Surveys API Response length: {Length}", responseContent?.Length ?? 0);
                _logger.LogInformation("Spectrum Surveys API Response preview: {Preview}",
                    responseContent?.Length > 200 ? responseContent.Substring(0, 200) : responseContent);
                return Content(responseContent, "application/json");
            }

            _logger.LogError("External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}, StatusCode={StatusCode}, Response={Response}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier, (int)response.StatusCode, responseContent);
            return StatusCode((int)response.StatusCode, new { error = true, context, url = MaskUrl(url), message = $"Failed to fetch surveys: {response.StatusCode}", statusCode = (int)response.StatusCode, traceId = HttpContext?.TraceIdentifier });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex,
                "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}, ExType={ExType}, ExMessage={ExMessage}, InnerType={InnerType}, InnerMessage={InnerMessage}, Inner2Type={Inner2Type}, Inner2Message={Inner2Message}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier,
                ex.GetType().FullName, ex.Message,
                ex.InnerException?.GetType()?.FullName, ex.InnerException?.Message,
                ex.InnerException?.InnerException?.GetType()?.FullName, ex.InnerException?.InnerException?.Message,
                FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}, ExType={ExType}, ExMessage={ExMessage}, InnerType={InnerType}, InnerMessage={InnerMessage}, Inner2Type={Inner2Type}, Inner2Message={Inner2Message}. Details={Details}",
                context, MaskUrl(url), HttpContext?.TraceIdentifier,
                ex.GetType().FullName, ex.Message,
                ex.InnerException?.GetType()?.FullName, ex.InnerException?.Message,
                ex.InnerException?.InnerException?.GetType()?.FullName, ex.InnerException?.InnerException?.Message,
                FlattenException(ex));
            return StatusCode(500, BuildSafeError(ex, context, url));
        }
    }

    // Add Project Methods
    [HttpPost]
    [Route("/VT/ApiProjects/AddProjectFromLucid")]
    public async Task<IActionResult> AddProjectFromLucid([FromBody] AddProjectFromLucidRequest request)
    {
        try
        {
            // Check session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || userId == 0)
            {
                return Ok(new { Result = false, Message = "Session expired. Please login again." });
            }

            // Check if project already exists
            if (string.IsNullOrWhiteSpace(request.ProjectId))
            {
                return Ok(new { Result = false, Message = "Project ID is required" });
            }

            try
            {
                var checkResult = await _projectsAPIController.GetProjectByProjectIdFromAPI(request.ProjectId.Trim(), "Lucid");
                if (checkResult is ObjectResult checkObjectResult && checkObjectResult.Value != null)
                {
                    int existingProjectId = 0;
                    string valueStr = checkObjectResult.Value.ToString();
                    if (!string.IsNullOrEmpty(valueStr) && int.TryParse(valueStr, out existingProjectId) && existingProjectId > 0)
                    {
                        _logger.LogInformation("Duplicate project detected: ProjectIdFromAPI={ProjectId}, ExistingProjectId={ExistingId}", request.ProjectId, existingProjectId);
                        return Ok(new { Result = false, Message = "Project Already Added" });
                    }
                }
            }
            catch (Exception checkEx)
            {
                _logger.LogWarning(checkEx, "Error checking for duplicate project, continuing with creation");
                // Continue - will check again if AddProject fails
            }

            // Parse input values
            double? cpi = null;
            int? loi = null;
            int? ir = null;
            int? totalRemaining = null;

            if (!string.IsNullOrEmpty(request.CPI) && double.TryParse(request.CPI, out double cpiValue))
                cpi = cpiValue;
            if (!string.IsNullOrEmpty(request.LOI) && int.TryParse(request.LOI, out int loiValue))
                loi = loiValue;
            if (!string.IsNullOrEmpty(request.IR) && int.TryParse(request.IR, out int irValue))
                ir = irValue;
            if (!string.IsNullOrEmpty(request.TotalRemaining) && int.TryParse(request.TotalRemaining, out int totalRemainingValue))
                totalRemaining = totalRemainingValue;

            // Create ProjectDTO
            ProjectDTO projectDTO = new ProjectDTO
            {
                ProjectId = 0, // New project
                Pname = request.ProjectId,
                Descriptions = request.ProjectId + "-Pulled from Lucid API",
                Pmanager = userId.Value,
                ClientId = DEFAULT_CLIENT_ID,
                Loi = loi?.ToString() ?? "",
                Irate = ir?.ToString() ?? "",
                Cpi = cpi,
                SampleSize = totalRemaining?.ToString() ?? "",
                Quota = totalRemaining?.ToString() ?? "",
                Sdate = DateTime.Now.ToString("MM-dd-yyyy"),
                Edate = DateTime.Now.AddMonths(1).ToString("MM-dd-yyyy"),
                CountryId = DEFAULT_COUNTRY_ID,
                Status = DEFAULT_STATUS,
                IsActive = 1,
                BlockDevice = "00000",
                CreationDate = DateTime.Now,
                Notes = "Added from Lucid from API",
                ProjectIdFromApi = request.ProjectId,
                ProjectFrom = "Lucid",
                Ltype = 0
            };

            // Add the project
            IActionResult addProjectResult;
            try
            {
                addProjectResult = await _projectsAPIController.AddProject(projectDTO);
            }
            catch (Exception addEx)
            {
                // Check if it's a duplicate error
                string errorMsg = addEx.Message;
                if (addEx.InnerException != null)
                {
                    errorMsg = addEx.InnerException.Message;
                }
                
                if (errorMsg.Contains("duplicate") || errorMsg.Contains("unique") || 
                    errorMsg.Contains("UNIQUE KEY") || errorMsg.Contains("PRIMARY KEY") ||
                    errorMsg.Contains("Cannot insert duplicate key"))
                {
                    return Ok(new { Result = false, Message = "Project Already Added" });
                }
                throw; // Re-throw if it's not a duplicate error
            }
            
            if (addProjectResult is ObjectResult objectResult && objectResult.StatusCode == 200)
            {
                // Get the created project ID by querying for it
                var getProjectResult = await _projectsAPIController.GetProjectByProjectIdFromAPI(request.ProjectId, "Lucid");
                
                int createdProjectId = 0;
                if (getProjectResult is ObjectResult getObjectResult && getObjectResult.Value != null)
                {
                    createdProjectId = Convert.ToInt32(getObjectResult.Value);
                }

                if (createdProjectId == 0)
                {
                    return Ok(new { Result = false, Message = "Failed to retrieve created project ID" });
                }

                // Process LiveLink - replace Lucid-specific placeholders
                string processedLiveLink = request.LiveLink?.Replace("[#scid#]&uid=[#scid2#]", "[respondentID]") ?? request.LiveLink ?? "";

                // Create ProjectURL
                ProjectsUrlDTO projectUrlDTO = new ProjectsUrlDTO
                {
                    Pid = createdProjectId,
                    Cid = DEFAULT_COUNTRY_ID,
                    Url = processedLiveLink,
                    Notes = "Added from lucid",
                    Cpi = cpi,
                    Quota = totalRemaining?.ToString() ?? "",
                    Status = DEFAULT_STATUS,
                    CreationDate = DateTime.Now
                };

                var addUrlResult = await _projectURLAPIController.AddProjectURL(projectUrlDTO);
                if (!(addUrlResult is ObjectResult urlObjectResult && urlObjectResult.StatusCode == 200))
                {
                    _logger.LogWarning("Failed to create ProjectURL for project {ProjectId}", createdProjectId);
                    // Continue anyway - project is created
                }

                // Create ProjectMapping
                string kid = CommonHelper.RandomString(32);
                string sid = CommonHelper.RandomString(8);
                string? maskingUrl = _configuration.GetValue<string>("MaskingUrl");
                string mUrl = $"{maskingUrl}?SID={sid}&ID=XXXXXXXXXX";
                string oUrl = processedLiveLink;

                ProjectMappingDTO projectMappingDTO = new ProjectMappingDTO
                {
                    ProjectId = createdProjectId,
                    CountryId = DEFAULT_COUNTRY_ID,
                    SupplierId = DEFAULT_SUPPLIER_ID,
                    Olink = oUrl,
                    Cpi = cpi,
                    Mlink = mUrl,
                    Sid = sid,
                    Code = kid,
                    AddHashing = 0,
                    IsUsed = 0,
                    CreationDate = DateTime.Now,
                    IsSent = 0,
                    Block = 0,
                    TrackingType = 0,
                    Rc = 0
                };

                var addMappingResult = await _projectMappingAPIController.AddProjectMapping(projectMappingDTO);
                if (!(addMappingResult is ObjectResult mappingObjectResult && mappingObjectResult.StatusCode == 200))
                {
                    _logger.LogWarning("Failed to create ProjectMapping for project {ProjectId}", createdProjectId);
                    // Continue anyway - project and URL are created
                }

                return Ok(new { Result = true, Message = "Project added successfully" });
            }
            else
            {
                string errorMessage = "Failed to create project";
                if (addProjectResult is ObjectResult errorResult)
                {
                    if (errorResult.Value != null)
                    {
                        errorMessage = errorResult.Value.ToString() ?? errorMessage;
                    }
                    // Check if it's a BadRequest with a specific message
                    if (errorResult.StatusCode == 400 && errorResult.Value is string)
                    {
                        errorMessage = errorResult.Value.ToString() ?? errorMessage;
                    }
                }
                else if (addProjectResult is BadRequestObjectResult badRequestResult)
                {
                    if (badRequestResult.Value != null)
                    {
                        errorMessage = badRequestResult.Value.ToString() ?? errorMessage;
                    }
                }
                
                _logger.LogWarning("AddProject failed: {ErrorMessage}", errorMessage);
                return Ok(new { Result = false, Message = errorMessage });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding project from Lucid. Exception: {ExceptionMessage}, InnerException: {InnerException}", 
                ex.Message, ex.InnerException?.Message);
            
            // Check if it's a duplicate error - check both exception message and inner exception
            string errorMessage = ex.Message;
            if (ex.InnerException != null)
            {
                errorMessage = ex.InnerException.Message;
            }
            
            // Check for duplicate key violations or constraint violations (case-insensitive)
            string lowerErrorMessage = errorMessage.ToLower();
            if (lowerErrorMessage.Contains("duplicate") || lowerErrorMessage.Contains("unique") || 
                lowerErrorMessage.Contains("unique key") || lowerErrorMessage.Contains("primary key") ||
                lowerErrorMessage.Contains("cannot insert duplicate key") ||
                lowerErrorMessage.Contains("violation of unique key") ||
                lowerErrorMessage.Contains("constraint") && lowerErrorMessage.Contains("unique"))
            {
                _logger.LogInformation("Duplicate project detected via exception: ProjectIdFromAPI={ProjectId}", request.ProjectId);
                
                // Double-check if project exists
                try
                {
                    var verifyResult = await _projectsAPIController.GetProjectByProjectIdFromAPI(request.ProjectId, "Lucid");
                    if (verifyResult is ObjectResult verifyObjectResult && verifyObjectResult.Value != null)
                    {
                        int existingProjectId = 0;
                        if (int.TryParse(verifyObjectResult.Value.ToString(), out existingProjectId) && existingProjectId > 0)
                        {
                            _logger.LogInformation("Verified duplicate project exists: ProjectId={ProjectId}", existingProjectId);
                            return Ok(new { Result = false, Message = "Project Already Added" });
                        }
                    }
                }
                catch (Exception verifyEx)
                {
                    _logger.LogWarning(verifyEx, "Error verifying duplicate project, still returning duplicate message");
                }
                return Ok(new { Result = false, Message = "Project Already Added" });
            }
            
            _logger.LogWarning("Non-duplicate error occurred: {ErrorMessage}", errorMessage);
            return Ok(new { Result = false, Message = errorMessage });
        }
    }

    [HttpPost]
    [Route("/VT/ApiProjects/AddProjectFromSpectrum")]
    public async Task<IActionResult> AddProjectFromSpectrum([FromBody] AddProjectFromSpectrumRequest request)
    {
        try
        {
            // Check session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || userId == 0)
            {
                return Ok(new { result = false, message = "Session expired. Please login again." });
            }

            // Check if project already exists
            if (string.IsNullOrWhiteSpace(request.ProjectId))
            {
                return Ok(new { result = false, message = "Project ID is required" });
            }

            try
            {
                var checkResult = await _projectsAPIController.GetProjectByProjectIdFromAPI(request.ProjectId.Trim(), "Spectrum");
                if (checkResult is ObjectResult checkObjectResult && checkObjectResult.Value != null)
                {
                    int existingProjectId = 0;
                    string valueStr = checkObjectResult.Value.ToString();
                    if (!string.IsNullOrEmpty(valueStr) && int.TryParse(valueStr, out existingProjectId) && existingProjectId > 0)
                    {
                        _logger.LogInformation("Duplicate project detected: ProjectIdFromAPI={ProjectId}, ExistingProjectId={ExistingId}", request.ProjectId, existingProjectId);
                        return Ok(new { result = false, message = "Project Already Added" });
                    }
                }
            }
            catch (Exception checkEx)
            {
                _logger.LogWarning(checkEx, "Error checking for duplicate project, continuing with creation");
                // Continue - will check again if AddProject fails
            }

            // Parse input values
            double? cpi = null;
            int? loi = null;
            int? ir = null;
            int? totalRemaining = null;

            if (!string.IsNullOrEmpty(request.CPI) && double.TryParse(request.CPI, out double cpiValue))
                cpi = cpiValue;
            if (!string.IsNullOrEmpty(request.LOI) && int.TryParse(request.LOI, out int loiValue))
                loi = loiValue;
            if (!string.IsNullOrEmpty(request.IR) && int.TryParse(request.IR, out int irValue))
                ir = irValue;
            if (!string.IsNullOrEmpty(request.TotalRemaining) && int.TryParse(request.TotalRemaining, out int totalRemainingValue))
                totalRemaining = totalRemainingValue;

            // Call Spectrum API to register survey and get survey_entry_url
            string surveyEntryUrl = null;
            const string registerContext = "AddProjectFromSpectrum-Register";
            var registerUrl = $"{SPECTRUM_API_BASE}/surveys/register/{request.ProjectId}";
            try
            {
                var registerClient = _httpClientFactory.CreateClient();
                registerClient.DefaultRequestHeaders.Add("access-token", SPECTRUM_ACCESS_TOKEN);
                registerClient.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse("no-cache");
                _logger.LogInformation("Calling Spectrum register API: {Url}", MaskUrl(registerUrl));

                var registerResponse = await registerClient.PostAsync(registerUrl, new StringContent(""));
                var registerContent = await registerResponse.Content.ReadAsStringAsync();

                if (registerResponse.IsSuccessStatusCode)
                {
                    var registerJson = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(registerContent);
                    if (registerJson != null && registerJson["apiStatus"]?.ToString() == "success")
                    {
                        surveyEntryUrl = registerJson["survey_entry_url"]?.ToString();
                        _logger.LogInformation("Spectrum survey registered successfully. Survey entry URL: {Url}", surveyEntryUrl);
                    }
                    else
                    {
                        _logger.LogWarning("Spectrum register API returned non-success status: {Content}", registerContent);
                        return Ok(new { result = false, message = "Failed to register survey with Spectrum API" });
                    }
                }
                else
                {
                    _logger.LogError("External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}, StatusCode={StatusCode}, Response={Response}",
                        registerContext, MaskUrl(registerUrl), HttpContext?.TraceIdentifier, (int)registerResponse.StatusCode, registerContent);
                    return StatusCode((int)registerResponse.StatusCode, new { error = true, context = registerContext, url = MaskUrl(registerUrl), message = $"Failed to register survey: {registerResponse.StatusCode}", statusCode = (int)registerResponse.StatusCode, traceId = HttpContext?.TraceIdentifier });
                }
            }
            catch (HttpRequestException registerEx)
            {
                _logger.LogError(registerEx, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                    registerContext, MaskUrl(registerUrl), HttpContext?.TraceIdentifier, FlattenException(registerEx));
                return StatusCode(500, BuildSafeError(registerEx, registerContext, registerUrl));
            }
            catch (Exception registerEx)
            {
                _logger.LogError(registerEx, "External API call failed. Context={Context}, Url={Url}, TraceId={TraceId}. Details={Details}",
                    registerContext, MaskUrl(registerUrl), HttpContext?.TraceIdentifier, FlattenException(registerEx));
                return StatusCode(500, BuildSafeError(registerEx, registerContext, registerUrl));
            }

            if (string.IsNullOrEmpty(surveyEntryUrl))
            {
                return Ok(new { result = false, message = "Failed to get survey entry URL from Spectrum API" });
            }

            // Create ProjectDTO
            ProjectDTO projectDTO = new ProjectDTO
            {
                ProjectId = 0, // New project
                Pname = request.ProjectName ?? request.ProjectId,
                Descriptions = request.ProjectId + "-Pulled from Spectrum API",
                Pmanager = userId.Value,
                ClientId = DEFAULT_CLIENT_ID,
                Loi = loi?.ToString() ?? "",
                Irate = ir?.ToString() ?? "",
                Cpi = cpi,
                SampleSize = totalRemaining?.ToString() ?? "",
                Quota = totalRemaining?.ToString() ?? "",
                Sdate = DateTime.Now.ToString("MM-dd-yyyy"),
                Edate = DateTime.Now.AddMonths(1).ToString("MM-dd-yyyy"),
                CountryId = DEFAULT_COUNTRY_ID,
                Status = DEFAULT_STATUS,
                IsActive = 1,
                BlockDevice = "00000",
                CreationDate = DateTime.Now,
                Notes = "Added from Spectrum from API",
                ProjectIdFromApi = request.ProjectId,
                ProjectFrom = "Spectrum",
                Ltype = 0
            };

            // Add the project
            IActionResult addProjectResult;
            try
            {
                addProjectResult = await _projectsAPIController.AddProject(projectDTO);
            }
            catch (Exception addEx)
            {
                // Check if it's a duplicate error
                string errorMsg = addEx.Message;
                if (addEx.InnerException != null)
                {
                    errorMsg = addEx.InnerException.Message;
                }
                
                if (errorMsg.Contains("duplicate") || errorMsg.Contains("unique") || 
                    errorMsg.Contains("UNIQUE KEY") || errorMsg.Contains("PRIMARY KEY") ||
                    errorMsg.Contains("Cannot insert duplicate key"))
                {
                    return Ok(new { result = false, message = "Project Already Added" });
                }
                throw; // Re-throw if it's not a duplicate error
            }
            
            if (addProjectResult is ObjectResult objectResult && objectResult.StatusCode == 200)
            {
                // Get the created project ID by querying for it
                var getProjectResult = await _projectsAPIController.GetProjectByProjectIdFromAPI(request.ProjectId, "Spectrum");
                
                int createdProjectId = 0;
                if (getProjectResult is ObjectResult getObjectResult && getObjectResult.Value != null)
                {
                    createdProjectId = Convert.ToInt32(getObjectResult.Value);
                }

                if (createdProjectId == 0)
                {
                    return Ok(new { result = false, message = "Failed to retrieve created project ID" });
                }

                // Process survey entry URL - add RespondentID parameter
                string processedUrl = surveyEntryUrl + "&ID=[RespondentID]";

                // Create ProjectURL
                ProjectsUrlDTO projectUrlDTO = new ProjectsUrlDTO
                {
                    Pid = createdProjectId,
                    Cid = DEFAULT_COUNTRY_ID,
                    Url = processedUrl,
                    Notes = "Added from Spectrum",
                    Cpi = cpi,
                    Quota = totalRemaining?.ToString() ?? "",
                    Status = DEFAULT_STATUS,
                    CreationDate = DateTime.Now
                };

                var addUrlResult = await _projectURLAPIController.AddProjectURL(projectUrlDTO);
                if (!(addUrlResult is ObjectResult urlObjectResult && urlObjectResult.StatusCode == 200))
                {
                    _logger.LogWarning("Failed to create ProjectURL for project {ProjectId}", createdProjectId);
                    // Continue anyway - project is created
                }

                // Create ProjectMapping
                string kid = CommonHelper.RandomString(32);
                string sid = CommonHelper.RandomString(8);
                string? maskingUrl = _configuration.GetValue<string>("MaskingUrl");
                string mUrl = $"{maskingUrl}?SID={sid}&ID=XXXXXXXXXX";
                string oUrl = processedUrl;

                ProjectMappingDTO projectMappingDTO = new ProjectMappingDTO
                {
                    ProjectId = createdProjectId,
                    CountryId = DEFAULT_COUNTRY_ID,
                    SupplierId = DEFAULT_SUPPLIER_ID,
                    Olink = oUrl,
                    Cpi = cpi,
                    Mlink = mUrl,
                    Sid = sid,
                    Code = kid,
                    AddHashing = 0,
                    IsUsed = 0,
                    CreationDate = DateTime.Now,
                    IsSent = 0,
                    Block = 0,
                    TrackingType = 0,
                    Rc = 0
                };

                var addMappingResult = await _projectMappingAPIController.AddProjectMapping(projectMappingDTO);
                if (!(addMappingResult is ObjectResult mappingObjectResult && mappingObjectResult.StatusCode == 200))
                {
                    _logger.LogWarning("Failed to create ProjectMapping for project {ProjectId}", createdProjectId);
                    // Continue anyway - project and URL are created
                }

                return Ok(new { result = true, message = "Project added successfully" });
            }
            else
            {
                string errorMessage = "Failed to create project";
                if (addProjectResult is ObjectResult errorResult)
                {
                    if (errorResult.Value != null)
                    {
                        errorMessage = errorResult.Value.ToString() ?? errorMessage;
                    }
                    // Check if it's a BadRequest with a specific message
                    if (errorResult.StatusCode == 400 && errorResult.Value is string)
                    {
                        errorMessage = errorResult.Value.ToString() ?? errorMessage;
                    }
                }
                else if (addProjectResult is BadRequestObjectResult badRequestResult)
                {
                    if (badRequestResult.Value != null)
                    {
                        errorMessage = badRequestResult.Value.ToString() ?? errorMessage;
                    }
                }
                
                _logger.LogWarning("AddProject failed: {ErrorMessage}", errorMessage);
                return Ok(new { result = false, message = errorMessage });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding project from Spectrum. Exception: {ExceptionMessage}", ex.Message);
            
            // Check if it's a duplicate error
            string errorMessage = ex.Message;
            if (ex.InnerException != null)
            {
                errorMessage = ex.InnerException.Message;
            }
            
            // Check for duplicate key violations or constraint violations (case-insensitive)
            string lowerErrorMessage = errorMessage.ToLower();
            if (lowerErrorMessage.Contains("duplicate") || lowerErrorMessage.Contains("unique") || 
                lowerErrorMessage.Contains("unique key") || lowerErrorMessage.Contains("primary key") ||
                lowerErrorMessage.Contains("cannot insert duplicate key") ||
                lowerErrorMessage.Contains("violation of unique key") ||
                lowerErrorMessage.Contains("constraint") && lowerErrorMessage.Contains("unique"))
            {
                _logger.LogInformation("Duplicate project detected via exception: ProjectIdFromAPI={ProjectId}", request.ProjectId);
                
                // Double-check if project exists
                try
                {
                    var verifyResult = await _projectsAPIController.GetProjectByProjectIdFromAPI(request.ProjectId, "Spectrum");
                    if (verifyResult is ObjectResult verifyObjectResult && verifyObjectResult.Value != null)
                    {
                        int existingProjectId = 0;
                        if (int.TryParse(verifyObjectResult.Value.ToString(), out existingProjectId) && existingProjectId > 0)
                        {
                            _logger.LogInformation("Verified duplicate project exists: ProjectId={ProjectId}", existingProjectId);
                            return Ok(new { result = false, message = "Project Already Added" });
                        }
                    }
                }
                catch (Exception verifyEx)
                {
                    _logger.LogWarning(verifyEx, "Error verifying duplicate project, still returning duplicate message");
                }
                return Ok(new { result = false, message = "Project Already Added" });
            }
            
            _logger.LogWarning("Non-duplicate error occurred: {ErrorMessage}", errorMessage);
            return Ok(new { result = false, message = errorMessage });
        }
    }
}

// Request Models
public class AddProjectFromLucidRequest
{
    public string ProjectId { get; set; }
    public string CPI { get; set; }
    public string LOI { get; set; }
    public string IR { get; set; }
    public string TotalRemaining { get; set; }
    public string LiveLink { get; set; }
}

public class AddProjectFromSpectrumRequest
{
    public string ProjectId { get; set; }
    public string ProjectName { get; set; }
    public string CPI { get; set; }
    public string LOI { get; set; }
    public string IR { get; set; }
    public string TotalRemaining { get; set; }
    public string LiveLink { get; set; }
}
