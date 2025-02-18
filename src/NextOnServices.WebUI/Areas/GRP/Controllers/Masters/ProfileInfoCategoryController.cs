using GRP.Endpoints.Masters;
using GRP.Infrastructure.Helper;
using GRP.Infrastructure.Models.Masters;
using GRP.Infrastructure.ViewModels.Profile;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NextOnServices.WebUI.Areas.GRP.Controllers.Masters;

[Area("GRP")]
public class ProfileInfoCategoryController : Controller
{
    private readonly ILogger<ProfileInfoCategoryController> _logger;
    private readonly ProfileInfoCategoryAPIController _profileInfoCategoryAPIController;
    private readonly ProfileInfoSurveyAPIController _profileInfoSurveyAPIController;
    private readonly QuestionTypeMasterAPIController _questionTypeMasterAPIController;

    public ProfileInfoCategoryController(ILogger<ProfileInfoCategoryController> logger, ProfileInfoCategoryAPIController profileInfoCategoryAPIController, ProfileInfoSurveyAPIController profileInfoSurveyAPIController, QuestionTypeMasterAPIController questionTypeMasterAPIController)
    {
        _logger = logger;
        _profileInfoCategoryAPIController = profileInfoCategoryAPIController;
        _profileInfoSurveyAPIController = profileInfoSurveyAPIController;
        _questionTypeMasterAPIController = questionTypeMasterAPIController;
    }
    public IActionResult ProfileInfoCategory()
    {
        return View();
    }
    public async Task<IActionResult> ListPartialView()
    {
        List<ProfileInfoCategoryDTO>? dto = new List<ProfileInfoCategoryDTO>();
        var res = await _profileInfoCategoryAPIController.ProfileInfoCategoryList();
        if (res != null)
        {

            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto = (List<ProfileInfoCategoryDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;

                foreach (var item in dto)
                {
                    item.EncryptedId = CommonHelper.EncryptURLHTML(item.ProfileInfoCategoryId.ToString());
                }
            }


        }
        return PartialView("_profileInfoCategory/_list", dto);
    }
    public async Task<IActionResult> AddPartialView([FromBody] ProfileInfoCategoryDTO inputDTO)
    {
        ProfileInfoCategoryDTO? dto = new ProfileInfoCategoryDTO();
        if (inputDTO.ProfileInfoCategoryId > 0)
        {
            var res = await _profileInfoCategoryAPIController.ProfileInfoCategoryById(inputDTO);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto = (ProfileInfoCategoryDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                }
            }
        }
        return PartialView("_profileInfoCategory/_add", dto);
    }
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] ProfileInfoCategoryDTO inputDTO)
    {
        try
        {
            inputDTO.IsActive = true;
            var res = await _profileInfoCategoryAPIController.SaveCategory(inputDTO);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    return Ok();
                }
                else
                {
                    throw new Exception(((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value.ToString());
                }
            }
            else
            {
                throw new Exception("Error occurred while creating category");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete([FromBody] ProfileInfoCategoryDTO inputDTO)
    {
        try
        {

            var res = await _profileInfoCategoryAPIController.DeleteCategory(inputDTO);
            return res;
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<IActionResult> DeleteProfileInfoSurvey([FromBody] ProfileInfoSurveyDTO inputDTO)
    {
        try
        {
            if (inputDTO.EncryptedProfileInfoSurveyId == null)
            {
                throw new Exception("Id not found");
            }
            inputDTO.ProfileInfoSurveyId = Convert.ToInt32(CommonHelper.DecryptURLHTML(inputDTO.EncryptedProfileInfoSurveyId));
            var res = await _profileInfoSurveyAPIController.DeleteProfileInfoSurvey(inputDTO);
            return res;
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<IActionResult> ViewSurvey(string enc)
    {
        ProfileSurveysViewModel dto = new ProfileSurveysViewModel();
        dto.EncryptedProfileInfoCategoryId = enc;
        int id = Convert.ToInt32(CommonHelper.DecryptURLHTML(enc));

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

        List<ProfileInfoSurveyViewModel> profileInfoSurveyViewModels1 = new List<ProfileInfoSurveyViewModel>();
        foreach (var item in data)
        {
            item.EncryptedProfileInfoSurveyId = CommonHelper.EncryptURLHTML(item.ProfileInfoSurveyId.ToString());
            ProfileInfoSurveyViewModel profileInfoSurveyViewModels = new ProfileInfoSurveyViewModel();
            profileInfoSurveyViewModels.QuestionLabel = item;
            profileInfoSurveyViewModels.AttributesSelect = QuestionTypeSelectList.Where(x => x.ControlId == item.ProfileInfoSurveyId).ToList();
            profileInfoSurveyViewModels1.Add(profileInfoSurveyViewModels);
        }
        dto.ProfileInfoSurveys = profileInfoSurveyViewModels1;
        return View(dto);
    }
    public async Task<IActionResult> AddSurveyPartialView([FromBody] ProfileInfoSurveyDTO inputDTO)
    {
        //ProfileInfoCategoryDTO? dto = new ProfileInfoCategoryDTO();
        ProfileInfoSurveyViewModel? dto = new ProfileInfoSurveyViewModel();

        var res = await _questionTypeMasterAPIController.QuestionTypeList();
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.QuestionTypeList = (List<QuestionTypeMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        var resCategories = await _profileInfoCategoryAPIController.ProfileInfoCategoryList();
        if (resCategories != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resCategories).StatusCode == 200)
        {
            dto.Categories = (IEnumerable<ProfileInfoCategoryDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resCategories).Value;
        }

        //if (inputDTO.ProfileInfoCategoryId > 0)
        //{
        //    var res = await _profileInfoCategoryAPIController.ProfileInfoCategoryById(inputDTO);
        //    if (res != null)
        //    {
        //        if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        //        {
        //            dto = (ProfileInfoCategoryDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
        //        }
        //    }
        //}
        return PartialView("_profileInfoCategory/_addSurvey", dto);
    }
    public async Task<IActionResult> EditProfileInfoSurveyFromViewSurvey(string encc, string encs = null)
    {
        int ProfileInfoSurveyId = 0;
        if (encs != null)
        {
            ProfileInfoSurveyId = Convert.ToInt32(CommonHelper.DecryptURLHTML(encs));
        }

        ProfileInfoSurveyViewModel? dto = new ProfileInfoSurveyViewModel();
        dto.EncryptedProfileInfoSurveyId = encs == null ? "" : encs;
        dto.EncryptedProfileInfoCategoryId = encc;
        dto.ProfileInfoCategoryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(encc));
        var res = await _questionTypeMasterAPIController.QuestionTypeList();
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.QuestionTypeList = (List<QuestionTypeMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }

        var resProfileInfoSurvey = await _profileInfoSurveyAPIController.ProfileInfoSurveyById(ProfileInfoSurveyId);
        if (resProfileInfoSurvey != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resProfileInfoSurvey).StatusCode == 200)
            {
                dto.QuestionLabel = (ProfileInfoSurveyDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resProfileInfoSurvey).Value;
                if (dto.QuestionLabel != null)
                {
                    dto.QuestionLabel.EncryptedProfileInfoCategoryId = CommonHelper.EncryptURLHTML(dto.QuestionLabel.ProfileInfoCategoryId.ToString());
                }
            }
        }
        var resQuestionTypeSelect = await _profileInfoSurveyAPIController.QuestionTypeSelectList(ProfileInfoSurveyId);
        if (resQuestionTypeSelect != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)resQuestionTypeSelect).StatusCode == 200)
            {
                dto.AttributesSelect = (IEnumerable<QuestionTypeSelectFrameworkDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resQuestionTypeSelect).Value;
            }
        }
        var resCategories = await _profileInfoCategoryAPIController.ProfileInfoCategoryList();
        if (resCategories != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resCategories).StatusCode == 200)
        {
            dto.Categories = (IEnumerable<ProfileInfoCategoryDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resCategories).Value;
        }

        return View(dto);
    }

    public async Task<IActionResult> SaveSurvey([FromBody] ProfileInfoSurveyViewModel inputDTO)
    {
        try
        {
            if (inputDTO == null)
            {
                return BadRequest("Invalid Data");
            }
            ProfileInfoSurveyDTO dto = new ProfileInfoSurveyDTO();
            var res = await _profileInfoCategoryAPIController.SaveProfileInfoSurvey(inputDTO);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    dto = (ProfileInfoSurveyDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    inputDTO.QuestionLabel.ProfileInfoSurveyId = dto.ProfileInfoSurveyId;

                    await _profileInfoCategoryAPIController.SaveQuestionOptionsData(inputDTO);
                }
            }

            return Ok(res);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        //try
        //{
        //    return Ok("");
        //}
        //catch (Exception ex)
        //{
        //    return BadRequest(ex.Message);
        //}
    }
}
