using GRP.Endpoints.Masters;
using GRP.Endpoints.Survey;
using GRP.Infrastructure.Helper;
using GRP.Infrastructure.Models.Account;
using GRP.Infrastructure.Models.Masters;
using GRP.Infrastructure.Models.Survey;
using GRP.Infrastructure.ViewModels.Pages;
using GRP.Infrastructure.ViewModels.Profile;
using GRP.Infrastructure.ViewModels.Survey;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace NextOnServices.WebUI.Areas.GRP.Controllers.Survey;

[Area("GRP")]
public class ProfileSurveyController : Controller
{
    private readonly ILogger<ProfileSurveyController> _logger;
    private readonly ProfileInfoSurveyAPIController _profileInfoSurveyAPIController;
    public ProfileSurveyController(ILogger<ProfileSurveyController> logger, ProfileInfoSurveyAPIController profileInfoSurveyAPIController)
    {
        _logger = logger;
        _profileInfoSurveyAPIController = profileInfoSurveyAPIController;
    }
    public IActionResult Index()
    {
        return View();
    }
    public async Task<IActionResult> AnswerSurvey(string enc)
    {
        ProfileSurveysViewModel dto = new ProfileSurveysViewModel();
        int id = Convert.ToInt32(CommonHelper.DecryptURLHTML(enc));
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

        return View(dto);
    }

   

    [HttpPost]
    public async Task<IActionResult> SaveProfileSurveyResponses([FromBody] ProfileSurveyResponsesViewModel inputDTO)
    {
        try
        {
            if (inputDTO == null)
            {
                return BadRequest("Invalid Data");
            }
            UserDTO userSession = (UserDTO)(JsonConvert.DeserializeObject<UserDTO>(HttpContext.Session.GetString("User")));
            var res = await _profileInfoSurveyAPIController.SaveProfileInforSurveyResponses(inputDTO, userSession.UserId);
            return res;
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
