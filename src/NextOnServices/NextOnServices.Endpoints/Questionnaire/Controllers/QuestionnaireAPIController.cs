using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.Questionnaire;
using NextOnServices.Infrastructure.ViewModels.Questionnaire;
using NextOnServices.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextOnServices.Endpoints.Questionnaire;

[Route("api/[controller]")]
[ApiController]
public class QuestionnaireAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<QuestionnaireAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly DapperDBSetting _dapperDBSetting;

    public QuestionnaireAPIController(IUnitOfWork unitOfWork, ILogger<QuestionnaireAPIController> logger, IMapper mapper, DapperDBSetting dapperDBSetting)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dapperDBSetting = dapperDBSetting;
    }

    [HttpPost]
    public async Task<IActionResult> GetQuestionsList(NextOnServices.Infrastructure.Helper.PagedListParams pagedListParams)
    {
        try
        {
            string orderbyColumn = "Id";
            pagedListParams.searchValue = pagedListParams.searchValue?.ToLower() ?? string.Empty;
            pagedListParams.sortColumnDirection = pagedListParams.sortColumnDirection ?? "desc";
            
            if (!string.IsNullOrEmpty(pagedListParams.sortColumn))
            {
                orderbyColumn = pagedListParams.sortColumn;
            }

            string sWhere = $" WHERE (LOWER(QuestionID) LIKE '%{pagedListParams.searchValue}%' OR LOWER(QuestionLabel) LIKE '%{pagedListParams.searchValue}%') ";
            string sOrderBy = $" ORDER BY {orderbyColumn} {pagedListParams.sortColumnDirection}";
            string sPaging = $" OFFSET {pagedListParams.skip} ROWS FETCH NEXT {pagedListParams.pageSize} ROWS ONLY";
            
            string sQuery = $@"
                SELECT q.Id, q.QuestionID, q.QuestionLabel, q.QuestionType, q.CreationDate,
                       (SELECT COUNT(*) FROM QuestionOptions WHERE QID = q.Id) AS OptionsCount
                FROM QuestionsMaster q
                {sWhere}
                {sOrderBy}
                {sPaging}";

            List<ListQuestionnaire> data = await _unitOfWork.QuestionsMaster.GetTableData<ListQuestionnaire>(sQuery);
            
            // Get question type names
            foreach (var item in data)
            {
                item.QuestionTypeName = GetQuestionTypeName(item.QuestionType ?? 0);
            }

            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retrieving questions list {nameof(GetQuestionsList)}");
            return StatusCode(500, "Error retrieving questions list");
        }
    }

    [HttpPost]
    public async Task<IActionResult> TotalQuestionsFiltered(NextOnServices.Infrastructure.Helper.PagedListParams pagedListParams)
    {
        try
        {
            pagedListParams.searchValue = pagedListParams.searchValue?.ToLower() ?? string.Empty;
            string sWhere = $" WHERE (LOWER(QuestionID) LIKE '%{pagedListParams.searchValue}%' OR LOWER(QuestionLabel) LIKE '%{pagedListParams.searchValue}%') ";
            string sQuery = $"SELECT COUNT(1) FROM QuestionsMaster {sWhere}";

            var data = await _unitOfWork.QuestionsMaster.GetTableData<int?>(sQuery);
            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retrieving total questions count {nameof(TotalQuestionsFiltered)}");
            return StatusCode(500, "Error retrieving total count");
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetQuestionById(QuestionsMasterDTO inputData)
    {
        try
        {
            var question = await _unitOfWork.QuestionsMaster.FindByIdAsync(inputData.Id);
            if (question == null)
            {
                return NotFound("Question not found");
            }

            var questionDTO = _mapper.Map<QuestionsMasterDTO>(question);
            
            // Get options for this question
            string optionsQuery = "SELECT * FROM QuestionOptions WHERE QID = @QID ORDER BY OptionNumber, Id";
            var options = await _unitOfWork.QuestionOption.GetTableData<QuestionOption>(optionsQuery, new { QID = question.Id });
            questionDTO.Options = _mapper.Map<List<QuestionOptionDTO>>(options);

            return Ok(questionDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retrieving question {nameof(GetQuestionById)}");
            return StatusCode(500, "Error retrieving question");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddQuestion(QuestionsMasterDTO inputData)
    {
        try
        {
            if (ModelState.IsValid && inputData != null)
            {
                var question = _mapper.Map<QuestionsMaster>(inputData);
                question.CreationDate = DateTime.Now;
                
                int questionId = await _unitOfWork.QuestionsMaster.AddAsync(question);
                
                if (questionId > 0)
                {
                    // Add options if provided
                    if (inputData.Options != null && inputData.Options.Any())
                    {
                        foreach (var optionDto in inputData.Options)
                        {
                            var option = _mapper.Map<QuestionOption>(optionDto);
                            option.Qid = questionId;
                            option.CrDate = DateTime.Now;
                            await _unitOfWork.QuestionOption.AddAsync(option);
                        }
                    }
                    
                    _unitOfWork.Save();
                    return Ok(new { Id = questionId, Message = "Question added successfully" });
                }
                else
                {
                    return BadRequest("Error in adding question");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in adding question {nameof(AddQuestion)}");
            return StatusCode(500, "Error adding question");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuestion(QuestionsMasterDTO inputData)
    {
        try
        {
            if (ModelState.IsValid && inputData != null && inputData.Id > 0)
            {
                var question = await _unitOfWork.QuestionsMaster.FindByIdAsync(inputData.Id);
                if (question == null)
                {
                    return NotFound("Question not found");
                }

                question.QuestionId = inputData.QuestionId;
                question.QuestionLabel = inputData.QuestionLabel;
                question.QuestionType = inputData.QuestionType;

                bool updated = await _unitOfWork.QuestionsMaster.UpdateAsync(question);
                
                if (updated)
                {
                    // Delete existing options
                    string deleteOptionsQuery = "DELETE FROM QuestionOptions WHERE QID = @QID";
                    await _unitOfWork.QuestionOption.ExecuteQueryAsync(deleteOptionsQuery, new { QID = question.Id });

                    // Add new options
                    if (inputData.Options != null && inputData.Options.Any())
                    {
                        foreach (var optionDto in inputData.Options)
                        {
                            var option = _mapper.Map<QuestionOption>(optionDto);
                            option.Qid = question.Id;
                            option.CrDate = DateTime.Now;
                            await _unitOfWork.QuestionOption.AddAsync(option);
                        }
                    }

                    _unitOfWork.Save();
                    return Ok("Question updated successfully");
                }
                else
                {
                    return BadRequest("Error in updating question");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in updating question {nameof(UpdateQuestion)}");
            return StatusCode(500, "Error updating question");
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteQuestion(QuestionsMasterDTO inputData)
    {
        try
        {
            if (inputData.Id > 0)
            {
                // Delete options first
                string deleteOptionsQuery = "DELETE FROM QuestionOptions WHERE QID = @QID";
                await _unitOfWork.QuestionOption.ExecuteQueryAsync(deleteOptionsQuery, new { QID = inputData.Id });

                // Delete question
                bool deleted = await _unitOfWork.QuestionsMaster.DeleteAsync(inputData.Id);
                
                if (deleted)
                {
                    _unitOfWork.Save();
                    return Ok("Question deleted successfully");
                }
                else
                {
                    return BadRequest("Error in deleting question");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid question ID");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in deleting question {nameof(DeleteQuestion)}");
            return StatusCode(500, "Error deleting question");
        }
    }

    private string GetQuestionTypeName(int questionType)
    {
        return questionType switch
        {
            1 => "Single Choice",
            2 => "Multiple Choice",
            3 => "Text Input",
            _ => "Unknown"
        };
    }
}

