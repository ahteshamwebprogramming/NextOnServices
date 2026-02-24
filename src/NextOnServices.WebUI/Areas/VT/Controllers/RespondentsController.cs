using Microsoft.AspNetCore.Mvc;
using NextOnServices.Endpoints.Projects;

namespace NextOnServices.WebUI.Areas.VT.Controllers;

[Area("VT")]
public class RespondentsController : Controller
{
    private readonly ILogger<RespondentsController> _logger;
    private readonly SurveyAPIController _surveyAPIController;

    public RespondentsController(ILogger<RespondentsController> logger, SurveyAPIController surveyAPIController)
    {
        _logger = logger;
        _surveyAPIController = surveyAPIController;
    }

    /// <summary>
    /// ID Search page (Respondents Report) - under Projects menu.
    /// </summary>
    [Route("/VT/Respondents/IDSearch")]
    [Route("/VT/Respondents")]
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "ID Search";
        return View();
    }

    [HttpPost]
    [Route("/VT/Respondents/GetRespondents")]
    public async Task<IActionResult> GetRespondents([FromBody] GetRespondentsRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Id))
        {
            return BadRequest(new { message = "Respondents Id is required." });
        }
        try
        {
            var list = await _surveyAPIController.GetRespondents(request.Id.Trim(), request.Type ?? "1");
            return Json(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetRespondents failed for Id={Id}, Type={Type}", request.Id, request.Type);
            return StatusCode(500, new { message = "An error occurred while searching. Please try again." });
        }
    }

    [HttpPost]
    [Route("/VT/Respondents/UpdateStatus")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateRespondentStatusRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Id))
        {
            return BadRequest(new { message = "Id is required." });
        }
        if (string.IsNullOrWhiteSpace(request.Status))
        {
            return BadRequest(new { message = "Status is required." });
        }
        try
        {
            var rows = await _surveyAPIController.UpdateRespondentStatus(request.Id.Trim(), request.Status.Trim());
            return Ok(new { updated = rows, message = rows > 0 ? "Updated successfully" : "No rows updated" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateStatus failed for Id={Id}, Status={Status}", request.Id, request.Status);
            return StatusCode(500, new { message = "An error occurred while updating status." });
        }
    }
}

public class GetRespondentsRequest
{
    public string Id { get; set; } = "";
    public string Type { get; set; } = "1";
}

public class UpdateRespondentStatusRequest
{
    public string Id { get; set; } = "";
    public string Status { get; set; } = "";
}
