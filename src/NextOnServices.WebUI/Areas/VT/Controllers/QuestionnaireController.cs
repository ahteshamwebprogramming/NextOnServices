using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NextOnServices.Endpoints.Questionnaire;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.Questionnaire;
using NextOnServices.Infrastructure.ViewModels.Questionnaire;
using PagedListParams = NextOnServices.Infrastructure.Helper.PagedListParams;

namespace NextOnServices.WebUI.VT.Controllers;

[Area("VT")]
[Authorize(Roles = "A")]
public class QuestionnaireController : Controller
{
    private readonly ILogger<QuestionnaireController> _logger;
    private readonly QuestionnaireAPIController _questionnaireAPIController;

    public QuestionnaireController(ILogger<QuestionnaireController> logger, QuestionnaireAPIController questionnaireAPIController)
    {
        _logger = logger;
        _questionnaireAPIController = questionnaireAPIController;
    }

    [Route("/VT/Questionnaire/AddQuestion/{eQuestionId=null}")]
    public async Task<IActionResult> AddQuestion(string? eQuestionId = null)
    {
        AddQuestionnaire addQuestionnaire = new AddQuestionnaire();
        QuestionsMasterDTO questionDTO = new QuestionsMasterDTO();

        try
        {
            if (eQuestionId == null || eQuestionId == "null")
            {
                questionDTO.Id = 0;
            }
            else
            {
                questionDTO.Id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eQuestionId));
                var resQuestion = await _questionnaireAPIController.GetQuestionById(questionDTO);
                if (resQuestion != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)resQuestion).StatusCode == 200)
                    {
                        questionDTO = (QuestionsMasterDTO)((Microsoft.AspNetCore.Mvc.ObjectResult)resQuestion).Value;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            questionDTO.Id = 0;
            _logger.LogError(ex, "Error loading question");
        }

        addQuestionnaire.Question = questionDTO;
        return View(addQuestionnaire);
    }

    [HttpPost]
    public async Task<IActionResult> ManageQuestion([FromBody] QuestionsMasterDTO inputDTO)
    {
        try
        {
            if (inputDTO.Id > 0)
            {
                var res = await _questionnaireAPIController.UpdateQuestion(inputDTO);
                if (res != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                    {
                        return Ok("Question Updated Successfully");
                    }
                    else
                    {
                        throw new Exception(((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value?.ToString() ?? "Update failed");
                    }
                }
                else
                {
                    throw new Exception("Unable to update question");
                }
            }
            else
            {
                inputDTO.CreationDate = DateTime.Now;
                var res = await _questionnaireAPIController.AddQuestion(inputDTO);
                if (res != null)
                {
                    if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                    {
                        return Ok("Question Added Successfully");
                    }
                    else
                    {
                        throw new Exception(((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value?.ToString() ?? "Add failed");
                    }
                }
                else
                {
                    throw new Exception("Unable to add question");
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public IActionResult QuestionsList()
    {
        return View();
    }

    [HttpPost]
    public async Task<JsonResult> GetQuestionsList()
    {
        var draw = Request.Form["draw"].FirstOrDefault();
        int drawValue = 0;
        int.TryParse(draw, out drawValue);

        try
        {
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnIndexStr = Request.Form["order[0][column]"].FirstOrDefault();
            int sortColumnIndex = 0;
            int.TryParse(sortColumnIndexStr, out sortColumnIndex);
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            PagedListParams pagedListParams = new PagedListParams();
            pagedListParams.searchValue = searchValue ?? string.Empty;
            pagedListParams.sortColumn = sortColumn;
            pagedListParams.sortColumnDirection = sortColumnDirection;
            pagedListParams.sortColumnIndex = sortColumnIndex;
            pagedListParams.skip = skip;
            pagedListParams.pageSize = pageSize;

            var resQuestions = await _questionnaireAPIController.GetQuestionsList(pagedListParams);
            var resQuestionsCount = await _questionnaireAPIController.TotalQuestionsFiltered(pagedListParams);

            if (resQuestions == null || !(resQuestions is Microsoft.AspNetCore.Mvc.ObjectResult objectResultQuestions) || objectResultQuestions.StatusCode != 200)
            {
                _logger.LogWarning("GetQuestionsList: resQuestions is null or invalid status code");
                return Json(new
                {
                    draw = drawValue,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<ListQuestionnaire>()
                });
            }

            if (resQuestionsCount == null || !(resQuestionsCount is Microsoft.AspNetCore.Mvc.ObjectResult objectResultCount) || objectResultCount.StatusCode != 200)
            {
                _logger.LogWarning("GetQuestionsList: resQuestionsCount is null or invalid status code");
                return Json(new
                {
                    draw = drawValue,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<ListQuestionnaire>()
                });
            }

            IEnumerable<ListQuestionnaire> data = objectResultQuestions.Value as IEnumerable<ListQuestionnaire> ?? new List<ListQuestionnaire>();

            foreach (var item in data)
            {
                if (item != null)
                {
                    item.IdEnc = CommonHelper.EncryptURLHTML(item.Id.ToString());
                }
            }

            List<int?> totalQuestionsCount = objectResultCount.Value as List<int?> ?? new List<int?>();
            int totalCount = totalQuestionsCount.FirstOrDefault() ?? 0;

            return Json(new
            {
                draw = drawValue,
                recordsTotal = totalCount,
                recordsFiltered = totalCount,
                data = data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetQuestionsList");
            return Json(new
            {
                draw = drawValue,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = new List<ListQuestionnaire>()
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteQuestion([FromBody] QuestionsMasterDTO inputDTO)
    {
        try
        {
            var res = await _questionnaireAPIController.DeleteQuestion(inputDTO);
            if (res != null)
            {
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                {
                    return Ok("Question Deleted Successfully");
                }
                else
                {
                    throw new Exception(((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value?.ToString() ?? "Delete failed");
                }
            }
            else
            {
                throw new Exception("Unable to delete question");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}

