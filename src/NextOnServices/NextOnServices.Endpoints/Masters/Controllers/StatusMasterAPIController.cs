using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Core.Helper;
using NextOnServices.Infrastructure.Models.Masters;

namespace NextOnServices.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class StatusMasterAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StatusMasterAPIController> _logger;
    private readonly IMapper _mapper;
    public StatusMasterAPIController(IUnitOfWork unitOfWork, ILogger<StatusMasterAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost(Name = "GetStatusMaster")]
    public async Task<ActionResult> GetStatusMaster()
    {
        try
        {
            IList<StatusMasterDTO> outputModel = new List<StatusMasterDTO>();
            outputModel = _mapper.Map<IList<StatusMasterDTO>>(await _unitOfWork.StatusMaster.GetAll(null,null));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetStatusMaster)}");
            throw;
        }
    }

}
