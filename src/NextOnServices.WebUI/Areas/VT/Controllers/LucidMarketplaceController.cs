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
public class LucidMarketplaceController : Controller
{
    private const string OpportunityViewRecent = LucidMarketplaceViewModel.OpportunityViewRecent;
    private const string OpportunityViewAdded = LucidMarketplaceViewModel.OpportunityViewAdded;
    private const string OpportunityViewAll = LucidMarketplaceViewModel.OpportunityViewAll;

    private readonly ILogger<LucidMarketplaceController> _logger;
    private readonly LucidMarketplaceAPIController _lucidMarketplaceAPIController;
    private readonly ClientsAPIController _clientsAPIController;
    private readonly CountryAPIController _countryAPIController;
    private readonly SuppliersAPIController _suppliersAPIController;

    public LucidMarketplaceController(
        ILogger<LucidMarketplaceController> logger,
        LucidMarketplaceAPIController lucidMarketplaceAPIController,
        ClientsAPIController clientsAPIController,
        CountryAPIController countryAPIController,
        SuppliersAPIController suppliersAPIController)
    {
        _logger = logger;
        _lucidMarketplaceAPIController = lucidMarketplaceAPIController;
        _clientsAPIController = clientsAPIController;
        _countryAPIController = countryAPIController;
        _suppliersAPIController = suppliersAPIController;
    }

    [Route("/VT/LucidMarketplace")]
    [Route("/VT/LucidMarketplace/Dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        ViewData["Title"] = "Lucid Marketplace Dashboard";

        var model = new LucidMarketplaceViewModel
        {
            Dashboard = await GetDashboardAsync(),
            Setting = await GetSettingAsync()
        };

        return View(model);
    }

    [Route("/VT/LucidMarketplace/Settings")]
    public async Task<IActionResult> Settings()
    {
        ViewData["Title"] = "Lucid Marketplace Settings";

        var model = new LucidMarketplaceViewModel
        {
            Setting = await GetSettingAsync(),
            Clients = await GetClientsAsync(),
            Countries = await GetCountriesAsync(),
            Suppliers = await GetSuppliersAsync()
        };

        return View(model);
    }

    [Route("/VT/LucidMarketplace/Subscriptions")]
    public async Task<IActionResult> Subscriptions()
    {
        ViewData["Title"] = "Lucid Marketplace Subscriptions";

        var model = new LucidMarketplaceViewModel
        {
            Setting = await GetSettingAsync(),
            Subscriptions = await GetSubscriptionsAsync()
        };

        return View(model);
    }

    [Route("/VT/LucidMarketplace/Logs")]
    public async Task<IActionResult> Logs()
    {
        ViewData["Title"] = "Lucid Marketplace Logs";

        var model = new LucidMarketplaceViewModel
        {
            Logs = await GetLogsAsync()
        };

        return View(model);
    }

    [Route("/VT/LucidMarketplace/Opportunities")]
    public async Task<IActionResult> Opportunities(string? view = null)
    {
        ViewData["Title"] = "Lucid Marketplace Opportunities";

        var normalizedView = NormalizeOpportunityView(view);
        var opportunities = await GetOpportunitiesAsync(normalizedView);

        var model = new LucidMarketplaceViewModel
        {
            Opportunities = opportunities,
            OpportunityView = normalizedView,
            OpportunityCount = opportunities.Count
        };

        return View(model);
    }

    [Route("/VT/LucidMarketplace/OpportunityDetails/{id:int}")]
    public async Task<IActionResult> OpportunityDetails(int id)
    {
        ViewData["Title"] = "Lucid Marketplace Opportunity Details";

        var model = new LucidMarketplaceViewModel
        {
            Opportunity = await GetOpportunityAsync(id),
            OpportunityQualifications = await GetOpportunityQualificationsAsync(id),
            OpportunityQuotas = await GetOpportunityQuotasAsync(id),
            OpportunityLogs = await GetOpportunityLogsAsync(id),
            EntryLink = await GetEntryLinkAsync(id),
            RespondentAttempts = await GetRespondentAttemptsAsync(id)
        };

        if (model.Opportunity.LucidMarketplaceOpportunityId <= 0)
        {
            return RedirectToAction(nameof(Opportunities));
        }

        return View(model);
    }

    [Route("/VT/LucidMarketplace/Outcomes")]
    public async Task<IActionResult> Outcomes()
    {
        ViewData["Title"] = "Lucid Marketplace Outcomes";

        var model = new LucidMarketplaceViewModel
        {
            Outcomes = await GetOutcomesAsync()
        };

        return View(model);
    }

    [Route("/VT/LucidMarketplace/OutcomeDetails/{id:int}")]
    public async Task<IActionResult> OutcomeDetails(int id)
    {
        ViewData["Title"] = "Lucid Marketplace Outcome Details";

        var outcome = await GetOutcomeAsync(id);
        if (outcome.Id <= 0)
        {
            return RedirectToAction(nameof(Outcomes));
        }

        var model = new LucidMarketplaceViewModel
        {
            Outcome = outcome,
            OutcomeHistory = await GetOutcomeHistoryAsync(id)
        };

        if (outcome.RelatedOpportunityId.HasValue && outcome.RelatedOpportunityId.Value > 0)
        {
            model.Opportunity = await GetOpportunityAsync(outcome.RelatedOpportunityId.Value);
        }

        if (outcome.RelatedOpportunityId.HasValue && outcome.RelatedOpportunityId.Value > 0)
        {
            model.RespondentAttempts = await GetRespondentAttemptsAsync(outcome.RelatedOpportunityId.Value);
        }

        return View(model);
    }

    [Route("/VT/LucidMarketplace/Reconciliation")]
    public async Task<IActionResult> Reconciliation(int? surveyId = null, int? internalProjectId = null, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        ViewData["Title"] = "Lucid Marketplace Reconciliation";

        var model = new LucidMarketplaceViewModel
        {
            ReconciliationFilter = new LucidMarketplaceReconciliationRunRequest
            {
                LucidSurveyId = surveyId,
                InternalProjectId = internalProjectId,
                DateFrom = dateFrom,
                DateTo = dateTo
            },
            ReconciliationRuns = await GetReconciliationRunsAsync()
        };

        return View(model);
    }

    [Route("/VT/LucidMarketplace/ReconciliationDetails/{runId:int}")]
    public async Task<IActionResult> ReconciliationDetails(int runId)
    {
        ViewData["Title"] = "Lucid Marketplace Reconciliation Details";

        var run = await GetReconciliationRunAsync(runId);
        if (run.Id <= 0)
        {
            return RedirectToAction(nameof(Reconciliation));
        }

        var model = new LucidMarketplaceViewModel
        {
            ReconciliationRun = run,
            ReconciliationItems = await GetReconciliationItemsAsync(runId)
        };

        return View(model);
    }

    private LucidMarketplaceViewModel BuildPlaceholderModel(string title, string message)
    {
        ViewData["Title"] = $"Lucid Marketplace {title}";

        return new LucidMarketplaceViewModel
        {
            PlaceholderTitle = title,
            PlaceholderMessage = message
        };
    }

    private async Task<LucidMarketplaceSettingDTO> GetSettingAsync()
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceSetting();
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as LucidMarketplaceSettingDTO ?? new LucidMarketplaceSettingDTO();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace setting view model");
        }

