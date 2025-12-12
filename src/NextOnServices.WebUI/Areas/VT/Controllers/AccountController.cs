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

            if (objResultData.UserType == "A")
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
}
