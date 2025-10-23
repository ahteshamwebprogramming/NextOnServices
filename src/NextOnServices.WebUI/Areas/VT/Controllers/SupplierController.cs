using GRP.Endpoints.Accounts;
using GRP.Infrastructure.Helper;
using Humanizer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using NextOnServices.Endpoints.Accounts;
using NextOnServices.Endpoints.Clients;
using NextOnServices.Endpoints.Masters;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Endpoints.Suppliers;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.Models.Supplier;
using NextOnServices.Infrastructure.ViewModels.Project;
using NextOnServices.Infrastructure.ViewModels.ProjectsURL;
using NextOnServices.VT.Infrastructure.ViewModels.Supplier;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using NextOnServices.Infrastructure.ViewModels.Supplier;
using NextOnServices.Core.Entities;
using System.Configuration;
using Newtonsoft.Json.Linq;
using NextOnServices.Infrastructure.ViewModels;
using NuGet.Protocol.Core.Types;
using System.Drawing.Printing;



namespace NextOnServices.WebUI.VT.Controllers;
[Area("VT")]
[Authorize]
public class SupplierController : Controller
{
    private readonly ILogger<SupplierController> _logger;
    private readonly SuppliersAPIController _suppliersAPIController;
    private readonly CountryAPIController _countryAPIController;
    private readonly SupplierChatAPIController _supplierChatApiController;
    private readonly IConfiguration _configuration;
    public SupplierController(ILogger<SupplierController> logger, SuppliersAPIController suppliersAPIController, CountryAPIController countryAPIController, SupplierChatAPIController supplierChatApiController, IConfiguration configuration)
    {
        _logger = logger;
        _suppliersAPIController = suppliersAPIController;
        this._countryAPIController = countryAPIController;
        _supplierChatApiController = supplierChatApiController;
        _configuration = configuration;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Login()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetProjectChatHistory([FromQuery] SupplierChatHistoryRequest request)
    {
        request ??= new SupplierChatHistoryRequest();

        PrepareChatApiController();
        var apiResult = await _supplierChatApiController.GetHistory(request);
        return ConvertApiResult(apiResult);
    }

    [HttpGet]
    public async Task<IActionResult> PollProjectChat(int projectMappingId, int? projectId, int? supplierId, string? pid, DateTimeOffset? after, int? pageSize, bool? unreadOnly)
    {
        var request = new SupplierChatHistoryRequest
        {
            ProjectMappingId = projectMappingId,
            ProjectId = projectId,
            SupplierId = supplierId,
            Pid = pid,
            PageSize = pageSize ?? 0,
            SinceCursor = after,
            UnreadOnly = unreadOnly ?? false
        };

        PrepareChatApiController();
        var apiResult = await _supplierChatApiController.Poll(request, after);
        return ConvertApiResult(apiResult);
    }

    [HttpPost]
    public async Task<IActionResult> PostProjectChatMessage([FromForm] SupplierChatSendRequest request)
        {
        if (request == null)
        {
            return BadRequest(new { message = "Message payload is required." });
        }

        PrepareChatApiController();
        var apiResult = await _supplierChatApiController.Send(request);
        return ConvertApiResult(apiResult);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadAttachment(int projectMappingId, string attachmentId, bool preview = false)
    {
        PrepareChatApiController();
        return await _supplierChatApiController.DownloadAttachment(projectMappingId, attachmentId, preview);
    }

    private void PrepareChatApiController()
    {
        _supplierChatApiController.ControllerContext = ControllerContext;
    }

    private IActionResult ConvertApiResult(IActionResult apiResult)
    {
        switch (apiResult)
        {
            case JsonResult jsonResult:
                return jsonResult;
            case ObjectResult objectResult:
                return new JsonResult(objectResult.Value)
                {
                    StatusCode = objectResult.StatusCode
                };
            case StatusCodeResult statusCodeResult:
                return StatusCode(statusCodeResult.StatusCode);
            case IStatusCodeActionResult status when status.StatusCode.HasValue:
                return StatusCode(status.StatusCode.Value);
            default:
                return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    public async Task<IActionResult> Dashboard()
    {
        var SupplierId = User.FindFirst("Id")?.Value;
        var loginSourceClaim = User.Claims
                .FirstOrDefault(c => c.Type == "LoginSource")?.Value;
        SupplierDashboardViewModel dto = new SupplierDashboardViewModel();

        if (SupplierId != null)
        {
            var rescard = await _suppliersAPIController.GetSupplierCardsData(Convert.ToInt32(SupplierId));
            var res = await _suppliersAPIController.GetSupplier(Convert.ToInt32(SupplierId));
            if (res != null &&  ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.Supplier = (SupplierDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }

            if (rescard != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)rescard).StatusCode == 200)
            {
                dto.ProjectDashboardCards = (ProjectDashboardCardsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)rescard).Value;
            }

            //var resProjects = await _suppliersAPIController.GetSupplierProjectsBySupplierId(Convert.ToInt32(SupplierId));
            //if (resProjects != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resProjects).StatusCode == 200)
            //{
            //    dto.SupplierProjects = (List<SupplierProjectsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resProjects).Value;
            //}
        }
        return View(dto);
    }
    public async Task<JsonResult> GetSupplierProjectsBySupplierId([FromBody] DataTablesRequest request)
    {
        try
        {
            var SupplierId = User.FindFirst("Id")?.Value;
            int? recordsTotal = 0;
            string search = (request != null && request.search != null && request.search.value != null) ? request.search.value : "";
            int start = (request != null && request.start != null) ? request.start : 0;
            string orderByColumn = (request != null && request.columns != null && request.order != null && request.order.Any()) ? request?.columns?[request?.order?[0].column ?? default(int)].data : "";
            string direction = (request != null && request.order != null && request.order.Any()) ? request?.order?[0].dir : "asc";
            var resProjects = await _suppliersAPIController.GetSupplierProjectsBySupplierId1("Table", Convert.ToInt32(SupplierId), request.start, request.length ?? default(int), search, orderByColumn, direction);
            List<SupplierProjectsDTO>? supplierProjects = new List<SupplierProjectsDTO>();
            if (resProjects != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resProjects).StatusCode == 200)
            {
                supplierProjects = (List<SupplierProjectsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resProjects).Value;
            }

            var resCounts = await _suppliersAPIController.GetSupplierProjectsBySupplierId1("Count", Convert.ToInt32(SupplierId), request.start, request.length ?? default(int), search, orderByColumn, direction);

            if (resCounts != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resCounts).StatusCode == 200)
            {
                List<int>? listrecordsTotal = (List<int>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resCounts).Value;
                if (listrecordsTotal != null && listrecordsTotal.Any())
                {
                    recordsTotal = listrecordsTotal[0];
                }
            }

            var result = new
            {
                draw = request.draw,
                recordsTotal = recordsTotal ?? default(int),
                recordsFiltered = recordsTotal ?? default(int),
                data = supplierProjects
            };
            return Json(result);
            //return Json(null);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retrieving Supplier Projects {nameof(GetSupplierProjectsBySupplierId)}");
            //return StatusCode(500, "Internal server error");
            return null;
        }
    }


