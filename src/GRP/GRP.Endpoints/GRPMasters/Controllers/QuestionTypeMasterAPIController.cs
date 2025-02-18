using AutoMapper;
using GRP.Core.Repository;
using GRP.Endpoints.Masters;
using GRP.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Mvc;

namespace GRP.Endpoints.Masters;

public class QuestionTypeMasterAPIController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<QuestionTypeMasterAPIController> _logger;
    private readonly IMapper _mapper;
    public QuestionTypeMasterAPIController(IUnitOfWork unitOfWork, ILogger<QuestionTypeMasterAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<IActionResult> QuestionTypeList()
    {
        try
        {
            List<QuestionTypeMasterDTO> dto = _mapper.Map<List<QuestionTypeMasterDTO>>(await _unitOfWork.QuestionTypeMaster.GetFilterAll(x => x.IsActive == true));
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(QuestionTypeList)}");
            throw;
        }
    }
}
