using AutoMapper;
using GRP.Core.Entities;
using GRP.Endpoints.Survey;
using GRP.Infrastructure.Models.Survey;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GRP.Core.Repository;
using NextOnServices.Core.Entities;

namespace Survey.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedirectsAPIController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RedirectsAPIController> _logger;
        private readonly IMapper _mapper;
        public RedirectsAPIController(IUnitOfWork unitOfWork, ILogger<RedirectsAPIController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IActionResult> UpdateRedirect(string encryptedStatus, string actualStatus, string identifier)
        {
            try
            {
                //SurveyRedirectDetailsDTO

                string sQuery = "Select * from [dbo].[SurveyRedirectDetails] where RespondentId=@RespondentId";
                var sParam = new { @RespondentId = identifier };
                var exists = await _unitOfWork.SurveyRedirectDetails.IsExists(sQuery, sParam);
                if (exists)
                {
                    var redirectRecord = await _unitOfWork.SurveyRedirectDetails.GetEntityData<SurveyRedirectDetails>(sQuery, sParam);
                    if (redirectRecord != null)
                    {
                        redirectRecord.EndDate = DateTime.Now;
                        redirectRecord.Status = encryptedStatus;
                        redirectRecord.ActualStatus = actualStatus;

                        var updated = await _unitOfWork.SurveyRedirectDetails.UpdateAsync(redirectRecord);
                        if (updated)
                        {
                            string pQuery = "Select * from PointsHistory where TransType='Credit' and IsActive=1 and Source='Survey' and SourceRefId=@SourceRefId and UserId=@UserId";
                            var pPAram = new { @SourceRefId = redirectRecord.SurveyId, @UserId = redirectRecord.UserId };
                            var points = await _unitOfWork.PointsHistory.GetEntityData<PointsHistory>(pQuery, pPAram);

                            string surveyQuery = "Select * from Survey where SurveyId=@SurveyId";
                            var surveyPAram = new { @SurveyId = redirectRecord.SurveyId };
                            var surveyDetails = await _unitOfWork.Survey.GetEntityData<SurveyDTO>(surveyQuery, surveyPAram);

                            if (points != null)
                            {
                                points.Points = surveyDetails.SurveyPoint ?? 0;
                                await _unitOfWork.PointsHistory.UpdateAsync(points);
                            }
                            return Ok("Redirect record updated successfully");
                        }
                        else
                        {
                            return BadRequest("Unable to update the redirect record");
                        }
                    }
                }
                else
                {
                    return BadRequest("Invalid Identifier");
                }
                return BadRequest("Unable to find the valid url");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving Attendance {nameof(UpdateRedirect)}");
                throw;
            }
        }
    }
}
