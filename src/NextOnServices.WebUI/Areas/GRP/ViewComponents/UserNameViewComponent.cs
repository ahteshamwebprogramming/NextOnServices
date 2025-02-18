using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using GRP.Infrastructure.Models.Account;

namespace NextOnServices.WebUI.Areas.GRP.ViewComponents;


[ViewComponent(Name = "GRP/UserName")]

public class UserNameViewComponent : ViewComponent
{
    public UserNameViewComponent()
    {

    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            UserDTO? userDTO = new UserDTO();
            string? strUser = HttpContext.Session.GetString("User");
            if (String.IsNullOrEmpty(strUser))
            {
                throw new Exception();
            }
            else
            {
                userDTO = JsonConvert.DeserializeObject<UserDTO>(strUser);
                if (userDTO == null)
                {
                    throw new Exception();
                }
                userDTO.IsActive = true;
                return View("UserName", userDTO);
            }
        }
        catch (Exception ex)
        {
            UserDTO? userDTO = new UserDTO();
            userDTO.IsActive = false;
            return View("UserName", userDTO);
        }
    }
}
