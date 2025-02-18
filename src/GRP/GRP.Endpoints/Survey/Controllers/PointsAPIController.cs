using AutoMapper;
using GRP.Core.Repository;
using GRP.Endpoints.Survey;
using GRP.Infrastructure.Models.Survey;
using GRP.Infrastructure.ViewModels.Survey;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GRP.Endpoints.Survey;

[Route("api/[controller]")]
[ApiController]
public class PointsAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PointsAPIController> _logger;
    private readonly IMapper _mapper;
    public PointsAPIController(IUnitOfWork unitOfWork, ILogger<PointsAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> PendingPointsList()
    {
        try
        {
            var res = await _unitOfWork.Survey.GetTableData<PendingPointListViewModel>(@"select (Select SurveyName from Survey s where s.SurveyId=ph.SourceRefId)SurveyName
                                                                                                                ,(Select SurveyIdHost from Survey s where s.SurveyId=ph.SourceRefId)SurveyIdHost
                                                                                                                ,Points
                                                                                                                ,(Select FirstName +' '+LastName from Users u where u.UserId=ph.UserID)UserName,ph.SourceRefId SurveyId
,ph.UserID,ph.PointsHistoryId
                                                                                                                from pointsHistory ph where TransType='Pending' and Source='Survey'");
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PendingPointsList)}");
            throw;
        }
    }
    public async Task<IActionResult> ApproveRejectPoints(PendingPointListViewModel inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                var res = await _unitOfWork.PointsHistory.FindByIdAsync(inputDTO.PointsHistoryId ?? default(int));
                if (inputDTO.Status == "Approved")
                {
                    res.TransType = "Credit";
                }
                else if (inputDTO.Status == "Rejected")
                {
                    res.TransType = "Credit";
                    res.Points = 0;
                }
                await _unitOfWork.PointsHistory.UpdateAsync(res);
                _unitOfWork.Save();
                return Ok(res);
            }
            return BadRequest("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PendingPointsList)}");
            throw;
        }
    }
    public async Task<IActionResult> EarnedPointsHistory(int UserId)
    {
        try
        {
            var res = await _unitOfWork.Survey.GetTableData<EarnedPointHistoryViewModel>(@"select PointsHistoryId ,Points ,TransType ,(Case when Source='Survey' then 'Survey' when Source='ProfileSurvey' then 'Profile' else '' end)Source ,( Case when Source='Survey' then (Select SurveyName +' ('+SurveyIdHost+')' from Survey where SurveyId=SourceRefID) when Source='ProfileSurvey' then  (Select CategoryName from [dbo].[ProfileInfoCategory] where ProfileInfoCategoryId=SourceRefID) else '' end)SourceRef  from PointsHistory where UserId=" + UserId + "");
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PendingPointsList)}");
            throw;
        }
    }
    public async Task<IActionResult> PendingPointsHistory(int UserId)
    {
        try
        {
            var res = await _unitOfWork.Survey.GetTableData<EarnedPointHistoryViewModel>(@"select PointsHistoryId ,Points ,TransType ,(Case when Source='Survey' then 'Survey' when Source='ProfileSurvey' then 'Profile' else '' end)Source ,( Case when Source='Survey' then  (Select SurveyName +' ('+SurveyIdHost+')' from Survey where SurveyId=SourceRefID) when Source='ProfileSurvey' then  (Select CategoryName from [dbo].[ProfileInfoCategory] where ProfileInfoCategoryId=SourceRefID)  else '' end)SourceRef from PointsHistory where UserId=" + UserId + " and TransType='Pending'");
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(PendingPointsList)}");
            throw;
        }
    }
}
