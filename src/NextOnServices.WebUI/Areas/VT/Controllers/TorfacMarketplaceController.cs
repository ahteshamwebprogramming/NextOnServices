using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Endpoints.Suppliers;
using NextOnServices.Infrastructure.Models.APIProjects;
using NextOnServices.Infrastructure.Models.Supplier;
using NextOnServices.VT.Infrastructure.ViewModels.Project;

namespace NextOnServices.WebUI.VT.Controllers;

[Area("VT")]
[Authorize]
public class TorfacMarketplaceController : Controller
{
    private readonly TorfacMarketplaceAPIController _torfacMarketplaceAPIController;
    private readonly SuppliersAPIController _suppliersAPIController;

    public TorfacMarketplaceController(
        TorfacMarketplaceAPIController torfacMarketplaceAPIController,
        SuppliersAPIController suppliersAPIController)
    {
        _torfacMarketplaceAPIController = torfacMarketplaceAPIController;
        _suppliersAPIController = suppliersAPIController;
    }

    [Route("/VT/TorfacMarketplace")]
    [Route("/VT/TorfacMarketplace/Dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        ViewData["Title"] = "Torfac Marketplace Dashboard";
        return View(new TorfacMarketplaceViewModel
        {
            Setting = await GetSettingAsync()
        });
    }

    [Route("/VT/TorfacMarketplace/GetSurveys")]
    public async Task<IActionResult> GetSurveys()
    {
        ViewData["Title"] = "Torfac Marketplace Get Surveys";
        return View(new TorfacMarketplaceViewModel
        {
            Setting = await GetSettingAsync()
        });
    }

    [Route("/VT/TorfacMarketplace/Settings")]
    public async Task<IActionResult> Settings()
    {
        ViewData["Title"] = "Torfac Marketplace Settings";
        return View(new TorfacMarketplaceViewModel
        {
            Setting = await GetSettingAsync(),
            Suppliers = await GetSuppliersAsync()
        });
    }

    private async Task<TorfacMarketplaceSettingDTO> GetSettingAsync()
    {
        var result = await _torfacMarketplaceAPIController.GetTorfacMarketplaceSetting();
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as TorfacMarketplaceSettingDTO ?? new TorfacMarketplaceSettingDTO()
            : new TorfacMarketplaceSettingDTO();
    }

    private async Task<List<SupplierDTO>> GetSuppliersAsync()
    {
        var result = await _suppliersAPIController.GetAllSuppliers();
        return result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK
            ? objectResult.Value as List<SupplierDTO> ?? new List<SupplierDTO>()
            : new List<SupplierDTO>();
    }
}
