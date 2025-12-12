
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using NextOnServices.Endpoints.Accounts;
using NextOnServices.Endpoints.Clients;
using NextOnServices.Endpoints.Masters;
using NextOnServices.Endpoints.Projects;
using Microsoft.AspNetCore.Http;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.Account;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.ViewModels.Project;
using System.Linq;

namespace NextOnServices.WebUI.VT.Controllers;
[Area("VT")]
public class ProjectsController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ProjectsAPIController _projectsAPIController;
    private readonly ClientsAPIController _clientsAPIController;
    private readonly AccountsController _accountAPIController;
    private readonly CountryAPIController _countryAPIController;
    private readonly StatusMasterAPIController _statusMasterAPIController;
    public ProjectsController(ILogger<HomeController> logger, ProjectsAPIController projectsAPIController, ClientsAPIController clientsAPIController, AccountsController accountAPIController, CountryAPIController countryAPIController, StatusMasterAPIController statusMasterAPIController)
    {
        _logger = logger;
        _projectsAPIController = projectsAPIController;
        _clientsAPIController = clientsAPIController;
        _accountAPIController = accountAPIController;
        _countryAPIController = countryAPIController;
        _statusMasterAPIController = statusMasterAPIController;
    }
    public IActionResult Index()
    {
        return View();
    }

    [Route("/VT/Projects/AddProject/{eProjectId=null}")]
    public async Task<IActionResult> AddProject(string? eProjectId = null)
    {
        AddProject addProject = new AddProject();
        ProjectDTO projectDTO = new ProjectDTO();
        List<ClientDTO> clientDTO = new List<ClientDTO>();
        List<UserDTO> userDTOs = new List<UserDTO>();
        List<CountryMasterDTO> countryMasterDTOs = new List<CountryMasterDTO>();
        List<StatusMasterDTO> statusMasterDTOs = new List<StatusMasterDTO>();
        try
        {
            if (eProjectId == null)
            {
                projectDTO.ProjectId = 0;
            }
            else if (eProjectId == "null")
            {
                projectDTO.ProjectId = 0;
            }
            else
            {
                projectDTO.ProjectId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eProjectId));
                var resProject = await _projectsAPIController.GetProjectById(projectDTO);
                if (resProject != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)resProject).StatusCode == 200)
                    {
                        projectDTO = (ProjectDTO)((Microsoft.AspNetCore.Mvc.ObjectResult)resProject).Value;
                    }
                }
            }

            var resClients = await _clientsAPIController.GetClients();
            if (resClients != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)resClients).StatusCode == 200)
                {
                    clientDTO = ((List<ClientDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)resClients).Value).OrderBy(x => x.Company).ToList();
                }
            }
            var resProjectManagers = await _accountAPIController.GetUsers();
            if (resProjectManagers != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)resProjectManagers).StatusCode == 200)
                {
                    userDTOs = ((List<UserDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)resProjectManagers).Value).OrderBy(x => x.UserName).ToList();
                }
            }
            var resCountries = await _countryAPIController.GetCountries();
            if (resCountries != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)resCountries).StatusCode == 200)
                {
                    countryMasterDTOs = ((List<CountryMasterDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)resCountries).Value);
                }
            }
            var resStatus = await _statusMasterAPIController.GetStatusMaster();
            if (resStatus != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)resStatus).StatusCode == 200)
                {
                    statusMasterDTOs = ((List<StatusMasterDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)resStatus).Value);
                }
            }

            if ((projectDTO.ProjectId == 0) && (!projectDTO.Status.HasValue || projectDTO.Status == 0))
            {
                var defaultStatus = statusMasterDTOs.FirstOrDefault(x => string.Equals(x.Pstatus, "Awarded", StringComparison.OrdinalIgnoreCase));
                if (defaultStatus?.Pvalue != null)
                {
                    projectDTO.Status = defaultStatus.Pvalue;
                }
            }
        }
        catch (Exception ex)
        {
            projectDTO.ProjectId = 0;
        }

        addProject.Project = projectDTO;
        addProject.ClientsList = clientDTO;
        addProject.UsersList = userDTOs;
        addProject.CountriesList = countryMasterDTOs;
        addProject.StatusList = statusMasterDTOs;
        return View(addProject);
    }

    [HttpPost]

    public async Task<IActionResult> ManageProject([FromBody] ProjectDTO inputDTO)
    {
        try
        {
            if (inputDTO.ProjectId > 0)
            {
                var res = await _projectsAPIController.UpdateProject(inputDTO);
                if (res != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                    {
                        return Ok("Project Updated Successfully");
                    }
                    else
                    {
                        throw new Exception(((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value.ToString());
                    }
                }
                else
                {
                    throw new Exception("Unable to update project");
                }
            }
            else
            {
                inputDTO.CreationDate = DateTime.Now;
                inputDTO.IsActive = 1;
                inputDTO.BlockDevice = "00000";
                var res = await _projectsAPIController.AddProject(inputDTO);
                if (res != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                    {
                        return Ok("Project Added Successfully");
                    }
                    else
                    {
                        throw new Exception(((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value.ToString());
                    }
                }
                else
                {
                    throw new Exception("Unable to update project");
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> ChangeProjectStatus([FromBody] ProjectDTO inputDTO)
    {
        try
        {
            if (inputDTO == null)
            {
                throw new ArgumentNullException(nameof(inputDTO));
            }

            var response = await _projectsAPIController.ChangeProjectStatus(inputDTO.Status ?? 0, inputDTO.ProjectId);

            if (response is ObjectResult objectResult)
            {
                if (objectResult.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok("Status Changed Successfully");
                }

                throw new Exception(objectResult.Value?.ToString() ?? "Unable to change project status");
            }

            throw new Exception("Unexpected response while changing project status");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public IActionResult ProjectsList()
    {
        return View();
    }

    [HttpPost]
    public async Task<JsonResult> GetProjectList()
    {
        var draw = Request.Form["draw"].FirstOrDefault();
        int drawValue = 0;
        int.TryParse(draw, out drawValue);

        try
        {
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnIndexStr = Request.Form["order[0][column]"].FirstOrDefault();
            int sortColumnIndex = 0;
            int.TryParse(sortColumnIndexStr, out sortColumnIndex);
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            PagedListParams pagedListParams = new PagedListParams();
            pagedListParams.searchValue = searchValue ?? string.Empty;
            pagedListParams.sortColumn = sortColumn;
            pagedListParams.sortColumnDirection = sortColumnDirection;
            pagedListParams.sortColumnIndex = sortColumnIndex;
            pagedListParams.skip = skip;
            pagedListParams.pageSize = pageSize;

            var resProject = await _projectsAPIController.GetProjectsList(pagedListParams);
            var resProjectCount = await _projectsAPIController.TotalProjectsFiltered(pagedListParams);

            // Always return a valid DataTables response format, even on errors
            if (resProject == null || !(resProject is ObjectResult objectResultProject) || objectResultProject.StatusCode != 200)
            {
                _logger.LogWarning("GetProjectList: resProject is null or invalid status code");
                return Json(new
                {
                    draw = drawValue,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<ListProject>()
                });
            }

            if (resProjectCount == null || !(resProjectCount is ObjectResult objectResultCount) || objectResultCount.StatusCode != 200)
            {
                _logger.LogWarning("GetProjectList: resProjectCount is null or invalid status code");
                return Json(new
                {
                    draw = drawValue,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<ListProject>()
                });
            }

            IEnumerable<ListProject> data = objectResultProject.Value as IEnumerable<ListProject> ?? new List<ListProject>();

            foreach (var item in data)
            {
                if (item != null)
                {
                    item.ProjectIdEnc = CommonHelper.EncryptURLHTML(item.ProjectId.ToString());
                }
            }

            List<int?> totalProjectCount = objectResultCount.Value as List<int?> ?? new List<int?>();
            int totalCount = totalProjectCount.FirstOrDefault() ?? 0;

            return Json(new
            {
                draw = drawValue,
                recordsTotal = totalCount,
                recordsFiltered = totalCount,
                data = data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetProjectList");
            // Always return a valid DataTables response format on exception
            return Json(new
            {
                draw = drawValue,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = new List<ListProject>()
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetProjectListTotals([FromBody] ProjectListExportRequest request)
    {
        try
        {
            var pagedListParams = BuildPagedListParams(request);
            var resProjectCount = await _projectsAPIController.TotalProjectsFiltered(pagedListParams);

            if (resProjectCount is not ObjectResult objectResult || objectResult.StatusCode != StatusCodes.Status200OK)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve project totals");
            }

            var totals = objectResult.Value as IEnumerable<int?>;
            var totalRecords = totals?.FirstOrDefault() ?? 0;

            return Ok(new { totalRecords });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetProjectListExportData([FromBody] ProjectListExportRequest request)
    {
        try
        {
            var pagedListParams = BuildPagedListParams(request);
            var resProject = await _projectsAPIController.GetProjectsList(pagedListParams);

            if (resProject is not ObjectResult objectResult || objectResult.StatusCode != StatusCodes.Status200OK)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve project data");
            }

            if (objectResult.Value is not IEnumerable<ListProject> data)
            {
                return Ok(Array.Empty<ListProject>());
            }

            var projectList = data.ToList();

            foreach (var item in projectList)
            {
                item.ProjectIdEnc = CommonHelper.EncryptURLHTML(item.ProjectId.ToString());
            }

            return Ok(projectList);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    private static PagedListParams BuildPagedListParams(ProjectListExportRequest request)
    {
        var safeRequest = request ?? new ProjectListExportRequest();

        return new PagedListParams
        {
            searchValue = safeRequest.SearchValue ?? string.Empty,
            sortColumn = string.IsNullOrWhiteSpace(safeRequest.SortColumn) ? "pid" : safeRequest.SortColumn,
            sortColumnDirection = string.IsNullOrWhiteSpace(safeRequest.SortDirection) ? "desc" : safeRequest.SortDirection,
            sortColumnIndex = safeRequest.SortColumnIndex ?? 0,
            skip = safeRequest.Start < 0 ? 0 : safeRequest.Start,
            pageSize = safeRequest.Length > 0 ? safeRequest.Length : 10
        };
    }

}

public class ProjectListExportRequest
{
    public int Start { get; set; }
    public int Length { get; set; }
    public string? SearchValue { get; set; }
    public string? SortColumn { get; set; }
    public string? SortDirection { get; set; }
    public int? SortColumnIndex { get; set; }
}
