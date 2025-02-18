using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.Account;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.ViewModels.Dashboard;
using System.Data;
using System.Linq.Expressions;

namespace NextOnServices.Endpoints.Accounts
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountsController> _logger;
        private readonly IMapper _mapper;
        public AccountsController(IUnitOfWork unitOfWork, ILogger<AccountsController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult GetLoginDetail(UserDTO userDTO)
        {
            try
            {
                Expression<Func<User, bool>> expression = u => u.UserName.ToLower() == userDTO.UserName.ToLower() && (userDTO.Password != null ? u.Password == userDTO.Password : true);
                User user = _unitOfWork.User.Find(expression).FirstOrDefault();
                UserDTO outputDTO = _mapper.Map<UserDTO>(user);

                HttpResponseMessage httpMessage = new HttpResponseMessage();
                if (outputDTO == null)
                {
                    httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                    outputDTO = CommonHelper.GetClassObject(outputDTO);
                }
                else
                    httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive == 1 ? true : false);

                outputDTO.HttpMessage = httpMessage;

                return Ok(outputDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetLoginDetail)}");
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                List<UserDTO> outputDTO = _mapper.Map<List<UserDTO>>(_unitOfWork.User.GetAll(null, null).Result.ToList());
                return Ok(outputDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetUsers)}");
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetManagers(int Status, int ProjectId)
        {
            try
            {
                var parms = new DynamicParameters();
                parms.Add(@"@userid", 0, DbType.Int32);
                parms.Add(@"@stattype", 0, DbType.Int32);
                parms.Add(@"@flagtype", 0, DbType.Int32);
                try
                {
                    //var result = await _unitOfWork.Project.GetStoredProcedure("GetDashboardbyManager", parms);
                    IList<UserDTO> result = _mapper.Map<IList<UserDTO>>(await _unitOfWork.User.GetAll(p => p.IsActive == 1 && p.Status == 1, orderBy: u => u.OrderBy(x => x.UserName)));
                    return StatusCode(StatusCodes.Status200OK, result);
                }
                catch (Exception ex) { return StatusCode(StatusCodes.Status500InternalServerError, ex); }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetManagers)}");
                throw;
            }
        }
    }
}
