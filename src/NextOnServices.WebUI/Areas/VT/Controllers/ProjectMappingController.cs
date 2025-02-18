using Microsoft.AspNetCore.Mvc;
using NextOnServices.Endpoints.Masters;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Endpoints.Suppliers;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.Models.Supplier;
using NextOnServices.Infrastructure.ViewModels.ProjectMapping;
using NextOnServices.Infrastructure.ViewModels.ProjectsURL;
using System;
using System.Configuration;

namespace NextOnServices.WebUI.VT.Controllers;
[Area("VT")]
public class ProjectMappingController : Controller
{
    private readonly ILogger<ProjectMappingController> _logger;
    private readonly ProjectsAPIController _projectsAPIController;
    private readonly ProjectURLAPIController _projectURLAPIController;
    private readonly ProjectMappingAPIController _projectMappingAPIController;
    private readonly CountryAPIController _countryAPIController;
    private readonly SuppliersAPIController _suppliersAPIController;
    private readonly IConfiguration _config;
    public ProjectMappingController(ILogger<ProjectMappingController> logger, ProjectsAPIController projectsAPIController, CountryAPIController countryAPIController, ProjectURLAPIController projectURLAPIController, ProjectMappingAPIController projectMappingAPIController, SuppliersAPIController suppliersAPIController, IConfiguration config)
    {
        _logger = logger;
        _projectsAPIController = projectsAPIController;
        _projectURLAPIController = projectURLAPIController;
        _countryAPIController = countryAPIController;
        _projectMappingAPIController = projectMappingAPIController;
        _suppliersAPIController = suppliersAPIController;
        _config = config;
    }
    [Route("/VT/Projects/ProjectMapping/{eProjectId=null}")]
    public async Task<IActionResult> ProjectMapping(string? eProjectId = null)
    {
        ProjectMappingViewModel? dto = new ProjectMappingViewModel();

        ProjectMappingWithChild projectMappingWithChild = new ProjectMappingWithChild();
        projectMappingWithChild.encProjectId = eProjectId;
        dto.ProjectMappingWithChild = projectMappingWithChild;
        return View(dto);
    }

    public async Task<IActionResult> AddProjectMappingPartialView([FromBody] ProjectMappingDTO inputDTO)
    {
        ProjectMappingViewModel? dto = new ProjectMappingViewModel();
        ProjectMappingWithChild? projectMappingWithChild = new ProjectMappingWithChild();
        try
        {
            if (inputDTO.Id > 0)
            {
                var res = await _projectMappingAPIController.GetProjectMappingById(inputDTO.Id);
                if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    projectMappingWithChild = (ProjectMappingWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    dto.ProjectMappingWithChild = projectMappingWithChild;
                    if (dto.ProjectMappingWithChild != null)
                    {
                        dto.ProjectMappingWithChild.AddHashingBool = dto.ProjectMappingWithChild.AddHashing == 1 ? true : false;
                    }
                }
            }
            if (inputDTO.encProjectId != null)
            {
                int projectId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encProjectId));
                var resCountries = await _projectMappingAPIController.GetProjectURLMappedCountries(projectId);
                if (resCountries != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resCountries).StatusCode == 200)
                {
                    dto.Countries = ((List<CountryMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resCountries).Value);
                }
                var resSuppliers = await _suppliersAPIController.GetSuppliers();
                if (resSuppliers != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resSuppliers).StatusCode == 200)
                {
                    dto.Suppliers = ((List<SupplierDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resSuppliers).Value);
                }
                var resProject = await _projectsAPIController.GetProjectById(projectId);
                if (resProject != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)resProject).StatusCode == 200)
                    {
                        dto.Project = (ProjectDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resProject).Value;
                    }
                }
                projectMappingWithChild.ParameterName = "enc";
                dto.ProjectMappingWithChild = projectMappingWithChild;
            }
            return PartialView("_projectMapping/_addProjectMapping", dto);
        }
        catch (Exception ex)
        {
            return PartialView("_projectMapping/_addProjectMapping", dto);
        }

    }
    public async Task<IActionResult> ListProjectMappingPartialView([FromBody] ProjectMappingDTO inputDTO)
    {
        ProjectMappingViewModel? dto = new ProjectMappingViewModel();
        if (inputDTO.encProjectId != null)
        {
            int projectId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encProjectId));
            var res = await _projectMappingAPIController.GetProjectMappingByProjectId(projectId);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.ProjectMappingWithChildList = (List<ProjectMappingWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return PartialView("_projectMapping/_listProjectMapping", dto);
    }
    public async Task<IActionResult> SaveProjectMapping([FromBody] ProjectMappingDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.encProjectId != null)
                {
                    inputDTO.ProjectId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encProjectId));
                    if (inputDTO.Id > 0)
                    {
                        inputDTO.AddHashing = inputDTO.AddHashingBool == true ? 1 : 0;
                        var res = await _projectMappingAPIController.UpdateProjectMapping(inputDTO);
                        return res;
                    }
                    else
                    {
                        if (inputDTO.encProjectId != null)
                        {
                            string KID = CommonHelper.RandomString(32);
                            string SID = CommonHelper.RandomString(8);
                            string? MURL = _config.GetValue<string>("MaskingUrl");
                            inputDTO.Mlink = $"{MURL}?SID={SID}&ID=XXXXXXXXXX";
                            string OUrl = ""; //Select URL from ProjectURL with countryid and projectid
                            inputDTO.Sid = SID;
                            inputDTO.Code = KID;
                            inputDTO.AddHashing = inputDTO.AddHashingBool == true ? 1 : 0;
                            var res = await _projectMappingAPIController.AddProjectMapping(inputDTO);
                            return res;
                        }
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
    public async Task<IActionResult> CheckProjectMapping([FromBody] ProjectMappingDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.Id > 0)
                {
                    var res = await _projectMappingAPIController.CheckProjectMapping(inputDTO.Id);
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