    public async Task<JsonResult> GetSupplierProjectsBySupplierId_ExportData([FromBody] DataTablesRequest request)
    {
        try
        {
            var SupplierId = User.FindFirst("Id")?.Value;
            int? recordsTotal = 0;
            string search = (request != null && request.search != null && request.search.value != null) ? request.search.value : "";
            int start = (request != null && request.start != null) ? request.start : 0;
            string orderByColumn = (request != null && request.columns != null && request.order != null && request.order.Any()) ? request?.columns?[request?.order?[0].column ?? default(int)].data : "";
            string direction = (request != null && request.order != null && request.order.Any()) ? request?.order?[0].dir : "asc";

            start = (start - 1) * request.length ?? default(int);

            var resProjects = await _suppliersAPIController.GetSupplierProjectsBySupplierId1("Table", Convert.ToInt32(SupplierId), start, request.length ?? default(int), search, orderByColumn, direction);
            List<SupplierProjectsDTO>? supplierProjects = new List<SupplierProjectsDTO>();
            if (resProjects != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resProjects).StatusCode == 200)
            {
                supplierProjects = (List<SupplierProjectsDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resProjects).Value;
            }



            //int page = request.start;
            //int pageSize = request.length ?? default(int);
            //string sql = home.SQLQuery_Quotes_Admin();
            //sql += $" ORDER BY Id OFFSET {(page - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";
            //var parameters = new { };
            //var data = await _repository.GetTableData<MQuoteDataAdmin>(sql, parameters);
            return Json(supplierProjects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retrieving Supplier Projects {nameof(GetSupplierProjectsBySupplierId)}");
            //return StatusCode(500, "Internal server error");
            return null;
        }
    }


