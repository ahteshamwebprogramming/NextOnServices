using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Infrastructure.Models.Settings;
using NextOnServices.Infrastructure.ViewModels.Settings;
using NextOnServices.WebUI.VT.Services;

namespace NextOnServices.WebUI.VT.Controllers;

[Area("VT")]
[Authorize(Roles = "A")]
public class SettingsController : Controller
{
    private readonly ILogger<SettingsController> _logger;
    private readonly IHashingSettingsService _hashingSettingsService;

    public SettingsController(
        ILogger<SettingsController> logger,
        IHashingSettingsService hashingSettingsService)
    {
        _logger = logger;
        _hashingSettingsService = hashingSettingsService;
    }

    [HttpGet]
    [Route("/VT/Settings/HashingSettings")]
    public async Task<IActionResult> HashingSettings(int? id = null)
    {
        ViewData["Title"] = "Hashing Settings";
        return View(await BuildViewModelAsync(id));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/VT/Settings/HashingSettings")]
    public async Task<IActionResult> HashingSettings(HashingSettingsViewModel model)
    {
        ViewData["Title"] = "Hashing Settings";

        if (!ModelState.IsValid)
        {
            return View(await BuildViewModelAsync(model.Form.HashingSettingId, model.Form));
        }

        try
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            await _hashingSettingsService.SaveAsync(model.Form, currentUserId);
            TempData["SuccessMessage"] = "Hashing setting saved successfully.";
            return RedirectToAction(nameof(HashingSettings));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to save hashing settings.");
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(await BuildViewModelAsync(model.Form.HashingSettingId, model.Form));
        }
    }

    private async Task<HashingSettingsViewModel> BuildViewModelAsync(int? id = null, HashingSettingDTO? form = null)
    {
        HashingSettingDTO? currentForm = form;
        if (currentForm == null && id.GetValueOrDefault() > 0)
        {
            currentForm = await _hashingSettingsService.GetByIdAsync(id.Value);
        }

        return new HashingSettingsViewModel
        {
            Form = currentForm ?? new HashingSettingDTO(),
            Items = await _hashingSettingsService.GetAllAsync(),
            SupportedHashingTypes = _hashingSettingsService.SupportedHashingTypes
        };
    }
}
