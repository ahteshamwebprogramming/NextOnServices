using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Endpoints.Clients;
using NextOnServices.Endpoints.Masters;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Endpoints.Suppliers;
using NextOnServices.Infrastructure.Models.APIProjects;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Supplier;
using NextOnServices.VT.Infrastructure.ViewModels.Project;

namespace NextOnServices.WebUI.VT.Controllers;

[Area("VT")]
[Authorize]
public class ZampliaController : Controller
{
    private readonly ILogger<ZampliaController> _logger;
    private readonly ZampliaAPIController _zampliaAPIController;
    private readonly ClientsAPIController _clientsAPIController;
    private readonly CountryAPIController _countryAPIController;
    private readonly SuppliersAPIController _suppliersAPIController;

    public ZampliaController(
        ILogger<ZampliaController> logger,
        ZampliaAPIController zampliaAPIController,
        ClientsAPIController clientsAPIController,
        CountryAPIController countryAPIController,
        SuppliersAPIController suppliersAPIController)
    {
        _logger = logger;
        _zampliaAPIController = zampliaAPIController;
        _clientsAPIController = clientsAPIController;
        _countryAPIController = countryAPIController;
        _suppliersAPIController = suppliersAPIController;
    }

    [Route("/VT/Zamplia")]
    [Route("/VT/Zamplia/Dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        ViewData["Title"] = "Zamplia Dashboard";
        return View(new ZampliaViewModel
        {
            Dashboard = await GetDashboardAsync(),
            Setting = await GetSettingAsync()
        });
    }

    [Route("/VT/Zamplia/Settings")]
    public async Task<IActionResult> Settings()
    {
        ViewData["Title"] = "Zamplia Settings";
        return View(new ZampliaViewModel
        {
            Setting = await GetSettingAsync(),
            Clients = await GetClientsAsync(),
            Countries = await GetCountriesAsync(),
            Suppliers = await GetSuppliersAsync()
        });
    }

    [Route("/VT/Zamplia/Surveys")]
    public async Task<IActionResult> Surveys()
    {
        ViewData["Title"] = "Zamplia Surveys";
        return View(new ZampliaViewModel
        {
            Surveys = await GetSurveysAsync(),
            Setting = await GetSettingAsync()
        });
    }

    [Route("/VT/Zamplia/SurveyDetails/{id:int}")]
    public async Task<IActionResult> SurveyDetails(int id)
    {
        ViewData["Title"] = "Zamplia Survey Details";
        var survey = await GetSurveyAsync(id);
        if (survey.ZampliaSurveyId <= 0)
        {
            return RedirectToAction(nameof(Surveys));
        }

        return View(new ZampliaViewModel
        {
            Survey = survey,
            Qualifications = await GetQualificationsAsync(id),
            Quotas = await GetQuotasAsync(id),
            ProjectMap = await GetProjectMapAsync(id),
            EntryLink = await GetEntryLinkAsync(id),
            Attempts = await GetAttemptsAsync(id),
            Logs = await GetLogsAsync(survey.SurveyId)
        });
    }

    [Route("/VT/Zamplia/Reconciliation")]
    public async Task<IActionResult> Reconciliation(int? runId = null)
    {
        ViewData["Title"] = "Zamplia Reconciliation";
        var runs = await GetReconciliationRunsAsync();
        var selectedRunId = runId ?? runs.FirstOrDefault()?.Id;
        return View(new ZampliaViewModel
        {
            ReconciliationRuns = runs,
            ReconciliationItems = selectedRunId.HasValue ? await GetReconciliationItemsAsync(selectedRunId.Value) : new List<ZampliaReconciliationItemDTO>()
        });
    }

    [Route("/VT/Zamplia/Logs")]
    public async Task<IActionResult> Logs()
    {
        ViewData["Title"] = "Zamplia Logs";
        return View(new ZampliaViewModel
        {
            Logs = await GetLogsAsync()
        });
    }

    private async Task<ZampliaSettingDTO> GetSettingAsync()
    {
        try
        {
            var result = await _zampliaAPIController.GetZampliaSetting();
            return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
                ? objectResult.Value as ZampliaSettingDTO ?? new ZampliaSettingDTO()
                : new ZampliaSettingDTO();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Zamplia setting view model");
            return new ZampliaSettingDTO();
        }
    }

    private async Task<ZampliaDashboardVM> GetDashboardAsync()
    {
        var result = await _zampliaAPIController.GetZampliaDashboard();
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as ZampliaDashboardVM ?? new ZampliaDashboardVM()
            : new ZampliaDashboardVM();
    }

    private async Task<List<ZampliaSurveyDTO>> GetSurveysAsync()
    {
        var result = await _zampliaAPIController.GetZampliaSurveys();
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as List<ZampliaSurveyDTO> ?? new List<ZampliaSurveyDTO>()
            : new List<ZampliaSurveyDTO>();
    }

    private async Task<ZampliaSurveyDTO> GetSurveyAsync(int id)
    {
        var result = await _zampliaAPIController.GetZampliaSurvey(id);
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as ZampliaSurveyDTO ?? new ZampliaSurveyDTO()
            : new ZampliaSurveyDTO();
    }

    private async Task<List<ZampliaSurveyQualificationDTO>> GetQualificationsAsync(int id)
    {
        var result = await _zampliaAPIController.GetZampliaSurveyQualifications(id);
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as List<ZampliaSurveyQualificationDTO> ?? new List<ZampliaSurveyQualificationDTO>()
            : new List<ZampliaSurveyQualificationDTO>();
    }

    private async Task<List<ZampliaSurveyQuotaDTO>> GetQuotasAsync(int id)
    {
        var result = await _zampliaAPIController.GetZampliaSurveyQuotas(id);
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as List<ZampliaSurveyQuotaDTO> ?? new List<ZampliaSurveyQuotaDTO>()
            : new List<ZampliaSurveyQuotaDTO>();
    }

    private async Task<ZampliaProjectMapDTO> GetProjectMapAsync(int id)
    {
        var result = await _zampliaAPIController.GetZampliaProjectMap(id);
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as ZampliaProjectMapDTO ?? new ZampliaProjectMapDTO()
            : new ZampliaProjectMapDTO();
    }

    private async Task<ZampliaEntryLinkDTO> GetEntryLinkAsync(int id)
    {
        var result = await _zampliaAPIController.GetZampliaEntryLink(id);
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as ZampliaEntryLinkDTO ?? new ZampliaEntryLinkDTO()
            : new ZampliaEntryLinkDTO();
    }

    private async Task<List<ZampliaRespondentAttemptDTO>> GetAttemptsAsync(int id)
    {
        var result = await _zampliaAPIController.GetZampliaRespondentAttemptsBySurvey(id);
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as List<ZampliaRespondentAttemptDTO> ?? new List<ZampliaRespondentAttemptDTO>()
            : new List<ZampliaRespondentAttemptDTO>();
    }

    private async Task<List<ZampliaSyncLogDTO>> GetLogsAsync(long? surveyId = null)
    {
        var result = await _zampliaAPIController.GetZampliaLogs();
        var logs = result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as List<ZampliaSyncLogDTO> ?? new List<ZampliaSyncLogDTO>()
            : new List<ZampliaSyncLogDTO>();

        return surveyId.HasValue ? logs.Where(log => log.RelatedSurveyId == surveyId.Value).ToList() : logs;
    }

    private async Task<List<ZampliaReconciliationRunDTO>> GetReconciliationRunsAsync()
    {
        var result = await _zampliaAPIController.GetZampliaReconciliationRuns();
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as List<ZampliaReconciliationRunDTO> ?? new List<ZampliaReconciliationRunDTO>()
            : new List<ZampliaReconciliationRunDTO>();
    }

    private async Task<List<ZampliaReconciliationItemDTO>> GetReconciliationItemsAsync(int id)
    {
        var result = await _zampliaAPIController.GetZampliaReconciliationItems(id);
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as List<ZampliaReconciliationItemDTO> ?? new List<ZampliaReconciliationItemDTO>()
            : new List<ZampliaReconciliationItemDTO>();
    }

    private async Task<List<ClientDTO>> GetClientsAsync()
    {
        var result = await _clientsAPIController.GetAllClients();
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as List<ClientDTO> ?? new List<ClientDTO>()
            : new List<ClientDTO>();
    }

    private async Task<List<CountryMasterDTO>> GetCountriesAsync()
    {
        var result = await _countryAPIController.GetCountries();
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as List<CountryMasterDTO> ?? new List<CountryMasterDTO>()
            : new List<CountryMasterDTO>();
    }

    private async Task<List<SupplierDTO>> GetSuppliersAsync()
    {
        var result = await _suppliersAPIController.GetAllSuppliers();
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as List<SupplierDTO> ?? new List<SupplierDTO>()
            : new List<SupplierDTO>();
    }
}
