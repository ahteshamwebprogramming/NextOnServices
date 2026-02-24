using Microsoft.AspNetCore.Mvc;
using NextOnServices.Endpoints.Projects;

namespace NextOnServices.WebUI.Areas.VT.Controllers;

[Area("VT")]
public class ManageStatusController : Controller
{
    private readonly ILogger<ManageStatusController> _logger;
    private readonly SurveyAPIController _surveyAPIController;

    public ManageStatusController(ILogger<ManageStatusController> logger, SurveyAPIController surveyAPIController)
    {
        _logger = logger;
        _surveyAPIController = surveyAPIController;
    }

    /// <summary>
    /// Reconciliation page (Manage Status).
    /// </summary>
    [Route("/VT/ManageStatus")]
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Reconciliation";
        return View();
    }

    [HttpPost]
    [Route("/VT/ManageStatus/GetProjects")]
    public async Task<IActionResult> GetProjects()
    {
        try
        {
            var list = await _surveyAPIController.GetProjectsForReconciliation();
            return Json(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetProjects failed");
            return StatusCode(500, new { message = "Failed to load projects." });
        }
    }

    [HttpPost]
    [Route("/VT/ManageStatus/UpdateStatus")]
    public async Task<IActionResult> UpdateStatus([FromBody] ReconciliationUpdateRequest request)
    {
        if (request == null)
            return BadRequest(new { message = "Invalid request." });
        if (string.IsNullOrWhiteSpace(request.Status))
            return BadRequest(new { message = "Status is required." });
        if (request.PId <= 0)
            return BadRequest(new { message = "Project is required." });
        try
        {
            var list = await _surveyAPIController.UpdateMultipleStatusReconciliation(
                request.Id ?? "", request.Status.Trim(), request.Type, request.PId);
            return Json(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateStatus failed");
            return StatusCode(500, new { message = "An error occurred while updating status." });
        }
    }

    [HttpPost]
    [Route("/VT/ManageStatus/ChangeAll")]
    public async Task<IActionResult> ChangeAll([FromBody] ReconciliationChangeAllRequest request)
    {
        if (request == null || request.PId <= 0)
            return BadRequest(new { message = "Project is required." });
        try
        {
            await _surveyAPIController.UpdateMultipleStatusReconciliation("0", request.Status ?? "Complete", 3, request.PId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ChangeAll failed");
            return StatusCode(500, new { message = "An error occurred." });
        }
    }
}

public class ReconciliationUpdateRequest
{
    public string Id { get; set; } = "";
    public string Status { get; set; } = "";
    public int Type { get; set; }
    public int PId { get; set; }
}

public class ReconciliationChangeAllRequest
{
    public string Status { get; set; } = "Complete";
    public int PId { get; set; }
}
