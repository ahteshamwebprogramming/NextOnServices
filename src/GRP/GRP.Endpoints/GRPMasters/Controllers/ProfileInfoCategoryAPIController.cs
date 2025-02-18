using AutoMapper;
using GRP.Core.Entities;
using GRP.Core.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using GRP.Infrastructure.Models.Masters;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using GRP.Infrastructure.ViewModels.Profile;

namespace GRP.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class ProfileInfoCategoryAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProfileInfoCategoryAPIController> _logger;
    private readonly IMapper _mapper;
    public ProfileInfoCategoryAPIController(IUnitOfWork unitOfWork, ILogger<ProfileInfoCategoryAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> ProfileInfoCategoryList()
    {
        try
        {
            List<ProfileInfoCategoryDTO> products = _mapper.Map<List<ProfileInfoCategoryDTO>>(await _unitOfWork.ProfileInfoCategory.GetFilterAll(x => x.IsActive == true));

            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ProfileInfoCategoryList)}");
            throw;
        }
    }
    public async Task<IActionResult> ProfileInfoCategoryById(ProfileInfoCategoryDTO inputDTO)
    {
        try
        {
            ProfileInfoCategoryDTO dto = _mapper.Map<ProfileInfoCategoryDTO>(await _unitOfWork.ProfileInfoCategory.FindByIdAsync(inputDTO.ProfileInfoCategoryId));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ProfileInfoCategoryList)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> SaveCategory(ProfileInfoCategoryDTO inputDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.ProfileInfoCategoryId == 0)
                {

                    List<ProfileInfoCategory> ecm = await _unitOfWork.ProfileInfoCategory.GetQueryAll("select 1 a from ProfileInfoCategory where CategoryName='" + inputDTO.CategoryName + "' and isactive=1");
                    if (ecm.Count == 0)
                    {
                        await _unitOfWork.ProfileInfoCategory.AddAsync(_mapper.Map<ProfileInfoCategory>(inputDTO));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Category already exists");
                    }
                }
                else
                {
                    List<ProfileInfoCategory> ecm = await _unitOfWork.ProfileInfoCategory.GetQueryAll("select 1 a from ProfileInfoCategory where CategoryName='" + inputDTO.CategoryName + "'  and ProfileInfoCategoryId!=" + inputDTO.ProfileInfoCategoryId + " and IsActive=1");
                    if (ecm.Count == 0)
                    {
                        ProfileInfoCategory dto = await _unitOfWork.ProfileInfoCategory.FindByIdAsync(inputDTO.ProfileInfoCategoryId);
                        dto.CategoryName = inputDTO.CategoryName;
                        await _unitOfWork.ProfileInfoCategory.UpdateAsync(_mapper.Map<ProfileInfoCategory>(dto));
                        _unitOfWork.Save();
                        return Ok("Success");
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Category already exists");
                    }
                }
            }
            return BadRequest("Invalid Model");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in Save Leave Year {nameof(SaveCategory)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory(ProfileInfoCategoryDTO inputDTO)
    {
        try
        {
            var res = await _unitOfWork.ProfileInfoCategory.FindByIdAsync(inputDTO.ProfileInfoCategoryId);
            res.IsActive = false;
            await _unitOfWork.ProfileInfoCategory.UpdateAsync(res);
            _unitOfWork.Save();
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(DeleteCategory)}");
            throw;
        }
    }
    

    public async Task<IActionResult> SaveProfileInfoSurvey(ProfileInfoSurveyViewModel inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.QuestionLabel != null)
                {
                    ProfileInfoSurveyDTO dto = inputDTO.QuestionLabel;
                    //ProfileInfoSurveyDTO res = await _unitOfWork.ProfileInfoSurvey.GetFilter(x => x.EmployeeId == inputDTO.performanceEmployeeDataDTO.EmployeeId && x.PerformanceSettingId == inputDTO.performanceEmployeeDataDTO.PerformanceSettingId);
                    if (dto.ProfileInfoSurveyId > 0)
                    {
                        var res = await _unitOfWork.ProfileInfoSurvey.FindByIdAsync(dto.ProfileInfoSurveyId);
                        res.SNo = dto.SNo;
                        res.QuestionLabel = dto.QuestionLabel;
                        res.QuestionType = dto.QuestionType;
                        res.ProfileInfoCategoryId = dto.ProfileInfoCategoryId;
                        await _unitOfWork.ProfileInfoSurvey.UpdateAsync(res);
                        _unitOfWork.Save();
                        return Ok(_mapper.Map<ProfileInfoSurveyDTO>(res));
                    }
                    else
                    {
                       dto.IsActive = true;
                        dto.CreatedDate = System.DateTime.Now;
                        int Id = await _unitOfWork.ProfileInfoSurvey.AddAsync(_mapper.Map<ProfileInfoSurvey>(dto));
                        inputDTO.QuestionLabel.ProfileInfoSurveyId = Id;
                        return Ok(dto);
                    }
                }
            }
            return BadRequest("Invalid Data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveProfileInfoSurvey)}");
            throw;
        }
    }

    //[HttpPost]
    //public async Task<IActionResult> SaveProfileInfoSurvey(ProfileInfoSurveyViewModel inputDTO)
    //{
    //    try
    //    {

    //        int Id = await _unitOfWork.ProfileInfoSurvey.AddAsync(_mapper.Map<ProfileInfoSurvey>(inputDTO));


    //        var res = await _unitOfWork.ProfileInfoCategory.FindByIdAsync(inputDTO. .ProfileInfoCategoryId);
    //        res.IsActive = false;
    //        await _unitOfWork.ProfileInfoCategory.UpdateAsync(res);
    //        _unitOfWork.Save();
    //        return Ok(res);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error in retriving Employee {nameof(DeleteCategory)}");
    //        throw;
    //    }

    //}
    public async Task<IActionResult> SaveQuestionOptionsData(ProfileInfoSurveyViewModel inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.QuestionLabel != null && inputDTO.AttributesSelect != null)
                {
                    string sQuery = @"DELETE FROM [QuestionTypeSelectFramework] WHERE ControlId=" + inputDTO.QuestionLabel.ProfileInfoSurveyId + "";
                    bool isSuccess = await _unitOfWork.QuestionTypeSelectFramework.RunSQLCommand(sQuery);
                    if (isSuccess)
                    {
                        sQuery = @"
                        insert into [QuestionTypeSelectFramework] (ControlId,Label,Value) Values(" + inputDTO.QuestionLabel.ProfileInfoSurveyId + ",@Label,@Value)";
                        isSuccess = await _unitOfWork.QuestionTypeSelectFramework.ExecuteListData<QuestionTypeSelectFramework>(_mapper.Map<List<QuestionTypeSelectFramework>>(inputDTO.AttributesSelect), sQuery);
                        if (isSuccess)
                        {
                            return Ok("Success");
                        }
                        else
                        {
                            return BadRequest("Error While saving data");
                        }
                    }
                }
            }
            return BadRequest("Invalid Data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveQuestionOptionsData)}");
            throw;
        }
    }
}
