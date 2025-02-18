using GRP.Infrastructure.Models.Account;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace NextOnServices.WebUI.Areas.GRP.ViewComponents;

[ViewComponent(Name = "GRP/SideBar")]
public class SideBarViewComponent : ViewComponent
{
    public SideBarViewComponent()
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
            userDTO.FirstName = "Not Found";
        }
        return View("SideBar", userDTO);
    }
}
