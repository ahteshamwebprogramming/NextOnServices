using Microsoft.AspNetCore.Mvc;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.Models.APIProjects;
using System.Globalization;
using System.Linq;

namespace NextOnServices.Endpoints.Projects;

[Route("api/[controller]")]
[ApiController]
public class TorfacMarketplaceAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public TorfacMarketplaceAPIController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("Setting")]
    public async Task<IActionResult> GetTorfacMarketplaceSetting()
    {
        var setting = await _unitOfWork.TorfacMarketplaceSetting.GetEntityData<TorfacMarketplaceSetting>(
            @"SELECT TOP 1 *
              FROM TorfacMarketplaceSetting
              ORDER BY IsActive DESC, TorfacMarketplaceSettingId DESC");

        return Ok(MapToDto(setting));
    }

    [HttpPost("Setting")]
    public async Task<IActionResult> SaveTorfacMarketplaceSetting(TorfacMarketplaceSettingDTO inputData)
    {
        if (inputData == null)
        {
            return BadRequest("Invalid Torfac Marketplace settings payload.");
        }

        if (string.IsNullOrWhiteSpace(inputData.SurveysUrl))
        {
            return BadRequest("Survey URL is required.");
        }

        var current = await GetCurrentSettingEntityAsync(inputData.TorfacMarketplaceSettingId);
        var normalizedSupplierIds = NormalizeSupplierIds(inputData.DefaultSupplierIds);
        if (current == null)
        {
            if (string.IsNullOrWhiteSpace(inputData.SecretKey))
            {
                return BadRequest("Secret key is required for new Torfac Marketplace settings.");
            }

            var entity = new TorfacMarketplaceSetting
            {
                SurveysUrl = inputData.SurveysUrl.Trim(),
                SecretKey = inputData.SecretKey.Trim(),
                DefaultClientId = inputData.DefaultClientId,
                RespondentIdUrlParts = NormalizeMappingText(inputData.RespondentIdUrlParts),
                RespondentPanelistIdUrlParts = NormalizeMappingText(inputData.RespondentPanelistIdUrlParts),
                DefaultSupplierIds = SerializeSupplierIds(normalizedSupplierIds),
                IsActive = inputData.IsActive,
                CreatedBy = inputData.CreatedBy,
                ModifiedBy = inputData.ModifiedBy
            };
            entity.CreatedDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;
            entity.TorfacMarketplaceSettingId = await _unitOfWork.TorfacMarketplaceSetting.AddAsync(entity);

            if (entity.TorfacMarketplaceSettingId <= 0)
            {
                return BadRequest("Unable to save Torfac Marketplace settings.");
            }

            return Ok(MapToDto(entity));
        }

        current.SurveysUrl = inputData.SurveysUrl.Trim();
        if (!string.IsNullOrWhiteSpace(inputData.SecretKey))
        {
            current.SecretKey = inputData.SecretKey.Trim();
        }

        current.DefaultClientId = inputData.DefaultClientId;
        current.RespondentIdUrlParts = NormalizeMappingText(inputData.RespondentIdUrlParts);
        current.RespondentPanelistIdUrlParts = NormalizeMappingText(inputData.RespondentPanelistIdUrlParts);
        current.DefaultSupplierIds = SerializeSupplierIds(normalizedSupplierIds);
        current.IsActive = inputData.IsActive;
        current.ModifiedDate = DateTime.Now;
        current.ModifiedBy = inputData.ModifiedBy;

        var updated = await _unitOfWork.TorfacMarketplaceSetting.UpdateAsync(current);
        if (!updated)
        {
            return BadRequest("Unable to update Torfac Marketplace settings.");
        }

        return Ok(MapToDto(current));
    }

    private async Task<TorfacMarketplaceSetting?> GetCurrentSettingEntityAsync(int requestedId)
    {
        if (requestedId > 0)
        {
            return await _unitOfWork.TorfacMarketplaceSetting.FindByIdAsync(requestedId);
        }

        return await _unitOfWork.TorfacMarketplaceSetting.GetEntityData<TorfacMarketplaceSetting>(
            @"SELECT TOP 1 *
              FROM TorfacMarketplaceSetting
              ORDER BY IsActive DESC, TorfacMarketplaceSettingId DESC");
    }

    private static TorfacMarketplaceSettingDTO MapToDto(TorfacMarketplaceSetting? entity)
    {
        if (entity == null)
        {
            return new TorfacMarketplaceSettingDTO();
        }

        return new TorfacMarketplaceSettingDTO
        {
            TorfacMarketplaceSettingId = entity.TorfacMarketplaceSettingId,
            SurveysUrl = entity.SurveysUrl,
            SecretKey = entity.SecretKey,
            DefaultClientId = entity.DefaultClientId,
            RespondentIdUrlParts = entity.RespondentIdUrlParts,
            RespondentPanelistIdUrlParts = entity.RespondentPanelistIdUrlParts,
            DefaultSupplierIds = ParseSupplierIds(entity.DefaultSupplierIds),
            IsActive = entity.IsActive,
            CreatedDate = entity.CreatedDate,
            ModifiedDate = entity.ModifiedDate,
            CreatedBy = entity.CreatedBy,
            ModifiedBy = entity.ModifiedBy
        };
    }

    private static List<int> ParseSupplierIds(string? serializedSupplierIds)
    {
        if (string.IsNullOrWhiteSpace(serializedSupplierIds))
        {
            return new List<int>();
        }

        return serializedSupplierIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(value => int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var supplierId) ? supplierId : 0)
            .Where(supplierId => supplierId > 0)
            .Distinct()
            .ToList();
    }

    private static List<int> NormalizeSupplierIds(IEnumerable<int>? supplierIds)
    {
        return supplierIds?
            .Where(supplierId => supplierId > 0)
            .Distinct()
            .ToList()
            ?? new List<int>();
    }

    private static string? SerializeSupplierIds(IEnumerable<int> supplierIds)
    {
        var normalizedSupplierIds = NormalizeSupplierIds(supplierIds);
        return normalizedSupplierIds.Count > 0
            ? string.Join(",", normalizedSupplierIds)
            : null;
    }

    private static string? NormalizeMappingText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
