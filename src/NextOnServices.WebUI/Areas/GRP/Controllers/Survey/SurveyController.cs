using GRP.Core.Entities;
using GRP.Endpoints.Masters;
using GRP.Endpoints.Survey;
using GRP.Infrastructure.Helper;
using GRP.Infrastructure.Models.Account;
using GRP.Infrastructure.Models.Masters;
using GRP.Infrastructure.Models.Survey;
using GRP.Infrastructure.ViewModels.Survey;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using NextOnServices.WebUI.Areas.GRP.Controllers.Masters;
using System.Security.AccessControl;

namespace NextOnServices.WebUI.Areas.GRP.Controllers.Survey;

[Area("GRP")]
public class SurveyController : Controller
{
    private readonly ILogger<SurveyController> _logger;
    private readonly SurveyAPIController _surveyAPIController;
    private readonly ProfileInfoSurveyAPIController _profileInfoSurveyAPIController;
    public SurveyController(ILogger<SurveyController> logger, SurveyAPIController surveyAPIController, ProfileInfoSurveyAPIController profileInfoSurveyAPIController)
    {
        _logger = logger;
        _surveyAPIController = surveyAPIController;
        _profileInfoSurveyAPIController = profileInfoSurveyAPIController;
    }
    public async Task<IActionResult> List()
    {
        List<SurveyDTO>? dto = new List<SurveyDTO>();
        var res = await _surveyAPIController.SurveyList();
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto = (List<SurveyDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        foreach (var item in dto)
        {
            item.encSurveyId = CommonHelper.EncryptURLHTML(item.SurveyId.ToString());
        }
        return View(dto);
    }

    public async Task<IActionResult> DeleteSurvey([FromBody] SurveyDTO inputDTO)
    {
        try
        {
            if (inputDTO.encSurveyId == null)
            {
                throw new Exception("Data not in valid format");
            }
            inputDTO.SurveyId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.encSurveyId));
            var res = await _surveyAPIController.DeleteSurvey(inputDTO);
            return res;
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

    }

    public async Task<IActionResult> SetCriteria(string enc = null)
    {
        SurveyCriteriaViewModel dto = new SurveyCriteriaViewModel();
        if (enc != null)
        {
            dto.SurveyId = Convert.ToInt32(CommonHelper.DecryptURLHTML(enc));
            var res = await _profileInfoSurveyAPIController.ProfileInfoSurveyList();
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto.profileInfoSurveys = (List<ProfileInfoSurveyDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }
            }
        }
        return View(dto);
    }
    public async Task<IActionResult> SaveCriteria([FromBody] SurveyCriteriaDTO inputDTO)
    {
        try
        {
            if (inputDTO == null)
            {
                throw new Exception("Data is not valid");
            }
            inputDTO.IsActive = true;
            inputDTO.CreatedDate = DateTime.Now;
            var res = await _surveyAPIController.SaveCriteria(inputDTO);
            return res;
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    public async Task<IActionResult> SetCriteriaPartialView([FromBody] SurveyCriteriaViewModel inputData)
    {
        SurveyCriteriaViewModel dto = new SurveyCriteriaViewModel();
        var res = await _profileInfoSurveyAPIController.QuestionTypeSelectList(inputData.ProfileInfoSurveyId ?? default(int));
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.QuestionTypeSelectFrameworkList = (List<QuestionTypeSelectFrameworkDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        var resCriteria = await _surveyAPIController.CriteriaBySurveyIdAndProfileSurveyId(inputData.SurveyId ?? default(int), inputData.ProfileInfoSurveyId ?? default(int));
        if (resCriteria != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resCriteria).StatusCode == 200)
            {
                dto.surveyCriteriaDTO = (SurveyCriteriaDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resCriteria).Value;
            }
        }


        return PartialView("_setCriteria/_setCriteria", dto);
    }
    public async Task<IActionResult> Add(string enc = null)
    {
        SurveyDTO dto = new SurveyDTO();
        if (enc != null)
        {
            int SurveyId = Convert.ToInt32(CommonHelper.DecryptURLHTML(enc));
            var res = await _surveyAPIController.SurveyById(SurveyId);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto = (SurveyDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }
            }
        }
        return View(dto);
    }
    public async Task<IActionResult> SaveSurvey([FromBody] SurveyDTO inputDTO)
    {
        try
        {
            if (inputDTO == null)
            {
                throw new Exception("Data is not valid");
            }

            var res = await _surveyAPIController.SaveSurvey(inputDTO);
            return res;
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return View();
    }

    public async Task<IActionResult> SurveyRedirectDetails([FromBody] SurveyRedirectDetailsDTO inputDTO)
    {
        try
        {
            UserDTO userSession = (UserDTO)(JsonConvert.DeserializeObject<UserDTO>(HttpContext.Session.GetString("User")));
            if (userSession == null)
            {
                return RedirectToAction("/Error/Page");
            }
            IActionResult res;
            do
            {
                inputDTO.RespondentId = CommonHelper.RandomString(32);
                inputDTO.UserId = userSession.UserId;
                inputDTO.Status = "Incomplete";
                inputDTO.IsSent = false;
                inputDTO.StartDate = DateTime.Now;
                inputDTO.CreatedDate = DateTime.Now;
                res = await _surveyAPIController.SurveyRedirectDetails(inputDTO);
            }
            while (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 226);
            return res;

        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Route("GRP/survey.response.{encStatus}/{respondentId}")]
    public async Task<IActionResult> SurveyComplete(string encStatus, string respondentId)
    {
        //UserDTO userSession = (UserDTO)(JsonConvert.DeserializeObject<UserDTO>(HttpContext.Session.GetString("User")));

        IActionResult res = null;
        SurveyRedirectDetailsDTO? dto = new SurveyRedirectDetailsDTO();
        string status = CommonHelper.DecryptURLHTML(encStatus);
        if (status != null && respondentId != null)
        {
            SurveyRedirectDetailsDTO surveyRedirectDetailsDTO = new SurveyRedirectDetailsDTO();
            surveyRedirectDetailsDTO.RespondentId = respondentId;
            surveyRedirectDetailsDTO.Status = status;
            surveyRedirectDetailsDTO.IsSent = true;
            surveyRedirectDetailsDTO.EndDate = DateTime.Now;
            surveyRedirectDetailsDTO.ActualStatus = status;

            res = await _surveyAPIController.UpdateSurveyRedirectDetails(surveyRedirectDetailsDTO);

        }
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto = (SurveyRedirectDetailsDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return View(dto);
    }
}
