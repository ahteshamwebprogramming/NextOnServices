using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NextOnServices.Endpoints.Clients;
using NextOnServices.Endpoints.Masters;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Endpoints.Suppliers;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.Models.Supplier;

namespace NextOnServices.WebUI.Areas.VT.Controllers;

[Area("VT")]
[Authorize(Roles = "A")]
public class ReportsController : Controller
{
    private readonly ILogger<ReportsController> _logger;
    private readonly SurveyAPIController _surveyAPIController;
    private readonly CountryAPIController _countryAPIController;
    private readonly ClientsAPIController _clientsAPIController;
    private readonly SuppliersAPIController _suppliersAPIController;

    public ReportsController(
        ILogger<ReportsController> logger,
        SurveyAPIController surveyAPIController,
        CountryAPIController countryAPIController,
        ClientsAPIController clientsAPIController,
        SuppliersAPIController suppliersAPIController)
    {
        _logger = logger;
        _surveyAPIController = surveyAPIController;
        _countryAPIController = countryAPIController;
        _clientsAPIController = clientsAPIController;
        _suppliersAPIController = suppliersAPIController;
    }

    [HttpGet]
    [Route("/VT/OverallReport.aspx")]
    [Route("/VT/Reports/Overall")]
    public IActionResult OverallReport()
    {
        ViewData["Title"] = "Overall Report";
        return View();
    }

    [HttpPost]
    [Route("/VT/OverallReport.aspx/GetFilterOptions")]
    public async Task<IActionResult> GetFilterOptions()
    {
        try
        {
            var countriesRes = await _countryAPIController.GetCountries();
            var clientsRes = await _clientsAPIController.GetClients();
            var suppliersRes = await _suppliersAPIController.GetSuppliers();

            var countries = ((countriesRes as ObjectResult)?.Value as IEnumerable<CountryMasterDTO> ?? Enumerable.Empty<CountryMasterDTO>())
                .Where(x => x.CountryId > 0)
                .Select(x => new { id = x.CountryId, name = x.Country ?? "" })
                .OrderBy(x => x.name)
                .ToList();

            var clients = ((clientsRes as ObjectResult)?.Value as IEnumerable<ClientDTO> ?? Enumerable.Empty<ClientDTO>())
                .Where(x => x.ClientId > 0 && !string.IsNullOrWhiteSpace(x.Company))
                .Select(x => new { id = x.ClientId, name = x.Company ?? "" })
                .OrderBy(x => x.name)
                .ToList();

            var suppliers = ((suppliersRes as ObjectResult)?.Value as IEnumerable<SupplierDTO> ?? Enumerable.Empty<SupplierDTO>())
                .Where(x => x.Id > 0 && !string.IsNullOrWhiteSpace(x.Name))
                .Select(x => new { id = x.Id, name = x.Name ?? "" })
                .OrderBy(x => x.name)
                .ToList();

            return Json(new
            {
                countries,
                clients,
                suppliers
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetFilterOptions failed");
            return StatusCode(500, new { message = "Unable to load report filters." });
        }
    }

    [HttpPost]
    [Route("/VT/OverallReport.aspx/GetDashboardData")]
    public async Task<IActionResult> GetDashboardData([FromBody] OverallReportRequest? request)
    {
        try
        {
            request ??= new OverallReportRequest();
            var filter = new OverallReportFilterDTO
            {
                CountryId = request.CountryId,
                ClientId = request.ClientId,
                SupplierId = request.SupplierId,
                SDate = request.SDate,
                EDate = request.EDate
            };

            var data = await _surveyAPIController.GetOverallReportSnapshot(filter);
            return Json(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetDashboardData failed");
            return Json(new OverallReportSnapshotDTO());
        }
    }

    [HttpGet]
    [Route("/VT/ProjectWiseReport.aspx")]
    [Route("/VT/Reports/ProjectWise")]
    public IActionResult ProjectWiseReport()
    {
        ViewData["Title"] = "Project Wise Report";
        return View();
    }

    [HttpPost]
    [Route("/VT/ProjectWiseReport.aspx/GetProjects")]
    public async Task<IActionResult> GetProjects()
    {
        try
        {
            var list = await _surveyAPIController.GetProjectsForProjectWiseReport();
            return Json(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetProjects (ProjectWiseReport) failed");
            return StatusCode(500, new { message = "Unable to load projects." });
        }
    }

    [HttpPost]
    [Route("/VT/ProjectWiseReport.aspx/GetProjectDetails")]
    public async Task<IActionResult> GetProjectDetails([FromBody] ProjectWiseReportRequest? request)
    {
        if (request == null || request.Id <= 0)
            return BadRequest(new { message = "Please select a project." });

        try
        {
            var data = await _surveyAPIController.GetProjectWiseReportDetails(request.Id);
            return Json(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetProjectDetails (ProjectWiseReport) failed for project {ProjectId}", request.Id);
            return StatusCode(500, new { message = "Unable to load project details." });
        }
    }
}

public class OverallReportRequest
{
    public int CountryId { get; set; }
    public int ClientId { get; set; }
    public int SupplierId { get; set; }
    public DateTime? SDate { get; set; }
    public DateTime? EDate { get; set; }
}

public class ProjectWiseReportRequest
{
    public int Id { get; set; }
}
