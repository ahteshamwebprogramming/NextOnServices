using GRP.Endpoints.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using NextOnServices.Core.Entities;
using NextOnServices.Endpoints.Accounts;
using NextOnServices.Endpoints.Clients;
using NextOnServices.Endpoints.Masters;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.ViewModels.ProjectsURL;

namespace NextOnServices.WebUI.VT.Controllers;
[Area("VT")]
public class ProjectURLsController : Controller
{
    private readonly ILogger<ProjectURLsController> _logger;
    private readonly ProjectsAPIController _projectsAPIController;
    private readonly ProjectURLAPIController _projectURLAPIController;
    private readonly CountryAPIController _countryAPIController;
    public ProjectURLsController(ILogger<ProjectURLsController> logger, ProjectsAPIController projectsAPIController, CountryAPIController countryAPIController, ProjectURLAPIController projectURLAPIController)
    {
        _logger = logger;
        _projectsAPIController = projectsAPIController;
        _projectURLAPIController = projectURLAPIController;
        _countryAPIController = countryAPIController;
    }

    [Route("/VT/Projects/ProjectUrls/{eProjectId=null}")]
    public async Task<IActionResult> ProjectUrls(string? eProjectId = null)
    {
        ProjectsURLViewModel? dto = new ProjectsURLViewModel();
        //var resCountries = await _countryAPIController.GetCountries();
        //if (resCountries != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resCountries).StatusCode == 200)
        //{
        //    dto.Countries = ((List<CountryMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resCountries).Value);
        //}
        ProjectsUrlDTO projectsUrlDTO = new ProjectsUrlDTO();
        projectsUrlDTO.encProjectId = eProjectId;
        dto.ProjectsURL = projectsUrlDTO;
        return View(dto);
    }
    public async Task<IActionResult> AddProjectURLPartialView([FromBody] ProjectsUrlDTO inputDTO)
    {
        ProjectsURLViewModel? dto = new ProjectsURLViewModel();
        try
        {
            var resCountries = await _countryAPIController.GetCountries();
            if (resCountries != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resCountries).StatusCode == 200)
            {
                dto.Countries = ((List<CountryMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resCountries).Value);
            }
            if (inputDTO.Id > 0)
            {
                var res = await _projectURLAPIController.GetProjectURLById(inputDTO.Id);
                if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto.ProjectsURL = (ProjectsUrlDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    if (dto.ProjectsURL != null)
                    {
                        dto.ProjectsURL.TokenBool = dto.ProjectsURL.Token == 1 ? true : false;
                    }
                }
            }
            if (inputDTO.encProjectId != null)
            {
                int projectId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encProjectId));
                var resProject = await _projectsAPIController.GetProjectById(projectId);
                if (resProject != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)resProject).StatusCode == 200)
                    {
                        dto.Project = (ProjectDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resProject).Value;
                    }
                }
            }
            return PartialView("_projectURLs/_addProjectURL", dto);
        }
        catch (Exception ex)
        {
            return PartialView("_projectURLs/_addProjectURL", dto);
        }

    }
    public async Task<IActionResult> ListProjectURLPartialView([FromBody] ProjectsUrlDTO inputDTO)
    {
        ProjectsURLViewModel? dto = new ProjectsURLViewModel();
        if (inputDTO.encProjectId != null)
        {
            int projectId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encProjectId));
            var res = await _projectURLAPIController.GetProjectURLByProjectId(projectId);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.ProjectURLWithChildList = (List<ProjectURLWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return PartialView("_projectURLs/_listProjectURL", dto);
    }
    public async Task<IActionResult> SaveProjectURL([FromBody] ProjectsUrlDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.Id > 0)
                {
                    if (inputDTO.encProjectId != null)
                    {
                        inputDTO.Pid = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encProjectId));
                        var res = await _projectURLAPIController.UpdateProjectURL(inputDTO);
                        return res;
                    }
                }
                else
                {
                    if (inputDTO.encProjectId != null)
                    {
                        inputDTO.Pid = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encProjectId));
                        inputDTO.CreationDate = DateTime.Now;
                        inputDTO.Status = 5;
                        inputDTO.Token = inputDTO.TokenBool == true ? 1 : 0;
                        var res = await _projectURLAPIController.AddProjectURL(inputDTO);
                        return res;
                    }
                }
            }
            return BadRequest("Unable To Save right now");
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    public async Task<IActionResult> UploadTokensFromModal([FromBody] ProjectsUrlDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.Id > 0)
                {
                    var res = await _projectURLAPIController.UploadTokens(inputDTO);
                    return res;
                }
            }
            return BadRequest("Unable To Save right now");
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    public async Task<IActionResult> ViewTokensFromModal([FromBody] ProjectsUrlDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.Id > 0)
                {
                    var res = await _projectURLAPIController.ViewTokens(inputDTO);
                    return res;
                }
            }
            return BadRequest("Unable To Save right now");
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

}
