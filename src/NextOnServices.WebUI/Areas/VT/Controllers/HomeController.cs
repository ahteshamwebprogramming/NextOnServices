
using ClosedXML.Excel;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NextOnServices.Endpoints.Accounts;
using NextOnServices.Endpoints.Masters;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Infrastructure.Models.Account;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.ViewModels.Dashboard;
using NextOnServices.Infrastructure.ViewModels.ProjectDetails;
using NextOnServices.WebUI.Models;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace NextOnServices.WebUI.VT.Controllers;
[Area("VT")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ProjectsAPIController _projectsAPIController;
    private readonly AccountsController _accountAPIController;
    private readonly StatusMasterAPIController _statusMasterAPIController;
    private readonly CountryAPIController _countryAPIController;
    public HomeController(ILogger<HomeController> logger, ProjectsAPIController projectsAPIController, AccountsController accountAPIController, StatusMasterAPIController statusMasterAPIController, CountryAPIController countryAPIController)
    {
        _logger = logger;
        _projectsAPIController = projectsAPIController;
        _accountAPIController = accountAPIController;
        _statusMasterAPIController = statusMasterAPIController;
        _countryAPIController = countryAPIController;
    }

    public IActionResult Index()
    {
        return View();
    }
    public async Task<IActionResult> ProjectPage(int projectId)
    {
        //int projectId = 18537;

        int? UserId = HttpContext.Session.GetInt32("UserId");
        if (UserId != null)
        {
            if (projectId == null)
            {
                return RedirectToAction("/Account/Login");
            }
            ProjectDetailPageViewModel outputData = new ProjectDetailPageViewModel();

            ProjectDTO inputData = new ProjectDTO();

            var resProjects =  await _projectsAPIController.GetProjectById(projectId);
            if (resProjects != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resProjects).StatusCode == 200)
            {
                ProjectDTO? projectDTO = (ProjectDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resProjects).Value;
                if(projectDTO != null)
                {
                    inputData = projectDTO;
                }                
            }

            inputData.ProjectId = projectId;

            IActionResult actionResultStatus = await _statusMasterAPIController.GetStatusMaster();
            ObjectResult objectResultStatus = (ObjectResult)actionResultStatus;
            List<StatusMasterDTO> statusMasterDTO = ((List<StatusMasterDTO>)objectResultStatus.Value);

            IActionResult actionResultProjectSum = await _projectsAPIController.GetDataProjectDetailsPage(inputData);
            ObjectResult objectResultProjectSum = (ObjectResult)actionResultProjectSum;
            outputData = ((ProjectDetailPageViewModel)objectResultProjectSum.Value);

            IActionResult actionResultFractionComplete0 = await _projectsAPIController.GetFractionComplete<FractionComplete0>(inputData, 0);
            ObjectResult objectResultFractionComplete0 = (ObjectResult)actionResultFractionComplete0;
            outputData.FractionComplete0 = ((FractionComplete0)objectResultFractionComplete0.Value);

            IActionResult actionResultFractionComplete1 = await _projectsAPIController.GetFractionComplete<FractionComplete1>(inputData, 1);
            ObjectResult objectResultFractionComplete1 = (ObjectResult)actionResultFractionComplete1;
            outputData.FractionComplete1 = ((FractionComplete1)objectResultFractionComplete1.Value);

            IActionResult actionResultFractionComplete3 = await _projectsAPIController.GetFractionComplete<FractionComplete3>(inputData, 3);
            ObjectResult objectResultFractionComplete3 = (ObjectResult)actionResultFractionComplete3;
            outputData.FractionComplete3 = ((FractionComplete3)objectResultFractionComplete3.Value);

            foreach (var item in outputData.ProjectDetailList)
            {
                item.StatusName = statusMasterDTO.Where(x => x.Pvalue == item.Status).Select(x => x.Pstatus).FirstOrDefault();
            }

            foreach (var item in outputData.SurveySpecsList)
            {
                item.StatusName = statusMasterDTO.Where(x => x.Pvalue == item.Status).Select(x => x.Pstatus).FirstOrDefault();
            }

            outputData.Project = inputData;

            return View(outputData);
        }
        else
        {
            return RedirectToAction("/Account/Login");
        }

        return View();
    }
    public async Task<IActionResult> Dashboard()
    {
        int? UserId = HttpContext.Session.GetInt32("UserId");
        if (UserId != null)
        {
            DashboardViewModel outputData = new DashboardViewModel();
            IActionResult actionResultUsers = await _accountAPIController.GetUsers();
            ObjectResult objectResultUsers = (ObjectResult)actionResultUsers;
            outputData.Managers = (List<UserDTO>)objectResultUsers.Value;

            IActionResult actionResultProjectSum = await _projectsAPIController.GetProjectsCount(UserId ?? default(int));
            ObjectResult objectResultProjectSum = (ObjectResult)actionResultProjectSum;
            outputData.ProjectCountSummary = ((IEnumerable<DashboardProjectCountSummaryViewModel>)objectResultProjectSum.Value).FirstOrDefault();

            UserDTO currentUser = new UserDTO();
            currentUser.UserId = UserId ?? default(int);
            outputData.CurrentUser = currentUser;

            return View(outputData);
        }
        else
        {
            return RedirectToAction("/VT/Account/Login");
        }

    }
    public async Task<IActionResult> GetProjects([FromBody] ProjectDTO inputData)
    {
        DashboardViewModel outputData = new DashboardViewModel();
        IActionResult actionResult = await _projectsAPIController.GetProjects(inputData);
        ObjectResult objResult = (ObjectResult)actionResult;
        outputData.ListOfProjects = (List<ProjectTableViewModel>)objResult.Value;

        return PartialView("_Dashboard/_ProjectTable", outputData);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> ChangeProjectStatus(int Status, int ProjectId)
    {
        IActionResult res = await _projectsAPIController.ChangeProjectStatus(Status, ProjectId);

        if (res is ObjectResult objectResult)
        {
            int statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;
            return StatusCode(statusCode, objectResult.Value);
        }

        return res;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }



    [HttpGet]
    public async Task<IActionResult> ExportToExcelDownloadData(int projectId)
    {
        ProjectDTO? projectDTO = new ProjectDTO();
        var resProjectDetils = await _projectsAPIController.GetProjectById(projectId);
        if (resProjectDetils != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resProjectDetils).StatusCode == 200)
        {
            projectDTO = (ProjectDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resProjectDetils).Value;
        }
        var res = await _projectsAPIController.GetProjectDetailsForDownload(projectId, 0);
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200 && projectDTO != null)
        {
            List<DownloadData>? resDownloadData = (List<DownloadData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            if (resDownloadData != null)
            {
                using var workbook = new XLWorkbook();

                string worksheetname = $"{projectDTO.Pid}_{projectDTO.Pname}";
                worksheetname = worksheetname.Substring(0, 30);

                var worksheet = workbook.Worksheets.Add(worksheetname);

                int row = 1;

                // Add Headers
                worksheet.Cell(row, 1).Value = "UID";
                worksheet.Cell(row, 2).Value = "SID";
                worksheet.Cell(row, 3).Value = "Country";
                worksheet.Cell(row, 4).Value = "Supplier Name";
                worksheet.Cell(row, 5).Value = "Supplier ID";
                worksheet.Cell(row, 6).Value = "Status";
                worksheet.Cell(row, 7).Value = "Start Date";
                worksheet.Cell(row, 8).Value = "End Date";
                worksheet.Cell(row, 9).Value = "Duration";
                worksheet.Cell(row, 10).Value = "Browser Details";
                worksheet.Cell(row, 11).Value = "IP Address";
                worksheet.Cell(row, 12).Value = "Device";
                worksheet.Cell(row, 13).Value = "Token";
                worksheet.Cell(row, 14).Value = "Notes";
                worksheet.Row(1).Style.Font.Bold = true;
                // Populate Data
                row += 1; // Start from the second row (after headers)
                foreach (var item in resDownloadData)
                {
                    worksheet.Cell(row, 1).Value = item.UID;
                    worksheet.Cell(row, 2).Value = item.SID;
                    worksheet.Cell(row, 3).Value = item.Country;
                    worksheet.Cell(row, 4).Value = item.SupplierName;
                    worksheet.Cell(row, 5).Value = item.SupplierID;
                    worksheet.Cell(row, 6).Value = item.Status;
                    if (DateTime.TryParse(item.StartDate, out DateTime startDate))
                    {
                        worksheet.Cell(row, 7).Value = startDate;
                        worksheet.Cell(row, 7).Style.DateFormat.Format = "dd:MM:yyyy:HH:mm:ss";
                    }
                    else
                    {
                        worksheet.Cell(row, 7).Value = item.StartDate; // Keep the original value if parsing fails
                    }
                    //worksheet.Cell(row, 7).Value = item.StartDate;
                    if (DateTime.TryParse(item.EndDate, out DateTime endDate))
                    {
                        worksheet.Cell(row, 8).Value = startDate;
                        worksheet.Cell(row, 8).Style.DateFormat.Format = "dd:MM:yyyy:HH:mm:ss";
                    }
                    else
                    {
                        worksheet.Cell(row, 8).Value = item.EndDate; // Keep the original value if parsing fails
                    }
                    //worksheet.Cell(row, 8).Value = item.EndDate;
                    worksheet.Cell(row, 9).Value = item.Duration;
                    worksheet.Cell(row, 10).Value = item.ClientBrowser;
                    worksheet.Cell(row, 11).Value = item.ClientIP;
                    worksheet.Cell(row, 12).Value = item.Device;
                    worksheet.Cell(row, 13).Value = item.Token;
                    worksheet.Cell(row, 14).Value = item.Notes;
                    row++;
                }

                // Adjust column widths
                worksheet.Columns().AdjustToContents();

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; // Reset stream position to the beginning

                // Return the Excel file without disposing of the stream
                var fileName = $"{projectDTO.Pid}_{projectDTO.Pname}_{DateTime.Now:ddMMyyyyHHmm}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

                // Save to MemoryStream
                //using var stream = new MemoryStream();
                //workbook.SaveAs(stream);
                //stream.Seek(0, SeekOrigin.Begin);

                //// Return Excel file
                //var fileName = $"ProjectDetails_{projectId}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        return null;
    }
    [HttpGet]
    public async Task<IActionResult> ExportToExcelDownloadResponseData(int projectId)
    {
        ProjectDTO? projectDTO = new ProjectDTO();
        var resProjectDetils = await _projectsAPIController.GetProjectById(projectId);
        if (resProjectDetils != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resProjectDetils).StatusCode == 200)
        {
            projectDTO = (ProjectDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resProjectDetils).Value;
        }
        var res = await _projectsAPIController.GetProjectDetailsForDownloadResponse(projectId, 0);
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200 && projectDTO != null)
        {
            List<ResponseData>? resDownloadData = (List<ResponseData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            if (resDownloadData != null)
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add($"SurveyResponses");

                int row = 1;

                // Add Headers
                worksheet.Cell(row, 1).Value = "UID";
                worksheet.Cell(row, 2).Value = "Col1";
                worksheet.Cell(row, 3).Value = "Col2";
                worksheet.Cell(row, 4).Value = "Col3";
                worksheet.Cell(row, 5).Value = "Col4";
                worksheet.Cell(row, 6).Value = "Col5";
                worksheet.Cell(row, 7).Value = "Col6";
                worksheet.Cell(row, 8).Value = "Col7";
                worksheet.Cell(row, 9).Value = "Col8";
                worksheet.Cell(row, 10).Value = "Col9";
                worksheet.Cell(row, 11).Value = "Col10";
                worksheet.Cell(row, 12).Value = "Col11";
                worksheet.Cell(row, 13).Value = "Col12";
                worksheet.Cell(row, 14).Value = "Col13";
                worksheet.Cell(row, 15).Value = "Col14";
                worksheet.Cell(row, 16).Value = "Col15";
                worksheet.Cell(row, 17).Value = "Col16";
                worksheet.Cell(row, 18).Value = "Col17";
                worksheet.Cell(row, 19).Value = "Col18";
                worksheet.Cell(row, 20).Value = "Col19";
                worksheet.Cell(row, 21).Value = "Col20";
                worksheet.Row(1).Style.Font.Bold = true;
                // Populate Data
                row += 1; // Start from the second row (after headers)
                foreach (var item in resDownloadData)
                {
                    worksheet.Cell(row, 1).Value = item.UID;
                    worksheet.Cell(row, 2).Value = item.Col1;
                    worksheet.Cell(row, 3).Value = item.Col2;
                    worksheet.Cell(row, 4).Value = item.Col3;
                    worksheet.Cell(row, 5).Value = item.Col4;
                    worksheet.Cell(row, 6).Value = item.Col5;
                    worksheet.Cell(row, 7).Value = item.Col6;
                    worksheet.Cell(row, 8).Value = item.Col7;
                    worksheet.Cell(row, 9).Value = item.Col8;
                    worksheet.Cell(row, 10).Value = item.Col9;
                    worksheet.Cell(row, 11).Value = item.Col10;
                    worksheet.Cell(row, 12).Value = item.Col11;
                    worksheet.Cell(row, 13).Value = item.Col12;
                    worksheet.Cell(row, 14).Value = item.Col13;
                    worksheet.Cell(row, 15).Value = item.Col14;
                    worksheet.Cell(row, 16).Value = item.Col15;
                    worksheet.Cell(row, 17).Value = item.Col16;
                    worksheet.Cell(row, 18).Value = item.Col17;
                    worksheet.Cell(row, 19).Value = item.Col18;
                    worksheet.Cell(row, 20).Value = item.Col19;
                    worksheet.Cell(row, 21).Value = item.Col20;

                    row++;
                }

                // Adjust column widths
                worksheet.Columns().AdjustToContents();

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; // Reset stream position to the beginning

                // Return the Excel file without disposing of the stream
                var fileName = $"SurveyResponse_{projectDTO.Pid}_{DateTime.Now:ddMMyyyyHHmm}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

                // Save to MemoryStream
                //using var stream = new MemoryStream();
                //workbook.SaveAs(stream);
                //stream.Seek(0, SeekOrigin.Begin);

                //// Return Excel file
                //var fileName = $"ProjectDetails_{projectId}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        return null;
    }
    [HttpGet]
    public async Task<IActionResult> ExportToExcelDetailedDateWiseReport(int projectId, DateTime startReportDate, DateTime endReportDate)
    {
        ProjectDTO? projectDTO = new ProjectDTO();
        var resProjectDetils = await _projectsAPIController.GetProjectById(projectId);
        if (resProjectDetils != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resProjectDetils).StatusCode == 200)
        {
            projectDTO = (ProjectDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resProjectDetils).Value;
        }
        var res = await _projectsAPIController.GetProjectDetailsDetailedDateWiseReport(projectId, startReportDate, endReportDate);
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200 && projectDTO != null)
        {
            List<DownloadData>? resDownloadData = (List<DownloadData>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            if (resDownloadData != null)
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add($"Report");

                int row = 1;

                // Add Headers
                worksheet.Cell(row, 1).Value = "Project Number";
                worksheet.Cell(row, 2).Value = "UID";
                worksheet.Cell(row, 3).Value = "Supplier Name";
                worksheet.Cell(row, 4).Value = "Supplier Id";
                worksheet.Cell(row, 5).Value = "Status";
                worksheet.Row(1).Style.Font.Bold = true;
                // Populate Data
                row += 1; // Start from the second row (after headers)
                foreach (var item in resDownloadData)
                {
                    worksheet.Cell(row, 1).Value = item.PID;
                    worksheet.Cell(row, 2).Value = item.UID;
                    worksheet.Cell(row, 3).Value = item.SupplierName;
                    worksheet.Cell(row, 4).Value = item.SupplierCode;
                    worksheet.Cell(row, 5).Value = item.Status;
                    row++;
                }

                // Adjust column widths
                worksheet.Columns().AdjustToContents();

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; // Reset stream position to the beginning

                // Return the Excel file without disposing of the stream
                var fileName = $"SurveyResponse_{projectDTO.Pid}_{DateTime.Now:ddMMyyyyHHmm}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

                // Save to MemoryStream
                //using var stream = new MemoryStream();
                //workbook.SaveAs(stream);
                //stream.Seek(0, SeekOrigin.Begin);

                //// Return Excel file
                //var fileName = $"ProjectDetails_{projectId}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        return null;
    }
    #region Danish
    public async Task<IActionResult> DeviceControl([FromBody] ProjectDTO model)
    {
        if (model == null)
        {
            return BadRequest("Invalid request");
        }
        var res = await _projectsAPIController.UpdateDeviceControl(model);
        return res;
    }

    
    public async Task<IActionResult> GetCountries()
    {
        var resCountries = await _countryAPIController.GetCountries();
        return resCountries;
        
    }

    public async Task<IActionResult> SaveMapIP([FromBody] SaveMapIPRequest model)
    {
        if (model == null)
        {
            return BadRequest("Invalid Request");
        }
        var res = await _projectsAPIController.UpdateMappings(model);

        return Ok(res);
    }

    public async Task<IActionResult> DeleteMappingCountries([FromBody] DeleteMapIPRequest model)
    {
        if (model == null)
        {
            return BadRequest("Invalid Request");
        }
        var res = await _projectsAPIController.DeleteMappings(model);

        return Ok(res);
    }
    public async Task<IActionResult> GetMappedCountries([FromBody] ProUrlIDModel ProUrlID)
    {
       
        var res = await _projectsAPIController.GetMappedCountries(ProUrlID.ProUrlID);
        return Ok(res);
    }
    [HttpPost]
    public async Task<IActionResult> GetProjectsDetailsbyid([FromBody]ProjectDTO model)
    {
         var ID= Convert.ToInt32(model.Action);
         var data = await _projectsAPIController.getProjectDetailsbyID(ID);
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus([FromBody] ProjectDTO request)
    {
        try
        {
            if ((request.ProjectId == 0) || string.IsNullOrEmpty(request.Action))
            {
                return Ok("Invalid request"); 
            }

          
            var result = await _projectsAPIController.UpdateStatus(request.ProjectId, request.Action);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while updating status");
        }
    }

    [HttpPost]
    public async Task<IActionResult> upDatePrescreening([FromBody] ProjectDTO request)
    {
        try
        {
            if (request.ProjectId == 0)
            {
                return Ok("Invalid request");
            }
            var result = await _projectsAPIController.upDatePrescreening(request.ProjectId, request.Prescreening);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while updating status");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdatepromappingCPI([FromBody] ProjectDTO model)
    {
        var result = await _projectsAPIController.UpdateCPIMapping(model);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateRedirectsInProMap([FromBody] ProjectDetailDTO model)
    {
        var result = await _projectsAPIController.UpdateRedirects(model);
        return Ok(result);
    }
    #endregion

}