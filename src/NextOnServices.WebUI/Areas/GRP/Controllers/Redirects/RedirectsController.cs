using GRP.Endpoints.Masters;
using GRP.Endpoints.Survey;
using GRP.Infrastructure.Helper;
using Microsoft.AspNetCore.Mvc;
using Survey.Controllers;

namespace NextOnServices.WebUI.Areas.GRP.Controllers.Redirects;

public class RedirectsController : Controller
{
    private readonly ILogger<RedirectsController> _logger;
    private readonly RedirectsAPIController _redirectsAPIController;

    public RedirectsController(ILogger<RedirectsController> logger, RedirectsAPIController redirectsAPIController)
    {
        _logger = logger;
        _redirectsAPIController = redirectsAPIController;
    }

    public IActionResult Index()
    {
        return View();
    }
    [Route("/GRP/Thanks/{encStatus}/{identifier}")]
    public async Task<IActionResult> BackRedirect(string encStatus, string identifier)
    {
        string actualStatus = CommonHelper.DecryptURLHTML(encStatus);
        var res = await _redirectsAPIController.UpdateRedirect(encStatus, actualStatus, identifier);

        return View("/Areas/GRP/Views/Redirects/BackRedirect.cshtml");
    }

}
