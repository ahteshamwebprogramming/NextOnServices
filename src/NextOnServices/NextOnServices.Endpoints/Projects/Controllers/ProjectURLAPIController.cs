using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.ViewModels.ProjectsURL;
using NextOnServices.Services.DBContext;
using System.Data;
using System.Security.Cryptography;

namespace NextOnServices.Endpoints.Projects;

[Route("api/[controller]")]
[ApiController]
public class ProjectURLAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProjectURLAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly DapperDBSetting _dapperDBSetting;
    public ProjectURLAPIController(IUnitOfWork unitOfWork, ILogger<ProjectURLAPIController> logger, IMapper mapper, DapperDBSetting dapperDBSetting)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dapperDBSetting = dapperDBSetting;
    }
    [HttpPost]
    public async Task<IActionResult> AddProjectURL(ProjectsUrlDTO inputData)
    {
        try
        {
            if (ModelState.IsValid && inputData != null)
            {
                Project project = await _unitOfWork.Project.FindByIdAsync(inputData.Pid ?? default(int));
                if (project != null)
                {
                    if (project.Ltype == 1)
                    {
                        string countQuery = "Select count(1) from ProjectsUrl where PID=@PID";
                        var countQueryParameters = new { PID = inputData.Pid };
                        if (await _unitOfWork.ProjectsUrl.GetEntityCount(countQuery, countQueryParameters) > 0)
                        {
                            return BadRequest("Only single country can be mapped with this project");
                        }
                    }
                    if (project.Ltype == 2)
                    {
                        string countQuery = "Select count(1) from ProjectsUrl where PID=@PID and CID=@CID";
                        var countQueryParameters = new { PID = inputData.Pid, CID = inputData.Cid };
                        if (await _unitOfWork.ProjectsUrl.GetEntityCount(countQuery, countQueryParameters) > 0)
                        {
                            return BadRequest("This country is already mapped with this project. Try mapping another country");
                        }
                    }
                }

                ProjectsUrl projectsUrl = _mapper.Map<ProjectsUrl>(inputData);
                int Id = await _unitOfWork.ProjectsUrl.AddAsync(projectsUrl);
                if (Id > 0)
                {
                    inputData.Id = Id;
                    if (inputData.TokenBool == true && !String.IsNullOrEmpty(inputData.TokenRaw))
                    {
                        char[] spliton = { '\n' };
                        string[] arrtokens = inputData.TokenRaw.Split(spliton, StringSplitOptions.RemoveEmptyEntries);
                        string tokenInsertQuery = "INSERT INTO tblTokens (projectUrlId, token) VALUES ";
                        List<string> values = new List<string>();
                        var insertParameters = new DynamicParameters();

                        int index = 0;
                        foreach (string singletoken in arrtokens)
                        {
                            string paramToken = singletoken.Trim();

                            string tokenExistenceQuery = "SELECT COUNT(1) FROM tblTokens WHERE projectUrlId = @projectUrlId AND token = @token";
                            var checkParams = new DynamicParameters();
                            checkParams.Add("@projectUrlId", Id);
                            checkParams.Add("@token", paramToken);
                            int count = await _unitOfWork.ProjectsUrl.GetEntityCount(tokenExistenceQuery, checkParams);
                            if (count == 0)
                            {
                                string paramTokenName = $"@token{index}";
                                string paramProjectIdName = $"@projectUrlId{index}";
                                values.Add($"({paramProjectIdName}, {paramTokenName})");
                                insertParameters.Add(paramProjectIdName, Id);
                                insertParameters.Add(paramTokenName, paramToken);

                                index++;
                            }
                        }
                        if (values.Count > 0)
                        {
                            tokenInsertQuery += string.Join(",", values);
                            bool isExecuted = await _unitOfWork.ProjectsUrl.ExecuteQueryAsync(tokenInsertQuery, insertParameters);
                            if (isExecuted)
                            {
                                return Ok(inputData);
                            }
                        }
                    }
                    return Ok(inputData);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AddProjectURL)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> UpdateProjectURL(ProjectsUrlDTO inputData)
    {
        try
        {
            if (ModelState.IsValid && inputData != null)
            {

                ProjectsUrl projectsUrl = await _unitOfWork.ProjectsUrl.FindByIdAsync(inputData.Id);
                projectsUrl.Cid = inputData.Cid;
                projectsUrl.Url = inputData.Url;
                projectsUrl.Token = inputData.TokenBool == true ? 1 : 0;
                projectsUrl.Quota = inputData.Quota;
                projectsUrl.Cpi = inputData.Cpi;
                projectsUrl.Notes = inputData.Notes;

                bool updated = await _unitOfWork.ProjectsUrl.UpdateAsync(projectsUrl);
                if (updated)
                {
                    if (inputData.TokenBool == true && !String.IsNullOrEmpty(inputData.TokenRaw))
                    {
                        int Id = inputData.Id;
                        char[] spliton = { '\n' };
                        string[] arrtokens = inputData.TokenRaw.Split(spliton, StringSplitOptions.RemoveEmptyEntries);
                        string tokenInsertQuery = "INSERT INTO tblTokens (projectUrlId, token) VALUES ";
                        List<string> values = new List<string>();
                        var insertParameters = new DynamicParameters();

                        int index = 0;
                        foreach (string singletoken in arrtokens)
                        {
                            string paramToken = singletoken.Trim();

                            string tokenExistenceQuery = "SELECT COUNT(1) FROM tblTokens WHERE projectUrlId = @projectUrlId AND token = @token";
                            var checkParams = new DynamicParameters();
                            checkParams.Add("@projectUrlId", Id);
                            checkParams.Add("@token", paramToken);
                            int count = await _unitOfWork.ProjectsUrl.GetEntityCount(tokenExistenceQuery, checkParams);
                            if (count == 0)
                            {
                                string paramTokenName = $"@token{index}";
                                string paramProjectIdName = $"@projectUrlId{index}";
                                values.Add($"({paramProjectIdName}, {paramTokenName})");
                                insertParameters.Add(paramProjectIdName, Id);
                                insertParameters.Add(paramTokenName, paramToken);

                                index++;
                            }
                        }
                        if (values.Count > 0)
                        {
                            tokenInsertQuery += string.Join(",", values);
                            bool isExecuted = await _unitOfWork.ProjectsUrl.ExecuteQueryAsync(tokenInsertQuery, insertParameters);
                            if (isExecuted)
                            {
                                return Ok(inputData);
                            }
                        }
                    }


                    return Ok(inputData);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AddProjectURL)}");
            throw;
        }
    }
    public async Task<IActionResult> GetProjectURLByProjectId(int projectId)
    {
        try
        {
            string query = "Select \r\np.PID ProjectCode\r\n,p.PName ProjectName\r\n,cm.Country\r\n,pu.* \r\nfrom \r\nProjectsUrl pu \r\njoin Projects p on pu.PID=p.ProjectId\r\nJoin CountryMaster cm on pu.CID=cm.CountryId\r\nwhere pu.PID=@PID";
            var parameters = new { PID = projectId };
            var res = await _unitOfWork.ProjectsUrl.GetTableData<ProjectURLWithChild>(query, parameters);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AddProjectURL)}");
            throw;
        }
    }
    public async Task<IActionResult> GetProjectURLById(int Id)
    {
        try
        {
            string query = "Select \r\np.PID ProjectCode\r\n,p.PName ProjectName\r\n,cm.Country\r\n,pu.* \r\nfrom \r\nProjectsUrl pu \r\njoin Projects p on pu.PID=p.ProjectId\r\nJoin CountryMaster cm on pu.CID=cm.CountryId\r\nwhere pu.ID=@ID";
            var parameters = new { ID = Id };
            var res = await _unitOfWork.ProjectsUrl.GetTableData<ProjectsUrlDTO>(query, parameters);
            if (res != null && res.Count > 0)
            {
                return Ok(res.FirstOrDefault());
            }
            else
            {
                return BadRequest("No record found");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AddProjectURL)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> UploadTokens(ProjectsUrlDTO inputData)
    {
        try
        {
            if (inputData != null)
            {
                if (!String.IsNullOrEmpty(inputData.TokenRaw))
                {
                    int Id = inputData.Id;
                    char[] spliton = { '\n' };
                    string[] arrtokens = inputData.TokenRaw.Split(spliton, StringSplitOptions.RemoveEmptyEntries);
                    string tokenInsertQuery = "INSERT INTO tblTokens (projectUrlId, token) VALUES ";
                    List<string> values = new List<string>();
                    var insertParameters = new DynamicParameters();

                    int index = 0;
                    foreach (string singletoken in arrtokens)
                    {
                        string paramToken = singletoken.Trim();

                        string tokenExistenceQuery = "SELECT COUNT(1) FROM tblTokens WHERE projectUrlId = @projectUrlId AND token = @token";
                        var checkParams = new DynamicParameters();
                        checkParams.Add("@projectUrlId", Id);
                        checkParams.Add("@token", paramToken);
                        int count = await _unitOfWork.ProjectsUrl.GetEntityCount(tokenExistenceQuery, checkParams);
                        if (count == 0)
                        {
                            string paramTokenName = $"@token{index}";
                            string paramProjectIdName = $"@projectUrlId{index}";
                            values.Add($"({paramProjectIdName}, {paramTokenName})");
                            insertParameters.Add(paramProjectIdName, Id);
                            insertParameters.Add(paramTokenName, paramToken);

                            index++;
                        }
                    }
                    if (values.Count > 0)
                    {
                        tokenInsertQuery += string.Join(",", values);
                        bool isExecuted = await _unitOfWork.ProjectsUrl.ExecuteQueryAsync(tokenInsertQuery, insertParameters);
                        if (isExecuted)
                        {
                            return Ok(inputData);
                        }
                    }
                }
                return BadRequest("Some error has occurred");
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AddProjectURL)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> ViewTokens(ProjectsUrlDTO inputData)
    {
        try
        {
            if (inputData != null)
            {
                int Id = inputData.Id;
                string query = "Select * from tblTokens where ProjectURLId=@ProjectURLId";
                var parameter = new { ProjectURLId = Id };
                var res = await _unitOfWork.ProjectsUrl.GetTableData<tblTokensDTO>(query, parameter);
                return Ok(res);
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AddProjectURL)}");
            throw;
        }
    }

}
