using AutoMapper;
using GRP.Core.Repository;
using GRP.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GRP.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class MastersAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MastersAPIController> _logger;
    private readonly IMapper _mapper;
    public MastersAPIController(IUnitOfWork unitOfWork, ILogger<MastersAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<List<CountryMasterDTO>> CountryList()
    {
        try
        {
            var res = _mapper.Map<List<CountryMasterDTO>>(await _unitOfWork.CountryMaster.FindAllAsync());
            return res;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(CountryList)}");
            throw;
        }
    }
}
