using GRP.Endpoints.Survey;
using GRP.Infrastructure.Models.Account;
using GRP.Infrastructure.Models.Survey;
using GRP.Infrastructure.ViewModels.Survey;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Survey.Controllers;

namespace NextOnServices.WebUI.Areas.GRP.Controllers.Survey;

[Area("GRP")]
public class PointsController : Controller
{
    private readonly ILogger<PointsController> _logger;
    private readonly PointsAPIController _pointsAPIController;
    public PointsController(ILogger<PointsController> logger, PointsAPIController pointsAPIController)
    {
        _logger = logger;
        _pointsAPIController = pointsAPIController;
    }

    public async Task<IActionResult> PointsManager()
    {
        return View();
    }
    public async Task<IActionResult> ListPointsPending_PartialView()
    {
        var res = await _pointsAPIController.PendingPointsList();
        List<PendingPointListViewModel>? dto = new List<PendingPointListViewModel>();
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto = (List<PendingPointListViewModel>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return PartialView("_pointsManager/_list", dto);
    }
    public async Task<IActionResult> ApproveRejectPoints([FromBody] PendingPointListViewModel inputDTO)
    {
        var res = await _pointsAPIController.ApproveRejectPoints(inputDTO);
        return res;
    }

    public async Task<IActionResult> PointsEarned()
    {
        UserDTO userSession = (UserDTO)(JsonConvert.DeserializeObject<UserDTO>(HttpContext.Session.GetString("User")));
        if (userSession == null)
        {
            return RedirectToAction("/Error/Page");
        }

        var res = await _pointsAPIController.EarnedPointsHistory(userSession.UserId);
        List<EarnedPointHistoryViewModel>? dto = new List<EarnedPointHistoryViewModel>();
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto = (List<EarnedPointHistoryViewModel>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return View(dto);
    }
    public async Task<IActionResult> PointsPending()
    {
        UserDTO userSession = (UserDTO)(JsonConvert.DeserializeObject<UserDTO>(HttpContext.Session.GetString("User")));
        if (userSession == null)
        {
            return RedirectToAction("/Error/Page");
        }

        var res = await _pointsAPIController.PendingPointsHistory(userSession.UserId);
        List<EarnedPointHistoryViewModel>? dto = new List<EarnedPointHistoryViewModel>();
        if (res != null)
        {
            if (((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto = (List<EarnedPointHistoryViewModel>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
        }
        return View(dto);
    }

}
