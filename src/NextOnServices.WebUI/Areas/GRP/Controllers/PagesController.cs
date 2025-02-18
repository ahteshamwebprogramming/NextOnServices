using Microsoft.AspNetCore.Mvc;

namespace NextOnServices.WebUI.Areas.GRP.Controllers
{
    [Area("GRP")]
    public class PagesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Message()
        {
            return View();
        }
    }
}