    public async Task<IActionResult> Dashboard_Old()
    {
        var SupplierId = User.FindFirst("Id")?.Value;
        var loginSourceClaim = User.Claims
                .FirstOrDefault(c => c.Type == "LoginSource")?.Value;
        if (SupplierId != null)
        {
            var res = await _suppliersAPIController.GetSupplier(Convert.ToInt32(SupplierId));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                SupplierDTO? supplierDTO = (SupplierDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                return View(supplierDTO);
            }
        }
        return View();
    }
    public async Task<IActionResult> Profile_old()
    {
        var SupplierId = User.FindFirst("Id")?.Value;
        if (SupplierId != null)
        {
            var res = await _suppliersAPIController.GetSupplier(Convert.ToInt32(SupplierId));
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                SupplierDTO? supplierDTO = (SupplierDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;

                if (supplierDTO != null)
                {
                    supplierDTO.encId = CommonHelper.EncryptURLHTML(supplierDTO.Id.ToString());
                }

                return View(supplierDTO);
            }
        }
        return View();
    }
    [Authorize(Roles = "Supplier")]
    public async Task<IActionResult> Profile()
    {
        //var SupplierId = User.FindFirst("Id")?.Value;
        //if (SupplierId != null)
        //{
        //    var res = await _suppliersAPIController.GetSupplier(Convert.ToInt32(SupplierId));
        //    if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        //    {
        //        SupplierDTO? supplierDTO = (SupplierDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;

        //        if (supplierDTO != null)
        //        {
        //            supplierDTO.encId = CommonHelper.EncryptURLHTML(supplierDTO.Id.ToString());
        //        }

        //        return View(supplierDTO);
        //    }
        //}
        //return View();

        //return;

        string? eSupplierId = User.FindFirst("Id")?.Value;

        SupplierViewModel dto = new SupplierViewModel();
        SupplierDTO? supplierDTO = new SupplierDTO();
        try
        {
            var countriesRes = await _countryAPIController.GetCountries();
            if (countriesRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)countriesRes).StatusCode == 200)
            {
                dto.Countries = (List<CountryMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)countriesRes).Value;
            }
            if (!String.IsNullOrEmpty(eSupplierId))
            {
                int supplierId = Convert.ToInt32(eSupplierId);
                var resSupplier = await _suppliersAPIController.GetSupplier(supplierId);
                if (resSupplier != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resSupplier).StatusCode == 200)
                {
                    supplierDTO = (SupplierDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resSupplier).Value;
                    if (supplierDTO != null)
                    {
                        supplierDTO.encId = CommonHelper.EncryptURLHTML(supplierDTO.Id.ToString());
                    }
                }
            }
            else
            {
                supplierDTO.Id = 0;
            }
        }
        catch (Exception ex)
        {
            if (supplierDTO == null)
                supplierDTO = new SupplierDTO();
            supplierDTO.Id = 0;
        }
        dto.Supplier = supplierDTO;
        return View(dto);

    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(SupplierLoginDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        //EHRMSLoginDTO? result = new EHRMSLoginDTO();
        SupplierWithChild? outputDTO = new SupplierWithChild();
        var res = await _suppliersAPIController.GetSupplierLoginDetail(dto);
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            outputDTO = (SupplierWithChild?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            if (outputDTO != null)
            {
                HttpContext.Session.SetString("User", JsonConvert.SerializeObject(outputDTO));
                var claims = new List<Claim>      {
                new Claim(ClaimTypes.Name,outputDTO.Name.ToString()),
                new Claim(ClaimTypes.Role,"Supplier"),
                new Claim("Id", outputDTO?.Id.ToString()),
                new Claim("Email", outputDTO?.Email?.ToString()),
                new Claim("LoginSource", "SupplierLogin") // Add the LoginSource claim                
            };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role)
                //{
                //    Label = "SupplierIdentity",  // Optional label
                //    BootstrapContext = "Supplier",  // Optional additional context
                //    //AuthenticationType = CookieAuthenticationDefaults.AuthenticationScheme
                //};

                if (outputDTO.FirstTimePasswordReset)
                {
                    var authProperties = new AuthenticationProperties
                    {
                        RedirectUri = "/VT/Supplier/Dashboard",
                        //IsPersistent = true,  // Keep the user logged in until they log out
                        //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Session expiration
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    return Redirect(authProperties.RedirectUri);
                }
                else
                {
                    var authProperties = new AuthenticationProperties
                    {
                        RedirectUri = "/VT/Supplier/ResetPassword/" + CommonHelper.EncryptURLHTML(outputDTO.Id.ToString()),
                        //IsPersistent = true,  // Keep the user logged in until they log out
                        //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Session expiration
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    return Redirect(authProperties.RedirectUri);
                }
            }
            else
            {
                ModelState.AddModelError("LoginNotFound", "Login Failed. Username or password is incorrect");
                return View();
            }
        }
        else
        {
            ModelState.AddModelError("LoginNotFound", "Login Failed. Username or password is incorrect");
            return View();
        }
    }
    [AllowAnonymous]
    public async Task<IActionResult> ForgetPassword()
    {
        return View();
    }
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ForgetPassword(SupplierForgetPassword dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        bool isSupplierCodeValid = await _suppliersAPIController.ValidateSupplierCode(dto);

        if (!isSupplierCodeValid)
        {
            ModelState.AddModelError(string.Empty, "The Supplier Code is not correct");
            return View(dto);
        }

        var resSupplier = await _suppliersAPIController.GetSupplierBySupplierCode(dto);
        if (resSupplier != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resSupplier).StatusCode == 200)
        {
            SupplierDTO? supplier = (SupplierDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resSupplier).Value;
            if (supplier != null)
            {
                if (!String.IsNullOrEmpty(supplier.Email))
                {
                    var ServerDomain = _configuration["ServerDomain"];
                    string body = $"<p>Dear {supplier.Name},</p> <p>To&nbsp;reset your account password, please click the link below:</p> <p><a href=\"{ServerDomain}/VT/Supplier/ResetPassword/" + CommonHelper.EncryptURLHTML(supplier.Id.ToString()) + "\" target=\"\\&quot;_new\\&quot;\" rel=\"\\&quot;noopener\\&quot;\"><strong>Click&nbsp;Here</strong></a></p> <p>If you have any questions, feel free to reach out to our support team.</p> <p>Thank you,<br />Nexton Services<br />XX-XXXXXXXXXX</p>";
                    string subject = "Welcome! Password Reset Link";
                    if (!String.IsNullOrEmpty(supplier.Email))
                    {
                        MailHelper.SendEmail(subject, body, supplier.Email);
                    }
                    //MailHelper.SendEmail(subject, body, "mhdahtesham@gmail.com");
                    return RedirectToAction("PasswordResetEmailSent");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No email is registered with this supplier code. Please contact admin");
                    return View(dto);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Unable to find details of this supplier code. Please contact admin");
                return View(dto);
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Unable to find details of this supplier code. Please contact admin");
            return View(dto);
        }
        //if (updated)
        //{
        //    // Log out the user after password change
        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //    // Redirect to the success page or login page
        //    return RedirectToAction("PasswordChangeSuccess");
        //}
        //else
        //{
        //    ModelState.AddModelError(string.Empty, "Unable to change password right now. Please try again later");
        //    return View(dto);
        //}

    }
    [AllowAnonymous]
    [Route("/VT/Supplier/ResetPassword/{endId}")]

    public async Task<IActionResult> ResetPassword(string endId)
    {
        SupplierResetPasswordDTO supplierResetPasswordDTO = new SupplierResetPasswordDTO();
        supplierResetPasswordDTO.encId = endId;
        return View(supplierResetPasswordDTO);
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ResetPassword(SupplierResetPasswordDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        if (!String.IsNullOrEmpty(dto.encId))
        {
            dto.Id = Convert.ToInt32(CommonHelper.DecryptURLHTML(dto.encId));
            var updated = await _suppliersAPIController.ResetPassword(dto);
            if (updated)
            {
                // Log out the user after password change
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Redirect to the success page or login page
                return RedirectToAction("PasswordResetSuccess");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Unable to reset password right now. Please try again later");
                return View(dto);
            }
        }


        ModelState.AddModelError(string.Empty, "Error validating the user. PLease login and try again.");
        return View(dto);
    }
    public async Task<IActionResult> ChangePassword()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> ChangePassword(SupplierChangePasswordDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var SupplierId = User.FindFirst("Id")?.Value;
        if (SupplierId == null)
        {
            ModelState.AddModelError(string.Empty, "Error validating the user. PLease login and try again.");
            return View(dto);
        }
        dto.Id = Convert.ToInt32(SupplierId);
        // Simulate password validation (replace with actual logic)
        bool isOldPasswordValid = await _suppliersAPIController.ValidateOldPassword(dto);

        if (!isOldPasswordValid)
        {
            ModelState.AddModelError(string.Empty, "The old password does not match.");
            return View(dto);
        }

        var updated = await _suppliersAPIController.ChangePassword(dto);
        if (updated)
        {
            // Log out the user after password change
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to the success page or login page
            return RedirectToAction("PasswordChangeSuccess");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Unable to change password right now. Please try again later");
            return View(dto);
        }
        // Proceed with changing the password if valid

    }

    [AllowAnonymous]
    public IActionResult PasswordChangeSuccess()
    {
        return View(); // This will show the success page
    }
    [AllowAnonymous]
    public IActionResult PasswordResetSuccess()
    {
        return View(); // This will show the success page
    }
    [AllowAnonymous]
    public IActionResult PasswordResetEmailSent()
    {
        return View(); // This will show the success page
    }

    [Route("/VT/Supplier/AddSupplier/{eSupplierId=null}")]
    public async Task<IActionResult> AddSupplier(string? eSupplierId = null)
    {
        SupplierViewModel dto = new SupplierViewModel();
        SupplierDTO? supplierDTO = new SupplierDTO();
        try
        {
            var countriesRes = await _countryAPIController.GetCountries();
            if (countriesRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)countriesRes).StatusCode == 200)
            {
                dto.Countries = (List<CountryMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)countriesRes).Value;
            }
            if (!String.IsNullOrEmpty(eSupplierId))
            {
                int supplierId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eSupplierId));
                var resSupplier = await _suppliersAPIController.GetSupplier(supplierId);
                if (resSupplier != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resSupplier).StatusCode == 200)
                {
                    supplierDTO = (SupplierDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resSupplier).Value;
                }
            }
            else
            {
                supplierDTO.Id = 0;
            }
        }
        catch (Exception ex)
        {
            if (supplierDTO == null)
                supplierDTO = new SupplierDTO();
            supplierDTO.Id = 0;
        }
        dto.Supplier = supplierDTO;
        return View(dto);
    }
    public async Task<IActionResult> SupplierList()
    {
        SupplierViewModel dto = new SupplierViewModel();
        var res = await _suppliersAPIController.GetAllSuppliers();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.SupplierList = (List<SupplierDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            if (dto.SupplierList != null)
            {
                foreach (var item in dto.SupplierList)
                {
                    item.encId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                }
            }
        }
        return View(dto);
    }
    [Route("/VT/Supplier/SupplierPreview/{eSupplierId}")]
    public async Task<IActionResult> SupplierPreview(string eSupplierId)
    {
        if (string.IsNullOrWhiteSpace(eSupplierId))
        {
            return RedirectToAction(nameof(SupplierList));
        }

        SupplierViewModel dto = new SupplierViewModel();

        try
        {
            int supplierId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eSupplierId));

            var resSupplier = await _suppliersAPIController.GetSupplier(supplierId);
            if (resSupplier is ObjectResult supplierResult && (supplierResult.StatusCode ?? StatusCodes.Status200OK) == StatusCodes.Status200OK)
            {
                if (supplierResult.Value is SupplierDTO supplier)
                {
                    dto.Supplier = supplier;
                    dto.Supplier.encId = eSupplierId;
                }
            }

            var resPanelSizes = await _suppliersAPIController.GetSupplierCountryPanelSizeBySupplierId(supplierId);
            if (resPanelSizes is ObjectResult panelSizeResult && (panelSizeResult.StatusCode ?? StatusCodes.Status200OK) == StatusCodes.Status200OK)
            {
                if (panelSizeResult.Value is IEnumerable<SupplierPanelSizeWithChild> panelSizes)
                {
                    dto.SupplierPanelSizes = panelSizes.ToList();
                }
            }

            var resDeliveries = await _suppliersAPIController.GetSupplierDeliverySummary(supplierId);
            if (resDeliveries is ObjectResult deliveryResult && (deliveryResult.StatusCode ?? StatusCodes.Status200OK) == StatusCodes.Status200OK)
            {
                if (deliveryResult.Value is IEnumerable<SupplierDeliverySummary> deliveries)
                {
                    dto.SupplierDeliveryDetails = deliveries.ToList();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while loading supplier preview for {SupplierKey}", eSupplierId);
        }

        if (dto.Supplier == null)
        {
            dto.Supplier = new SupplierDTO { encId = eSupplierId };
        }

        return View(dto);
    }
    public async Task<IActionResult> ManageSupplier([FromBody] SupplierDTO inputDTO)
    {
        try
        {
            if (inputDTO.Id > 0)
            {
                var res = await _suppliersAPIController.AddSupplier(inputDTO);
                return res;
            }
            else
            {
                inputDTO.Sstatus = 1;
                inputDTO.IsActive = 1;
                inputDTO.AllowHashing = 0;
                inputDTO.CreationDate = DateTime.Now;
                var res = await _suppliersAPIController.AddSupplier(inputDTO);

                return res;
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    [Route("/VT/Supplier/SupplierCountryPanelSize/{eSupplierId=null}")]
    public async Task<IActionResult> SupplierCountryPanelSize(string? eSupplierId = null)
    {
        SupplierDTO? supplierDTO = new SupplierDTO();
        supplierDTO.encId = eSupplierId;
        return View(supplierDTO);
    }

    public async Task<IActionResult> AddSupplierCountryPanelSizePartialView([FromBody] SupplierPanelSizeDTO inputDTO)
    {
        SupplierPanelSizeViewModel? dto = new SupplierPanelSizeViewModel();
        try
        {
            var resCountries = await _countryAPIController.GetCountries();
            if (resCountries != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resCountries).StatusCode == 200)
            {
                dto.Countries = ((List<CountryMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resCountries).Value);
            }
            if (inputDTO.Id > 0)
            {
                var res = await _suppliersAPIController.GetSupplierCountryPanelSizeById(inputDTO.Id);
                if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto.SupplierPanelSize = (SupplierPanelSizeDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }
            }
            return PartialView("_supplierCountryPanelSize/_addSupplierCountryPanelSize", dto);
        }
        catch (Exception ex)
        {
            return PartialView("_supplierCountryPanelSize/_addSupplierCountryPanelSize", dto);
        }
    }
    public async Task<IActionResult> ListSupplierCountryPanelSizePartialView([FromBody] SupplierPanelSizeDTO inputDTO)
    {
        SupplierPanelSizeViewModel? dto = new SupplierPanelSizeViewModel();
        if (inputDTO.encSupplierId != null)
        {
            int SupplierId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encSupplierId));
            var res = await _suppliersAPIController.GetSupplierCountryPanelSizeBySupplierId(SupplierId);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.SupplierPanelSizeWithChildren = (List<SupplierPanelSizeWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return PartialView("_supplierCountryPanelSize/_listSupplierCountryPanelSize", dto);
    }
    public async Task<IActionResult> SaveSupplierCountryPanelSize([FromBody] SupplierPanelSizeDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.encSupplierId != null)
                {
                    inputDTO.SupplierId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encSupplierId));
                    var res = await _suppliersAPIController.AddSupplierCountryPanelSize(inputDTO);
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
    public async Task<IActionResult> ChangeSupplierStatus([FromBody] SupplierDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.Id > 0)
                {
                    var res = await _suppliersAPIController.ChangeSupplierStatus(inputDTO);
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

    public async Task<IActionResult> UpdateSupplierDetailsFromProfilePage([FromBody] SupplierFromProfile inputDTO)
    {
        try
        {
            if (inputDTO != null && !(String.IsNullOrEmpty(inputDTO.encId)))
            {
                inputDTO.Id = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encId));

                if (inputDTO.Id > 0)
                {
                    var res = await _suppliersAPIController.UpdateSupplierFromProfile(inputDTO);
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
    public async Task<IActionResult> WithDrawlProject([FromBody] ProjectMappingDTO inputDTO)
    {
        try
        {
            if (inputDTO != null && inputDTO.Id > 0)
            {
                var res = await _suppliersAPIController.WithDrawlProjectFromSupplier(inputDTO.Id);
                return res;
            }
            return BadRequest("Unable To Save right now");
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    public async Task<IActionResult> StartProject([FromBody] ProjectMappingDTO inputDTO)
    {
        try
        {
            if (inputDTO != null && inputDTO.Id > 0)
            {
                var res = await _suppliersAPIController.StartProjectFromSupplier(inputDTO.Id);
                return res;
            }
            return BadRequest("Unable To Save right now");
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}

