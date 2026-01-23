using Microsoft.AspNetCore.Mvc;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Endpoints.Questionnaire;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.Models.Questionnaire;
using NextOnServices.Infrastructure.ViewModels.ProjectQuestionsMapping;

namespace NextOnServices.WebUI.VT.Controllers;

[Area("VT")]
public class ProjectQuestionsMappingController : Controller
{
    private readonly ILogger<ProjectQuestionsMappingController> _logger;
    private readonly ProjectsAPIController _projectsAPIController;
    private readonly ProjectMappingAPIController _projectMappingAPIController;
    private readonly QuestionnaireAPIController _questionnaireAPIController;

    public ProjectQuestionsMappingController(
        ILogger<ProjectQuestionsMappingController> logger,
        ProjectsAPIController projectsAPIController,
        ProjectMappingAPIController projectMappingAPIController,
        QuestionnaireAPIController questionnaireAPIController)
    {
        _logger = logger;
        _projectsAPIController = projectsAPIController;
        _projectMappingAPIController = projectMappingAPIController;
        _questionnaireAPIController = questionnaireAPIController;
    }

    [Route("/VT/PreScreening/Mapping/{eProjectId?}")]
    public async Task<IActionResult> Mapping(string? eProjectId = null)
    {
        ProjectQuestionsMappingViewModel dto = new ProjectQuestionsMappingViewModel();
        
        if (!string.IsNullOrEmpty(eProjectId))
        {
            int projectId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eProjectId));
            
            // Get project details
            var resProject = await _projectsAPIController.GetProjectById(projectId);
            if (resProject != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resProject).StatusCode == 200)
            {
                dto.Project = (ProjectDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resProject).Value;
            }
            
            // Store encrypted project ID for use in view
            ViewBag.ProjectIdEnc = eProjectId;
            
            // Get countries mapped with project
            var resCountries = await _projectMappingAPIController.GetProjectURLMappedCountries(projectId);
            if (resCountries != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resCountries).StatusCode == 200)
            {
                dto.Countries = ((List<NextOnServices.Infrastructure.Models.Masters.CountryMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resCountries).Value);
            }
            
            // Get all questions
            var resQuestions = await _projectMappingAPIController.GetAllQuestions();
            if (resQuestions != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resQuestions).StatusCode == 200)
            {
                dto.Questions = ((List<QuestionsMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resQuestions).Value);
            }
        }
        
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> GetCountriesByProject([FromBody] ProjectQuestionsMappingDTO inputDTO)
    {
        try
        {
            if (inputDTO?.encProjectId != null)
            {
                int projectId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encProjectId));
                var res = await _projectMappingAPIController.GetProjectURLMappedCountries(projectId);
                if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    var countries = ((List<NextOnServices.Infrastructure.Models.Masters.CountryMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value);
                    return Ok(countries);
                }
            }
            return Ok(new List<NextOnServices.Infrastructure.Models.Masters.CountryMasterDTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting countries by project");
            return BadRequest("Error retrieving countries");
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetQuestions()
    {
        try
        {
            var res = await _projectMappingAPIController.GetAllQuestions();
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                var questions = ((List<QuestionsMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value);
                return Ok(questions);
            }
            return Ok(new List<QuestionsMasterDTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting questions");
            return BadRequest("Error retrieving questions");
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetProjectQuestionsMapping([FromBody] ProjectQuestionsMappingDTO inputDTO)
    {
        try
        {
            if (inputDTO?.encProjectId != null && inputDTO.Cid.HasValue)
            {
                int projectId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encProjectId));
                var res = await _projectMappingAPIController.GetProjectQuestionsMappingByProjectAndCountry(projectId, inputDTO.Cid.Value);
                if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    var mapping = ((ProjectQuestionsMappingDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value);
                    return Ok(mapping);
                }
            }
            return Ok((ProjectQuestionsMappingDTO?)null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting project questions mapping");
            return BadRequest("Error retrieving mapping");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveProjectQuestionsMapping([FromBody] ProjectQuestionsMappingDTO inputDTO)
    {
        try
        {
            if (inputDTO?.encProjectId != null)
            {
                int projectId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encProjectId));
                inputDTO.Pid = projectId;
                
                var res = await _projectMappingAPIController.SaveProjectQuestionsMapping(inputDTO);
                return res;
            }
            return BadRequest("Invalid request");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving project questions mapping");
            return BadRequest("Error saving mapping");
        }
    }
}

