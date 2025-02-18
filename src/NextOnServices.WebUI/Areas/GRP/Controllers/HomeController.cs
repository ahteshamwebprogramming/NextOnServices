using GRP.Endpoints.Masters;
using GRP.Endpoints.Survey;
using GRP.Infrastructure.Helper;
using GRP.Infrastructure.Models.Account;
using GRP.Infrastructure.Models.Masters;
using GRP.Infrastructure.Models.Survey;
using GRP.Infrastructure.ViewModels.Dashboard;
using GRP.Infrastructure.ViewModels.Home;
using GRP.Infrastructure.ViewModels.Pages;
using GRP.Infrastructure.ViewModels.Profile;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;


namespace NextOnServices.WebUI.Areas.GRP.Controllers;

[Area("GRP")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ProfileInfoCategoryAPIController _profileInfoCategoryAPIController;
    private readonly ProfileInfoSurveyAPIController _profileInfoSurveyAPIController;
    private readonly SurveyAPIController _surveyAPIController;
    public HomeController(ILogger<HomeController> logger, ProfileInfoCategoryAPIController profileInfoCategoryAPIController, ProfileInfoSurveyAPIController profileInfoSurveyAPIController, SurveyAPIController surveyAPIController)
    {
        _logger = logger;
        _profileInfoCategoryAPIController = profileInfoCategoryAPIController;
        _profileInfoSurveyAPIController = profileInfoSurveyAPIController;
        _surveyAPIController = surveyAPIController;
    }

    public async Task<IActionResult> Dashboard()
    {
        UserDTO userSession = (UserDTO)(JsonConvert.DeserializeObject<UserDTO>(HttpContext.Session.GetString("User")));
        if (userSession == null)
        {
            return RedirectToAction("/Error/Page");
        }
        DashboardViewModel dto = new DashboardViewModel();
        var resSurveyList = await _surveyAPIController.SurveyListByUserIdNew(userSession.UserId);
        if (resSurveyList != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resSurveyList).StatusCode == 200)
            {
                dto.surveys = (List<SurveyDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resSurveyList).Value;
                foreach (var item in dto.surveys)
                {
                    item.encSurveyId = CommonHelper.EncryptURLHTML(item.SurveyId.ToString());
                }
            }
        }
        var resProfileCompletePercent = await _surveyAPIController.ProfileCompletePercent(userSession.UserId);
        if (resProfileCompletePercent != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resProfileCompletePercent).StatusCode == 200)
            {
                dto.ProfileCompletePercent = (float?)((Microsoft.AspNetCore.Mvc.ObjectResult)resProfileCompletePercent).Value;
            }
        }
        var resPointsTransaction = await _surveyAPIController.PointsTransaction(userSession.UserId);
        if (resPointsTransaction != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resPointsTransaction).StatusCode == 200)
            {
                dto.PointsTransaction = (PointsTransactionDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resPointsTransaction).Value;
            }
        }
        var resPointsPending = await _surveyAPIController.PointsPending(userSession.UserId);
        if (resPointsPending != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resPointsPending).StatusCode == 200)
            {
                dto.PointsPending = (int?)((Microsoft.AspNetCore.Mvc.ObjectResult)resPointsPending).Value;
            }
        }

        string City = "Mumbai";

        string appId = "cc338e6480566a0f6c5ffaa020bafcfc";

        //API path with CITY parameter and other parameters.  
        string url = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&units=metric&cnt=1&APPID={1}", City, appId);

        //using (WebClient client = new WebClient())
        //{
        //    string json = client.DownloadString(url);
        //}

        return View(dto);
    }
    public async Task<IActionResult> Dashboard_Old()
    {
        return View();
    }
    public async Task<IActionResult> Home()
    {
        return View();
    }
        
    public async Task<IActionResult> AnswerSurveyCW()
    {
        var res = await _profileInfoCategoryAPIController.ProfileInfoCategoryList();
        if (res == null)
        {
            return null;
        }
        if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode != 200)
        {
            return null;
        }

        IEnumerable<ProfileInfoCategoryDTO> data = ((IEnumerable<ProfileInfoCategoryDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value);   //_context.Set<Employees>().AsQueryable();

        foreach (var item in data)
        {
            item.EncryptedId = CommonHelper.EncryptURLHTML(item.ProfileInfoCategoryId.ToString());
        }
        return View(data);
    }
    public async Task<IActionResult> AnswerSurveyPartialView(int CategoryId)
    {
        ProfileSurveysViewModel dto = new ProfileSurveysViewModel();
        //int id = Convert.ToInt32(CommonHelper.DecryptURLHTML(enc));
        int id = CategoryId;
        UserDTO userSession = (UserDTO)(JsonConvert.DeserializeObject<UserDTO>(HttpContext.Session.GetString("User")));
        if (userSession == null)
        {
            Message message = new Message();
            message.MessageHeading = "Session Timeout";
            message.MessageInfo = "You have been logged out. Please login again.";
            message.ButtonText = "Go to login page";
            message.Redirect = "/GRP/Account/Login";
            return View("../Page/Message", message);
        }
        List<QuestionTypeSelectFrameworkDTO> QuestionTypeSelectList = new List<QuestionTypeSelectFrameworkDTO>();

        var res = await _profileInfoSurveyAPIController.ProfileInfoSurveyList(id);
        if (res == null)
        {
            return null;
        }
        if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode != 200)
        {
            return null;
        }
        IEnumerable<ProfileInfoSurveyDTO> data = ((IEnumerable<ProfileInfoSurveyDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value);   //_context.Set<Employees>().AsQueryable();
        int[] ProfileInfoSurveyIds = data.Select(x => x.ProfileInfoSurveyId).ToArray();
        var resQuestionTypeSelectList = await _profileInfoSurveyAPIController.QuestionTypeSelectList(ProfileInfoSurveyIds);
        if (resQuestionTypeSelectList != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resQuestionTypeSelectList).StatusCode == 200)
            {
                QuestionTypeSelectList = ((List<QuestionTypeSelectFrameworkDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)resQuestionTypeSelectList).Value);
            }
        }
        List<ProfileSurveyResponseDTO>? profileSurveyResponseDTOs = new List<ProfileSurveyResponseDTO>();
        var resProfileInfoSurveyListResponses = await _profileInfoSurveyAPIController.ProfileInfoSurveyListResponses(id, userSession.UserId);
        if (resProfileInfoSurveyListResponses != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resProfileInfoSurveyListResponses).StatusCode == 200)
            {
                profileSurveyResponseDTOs = ((List<ProfileSurveyResponseDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)resProfileInfoSurveyListResponses).Value);
            }
        }

        List<ProfileInfoSurveyViewModel> profileInfoSurveyViewModels1 = new List<ProfileInfoSurveyViewModel>();
        foreach (var item in data)
        {
            ProfileInfoSurveyViewModel profileInfoSurveyViewModels = new ProfileInfoSurveyViewModel();
            profileInfoSurveyViewModels.QuestionLabel = item;
            profileInfoSurveyViewModels.AttributesSelect = QuestionTypeSelectList.Where(x => x.ControlId == item.ProfileInfoSurveyId).ToList();
            var respondentResponse = profileSurveyResponseDTOs.Where(x => x.QuestionId == item.ProfileInfoSurveyId);
            profileInfoSurveyViewModels.RespondentResponse = respondentResponse.Count() > 0 ? respondentResponse.FirstOrDefault().AnswerId : "";
            profileInfoSurveyViewModels1.Add(profileInfoSurveyViewModels);
        }
        dto.ProfileInfoSurveys = profileInfoSurveyViewModels1;
        dto.ProfileInfoCategoryId = id;
        return PartialView("_answerSurveyCW/_answerSurveyCW", dto);
        return View(dto);
    }

    public async Task<IActionResult> AnswerSurveyCats()
    {
        var res = await _profileInfoCategoryAPIController.ProfileInfoCategoryList();
        if (res == null)
        {
            return null;
        }
        if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode != 200)
        {
            return null;
        }

        IEnumerable<ProfileInfoCategoryDTO> data = ((IEnumerable<ProfileInfoCategoryDTO>)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value);   //_context.Set<Employees>().AsQueryable();

        foreach (var item in data)
        {
            item.EncryptedId = CommonHelper.EncryptURLHTML(item.ProfileInfoCategoryId.ToString());
        }
        return View(data);
    }


    public async Task<IActionResult> Index()
    {
        return View();
    }
    public async Task<IActionResult> Index1()
    {
        return View();
    }
    public async Task<IActionResult> AboutUs()
    {
        return View();
    }
    public async Task<IActionResult> HowItWorks()
    {
        return View();
    }
    public async Task<IActionResult> ContactUs()
    {
        return View();
    }
    public async Task<IActionResult> SubmitContactUsDetails([FromBody] ContactUsViewModel inputDTO)
    {
        string subject = "Query generated from Contact Us";
        string body = @"<p>Name : " + inputDTO.Name + "</p><p>Contact No : " + inputDTO.ContactNo + "</p><p>Email Id : <a href=\"" + inputDTO.EmailId + "\">" + inputDTO.EmailId + "</a></p><p>Message : " + inputDTO.Message + "&nbsp;</p>";
        string mailTo = "community@globalresearchpanels.com";
        MailHelper.SendEmail(subject, body, mailTo);

        subject = "Acknowledgement from GRP";
        body = "<p>We have received your query.</p><p>Our team will reach you with in 24 hours.</p><p></p><p>Thanks and Regards&nbsp;</p><p>GRP Team</p>";
        mailTo = inputDTO.EmailId;
        MailHelper.SendEmail(subject, body, mailTo);

        return Ok("");
    }
}
