using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.ViewModels.Dashboard;
using NextOnServices.Infrastructure.ViewModels.Supplier;
using NextOnServices.Infrastructure.ViewModels.SurveyRedirects;
using NextOnServices.Services.DBContext;
using System.Data;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace NextOnServices.Endpoints.Projects;

[Route("api/[controller]")]
[ApiController]
public class SurveyAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SurveyAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly DapperDBSetting _dapperDBSetting;
    public SurveyAPIController(IUnitOfWork unitOfWork, ILogger<SurveyAPIController> logger, IMapper mapper, DapperDBSetting dapperDBSetting)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dapperDBSetting = dapperDBSetting;
    }
    [HttpPost]
    public async Task<IActionResult> Updatestatussupplierprojects(string ID, string action, string sid)
    {
        try
        {
            if (String.IsNullOrEmpty(ID))
            {
                ID = "0";
            }
            SupplierProjectStatusDTO inputData = new SupplierProjectStatusDTO
            {
                Id = Convert.ToInt32(ID == "" ? 0 : ID),
                Action = action,
                SId = sid
            };

            var parameters = new DynamicParameters();
            parameters.Add("@id", inputData.Id, DbType.Int32);
            parameters.Add("@action", inputData.Action, DbType.String, size: 200);
            parameters.Add("@ssid", inputData.SId, DbType.String, size: 200);
            parameters.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: 200);

            var result = await _unitOfWork.Project.GetOutputFromStoredProcedure<string>(
                "mgrOverquota",
                parameters,
                "output"
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in {nameof(Updatestatussupplierprojects)}");
            return StatusCode(500, "There is some error. Please contact administrator.");
        }
    }

    public async Task<string> GetIPFromDatabase(string IP, string Country_Code, string Country, string City, int opt)
    {
        try
        {
            string squery = "IPManager";
            var param = new { @IP = IP, @Country_Code = Country_Code, @Country = Country, @City = City, @opt = opt };
            var res = await _unitOfWork.GenOperations.GetEntityDataSP<string>(squery, param);
            return res;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in {nameof(Updatestatussupplierprojects)}");
            return "0";
            //return StatusCode(500, "There is some error. Please contact administrator.");
        }
    }
    public async Task<List<dynamic>> IPSecurityControlmgr(int id, int opt, string count)
    {
        DataSet DsRech = new DataSet();
        string status = "";
        try
        {
            string sQuery = "IPSecurityControlmgr";
            var sprams = new { @projectid = id, @Opt = opt, @count = count };
            var data = await _unitOfWork.GenOperations.GetTableDataSP<dynamic>(sQuery, sprams);
            return data;
        }
        catch (SystemException ex)
        {
            return null;
        }
    }
    [HttpPost]
    public async Task<List<dynamic>> AuthenticateIP(string SID, int OPT)
    {
        try
        {
            string sQuery = "AutheticateIP";
            var sparam = new { @sid = SID, @opt = OPT };
            var result = await _unitOfWork.GenOperations.GetTableDataSP<dynamic>(sQuery, sparam);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AuthenticateIP)}");
            throw;
        }
    }
    public async Task<List<dynamic>> TokenMgr(string nid, string sid, int opt)
    {
        try
        {
            string sQuery = "Tokensmgr";
            var sparam = new { @opt = opt, @sid = sid, @nid = nid };
            var result = await _unitOfWork.GenOperations.GetTableDataSP<dynamic>(sQuery, sparam);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AuthenticateIP)}");
            throw;
        }
    }
    public async Task<List<dynamic>> GenericDataFetcher(string sQuery, object parameters = null)
    {
        try
        {
            var result = await _unitOfWork.GenOperations.GetTableDataSP<dynamic>(sQuery, parameters);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AuthenticateIP)}");
            throw;
        }
    }
    public async Task<string> SaveSupplierProject(string ClientIP, string ClientBrowser, string Status, string SID, string Code, string UID, string PrevUID, string Device, int opt, string enc)
    {
        try
        {

            var parameters = new DynamicParameters();
            parameters.Add("@PMID", Code, DbType.String, size: 50);
            parameters.Add("@ClientIP", ClientIP, DbType.String, size: 50);
            parameters.Add("@ClientBrowser", ClientBrowser, DbType.String, size: 50);
            parameters.Add("@Status", Status, DbType.String, size: 50);
            parameters.Add("@UID", UID, DbType.String, size: 50);
            parameters.Add("@SID", SID, DbType.String, size: 50);
            parameters.Add("@PrevUID", PrevUID, DbType.String, size: 50);
            parameters.Add("@clientdevice", Device, DbType.String, size: 50);
            parameters.Add("@enc", enc, DbType.String, size: 250);
            parameters.Add("@opt", opt, DbType.Int32);


            parameters.Add("@Result", dbType: DbType.String, direction: ParameterDirection.Output, size: 200);

            var result = await _unitOfWork.Project.GetOutputFromStoredProcedure<string>(
                "Usp_GetSupplierLink",
                parameters,
                "Result"
            );
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in {nameof(Updatestatussupplierprojects)}");
            return "";
            //return StatusCode(500, "There is some error. Please contact administrator.");
        }
    }

    public async Task<string> UpdateProjectDetails(string ClientIP, string ClientBrowser, string Status, string SID, string Code, int IsUsed, int Type)
    {
        try
        {

            var parameters = new DynamicParameters();
            parameters.Add("@IP", ClientIP, DbType.String, size: 50);
            parameters.Add("@Browser", ClientBrowser, DbType.String, size: 50);
            parameters.Add("@Status", Status, DbType.String, size: 50);
            parameters.Add("@IsUsed", IsUsed, DbType.Int32);
            parameters.Add("@Code", Code, DbType.String, size: 50);
            parameters.Add("@SID", SID, DbType.String, size: 50);
            parameters.Add("@Opt", Type, DbType.Int32);

            parameters.Add("@Result", dbType: DbType.String, direction: ParameterDirection.Output, size: 200);

            var result = await _unitOfWork.Project.GetOutputFromStoredProcedure<string>(
                "Usp_UpdateBrowserDetails",
                parameters,
                "Result"
            );
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in {nameof(Updatestatussupplierprojects)}");
            return "";
            //return StatusCode(500, "There is some error. Please contact administrator.");
        }
    }

    public async Task<int> UpdateRequestStatus(string ID)
    {
        try
        {
            bool res = await _unitOfWork.GenOperations.ExecuteQueryAsync("update [dbo].[SupplierProjects] set IsSent=1 where [UID]=@Id", new { @Id = ID });
            if (res) { return 1; }
            else { return 0; }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AuthenticateIP)}");
            throw;
        }
    }
    public async Task<int> CheckForPixelTracking(string ID, int opt, int PID)
    {
        try
        {
            var res = await _unitOfWork.GenOperations.GetTableDataSP<dynamic>("CheckPixelTracking", new { @Id = ID, @opt = opt, @pid = PID });
            if (res != null && res.Any())
            {
                var row = (IDictionary<string, object>)res.First();
                var val = row.Values.ElementAtOrDefault(0)?.ToString();

                return int.TryParse(val, out int result) ? result : -1;
            }
            return -1;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AuthenticateIP)}");
            throw;
        }
    }

    public async Task<ProjectMappingDTO> GetProjectMappingRecordBYSID(string SID)
    {
        try
        {
            var res = await _unitOfWork.GenOperations.GetEntityData<ProjectMappingDTO>("Select * from ProjectMapping where sid=@SID", new { @SID = SID });
            return res;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AuthenticateIP)}");
            throw;
        }
    }
    public async Task<IActionResult> UpdateSupplierENCByUID(string UID, string hashcode)
    {
        try
        {

            var res = await _unitOfWork.GenOperations.GetEntityData<SupplierProjects>("Select * from SupplierProjects where UID=@UID", new { UID });
            if (res != null)
            {
                res.ENC = hashcode;
                await _unitOfWork.SupplierProjects.UpdateAsync(res);
                return Ok("");
            }
            return BadRequest("");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AuthenticateIP)}");
            throw;
        }
    }

    public async Task<bool> CheckUIDForRecontact(string UID, string SID)
    {
        try
        {
            string sQuery = "SELECT 1 FROM dbo.RecontactProjects rp INNER JOIN dbo.RecontactProjectsIds rp2  ON rp.id=rp2.recontactprojectsid INNER JOIN dbo.ProjectMapping pm ON pm.id = rp.ProMapId INNER JOIN dbo.SupplierProjects sp ON pm.sid = sp.sid WHERE sp.uid = @UID AND pm.SID = @SID AND rp2.uid = @UID";
            var sparam = new { @UID = UID, @SID = SID };
            var res = await _unitOfWork.GenOperations.IsExists(sQuery, sparam);
            return res;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AuthenticateIP)}");
            throw;
        }
    }

    /// <summary>
    /// ID Search: get respondents by Unique Id (type=1) or Supplier Id (type=2). Uses USP_mgrRespondents.
    /// </summary>
    public async Task<List<RespondentSearchResultDTO>> GetRespondents(string id, string type)
    {
        try
        {
            var rows = await _unitOfWork.GenOperations.GetTableDataSP<dynamic>("USP_mgrRespondents", new { @id = id, @type = type });
            if (rows == null || !rows.Any())
                return new List<RespondentSearchResultDTO>();

            var result = new List<RespondentSearchResultDTO>();
            foreach (var row in rows)
            {
                var d = row as IDictionary<string, object>;
                if (d == null) continue;
                result.Add(new RespondentSearchResultDTO
                {
                    SupplierName = GetString(d, "Supplier Name"),
                    SID = GetString(d, "SID"),
                    UID = GetString(d, "UID"),
                    Country = GetString(d, "Country"),
                    SupplierId = GetString(d, "Supplier ID"),
                    Status = GetString(d, "Status"),
                    Sdate = GetString(d, "StartDate"),
                    Edate = GetString(d, "EndDate"),
                    Duration = GetString(d, "Duration"),
                    ClientBrowser = GetString(d, "ClientBrowser"),
                    ClientIP = GetString(d, "ClientIP"),
                    Device = GetString(d, "Device")
                });
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(GetRespondents));
            throw;
        }
    }

    /// <summary>
    /// ID Search: update respondent status in SupplierProjects by UID.
    /// </summary>
    public async Task<int> UpdateRespondentStatus(string id, string status)
    {
        try
        {
            var updated = await _unitOfWork.GenOperations.ExecuteQueryAsync(
                "UPDATE SupplierProjects SET Status = @Status WHERE UID = @Id",
                new { @Status = status, @Id = id });
            return updated ? 1 : 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(UpdateRespondentStatus));
            throw;
        }
    }

    private static string GetString(IDictionary<string, object> d, string key)
    {
        if (d == null || !d.ContainsKey(key)) return "";
        var v = d[key];
        return v == null || v == DBNull.Value ? "" : Convert.ToString(v) ?? "";
    }

    private static int GetInt(IDictionary<string, object> d, string key)
    {
        if (d == null || !d.ContainsKey(key) || d[key] == null || d[key] == DBNull.Value) return 0;
        return int.TryParse(Convert.ToString(d[key]), out var value) ? value : 0;
    }

    private static decimal GetDecimal(IDictionary<string, object> d, string key)
    {
        if (d == null || !d.ContainsKey(key) || d[key] == null || d[key] == DBNull.Value) return 0m;
        return decimal.TryParse(Convert.ToString(d[key]), out var value) ? value : 0m;
    }

    /// <summary>
    /// Reconciliation: get projects list for dropdown. Returns Id and PName (Pid).
    /// </summary>
    public async Task<List<ReconciliationProjectDTO>> GetProjectsForReconciliation()
    {
        try
        {
            var query = "SELECT ProjectId AS Id, Pid, Pname AS PName, (ISNULL(RTRIM(Pid),'') + '_' + ISNULL(RTRIM(Pname),'')) AS DisplayText FROM Projects WHERE IsActive = 1 ORDER BY Pid";
            var list = await _unitOfWork.Project.GetTableData<ReconciliationProjectDTO>(query);
            return list ?? new List<ReconciliationProjectDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(GetProjectsForReconciliation));
            throw;
        }
    }

    /// <summary>
    /// Reconciliation: update status via USP_updatestatus. Returns list of respondent rows or error rows.
    /// </summary>
    public async Task<List<RespondentSearchResultDTO>> UpdateMultipleStatusReconciliation(string id, string status, int type, int pid)
    {
        try
        {
            var rows = await _unitOfWork.GenOperations.GetTableDataSP<dynamic>("USP_updatestatus", new { @id = id, @type = type, @status = status, @pid = pid });
            if (rows == null || !rows.Any())
                return new List<RespondentSearchResultDTO>();

            var result = new List<RespondentSearchResultDTO>();
            foreach (var row in rows)
            {
                var d = row as IDictionary<string, object>;
                if (d == null) continue;
                if (d.ContainsKey("Error") && d["Error"] != null && d["Error"] != DBNull.Value)
                {
                    result.Add(new RespondentSearchResultDTO { Error = GetString(d, "Error") });
                    continue;
                }
                result.Add(new RespondentSearchResultDTO
                {
                    SupplierName = GetString(d, "Supplier Name"),
                    SID = GetString(d, "SID"),
                    UID = GetString(d, "UID"),
                    Country = GetString(d, "Country"),
                    SupplierId = GetString(d, "Supplier ID"),
                    Status = GetString(d, "Status"),
                    Sdate = GetString(d, "StartDate"),
                    Edate = GetString(d, "EndDate"),
                    Duration = GetString(d, "Duration"),
                    ClientBrowser = GetString(d, "ClientBrowser"),
                    ClientIP = GetString(d, "ClientIP"),
                    Device = GetString(d, "Device")
                });
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(UpdateMultipleStatusReconciliation));
            throw;
        }
    }

    /// <summary>
    /// Recontact Projects: list summary rows using Recontactmgr.
    /// </summary>
    public async Task<List<RecontactProjectDTO>> GetRecontactProjectsList()
    {
        try
        {
            var rows = await _unitOfWork.GenOperations.GetTableDataSP<dynamic>("Recontactmgr", new { @opt = 2 });
            if (rows == null || !rows.Any())
                return new List<RecontactProjectDTO>();

            var result = new List<RecontactProjectDTO>();
            foreach (var row in rows)
            {
                //{{DapperRow, id = '1', projectid = '1', PID = 'NXT000073', projectname = 'test', cpi = '10', status = '5', total = '0', complete = '0', terminate = '0', overquota = '0', securityterm = '0', fauderror = '0', incomplete = '0'}}
                var d = row as IDictionary<string, object>;
                if (d == null) continue;
                result.Add(new RecontactProjectDTO
                {
                    ID = GetInt(d, "projectid"),
                    PID = GetString(d, "PID"),
                    RecontactName = GetString(d, "projectname"),
                    CPI = GetDecimal(d, "cpi"),
                    Total = GetInt(d, "total"),
                    Complete = GetInt(d, "complete"),
                    Terminate = GetInt(d, "terminate"),
                    Overquota = GetInt(d, "overquota"),
                    S_term = GetInt(d, "securityterm"),
                    F_error = GetInt(d, "fauderror"),
                    Incomplete = GetInt(d, "incomplete"),
                    Statusint = GetInt(d, "status")
                });
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(GetRecontactProjectsList));
            throw;
        }
    }

    /// <summary>
    /// Recontact Projects: update status in RecontactProjects table.
    /// </summary>
    public async Task<int> UpdateRecontactProjectStatus(int id, int status)
    {
        try
        {
            var updated = await _unitOfWork.GenOperations.ExecuteQueryAsync(
                "UPDATE dbo.RecontactProjects SET Status = @Status WHERE Id = @Id",
                new { @Status = status, @Id = id });
            return updated ? 1 : 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(UpdateRecontactProjectStatus));
            throw;
        }
    }

    /// <summary>
    /// Recontact Page Details: get country, supplier, redirects and summary using RecontactPageDetails SP.
    /// </summary>
    public async Task<RecontactDetailsDTO> GetRecontactDetails(int id, int opt)
    {
        try
        {
            using var connection = new SqlConnection(_dapperDBSetting.ConnectionString);
            await connection.OpenAsync();

            using var multi = await connection.QueryMultipleAsync(
                "RecontactPageDetails",
                new { id, opt },
                commandType: CommandType.StoredProcedure);

            var countries = multi.Read<RecontactCountryDTO>().ToList();
            var suppliers = multi.Read<RecontactSupplierDTO>().ToList();
            var redirects = multi.Read<RecontactRedirectDTO>().ToList();
            var summary = multi.Read<RecontactSummaryDTO>().ToList();

            return new RecontactDetailsDTO
            {
                Countries = countries,
                Suppliers = suppliers,
                Redirects = redirects,
                Summary = summary
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(GetRecontactDetails));
            throw;
        }
    }

    /// <summary>
    /// Recontact Page Details: update recontact project settings via updateRecontactProjects SP.
    /// </summary>
    public async Task<string> UpdateRecontactProjects(RecontactUpdateDTO formdata, int opt)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@id", formdata.ID, DbType.Int32);
            parameters.Add("@pdescription", formdata.RecontactDescription ?? string.Empty, DbType.String, size: 100);
            parameters.Add("@loi", formdata.LOI, DbType.Double);
            parameters.Add("@cpi", formdata.CPI, DbType.Double);
            parameters.Add("@ir", formdata.IR, DbType.Double);
            parameters.Add("@rcq", formdata.RCQ, DbType.Double);
            parameters.Add("@notes", formdata.Notes ?? string.Empty, DbType.String, size: 500);
            parameters.Add("@opt", opt, DbType.Int32);
            parameters.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: 25);

            var result = await _unitOfWork.Project.GetOutputFromStoredProcedure<string>(
                "updateRecontactProjects",
                parameters,
                "output");

            return result ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(UpdateRecontactProjects));
            throw;
        }
    }

    // ---------- Create Recontact (RecontactCreate.aspx) ----------

    /// <summary>Projects dropdown for Create Recontact (UspGetAllUsers Type P).</summary>
    public async Task<List<RecontactCreateProjectDTO>> GetProjectsForRecontactCreate()
    {
        try
        {
            var rows = await _unitOfWork.GenOperations.GetTableDataSP<RecontactCreateProjectDTO>(
                "UspGetAllUsers",
                new { @ID = 0, @Type = "P" });
            return rows ?? new List<RecontactCreateProjectDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(GetProjectsForRecontactCreate));
            throw;
        }
    }

    /// <summary>Recontact list for Create Recontact page (FetchRecontact).</summary>
    public async Task<List<RecontactCreateListItemDTO>> LoadRecontactProjectsForCreate(int projectId, int countryId, int supplierId, int opt)
    {
        try
        {
            var rows = await _unitOfWork.GenOperations.GetTableDataSP<RecontactCreateListItemDTO>(
                "FetchRecontact",
                new { @projectid = projectId, @countryid = countryId, @supplierid = supplierId, @opt = opt });
            return rows ?? new List<RecontactCreateListItemDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(LoadRecontactProjectsForCreate));
            throw;
        }
    }

    /// <summary>Next PID for new recontact (GetMaxPID).</summary>
    public async Task<string> GetNextRecontactPID()
    {
        try
        {
            var rows = await _unitOfWork.GenOperations.GetTableDataSP<NextPidResult>("GetMaxPID", null);
            var first = rows?.FirstOrDefault();
            if (first == null) return string.Empty;
            return first.PID ?? first.MaxPID ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(GetNextRecontactPID));
            throw;
        }
    }

    /// <summary>Insert recontact header (Recontactmgr opt=0). Returns first row with status and rcid.</summary>
    public async Task<InsertRecontactResultRow?> InsertRecontactCreate(string recontactname, string CPI, string Notes, string recontactDescription, int Status, double LOI, double IR, double RCQ, string SID, string MURL, string PID, int opt)
    {
        try
        {
            var rows = await _unitOfWork.GenOperations.GetTableDataSP<InsertRecontactResultRow>(
                "Recontactmgr",
                new
                {
                    @recontactname = recontactname,
                    @CPI = float.Parse(CPI, System.Globalization.CultureInfo.InvariantCulture),
                    @Notes = Notes,
                    @MURL = MURL,
                    @PID = PID,
                    @recontactdescription = recontactDescription,
                    @status = Status,
                    @loi = LOI,
                    @IR = IR,
                    @rcq = RCQ,
                    @opt = opt,
                    @SID = SID
                });
            return rows?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(InsertRecontactCreate));
            throw;
        }
    }

    /// <summary>Validate UID for recontact (validateUID). Returns first row.</summary>
    public async Task<ValidateUIDRecontactResultRow?> ValidateUIDForRecontact(string uid, int opt, string SID)
    {
        try
        {
            var rows = await _unitOfWork.GenOperations.GetTableDataSP<ValidateUIDRecontactResultRow>(
                "validateUID",
                new { @uid = uid, @opt = opt, @SID = SID });
            return rows?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(ValidateUIDForRecontact));
            throw;
        }
    }

    /// <summary>Update recontact respondent row (RecontactRespondanttbl opt=2).</summary>
    public async Task<List<RecontactRespondantResultRow>> UpdateRecontactRespondantstblCreate(string UID, string RespondantLink, string Var1, string Var2, string Var3, string Var4, string Var5, string PMID, string MURL, string id, string SID, int projectmappingid, int projectid, int countryid, int supplierid, int opt)
    {
        try
        {
            var rows = await _unitOfWork.GenOperations.GetTableDataSP<RecontactRespondantResultRow>(
                "RecontactRespondanttbl",
                new
                {
                    @opt = opt,
                    @UID = UID,
                    @RedirectLink = RespondantLink,
                    @Var1 = Var1,
                    @Var2 = Var2,
                    @Var3 = Var3,
                    @Var4 = Var4,
                    @Var5 = Var5,
                    @PMID = PMID,
                    @MURL = MURL,
                    @id = id,
                    @SID = SID,
                    @projectmappingid = projectmappingid,
                    @projectid = projectid,
                    @countryid = countryid,
                    @supplierid = supplierid
                });
            return rows ?? new List<RecontactRespondantResultRow>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(UpdateRecontactRespondantstblCreate));
            throw;
        }
    }

    /// <summary>Insert variable for recontact (mgrVariables opt=0).</summary>
    public async Task InsertRecontactVariable(string variableName, int rcid, int opt)
    {
        try
        {
            await _unitOfWork.GenOperations.GetTableDataSP<dynamic>(
                "mgrVariables",
                new { @variablename = variableName, @rcid = rcid, @opt = opt });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(InsertRecontactVariable));
            throw;
        }
    }

    /// <summary>Delete one UID from recontact (DELETERecontactprojects opt=1). Returns status count from first row.</summary>
    public async Task<int> DeleteRecontactUid(string uid, int id, int opt)
    {
        try
        {
            var rows = await _unitOfWork.GenOperations.GetTableDataSP<DeleteRecontactResultRow>(
                "DELETERecontactprojects",
                new { @id = id, @uid = uid, @opt = opt });
            var first = rows?.FirstOrDefault();
            return first?.status ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(DeleteRecontactUid));
            throw;
        }
    }

    /// <summary>Add more respondents to recontact (UpdateRecontctRespondants opt=1).</summary>
    public async Task<int> UpdateRecontactProjectsAddMore(string uid, string clientURL, string maskingURL, int rpid, int opt)
    {
        try
        {
            var rows = await _unitOfWork.GenOperations.GetTableDataSP<AddMoreRecontactResultRow>(
                "UpdateRecontctRespondants",
                new { @opt = opt, @uid = uid, @clientURL = clientURL, @maskingURL = maskingURL, @rpid = rpid });
            var first = rows?.FirstOrDefault();
            return first?.status ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(UpdateRecontactProjectsAddMore));
            throw;
        }
    }

    /// <summary>Download recontact data for View Data (GetRecontactExcelData). Old page calls with opt=1, pro=0, cid=0, supp=recontactProjectId.</summary>
    public async Task<List<RecontactDownloadDTO>> GetRecontactExcelDataForCreate(int recontactProjectId)
    {
        try
        {
            var rows = await _unitOfWork.GenOperations.GetTableDataSP<RecontactDownloadDTO>(
                "GetRecontactExcelData",
                new { @opt = 1, @pid = 0, @cid = 0, @sid = recontactProjectId });
            return rows ?? new List<RecontactDownloadDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(GetRecontactExcelDataForCreate));
            throw;
        }
    }

}
