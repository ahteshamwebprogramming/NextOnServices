using Microsoft.AspNetCore.Mvc;
using NextOnServices.Endpoints.Projects;

namespace NextOnServices.WebUI.Areas.VT.Controllers;

[Area("VT")]
public class IdMappingController : Controller
{
    private readonly ILogger<IdMappingController> _logger;
    private readonly SurveyAPIController _surveyAPIController;

    public IdMappingController(ILogger<IdMappingController> logger, SurveyAPIController surveyAPIController)
    {
        _logger = logger;
        _surveyAPIController = surveyAPIController;
    }

    [HttpGet]
    [Route("/VT/ProjectInfoSearc.aspx")]
    [Route("/VT/IdMapping")]
    [Route("/VT/Projects/IdMapping")]
    public IActionResult Index()
    {
        ViewData["Title"] = "ID Mapping";
        return View();
    }

    [HttpPost]
    [Route("/VT/ProjectInfoSearc.aspx/Getstatus")]
    [Route("/VT/IdMapping/GetStatus")]
    public async Task<IActionResult> GetStatus([FromBody] IdMappingRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Id))
            return BadRequest(new { message = "Id is required." });

        if (request.Opt != 1 && request.Opt != 2)
            return BadRequest(new { message = "Invalid id type." });

        try
        {
            var list = await _surveyAPIController.GetMultipleStatus(request.Opt, request.Id.Trim());
            return Json(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetStatus failed for Id={Id}, Opt={Opt}", request.Id, request.Opt);
            return StatusCode(500, new { message = "An error occurred while fetching ID mapping." });
        }
    }
}

public class IdMappingRequest
{
    public int Opt { get; set; }
    public string Id { get; set; } = "";
}
