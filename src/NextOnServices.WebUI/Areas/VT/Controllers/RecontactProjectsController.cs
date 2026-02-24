using Microsoft.AspNetCore.Mvc;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Infrastructure.Models.Projects;

namespace NextOnServices.WebUI.Areas.VT.Controllers;

[Area("VT")]
public class RecontactProjectsController : Controller
{
    private readonly ILogger<RecontactProjectsController> _logger;
    private readonly SurveyAPIController _surveyAPIController;

    public RecontactProjectsController(ILogger<RecontactProjectsController> logger, SurveyAPIController surveyAPIController)
    {
        _logger = logger;
        _surveyAPIController = surveyAPIController;
    }

    [HttpGet]
    [Route("/VT/RecontactProjects")]
    public IActionResult Index()
    {
        ViewData["Title"] = "Recontact Projects";
        return View();
    }

    [HttpPost]
    [Route("/VT/RecontactProjects/GetProjects")]
    public async Task<IActionResult> GetProjects()
    {
        try
        {
            var list = await _surveyAPIController.GetRecontactProjectsList();
            return Json(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetProjects (Recontact) failed");
            return StatusCode(500, new { message = "Failed to load recontact projects." });
        }
    }

    public class RecontactStatusUpdateRequest
    {
        public int Id { get; set; }
        public int Status { get; set; }
    }

    [HttpPost]
    [Route("/VT/RecontactProjects/UpdateStatus")]
    public async Task<IActionResult> UpdateStatus([FromBody] RecontactStatusUpdateRequest request)
    {
        if (request == null || request.Id <= 0)
            return BadRequest(new { message = "Invalid request." });
        if (request.Status <= 0)
            return BadRequest(new { message = "Status is required." });

        try
        {
            var affected = await _surveyAPIController.UpdateRecontactProjectStatus(request.Id, request.Status);
            if (affected > 0)
                return Ok(new { message = "Updated successfully." });

            return StatusCode(500, new { message = "Unable to update status." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateStatus (Recontact) failed");
            return StatusCode(500, new { message = "An error occurred while updating status." });
        }
    }
}

