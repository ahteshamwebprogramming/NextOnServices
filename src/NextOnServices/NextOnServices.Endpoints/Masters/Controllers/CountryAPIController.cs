using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Core.Helper;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Services.DBContext;
namespace NextOnServices.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class CountryAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CountryAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly DapperDBSetting _dapperDBSetting;
    public CountryAPIController(IUnitOfWork unitOfWork, ILogger<CountryAPIController> logger, IMapper mapper, DapperDBSetting dapperDBSetting)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dapperDBSetting = dapperDBSetting;
    }
    [HttpPost(Name = "GetCountries")]
    public async Task<IActionResult> GetCountries()
    {
        try
        {
            RequestParams requestParams = new RequestParams();
            requestParams.PageSize = 10000;
            requestParams.PageNumber = 1;

            IList<CountryMasterDTO> outputModel = new List<CountryMasterDTO>();
            //outputModel = _mapper.Map<IList<ClientDTO>>(await _unitOfWork.Client.GetAllPagedAsync(10, 0, "where isactive=1", "order by id desc"));
            outputModel = _mapper.Map<IList<CountryMasterDTO>>(await _unitOfWork.CountryMaster.GetAll(requestParams, null, null));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetCountries)}");
            throw;
        }
    }
}
