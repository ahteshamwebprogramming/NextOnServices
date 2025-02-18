using AutoMapper;
using GRP.Core.Entities;
using GRP.Core.Repository;
using GRP.Infrastructure.Helper;
using GRP.Infrastructure.Models.Account;
using GRP.Infrastructure.ViewModels.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;

namespace GRP.Endpoints.Accounts;

[Route("api/[controller]")]
[ApiController]
public class AccountsAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AccountsAPIController> _logger;
    private readonly IMapper _mapper;
    public AccountsAPIController(IUnitOfWork unitOfWork, ILogger<AccountsAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    [HttpPost]
    public IActionResult GetLoginDetail(LoginDetailDTO loginDTO)
    {
        try
        {
            Expression<Func<LoginDetail, bool>> expression = u => u.Username.ToLower() == loginDTO.Username.ToLower() && (loginDTO.Password != null ? u.Password == loginDTO.EncryptedPassword : true);
            LoginDetail loginDetail = _unitOfWork.LoginDetail.Find(expression).FirstOrDefault();
            LoginDetailDTO outputDTO = _mapper.Map<LoginDetailDTO>(loginDetail);

            HttpResponseMessage httpMessage = new HttpResponseMessage();
            if (outputDTO == null)
            {
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                outputDTO = CommonHelper.GetClassObject(outputDTO);
            }
            else
                httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);

            outputDTO.HttpMessage = httpMessage;
            return Ok(outputDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetLoginDetail)}");
            throw;
        }
    }
    public async Task<UserDTO> GetUserById(int userId)
    {
        try
        {
            UserDTO dto = _mapper.Map<UserDTO>(await _unitOfWork.User.GetByIdAsync(userId));
            if (dto != null && dto.RoleId != null)
            {
                var roleNameRes = await _unitOfWork.Survey.GetTableData<string>("Select RoleName from Roles where RoleId = " + dto.RoleId + "");
                if (roleNameRes != null && roleNameRes.Count > 0)
                {
                    dto.RoleName = roleNameRes.FirstOrDefault();
                }
            }
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetLoginDetail)}");
            throw;
        }
    }

    public async Task<IActionResult> DeleteUser(UserDTO inputDTO)
    {
        try
        {
            if (inputDTO.UserId == 0)
            {
                User user = await _unitOfWork.User.GetByIdAsync(inputDTO.UserId);
                user.IsActive = false;
                _unitOfWork.User.Update(user);
                _unitOfWork.Save();
                return Ok("Success");
            }
            return BadRequest("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetLoginDetail)}");
            throw;
        }
    }

    public async Task<IActionResult> RegisterUser(UserDTO inputDTO, string password)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (inputDTO.UserId == 0)
                {
                    if (!_unitOfWork.User.Exists(x => x.IsActive == true && x.EmailId == inputDTO.EmailId) || !_unitOfWork.LoginDetail.Exists(x => x.IsActive == true && x.Username == inputDTO.EmailId))
                    {
                        var user = _mapper.Map<User>(inputDTO);
                        var user1 = _unitOfWork.User.Create(user);
                        int id = user1.UserId;
                        if (id > 0)
                        {
                            LoginDetail loginDetail = new LoginDetail();
                            loginDetail.UserId = id;
                            loginDetail.Username = inputDTO.EmailId;
                            loginDetail.Password = CommonHelper.Encrypt(password);
                            loginDetail.IsActive = true;
                            loginDetail.CreatedDate = DateTime.Now;
                            loginDetail = _unitOfWork.LoginDetail.Create(loginDetail);
                            if (loginDetail.LoginId > 0)
                            {
                                return Ok("Success");
                            }
                            else
                            {
                                inputDTO.UserId = id;
                                await DeleteUser(inputDTO);
                                return BadRequest("Unable to create login credentials");
                            }
                        }
                        else
                        {
                            return BadRequest("Unable to create User");
                        }
                    }
                    else
                    {
                        return BadRequest("Email Id already exists");
                    }
                }
            }
            return BadRequest("Error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetLoginDetail)}");
            throw;
        }
    }
    [HttpPost]
    public IActionResult CheckEmailExists(LoginDetailDTO inputDTO)
    {
        try
        {
            Expression<Func<LoginDetail, bool>> expression = u => u.Username == inputDTO.Username && u.IsActive == true;
            bool isExists = _unitOfWork.LoginDetail.Exists(expression);

            if (isExists)
            {
                LoginDetailDTO? dto = _mapper.Map<LoginDetailDTO>(_unitOfWork.LoginDetail.GetWithRawSql("select * from [logindetails] where username='" + inputDTO.Username + "'").FirstOrDefault());
                return Ok(dto);
            }
            else
            {
                return null;
            }

            return Ok(isExists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(CheckEmailExists)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> UpdatePassword(LoginDetailDTO inputDTO)
    {
        try
        {
            LoginDetail dto = await _unitOfWork.LoginDetail.GetByIdAsync(inputDTO.LoginId);
            dto.Password = inputDTO.Password;

            _unitOfWork.LoginDetail.UpdateDbEntry(dto, "Password");
            _unitOfWork.Save();

            //SqlParameter[] parm = new SqlParameter[2];
            //parm[0] = new SqlParameter("@password", inputDTO.Password);
            //parm[1] = new SqlParameter("@ID", inputDTO.ID);

            //_unitOfWork.TBL_UserProfile.GetWithRawSql("update [TBL_UserProfile] set Password=@password where Id=@ID", parm);

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(CheckEmailExists)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> UpdatePasswordByOldPassword(ChangePasswordViewModel inputDTO)
    {
        try
        {


            LoginDetail dto = await _unitOfWork.LoginDetail.GetByIdAsync(inputDTO.MID);

            string password = CommonHelper.Decrypt(dto.Password);
            if (password == inputDTO.OldPassword)
            {
                dto.Password = CommonHelper.Encrypt(inputDTO.Password);
                dto.PasswordChanged = true;
                _unitOfWork.LoginDetail.UpdateDbEntry(dto, "Password");
                _unitOfWork.Save();
            }
            else
            {
                return BadRequest("Old Password is incorrect");
            }
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(CheckEmailExists)}");
            throw;
        }
    }


}
