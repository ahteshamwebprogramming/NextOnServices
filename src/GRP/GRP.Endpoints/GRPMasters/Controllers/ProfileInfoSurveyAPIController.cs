using AutoMapper;
using GRP.Core.Entities;
using GRP.Core.Repository;
using GRP.Infrastructure.Models.Masters;
using GRP.Infrastructure.Models.Survey;
using GRP.Infrastructure.ViewModels.Survey;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace GRP.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class ProfileInfoSurveyAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProfileInfoSurveyAPIController> _logger;
    private readonly IMapper _mapper;
    public ProfileInfoSurveyAPIController(IUnitOfWork unitOfWork, ILogger<ProfileInfoSurveyAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> ProfileInfoSurveyList()
    {
        try
        {
            List<ProfileInfoSurveyDTO> products = _mapper.Map<List<ProfileInfoSurveyDTO>>(await _unitOfWork.ProfileInfoSurvey.GetFilterAll(x => x.IsActive == true));
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ProfileInfoSurveyList)}");
            throw;
        }
    }
    public async Task<IActionResult> ProfileInfoSurveyList(int ProfileInfoCategoryId)
    {
        try
        {
            List<ProfileInfoSurveyDTO> products = _mapper.Map<List<ProfileInfoSurveyDTO>>(await _unitOfWork.ProfileInfoSurvey.GetFilterAll(x => x.IsActive == true && x.ProfileInfoCategoryId == ProfileInfoCategoryId));
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ProfileInfoSurveyList)}");
            throw;
        }
    }
    public async Task<IActionResult> ProfileInfoSurveyListResponses(int ProfileInfoCategoryId, int UserId)
    {
        try
        {
            List<ProfileSurveyResponseDTO> dto = await _unitOfWork.ProfileSurveyResponse.GetTableData<ProfileSurveyResponseDTO>(@"select * from ProfileSurveyResponses where userid=" + UserId + " and QuestionId in (select ProfileInfoSurveyId from ProfileInfoSurvey where ProfileInfoCategoryId=" + ProfileInfoCategoryId + ")");
            if (dto != null && dto.Count > 0)
            {
                return Ok(dto);
            }
            else
            {
                return BadRequest("Error");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ProfileInfoSurveyList)}");
            throw;
        }
    }
    public async Task<IActionResult> ProfileInfoSurveyById(int ProfileInfoSurveyId)
    {
        try
        {
            ProfileInfoSurveyDTO dto = _mapper.Map<ProfileInfoSurveyDTO>(await _unitOfWork.ProfileInfoSurvey.FindByIdAsync(ProfileInfoSurveyId));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ProfileInfoSurveyList)}");
            throw;
        }
    }
    public async Task<IActionResult> QuestionTypeSelectList(int[] ProfileInfoSurveyIds)
    {
        try
        {
            IEnumerable<QuestionTypeSelectFrameworkDTO> dto = _mapper.Map<IEnumerable<QuestionTypeSelectFrameworkDTO>>(await _unitOfWork.QuestionTypeSelectFramework.GetFilterAll(x => ProfileInfoSurveyIds.Contains(x.ControlId ?? default(int))));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ProfileInfoSurveyList)}");
            throw;
        }
    }
    public async Task<IActionResult> QuestionTypeSelectList(int ProfileInfoSurveyId)
    {
        try
        {
            IEnumerable<QuestionTypeSelectFrameworkDTO> dto = _mapper.Map<IEnumerable<QuestionTypeSelectFrameworkDTO>>(await _unitOfWork.QuestionTypeSelectFramework.GetFilterAll(x => x.ControlId == ProfileInfoSurveyId));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ProfileInfoSurveyList)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProfileInfoSurvey(ProfileInfoSurveyDTO inputDTO)
    {
        try
        {
            var res = await _unitOfWork.ProfileInfoSurvey.FindByIdAsync(inputDTO.ProfileInfoSurveyId);
            res.IsActive = false;
            await _unitOfWork.ProfileInfoSurvey.UpdateAsync(res);
            _unitOfWork.Save();
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(DeleteProfileInfoSurvey)}");
            throw;
        }
    }

    public async Task<IActionResult> SaveProfileInforSurveyResponses(ProfileSurveyResponsesViewModel inputDTO, int userId)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.profileSurveyResponses != null)
                {
                    string sQuery = @"DELETE FROM [ProfileSurveyResponses] WHERE ProfileSurveyCategoryId=" + inputDTO.profileSurveyResponses.FirstOrDefault().ProfileSurveyCategoryId + " and UserId=" + userId + "";
                    bool isSuccess = await _unitOfWork.ProfileSurveyResponse.RunSQLCommand(sQuery);
                    if (isSuccess)
                    {
                        sQuery = @"
                        insert into [ProfileSurveyResponses](UserId,QuestionId,AnswerId,IsActive,CreatedDate,CreatedBy,QuestionCode,ProfileSurveyCategoryId) 
                        Values(" + userId + ",@QuestionId,@AnswerId,1,getdate()," + userId + ",@QuestionCode,@ProfileSurveyCategoryId)";
                        isSuccess = await _unitOfWork.ProfileSurveyResponse.ExecuteListData<ProfileSurveyResponseDTO>(_mapper.Map<List<ProfileSurveyResponseDTO>>(inputDTO.profileSurveyResponses), sQuery);
                        if (isSuccess)
                        {
                            if (inputDTO.Completed == false)
                            {
                                return Ok();
                            }

                            var PointsHistoryCount = await _unitOfWork.PointsHistory.GetFilterAll(x => x.UserId == userId && x.SourceRefId == inputDTO.profileSurveyResponses.FirstOrDefault().ProfileSurveyCategoryId);

                            if (PointsHistoryCount.Count == 0)
                            {
                                PointsHistory pointsHistory = new PointsHistory();
                                pointsHistory.Points = 10;
                                pointsHistory.TransType = "Credit";
                                pointsHistory.TransType = "Credit";
                                pointsHistory.Source = "ProfileSurvey";
                                pointsHistory.SourceRefId = inputDTO.profileSurveyResponses.FirstOrDefault().ProfileSurveyCategoryId;
                                pointsHistory.IsActive = true;
                                pointsHistory.CreatedDate = DateTime.Now;
                                pointsHistory.UserId = userId;
                                var PoinHistoryId = await _unitOfWork.PointsHistory.AddAsync(pointsHistory);
                                _unitOfWork.Save();
                            }


                            //PointsTransaction pointsTransaction = await _unitOfWork.PointsTransaction.GetFilter(x => x.UserId == userId);
                            //if (pointsTransaction != null)
                            //{
                            //    pointsTransaction.EarnedPoints += 10;
                            //    pointsTransaction.BalancePoints = (pointsTransaction.EarnedPoints + pointsTransaction.RedeemPoints) ?? default(double);
                            //    await _unitOfWork.PointsTransaction.UpdateAsync(pointsTransaction);
                            //    _unitOfWork.Save();
                            //}
                            //else
                            //{
                            //    pointsTransaction = new PointsTransaction();
                            //    pointsTransaction.EarnedPoints = 10;
                            //    pointsTransaction.RedeemPoints = 0;
                            //    pointsTransaction.BalancePoints = 10;
                            //    pointsTransaction.UserId = userId;
                            //    await _unitOfWork.PointsTransaction.AddAsync(pointsTransaction);
                            //    _unitOfWork.Save();
                            //}
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
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SaveProfileInforSurveyResponses)}");
            throw;
        }
    }
}
