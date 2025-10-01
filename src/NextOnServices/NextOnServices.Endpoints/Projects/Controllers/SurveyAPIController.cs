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

}
