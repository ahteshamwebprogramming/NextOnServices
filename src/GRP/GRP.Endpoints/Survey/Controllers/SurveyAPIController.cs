using AutoMapper;
using GRP.Core.Entities;
using GRP.Core.Repository;
using GRP.Infrastructure.Models.Masters;
using GRP.Infrastructure.Models.Survey;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GRP.Endpoints.Survey;

[Route("api/[controller]")]
[ApiController]
public class SurveyAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SurveyAPIController> _logger;
    private readonly IMapper _mapper;
    public SurveyAPIController(IUnitOfWork unitOfWork, ILogger<SurveyAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> SurveyList()
    {
        try
        {
            List<SurveyDTO> dto = await _unitOfWork.Survey.GetTableData<SurveyDTO>(@"Select * from survey where IsActive=1");
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SurveyList)}");
            throw;
        }
    }
    public async Task<IActionResult> SurveyListByUserId(int UserId)
    {
        try
        {
            List<SurveyDTO> dto = await _unitOfWork.Survey.GetTableData<SurveyDTO>(@"Select * from survey where SurveyId not in (select SurveyId from [SurveyRedirectDetails] where userid=" + UserId + ")");
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SurveyList)}");
            throw;
        }
    }
    public async Task<IActionResult> SurveyListByUserIdNew(int UserId)
    {
        try
        {
            string query = $"Select * from survey ssur where SurveyId not in (select SurveyId from [SurveyRedirectDetails] where userid={UserId}) and  SurveyId not in  (select a.SurveyId from ( select SurveyId,ProfileSurveyId,Options,pis.QuestionType,QuestionName  ,(Case  when (QuestionType=4 and (select count(1) from ProfileSurveyResponses psr where psr.UserId={UserId} and psr.QuestionId=sc.ProfileSurveyId and psr.AnswerId between Convert(int,(select top 1 value from string_split(sc.Options,',') order by value asc)) and Convert(int,(select top 1 value from string_split(sc.Options,',') order by value desc)) )>0 ) then 1 when ( QuestionType!=4 and ((select value from string_split((select psr.AnswerId from ProfileSurveyResponses psr where psr.UserId={UserId} and psr.QuestionId=sc.ProfileSurveyId),',')) in (select value from string_split(sc.Options,','))) )then 1 else 0 end  ) Criteria from SurveyCriteria sc  join ProfileInfoSurvey pis on sc.ProfileSurveyId=pis.ProfileInfoSurveyId where sc.IsActive=1 and pis.IsActive=1 and ProfileSurveyId!=0 )a where a.Criteria=0)  and ssur.IsActive=1";
            List<SurveyDTO> dto = await _unitOfWork.Survey.GetTableData<SurveyDTO>(query);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SurveyList)}");
            throw;
        }
    }
    public async Task<IActionResult> ProfileCompletePercent(int UserId)
    {
        try
        {

            //List<float> dto = await _unitOfWork.Survey.GetTableData<float>(@"select  isnull(((select count(distinct(ProfileSurveyCategoryId)) from ProfileSurveyResponses where UserId=" + UserId + ")*100)/(select count(1) from profileInfoCategory where isactive=1),0)");
            List<float> dto = await _unitOfWork.Survey.GetTableData<float>(@"select  isnull(((select count(1) from ProfileInfoSurvey pis join  ProfileSurveyResponses pir on pis.ProfileInfoSurveyId = pir.QuestionId where userid=" + UserId + " and (AnswerId !='0' and ltrim(rtrim(AnswerId))!='') and pis.IsActive=1 and pir.IsActive=1 )*100)/(select count(1) from ProfileInfoSurvey pis where pis.IsActive=1),0)");
            if (dto != null && dto.Count > 0)
            {
                return Ok(dto.FirstOrDefault());
            }
            else { return BadRequest("No Data Found"); }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SurveyList)}");
            throw;
        }
    }
    public async Task<IActionResult> PointsTransaction(int UserId)
    {
        try
        {

            List<PointsTransactionDTO> dto = await _unitOfWork.Survey.GetTableData<PointsTransactionDTO>(@"select * from [dbo].[PointsTransaction] where userid=" + UserId + "");
            if (dto != null && dto.Count > 0)
            {
                return Ok(dto.FirstOrDefault());
            }
            else
            {
                return BadRequest("No Data Found");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SurveyList)}");
            throw;
        }
    }
    public async Task<IActionResult> PointsPending(int UserId)
    {
        try
        {

            List<int> dto = await _unitOfWork.Survey.GetTableData<int>(@"select isnull(Sum(Points),0) PendingPoints from PointsHistory where userId=" + UserId + " and TransType='Pending'");
            if (dto != null && dto.Count > 0)
            {
                return Ok(dto.FirstOrDefault());
            }
            else
            {
                return BadRequest("No Data Found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SurveyList)}");
            throw;
        }
    }
    public async Task<IActionResult> SurveyById(int SurveyId)
    {
        try
        {
            SurveyDTO dto = _mapper.Map<SurveyDTO>(await _unitOfWork.Survey.FindByIdAsync(SurveyId));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SurveyList)}");
            throw;
        }
    }
    public async Task<IActionResult> CriteriaBySurveyIdAndProfileSurveyId(int SurveyId, int ProfileSurveyId)
    {
        try
        {
            SurveyCriteriaDTO dto = _mapper.Map<SurveyCriteriaDTO>(await _unitOfWork.SurveyCriteria.GetFilter(x => x.ProfileSurveyId == ProfileSurveyId && x.SurveyId == SurveyId && x.IsActive == true));
            //SurveyDTO dto = _mapper.Map<SurveyDTO>(await _unitOfWork.Survey.FindByIdAsync(SurveyId));
            if (dto != null)
            {
                return Ok(dto);
            }
            else
            {
                return BadRequest("No Record Found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SurveyList)}");
            throw;
        }
    }

    public async Task<IActionResult> SaveCriteria(SurveyCriteriaDTO inputDTO)
    {
        try
        {
            var res = await _unitOfWork.SurveyCriteria.GetFilterAll(x => x.IsActive == true && x.SurveyId == inputDTO.SurveyId && x.ProfileSurveyId == inputDTO.ProfileSurveyId);
            if (res != null && res.Count > 0)
            {
                inputDTO.ModifiedDate = DateTime.Now;
                inputDTO.SurveyCriteriaId = res.FirstOrDefault().SurveyCriteriaId;
                await _unitOfWork.SurveyCriteria.UpdateAsync(_mapper.Map<SurveyCriteria>(inputDTO));
                _unitOfWork.Save();
                return Ok("Success");
            }
            else
            {
                await _unitOfWork.SurveyCriteria.AddAsync(_mapper.Map<SurveyCriteria>(inputDTO));
                _unitOfWork.Save();
                return Ok("Success");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SurveyList)}");
            throw;
        }
    }

    public async Task<IActionResult> SaveSurvey(SurveyDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.SurveyId > 0)
                {
                    await _unitOfWork.Survey.UpdateAsync(_mapper.Map<GRP.Core.Entities.Survey>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
                else
                {
                    await _unitOfWork.Survey.AddAsync(_mapper.Map<GRP.Core.Entities.Survey>(inputDTO));
                    _unitOfWork.Save();
                    return Ok("Success");
                }
            }
            return BadRequest("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SurveyList)}");
            throw;
        }
    }
    public async Task<IActionResult> SurveyRedirectDetails(SurveyRedirectDetailsDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                var respondentIds = await _unitOfWork.SurveyRedirectDetails.GetFilterAll(x => x.RespondentId == inputDTO.RespondentId);

                if (respondentIds.Any())
                {
                    return StatusCode(StatusCodes.Status226IMUsed);
                }
                else
                {
                    var RespondentSurveys = await _unitOfWork.SurveyRedirectDetails.GetFilterAll(x => x.SurveyId == inputDTO.SurveyId && x.UserId == inputDTO.UserId);

                    if (RespondentSurveys.Any())
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, "Survey already taken");
                    }
                    else
                    {
                        int Id = await _unitOfWork.SurveyRedirectDetails.AddAsync(_mapper.Map<SurveyRedirectDetails>(inputDTO));
                        _unitOfWork.Save();

                        var SurveyDetails = await _unitOfWork.Survey.FindByIdAsync(inputDTO.SurveyId ?? default(int));

                        PointsHistory pointsHistory = new PointsHistory();
                        pointsHistory.Points = 0;
                        pointsHistory.TransType = "Pending";
                        pointsHistory.Source = "Survey";
                        pointsHistory.SourceRefId = inputDTO.SurveyId ?? default(int);
                        pointsHistory.IsActive = true;
                        pointsHistory.UserId = inputDTO.UserId ?? default(int);
                        pointsHistory.CreatedDate = DateTime.Now;
                        await _unitOfWork.PointsHistory.AddAsync(pointsHistory);
                        _unitOfWork.Save();

                        return StatusCode(StatusCodes.Status200OK, inputDTO);
                    }
                }

                //if (inputDTO.SurveyId > 0)
                //{
                //    await _unitOfWork.Survey.UpdateAsync(_mapper.Map<GRP.Core.Entities.Survey>(inputDTO));
                //    _unitOfWork.Save();
                //    return Ok("Success");
                //}
                //else
                //{
                //    await _unitOfWork.Survey.AddAsync(_mapper.Map<GRP.Core.Entities.Survey>(inputDTO));
                //    _unitOfWork.Save();
                //    return Ok("Success");
                //}
            }
            return BadRequest("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SurveyList)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSurvey(SurveyDTO inputDTO)
    {
        try
        {
            var res = await _unitOfWork.Survey.FindByIdAsync(inputDTO.SurveyId);
            res.IsActive = false;
            await _unitOfWork.Survey.UpdateAsync(res);
            _unitOfWork.Save();
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Employee {nameof(DeleteSurvey)}");
            throw;
        }
    }


    public async Task<IActionResult> UpdateSurveyRedirectDetails(SurveyRedirectDetailsDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                var respondent = await _unitOfWork.SurveyRedirectDetails.GetFilter(x => x.RespondentId == inputDTO.RespondentId);
                if (respondent != null)
                {
                    respondent.Status = inputDTO.Status;
                    respondent.IsSent = inputDTO.IsSent;
                    respondent.EndDate = inputDTO.EndDate;
                    respondent.ActualStatus = inputDTO.ActualStatus;

                    await _unitOfWork.SurveyRedirectDetails.UpdateAsync(respondent);
                    _unitOfWork.Save();

                    var pointHistory = (await _unitOfWork.PointsHistory.GetFilterAll(x => x.SourceRefId == respondent.SurveyId && x.UserId == respondent.UserId && x.Source == "Survey")).FirstOrDefault();
                    var survey = await _unitOfWork.Survey.FindByIdAsync(respondent.SurveyId ?? default(int));
                    if (survey != null && pointHistory != null)
                    {
                        if (inputDTO.ActualStatus.ToUpper().Trim() == "COMPLETE" || inputDTO.ActualStatus.ToUpper().Trim() == "COMPLETED")
                        {
                            pointHistory.Points = Convert.ToDouble(survey.SurveyPoint ?? default(double));
                            pointHistory.TransType = "Pending";
                        }
                        else
                        {
                            pointHistory.Points = 0;
                            pointHistory.TransType = "Credit";
                        }
                        await _unitOfWork.PointsHistory.UpdateAsync(pointHistory);
                        _unitOfWork.Save();
                    }

                    return Ok(_mapper.Map<SurveyRedirectDetailsDTO>(respondent));
                }
            }
            return BadRequest("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(SurveyList)}");
            throw;
        }
    }

}
