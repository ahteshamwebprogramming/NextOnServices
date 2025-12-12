using AutoMapper;
using Azure.Core;
using Dapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.Account;
using NextOnServices.Infrastructure.Models.Client;
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
using System.Linq;
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
                string sQuery = "select ProjectId,PID,PName,c.Company,u.Username,LOI,p.Status,Convert(nvarchar,Convert(date,SDate,101),106)sdate,Convert(nvarchar,Convert(date,EDate,101),106)edate from Projects P join Clients c on p.ClientID = c.clientId Join Users u on p.pmanager=u.UserID" + sWhere + sOrderBy + sPaging;
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
    public async Task<IActionResult> GetClientProjects(int clientId)
    {
        try
        {
            string query = @"SELECT p.ProjectId,
                                     p.PName AS ProjectName,
                                     cm.Country,
                                     p.Quota,
                                     p.Loi,
                                     p.Cpi,
                                     p.Irate,
                                     p.Status AS StatusId,
                                     sm.Pstatus AS StatusLabel
                              FROM Projects p
                              LEFT JOIN CountryMaster cm ON cm.CountryId = p.CountryId
                              LEFT JOIN StatusMaster sm ON sm.Pvalue = p.Status
                              WHERE p.ClientId = @ClientId
                              ORDER BY p.PName";

            var parameters = new { ClientId = clientId };
            var projects = await _unitOfWork.Project.GetTableData<ClientProjectSummary>(query, parameters);
            return Ok(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retrieving client projects {nameof(GetClientProjects)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> UpdateDeviceControl(ProjectDTO inputDTO)
    {
        try
        {
            try
            {

                string sQuery = "Select * from Projects where ProjectId=@Id";
                var sParam = new { @Id = inputDTO.ProjectId };
                var projects = await _unitOfWork.Project.GetEntityData<Project>(sQuery, sParam);
                if (projects != null) {
                    projects.BlockDevice = inputDTO.BlockDevice;
                    var updated = await _unitOfWork.Project.UpdateAsync(projects);
                    if (updated)
                    {
                        return Ok();
                    }
                }
                return BadRequest("Unable to update the device control");
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
                    
                    // Create notifications for all suppliers associated with this project
                    await NotifyAllSuppliersForProjectUpdateAsync(inputData.ProjectId, "Project details have been updated (Name, CPI, Quota, Dates, Status, etc.).");
                    
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
    public async Task<IActionResult> UpdateMappings(SaveMapIPRequest inputDTO)
    {
        try
        {
            var projecturlid = inputDTO.Id;
            var selectedCountries = inputDTO.Countries;
            var mode = inputDTO.Mode ?? "include"; // Default to include if not specified

            if (selectedCountries == null || selectedCountries.Count == 0)
            {
                return BadRequest(new { success = false, message = "No countries selected." });
            }

            // NEW APPROACH: Store only selected countries, not all mapped countries
            // For Include mode: Store only the included countries
            // For Exclude mode: Store only the excluded countries (much faster!)
            var countriesToStore = selectedCountries; // Store only what user selected
            string mappingMode = mode.ToLower() == "exclude" ? "Exclude" : "Include";

            // Store the mode in stat column: 0 = include, 1 = exclude
            int statValue = mode.ToLower() == "exclude" ? 1 : 0;

            // Optimized: Store only selected countries (not all mapped ones)
            // Convert country IDs to integers
            var countryIdList = countriesToStore.Select(c => int.Parse(c)).ToList();
            
            if (countryIdList.Any())
            {
                // Use a single optimized SQL statement that does everything at once
                // This is the fastest approach - one query instead of multiple operations
                var countryIdsString = string.Join(",", countryIdList);
                
                // Use stat column only (works without MappingMode column)
                // This stores only selected countries, not all mapped ones - much faster!
                string optimizedBatchQuery = $@"
                    -- Step 1: Deactivate all existing mappings for this project
                    UPDATE tblIPMapping 
                    SET isactive = 0 
                    WHERE prourlid = {projecturlid} AND isactive = 1;
                    
                    -- Step 2: Insert new mappings that don't exist (batch insert) - only selected countries
                    INSERT INTO tblIPMapping (prourlid, countryid, stat, isactive)
                    SELECT {projecturlid}, CountryId, {statValue}, 1
                    FROM CountryMaster
                    WHERE CountryId IN ({countryIdsString})
                    AND NOT EXISTS (
                        SELECT 1 FROM tblIPMapping 
                        WHERE prourlid = {projecturlid} AND countryid = CountryMaster.CountryId
                    );
                    
                    -- Step 3: Reactivate existing mappings
                    UPDATE tblIPMapping 
                    SET isactive = 1, stat = {statValue}
                    WHERE prourlid = {projecturlid} 
                    AND countryid IN ({countryIdsString})
                    AND isactive = 0;";

                await _unitOfWork.Project.ExecuteQueryAsync(optimizedBatchQuery, null);
            }
            else
            {
                // If no countries to map, just deactivate all
                string deactivateQuery = @"
                    UPDATE tblIPMapping 
                    SET isactive = 0 
                    WHERE prourlid = @prourlid AND isactive = 1";
                await _unitOfWork.Project.ExecuteQueryAsync(deactivateQuery, new { prourlid = projecturlid });
            }

            // Get country names for selected countries (do this after the main operation for better UX)
            List<SelectedCountryInfo> selectedCountriesInfo = new List<SelectedCountryInfo>();
            if (selectedCountries != null && selectedCountries.Any())
            {
                var selectedCountryIds = selectedCountries.Select(c => int.Parse(c)).ToList();
                string placeholders = string.Join(",", selectedCountryIds);
                string getSelectedCountriesQuery = $@"
                    SELECT CountryId, Country 
                    FROM CountryMaster 
                    WHERE CountryId IN ({placeholders})";
                var selectedCountriesData = await _unitOfWork.Project.GetTableData<CountryMaster>(getSelectedCountriesQuery, null);
                
                if (selectedCountriesData != null)
                {
                    selectedCountriesInfo = selectedCountriesData.Select(country => new SelectedCountryInfo
                    {
                        CountryId = country.CountryId,
                        CountryName = country.Country ?? ""
                    }).ToList();
                }
            }

            // For exclude mode, also store the excluded countries in a special record
            // We'll use a negative countryid or special marker to store excluded countries
            // Actually, let's store them in a JSON-like format or use a different approach
            // For now, we'll calculate them when retrieving

            string modeText = mode.ToLower() == "exclude" ? "excluded" : "included";
            return Ok(new { 
                success = true, 
                message = $"Countries {modeText} successfully.", 
                mode = mode, 
                mappedCount = countryIdList.Count,
                selectedCountries = selectedCountriesInfo
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in UpdateMappings - ProjectUrlId: {inputDTO?.Id}, Mode: {inputDTO?.Mode}, Error: {ex.Message}");
            return StatusCode(500, new { success = false, message = "An error occurred while saving countries.", error = ex.Message });
        }
    


    }

    [HttpPost]
    public async Task<IActionResult> DeleteMappings(DeleteMapIPRequest inputDTO)
    {
        try
        {
            var projecturlid = inputDTO.ProjectURLID;
            var countryId = inputDTO.CountryID;

            // NEW APPROACH: Simply remove the country from stored list (works for both modes)
            // In both Include and Exclude modes, we just remove the selected country
            string sQuery = @"
                UPDATE tblIPMapping 
                SET isactive = 0 
                WHERE countryid = @countryid AND prourlid = @projecturlid AND isactive = 1";

            var sParam = new
            {
                projecturlid = projecturlid,
                countryid = countryId
            };

            await _unitOfWork.Project.ExecuteQueryAsync(sQuery, sParam);

            return Ok(new { success = true, message = "Country removed successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in DeleteMappings - ProjectUrlId: {inputDTO?.ProjectURLID}, CountryId: {inputDTO?.CountryID}, Error: {ex.Message}");
            return StatusCode(500, new { success = false, message = "An error occurred while removing country.", error = ex.Message });
        }
    }
    public async Task<IActionResult> GetMappedCountries(int id)
    {
        try
        {
            var projecturlid = id;

            // Get the mode from the first active mapping using stat column
            // stat: 0 = include, 1 = exclude
            string mode = "include";
            var modeParam = new { projecturlid };
            
            string getModeQuery = @"
                SELECT TOP 1 stat
                FROM tblIPMapping 
                WHERE prourlid = @projecturlid AND isactive = 1";
            var modeResult = await _unitOfWork.Project.GetTableData<dynamic>(getModeQuery, modeParam);
            if (modeResult != null && modeResult.Any())
            {
                var stat = modeResult.First();
                // Use stat column (0 = include, 1 = exclude)
                if (stat.stat != null && Convert.ToInt32(stat.stat) == 1)
                {
                    mode = "exclude";
                }
            }

            // NEW APPROACH: Get only the stored selected countries (much faster!)
            // The stored countries are the ones the user selected, not all mapped countries
            List<SelectedCountryInfo> selectedCountries = new List<SelectedCountryInfo>();
            
            // Get stored countries (these are the selected countries, not all mapped ones)
            string sQuery = @"
                SELECT DISTINCT tp.countryid, tc.country 
                FROM tblIPMapping tp 
                INNER JOIN CountryMaster tc ON tp.countryid = tc.countryid  
                WHERE tp.prourlid = @projecturlid AND tp.isactive = 1";
            var sParam = new { projecturlid };
            var storedCountries = await _unitOfWork.Project.GetTableData<dynamic>(sQuery, sParam);
            
            if (storedCountries != null)
            {
                foreach (var item in storedCountries)
                {
                    selectedCountries.Add(new SelectedCountryInfo
                    {
                        CountryId = Convert.ToInt32(item.countryid),
                        CountryName = item.country?.ToString() ?? ""
                    });
                }
            }

            var response = new IPMappingConfigResponse
            {
                Mode = mode,
                SelectedCountries = selectedCountries
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
           
            Console.WriteLine($"Error fetching mapped countries: {ex.Message}");

            
            return StatusCode(500, "An error occurred while fetching mapped countries.");
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

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, string action)
    {
       
        var parameters = new DynamicParameters();
        parameters.Add("@id", id);
        parameters.Add("@action", action);
        parameters.Add("@ssid", "YUQFALPJ");
        parameters.Add("@output", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

        var message = await _unitOfWork.Project.GetOutputFromStoredProcedure<string>("mgrOverquota",parameters,"@output");

        return Ok(new { message });
    }

    [HttpPost]
    public async Task<IActionResult> upDatePrescreening(int id, int action)
    {

        if (id <= 0)
        {
            return BadRequest(new { RetVal = -1, RetMessage = "Invalid request" });
        }

        // action is already int, so no conversion needed
        int prescreening = action;

        string sql = "UPDATE dbo.ProjectMapping SET Prescreening = @Prescreening WHERE Id = @Id";

        bool success = await _unitOfWork.Project.ExecuteQueryAsync(sql, new { Id = id, Prescreening = prescreening });

        if (success)
        {
            return Ok(new { RetVal = 1, RetMessage = "Updated" });
        }
        else
        {
            return Ok(new { RetVal = -1, RetMessage = "Failed" });
        }

    }

    public async Task<IActionResult> getProjectDetailsbyID(int ID)
    {
        string sql = @"
        select p.PID,pm.ID,pm.ProjectID, p.PName as projectname,pm.CountryID,tc.Country,pm.SUpplierID,s.Name as Supplier,pm.OLink,pm.CPI,pm.MLink,pm.Respondants,pm.Notes,pm.IsChecked,
            pm.SID,pm.Code,pm.Completes,pm.Terminate,pm.Quotafull,pm.Screened,pm.Overquota,pm.Incomplete,pm.Security,pm.Fraud,pm.SUCCESS,pm.[DEFAULT],pm.FAILURE,pm.[QUALITYTERMINATION],
            pm.[OVERQUOTA],pm.IsActive,pm.Block,
            (CASE WHEN pm.trackingtype is NULL THEN 0 ELSE pm.trackingtype end) AS trackingtype
            ,isnull(AddHashing,0)AddHashing
            ,isnull(ParameterName,'')ParameterName
            ,isnull(HashingType,'')HashingType
             from projectmapping pm left join projects p on pm.ProjectID=p.projectid left join CountryMaster tc on pm.CountryID=tc.CountryId 
            left join Suppliers s on pm.SUpplierID = s.ID where pm.ID=@id";

        var result = await _unitOfWork.Project.GetEntityData<ProjectDetailDTO>(sql, new { id = ID });

        if (result == null)
            return NotFound(new { RetVal = -1, RetMessage = "Project not found." });

        return Ok(result);
    }

    public async Task<IActionResult> UpdateCPIMapping(ProjectDTO model)
    {
        // Get project mapping to find supplier - Note: model.ProjectId is actually ProjectMapping.ID
        var projectMappingSql = "SELECT * FROM ProjectMapping WHERE ID = @Id";
        var projectMapping = await _unitOfWork.ProjectMapping.GetEntityData<ProjectMapping>(projectMappingSql, new { Id = model.ProjectId });
        
        string sql = @"UPDATE projectmapping 
                   SET Respondants = @Quota, CPI = @CPI, Notes = @Notes 
                   WHERE ID = @Id";

        bool success = await _unitOfWork.Project.ExecuteQueryAsync(sql, new
        {
            Id = model.ProjectId,
            CPI = model.Cpi,
            Quota = model.Quota,
            Notes = model.Notes
        });

        if (success && projectMapping != null)
        {
            // Create notification for project change
            if (projectMapping.SupplierId.HasValue && projectMapping.ProjectId.HasValue)
            {
                await CreateProjectChangeNotificationAsync(projectMapping.Id, projectMapping.ProjectId.Value, projectMapping.SupplierId.Value, "Project CPI, Quota, or Notes have been updated.");
            }
            
            return Ok(new { RetVal = 1, RetMessage = "CPI Mapping updated successfully." });
        }
        else
        {
            return Ok(new { RetVal = -1, RetMessage = "Failed to update CPI Mapping." });
        }
    }

    private async Task CreateProjectChangeNotificationAsync(int projectMappingId, int projectId, int supplierId, string changeDescription)
    {
        try
        {
            var message = $"[PROJECT_UPDATE]{changeDescription}";
            
            var notificationSql = @"
                INSERT INTO SupplierProjectMessages 
                (ProjectMappingId, ProjectId, SupplierId, Message, CreatedBy, CreatedByName, CreatedUtc, FromSupplier, IsRead)
                VALUES 
                (@ProjectMappingId, @ProjectId, @SupplierId, @Message, @CreatedBy, @CreatedByName, @CreatedUtc, @FromSupplier, @IsRead)";

            var senderName = "System";
            int? createdBy = null;
            
            if (HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = HttpContext.User.FindFirst("Id")?.Value;
                if (int.TryParse(userIdClaim, out var userId))
                {
                    createdBy = userId;
                }
                
                var userNameClaim = HttpContext.User.FindFirst("Name")?.Value ?? 
                                   HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                if (!string.IsNullOrEmpty(userNameClaim))
                {
                    senderName = userNameClaim;
                }
            }

            await _unitOfWork.ProjectMapping.ExecuteQueryAsync(notificationSql, new
            {
                ProjectMappingId = projectMappingId,
                ProjectId = projectId,
                SupplierId = supplierId,
                Message = message,
                CreatedBy = createdBy,
                CreatedByName = senderName,
                CreatedUtc = DateTime.UtcNow,
                FromSupplier = false,
                IsRead = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project change notification for ProjectMappingId {ProjectMappingId}", projectMappingId);
            // Don't throw - notification failure shouldn't break the update
        }
    }

    private async Task NotifyAllSuppliersForProjectUpdateAsync(int projectId, string changeDescription)
    {
        try
        {
            // Get all project mappings for this project
            var mappingsSql = "SELECT * FROM ProjectMapping WHERE ProjectID = @ProjectId AND SupplierID IS NOT NULL";
            var mappings = await _unitOfWork.ProjectMapping.GetTableData<ProjectMapping>(mappingsSql, new { ProjectId = projectId });

            if (mappings != null && mappings.Any())
            {
                foreach (var mapping in mappings)
                {
                    if (mapping.SupplierId.HasValue && mapping.Id > 0)
                    {
                        await CreateProjectChangeNotificationAsync(mapping.Id, projectId, mapping.SupplierId.Value, changeDescription);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying suppliers for project update - ProjectId {ProjectId}", projectId);
            // Don't throw - notification failure shouldn't break the update
        }
    }

    public async Task<IActionResult> UpdateRedirects(ProjectDetailDTO model)
    {
        string sql = @"UPDATE ProjectMapping 
               SET Completes = @Completes,
                   Terminate = @Terminate,
                   Overquota = @Overquota,
                   Security = @Security,
                   Fraud = @Fraud,
                   TrackingType = @TrackingType
               WHERE ID = @Id";

        bool rowsAffected = await _unitOfWork.Project.ExecuteQueryAsync(sql, new
        {
            Id = model.ID,
            Completes = model.Completes,
            Terminate = model.Terminate,
            Overquota = model.OVERQUOTA,
            Security = model.Security,
            Fraud = model.Fraud,
            TrackingType = model.trackingtype
        });

        if (rowsAffected)
        {
            return Ok(new { RetVal = 1, RetMessage = "Tracking data updated successfully." });
        }
        else
        {
            return Ok(new { RetVal = -1, RetMessage = "No data was updated." });
        }
    }
 }
