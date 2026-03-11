using Microsoft.AspNetCore.Mvc;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.Account;

namespace NextOnServices.WebUI.VT.Controllers;

[Area("VT")]
public class ToolsController : Controller
{
    [HttpGet]
    [Route("/VT/EncryptDecryptAdminOnlyPage")]
    public IActionResult EncryptDecryptAdminOnlyPage()
    {
        return View(new AdminEncryption());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/VT/EncryptPassword")]
    public IActionResult EncryptPassword(AdminEncryption input)
    {
        input ??= new AdminEncryption();

        if (string.IsNullOrWhiteSpace(input.EncryptInput))
        {
            ModelState.AddModelError(nameof(AdminEncryption.EncryptInput), "Plain text is required.");
            return View("EncryptDecryptAdminOnlyPage", input);
        }

        try
        {
            input.EncryptOutput = CommonHelper.Encrypt(input.EncryptInput);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(nameof(AdminEncryption.EncryptInput), ex.Message);
        }

        return View("EncryptDecryptAdminOnlyPage", input);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/VT/DecryptPassword")]
    public IActionResult DecryptPassword(AdminEncryption input)
    {
        input ??= new AdminEncryption();

        if (string.IsNullOrWhiteSpace(input.DecryptInput))
        {
            ModelState.AddModelError(nameof(AdminEncryption.DecryptInput), "Encrypted text is required.");
            return View("EncryptDecryptAdminOnlyPage", input);
        }

        try
        {
            input.DecryptOutput = CommonHelper.Decrypt(input.DecryptInput);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(nameof(AdminEncryption.DecryptInput), ex.Message);
        }

        return View("EncryptDecryptAdminOnlyPage", input);
    }
}

