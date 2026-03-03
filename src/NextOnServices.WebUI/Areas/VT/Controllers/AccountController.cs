using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NextOnServices.Endpoints.Accounts;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.Account;
using System.Security.Claims;

namespace NextOnServices.WebUI.VT.Controllers;

[Area("VT")]
public class AccountController : Controller
{
    private readonly AccountsController _accountsApiController;
    public AccountController(AccountsController accountsApiController)
    {
        _accountsApiController = accountsApiController;
    }
    public IActionResult Login()
    {
        HttpContext.Session.Clear();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserDTO user)
    {
        IActionResult actionResult = _accountsApiController.GetLoginDetail(user);
        if (actionResult == null || !(actionResult is ObjectResult objResult) || objResult.Value == null)
        {
            return RedirectToAction("Login");
        }

        UserDTO objResultData = (UserDTO)objResult.Value;
        HttpResponseMessage getdata = objResultData.HttpMessage;
        if (getdata.IsSuccessStatusCode)
        {
            string result = getdata.Content.ReadAsStringAsync().Result;
            HttpContext.Session.SetInt32("UserId", objResultData.UserId);

            HttpContext.Session.SetString("User", JsonConvert.SerializeObject(user));

            string routePage = "";

            if (objResultData.UserType == "A" || objResultData.UserType == "U")
            {
                routePage = "/VT/Home/Dashboard";
            }
            else
            {
                routePage = "/VT/Projects/ProjectsList";
            }

            var claims = new List<Claim>      {
                new Claim(ClaimTypes.Name,objResultData.UserName.ToString()),
                new Claim(ClaimTypes.Role,objResultData.UserType),
                new Claim("Id", objResultData?.UserId.ToString()),
                new Claim("Email", objResultData?.EmailId?.ToString()),
                new Claim("LoginSource", "VTLogin") // Add the LoginSource claim                
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                RedirectUri = routePage,
                //IsPersistent = true,  // Keep the user logged in until they log out
                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Session expiration
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            return Redirect(authProperties.RedirectUri);

            //return RedirectToAction("Dashboard", "Home");
        }
        else
        {

            if (getdata.ReasonPhrase == "Not Found")
            {
                ModelState.AddModelError("LoginNotFound", "Login Failed. Username or password is incorrect");
                return View();
            }
            else
            {
                ModelState.AddModelError("LoginNotFound", "Login Failed. " + getdata.ReasonPhrase);
                return View();
            }
        }
    }
    public ActionResult AdminEncryptionPage()
    {
        AdminEncryption inp = new AdminEncryption();
        return View(inp);
    }
    public ActionResult EncryptInput(AdminEncryption input)
    {
        input.EncryptOutput = CommonHelper.Encrypt(input.EncryptInput);
        return View("AdminEncryptionPage", input);
    }
    public ActionResult DecryptInput(AdminEncryption input)
    {
        input.DecryptOutput = CommonHelper.Decrypt(input.DecryptInput);
        return View("AdminEncryptionPage", input);
    }

    /// <summary>GET Add User page (VT theme).</summary>
    [Route("/VT/Account/AddUser")]
    [HttpGet]
    public IActionResult AddUser()
    {
        return View();
    }

    /// <summary>POST Create user from VT Add User form.</summary>
    [Route("/VT/Account/CreateUser")]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] RegisterUserRequest request)
    {
        if (request == null)
            return BadRequest("Invalid request.");
        IActionResult result = await _accountsApiController.RegisterUser(request);
        if (result is ObjectResult objRes)
        {
            if (objRes.StatusCode == 200)
                return Ok(objRes.Value);
            return StatusCode(objRes.StatusCode ?? 400, objRes.Value);
        }
        return BadRequest("User creation failed.");
    }

    /// <summary>GET Users Details list page (VT theme).</summary>
    [Route("/VT/Account/UsersDetails")]
    [HttpGet]
    public IActionResult UsersDetails()
    {
        return View();
    }

    /// <summary>POST Get user list for VT Users Details DataTable.</summary>
    [Route("/VT/Account/GetUserDetails")]
    [HttpPost]
    public async Task<IActionResult> GetUserDetails()
    {
        var result = await _accountsApiController.GetUsers();
        if (result is not ObjectResult objRes || objRes.Value == null)
            return Ok(new List<object>());
        var users = (List<UserDTO>)objRes.Value;
        var list = users.Select(u => new
        {
            userId = u.UserId,
            userName = u.UserName ?? u.UserCode ?? "",
            emailId = u.EmailId ?? "",
            contactNumber = u.ContactNumber ?? "",
            isActive = u.IsActive ?? 0,
            profileUrl = "/VT/Account/UserProfile/" + CommonHelper.EncryptURLHTML(u.UserId.ToString()),
            editUrl = "/VT/Account/EditUser/" + CommonHelper.EncryptURLHTML(u.UserId.ToString())
        }).ToList();
        return Ok(list);
    }

    /// <summary>POST Set user active/inactive from VT Users Details.</summary>
    [Route("/VT/Account/SetUserStatus")]
    [HttpPost]
    public IActionResult SetUserStatus([FromBody] SetUserStatusRequest request)
    {
        if (request == null)
            return BadRequest("Invalid request.");
        var result = _accountsApiController.SetUserStatus(request);
        if (result is ObjectResult objRes)
            return StatusCode(objRes.StatusCode ?? 200, objRes.Value);
        return BadRequest();
    }

    /// <summary>GET User Profile page (read-only view).</summary>
    [Route("/VT/Account/UserProfile/{eUserId}")]
    [HttpGet]
    public async Task<IActionResult> UserProfile(string eUserId)
    {
        if (string.IsNullOrEmpty(eUserId))
            return RedirectToAction("UsersDetails");
        int userId;
        try
        {
            var userIdStr = CommonHelper.DecryptURLHTML(eUserId);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out userId))
                return RedirectToAction("UsersDetails");
        }
        catch
        {
            return RedirectToAction("UsersDetails");
        }
        var result = await _accountsApiController.GetUserById(new GetUserByIdRequest { UserId = userId });
        if (result is not ObjectResult objRes || objRes.Value == null)
            return RedirectToAction("UsersDetails");
        return View((UserDTO)objRes.Value);
    }

    /// <summary>GET Edit User page; loads user and returns edit form.</summary>
    [Route("/VT/Account/EditUser/{eUserId}")]
    [HttpGet]
    public async Task<IActionResult> EditUser(string eUserId)
    {
        if (string.IsNullOrEmpty(eUserId))
            return RedirectToAction("UsersDetails");
        int userId;
        try
        {
            var userIdStr = CommonHelper.DecryptURLHTML(eUserId);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out userId))
                return RedirectToAction("UsersDetails");
        }
        catch
        {
            return RedirectToAction("UsersDetails");
        }
        var result = await _accountsApiController.GetUserById(new GetUserByIdRequest { UserId = userId });
        if (result is not ObjectResult objRes || objRes.Value == null)
            return RedirectToAction("UsersDetails");
        var user = (UserDTO)objRes.Value;
        ViewData["EncryptedUserId"] = eUserId;
        return View(user);
    }

    /// <summary>POST Update user from VT Edit User form.</summary>
    [Route("/VT/Account/UpdateUser")]
    [HttpPost]
    public IActionResult UpdateUser([FromBody] UpdateUserRequest request)
    {
        if (request == null)
            return BadRequest("Invalid request.");
        var result = _accountsApiController.UpdateUser(request);
        if (result is ObjectResult objRes)
            return StatusCode(objRes.StatusCode ?? 200, objRes.Value);
        return BadRequest();
    }

    /// <summary>GET current user's profile (convenience route for header dropdown).</summary>
    [Route("/VT/Account/MyProfile")]
    [HttpGet]
    public async Task<IActionResult> MyProfile()
    {
        // Try to get user id from claims first
        int userId = 0;
        var idClaim = User?.FindFirst("Id")?.Value;
        if (!string.IsNullOrEmpty(idClaim) && int.TryParse(idClaim, out userId) && userId > 0)
        {
            // ok
        }
        else
        {
            // Fallback to session
            var userJson = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userJson))
                return RedirectToAction("Login");

            UserDTO? sessionUser = null;
            try
            {
                sessionUser = JsonConvert.DeserializeObject<UserDTO>(userJson);
            }
            catch
            {
                // ignore, will redirect to login below
            }

            if (sessionUser == null || sessionUser.UserId <= 0)
                return RedirectToAction("Login");

            userId = sessionUser.UserId;
        }

        var result = await _accountsApiController.GetUserById(new GetUserByIdRequest { UserId = userId });
        if (result is not ObjectResult objRes || objRes.Value == null)
            return RedirectToAction("Login");

        var userDto = (UserDTO)objRes.Value;
        return View("UserProfile", userDto);
    }

    /// <summary>GET VT Change Password page for the current user.</summary>
    [Route("/VT/Account/ChangePassword")]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return RedirectToAction("Login");
        return View();
    }

    /// <summary>POST VT Change Password for the current user.</summary>
    [Route("/VT/Account/ChangePassword")]
    [HttpPost]
    public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return RedirectToAction("Login");

        if (string.IsNullOrWhiteSpace(oldPassword) ||
            string.IsNullOrWhiteSpace(newPassword) ||
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            ViewBag.Error = "All fields are required.";
            return View();
        }

        if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
        {
            ViewBag.Error = "New password and confirm password do not match.";
            return View();
        }

        // Resolve current user id (claims first, then session fallback)
        int userId = 0;
        var idClaim = User?.FindFirst("Id")?.Value;
        if (!string.IsNullOrEmpty(idClaim) && int.TryParse(idClaim, out userId) && userId > 0)
        {
            // ok
        }
        else
        {
            var userJson = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userJson))
            {
                return RedirectToAction("Login");
            }

            UserDTO? sessionUser = null;
            try
            {
                sessionUser = JsonConvert.DeserializeObject<UserDTO>(userJson);
            }
            catch
            {
            }

            if (sessionUser == null || sessionUser.UserId <= 0)
                return RedirectToAction("Login");

            userId = sessionUser.UserId;
        }

        try
        {
            var apiResult = await _accountsApiController.ChangePasswordVT(new ChangePasswordVTRequest
            {
                UserId = userId,
                OldPassword = oldPassword,
                NewPassword = newPassword
            });

            if (apiResult is ObjectResult objRes)
            {
                if (objRes.StatusCode == 200)
                {
                    // On success, sign the user out and ask them to log in again.
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.Session.Clear();
                    TempData["ChangePasswordSuccess"] = "Password changed successfully. Please login again.";
                    return RedirectToAction("Login");
                }

                ViewBag.Error = objRes.Value?.ToString() ?? "Unable to change password.";
                return View();
            }

            if (apiResult is BadRequestObjectResult badRequest)
            {
                ViewBag.Error = badRequest.Value?.ToString() ?? "Unable to change password.";
                return View();
            }

            ViewBag.Error = "Unable to change password.";
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View();
        }
    }

    /// <summary>Logout current VT user: clear auth cookie and session, then go to login.</summary>
    [Route("/VT/Account/Logout")]
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
