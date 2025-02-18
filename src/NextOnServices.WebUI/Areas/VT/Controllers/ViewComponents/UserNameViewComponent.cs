using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NextOnServices.Infrastructure.Models.Account;

namespace NextOnServices.WebUI.VT.Controllers.ViewComponents;

[ViewComponent(Name = "UserName")]
[Area("VT")]
public class UserNameViewComponent : ViewComponent
{
    public UserNameViewComponent()
    {

    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        UserDTO userDTO = new UserDTO();
        try
        {
            userDTO = JsonConvert.DeserializeObject<UserDTO>(HttpContext.Session.GetString("User"));
        }
        catch (Exception ex)
        {
            userDTO.UserName = "Not Found";
        }
        return View("UserName", userDTO);
    }
}
