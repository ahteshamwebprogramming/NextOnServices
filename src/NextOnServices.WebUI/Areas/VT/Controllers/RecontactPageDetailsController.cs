using Microsoft.AspNetCore.Mvc;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Infrastructure.Models.Projects;

namespace NextOnServices.WebUI.Areas.VT.Controllers;

[Area("VT")]
public class RecontactPageDetailsController : Controller
{
    private readonly ILogger<RecontactPageDetailsController> _logger;
    private readonly SurveyAPIController _surveyAPIController;

    public RecontactPageDetailsController(ILogger<RecontactPageDetailsController> logger, SurveyAPIController surveyAPIController)
    {
        _logger = logger;
        _surveyAPIController = surveyAPIController;
    }

    [HttpGet]
    [Route("/VT/RecontactPageDetails")]
    public IActionResult Index(int id)
    {
        ViewData["Title"] = "Recontact Details";
        ViewData["RecontactId"] = id;
        return View();
    }

    public class RecontactDetailsRequest
    {
        public int Id { get; set; }
        public int Opt { get; set; }
    }

    [HttpPost]
    [Route("/VT/RecontactPageDetails/FetchRecontactProjects")]
    public async Task<IActionResult> FetchRecontactProjects([FromBody] RecontactDetailsRequest request)
    {
        if (request == null || request.Id <= 0)
            return BadRequest(new { message = "Invalid request." });

        try
        {
            var details = await _surveyAPIController.GetRecontactDetails(request.Id, request.Opt);
            // Match legacy shape: array of 4 arrays [countryWise, supplierWise, redirects, remainingDetails]
            var payload = new object[]
            {
                details.Countries,
                details.Suppliers,
                details.Redirects,
                details.Summary
            };
            return Json(payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FetchRecontactProjects failed");
            return StatusCode(500, new { message = "Failed to load recontact details." });
        }
    }

    [HttpPost]
    [Route("/VT/RecontactPageDetails/UpdateRecontactProjects")]
    public async Task<IActionResult> UpdateRecontactProjects([FromBody] RecontactUpdateDTO formdata)
    {
        if (formdata == null || formdata.ID <= 0)
            return BadRequest(new { message = "Invalid request." });

        try
        {
            var result = await _surveyAPIController.UpdateRecontactProjects(formdata, 0);
            // Legacy returned plain string ("1", "0", "error")
            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateRecontactProjects failed");
            return StatusCode(500, new { message = "An error occurred while updating recontact details." });
        }
    }
}

