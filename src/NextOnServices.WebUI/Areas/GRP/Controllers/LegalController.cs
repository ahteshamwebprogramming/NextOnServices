using Microsoft.AspNetCore.Mvc;

namespace NextOnServices.WebUI.Areas.GRP.Controllers
{
    [Area("GRP")]
    public class LegalController : Controller
    {
        public IActionResult TermsOfServices()
        {
            return View();
        }
        public IActionResult PrivacyPolicy()
        {
            return View();
        }
    }
}
