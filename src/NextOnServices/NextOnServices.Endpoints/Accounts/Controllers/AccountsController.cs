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
                await PopulateLegacyProfileFieldsAsync(dto);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}", nameof(GetUserById));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private async Task PopulateLegacyProfileFieldsAsync(UserDTO dto)
        {
            if (dto == null || dto.UserId <= 0)
                return;

            try
            {
                var legacyByStoredProcedure = await _unitOfWork.GenOperations.GetEntityDataSP<LegacyUserProfileProjection>(
                    "usp_getuserprofile",
                    new { ID = dto.UserId.ToString() }
                );

                if (legacyByStoredProcedure != null)
                {
                    dto.Country = ChooseFirstNonEmpty(dto.Country, legacyByStoredProcedure.Country);
                    dto.Rating = ChooseFirstNonEmpty(dto.Rating, legacyByStoredProcedure.Rating);
                    dto.Projects = ChooseFirstNonEmpty(dto.Projects, legacyByStoredProcedure.Projects, legacyByStoredProcedure.Project);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Legacy profile stored procedure unavailable for user {UserId}", dto.UserId);
            }

            // Fallback: read legacy optional columns directly from Users table when available.
            if (string.IsNullOrWhiteSpace(dto.Country) || string.IsNullOrWhiteSpace(dto.Rating) || string.IsNullOrWhiteSpace(dto.Projects))
            {
                try
                {
                    const string fallbackQuery = @"
DECLARE @Sql NVARCHAR(MAX) = N'SELECT ';

SET @Sql += CASE WHEN COL_LENGTH('dbo.Users', 'Country') IS NOT NULL
                 THEN N'CAST([Country] AS NVARCHAR(255)) AS Country'
                 ELSE N'NULL AS Country' END;

SET @Sql += N', ' + CASE WHEN COL_LENGTH('dbo.Users', 'Rating') IS NOT NULL
                         THEN N'CAST([Rating] AS NVARCHAR(255)) AS Rating'
                         ELSE N'NULL AS Rating' END;

SET @Sql += N', ' + CASE WHEN COL_LENGTH('dbo.Users', 'Projects') IS NOT NULL
                         THEN N'CAST([Projects] AS NVARCHAR(4000)) AS Projects'
                         WHEN COL_LENGTH('dbo.Users', 'Project') IS NOT NULL
                         THEN N'CAST([Project] AS NVARCHAR(4000)) AS Projects'
                         ELSE N'NULL AS Projects' END;

SET @Sql += N' FROM [dbo].[Users] WHERE [UserId] = @UserId';

EXEC sp_executesql @Sql, N'@UserId INT', @UserId = @UserId;";

                    var legacyByQuery = await _unitOfWork.GenOperations.GetEntityData<LegacyUserProfileProjection>(
                        fallbackQuery,
                        new { dto.UserId }
                    );

                    if (legacyByQuery != null)
                    {
                        dto.Country = ChooseFirstNonEmpty(dto.Country, legacyByQuery.Country);
                        dto.Rating = ChooseFirstNonEmpty(dto.Rating, legacyByQuery.Rating);
                        dto.Projects = ChooseFirstNonEmpty(dto.Projects, legacyByQuery.Projects, legacyByQuery.Project);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Legacy profile fallback query unavailable for user {UserId}", dto.UserId);
                }
            }
        }

        private static string? ChooseFirstNonEmpty(params string?[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    return value.Trim();
            }

            return null;
        }

        private sealed class LegacyUserProfileProjection
        {
            public string? Country { get; set; }
            public string? Rating { get; set; }
            public string? Project { get; set; }
            public string? Projects { get; set; }
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

        /// <summary>
        /// Change password for a VT user by validating the old password.
        /// This is called from the VT web UI Change Password page.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ChangePasswordVT([FromBody] ChangePasswordVTRequest request)
        {
            try
            {
                if (request == null || request.UserId <= 0 ||
                    string.IsNullOrWhiteSpace(request.OldPassword) ||
                    string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    return BadRequest("Invalid request.");
                }

                var user = await _unitOfWork.User.GetByIdAsync(request.UserId);
                if (user == null)
                    return BadRequest("User not found.");

                var stored = user.Password ?? string.Empty;
                var old = request.OldPassword ?? string.Empty;

                bool oldMatches = false;

                // 1) Direct match (plain text)
                if (!string.IsNullOrEmpty(stored) && stored == old)
                {
                    oldMatches = true;
                }
                else
                {
                    // 2) Try decrypting stored password (in case it was persisted encrypted)
                    try
                    {
                        var decrypted = CommonHelper.Decrypt(stored);
                        if (!string.IsNullOrEmpty(decrypted) && decrypted == old)
                        {
                            oldMatches = true;
                        }
                    }
                    catch
                    {
                        // ignore decrypt failures; will fall back to "incorrect"
                    }
                }

                if (!oldMatches)
                    return BadRequest("Old password is incorrect.");

                // If the existing password appears to be encrypted (decrypt != stored),
                // then encrypt the new password as well; otherwise, store as-is.
                bool storedWasEncrypted = false;
                try
                {
                    var decrypted = CommonHelper.Decrypt(stored);
                    if (!string.IsNullOrEmpty(decrypted) && decrypted != stored)
                        storedWasEncrypted = true;
                }
                catch
                {
                    storedWasEncrypted = false;
                }

                user.Password = storedWasEncrypted
                    ? CommonHelper.Encrypt(request.NewPassword)
                    : request.NewPassword;

                _unitOfWork.User.Update(user);
                _unitOfWork.Save();

                return Ok("Password changed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}", nameof(ChangePasswordVT));
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to change password.");
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

    public class ChangePasswordVTRequest
    {
        public int UserId { get; set; }
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
