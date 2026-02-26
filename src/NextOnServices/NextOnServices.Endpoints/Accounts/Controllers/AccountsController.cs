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
                Expression<Func<User, bool>> expression = u => u.UserCode.ToLower() == userDTO.UserName.ToLower() && (userDTO.Password != null ? u.Password == userDTO.Password : true);
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

        /// <summary>
        /// Create a new VT user (admin/user). Used by VT AddUser page.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.UserCode) || string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest("Username and password are required.");

                string userCodeLower = request.UserCode.Trim().ToLower();
                bool userCodeExists = _unitOfWork.User.Exists(u => u.UserCode != null && u.UserCode.ToLower() == userCodeLower && (u.IsActive == 1));
                if (userCodeExists)
                    return BadRequest("Username already registered.");

                string? emailLower = string.IsNullOrWhiteSpace(request.EmailId) ? null : request.EmailId.Trim().ToLower();
                bool emailExists = emailLower != null && _unitOfWork.User.Exists(u => u.EmailId != null && u.EmailId.ToLower() == emailLower && (u.IsActive == 1));
                if (emailExists)
                    return BadRequest("Email address already registered.");

                var user = new User
                {
                    UserCode = request.UserCode.Trim(),
                    UserName = request.UserName?.Trim(),
                    EmailId = request.EmailId?.Trim(),
                    Password = CommonHelper.Encrypt(request.Password),
                    ContactNumber = request.ContactNumber?.Trim(),
                    Address = request.Address?.Trim(),
                    UserType = request.UserType ?? "U",
                    Status = 1,
                    IsActive = 1,
                    CreatedDate = DateTime.Now
                };

                _unitOfWork.User.AddAsync(user);
                _unitOfWork.Save();

                return Ok("User created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}", nameof(RegisterUser));
                return StatusCode(StatusCodes.Status500InternalServerError, "User creation failed.");
            }
        }

        /// <summary>
        /// Set user active/inactive. Used by VT Users Details page.
        /// </summary>
        [HttpPost]
        public IActionResult SetUserStatus([FromBody] SetUserStatusRequest request)
        {
            try
            {
                if (request == null || request.UserId <= 0)
                    return BadRequest("Invalid user.");
                var user = _unitOfWork.User.Find(u => u.UserId == request.UserId).FirstOrDefault();
                if (user == null)
                    return BadRequest("User not found.");
                user.IsActive = request.IsActive;
                _unitOfWork.User.Update(user);
                _unitOfWork.Save();
                return Ok(1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}", nameof(SetUserStatus));
                return StatusCode(StatusCodes.Status500InternalServerError, 0);
            }
        }

        /// <summary>Get a single user by id. Used by VT Edit User.</summary>
        [HttpPost]
        public async Task<IActionResult> GetUserById([FromBody] GetUserByIdRequest request)
        {
            try
            {
                if (request == null || request.UserId <= 0)
                    return BadRequest("Invalid user.");
                var user = await _unitOfWork.User.GetByIdAsync(request.UserId);
                if (user == null)
                    return NotFound();
                var dto = _mapper.Map<UserDTO>(user);
                dto.Password = null; // never return password to client
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}", nameof(GetUserById));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update existing user. Password optional (leave blank to keep current).</summary>
        [HttpPost]
        public IActionResult UpdateUser([FromBody] UpdateUserRequest request)
        {
            try
            {
                if (request == null || request.UserId <= 0)
                    return BadRequest("Invalid user.");
                var user = _unitOfWork.User.Find(u => u.UserId == request.UserId).FirstOrDefault();
                if (user == null)
                    return BadRequest("User not found.");
                string? emailLower = string.IsNullOrWhiteSpace(request.EmailId) ? null : request.EmailId.Trim().ToLower();
                if (emailLower != null)
                {
                    bool emailTaken = _unitOfWork.User.Exists(u => u.UserId != request.UserId && u.EmailId != null && u.EmailId.ToLower() == emailLower && (u.IsActive == 1));
                    if (emailTaken)
                        return BadRequest("Email address already in use by another user.");
                }
                user.UserName = request.UserName?.Trim();
                user.EmailId = request.EmailId?.Trim();
                user.ContactNumber = request.ContactNumber?.Trim();
                user.Address = request.Address?.Trim();
                user.UserType = string.IsNullOrWhiteSpace(request.UserType) ? user.UserType : request.UserType.Trim();
                if (!string.IsNullOrWhiteSpace(request.Password))
                    user.Password = CommonHelper.Encrypt(request.Password);
                _unitOfWork.User.Update(user);
                _unitOfWork.Save();
                return Ok("User updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}", nameof(UpdateUser));
                return StatusCode(StatusCodes.Status500InternalServerError, "Update failed.");
            }
        }
    }

    public class GetUserByIdRequest
    {
        public int UserId { get; set; }
    }

    public class UpdateUserRequest
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? EmailId { get; set; }
        public string? Password { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public string? UserType { get; set; }
    }

    public class SetUserStatusRequest
    {
        public int UserId { get; set; }
        public int IsActive { get; set; }
    }

    public class RegisterUserRequest
    {
        public string UserCode { get; set; } = string.Empty;  // login username
        public string? UserName { get; set; }                 // display name
        public string? EmailId { get; set; }
        public string Password { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public string? UserType { get; set; }                 // "A" = Admin, "U" = User
    }
}
