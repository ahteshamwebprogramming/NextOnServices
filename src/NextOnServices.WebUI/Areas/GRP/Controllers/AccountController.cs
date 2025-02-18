using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


using GRP.Endpoints.Accounts;
using GRP.Infrastructure.Helper;
using GRP.Infrastructure.Models.Account;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Components.Forms;
using GRP.Infrastructure.ViewModels.Account;
using GRP.Core.Entities;
using NextOnServices.Core.Entities;

using GRP.Infrastructure.ViewModels.Pages;
using Humanizer;
using Microsoft.VisualBasic;

using System.Reflection.Metadata;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Security.Cryptography;
using GRP.Endpoints.Masters;

namespace NextOnServices.WebUI.Areas.GRP.Controllers;

[Area("GRP")]
public class AccountController : Controller
{
    private readonly AccountsAPIController _accountsApiController;
    private readonly MastersAPIController _mastersAPIController;
    public AccountController(AccountsAPIController accountsApiController, MastersAPIController mastersAPIController)
    {
        _accountsApiController = accountsApiController;
        _mastersAPIController = mastersAPIController;
    }
    public IActionResult Login()
    {
        HttpContext.Session.Clear();
        return View();
    }
    public async Task<IActionResult> Register()
    {
        UserDTO userDTO = new UserDTO();
        userDTO.Countries = await _mastersAPIController.CountryList();
        return View(userDTO);
    }
    public IActionResult ForgetPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDetailDTO login)
    {
        if (!ModelState.IsValid)
        {
            return View(login);
        }
        login.EncryptedPassword = CommonHelper.Encrypt(login.Password);
        IActionResult actionResult = _accountsApiController.GetLoginDetail(login);
        ObjectResult objResult = (ObjectResult)actionResult;
        LoginDetailDTO objResultData = (LoginDetailDTO)objResult.Value;
        HttpResponseMessage getdata = objResultData.HttpMessage;
        if (getdata.IsSuccessStatusCode)
        {

            string result = getdata.Content.ReadAsStringAsync().Result;
            HttpContext.Session.SetString("LoginId", objResultData.LoginId.ToString());
            HttpContext.Session.SetString("UserId", objResultData.LoginId.ToString());
            UserDTO user = await _accountsApiController.GetUserById(objResultData.UserId);
            HttpContext.Session.SetString("User", JsonConvert.SerializeObject(user));

            if (objResultData.PasswordChanged == false)
            {
                return RedirectToAction("ChangePassword");
            }

            if (user != null)
            {
                if (user.RoleName == "Respondent")
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                else if (user.RoleName == "Admin")
                {
                    return RedirectToAction("List", "Survey");
                }
                else if (user.RoleName == "Panel Manager")
                {
                    return RedirectToAction("PointsManager", "Points");
                }
                else
                {

                }
            }

            return RedirectToAction("Dashboard", "Home");
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
    [HttpPost]
    public async Task<IActionResult> Register(UserDTO userDTO)
    {
        if (ModelState.IsValid)
        {
            string randomGeneratedPassword = CommonHelper.RandomString(8);

            userDTO.IsActive = true;
            userDTO.CreatedDate = DateTime.Now;
            userDTO.RoleId = 3;
            var objRes = await _accountsApiController.RegisterUser(userDTO, randomGeneratedPassword);
            if (objRes != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).StatusCode == 200)
                {
                    string subject = "Login Credentials";
                    string body = @"<p>Dear " + userDTO.FirstName + ",</p><p> We are delighted to inform you that your account has been successfully created. Welcome aboard to our platform! </p> <p> Below are your login credentials:</p> <p> Username: " + userDTO.EmailId + "</p><p> Password: " + randomGeneratedPassword + "</p> <p> We highly recommend keeping this information secure and not sharing it with anyone. If you have any concerns regarding your account security or if you suspect any unauthorized activity, please don't hesitate to contact us immediately.</p> <p> Thank you for choosing to be a part of our community.We look forward to providing you with a seamless and enjoyable experience.</p> <p> Best regards,</p> <p> Global Research Team </p>";
                    string mailTo = userDTO.EmailId;
                    MailHelper.SendEmail(subject, body, mailTo);

                    ModelState.AddModelError("Success", "Registered Successfully");
                    UserDTO u = new UserDTO();
                    Message message = new Message();
                    message.MessageHeading = "Registered Successfully";
                    message.MessageInfo = "Please click below to go to login page";
                    message.ButtonText = "Go to login page";
                    message.Redirect = "/GRP/Account/Login";
                    return View("../Pages/Message", message);
                    //return RedirectToAction("Message", "Pages");
                }
                else
                {
                    string error = ((Microsoft.AspNetCore.Mvc.ObjectResult)objRes).Value.ToString();
                    ModelState.AddModelError("Error", error);
                    return View(userDTO);
                }
            }
            else
            {
                return View(userDTO);
            }
        }
        else
        {
            return View(userDTO);
        }

    }
    [HttpPost]
    public async Task<ActionResult> ForgetPassword(ForgetPasswordViewModel inputData)
    {
        try
        {
            if (String.IsNullOrEmpty(inputData.Email))
            {
                throw new Exception("Email Id is required");
            }
            else
            {
                LoginDetailDTO userDTO = new LoginDetailDTO();
                userDTO.Username = inputData.Email;
                var res = _accountsApiController.CheckEmailExists(userDTO);
                if (res != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                    {
                        var employeeObj = ((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                        if (employeeObj != null)
                        {
                            LoginDetailDTO employee = (LoginDetailDTO)employeeObj;
                            var resUser = await _accountsApiController.GetUserById(employee.UserId);
                            if (resUser == null)
                            {
                                throw new Exception("Unable to find user details");
                            }
                            if (employee != null)
                            {
                                string subject = "Password Reset Link";
                                string body = "<p>Hi " + resUser.FirstName + ",</p><p>Below is the link to reset you password.</p><p><a href=\"http://182.18.138.221/grp.resetpassword/" + CommonHelper.EncryptURLHTML(employee.LoginId.ToString()) + "\">http://182.18.138.221/grp.resetpassword/" + CommonHelper.EncryptURLHTML(employee.LoginId.ToString()) + "</a></p><p>Thanks and Regrads</p><p>Global Research Panel Team</p>";
                                //string body = "<p>Hi " + resUser.FirstName + ",</p><p>Below is the link to reset you password.</p><p><a href=\"http://localhost:5288/grp.resetpassword/" + CommonHelper.EncryptURLHTML(employee.LoginId.ToString()) + "\">http://localhost:5288/grp.resetpassword/" + CommonHelper.EncryptURLHTML(employee.LoginId.ToString()) + "</a></p><p>Thanks and Regrads</p><p>Global Research Panel Team</p>";
                                string mailTo = employee.Username;
                                MailHelper.SendEmail(subject, body, mailTo);
                            }
                            else
                            {
                                throw new Exception("Unable to find the email");
                            }
                        }
                        else
                        {
                            throw new Exception("Unable to find the email");
                        }
                    }
                    else
                    {
                        throw new Exception("Unable to find the email");
                    }
                }
                else
                {
                    throw new Exception("Unable to find the email");
                }
            }
            ModelState.AddModelError("LoginNotFound", "The password reset link has been sent to your mail.");
            return View(inputData);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("LoginNotFound", ex.Message);
            return View(inputData);
        }
    }
    [Route("grp.resetpassword/{MID}")]
    public IActionResult ResetPasswordByUser(string MID)
    {
        ResetPasswordViewModel dto = new ResetPasswordViewModel();

        dto.MID = long.Parse(CommonHelper.DecryptURLHTML(MID));


        //dto.MID = 1234456654;

        return View(dto);
    }
    [HttpPost]
    public async Task<ActionResult> ResetPasswordByUser(ResetPasswordViewModel inputData)
    {
        if (!ModelState.IsValid)
        {
            return View(inputData);
        }

        LoginDetailDTO userDTO = new LoginDetailDTO();
        userDTO.LoginId = Convert.ToInt32(inputData.MID);
        userDTO.Password = CommonHelper.Encrypt(inputData.Password);
        var res = await _accountsApiController.UpdatePassword(userDTO);
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                return RedirectToAction("Login", "Account");
            }
        }
        return View(inputData);
    }


    public IActionResult ChangePassword()
    {
        UserDTO? userSession = (UserDTO?)(JsonConvert.DeserializeObject<UserDTO>(HttpContext.Session.GetString("User")));
        if (userSession == null)
        {
            Message message = new Message();
            message.MessageHeading = "Session expired";
            message.MessageInfo = "You have been locked out of the account";
            message.ButtonText = "Please login again";
            message.Redirect = "/GRP/Account/Login";
            return View("../Pages/Message", message);
        }


        ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel();
        changePasswordViewModel.MID = userSession.UserId;

        return View(changePasswordViewModel);
    }
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel inputDTO)
    {
        UserDTO? userSession = (UserDTO?)(JsonConvert.DeserializeObject<UserDTO?>(HttpContext.Session.GetString("User")));
        if (userSession == null)
        {
            Message message = new Message();
            message.MessageHeading = "Session expired";
            message.MessageInfo = "You have been locked out of the account";
            message.ButtonText = "Please login again";
            message.Redirect = "/GRP/Account/Login";
            return View("../Pages/Message", message);
        }
        if (!ModelState.IsValid)
        {
            return View(inputDTO);
        }
        var res = await _accountsApiController.UpdatePasswordByOldPassword(inputDTO);
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                Message message = new Message();
                message.MessageHeading = "Password Changed";
                message.MessageInfo = "Password has been changed successfully";
                message.ButtonText = "Please login again";
                message.Redirect = "/GRP/Account/Login";
                return View("../Pages/Message", message);
            }
            else
            {
                string? mes = ((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value.ToString();
                ModelState.AddModelError("Error", mes);
                return View(inputDTO);
            }
        }
        else
        {
            ModelState.AddModelError("Error", "");
            return View(inputDTO);
        }
    }
}
