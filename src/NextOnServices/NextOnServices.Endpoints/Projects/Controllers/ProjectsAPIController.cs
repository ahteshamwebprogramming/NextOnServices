using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.Account;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.ViewModels.Dashboard;
using NextOnServices.Infrastructure.ViewModels.Project;
using NextOnServices.Infrastructure.ViewModels.ProjectDetails;
using NextOnServices.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing.Printing;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace NextOnServices.Endpoints.Projects;

[Route("api/[controller]")]
[ApiController]
public class ProjectsAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProjectsAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly DapperDBSetting _dapperDBSetting;
    public ProjectsAPIController(IUnitOfWork unitOfWork, ILogger<ProjectsAPIController> logger, IMapper mapper, DapperDBSetting dapperDBSetting)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dapperDBSetting = dapperDBSetting;

    }
    //[HttpPost]
    //public async Task<IActionResult> GetProjects1()
    //{
    //    try
    //    {
    //        List<ProjectDTO> outputDTO = _mapper.Map<List<ProjectDTO>>(_unitOfWork.Project.GetAll( null, orderBy: (m => m.OrderByDescending(x => x.Pid))).Result.ToList());
    //        return Ok(outputDTO);


    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetProjects)}");
    //        throw;
    //    }
    //}


    [HttpPost]
    public async Task<IActionResult> GetProjectsList(PagedListParams pagedListParams)
    {
        try
        {
            try
            {
                ////var result = await _unitOfWork.Project.GetStoredProcedure("GetDashboardbyManager", parms);
                //var s = await _unitOfWork.GProject.ProjectsListPaged(pagedListParams);
                string orderbyColumn = "";
                pagedListParams.searchValue = pagedListParams.searchValue.ToLower();
                pagedListParams.sortColumnDirection = pagedListParams.sortColumnDirection == null ? "desc" : pagedListParams.sortColumnDirection;
                orderbyColumn = pagedListParams == null ? "pid" : pagedListParams.sortColumn == null ? "pid" : pagedListParams.sortColumn == "pid" ? "pid" : pagedListParams.sortColumn == "pname" ? "pname" : pagedListParams.sortColumn == "loi" ? "loi" : "pid";

                string sWhere = $" where (lower(PID) like('%" + pagedListParams.searchValue + "%') or lower(PName) like('%" + pagedListParams.searchValue + "%') or lower(c.Company) like('%" + pagedListParams.searchValue + "%') or lower(u.Username) like('%" + pagedListParams.searchValue + "%') or lower(LOI) like('%" + pagedListParams.searchValue + "%') or lower(Convert(nvarchar,Convert(date,SDate,101),106)) like('%" + pagedListParams.searchValue + "%') or lower(Convert(nvarchar,Convert(date,EDate,101),106)) like('%" + pagedListParams.searchValue + "%')) and p.isactive=1 ";
                string sOrderBy = " ORDER BY " + orderbyColumn + " " + pagedListParams.sortColumnDirection;
                string sPaging = " OFFSet " + pagedListParams.skip + " rows fetch next " + pagedListParams.pageSize + " rows only";
                string sQuery = "select ProjectId,PID,PName,c.Company,u.Username,LOI,Convert(nvarchar,Convert(date,SDate,101),106)sdate,Convert(nvarchar,Convert(date,EDate,101),106)edate from Projects P join Clients c on p.ClientID = c.clientId Join Users u on p.pmanager=u.UserID" + sWhere + sOrderBy + sPaging;
                List<ListProject> data = _mapper.Map<List<ListProject>>(await _unitOfWork.Project.GetTableData<ListProject>(sQuery));
                return Ok(data);
            }
            catch (Exception ex)
            {
                return null;
            }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetProjects)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> TotalProjectsFiltered(PagedListParams pagedListParams)
    {
        try
        {
            try
            {
                pagedListParams.searchValue = pagedListParams.searchValue.ToLower();
                string sWhere = $" where (lower(PID) like('%" + pagedListParams.searchValue + "%') or lower(PName) like('%" + pagedListParams.searchValue + "%') or lower(c.Company) like('%" + pagedListParams.searchValue + "%') or lower(u.Username) like('%" + pagedListParams.searchValue + "%') or lower(LOI) like('%" + pagedListParams.searchValue + "%') or lower(Convert(nvarchar,Convert(date,SDate,101),106)) like('%" + pagedListParams.searchValue + "%') or lower(Convert(nvarchar,Convert(date,EDate,101),106)) like('%" + pagedListParams.searchValue + "%')) and p.isactive=1 ";
                string sOrderBy = "";
                string sPaging = "";
                string sQuery = "select count(1) from Projects P join Clients c on p.ClientID = c.clientId Join Users u on p.pmanager=u.UserID" + sWhere + sOrderBy + sPaging;

                var data = await _unitOfWork.Project.GetTableData<int?>(sQuery);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetProjects)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> GetProjects(ProjectDTO inputData)
    {
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"@userid", 0, DbType.Int32);
            parms.Add(@"@stattype", 0, DbType.Int32);
            parms.Add(@"@flagtype", 0, DbType.Int32);
            try
            {
                //var result = await _unitOfWork.Project.GetStoredProcedure("GetDashboardbyManager", parms);

                var result = await _unitOfWork.Project.GetQueryAll<ProjectTableViewModel>(@"GetDashboardbyManager @userid=" + inputData.Pmanager + ",@stattype=" + inputData.Status + ",@flagtype=" + inputData.Flag + "");

                return Ok(result);
            }
            catch (Exception ex) { return null; }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetProjects)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetProjectsCount(int UserId)
    {
        try
        {
            var result = await _unitOfWork.Project.GetQueryAll<DashboardProjectCountSummaryViewModel>(@"usp_GetDashboardDetails @manager=" + UserId + "");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetProjects)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> ChangeProjectStatus(int Status, int ProjectId)
    {
        try
        {
            var parms = new DynamicParameters();
            parms.Add(@"@userid", 0, DbType.Int32);
            parms.Add(@"@stattype", 0, DbType.Int32);
            parms.Add(@"@flagtype", 0, DbType.Int32);
            try
            {
                //var result = await _unitOfWork.Project.GetStoredProcedure("GetDashboardbyManager", parms);
                var result = await _unitOfWork.Project.GetQueryAll<DashboardViewModel>(@"update projects set status=" + Status + " where projectid=" + ProjectId);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex) { return StatusCode(StatusCodes.Status500InternalServerError, ex); }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetProjects)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetDataProjectDetailsPage(ProjectDTO inputData)
    {
        try
        {
            try
            {
                //var results = await DbConnection.QueryMultipleAsync("");

                //var result = await _unitOfWork.Project.GetQueryAll<ProjectDetailPageViewModel>(@"usp_GetProjectView @ID=" + inputData.ProjectId + "");
                var result = await _unitOfWork.Project.GetProjectDetailPageMultipleAsync(@"usp_GetProjectView @ID=" + inputData.ProjectId + "");

                //result.Read<ProjectDetail>().ToList();

                //var result = await _unitOfWork.Project.GetQueryAll(@"usp_GetProjectView @ID=" + inputData.ProjectId + "");
                return Ok(result);
            }
            catch (Exception ex) { return null; }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetProjects)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetFractionComplete<T>(ProjectDTO inputData, int opt)
    {
        try
        {
            try
            {
                var result = await _unitOfWork.Project.GetQueryAll<T>(@"getCompleteFraction @projectid=" + inputData.ProjectId + ",@opt=" + opt + "");
                return Ok(result.FirstOrDefault());
            }
            catch (Exception ex) { return null; }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetProjects)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetProjectById(ProjectDTO inputData)
    {
        try
        {
            try
            {
                inputData = _mapper.Map<ProjectDTO>(await _unitOfWork.Project.FindByIdAsync(inputData.ProjectId));
                return Ok(inputData);
            }
            catch (Exception ex) { return null; }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetProjectById)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> GetProjectById(int projectId)
    {
        try
        {
            try
            {
                ProjectDTO inputData = new ProjectDTO();
                inputData = _mapper.Map<ProjectDTO>(await _unitOfWork.Project.FindByIdAsync(projectId));
                return Ok(inputData);
            }
            catch (Exception ex) { return null; }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetProjectById)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> UpdateProject(ProjectDTO inputData)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputData.ProjectId > 0)
                {
                    Project project = new Project();
                    project = await _unitOfWork.Project.FindByIdAsync(inputData.ProjectId);
                    project.Pname = inputData.Pname;
                    project.Descriptions = inputData.Descriptions;
                    project.ClientId = inputData.ClientId;
                    project.Pmanager = inputData.Pmanager;
                    project.Loi = inputData.Loi;
                    project.Irate = inputData.Irate;
                    project.Cpi = inputData.Cpi;
                    project.SampleSize = inputData.SampleSize;
                    project.Quota = inputData.Quota;
                    project.Sdate = CommonHelper.ConvertStringDateTimeToString(inputData.Sdate, "yyyy-MM-dd", "MM-dd-yyyy");
                    project.Edate = CommonHelper.ConvertStringDateTimeToString(inputData.Edate, "yyyy-MM-dd", "MM-dd-yyyy"); ;
                    project.CountryId = inputData.CountryId;
                    project.Ltype = inputData.Ltype;
                    project.Status = inputData.Status;
                    project.Notes = inputData.Notes;
                    await _unitOfWork.Project.UpdateAsync(project);
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Record not found");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetProjectById)}");
            throw;
        }
    }


    [HttpPost]
    public async Task<IActionResult> AddProject(ProjectDTO inputData)
    {
        try
        {
            if (ModelState.IsValid && inputData != null)
            {
                string query = "SELECT max(p.PID) FROM dbo.Projects p";
                var res = await _unitOfWork.Project.GetEntityData<string>(query);
                inputData.Pid = await GenerateProjectId(res);
                inputData.Sdate = CommonHelper.ConvertStringToDateTime(inputData.Sdate, "yyyy-MM-dd")?.ToString("MM-dd-yyyy");
                inputData.Edate = CommonHelper.ConvertStringToDateTime(inputData.Edate, "yyyy-MM-dd")?.ToString("MM-dd-yyyy");
                inputData.ProjectId = await _unitOfWork.Project.AddAsync(_mapper.Map<Project>(inputData));
                if (inputData.ProjectId > 0)
                {
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                {
                    return BadRequest("Error in adding project");
                }

            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetProjectById)}");
            throw;
        }
    }

    private async Task<string> GenerateProjectId(string startValue)
    {
        if (startValue == "" || int.Parse(startValue.Substring(3)) < 200000)
        {
            startValue = "NXT200000";
        }
        char letter1 = startValue[0];
        char letter2 = startValue[1];
        char letter3 = startValue[2];
        int len = startValue.Length - 3;
        int number = int.Parse(startValue.Substring(3));
        number++;
        if (number >= Math.Pow(10, len)) number = 1; // start again at 1

        string PID = String.Format("{0}{1}{2}{3:D" + len.ToString() + "}", letter1, letter2, letter3, number);

        while (await _unitOfWork.Project.GetEntityCount($"Select count(1) from Projects where PID='{PID}'") > 0)
        {
            number++;
            if (number >= Math.Pow(10, len)) number = 1; // start again at 1
            PID = String.Format("{0}{1}{2}{3:D" + len.ToString() + "}", letter1, letter2, letter3, number);
        }
        return PID;
    }
    public async Task<IActionResult> GetProjectDetailsForDownload(int projectId, int opt)
    {
        string sQuery = "exec usp_mgrdataDownload2 @projectid=@projectid,@opt=@opt";
        var sParam = new { @projectid = projectId, @opt = opt };
        var res = await _unitOfWork.Project.GetTableData<DownloadData>(sQuery, sParam);
        return Ok(res);
    }
    public async Task<IActionResult> GetProjectDetailsForDownloadResponse(int projectId, int opt)
    {
        string sQuery = "exec fetchPreScreeningResponse_1 @projectid=@projectid,@opt=@opt,@sid=@sid";
        var sParam = new { @projectid = projectId, @opt = opt, @sid = "" };
        var res = await _unitOfWork.Project.GetTableData<ResponseData>(sQuery, sParam);
        return Ok(res);
    }
    public async Task<IActionResult> GetProjectDetailsDetailedDateWiseReport(int projectId, DateTime startReportDate, DateTime endReportDate)
    {
        string sQuery = @"Select 
                            P.PID
                            ,SP.UID
                            ,S.Name SupplierName
                            ,S.SupplierCode
                            ,SP.Status
                            from SupplierProjects sp
                            Join ProjectMapping pm on sp.SID=pm.SID
                            Join Projects p on pm.ProjectID=p.ProjectId
                            Join Suppliers s on pm.SupplierId=s.Id
                            where
                            p.ProjectID=@ProjectID and
                            StartDate BETWEEN @startReportDate AND @endReportDate";
        var sParam = new { @ProjectID = projectId, @startReportDate = startReportDate, @endReportDate = endReportDate };
        var res = await _unitOfWork.Project.GetTableData<DownloadData>(sQuery, sParam);
        return Ok(res);
    }


}