        return new LucidMarketplaceSettingDTO();
    }

    private async Task<LucidMarketplaceDashboardVM> GetDashboardAsync()
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceDashboard();
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as LucidMarketplaceDashboardVM ?? new LucidMarketplaceDashboardVM();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace dashboard view model");
        }

        return new LucidMarketplaceDashboardVM();
    }

    private async Task<List<LucidMarketplaceSubscriptionDTO>> GetSubscriptionsAsync()
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceSubscriptions();
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as List<LucidMarketplaceSubscriptionDTO> ?? new List<LucidMarketplaceSubscriptionDTO>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace subscriptions view model");
        }

        return new List<LucidMarketplaceSubscriptionDTO>();
    }

    private async Task<List<LucidMarketplaceSyncLogDTO>> GetLogsAsync()
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceLogs();
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as List<LucidMarketplaceSyncLogDTO> ?? new List<LucidMarketplaceSyncLogDTO>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace logs view model");
        }

        return new List<LucidMarketplaceSyncLogDTO>();
    }

    private async Task<List<LucidMarketplaceOpportunityDTO>> GetOpportunitiesAsync(string? view = null)
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceOpportunities(view);
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as List<LucidMarketplaceOpportunityDTO> ?? new List<LucidMarketplaceOpportunityDTO>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace opportunities view model");
        }

        return new List<LucidMarketplaceOpportunityDTO>();
    }

    private static string NormalizeOpportunityView(string? view)
    {
        if (string.Equals(view, OpportunityViewAdded, StringComparison.OrdinalIgnoreCase))
        {
            return OpportunityViewAdded;
        }

        if (string.Equals(view, OpportunityViewAll, StringComparison.OrdinalIgnoreCase))
        {
            return OpportunityViewAll;
        }

        return OpportunityViewRecent;
    }

    private async Task<LucidMarketplaceOpportunityDTO> GetOpportunityAsync(int id)
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceOpportunity(id);
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as LucidMarketplaceOpportunityDTO ?? new LucidMarketplaceOpportunityDTO();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace opportunity detail view model");
        }

        return new LucidMarketplaceOpportunityDTO();
    }

    private async Task<List<LucidMarketplaceOpportunityQualificationDTO>> GetOpportunityQualificationsAsync(int id)
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceOpportunityQualifications(id);
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as List<LucidMarketplaceOpportunityQualificationDTO> ?? new List<LucidMarketplaceOpportunityQualificationDTO>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace opportunity qualifications view model");
        }

        return new List<LucidMarketplaceOpportunityQualificationDTO>();
    }

    private async Task<List<LucidMarketplaceOpportunityQuotaDTO>> GetOpportunityQuotasAsync(int id)
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceOpportunityQuotas(id);
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as List<LucidMarketplaceOpportunityQuotaDTO> ?? new List<LucidMarketplaceOpportunityQuotaDTO>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace opportunity quotas view model");
        }

        return new List<LucidMarketplaceOpportunityQuotaDTO>();
    }

    private async Task<List<LucidMarketplaceSyncLogDTO>> GetOpportunityLogsAsync(int id)
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceOpportunityLogs(id);
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as List<LucidMarketplaceSyncLogDTO> ?? new List<LucidMarketplaceSyncLogDTO>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace opportunity logs view model");
        }

        return new List<LucidMarketplaceSyncLogDTO>();
    }

    private async Task<LucidMarketplaceEntryLinkDTO> GetEntryLinkAsync(int id)
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceEntryLink(id);
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as LucidMarketplaceEntryLinkDTO ?? new LucidMarketplaceEntryLinkDTO();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace entry-link view model");
        }

        return new LucidMarketplaceEntryLinkDTO();
    }

    private async Task<List<LucidMarketplaceRespondentAttemptDTO>> GetRespondentAttemptsAsync(int id)
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceRespondentAttempts(id);
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as List<LucidMarketplaceRespondentAttemptDTO> ?? new List<LucidMarketplaceRespondentAttemptDTO>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace respondent-attempt view model");
        }

        return new List<LucidMarketplaceRespondentAttemptDTO>();
    }

    private async Task<List<LucidMarketplaceRespondentOutcomeDTO>> GetOutcomesAsync()
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceRespondentOutcomes();
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as List<LucidMarketplaceRespondentOutcomeDTO> ?? new List<LucidMarketplaceRespondentOutcomeDTO>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace outcomes view model");
        }

        return new List<LucidMarketplaceRespondentOutcomeDTO>();
    }

    private async Task<LucidMarketplaceRespondentOutcomeDTO> GetOutcomeAsync(int id)
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceRespondentOutcome(id);
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as LucidMarketplaceRespondentOutcomeDTO ?? new LucidMarketplaceRespondentOutcomeDTO();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace outcome detail view model");
        }

        return new LucidMarketplaceRespondentOutcomeDTO();
    }

    private async Task<List<LucidMarketplaceRespondentOutcomeDTO>> GetOutcomeHistoryAsync(int id)
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceRespondentOutcomeHistory(id);
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as List<LucidMarketplaceRespondentOutcomeDTO> ?? new List<LucidMarketplaceRespondentOutcomeDTO>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace outcome history view model");
        }

        return new List<LucidMarketplaceRespondentOutcomeDTO>();
    }

    private async Task<List<ClientDTO>> GetClientsAsync()
    {
        var result = await _clientsAPIController.GetAllClients();
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as List<ClientDTO> ?? new List<ClientDTO>()
            : new List<ClientDTO>();
    }

    private async Task<List<LucidMarketplaceReconciliationRunDTO>> GetReconciliationRunsAsync()
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceReconciliationRuns();
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as List<LucidMarketplaceReconciliationRunDTO> ?? new List<LucidMarketplaceReconciliationRunDTO>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace reconciliation runs view model");
        }

        return new List<LucidMarketplaceReconciliationRunDTO>();
    }

    private async Task<LucidMarketplaceReconciliationRunDTO> GetReconciliationRunAsync(int runId)
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceReconciliationRun(runId);
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as LucidMarketplaceReconciliationRunDTO ?? new LucidMarketplaceReconciliationRunDTO();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace reconciliation detail view model");
        }

        return new LucidMarketplaceReconciliationRunDTO();
    }

    private async Task<List<LucidMarketplaceReconciliationItemDTO>> GetReconciliationItemsAsync(int runId)
    {
        try
        {
            var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceReconciliationItems(runId);
            if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
            {
                return objectResult.Value as List<LucidMarketplaceReconciliationItemDTO> ?? new List<LucidMarketplaceReconciliationItemDTO>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing Lucid Marketplace reconciliation items view model");
        }

        return new List<LucidMarketplaceReconciliationItemDTO>();
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
