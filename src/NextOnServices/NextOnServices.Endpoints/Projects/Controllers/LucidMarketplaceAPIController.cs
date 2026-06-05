using System.Data.SqlClient;
using System.Globalization;
using System.Text.Json;
using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.Models.APIProjects;
using NextOnServices.Services.DBContext;
using NextOnServices.VT.Infrastructure.ViewModels.Project;

namespace NextOnServices.Endpoints.Projects;

[Route("api/[controller]")]
[ApiController]
public class LucidMarketplaceAPIController : ControllerBase
{
    private const string OpportunityViewRecent = LucidMarketplaceViewModel.OpportunityViewRecent;
    private const string OpportunityViewAdded = LucidMarketplaceViewModel.OpportunityViewAdded;
    private const string OpportunityViewAll = LucidMarketplaceViewModel.OpportunityViewAll;
    private const string OpportunitiesSubscriptionType = "Opportunities";
    private const string RespondentOutcomesSubscriptionType = "RespondentOutcomes";

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LucidMarketplaceAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly DapperDBSetting _dapperDBSetting;

    public LucidMarketplaceAPIController(
        IUnitOfWork unitOfWork,
        ILogger<LucidMarketplaceAPIController> logger,
        IMapper mapper,
        DapperDBSetting dapperDBSetting)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dapperDBSetting = dapperDBSetting;
    }

    [HttpGet("Setting")]
    public async Task<IActionResult> GetLucidMarketplaceSetting()
    {
        try
        {
            const string query = @"SELECT TOP 1 *
                                   FROM LucidMarketplaceSetting
                                   ORDER BY IsActive DESC, LucidMarketplaceSettingId DESC";

            var setting = await _unitOfWork.LucidMarketplaceSetting.GetEntityData<LucidMarketplaceSettingDTO>(query);
            return Ok(setting ?? new LucidMarketplaceSettingDTO());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Lucid Marketplace setting");
            throw;
        }
    }

    [HttpPost("Setting")]
    public async Task<IActionResult> SaveLucidMarketplaceSetting(LucidMarketplaceSettingDTO inputData)
    {
        try
        {
            if (inputData == null)
            {
                return BadRequest("Invalid settings payload.");
            }

            var current = await GetCurrentSettingEntityAsync(inputData.LucidMarketplaceSettingId);
            if (current == null)
            {
                var entity = _mapper.Map<LucidMarketplaceSetting>(inputData);
                entity.CreatedDate = DateTime.Now;
                entity.ModifiedDate = DateTime.Now;
                entity.LucidMarketplaceSettingId = await _unitOfWork.LucidMarketplaceSetting.AddAsync(entity);

                if (entity.LucidMarketplaceSettingId <= 0)
                {
                    return BadRequest("Unable to save Lucid Marketplace settings.");
                }

                inputData.LucidMarketplaceSettingId = entity.LucidMarketplaceSettingId;
                inputData.CreatedDate = entity.CreatedDate;
                inputData.ModifiedDate = entity.ModifiedDate;
                return Ok(inputData);
            }

            current.BaseUrl = inputData.BaseUrl?.Trim();
            if (!string.IsNullOrWhiteSpace(inputData.ApiKey))
            {
                current.ApiKey = inputData.ApiKey.Trim();
            }
            if (!string.IsNullOrWhiteSpace(inputData.EntryLinkSecretKey))
            {
                current.EntryLinkSecretKey = inputData.EntryLinkSecretKey.Trim();
            }
            current.SupplierCode = inputData.SupplierCode?.Trim();
            current.OpportunitiesCallbackUrl = inputData.OpportunitiesCallbackUrl?.Trim();
            current.OutcomesCallbackUrl = inputData.OutcomesCallbackUrl?.Trim();
            current.UseConsultingsBridge = inputData.UseConsultingsBridge;
            current.AutoSyncEnabled = inputData.AutoSyncEnabled;
            current.SyncIntervalMinutes = inputData.SyncIntervalMinutes;
            current.DefaultClientId = inputData.DefaultClientId;
            current.DefaultCountryId = inputData.DefaultCountryId;
            current.DefaultSupplierId = inputData.DefaultSupplierId;
            current.SupplierLinkTypeCode = inputData.SupplierLinkTypeCode?.Trim();
            current.TrackingTypeCode = inputData.TrackingTypeCode?.Trim();
            current.IsActive = inputData.IsActive;
            current.ModifiedDate = DateTime.Now;
            current.ModifiedBy = inputData.ModifiedBy;

            var updated = await _unitOfWork.LucidMarketplaceSetting.UpdateAsync(current);
            if (!updated)
            {
                return BadRequest("Unable to update Lucid Marketplace settings.");
            }

            inputData.LucidMarketplaceSettingId = current.LucidMarketplaceSettingId;
            inputData.CreatedDate = current.CreatedDate;
            inputData.ModifiedDate = current.ModifiedDate;
            inputData.CreatedBy = current.CreatedBy;
            return Ok(inputData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving Lucid Marketplace setting");
            throw;
        }
    }

    [HttpGet("Subscriptions")]
    public async Task<IActionResult> GetLucidMarketplaceSubscriptions()
    {
        try
        {
            const string query = @"SELECT *
                                   FROM LucidMarketplaceSubscription
                                   ORDER BY ISNULL(ModifiedDate, CreatedDate) DESC, LucidMarketplaceSubscriptionId DESC";

            var items = await _unitOfWork.LucidMarketplaceSubscription.GetTableData<LucidMarketplaceSubscriptionDTO>(query);
            return Ok(items ?? new List<LucidMarketplaceSubscriptionDTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Lucid Marketplace subscriptions");
            throw;
        }
    }

    [HttpPost("Subscription")]
    public async Task<IActionResult> SaveLucidMarketplaceSubscription(LucidMarketplaceSubscriptionDTO inputData)
    {
        try
        {
            if (inputData == null)
            {
                return BadRequest("Invalid subscription payload.");
            }

            inputData.IncludeQuotas = NormalizeIncludeQuotas(inputData.SubscriptionType, inputData.IncludeQuotas);

            var current = await GetCurrentSubscriptionEntityAsync(
                inputData.LucidMarketplaceSubscriptionId,
                inputData.SubscriptionType,
                inputData.SupplierCode);

            LucidMarketplaceCallbackSignatureHelper.PopulateWebhookVerificationMetadata(
                inputData,
                BuildSubscriptionMetadataSnapshot(current));

            if (current != null)
            {
                current.SubscriptionType = inputData.SubscriptionType?.Trim();
                current.SupplierCode = inputData.SupplierCode?.Trim();
                current.CallbackUrl = inputData.CallbackUrl?.Trim();
                current.IncludeQuotas = inputData.IncludeQuotas;
                current.RemoteSubscriptionId = inputData.RemoteSubscriptionId?.Trim();
                current.LastStatus = inputData.LastStatus?.Trim();
                current.RequestPayloadSnapshot = inputData.RequestPayloadSnapshot;
                current.ResponsePayloadSnapshot = inputData.ResponsePayloadSnapshot;
                current.WebhookKeyId = inputData.WebhookKeyId?.Trim();
                current.WebhookKeyIdFull = inputData.WebhookKeyIdFull?.Trim();
                current.WebhookPublicKey = inputData.WebhookPublicKey;
                current.WebhookSecuritySnapshot = inputData.WebhookSecuritySnapshot;
                current.IsActive = inputData.IsActive;
                current.LastValidatedOn = inputData.LastValidatedOn;
                current.ModifiedDate = DateTime.Now;
                current.ModifiedBy = inputData.ModifiedBy;

                var updated = await _unitOfWork.LucidMarketplaceSubscription.UpdateAsync(current);
                if (!updated)
                {
                    return BadRequest("Unable to update subscription.");
                }

                inputData.LucidMarketplaceSubscriptionId = current.LucidMarketplaceSubscriptionId;
                inputData.CreatedDate = current.CreatedDate;
                inputData.ModifiedDate = current.ModifiedDate;
                inputData.CreatedBy = current.CreatedBy;
                return Ok(inputData);
            }

            var entity = _mapper.Map<LucidMarketplaceSubscription>(inputData);
            entity.CreatedDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;
            entity.LucidMarketplaceSubscriptionId = await _unitOfWork.LucidMarketplaceSubscription.AddAsync(entity);

            if (entity.LucidMarketplaceSubscriptionId <= 0)
            {
                return BadRequest("Unable to save subscription.");
            }

            inputData.LucidMarketplaceSubscriptionId = entity.LucidMarketplaceSubscriptionId;
            inputData.CreatedDate = entity.CreatedDate;
            inputData.ModifiedDate = entity.ModifiedDate;
            return Ok(inputData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving Lucid Marketplace subscription");
            throw;
        }
    }

    [HttpPost("Subscription/Deactivate")]
    public async Task<IActionResult> DeactivateLucidMarketplaceSubscription(int subscriptionId, int? modifiedBy = null)
    {
        try
        {
            var current = await _unitOfWork.LucidMarketplaceSubscription.FindByIdAsync(subscriptionId);
            if (current == null)
            {
                return NotFound("Subscription record not found.");
            }

            current.IsActive = false;
            current.LastStatus = "Deactivated";
            current.ModifiedDate = DateTime.Now;
            current.ModifiedBy = modifiedBy;

            var updated = await _unitOfWork.LucidMarketplaceSubscription.UpdateAsync(current);
            return updated ? Ok(true) : BadRequest("Unable to deactivate subscription.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating Lucid Marketplace subscription");
            throw;
        }
    }

    [HttpGet("Opportunities")]
    public async Task<IActionResult> GetLucidMarketplaceOpportunities(string? view = null, int take = 500)
    {
        try
        {
            var normalizedView = NormalizeOpportunityView(view);
            var isRecentView = string.Equals(normalizedView, OpportunityViewRecent, StringComparison.OrdinalIgnoreCase);
            var isAddedView = string.Equals(normalizedView, OpportunityViewAdded, StringComparison.OrdinalIgnoreCase);
            var safeTake = take <= 0 ? 500 : Math.Min(take, 2000);
            var topClause = isRecentView ? $"TOP ({safeTake})" : string.Empty;
            var whereClause = isAddedView
                ? "WHERE COALESCE(pm.InternalProjectId, p.ProjectId, 0) > 0"
                : string.Empty;
            var query = $@"
                SELECT {topClause}
                    o.*,
                    pm.Id AS LucidMarketplaceProjectMapId,
                    InternalProjectId = COALESCE(pm.InternalProjectId, p.ProjectId),
                    LegacyProjectIdFromProjectTable = p.ProjectId,
                    InternalProjectUrlId = COALESCE(pm.InternalProjectUrlId, pu.ID),
                    InternalProjectMappingId = COALESCE(pm.InternalProjectMappingId, map.ID),
                    IsMapped = CAST(CASE WHEN COALESCE(pm.InternalProjectId, p.ProjectId, 0) > 0 THEN 1 ELSE 0 END AS bit),
                    MappingStatus = CASE
                        WHEN COALESCE(pm.InternalProjectId, p.ProjectId, 0) > 0 THEN 'Mapped'
                        ELSE ISNULL(NULLIF(o.LocalState, ''), 'New')
                    END,
                    QualificationCount = CAST(0 AS int),
                    QuotaCount = CAST(0 AS int)
                FROM LucidMarketplaceOpportunity o
                OUTER APPLY (
                    SELECT TOP 1 *
                    FROM LucidMarketplaceProjectMap pm
                    WHERE pm.LucidMarketplaceOpportunityId = o.LucidMarketplaceOpportunityId
                      AND pm.IsActive = 1
                    ORDER BY pm.Id DESC
                ) pm
                OUTER APPLY (
                    SELECT TOP 1 p.ProjectId
                    FROM Projects p
                    WHERE p.ProjectIdFromAPI = CAST(o.SurveyId AS nvarchar(100))
                      AND p.ProjectFrom = 'LucidMarketplace'
                      AND p.IsActive = 1
                    ORDER BY p.ProjectId DESC
                ) p
                OUTER APPLY (
                    SELECT TOP 1 pu.*
                    FROM ProjectsUrl pu
                    WHERE pu.PID = COALESCE(pm.InternalProjectId, p.ProjectId)
                    ORDER BY pu.ID DESC
                ) pu
                OUTER APPLY (
                    SELECT TOP 1 map.*
                    FROM ProjectMapping map
                    WHERE map.ProjectID = COALESCE(pm.InternalProjectId, p.ProjectId)
                    ORDER BY map.ID DESC
                ) map
                {whereClause}
                ORDER BY ISNULL(o.LastSyncedOn, ISNULL(o.ModifiedDate, o.CreatedDate)) DESC,
                         o.LucidMarketplaceOpportunityId DESC";

            var items = await _unitOfWork.LucidMarketplaceOpportunity.GetTableData<LucidMarketplaceOpportunityDTO>(query);
            return Ok(items ?? new List<LucidMarketplaceOpportunityDTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Lucid Marketplace opportunities");
            throw;
        }
    }

    private static string NormalizeOpportunityView(string? view)
    {
        if (string.Equals(view, OpportunityViewAdded, StringComparison.OrdinalIgnoreCase))
        {
            return OpportunityViewAdded;
        }

        if (string.Equals(view, OpportunityViewAll, StringComparison.OrdinalIgnoreCase))
        {
            return OpportunityViewAll;
        }

        return OpportunityViewRecent;
    }

    [HttpGet("Opportunity/{id}")]
    public async Task<IActionResult> GetLucidMarketplaceOpportunity(int id)
    {
        try
        {
            const string query = @"
                SELECT TOP 1
                    o.*,
                    pm.Id AS LucidMarketplaceProjectMapId,
                    InternalProjectId = COALESCE(pm.InternalProjectId, p.ProjectId),
                    LegacyProjectIdFromProjectTable = p.ProjectId,
                    InternalProjectUrlId = COALESCE(pm.InternalProjectUrlId, pu.ID),
                    InternalProjectMappingId = COALESCE(pm.InternalProjectMappingId, map.ID),
                    IsMapped = CAST(CASE WHEN COALESCE(pm.InternalProjectId, p.ProjectId, 0) > 0 THEN 1 ELSE 0 END AS bit),
                    MappingStatus = CASE
                        WHEN COALESCE(pm.InternalProjectId, p.ProjectId, 0) > 0 THEN 'Mapped'
                        ELSE ISNULL(NULLIF(o.LocalState, ''), 'New')
                    END,
                    QualificationCount = CAST(0 AS int),
                    QuotaCount = CAST(0 AS int)
                FROM LucidMarketplaceOpportunity o
                OUTER APPLY (
                    SELECT TOP 1 *
                    FROM LucidMarketplaceProjectMap pm
                    WHERE pm.LucidMarketplaceOpportunityId = o.LucidMarketplaceOpportunityId
                      AND pm.IsActive = 1
                    ORDER BY pm.Id DESC
                ) pm
                OUTER APPLY (
                    SELECT TOP 1 p.ProjectId
                    FROM Projects p
                    WHERE p.ProjectIdFromAPI = CAST(o.SurveyId AS nvarchar(100))
                      AND p.ProjectFrom = 'LucidMarketplace'
                      AND p.IsActive = 1
                    ORDER BY p.ProjectId DESC
                ) p
                OUTER APPLY (
                    SELECT TOP 1 pu.*
                    FROM ProjectsUrl pu
                    WHERE pu.PID = COALESCE(pm.InternalProjectId, p.ProjectId)
                    ORDER BY pu.ID DESC
                ) pu
                OUTER APPLY (
                    SELECT TOP 1 map.*
                    FROM ProjectMapping map
                    WHERE map.ProjectID = COALESCE(pm.InternalProjectId, p.ProjectId)
                    ORDER BY map.ID DESC
                ) map
                WHERE o.LucidMarketplaceOpportunityId = @Id";

            var item = await _unitOfWork.LucidMarketplaceOpportunity.GetEntityData<LucidMarketplaceOpportunityDTO>(query, new { Id = id });
            return item == null ? NotFound() : Ok(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Lucid Marketplace opportunity detail");
            throw;
        }
    }

    [HttpGet("Opportunity/{id}/Qualifications")]
    public async Task<IActionResult> GetLucidMarketplaceOpportunityQualifications(int id)
    {
        return Ok(new List<LucidMarketplaceOpportunityQualificationDTO>());
    }

    [HttpGet("Opportunity/{id}/Quotas")]
    public async Task<IActionResult> GetLucidMarketplaceOpportunityQuotas(int id)
    {
        return Ok(new List<LucidMarketplaceOpportunityQuotaDTO>());
    }

    [HttpGet("Opportunity/{id}/Logs")]
    public async Task<IActionResult> GetLucidMarketplaceOpportunityLogs(int id)
    {
        return Ok(new List<LucidMarketplaceSyncLogDTO>());
    }

    [HttpGet("Opportunity/{id}/ProjectMap")]
    public async Task<IActionResult> GetLucidMarketplaceProjectMap(int id)
    {
        try
        {
            const string query = @"
                SELECT TOP 1
                    Id = COALESCE(pm.Id, 0),
                    LucidMarketplaceOpportunityId = o.LucidMarketplaceOpportunityId,
                    LucidSurveyId = o.SurveyId,
                    InternalProjectId = COALESCE(pm.InternalProjectId, p.ProjectId),
                    InternalProjectUrlId = COALESCE(pm.InternalProjectUrlId, pu.ID),
                    InternalProjectMappingId = COALESCE(pm.InternalProjectMappingId, map.ID),
                    SupplierCode = COALESCE(pm.SupplierCode, o.SupplierCode),
                    AddedBy = pm.AddedBy,
                    AddedOn = pm.AddedOn,
                    IsActive = CAST(CASE WHEN COALESCE(pm.InternalProjectId, p.ProjectId, 0) > 0 THEN 1 ELSE 0 END AS bit),
                    RawJson = pm.RawJson
                FROM LucidMarketplaceOpportunity o
                OUTER APPLY (
                    SELECT TOP 1 *
                    FROM LucidMarketplaceProjectMap pm
                    WHERE pm.LucidMarketplaceOpportunityId = o.LucidMarketplaceOpportunityId
                    ORDER BY IsActive DESC, Id DESC
                ) pm
                OUTER APPLY (
                    SELECT TOP 1 p.ProjectId
                    FROM Projects p
                    WHERE p.ProjectIdFromAPI = CAST(o.SurveyId AS nvarchar(100))
                      AND p.ProjectFrom = 'LucidMarketplace'
                      AND p.IsActive = 1
                    ORDER BY p.ProjectId DESC
                ) p
                OUTER APPLY (
                    SELECT TOP 1 pu.*
                    FROM ProjectsUrl pu
                    WHERE pu.PID = COALESCE(pm.InternalProjectId, p.ProjectId)
                    ORDER BY pu.ID DESC
                ) pu
                OUTER APPLY (
                    SELECT TOP 1 map.*
                    FROM ProjectMapping map
                    WHERE map.ProjectID = COALESCE(pm.InternalProjectId, p.ProjectId)
                    ORDER BY map.ID DESC
                ) map
                WHERE o.LucidMarketplaceOpportunityId = @Id
                ORDER BY CASE WHEN COALESCE(pm.InternalProjectId, p.ProjectId, 0) > 0 THEN 0 ELSE 1 END,
                         ISNULL(pm.Id, 0) DESC";

            var item = await _unitOfWork.LucidMarketplaceProjectMap.GetEntityData<LucidMarketplaceProjectMapDTO>(query, new { Id = id });
            return Ok(item ?? new LucidMarketplaceProjectMapDTO());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Lucid Marketplace project map");
            throw;
        }
    }

    [HttpPost("ProjectMap")]
    public async Task<IActionResult> SaveLucidMarketplaceProjectMap(LucidMarketplaceProjectMapDTO inputData)
    {
        try
        {
            if (inputData == null || inputData.LucidMarketplaceOpportunityId <= 0 || inputData.LucidSurveyId <= 0)
            {
                return BadRequest("Invalid Lucid Marketplace project mapping payload.");
            }

            LucidMarketplaceProjectMap? current = null;
            if (inputData.Id > 0)
            {
                current = await _unitOfWork.LucidMarketplaceProjectMap.FindByIdAsync(inputData.Id);
            }

            current ??= await _unitOfWork.LucidMarketplaceProjectMap.GetEntityData<LucidMarketplaceProjectMap>(
                @"SELECT TOP 1 *
                  FROM LucidMarketplaceProjectMap
                  WHERE LucidMarketplaceOpportunityId = @LucidMarketplaceOpportunityId
                  ORDER BY Id DESC",
                new { inputData.LucidMarketplaceOpportunityId });

            if (current == null)
            {
                var entity = _mapper.Map<LucidMarketplaceProjectMap>(inputData);
                entity.AddedOn = inputData.AddedOn ?? DateTime.Now;
                entity.IsActive = inputData.IsActive;
                entity.Id = await _unitOfWork.LucidMarketplaceProjectMap.AddAsync(entity);
                if (entity.Id <= 0)
                {
                    return BadRequest("Unable to save Lucid Marketplace project mapping.");
                }

                inputData.Id = entity.Id;
                inputData.AddedOn = entity.AddedOn;
                return Ok(inputData);
            }

            current.LucidMarketplaceOpportunityId = inputData.LucidMarketplaceOpportunityId;
            current.LucidSurveyId = inputData.LucidSurveyId;
            current.InternalProjectId = inputData.InternalProjectId;
            current.InternalProjectUrlId = inputData.InternalProjectUrlId;
            current.InternalProjectMappingId = inputData.InternalProjectMappingId;
            current.SupplierCode = inputData.SupplierCode?.Trim();
            current.IsActive = inputData.IsActive;
            current.RawJson = inputData.RawJson;
            current.AddedBy ??= inputData.AddedBy;
            current.AddedOn ??= inputData.AddedOn ?? DateTime.Now;

            var updated = await _unitOfWork.LucidMarketplaceProjectMap.UpdateAsync(current);
            if (!updated)
            {
                return BadRequest("Unable to update Lucid Marketplace project mapping.");
            }

            inputData.Id = current.Id;
            inputData.AddedBy = current.AddedBy;
            inputData.AddedOn = current.AddedOn;
            return Ok(inputData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving Lucid Marketplace project mapping");
            throw;
        }
    }

    [HttpGet("Opportunity/{id}/EntryLink")]
    public async Task<IActionResult> GetLucidMarketplaceEntryLink(int id)
    {
        try
        {
            const string query = @"SELECT TOP 1 *
                                   FROM LucidMarketplaceEntryLink
                                   WHERE LucidMarketplaceOpportunityId = @Id
                                   ORDER BY IsActive DESC, ISNULL(ModifiedDate, CreatedDate) DESC, Id DESC";

            var item = await _unitOfWork.LucidMarketplaceEntryLink.GetEntityData<LucidMarketplaceEntryLinkDTO>(query, new { Id = id });
            return Ok(item ?? new LucidMarketplaceEntryLinkDTO());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Lucid Marketplace entry link");
            throw;
        }
    }

    [HttpPost("EntryLink")]
    public async Task<IActionResult> SaveLucidMarketplaceEntryLink(LucidMarketplaceEntryLinkDTO inputData)
    {
        try
        {
            if (inputData == null || inputData.LucidMarketplaceOpportunityId <= 0 || inputData.LucidSurveyId <= 0)
            {
                return BadRequest("Invalid Lucid Marketplace entry-link payload.");
            }

            var current = inputData.Id > 0
                ? await _unitOfWork.LucidMarketplaceEntryLink.FindByIdAsync(inputData.Id)
                : await GetCurrentEntryLinkEntityAsync(inputData.LucidMarketplaceOpportunityId);

            if (current == null)
            {
                var entity = _mapper.Map<LucidMarketplaceEntryLink>(inputData);
                entity.CreatedDate = DateTime.Now;
                entity.ModifiedDate = DateTime.Now;
                entity.Id = await _unitOfWork.LucidMarketplaceEntryLink.AddAsync(entity);
                if (entity.Id <= 0)
                {
                    return BadRequest("Unable to save Lucid Marketplace entry link.");
                }

                inputData.Id = entity.Id;
                inputData.CreatedDate = entity.CreatedDate;
                inputData.ModifiedDate = entity.ModifiedDate;
                return Ok(inputData);
            }

            current.LucidMarketplaceOpportunityId = inputData.LucidMarketplaceOpportunityId;
            current.LucidSurveyId = inputData.LucidSurveyId;
            current.SupplierCode = inputData.SupplierCode?.Trim();
            current.InternalProjectId = inputData.InternalProjectId;
            current.InternalProjectUrlId = inputData.InternalProjectUrlId;
            current.InternalProjectMappingId = inputData.InternalProjectMappingId;
            current.SupplierLinkTypeCode = inputData.SupplierLinkTypeCode?.Trim();
            current.TrackingTypeCode = inputData.TrackingTypeCode?.Trim();
            current.DefaultLink = inputData.DefaultLink;
            current.SuccessLink = inputData.SuccessLink;
            current.FailureLink = inputData.FailureLink;
            current.OverQuotaLink = inputData.OverQuotaLink;
            current.QualityTerminationLink = inputData.QualityTerminationLink;
            current.LiveLink = inputData.LiveLink;
            current.TestLink = inputData.TestLink;
            current.SupplierLinkSid = inputData.SupplierLinkSid?.Trim();
            current.RPIValue = inputData.RPIValue;
            current.RPICurrencyCode = inputData.RPICurrencyCode?.Trim();
            current.RawJson = inputData.RawJson;
            current.IsActive = inputData.IsActive;
            current.ModifiedDate = DateTime.Now;
            current.ModifiedBy = inputData.ModifiedBy;
            current.CreatedBy ??= inputData.CreatedBy;

            var updated = await _unitOfWork.LucidMarketplaceEntryLink.UpdateAsync(current);
            if (!updated)
            {
                return BadRequest("Unable to update Lucid Marketplace entry link.");
            }

            inputData.Id = current.Id;
            inputData.CreatedDate = current.CreatedDate;
            inputData.ModifiedDate = current.ModifiedDate;
            inputData.CreatedBy = current.CreatedBy;
            return Ok(inputData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving Lucid Marketplace entry link");
            throw;
        }
    }

    [HttpGet("Opportunity/{id}/Attempts")]
    public async Task<IActionResult> GetLucidMarketplaceRespondentAttempts(int id, int take = 100)
    {
        try
        {
            var safeTake = take <= 0 ? 100 : Math.Min(take, 500);
            var query = $@"SELECT TOP ({safeTake}) *
                           FROM LucidMarketplaceRespondentAttempt
                           WHERE LucidMarketplaceOpportunityId = @Id
                           ORDER BY ISNULL(AttemptedOn, CreatedDate) DESC, Id DESC";

            var items = await _unitOfWork.LucidMarketplaceRespondentAttempt
                .GetTableData<LucidMarketplaceRespondentAttemptDTO>(query, new { Id = id });

            return Ok(items ?? new List<LucidMarketplaceRespondentAttemptDTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Lucid Marketplace respondent attempts");
            throw;
        }
    }

    [HttpGet("RespondentAttempt/{id}")]
    public async Task<IActionResult> GetLucidMarketplaceRespondentAttempt(int id)
    {
        try
        {
            var entity = await _unitOfWork.LucidMarketplaceRespondentAttempt.FindByIdAsync(id);
            return entity == null ? NotFound() : Ok(_mapper.Map<LucidMarketplaceRespondentAttemptDTO>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Lucid Marketplace respondent attempt detail");
            throw;
        }
    }

    [HttpGet("Outcomes")]
    public async Task<IActionResult> GetLucidMarketplaceRespondentOutcomes(int take = 500, int? surveyId = null, string? finalStatus = null)
    {
        try
        {
            var safeTake = take <= 0 ? 500 : Math.Min(take, 2000);
            var query = $@"
                SELECT TOP ({safeTake})
                    o.*,
                    AttemptMatched = CAST(CASE WHEN ISNULL(o.RelatedAttemptId, 0) > 0 THEN 1 ELSE 0 END AS bit)
                FROM LucidMarketplaceRespondentOutcome o
                WHERE o.IsLatest = 1
                  AND (@SurveyId IS NULL OR o.SurveyId = @SurveyId)
                  AND (@FinalStatus IS NULL OR o.FinalStatus = @FinalStatus)
                ORDER BY ISNULL(o.ReceivedOn, o.CreatedDate) DESC, o.Id DESC";

            var items = await _unitOfWork.LucidMarketplaceRespondentOutcome.GetTableData<LucidMarketplaceRespondentOutcomeDTO>(query, new
            {
                SurveyId = surveyId,
                FinalStatus = string.IsNullOrWhiteSpace(finalStatus) ? null : finalStatus.Trim()
            });

            return Ok(items ?? new List<LucidMarketplaceRespondentOutcomeDTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Lucid Marketplace respondent outcomes");
            throw;
        }
    }

    [HttpGet("RespondentOutcome/{id}")]
    public async Task<IActionResult> GetLucidMarketplaceRespondentOutcome(int id)
    {
        try
        {
            const string query = @"
                SELECT TOP 1
                    o.*,
                    AttemptMatched = CAST(CASE WHEN ISNULL(o.RelatedAttemptId, 0) > 0 THEN 1 ELSE 0 END AS bit)
                FROM LucidMarketplaceRespondentOutcome o
                WHERE o.Id = @Id";

            var entity = await _unitOfWork.LucidMarketplaceRespondentOutcome.GetEntityData<LucidMarketplaceRespondentOutcomeDTO>(query, new { Id = id });
            return entity == null ? NotFound() : Ok(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Lucid Marketplace respondent outcome detail");
            throw;
        }
    }

    [HttpGet("RespondentOutcome/{id}/History")]
    public async Task<IActionResult> GetLucidMarketplaceRespondentOutcomeHistory(int id, int take = 25)
    {
        try
        {
            var current = await _unitOfWork.LucidMarketplaceRespondentOutcome.FindByIdAsync(id);
            if (current == null)
            {
                return NotFound();
            }

            var safeTake = take <= 0 ? 25 : Math.Min(take, 200);
            var query = $@"
                SELECT TOP ({safeTake})
                    o.*,
                    AttemptMatched = CAST(CASE WHEN ISNULL(o.RelatedAttemptId, 0) > 0 THEN 1 ELSE 0 END AS bit)
                FROM LucidMarketplaceRespondentOutcome o
                WHERE (
                        @SessionId IS NOT NULL
                        AND ISNULL(o.SupplierCode, '') = ISNULL(@SupplierCode, '')
                        AND o.SessionId = @SessionId
                      )
                   OR (
                        @SessionId IS NULL
                        AND @RespondentId IS NOT NULL
                        AND ISNULL(o.SupplierCode, '') = ISNULL(@SupplierCode, '')
                        AND ISNULL(o.RespondentId, '') = @RespondentId
                        AND (
                            (@SurveyId IS NULL AND o.SurveyId IS NULL)
                            OR o.SurveyId = @SurveyId
                        )
                      )
                   OR (
                        @SessionId IS NULL
                        AND @RespondentId IS NULL
                        AND @PanelistId IS NOT NULL
                        AND @SurveyId IS NOT NULL
                        AND ISNULL(o.SupplierCode, '') = ISNULL(@SupplierCode, '')
                        AND ISNULL(o.PanelistId, '') = @PanelistId
                        AND o.SurveyId = @SurveyId
                      )
                ORDER BY ISNULL(o.ReceivedOn, o.CreatedDate) DESC, o.Id DESC";

            var items = await _unitOfWork.LucidMarketplaceRespondentOutcome.GetTableData<LucidMarketplaceRespondentOutcomeDTO>(query, new
            {
                current.SupplierCode,
                current.SessionId,
                current.RespondentId,
                current.PanelistId,
                current.SurveyId
            });

            return Ok(items ?? new List<LucidMarketplaceRespondentOutcomeDTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Lucid Marketplace respondent outcome history");
            throw;
        }
    }

    [HttpGet("ReconciliationRuns")]
    public async Task<IActionResult> GetLucidMarketplaceReconciliationRuns(int take = 50)
    {
        try
        {
            var safeTake = take <= 0 ? 50 : Math.Min(take, 250);
            var query = $@"
                SELECT TOP ({safeTake}) *
                FROM LucidMarketplaceReconciliationRun
                ORDER BY ISNULL(CompletedOn, ISNULL(StartedOn, CreatedDate)) DESC, Id DESC";

            var items = await _unitOfWork.LucidMarketplaceReconciliationRun
                .GetTableData<LucidMarketplaceReconciliationRunDTO>(query);

            return Ok(items ?? new List<LucidMarketplaceReconciliationRunDTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Lucid Marketplace reconciliation runs");
            throw;
        }
    }

    [HttpGet("ReconciliationRun/{id}")]
    public async Task<IActionResult> GetLucidMarketplaceReconciliationRun(int id)
    {
        try
        {
            var entity = await _unitOfWork.LucidMarketplaceReconciliationRun.FindByIdAsync(id);
            return entity == null ? NotFound() : Ok(_mapper.Map<LucidMarketplaceReconciliationRunDTO>(entity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Lucid Marketplace reconciliation run");
            throw;
        }
    }

    [HttpGet("ReconciliationRun/{id}/Items")]
    public async Task<IActionResult> GetLucidMarketplaceReconciliationItems(int id)
    {
        try
        {
            const string query = @"
                SELECT *
                FROM LucidMarketplaceReconciliationItem
                WHERE ReconciliationRunId = @Id
                ORDER BY IsMismatch DESC, LucidSurveyId DESC, Id DESC";

            var items = await _unitOfWork.LucidMarketplaceReconciliationItem
                .GetTableData<LucidMarketplaceReconciliationItemDTO>(query, new { Id = id });

            return Ok(items ?? new List<LucidMarketplaceReconciliationItemDTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Lucid Marketplace reconciliation items");
            throw;
        }
    }

    [HttpPost("ReconciliationRun")]
    public async Task<IActionResult> RunLucidMarketplaceReconciliation(LucidMarketplaceReconciliationRunRequest? inputData, int? createdBy = null)
    {
        var normalizedRequest = NormalizeReconciliationRequest(inputData);
        var startedOn = DateTime.Now;
        var scopeJson = JsonSerializer.Serialize(new
        {
            normalizedRequest.RunType,
            normalizedRequest.SupplierCode,
            normalizedRequest.LucidSurveyId,
            normalizedRequest.InternalProjectId,
            normalizedRequest.DateFrom,
            normalizedRequest.DateTo
        });

        await InsertLogAsync(new LucidMarketplaceSyncLogDTO
        {
            ModuleName = "Lucid Marketplace",
            ActionName = "ReconciliationRunStart",
            Source = "manual",
            SupplierCode = normalizedRequest.SupplierCode,
            RelatedSurveyId = normalizedRequest.LucidSurveyId,
            RelatedEntityId = normalizedRequest.InternalProjectId,
            RequestBodySnapshot = Truncate(scopeJson, 8000),
            ResponseStatusCode = StatusCodes.Status200OK,
            ResponseBodySnapshot = $"Reconciliation started for {normalizedRequest.RunType} scope.",
            IsSuccess = true,
            StartedOn = startedOn,
            CompletedOn = startedOn,
            CreatedBy = createdBy
        });

        try
        {
            var buildResult = await BuildLucidMarketplaceReconciliationAsync(normalizedRequest, createdBy, startedOn, scopeJson);
            var runId = await PersistReconciliationRunAsync(buildResult.Run, buildResult.Items);

            await InsertLogAsync(new LucidMarketplaceSyncLogDTO
            {
                ModuleName = "Lucid Marketplace",
                ActionName = "ReconciliationRunFinish",
                Source = "manual",
                SupplierCode = buildResult.Run.SupplierCode,
                RelatedSurveyId = buildResult.Run.LucidSurveyId,
                RelatedEntityId = runId,
                RequestBodySnapshot = Truncate(scopeJson, 8000),
                ResponseStatusCode = StatusCodes.Status200OK,
                ResponseBodySnapshot = $"Reconciliation run {runId} completed. Reviewed {buildResult.Run.TotalReviewed}, mismatched {buildResult.Run.TotalMismatched}.",
                IsSuccess = true,
                StartedOn = startedOn,
                CompletedOn = DateTime.Now,
                CreatedBy = createdBy
            });

            return Ok(new LucidMarketplaceReconciliationRunResult
            {
                ReconciliationRunId = runId,
                TotalReviewed = buildResult.Run.TotalReviewed,
                TotalMismatched = buildResult.Run.TotalMismatched,
                Success = true,
                Message = $"Reconciliation run #{runId} completed successfully."
            });
        }
        catch (Exception ex)
        {
            var failedRun = BuildFailedReconciliationRun(normalizedRequest, createdBy, startedOn, scopeJson, ex.Message);
            int? failedRunId = null;

            try
            {
                failedRunId = await PersistReconciliationRunAsync(failedRun, new List<LucidMarketplaceReconciliationItemDTO>());
            }
            catch (Exception persistenceEx)
            {
                _logger.LogError(persistenceEx, "Error persisting failed Lucid Marketplace reconciliation run");
            }

            await InsertLogAsync(new LucidMarketplaceSyncLogDTO
            {
                ModuleName = "Lucid Marketplace",
                ActionName = "ReconciliationRunFailure",
                Source = "manual",
                SupplierCode = normalizedRequest.SupplierCode,
                RelatedSurveyId = normalizedRequest.LucidSurveyId,
                RelatedEntityId = failedRunId,
                RequestBodySnapshot = Truncate(scopeJson, 8000),
                ResponseStatusCode = StatusCodes.Status500InternalServerError,
                ErrorText = ex.Message,
                ResponseBodySnapshot = Truncate(ex.ToString(), 8000),
                IsSuccess = false,
                StartedOn = startedOn,
                CompletedOn = DateTime.Now,
                CreatedBy = createdBy
            });

            return StatusCode(StatusCodes.Status500InternalServerError, "Unable to complete Lucid Marketplace reconciliation.");
        }
    }

    [HttpPost("RespondentAttempt")]
    public async Task<IActionResult> SaveLucidMarketplaceRespondentAttempt(LucidMarketplaceRespondentAttemptDTO inputData)
    {
        try
        {
            if (inputData == null || inputData.LucidMarketplaceOpportunityId <= 0 || inputData.LucidSurveyId <= 0)
            {
                return BadRequest("Invalid Lucid Marketplace respondent attempt payload.");
            }

            var current = inputData.Id > 0
                ? await _unitOfWork.LucidMarketplaceRespondentAttempt.FindByIdAsync(inputData.Id)
                : null;

            if (current == null)
            {
                var entity = _mapper.Map<LucidMarketplaceRespondentAttempt>(inputData);
                entity.CreatedDate = DateTime.Now;
                entity.ModifiedDate = DateTime.Now;
                entity.Id = await _unitOfWork.LucidMarketplaceRespondentAttempt.AddAsync(entity);
                if (entity.Id <= 0)
                {
                    return BadRequest("Unable to save Lucid Marketplace respondent attempt.");
                }

                inputData.Id = entity.Id;
                inputData.CreatedDate = entity.CreatedDate;
                inputData.ModifiedDate = entity.ModifiedDate;
                return Ok(inputData);
            }

            current.LucidMarketplaceOpportunityId = inputData.LucidMarketplaceOpportunityId;
            current.InternalProjectId = inputData.InternalProjectId;
            current.InternalProjectUrlId = inputData.InternalProjectUrlId;
            current.InternalProjectMappingId = inputData.InternalProjectMappingId;
            current.LucidSurveyId = inputData.LucidSurveyId;
            current.SupplierCode = inputData.SupplierCode?.Trim();
            current.RespondentId = inputData.RespondentId?.Trim();
            current.ParentSessionId = inputData.ParentSessionId?.Trim();
            current.PanelistId = inputData.PanelistId?.Trim();
            current.SessionId = inputData.SessionId?.Trim();
            current.LaunchUrl = inputData.LaunchUrl;
            current.AttemptType = inputData.AttemptType?.Trim();
            current.AttemptedOn = inputData.AttemptedOn;
            current.ReturnStatus = inputData.ReturnStatus?.Trim();
            current.ReturnCode = inputData.ReturnCode?.Trim();
            current.ReturnRawQuery = inputData.ReturnRawQuery;
            current.MarketplaceStatus = inputData.MarketplaceStatus;
            current.ClientStatus = inputData.ClientStatus;
            current.FinalStatus = inputData.FinalStatus?.Trim();
            current.FinalStatusSource = inputData.FinalStatusSource?.Trim();
            current.AsyncLastReceivedOn = inputData.AsyncLastReceivedOn;
            current.LatestRespondentOutcomeId = inputData.LatestRespondentOutcomeId;
            current.RevenueValue = inputData.RevenueValue;
            current.RevenueCurrencyCode = inputData.RevenueCurrencyCode?.Trim();
            current.IsCompleted = inputData.IsCompleted;
            current.IsTerminated = inputData.IsTerminated;
            current.IsOverQuota = inputData.IsOverQuota;
            current.IsQualityTermination = inputData.IsQualityTermination;
            current.IsDuplicate = inputData.IsDuplicate;
            current.IsSecurityTermination = inputData.IsSecurityTermination;
            current.RawJson = inputData.RawJson;
            current.Notes = inputData.Notes;
            current.ModifiedDate = DateTime.Now;

            var updated = await _unitOfWork.LucidMarketplaceRespondentAttempt.UpdateAsync(current);
            if (!updated)
            {
                return BadRequest("Unable to update Lucid Marketplace respondent attempt.");
            }

            inputData.Id = current.Id;
            inputData.CreatedDate = current.CreatedDate;
            inputData.ModifiedDate = current.ModifiedDate;
            return Ok(inputData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving Lucid Marketplace respondent attempt");
            throw;
        }
    }

    [HttpPost("RespondentOutcome")]
    public async Task<IActionResult> SaveLucidMarketplaceRespondentOutcome(LucidMarketplaceRespondentOutcomeDTO inputData)
    {
        try
        {
            if (inputData == null)
            {
                return BadRequest("Invalid Lucid Marketplace respondent outcome payload.");
            }

            var current = inputData.Id > 0
                ? await _unitOfWork.LucidMarketplaceRespondentOutcome.FindByIdAsync(inputData.Id)
                : null;

            if (current == null)
            {
                var entity = _mapper.Map<LucidMarketplaceRespondentOutcome>(inputData);
                entity.CreatedDate = DateTime.Now;
                entity.ModifiedDate = DateTime.Now;
                entity.Id = await _unitOfWork.LucidMarketplaceRespondentOutcome.AddAsync(entity);
                if (entity.Id <= 0)
                {
                    return BadRequest("Unable to save Lucid Marketplace respondent outcome.");
                }

                inputData.Id = entity.Id;
                inputData.CreatedDate = entity.CreatedDate;
                inputData.ModifiedDate = entity.ModifiedDate;
                return Ok(inputData);
            }

            current.SupplierCode = inputData.SupplierCode?.Trim();
            current.RespondentId = inputData.RespondentId?.Trim();
            current.ParentSessionId = inputData.ParentSessionId?.Trim();
            current.PanelistId = inputData.PanelistId?.Trim();
            current.SessionId = inputData.SessionId?.Trim();
            current.MarketplaceStatus = inputData.MarketplaceStatus;
            current.ClientStatus = inputData.ClientStatus;
            current.EntryDate = inputData.EntryDate;
            current.LastDate = inputData.LastDate;
            current.SurveyId = inputData.SurveyId;
            current.RevenueValue = inputData.RevenueValue;
            current.RevenueCurrencyCode = inputData.RevenueCurrencyCode?.Trim();
            current.StudyType = inputData.StudyType?.Trim();
            current.BuyerId = inputData.BuyerId;
            current.ProofCostPerInterview = inputData.ProofCostPerInterview;
            current.FinalStatus = inputData.FinalStatus?.Trim();
            current.RawJson = inputData.RawJson;
            current.Source = inputData.Source?.Trim();
            current.ReceivedOn = inputData.ReceivedOn;
            current.IsLatest = inputData.IsLatest;
            current.RelatedAttemptId = inputData.RelatedAttemptId;
            current.RelatedOpportunityId = inputData.RelatedOpportunityId;
            current.RelatedInternalProjectId = inputData.RelatedInternalProjectId;
            current.ModifiedDate = DateTime.Now;

            var updated = await _unitOfWork.LucidMarketplaceRespondentOutcome.UpdateAsync(current);
            if (!updated)
            {
                return BadRequest("Unable to update Lucid Marketplace respondent outcome.");
            }

            inputData.Id = current.Id;
            inputData.CreatedDate = current.CreatedDate;
            inputData.ModifiedDate = current.ModifiedDate;
            return Ok(inputData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving Lucid Marketplace respondent outcome");
            throw;
        }
    }

    [HttpPost("Opportunity/State")]
    public async Task<IActionResult> UpdateLucidMarketplaceOpportunityState(LucidMarketplaceOpportunityStateRequest inputData, int? modifiedBy = null)
    {
        try
        {
            if (inputData == null || inputData.LucidMarketplaceOpportunityId <= 0)
            {
                return BadRequest("A valid opportunity id is required.");
            }

            var current = await _unitOfWork.LucidMarketplaceOpportunity.FindByIdAsync(inputData.LucidMarketplaceOpportunityId);
            if (current == null)
            {
                return NotFound("Opportunity record not found.");
            }

            current.IsActive = inputData.IsActive;
            if (!string.IsNullOrWhiteSpace(inputData.LocalState))
            {
                current.LocalState = inputData.LocalState.Trim();
            }

            current.ModifiedDate = DateTime.Now;
            current.ModifiedBy = modifiedBy;

            var updated = await _unitOfWork.LucidMarketplaceOpportunity.UpdateAsync(current);
            return updated ? Ok(true) : BadRequest("Unable to update opportunity state.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Lucid Marketplace opportunity state");
            throw;
        }
    }

    [HttpGet("Logs")]
    public async Task<IActionResult> GetLucidMarketplaceLogs(int take = 250)
    {
        return Ok(new List<LucidMarketplaceSyncLogDTO>());
    }

    [HttpGet("Log/{id}")]
    public async Task<IActionResult> GetLucidMarketplaceLog(int id)
    {
        return NotFound();
    }

    [HttpPost("Log")]
    public async Task<IActionResult> AddLucidMarketplaceLog(LucidMarketplaceSyncLogDTO inputData)
    {
        if (inputData == null)
        {
            return BadRequest("Invalid log payload.");
        }

        return Ok(inputData);
    }

    [HttpGet("Dashboard")]
    public async Task<IActionResult> GetLucidMarketplaceDashboard()
    {
        try
        {
            const string query = @"
                SELECT
                    ActiveSettings = (SELECT COUNT(1) FROM LucidMarketplaceSetting WHERE IsActive = 1),
                    ActiveSubscriptions = (SELECT COUNT(1) FROM LucidMarketplaceSubscription WHERE IsActive = 1),
                    ActiveOpportunities = (SELECT COUNT(1) FROM LucidMarketplaceOpportunity WHERE IsActive = 1),
                    LastSyncTime = (SELECT MAX(LastSyncedOn) FROM LucidMarketplaceOpportunity),
                    TotalLogs = CAST(0 AS int),
                    SuccessfulApiCalls = CAST(0 AS int),
                    FailedApiCalls = CAST(0 AS int),
                    LatestOutcomesReceivedCount = (SELECT COUNT(1) FROM LucidMarketplaceRespondentOutcome WHERE IsLatest = 1),
                    LatestReconciliationRunTime = (
                        SELECT MAX(COALESCE(CompletedOn, StartedOn, CreatedDate))
                        FROM LucidMarketplaceReconciliationRun
                        WHERE Success = 1
                    ),
                    LatestReconciliationMismatchCount = ISNULL((
                        SELECT TOP 1 TotalMismatched
                        FROM LucidMarketplaceReconciliationRun
                        WHERE Success = 1
                        ORDER BY ISNULL(CompletedOn, ISNULL(StartedOn, CreatedDate)) DESC, Id DESC
                    ), 0),
                    LatestReconciliationCompleteCount = ISNULL((
                        SELECT TOP 1 CompleteCount
                        FROM LucidMarketplaceReconciliationRun
                        WHERE Success = 1
                        ORDER BY ISNULL(CompletedOn, ISNULL(StartedOn, CreatedDate)) DESC, Id DESC
                    ), 0),
                    LatestReconciliationTerminateCount = ISNULL((
                        SELECT TOP 1 TerminateCount
                        FROM LucidMarketplaceReconciliationRun
                        WHERE Success = 1
                        ORDER BY ISNULL(CompletedOn, ISNULL(StartedOn, CreatedDate)) DESC, Id DESC
                    ), 0)";

            var summary = await _unitOfWork.LucidMarketplaceOpportunity.GetEntityData<LucidMarketplaceDashboardVM>(query);
            return Ok(summary ?? new LucidMarketplaceDashboardVM());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Lucid Marketplace dashboard summary");
            throw;
        }
    }

    [NonAction]
    public async Task<LucidMarketplaceOpportunityProcessResult> ProcessOpportunitiesPayloadAsync(
        string rawJson,
        string source,
        string? supplierCode,
        int? userId = null)
    {
        var result = new LucidMarketplaceOpportunityProcessResult();
        var effectiveSupplierCode = supplierCode?.Trim();
        var now = DateTime.Now;

        try
        {
            using var document = JsonDocument.Parse(rawJson, new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            });

            var opportunities = ExtractOpportunityElements(document.RootElement);
            if (opportunities.Count == 0)
            {
                await InsertLogAsync(new LucidMarketplaceSyncLogDTO
                {
                    ModuleName = "Lucid Marketplace",
                    ActionName = "CallbackParsed",
                    Source = source,
                    SupplierCode = effectiveSupplierCode,
                    RequestBodySnapshot = Truncate(rawJson, 8000),
                    ResponseStatusCode = StatusCodes.Status200OK,
                    ResponseBodySnapshot = "No survey opportunities were found in the callback payload.",
                    IsSuccess = true,
                    StartedOn = now,
                    CompletedOn = DateTime.Now,
                    CreatedBy = userId
                });

                return result;
            }

            foreach (var opportunityElement in opportunities)
            {
                if (!TryGetRequiredSurveyId(opportunityElement, out var surveyId))
                {
                    result.SkippedCount++;
                    result.Errors.Add("Skipped an opportunity because survey_id was missing or invalid.");

                    await InsertLogAsync(new LucidMarketplaceSyncLogDTO
                    {
                        ModuleName = "Lucid Marketplace",
                        ActionName = "CallbackSaveSkipped",
                        Source = source,
                        SupplierCode = effectiveSupplierCode,
                        RequestBodySnapshot = Truncate(opportunityElement.GetRawText(), 8000),
                        ResponseStatusCode = StatusCodes.Status400BadRequest,
                        ErrorText = "survey_id was missing or invalid.",
                        IsSuccess = false,
                        StartedOn = DateTime.Now,
                        CompletedOn = DateTime.Now,
                        CreatedBy = userId
                    });
                    continue;
                }

                try
                {
                    effectiveSupplierCode ??= GetStringValue(opportunityElement, "supplier_code", "supplierCode", "name");
                    if (string.IsNullOrWhiteSpace(effectiveSupplierCode))
                    {
                        var setting = await GetCurrentSettingEntityAsync(0);
                        effectiveSupplierCode = setting?.SupplierCode?.Trim();
                    }

                    var opportunityId = await UpsertOpportunityAsync(opportunityElement, effectiveSupplierCode, userId);
                    result.ProcessedCount++;
                    result.OpportunityIds.Add(opportunityId);
                    result.SurveyIds.Add(surveyId);

                    await InsertLogAsync(new LucidMarketplaceSyncLogDTO
                    {
                        ModuleName = "Lucid Marketplace",
                        ActionName = "CallbackSaveSuccess",
                        Source = source,
                        SupplierCode = effectiveSupplierCode,
                        RelatedEntityId = opportunityId,
                        RelatedSurveyId = surveyId,
                        RequestBodySnapshot = Truncate(opportunityElement.GetRawText(), 8000),
                        ResponseStatusCode = StatusCodes.Status200OK,
                        ResponseBodySnapshot = $"Saved opportunity {surveyId}.",
                        IsSuccess = true,
                        StartedOn = DateTime.Now,
                        CompletedOn = DateTime.Now,
                        CreatedBy = userId
                    });
                }
                catch (Exception ex)
                {
                    result.SkippedCount++;
                    result.Errors.Add(ex.Message);

                    await InsertLogAsync(new LucidMarketplaceSyncLogDTO
                    {
                        ModuleName = "Lucid Marketplace",
                        ActionName = "CallbackSaveFailure",
                        Source = source,
                        SupplierCode = effectiveSupplierCode,
                        RelatedSurveyId = surveyId,
                        RequestBodySnapshot = Truncate(opportunityElement.GetRawText(), 8000),
                        ResponseStatusCode = StatusCodes.Status500InternalServerError,
                        ErrorText = ex.Message,
                        ResponseBodySnapshot = Truncate(ex.ToString(), 8000),
                        IsSuccess = false,
                        StartedOn = DateTime.Now,
                        CompletedOn = DateTime.Now,
                        CreatedBy = userId
                    });
                }
            }

            return result;
        }
        catch (JsonException ex)
        {
            await InsertLogAsync(new LucidMarketplaceSyncLogDTO
            {
                ModuleName = "Lucid Marketplace",
                ActionName = "CallbackParseFailure",
                Source = source,
                SupplierCode = effectiveSupplierCode,
                RequestBodySnapshot = Truncate(rawJson, 8000),
                ResponseStatusCode = StatusCodes.Status400BadRequest,
                ErrorText = ex.Message,
                ResponseBodySnapshot = Truncate(ex.ToString(), 8000),
                IsSuccess = false,
                StartedOn = now,
                CompletedOn = DateTime.Now,
                CreatedBy = userId
            });
            throw;
        }
    }

    [NonAction]
    public async Task<LucidMarketplaceOutcomeProcessResult> ProcessRespondentOutcomesPayloadAsync(
        string rawJson,
        string source,
        string? supplierCode,
        int? userId = null)
    {
        var result = new LucidMarketplaceOutcomeProcessResult();
        var effectiveSupplierCode = supplierCode?.Trim();
        var now = DateTime.Now;
        var legacyCompletionCandidates = new Dictionary<string, LucidMarketplaceLegacyCompletionCandidate>(StringComparer.OrdinalIgnoreCase);

        try
        {
            using var document = JsonDocument.Parse(rawJson, new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            });

            var outcomes = ExtractOutcomeElements(document.RootElement);
            if (outcomes.Count == 0)
            {
                await InsertLogAsync(new LucidMarketplaceSyncLogDTO
                {
                    ModuleName = "Lucid Marketplace",
                    ActionName = "OutcomesCallbackParsed",
                    Source = source,
                    SupplierCode = effectiveSupplierCode,
                    RequestBodySnapshot = Truncate(rawJson, 8000),
                    ResponseStatusCode = StatusCodes.Status200OK,
                    ResponseBodySnapshot = "No respondent outcomes were found in the callback payload.",
                    IsSuccess = true,
                    StartedOn = now,
                    CompletedOn = DateTime.Now,
                    CreatedBy = userId
                });

                return result;
            }

            foreach (var outcomeElement in outcomes)
            {
                try
                {
                    effectiveSupplierCode ??= GetStringValue(outcomeElement, "supplier_code", "supplierCode", "name");
                    if (string.IsNullOrWhiteSpace(effectiveSupplierCode))
                    {
                        var setting = await GetCurrentSettingEntityAsync(0);
                        effectiveSupplierCode = setting?.SupplierCode?.Trim();
                    }

                    var saveResult = await UpsertRespondentOutcomeAsync(outcomeElement, source, effectiveSupplierCode, userId);
                    result.ProcessedCount++;
                    result.OutcomeIds.Add(saveResult.OutcomeId);
                    if (saveResult.SurveyId.HasValue && saveResult.SurveyId.Value > 0)
                    {
                        result.SurveyIds.Add(saveResult.SurveyId.Value);
                    }

                    if (saveResult.AttemptId.HasValue && saveResult.AttemptId.Value > 0)
                    {
                        result.MatchedAttemptCount++;
                        result.AttemptIds.Add(saveResult.AttemptId.Value);

                        await InsertLogAsync(new LucidMarketplaceSyncLogDTO
                        {
                            ModuleName = "Lucid Marketplace",
                            ActionName = "OutcomeAttemptMatched",
                            Source = source,
                            SupplierCode = saveResult.SupplierCode,
                            RelatedEntityId = saveResult.RelatedOpportunityId ?? saveResult.OutcomeId,
                            RelatedSurveyId = saveResult.SurveyId,
                            RequestBodySnapshot = Truncate(outcomeElement.GetRawText(), 8000),
                            ResponseStatusCode = StatusCodes.Status200OK,
                            ResponseBodySnapshot = $"Matched attempt {saveResult.AttemptId.Value} using {saveResult.MatchType}.",
                            IsSuccess = true,
                            StartedOn = DateTime.Now,
                            CompletedOn = DateTime.Now,
                            CreatedBy = userId
                        });

                        if (saveResult.IsLatestOutcome)
                        {
                            result.UpdatedAttemptCount++;
                            await InsertLogAsync(new LucidMarketplaceSyncLogDTO
                            {
                                ModuleName = "Lucid Marketplace",
                                ActionName = "OutcomeAttemptFinalized",
                                Source = source,
                                SupplierCode = saveResult.SupplierCode,
                                RelatedEntityId = saveResult.RelatedOpportunityId ?? saveResult.OutcomeId,
                                RelatedSurveyId = saveResult.SurveyId,
                                RequestBodySnapshot = Truncate(outcomeElement.GetRawText(), 8000),
                                ResponseStatusCode = StatusCodes.Status200OK,
                                ResponseBodySnapshot = $"Attempt {saveResult.AttemptId.Value} final status updated to {saveResult.FinalStatus}.",
                                IsSuccess = true,
                                StartedOn = DateTime.Now,
                                CompletedOn = DateTime.Now,
                                CreatedBy = userId
                            });
                        }
                    }
                    else
                    {
                        await InsertLogAsync(new LucidMarketplaceSyncLogDTO
                        {
                            ModuleName = "Lucid Marketplace",
                            ActionName = "OutcomeAttemptMatchMissing",
                            Source = source,
                            SupplierCode = saveResult.SupplierCode,
                            RelatedEntityId = saveResult.RelatedOpportunityId ?? saveResult.OutcomeId,
                            RelatedSurveyId = saveResult.SurveyId,
                            RequestBodySnapshot = Truncate(outcomeElement.GetRawText(), 8000),
                            ResponseStatusCode = StatusCodes.Status404NotFound,
                            ErrorText = "No matching launch attempt was found for the async outcome.",
                            ResponseBodySnapshot = $"Outcome {saveResult.OutcomeId} stored without a matching attempt.",
                            IsSuccess = false,
                            StartedOn = DateTime.Now,
                            CompletedOn = DateTime.Now,
                            CreatedBy = userId
                        });
                    }

                    if (saveResult.IsLatestOutcome &&
                        !string.IsNullOrWhiteSpace(saveResult.LegacyProjectStatus) &&
                        !string.IsNullOrWhiteSpace(saveResult.InternalRespondentUid))
                    {
                        var candidateKey = saveResult.AttemptId.HasValue && saveResult.AttemptId.Value > 0
                            ? $"attempt:{saveResult.AttemptId.Value}"
                            : $"uid:{saveResult.InternalRespondentUid}|survey:{saveResult.SurveyId.GetValueOrDefault()}";

                        legacyCompletionCandidates[candidateKey] = new LucidMarketplaceLegacyCompletionCandidate
                        {
                            OutcomeId = saveResult.OutcomeId,
                            AttemptId = saveResult.AttemptId,
                            RelatedOpportunityId = saveResult.RelatedOpportunityId,
                            SurveyId = saveResult.SurveyId,
                            InternalRespondentUid = saveResult.InternalRespondentUid,
                            FinalStatus = saveResult.FinalStatus,
                            LegacyProjectStatus = saveResult.LegacyProjectStatus
                        };
                    }

                    await InsertLogAsync(new LucidMarketplaceSyncLogDTO
                    {
                        ModuleName = "Lucid Marketplace",
                        ActionName = "OutcomeSaveSuccess",
                        Source = source,
                        SupplierCode = saveResult.SupplierCode,
                        RelatedEntityId = saveResult.RelatedOpportunityId ?? saveResult.OutcomeId,
                        RelatedSurveyId = saveResult.SurveyId,
                        RequestBodySnapshot = Truncate(outcomeElement.GetRawText(), 8000),
                        ResponseStatusCode = StatusCodes.Status200OK,
                        ResponseBodySnapshot = $"Saved respondent outcome {saveResult.OutcomeId} with final status {saveResult.FinalStatus}.",
                        IsSuccess = true,
                        StartedOn = DateTime.Now,
                        CompletedOn = DateTime.Now,
                        CreatedBy = userId
                    });
                }
                catch (Exception ex)
                {
                    result.SkippedCount++;
                    result.Errors.Add(ex.Message);

                    await InsertLogAsync(new LucidMarketplaceSyncLogDTO
                    {
                        ModuleName = "Lucid Marketplace",
                        ActionName = "OutcomeSaveFailure",
                        Source = source,
                        SupplierCode = effectiveSupplierCode,
                        RequestBodySnapshot = Truncate(outcomeElement.GetRawText(), 8000),
                        ResponseStatusCode = StatusCodes.Status500InternalServerError,
                        ErrorText = ex.Message,
                        ResponseBodySnapshot = Truncate(ex.ToString(), 8000),
                        IsSuccess = false,
                        StartedOn = DateTime.Now,
                        CompletedOn = DateTime.Now,
                        CreatedBy = userId
                    });
                }
            }

            result.LegacyCompletionCandidates.AddRange(legacyCompletionCandidates.Values);
            return result;
        }
        catch (JsonException ex)
        {
            await InsertLogAsync(new LucidMarketplaceSyncLogDTO
            {
                ModuleName = "Lucid Marketplace",
                ActionName = "OutcomesCallbackParseFailure",
                Source = source,
                SupplierCode = effectiveSupplierCode,
                RequestBodySnapshot = Truncate(rawJson, 8000),
                ResponseStatusCode = StatusCodes.Status400BadRequest,
                ErrorText = ex.Message,
                ResponseBodySnapshot = Truncate(ex.ToString(), 8000),
                IsSuccess = false,
                StartedOn = now,
                CompletedOn = DateTime.Now,
                CreatedBy = userId
            });
            throw;
        }
    }

    [NonAction]
    public async Task<LucidMarketplaceLaunchContextDTO?> GetLucidMarketplaceLaunchContextAsync(
        int? lucidMarketplaceOpportunityId = null,
        int? internalProjectId = null,
        int? internalProjectMappingId = null)
    {
        const string query = @"
            SELECT TOP 1
                o.LucidMarketplaceOpportunityId,
                LucidSurveyId = o.SurveyId,
                o.SurveyNumber,
                o.SupplierCode,
                SurveyName = o.SurveyName,
                OpportunityRawJson = o.RawJson,
                InternalProjectId = COALESCE(pm.InternalProjectId, p.ProjectId),
                InternalProjectUrlId = COALESCE(pm.InternalProjectUrlId, puById.ID, puFallback.ID),
                InternalProjectMappingId = COALESCE(pm.InternalProjectMappingId, mapById.ID, mapFallback.ID),
                ProjectMappingSid = COALESCE(mapById.SID, mapFallback.SID),
                ProjectMappingCode = COALESCE(mapById.Code, mapFallback.Code),
                ProjectFrom = p.ProjectFrom,
                CountryId = p.CountryId,
                SupplierId = COALESCE(mapById.SUpplierID, mapFallback.SUpplierID, settingRow.DefaultSupplierId),
                settingRow.BaseUrl,
                settingRow.ApiKey,
                settingRow.EntryLinkSecretKey,
                settingRow.UseConsultingsBridge,
                settingRow.SupplierLinkTypeCode,
                settingRow.TrackingTypeCode,
                ExistingProjectUrl = COALESCE(puById.Url, puFallback.Url),
                ExistingMaskingUrl = COALESCE(mapById.MLink, mapFallback.MLink)
            FROM LucidMarketplaceOpportunity o
            OUTER APPLY (
                SELECT TOP 1 *
                FROM LucidMarketplaceProjectMap pm
                WHERE pm.LucidMarketplaceOpportunityId = o.LucidMarketplaceOpportunityId
                  AND pm.IsActive = 1
                  AND (@InternalProjectId IS NULL OR pm.InternalProjectId = @InternalProjectId)
                  AND (@InternalProjectMappingId IS NULL OR pm.InternalProjectMappingId = @InternalProjectMappingId)
                ORDER BY pm.Id DESC
            ) pm
            OUTER APPLY (
                SELECT TOP 1 p.*
                FROM Projects p
                WHERE p.ProjectFrom = 'LucidMarketplace'
                  AND (@InternalProjectId IS NULL OR p.ProjectId = @InternalProjectId)
                  AND (
                        p.ProjectId = COALESCE(pm.InternalProjectId, 0)
                        OR p.ProjectIdFromAPI = CAST(o.SurveyId AS nvarchar(100))
                      )
                ORDER BY CASE WHEN p.ProjectId = COALESCE(pm.InternalProjectId, 0) THEN 0 ELSE 1 END, p.ProjectId DESC
            ) p
            OUTER APPLY (
                SELECT TOP 1 *
                FROM ProjectsUrl pu
                WHERE pu.ID = pm.InternalProjectUrlId
            ) puById
            OUTER APPLY (
                SELECT TOP 1 *
                FROM ProjectsUrl pu
                WHERE pu.PID = COALESCE(pm.InternalProjectId, p.ProjectId)
                  AND (p.CountryId IS NULL OR pu.CID = p.CountryId)
                ORDER BY CASE WHEN pu.CID = p.CountryId THEN 0 ELSE 1 END, pu.ID DESC
            ) puFallback
            OUTER APPLY (
                SELECT TOP 1 *
                FROM ProjectMapping map
                WHERE map.ID = COALESCE(pm.InternalProjectMappingId, @InternalProjectMappingId)
            ) mapById
            OUTER APPLY (
                SELECT TOP 1 *
                FROM ProjectMapping map
                WHERE map.ProjectID = COALESCE(pm.InternalProjectId, p.ProjectId)
                  AND (@InternalProjectMappingId IS NULL OR map.ID = @InternalProjectMappingId OR map.ID = pm.InternalProjectMappingId)
                  AND (p.CountryId IS NULL OR map.CountryID = p.CountryId)
                ORDER BY CASE WHEN map.ID = @InternalProjectMappingId THEN 0 ELSE 1 END,
                         CASE WHEN map.ID = pm.InternalProjectMappingId THEN 0 ELSE 1 END,
                         CASE WHEN map.CountryID = p.CountryId THEN 0 ELSE 1 END,
                         map.ID DESC
            ) mapFallback
            OUTER APPLY (
                SELECT TOP 1 *
                FROM LucidMarketplaceSetting
                ORDER BY IsActive DESC, LucidMarketplaceSettingId DESC
            ) settingRow
            WHERE (@LucidMarketplaceOpportunityId IS NULL OR o.LucidMarketplaceOpportunityId = @LucidMarketplaceOpportunityId)
              AND (@InternalProjectId IS NULL OR COALESCE(pm.InternalProjectId, p.ProjectId) = @InternalProjectId)
              AND (@InternalProjectMappingId IS NULL OR COALESCE(pm.InternalProjectMappingId, mapById.ID, mapFallback.ID) = @InternalProjectMappingId)
            ORDER BY CASE WHEN COALESCE(pm.InternalProjectId, p.ProjectId) = @InternalProjectId THEN 0 ELSE 1 END,
                     CASE WHEN COALESCE(pm.InternalProjectMappingId, mapById.ID, mapFallback.ID) = @InternalProjectMappingId THEN 0 ELSE 1 END,
                     o.LucidMarketplaceOpportunityId DESC";

        return await _unitOfWork.LucidMarketplaceOpportunity.GetEntityData<LucidMarketplaceLaunchContextDTO>(query, new
        {
            LucidMarketplaceOpportunityId = lucidMarketplaceOpportunityId,
            InternalProjectId = internalProjectId,
            InternalProjectMappingId = internalProjectMappingId
        });
    }

    [NonAction]
    public async Task<LucidMarketplaceRespondentAttemptDTO?> GetLatestLucidMarketplaceRespondentAttemptAsync(
        int? internalProjectMappingId = null,
        int? lucidMarketplaceOpportunityId = null,
        string? sessionId = null,
        string? respondentId = null)
    {
        const string query = @"
            SELECT TOP 1 *
            FROM LucidMarketplaceRespondentAttempt
            WHERE (@InternalProjectMappingId IS NULL OR InternalProjectMappingId = @InternalProjectMappingId)
              AND (@LucidMarketplaceOpportunityId IS NULL OR LucidMarketplaceOpportunityId = @LucidMarketplaceOpportunityId)
              AND (@SessionId IS NULL OR SessionId = @SessionId)
              AND (@RespondentId IS NULL OR RespondentId = @RespondentId)
            ORDER BY
                CASE WHEN @SessionId IS NOT NULL AND SessionId = @SessionId THEN 0 ELSE 1 END,
                ISNULL(AsyncLastReceivedOn, ISNULL(AttemptedOn, CreatedDate)) DESC,
                Id DESC";

        return await _unitOfWork.LucidMarketplaceRespondentAttempt.GetEntityData<LucidMarketplaceRespondentAttemptDTO>(query, new
        {
            InternalProjectMappingId = internalProjectMappingId,
            LucidMarketplaceOpportunityId = lucidMarketplaceOpportunityId,
            SessionId = string.IsNullOrWhiteSpace(sessionId) ? null : sessionId.Trim(),
            RespondentId = string.IsNullOrWhiteSpace(respondentId) ? null : respondentId.Trim()
        });
    }

    [NonAction]
    public async Task<bool> SyncLucidMarketplaceProjectLaunchUrlAsync(
        int lucidMarketplaceOpportunityId,
        string launchUrl)
    {
        if (lucidMarketplaceOpportunityId <= 0 || string.IsNullOrWhiteSpace(launchUrl))
        {
            return false;
        }

        var context = await GetLucidMarketplaceLaunchContextAsync(lucidMarketplaceOpportunityId: lucidMarketplaceOpportunityId);
        if (context == null || !context.InternalProjectId.HasValue || context.InternalProjectId.Value <= 0)
        {
            return false;
        }

        return await SyncLucidMarketplaceProjectLaunchUrlByIdsAsync(
            lucidMarketplaceOpportunityId,
            context.InternalProjectId,
            context.InternalProjectUrlId,
            context.InternalProjectMappingId,
            context.CountryId,
            launchUrl);
    }

    [NonAction]
    public async Task<bool> SyncLucidMarketplaceProjectLaunchUrlByIdsAsync(
        int? lucidMarketplaceOpportunityId,
        int? internalProjectId,
        int? internalProjectUrlId,
        int? internalProjectMappingId,
        int? countryId,
        string launchUrl)
    {
        if (!internalProjectId.HasValue || internalProjectId.Value <= 0 || string.IsNullOrWhiteSpace(launchUrl))
        {
            return false;
        }

        var resolvedProjectUrl = internalProjectUrlId.HasValue && internalProjectUrlId.Value > 0
            ? await _unitOfWork.ProjectsUrl.FindByIdAsync(internalProjectUrlId.Value)
            : null;

        if (resolvedProjectUrl == null)
        {
            resolvedProjectUrl = await _unitOfWork.ProjectsUrl.GetEntityData<ProjectsUrl>(
                @"SELECT TOP 1 *
                  FROM ProjectsUrl
                  WHERE PID = @ProjectId
                    AND (@CountryId IS NULL OR CID = @CountryId)
                  ORDER BY ID DESC",
                new
                {
                    ProjectId = internalProjectId.Value,
                    CountryId = countryId
                });
        }

        var resolvedProjectMapping = internalProjectMappingId.HasValue && internalProjectMappingId.Value > 0
            ? await _unitOfWork.ProjectMapping.FindByIdAsync(internalProjectMappingId.Value)
            : null;

        if (resolvedProjectMapping == null)
        {
            resolvedProjectMapping = await _unitOfWork.ProjectMapping.GetEntityData<ProjectMapping>(
                @"SELECT TOP 1 *
                  FROM ProjectMapping
                  WHERE ProjectID = @ProjectId
                    AND (@CountryId IS NULL OR CountryID = @CountryId)
                  ORDER BY ID DESC",
                new
                {
                    ProjectId = internalProjectId.Value,
                    CountryId = countryId
                });
        }

        var updated = false;
        if (resolvedProjectUrl != null)
        {
            if (string.IsNullOrWhiteSpace(resolvedProjectUrl.OriginalUrl) &&
                !string.IsNullOrWhiteSpace(resolvedProjectUrl.Url) &&
                !string.Equals(resolvedProjectUrl.Url, launchUrl, StringComparison.OrdinalIgnoreCase))
            {
                resolvedProjectUrl.OriginalUrl = resolvedProjectUrl.Url;
            }

            resolvedProjectUrl.Url = launchUrl;
            updated = await _unitOfWork.ProjectsUrl.UpdateAsync(resolvedProjectUrl) || updated;
        }

        if (resolvedProjectMapping != null)
        {
            resolvedProjectMapping.Olink = launchUrl;
            updated = await _unitOfWork.ProjectMapping.UpdateAsync(resolvedProjectMapping) || updated;
        }

        var projectMapEntity = await _unitOfWork.LucidMarketplaceProjectMap.GetEntityData<LucidMarketplaceProjectMap>(
            @"SELECT TOP 1 *
              FROM LucidMarketplaceProjectMap
              WHERE LucidMarketplaceOpportunityId = @LucidMarketplaceOpportunityId
              ORDER BY Id DESC",
            new { LucidMarketplaceOpportunityId = lucidMarketplaceOpportunityId });

        if (projectMapEntity != null)
        {
            projectMapEntity.InternalProjectId = internalProjectId;

            if (resolvedProjectUrl != null)
            {
                projectMapEntity.InternalProjectUrlId = resolvedProjectUrl.Id;
            }

            if (resolvedProjectMapping != null)
            {
                projectMapEntity.InternalProjectMappingId = resolvedProjectMapping.Id;
            }

            updated = await _unitOfWork.LucidMarketplaceProjectMap.UpdateAsync(projectMapEntity) || updated;
        }

        return updated;
    }

    private async Task<LucidMarketplaceReconciliationBuildResult> BuildLucidMarketplaceReconciliationAsync(
        LucidMarketplaceReconciliationRunRequest request,
        int? createdBy,
        DateTime startedOn,
        string scopeJson)
    {
        var attempts = await GetScopedRespondentAttemptsAsync(request);
        var outcomeHistory = await GetScopedRespondentOutcomeHistoryAsync(request);
        var outcomeGroups = BuildOutcomeGroups(outcomeHistory);
        var usedOutcomeIds = new HashSet<int>();
        var items = new List<LucidMarketplaceReconciliationItemDTO>();
        var now = DateTime.Now;

        foreach (var attempt in attempts
                     .OrderByDescending(x => x.AsyncLastReceivedOn ?? x.AttemptedOn ?? x.CreatedDate)
                     .ThenByDescending(x => x.Id))
        {
            var matchedGroup = FindBestOutcomeGroup(attempt, outcomeGroups, usedOutcomeIds);
            if (matchedGroup != null)
            {
                usedOutcomeIds.Add(matchedGroup.LatestOutcome.Id);
            }

            items.Add(BuildReconciliationItem(
                request,
                attempt,
                matchedGroup?.LatestOutcome,
                matchedGroup?.OutcomeChangedOverTime ?? false,
                matchedGroup?.MatchType,
                now));
        }

        foreach (var unmatchedGroup in outcomeGroups
                     .Where(group => !usedOutcomeIds.Contains(group.LatestOutcome.Id))
                     .OrderByDescending(group => group.LatestOutcome.ReceivedOn ?? group.LatestOutcome.CreatedDate)
                     .ThenByDescending(group => group.LatestOutcome.Id))
        {
            items.Add(BuildReconciliationItem(
                request,
                null,
                unmatchedGroup.LatestOutcome,
                unmatchedGroup.OutcomeChangedOverTime,
                "OutcomeOnly",
                now));
        }

        var run = new LucidMarketplaceReconciliationRunDTO
        {
            RunType = request.RunType,
            SupplierCode = request.SupplierCode,
            LucidSurveyId = request.LucidSurveyId,
            InternalProjectId = request.InternalProjectId,
            RunScopeJson = scopeJson,
            StartedOn = startedOn,
            CompletedOn = now,
            Success = true,
            Notes = BuildReconciliationRunNote(request, items.Count),
            TotalReviewed = items.Count,
            TotalMatched = items.Count(item => !item.IsMismatch),
            TotalMismatched = items.Count(item => item.IsMismatch),
            CompleteCount = items.Count(item => string.Equals(item.FinalStatus, "Complete", StringComparison.OrdinalIgnoreCase)),
            TerminateCount = items.Count(item => string.Equals(item.FinalStatus, "Terminate", StringComparison.OrdinalIgnoreCase)),
            OverQuotaCount = items.Count(item => string.Equals(item.FinalStatus, "OverQuota", StringComparison.OrdinalIgnoreCase)),
            QualityTerminationCount = items.Count(item => string.Equals(item.FinalStatus, "QualityTermination", StringComparison.OrdinalIgnoreCase)),
            DuplicateCount = items.Count(item => string.Equals(item.FinalStatus, "Duplicate", StringComparison.OrdinalIgnoreCase)),
            SecurityTerminationCount = items.Count(item => string.Equals(item.FinalStatus, "SecurityTermination", StringComparison.OrdinalIgnoreCase)),
            OpenCount = items.Count(item => string.Equals(item.FinalStatus, "Open", StringComparison.OrdinalIgnoreCase)),
            UnknownCount = items.Count(item => string.Equals(item.FinalStatus, "Unknown", StringComparison.OrdinalIgnoreCase)),
            CreatedBy = createdBy,
            CreatedDate = startedOn
        };

        return new LucidMarketplaceReconciliationBuildResult
        {
            Run = run,
            Items = items
        };
    }

    private async Task<List<LucidMarketplaceRespondentAttemptDTO>> GetScopedRespondentAttemptsAsync(LucidMarketplaceReconciliationRunRequest request)
    {
        const string query = @"
            SELECT *
            FROM LucidMarketplaceRespondentAttempt
            WHERE (@SupplierCode IS NULL OR ISNULL(SupplierCode, '') = @SupplierCode)
              AND (@LucidSurveyId IS NULL OR LucidSurveyId = @LucidSurveyId)
              AND (@InternalProjectId IS NULL OR InternalProjectId = @InternalProjectId)
              AND (@DateFrom IS NULL OR ISNULL(AttemptedOn, CreatedDate) >= @DateFrom)
              AND (@DateToExclusive IS NULL OR ISNULL(AttemptedOn, CreatedDate) < @DateToExclusive)
            ORDER BY ISNULL(AsyncLastReceivedOn, ISNULL(AttemptedOn, CreatedDate)) DESC, Id DESC";

        var items = await _unitOfWork.LucidMarketplaceRespondentAttempt.GetTableData<LucidMarketplaceRespondentAttemptDTO>(query, new
        {
            request.SupplierCode,
            request.LucidSurveyId,
            request.InternalProjectId,
            DateFrom = request.DateFrom,
            DateToExclusive = GetDateToExclusive(request.DateTo)
        });

        return items ?? new List<LucidMarketplaceRespondentAttemptDTO>();
    }

    private async Task<List<LucidMarketplaceRespondentOutcomeDTO>> GetScopedRespondentOutcomeHistoryAsync(LucidMarketplaceReconciliationRunRequest request)
    {
        const string query = @"
            SELECT
                o.*,
                AttemptMatched = CAST(CASE WHEN ISNULL(o.RelatedAttemptId, 0) > 0 THEN 1 ELSE 0 END AS bit)
            FROM LucidMarketplaceRespondentOutcome o
            WHERE (@SupplierCode IS NULL OR ISNULL(o.SupplierCode, '') = @SupplierCode)
              AND (@LucidSurveyId IS NULL OR o.SurveyId = @LucidSurveyId)
              AND (@InternalProjectId IS NULL OR o.RelatedInternalProjectId = @InternalProjectId)
              AND (@DateFrom IS NULL OR ISNULL(o.ReceivedOn, o.CreatedDate) >= @DateFrom)
              AND (@DateToExclusive IS NULL OR ISNULL(o.ReceivedOn, o.CreatedDate) < @DateToExclusive)
            ORDER BY ISNULL(o.ReceivedOn, o.CreatedDate) DESC, o.Id DESC";

        var items = await _unitOfWork.LucidMarketplaceRespondentOutcome.GetTableData<LucidMarketplaceRespondentOutcomeDTO>(query, new
        {
            request.SupplierCode,
            request.LucidSurveyId,
            request.InternalProjectId,
            DateFrom = request.DateFrom,
            DateToExclusive = GetDateToExclusive(request.DateTo)
        });

        return items ?? new List<LucidMarketplaceRespondentOutcomeDTO>();
    }

    private static List<LucidMarketplaceOutcomeGroup> BuildOutcomeGroups(IEnumerable<LucidMarketplaceRespondentOutcomeDTO> outcomeHistory)
    {
        var grouped = outcomeHistory
            .GroupBy(outcome => BuildOutcomeGroupKey(outcome))
            .Where(group => !string.IsNullOrWhiteSpace(group.Key))
            .Select(group =>
            {
                var ordered = group
                    .OrderByDescending(item => item.IsLatest)
                    .ThenByDescending(item => item.ReceivedOn ?? item.CreatedDate)
                    .ThenByDescending(item => item.Id)
                    .ToList();

                var latest = ordered.First();
                var changed = ordered
                    .Select(item => item.FinalStatus?.Trim() ?? string.Empty)
                    .Where(status => !string.IsNullOrWhiteSpace(status))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Count() > 1;

                return new LucidMarketplaceOutcomeGroup
                {
                    LatestOutcome = latest,
                    OutcomeChangedOverTime = changed
                };
            })
            .ToList();

        return grouped;
    }

    private static LucidMarketplaceOutcomeGroup? FindBestOutcomeGroup(
        LucidMarketplaceRespondentAttemptDTO attempt,
        IReadOnlyCollection<LucidMarketplaceOutcomeGroup> outcomeGroups,
        ISet<int> usedOutcomeIds)
    {
        var available = outcomeGroups
            .Where(group => !usedOutcomeIds.Contains(group.LatestOutcome.Id))
            .ToList();

        if (!string.IsNullOrWhiteSpace(attempt.SessionId))
        {
            var bySession = OrderOutcomeGroupsForAttempt(
                available.Where(group => string.Equals(group.LatestOutcome.SessionId, attempt.SessionId?.Trim(), StringComparison.OrdinalIgnoreCase)),
                attempt);
            if (bySession.Any())
            {
                var match = bySession.First();
                match.MatchType = "SessionId";
                return match;
            }
        }

        if (!string.IsNullOrWhiteSpace(attempt.RespondentId))
        {
            var byRespondent = OrderOutcomeGroupsForAttempt(
                available.Where(group => string.Equals(group.LatestOutcome.RespondentId, attempt.RespondentId?.Trim(), StringComparison.OrdinalIgnoreCase)),
                attempt);
            if (byRespondent.Any())
            {
                var match = byRespondent.First();
                match.MatchType = "RespondentId";
                return match;
            }
        }

        if (!string.IsNullOrWhiteSpace(attempt.PanelistId) && attempt.LucidSurveyId > 0)
        {
            var byPanelist = OrderOutcomeGroupsForAttempt(
                available.Where(group =>
                    string.Equals(group.LatestOutcome.PanelistId, attempt.PanelistId?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    group.LatestOutcome.SurveyId == attempt.LucidSurveyId),
                attempt);
            if (byPanelist.Any())
            {
                var match = byPanelist.First();
                match.MatchType = "PanelistId+SurveyId";
                return match;
            }
        }

        return null;
    }

    private static IOrderedEnumerable<LucidMarketplaceOutcomeGroup> OrderOutcomeGroupsForAttempt(
        IEnumerable<LucidMarketplaceOutcomeGroup> groups,
        LucidMarketplaceRespondentAttemptDTO attempt)
    {
        return groups
            .OrderBy(group => group.LatestOutcome.SurveyId == attempt.LucidSurveyId ? 0 : 1)
            .ThenBy(group => group.LatestOutcome.RelatedInternalProjectId == attempt.InternalProjectId ? 0 : 1)
            .ThenByDescending(group => group.LatestOutcome.ReceivedOn ?? group.LatestOutcome.CreatedDate)
            .ThenByDescending(group => group.LatestOutcome.Id);
    }

    private static LucidMarketplaceReconciliationItemDTO BuildReconciliationItem(
        LucidMarketplaceReconciliationRunRequest request,
        LucidMarketplaceRespondentAttemptDTO? attempt,
        LucidMarketplaceRespondentOutcomeDTO? outcome,
        bool outcomeChangedOverTime,
        string? matchType,
        DateTime createdOn)
    {
        var resolution = LucidMarketplaceReconciliationHelper.ResolveFinalStatus(attempt, outcome);
        var mismatch = LucidMarketplaceReconciliationHelper.DetermineMismatch(attempt, outcome, outcomeChangedOverTime, createdOn);
        var notes = BuildReconciliationItemNotes(attempt, outcome, mismatch, matchType);

        return new LucidMarketplaceReconciliationItemDTO
        {
            LucidMarketplaceRespondentAttemptId = attempt?.Id,
            LucidMarketplaceRespondentOutcomeId = outcome?.Id,
            LucidSurveyId = outcome?.SurveyId ?? (attempt != null && attempt.LucidSurveyId > 0 ? attempt.LucidSurveyId : null),
            InternalProjectId = attempt?.InternalProjectId ?? outcome?.RelatedInternalProjectId,
            SessionId = FirstNonEmpty(outcome?.SessionId, attempt?.SessionId),
            RespondentId = FirstNonEmpty(outcome?.RespondentId, attempt?.RespondentId),
            PanelistId = FirstNonEmpty(outcome?.PanelistId, attempt?.PanelistId),
            RedirectStatus = attempt?.ReturnStatus,
            RedirectCode = attempt?.ReturnCode,
            OutcomeMarketplaceStatus = outcome?.MarketplaceStatus,
            OutcomeClientStatus = outcome?.ClientStatus,
            FinalStatus = resolution.FinalStatus,
            FinalStatusSource = resolution.FinalStatusSource,
            IsMismatch = mismatch.IsMismatch,
            MismatchType = mismatch.MismatchType,
            RevenueValue = outcome?.RevenueValue ?? attempt?.RevenueValue,
            RevenueCurrencyCode = FirstNonEmpty(outcome?.RevenueCurrencyCode, attempt?.RevenueCurrencyCode),
            Notes = Truncate(notes, 1000),
            RawSnapshotJson = JsonSerializer.Serialize(new
            {
                request.RunType,
                request.SupplierCode,
                request.LucidSurveyId,
                request.InternalProjectId,
                MatchType = matchType,
                RedirectFinalStatus = resolution.RedirectFinalStatus,
                OutcomeFinalStatus = resolution.OutcomeFinalStatus,
                attempt,
                outcome
            }),
            CreatedDate = createdOn
        };
    }

    private async Task<int> PersistReconciliationRunAsync(
        LucidMarketplaceReconciliationRunDTO run,
        IReadOnlyCollection<LucidMarketplaceReconciliationItemDTO> items)
    {
        using var connection = await CreateOpenConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string insertRunSql = @"
                INSERT INTO LucidMarketplaceReconciliationRun
                (
                    RunType, SupplierCode, LucidSurveyId, InternalProjectId, RunScopeJson,
                    StartedOn, CompletedOn, Success, Notes, TotalReviewed, TotalMatched, TotalMismatched,
                    CompleteCount, TerminateCount, OverQuotaCount, QualityTerminationCount, DuplicateCount,
                    SecurityTerminationCount, OpenCount, UnknownCount, CreatedBy, CreatedDate
                )
                VALUES
                (
                    @RunType, @SupplierCode, @LucidSurveyId, @InternalProjectId, @RunScopeJson,
                    @StartedOn, @CompletedOn, @Success, @Notes, @TotalReviewed, @TotalMatched, @TotalMismatched,
                    @CompleteCount, @TerminateCount, @OverQuotaCount, @QualityTerminationCount, @DuplicateCount,
                    @SecurityTerminationCount, @OpenCount, @UnknownCount, @CreatedBy, @CreatedDate
                );
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            var runId = await connection.ExecuteScalarAsync<int>(insertRunSql, run, transaction);

            if (items.Count > 0)
            {
                const string insertItemSql = @"
                    INSERT INTO LucidMarketplaceReconciliationItem
                    (
                        ReconciliationRunId, LucidMarketplaceRespondentAttemptId, LucidMarketplaceRespondentOutcomeId,
                        LucidSurveyId, InternalProjectId, SessionId, RespondentId, PanelistId, RedirectStatus,
                        RedirectCode, OutcomeMarketplaceStatus, OutcomeClientStatus, FinalStatus, FinalStatusSource,
                        IsMismatch, MismatchType, RevenueValue, RevenueCurrencyCode, Notes, RawSnapshotJson, CreatedDate
                    )
                    VALUES
                    (
                        @ReconciliationRunId, @LucidMarketplaceRespondentAttemptId, @LucidMarketplaceRespondentOutcomeId,
                        @LucidSurveyId, @InternalProjectId, @SessionId, @RespondentId, @PanelistId, @RedirectStatus,
                        @RedirectCode, @OutcomeMarketplaceStatus, @OutcomeClientStatus, @FinalStatus, @FinalStatusSource,
                        @IsMismatch, @MismatchType, @RevenueValue, @RevenueCurrencyCode, @Notes, @RawSnapshotJson, @CreatedDate
                    );";

                foreach (var item in items)
                {
                    item.ReconciliationRunId = runId;
                    await connection.ExecuteAsync(insertItemSql, item, transaction);
                }
            }

            transaction.Commit();
            return runId;
        }
        catch
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // ignored
            }

            throw;
        }
    }

    private static LucidMarketplaceReconciliationRunDTO BuildFailedReconciliationRun(
        LucidMarketplaceReconciliationRunRequest request,
        int? createdBy,
        DateTime startedOn,
        string scopeJson,
        string errorText)
    {
        return new LucidMarketplaceReconciliationRunDTO
        {
            RunType = request.RunType,
            SupplierCode = request.SupplierCode,
            LucidSurveyId = request.LucidSurveyId,
            InternalProjectId = request.InternalProjectId,
            RunScopeJson = scopeJson,
            StartedOn = startedOn,
            CompletedOn = DateTime.Now,
            Success = false,
            Notes = Truncate(errorText, 1000),
            CreatedBy = createdBy,
            CreatedDate = startedOn
        };
    }

    private static LucidMarketplaceReconciliationRunRequest NormalizeReconciliationRequest(LucidMarketplaceReconciliationRunRequest? inputData)
    {
        var now = DateTime.Now;
        var normalized = new LucidMarketplaceReconciliationRunRequest
        {
            SupplierCode = string.IsNullOrWhiteSpace(inputData?.SupplierCode) ? null : inputData.SupplierCode.Trim(),
            LucidSurveyId = inputData?.LucidSurveyId > 0 ? inputData.LucidSurveyId : null,
            InternalProjectId = inputData?.InternalProjectId > 0 ? inputData.InternalProjectId : null,
            DateFrom = inputData?.DateFrom?.Date,
            DateTo = inputData?.DateTo?.Date
        };

        normalized.RunType = normalized.InternalProjectId.HasValue
            ? "Project"
            : normalized.LucidSurveyId.HasValue
                ? "Survey"
                : normalized.DateFrom.HasValue || normalized.DateTo.HasValue
                    ? "DateRange"
                    : string.Equals(inputData?.RunType, "Scheduled", StringComparison.OrdinalIgnoreCase)
                        ? "Scheduled"
                        : "Manual";

        if (!normalized.DateFrom.HasValue &&
            !normalized.DateTo.HasValue &&
            !normalized.LucidSurveyId.HasValue &&
            !normalized.InternalProjectId.HasValue)
        {
            normalized.DateFrom = now.Date.AddDays(-7);
            normalized.DateTo = now.Date;
        }

        if (normalized.DateFrom.HasValue && normalized.DateTo.HasValue && normalized.DateFrom > normalized.DateTo)
        {
            (normalized.DateFrom, normalized.DateTo) = (normalized.DateTo, normalized.DateFrom);
        }

        return normalized;
    }

    private static DateTime? GetDateToExclusive(DateTime? dateTo)
    {
        return dateTo?.Date.AddDays(1);
    }

    private static string BuildReconciliationRunNote(LucidMarketplaceReconciliationRunRequest request, int totalReviewed)
    {
        if (request.InternalProjectId.HasValue)
        {
            return $"Project reconciliation reviewed {totalReviewed} respondent/session records for internal project {request.InternalProjectId.Value}.";
        }

        if (request.LucidSurveyId.HasValue)
        {
            return $"Survey reconciliation reviewed {totalReviewed} respondent/session records for Lucid survey {request.LucidSurveyId.Value}.";
        }

        if (request.DateFrom.HasValue || request.DateTo.HasValue)
        {
            return $"Date-range reconciliation reviewed {totalReviewed} respondent/session records.";
        }

        return $"Recent reconciliation reviewed {totalReviewed} respondent/session records.";
    }

    private static string BuildReconciliationItemNotes(
        LucidMarketplaceRespondentAttemptDTO? attempt,
        LucidMarketplaceRespondentOutcomeDTO? outcome,
        LucidMarketplaceMismatchResult mismatch,
        string? matchType)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(matchType))
        {
            parts.Add($"Matched by {matchType}.");
        }

        if (attempt == null && outcome != null)
        {
            parts.Add("Stored only from async outcomes feed.");
        }
        else if (attempt != null && outcome == null)
        {
            parts.Add("Stored only from browser redirect / launch attempt history.");
        }

        if (!string.IsNullOrWhiteSpace(mismatch.Notes))
        {
            parts.Add(mismatch.Notes);
        }

        return string.Join(" ", parts.Where(part => !string.IsNullOrWhiteSpace(part)));
    }

    private static string BuildOutcomeGroupKey(LucidMarketplaceRespondentOutcomeDTO outcome)
    {
        var supplierCode = (outcome.SupplierCode ?? string.Empty).Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(outcome.SessionId))
        {
            return $"S|{supplierCode}|{outcome.SessionId.Trim().ToLowerInvariant()}";
        }

        if (!string.IsNullOrWhiteSpace(outcome.RespondentId))
        {
            return $"R|{supplierCode}|{outcome.RespondentId.Trim().ToLowerInvariant()}|{outcome.SurveyId.GetValueOrDefault()}";
        }

        if (!string.IsNullOrWhiteSpace(outcome.PanelistId) && outcome.SurveyId.HasValue && outcome.SurveyId.Value > 0)
        {
            return $"P|{supplierCode}|{outcome.PanelistId.Trim().ToLowerInvariant()}|{outcome.SurveyId.Value}";
        }

        return string.Empty;
    }

    private static string? FirstNonEmpty(params string?[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
    }

    private async Task<LucidMarketplaceSetting?> GetCurrentSettingEntityAsync(int requestedId)
    {
        if (requestedId > 0)
        {
            return await _unitOfWork.LucidMarketplaceSetting.FindByIdAsync(requestedId);
        }

        const string query = @"SELECT TOP 1 *
                               FROM LucidMarketplaceSetting
                               ORDER BY IsActive DESC, LucidMarketplaceSettingId DESC";

        return await _unitOfWork.LucidMarketplaceSetting.GetEntityData<LucidMarketplaceSetting>(query);
    }

    private async Task<LucidMarketplaceSubscription?> GetCurrentSubscriptionEntityAsync(
        int requestedId,
        string? subscriptionType,
        string? supplierCode)
    {
        if (requestedId > 0)
        {
            return await _unitOfWork.LucidMarketplaceSubscription.FindByIdAsync(requestedId);
        }

        if (string.IsNullOrWhiteSpace(subscriptionType) || string.IsNullOrWhiteSpace(supplierCode))
        {
            return null;
        }

        const string query = @"SELECT TOP 1 *
                               FROM LucidMarketplaceSubscription
                               WHERE SubscriptionType = @SubscriptionType
                                 AND SupplierCode = @SupplierCode
                               ORDER BY IsActive DESC, LucidMarketplaceSubscriptionId DESC";

        return await _unitOfWork.LucidMarketplaceSubscription.GetEntityData<LucidMarketplaceSubscription>(query, new
        {
            SubscriptionType = subscriptionType.Trim(),
            SupplierCode = supplierCode.Trim()
        });
    }

    private static LucidMarketplaceSubscriptionDTO? BuildSubscriptionMetadataSnapshot(LucidMarketplaceSubscription? subscription)
    {
        if (subscription == null)
        {
            return null;
        }

        return new LucidMarketplaceSubscriptionDTO
        {
            LucidMarketplaceSubscriptionId = subscription.LucidMarketplaceSubscriptionId,
            SubscriptionType = subscription.SubscriptionType,
            SupplierCode = subscription.SupplierCode,
            ResponsePayloadSnapshot = subscription.ResponsePayloadSnapshot,
            WebhookKeyId = subscription.WebhookKeyId,
            WebhookKeyIdFull = subscription.WebhookKeyIdFull,
            WebhookPublicKey = subscription.WebhookPublicKey,
            WebhookSecuritySnapshot = subscription.WebhookSecuritySnapshot,
            IsActive = subscription.IsActive,
            LastValidatedOn = subscription.LastValidatedOn,
            CreatedDate = subscription.CreatedDate,
            ModifiedDate = subscription.ModifiedDate
        };
    }

    private async Task<LucidMarketplaceEntryLink?> GetCurrentEntryLinkEntityAsync(int lucidMarketplaceOpportunityId)
    {
        const string query = @"SELECT TOP 1 *
                               FROM LucidMarketplaceEntryLink
                               WHERE LucidMarketplaceOpportunityId = @LucidMarketplaceOpportunityId
                               ORDER BY IsActive DESC, ISNULL(ModifiedDate, CreatedDate) DESC, Id DESC";

        return await _unitOfWork.LucidMarketplaceEntryLink.GetEntityData<LucidMarketplaceEntryLink>(query, new
        {
            LucidMarketplaceOpportunityId = lucidMarketplaceOpportunityId
        });
    }

    private async Task<LucidMarketplaceOutcomeSaveContext> UpsertRespondentOutcomeAsync(
        JsonElement outcomeElement,
        string source,
        string? supplierCode,
        int? userId)
    {
        if (string.IsNullOrWhiteSpace(supplierCode))
        {
            throw new InvalidOperationException("SupplierCode could not be determined for the respondent outcome payload.");
        }

        var normalizedSupplierCode = supplierCode.Trim();
        var respondentId = GetStringValue(outcomeElement, "respondent_id", "respondentId")?.Trim();
        var parentSessionId = GetStringValue(outcomeElement, "parent_session_id", "parentSessionId")?.Trim();
        var panelistId = GetStringValue(outcomeElement, "panelist_id", "panelistId")?.Trim();
        var sessionId = GetStringValue(outcomeElement, "session_id", "sessionId", "mid")?.Trim();
        var marketplaceStatus = GetIntValue(outcomeElement, "marketplace_status", "marketplaceStatus");
        var clientStatus = GetIntValue(outcomeElement, "client_status", "clientStatus");
        var surveyId = GetIntValue(outcomeElement, "survey_id", "surveyId");
        var receivedOn = DateTime.Now;
        var statusInfo = LucidMarketplaceOutcomeStatusHelper.Resolve(marketplaceStatus, clientStatus);

        var relatedContext = await GetOutcomeRelatedContextAsync(normalizedSupplierCode, surveyId);
        var (matchedAttempt, matchType) = await FindMatchingRespondentAttemptAsync(
            normalizedSupplierCode,
            sessionId,
            respondentId,
            panelistId,
            surveyId);

        if (matchedAttempt != null)
        {
            relatedContext = new LucidMarketplaceOutcomeRelationshipContext
            {
                LucidMarketplaceOpportunityId = matchedAttempt.LucidMarketplaceOpportunityId,
                InternalProjectId = matchedAttempt.InternalProjectId
            };
        }

        var outcomeEntity = new LucidMarketplaceRespondentOutcome
        {
            SupplierCode = normalizedSupplierCode,
            RespondentId = respondentId,
            ParentSessionId = parentSessionId,
            PanelistId = panelistId,
            SessionId = sessionId,
            MarketplaceStatus = marketplaceStatus,
            ClientStatus = clientStatus,
            EntryDate = GetDateTimeValue(outcomeElement, "entry_date", "entryDate"),
            LastDate = GetDateTimeValue(outcomeElement, "last_date", "lastDate"),
            SurveyId = surveyId,
            RevenueValue = GetMonetaryValue(outcomeElement, "revenue_per_interview", "revenuePerInterview"),
            RevenueCurrencyCode = GetMonetaryCurrency(outcomeElement, "revenue_per_interview", "revenuePerInterview"),
            StudyType = GetStringValue(outcomeElement, "study_type", "studyType"),
            BuyerId = GetIntValue(outcomeElement, "buyer_id", "buyerId"),
            ProofCostPerInterview = GetDecimalValue(outcomeElement, "proof_cost_per_interview", "proofCostPerInterview"),
            FinalStatus = statusInfo.FinalStatus,
            RawJson = outcomeElement.GetRawText(),
            Source = source,
            ReceivedOn = receivedOn,
            IsLatest = true,
            RelatedAttemptId = matchedAttempt?.Id,
            RelatedOpportunityId = relatedContext.LucidMarketplaceOpportunityId,
            RelatedInternalProjectId = matchedAttempt?.InternalProjectId ?? relatedContext.InternalProjectId,
            CreatedDate = receivedOn,
            ModifiedDate = receivedOn
        };

        using var connection = await CreateOpenConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var existingLatestOutcome = await GetExistingLatestOutcomeAsync(
                connection,
                transaction,
                normalizedSupplierCode,
                sessionId,
                respondentId,
                panelistId,
                surveyId);

            var incomingOutcomeEventOn = GetOutcomeEventDate(outcomeEntity.EntryDate, outcomeEntity.LastDate, outcomeEntity.ReceivedOn, outcomeEntity.CreatedDate);
            var existingLatestOutcomeEventOn = GetOutcomeEventDate(
                existingLatestOutcome?.EntryDate,
                existingLatestOutcome?.LastDate,
                existingLatestOutcome?.ReceivedOn,
                existingLatestOutcome?.CreatedDate);

            var isLatestOutcome = existingLatestOutcome == null || incomingOutcomeEventOn >= existingLatestOutcomeEventOn;
            var isDuplicateLatest = existingLatestOutcome != null && IsEquivalentOutcome(existingLatestOutcome, outcomeEntity);

            int outcomeId;
            if (isDuplicateLatest)
            {
                outcomeEntity.IsLatest = true;
                outcomeId = existingLatestOutcome!.Id;
                await UpdateRespondentOutcomeAsync(connection, transaction, outcomeId, outcomeEntity);
            }
            else
            {
                outcomeEntity.IsLatest = isLatestOutcome;

                if (isLatestOutcome)
                {
                    await ResetPreviousLatestOutcomeAsync(
                        connection,
                        transaction,
                        normalizedSupplierCode,
                        sessionId,
                        respondentId,
                        panelistId,
                        surveyId,
                        receivedOn);
                }

                outcomeId = await InsertRespondentOutcomeAsync(connection, transaction, outcomeEntity);
            }

            if (matchedAttempt != null && isLatestOutcome)
            {
                var mergedAttempt = BuildMergedAttemptFromOutcome(matchedAttempt, outcomeEntity, outcomeId, statusInfo, relatedContext, receivedOn);
                await UpdateRespondentAttemptAsync(connection, transaction, mergedAttempt);
            }

            transaction.Commit();

            return new LucidMarketplaceOutcomeSaveContext
            {
                OutcomeId = outcomeId,
                AttemptId = matchedAttempt?.Id,
                RelatedOpportunityId = relatedContext.LucidMarketplaceOpportunityId,
                SupplierCode = normalizedSupplierCode,
                SurveyId = surveyId,
                FinalStatus = statusInfo.FinalStatus,
                MatchType = matchType,
                InternalRespondentUid = matchedAttempt?.RespondentId ?? respondentId,
                LegacyProjectStatus = isLatestOutcome
                    ? LucidMarketplaceReconciliationHelper.MapLucidFinalStatusToInternalStatus(statusInfo.FinalStatus)
                    : null,
                IsLatestOutcome = isLatestOutcome
            };
        }
        catch
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // ignored
            }

            throw;
        }
    }

    private async Task<LucidMarketplaceOutcomeRelationshipContext> GetOutcomeRelatedContextAsync(string supplierCode, int? surveyId)
    {
        if (string.IsNullOrWhiteSpace(supplierCode) || !surveyId.HasValue || surveyId.Value <= 0)
        {
            return new LucidMarketplaceOutcomeRelationshipContext();
        }

        const string query = @"
            SELECT TOP 1
                LucidMarketplaceOpportunityId = o.LucidMarketplaceOpportunityId,
                InternalProjectId = COALESCE(pm.InternalProjectId, p.ProjectId)
            FROM LucidMarketplaceOpportunity o
            OUTER APPLY (
                SELECT TOP 1 *
                FROM LucidMarketplaceProjectMap pm
                WHERE pm.LucidMarketplaceOpportunityId = o.LucidMarketplaceOpportunityId
                  AND pm.IsActive = 1
                ORDER BY pm.Id DESC
            ) pm
            OUTER APPLY (
                SELECT TOP 1 p.ProjectId
                FROM Projects p
                WHERE p.ProjectIdFromAPI = CAST(o.SurveyId AS nvarchar(100))
                  AND p.ProjectFrom = 'LucidMarketplace'
                  AND p.IsActive = 1
                ORDER BY p.ProjectId DESC
            ) p
            WHERE o.SupplierCode = @SupplierCode
              AND o.SurveyId = @SurveyId
            ORDER BY o.LucidMarketplaceOpportunityId DESC";

        return await _unitOfWork.LucidMarketplaceOpportunity.GetEntityData<LucidMarketplaceOutcomeRelationshipContext>(query, new
        {
            SupplierCode = supplierCode,
            SurveyId = surveyId.Value
        }) ?? new LucidMarketplaceOutcomeRelationshipContext();
    }

    private async Task<(LucidMarketplaceRespondentAttemptDTO? Attempt, string? MatchType)> FindMatchingRespondentAttemptAsync(
        string supplierCode,
        string? sessionId,
        string? respondentId,
        string? panelistId,
        int? surveyId)
    {
        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            const string query = @"
                SELECT TOP 1 *
                FROM LucidMarketplaceRespondentAttempt
                WHERE SessionId = @SessionId
                  AND (@SupplierCode IS NULL OR SupplierCode = @SupplierCode)
                ORDER BY
                    CASE WHEN @SurveyId IS NOT NULL AND LucidSurveyId = @SurveyId THEN 0 ELSE 1 END,
                    ISNULL(AsyncLastReceivedOn, ISNULL(AttemptedOn, CreatedDate)) DESC,
                    Id DESC";

            var attempt = await _unitOfWork.LucidMarketplaceRespondentAttempt.GetEntityData<LucidMarketplaceRespondentAttemptDTO>(query, new
            {
                SessionId = sessionId.Trim(),
                SupplierCode = string.IsNullOrWhiteSpace(supplierCode) ? null : supplierCode,
                SurveyId = surveyId
            });

            if (attempt != null)
            {
                return (attempt, "SessionId");
            }
        }

        if (!string.IsNullOrWhiteSpace(respondentId))
        {
            const string query = @"
                SELECT TOP 1 *
                FROM LucidMarketplaceRespondentAttempt
                WHERE RespondentId = @RespondentId
                  AND (@SupplierCode IS NULL OR SupplierCode = @SupplierCode)
                ORDER BY
                    CASE WHEN @SurveyId IS NOT NULL AND LucidSurveyId = @SurveyId THEN 0 ELSE 1 END,
                    ISNULL(AsyncLastReceivedOn, ISNULL(AttemptedOn, CreatedDate)) DESC,
                    Id DESC";

            var attempt = await _unitOfWork.LucidMarketplaceRespondentAttempt.GetEntityData<LucidMarketplaceRespondentAttemptDTO>(query, new
            {
                RespondentId = respondentId.Trim(),
                SupplierCode = string.IsNullOrWhiteSpace(supplierCode) ? null : supplierCode,
                SurveyId = surveyId
            });

            if (attempt != null)
            {
                return (attempt, "RespondentId");
            }
        }

        if (!string.IsNullOrWhiteSpace(panelistId) && surveyId.HasValue && surveyId.Value > 0)
        {
            const string query = @"
                SELECT TOP 1 *
                FROM LucidMarketplaceRespondentAttempt
                WHERE PanelistId = @PanelistId
                  AND LucidSurveyId = @SurveyId
                  AND (@SupplierCode IS NULL OR SupplierCode = @SupplierCode)
                ORDER BY ISNULL(AsyncLastReceivedOn, ISNULL(AttemptedOn, CreatedDate)) DESC, Id DESC";

            var attempt = await _unitOfWork.LucidMarketplaceRespondentAttempt.GetEntityData<LucidMarketplaceRespondentAttemptDTO>(query, new
            {
                PanelistId = panelistId.Trim(),
                SurveyId = surveyId.Value,
                SupplierCode = string.IsNullOrWhiteSpace(supplierCode) ? null : supplierCode
            });

            if (attempt != null)
            {
                return (attempt, "PanelistId+SurveyId");
            }
        }

        return (null, null);
    }

    private static LucidMarketplaceRespondentAttemptDTO BuildMergedAttemptFromOutcome(
        LucidMarketplaceRespondentAttemptDTO currentAttempt,
        LucidMarketplaceRespondentOutcome outcomeEntity,
        int outcomeId,
        LucidMarketplaceOutcomeStatusInfo statusInfo,
        LucidMarketplaceOutcomeRelationshipContext relatedContext,
        DateTime receivedOn)
    {
        return new LucidMarketplaceRespondentAttemptDTO
        {
            Id = currentAttempt.Id,
            LucidMarketplaceOpportunityId = currentAttempt.LucidMarketplaceOpportunityId > 0
                ? currentAttempt.LucidMarketplaceOpportunityId
                : relatedContext.LucidMarketplaceOpportunityId.GetValueOrDefault(),
            InternalProjectId = currentAttempt.InternalProjectId ?? relatedContext.InternalProjectId,
            InternalProjectUrlId = currentAttempt.InternalProjectUrlId,
            InternalProjectMappingId = currentAttempt.InternalProjectMappingId,
            LucidSurveyId = currentAttempt.LucidSurveyId > 0
                ? currentAttempt.LucidSurveyId
                : outcomeEntity.SurveyId.GetValueOrDefault(),
            SupplierCode = string.IsNullOrWhiteSpace(currentAttempt.SupplierCode) ? outcomeEntity.SupplierCode : currentAttempt.SupplierCode,
            RespondentId = string.IsNullOrWhiteSpace(currentAttempt.RespondentId) ? outcomeEntity.RespondentId : currentAttempt.RespondentId,
            ParentSessionId = string.IsNullOrWhiteSpace(outcomeEntity.ParentSessionId) ? currentAttempt.ParentSessionId : outcomeEntity.ParentSessionId,
            PanelistId = string.IsNullOrWhiteSpace(outcomeEntity.PanelistId) ? currentAttempt.PanelistId : outcomeEntity.PanelistId,
            SessionId = string.IsNullOrWhiteSpace(outcomeEntity.SessionId) ? currentAttempt.SessionId : outcomeEntity.SessionId,
            LaunchUrl = currentAttempt.LaunchUrl,
            AttemptType = currentAttempt.AttemptType,
            AttemptedOn = currentAttempt.AttemptedOn,
            ReturnStatus = currentAttempt.ReturnStatus,
            ReturnCode = currentAttempt.ReturnCode,
            ReturnRawQuery = currentAttempt.ReturnRawQuery,
            MarketplaceStatus = outcomeEntity.MarketplaceStatus,
            ClientStatus = outcomeEntity.ClientStatus,
            FinalStatus = statusInfo.FinalStatus,
            FinalStatusSource = "OutcomesFeed",
            AsyncLastReceivedOn = receivedOn,
            LatestRespondentOutcomeId = outcomeId,
            RevenueValue = outcomeEntity.RevenueValue,
            RevenueCurrencyCode = outcomeEntity.RevenueCurrencyCode,
            IsCompleted = statusInfo.IsCompleted,
            IsTerminated = statusInfo.IsTerminated,
            IsOverQuota = statusInfo.IsOverQuota,
            IsQualityTermination = statusInfo.IsQualityTermination,
            IsDuplicate = statusInfo.IsDuplicate,
            IsSecurityTermination = statusInfo.IsSecurityTermination,
            RawJson = currentAttempt.RawJson,
            Notes = currentAttempt.Notes,
            CreatedDate = currentAttempt.CreatedDate,
            ModifiedDate = receivedOn
        };
    }

    private static async Task<LucidMarketplaceRespondentOutcomeDTO?> GetExistingLatestOutcomeAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        string supplierCode,
        string? sessionId,
        string? respondentId,
        string? panelistId,
        int? surveyId)
    {
        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            return await connection.QueryFirstOrDefaultAsync<LucidMarketplaceRespondentOutcomeDTO>(
                @"SELECT TOP 1 *
                  FROM LucidMarketplaceRespondentOutcome
                  WHERE IsLatest = 1
                    AND ISNULL(SupplierCode, '') = @SupplierCode
                    AND SessionId = @SessionId
                  ORDER BY ISNULL(LastDate, ISNULL(EntryDate, ISNULL(ReceivedOn, CreatedDate))) DESC, Id DESC",
                new
                {
                    SupplierCode = supplierCode,
                    SessionId = sessionId.Trim()
                },
                transaction);
        }

        if (!string.IsNullOrWhiteSpace(respondentId))
        {
            return await connection.QueryFirstOrDefaultAsync<LucidMarketplaceRespondentOutcomeDTO>(
                @"SELECT TOP 1 *
                  FROM LucidMarketplaceRespondentOutcome
                  WHERE IsLatest = 1
                    AND ISNULL(SupplierCode, '') = @SupplierCode
                    AND ISNULL(RespondentId, '') = @RespondentId
                    AND ((@SurveyId IS NULL AND SurveyId IS NULL) OR SurveyId = @SurveyId)
                  ORDER BY ISNULL(LastDate, ISNULL(EntryDate, ISNULL(ReceivedOn, CreatedDate))) DESC, Id DESC",
                new
                {
                    SupplierCode = supplierCode,
                    RespondentId = respondentId.Trim(),
                    SurveyId = surveyId
                },
                transaction);
        }

        if (!string.IsNullOrWhiteSpace(panelistId) && surveyId.HasValue && surveyId.Value > 0)
        {
            return await connection.QueryFirstOrDefaultAsync<LucidMarketplaceRespondentOutcomeDTO>(
                @"SELECT TOP 1 *
                  FROM LucidMarketplaceRespondentOutcome
                  WHERE IsLatest = 1
                    AND ISNULL(SupplierCode, '') = @SupplierCode
                    AND ISNULL(PanelistId, '') = @PanelistId
                    AND SurveyId = @SurveyId
                  ORDER BY ISNULL(LastDate, ISNULL(EntryDate, ISNULL(ReceivedOn, CreatedDate))) DESC, Id DESC",
                new
                {
                    SupplierCode = supplierCode,
                    PanelistId = panelistId.Trim(),
                    SurveyId = surveyId.Value
                },
                transaction);
        }

        return null;
    }

    private static DateTime GetOutcomeEventDate(DateTime? entryDate, DateTime? lastDate, DateTime? receivedOn, DateTime? createdDate)
    {
        return lastDate ?? entryDate ?? receivedOn ?? createdDate ?? DateTime.MinValue;
    }

    private static bool IsEquivalentOutcome(LucidMarketplaceRespondentOutcomeDTO existingOutcome, LucidMarketplaceRespondentOutcome incomingOutcome)
    {
        return string.Equals(existingOutcome.SupplierCode?.Trim(), incomingOutcome.SupplierCode?.Trim(), StringComparison.OrdinalIgnoreCase) &&
               string.Equals(existingOutcome.RespondentId?.Trim(), incomingOutcome.RespondentId?.Trim(), StringComparison.OrdinalIgnoreCase) &&
               string.Equals(existingOutcome.ParentSessionId?.Trim(), incomingOutcome.ParentSessionId?.Trim(), StringComparison.OrdinalIgnoreCase) &&
               string.Equals(existingOutcome.PanelistId?.Trim(), incomingOutcome.PanelistId?.Trim(), StringComparison.OrdinalIgnoreCase) &&
               string.Equals(existingOutcome.SessionId?.Trim(), incomingOutcome.SessionId?.Trim(), StringComparison.OrdinalIgnoreCase) &&
               existingOutcome.MarketplaceStatus == incomingOutcome.MarketplaceStatus &&
               existingOutcome.ClientStatus == incomingOutcome.ClientStatus &&
               existingOutcome.SurveyId == incomingOutcome.SurveyId &&
               string.Equals(existingOutcome.FinalStatus?.Trim(), incomingOutcome.FinalStatus?.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static async Task ResetPreviousLatestOutcomeAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        string supplierCode,
        string? sessionId,
        string? respondentId,
        string? panelistId,
        int? surveyId,
        DateTime modifiedDate)
    {
        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            await connection.ExecuteAsync(
                @"UPDATE LucidMarketplaceRespondentOutcome
                  SET IsLatest = 0,
                      ModifiedDate = @ModifiedDate
                  WHERE IsLatest = 1
                    AND ISNULL(SupplierCode, '') = @SupplierCode
                    AND SessionId = @SessionId",
                new
                {
                    SupplierCode = supplierCode,
                    SessionId = sessionId.Trim(),
                    ModifiedDate = modifiedDate
                },
                transaction);

            return;
        }

        if (!string.IsNullOrWhiteSpace(respondentId))
        {
            await connection.ExecuteAsync(
                @"UPDATE LucidMarketplaceRespondentOutcome
                  SET IsLatest = 0,
                      ModifiedDate = @ModifiedDate
                  WHERE IsLatest = 1
                    AND ISNULL(SupplierCode, '') = @SupplierCode
                    AND ISNULL(RespondentId, '') = @RespondentId
                    AND (
                        (@SurveyId IS NULL AND SurveyId IS NULL)
                        OR SurveyId = @SurveyId
                    )",
                new
                {
                    SupplierCode = supplierCode,
                    RespondentId = respondentId.Trim(),
                    SurveyId = surveyId,
                    ModifiedDate = modifiedDate
                },
                transaction);

            return;
        }

        if (!string.IsNullOrWhiteSpace(panelistId) && surveyId.HasValue && surveyId.Value > 0)
        {
            await connection.ExecuteAsync(
                @"UPDATE LucidMarketplaceRespondentOutcome
                  SET IsLatest = 0,
                      ModifiedDate = @ModifiedDate
                  WHERE IsLatest = 1
                    AND ISNULL(SupplierCode, '') = @SupplierCode
                    AND ISNULL(PanelistId, '') = @PanelistId
                    AND SurveyId = @SurveyId",
                new
                {
                    SupplierCode = supplierCode,
                    PanelistId = panelistId.Trim(),
                    SurveyId = surveyId.Value,
                    ModifiedDate = modifiedDate
                },
                transaction);
        }
    }

    private static async Task UpdateRespondentOutcomeAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        int outcomeId,
        LucidMarketplaceRespondentOutcome entity)
    {
        const string updateSql = @"
            UPDATE LucidMarketplaceRespondentOutcome
            SET SupplierCode = @SupplierCode,
                RespondentId = @RespondentId,
                ParentSessionId = @ParentSessionId,
                PanelistId = @PanelistId,
                SessionId = @SessionId,
                MarketplaceStatus = @MarketplaceStatus,
                ClientStatus = @ClientStatus,
                EntryDate = @EntryDate,
                LastDate = @LastDate,
                SurveyId = @SurveyId,
                RevenueValue = @RevenueValue,
                RevenueCurrencyCode = @RevenueCurrencyCode,
                StudyType = @StudyType,
                BuyerId = @BuyerId,
                ProofCostPerInterview = @ProofCostPerInterview,
                FinalStatus = @FinalStatus,
                RawJson = @RawJson,
                Source = @Source,
                ReceivedOn = @ReceivedOn,
                IsLatest = @IsLatest,
                RelatedAttemptId = @RelatedAttemptId,
                RelatedOpportunityId = @RelatedOpportunityId,
                RelatedInternalProjectId = @RelatedInternalProjectId,
                ModifiedDate = @ModifiedDate
            WHERE Id = @Id";

        await connection.ExecuteAsync(updateSql, new
        {
            Id = outcomeId,
            entity.SupplierCode,
            entity.RespondentId,
            entity.ParentSessionId,
            entity.PanelistId,
            entity.SessionId,
            entity.MarketplaceStatus,
            entity.ClientStatus,
            entity.EntryDate,
            entity.LastDate,
            entity.SurveyId,
            entity.RevenueValue,
            entity.RevenueCurrencyCode,
            entity.StudyType,
            entity.BuyerId,
            entity.ProofCostPerInterview,
            entity.FinalStatus,
            entity.RawJson,
            entity.Source,
            entity.ReceivedOn,
            entity.IsLatest,
            entity.RelatedAttemptId,
            entity.RelatedOpportunityId,
            entity.RelatedInternalProjectId,
            entity.ModifiedDate
        }, transaction);
    }

    private static async Task<int> InsertRespondentOutcomeAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        LucidMarketplaceRespondentOutcome entity)
    {
        const string insertSql = @"
            INSERT INTO LucidMarketplaceRespondentOutcome
            (
                SupplierCode, RespondentId, ParentSessionId, PanelistId, SessionId,
                MarketplaceStatus, ClientStatus, EntryDate, LastDate, SurveyId,
                RevenueValue, RevenueCurrencyCode, StudyType, BuyerId, ProofCostPerInterview,
                FinalStatus, RawJson, Source, ReceivedOn, IsLatest,
                RelatedAttemptId, RelatedOpportunityId, RelatedInternalProjectId, CreatedDate, ModifiedDate
            )
            VALUES
            (
                @SupplierCode, @RespondentId, @ParentSessionId, @PanelistId, @SessionId,
                @MarketplaceStatus, @ClientStatus, @EntryDate, @LastDate, @SurveyId,
                @RevenueValue, @RevenueCurrencyCode, @StudyType, @BuyerId, @ProofCostPerInterview,
                @FinalStatus, @RawJson, @Source, @ReceivedOn, @IsLatest,
                @RelatedAttemptId, @RelatedOpportunityId, @RelatedInternalProjectId, @CreatedDate, @ModifiedDate
            );
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        return await connection.ExecuteScalarAsync<int>(insertSql, entity, transaction);
    }

    private static async Task UpdateRespondentAttemptAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        LucidMarketplaceRespondentAttemptDTO attempt)
    {
        const string updateSql = @"
            UPDATE LucidMarketplaceRespondentAttempt
            SET LucidMarketplaceOpportunityId = @LucidMarketplaceOpportunityId,
                InternalProjectId = @InternalProjectId,
                InternalProjectUrlId = @InternalProjectUrlId,
                InternalProjectMappingId = @InternalProjectMappingId,
                LucidSurveyId = @LucidSurveyId,
                SupplierCode = @SupplierCode,
                RespondentId = @RespondentId,
                ParentSessionId = @ParentSessionId,
                PanelistId = @PanelistId,
                SessionId = @SessionId,
                LaunchUrl = @LaunchUrl,
                AttemptType = @AttemptType,
                AttemptedOn = @AttemptedOn,
                ReturnStatus = @ReturnStatus,
                ReturnCode = @ReturnCode,
                ReturnRawQuery = @ReturnRawQuery,
                MarketplaceStatus = @MarketplaceStatus,
                ClientStatus = @ClientStatus,
                FinalStatus = @FinalStatus,
                FinalStatusSource = @FinalStatusSource,
                AsyncLastReceivedOn = @AsyncLastReceivedOn,
                LatestRespondentOutcomeId = @LatestRespondentOutcomeId,
                RevenueValue = @RevenueValue,
                RevenueCurrencyCode = @RevenueCurrencyCode,
                IsCompleted = @IsCompleted,
                IsTerminated = @IsTerminated,
                IsOverQuota = @IsOverQuota,
                IsQualityTermination = @IsQualityTermination,
                IsDuplicate = @IsDuplicate,
                IsSecurityTermination = @IsSecurityTermination,
                RawJson = @RawJson,
                Notes = @Notes,
                ModifiedDate = @ModifiedDate
            WHERE Id = @Id";

        await connection.ExecuteAsync(updateSql, attempt, transaction);
    }

    private async Task<int> UpsertOpportunityAsync(JsonElement opportunityElement, string? supplierCode, int? userId)
    {
        if (string.IsNullOrWhiteSpace(supplierCode))
        {
            throw new InvalidOperationException("SupplierCode could not be determined for the opportunity payload.");
        }

        if (!TryGetRequiredSurveyId(opportunityElement, out var surveyId))
        {
            throw new InvalidOperationException("survey_id was missing or invalid.");
        }

        using var connection = await CreateOpenConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var existing = await connection.QueryFirstOrDefaultAsync<LucidMarketplaceOpportunity>(
                @"SELECT TOP 1 *
                  FROM LucidMarketplaceOpportunity
                  WHERE SupplierCode = @SupplierCode
                    AND SurveyId = @SurveyId",
                new
                {
                    SupplierCode = supplierCode.Trim(),
                    SurveyId = surveyId
                },
                transaction);

            var now = DateTime.Now;
            var isLive = GetBoolValue(opportunityElement, "is_live", "isLive");
            var localState = string.IsNullOrWhiteSpace(existing?.LocalState) ? "New" : existing!.LocalState;
            var preserveIgnored = string.Equals(localState, "Ignored", StringComparison.OrdinalIgnoreCase) && existing?.IsActive == false;
            var incomingSurveyNumber = GetStringValue(opportunityElement, "survey_number", "surveyNumber");

            var entity = existing ?? new LucidMarketplaceOpportunity();
            entity.SupplierCode = supplierCode.Trim();
            entity.SurveyId = surveyId;
            entity.SurveyNumber = string.IsNullOrWhiteSpace(incomingSurveyNumber)
                ? (!string.IsNullOrWhiteSpace(existing?.SurveyNumber)
                    ? existing!.SurveyNumber
                    : surveyId.ToString(CultureInfo.InvariantCulture))
                : incomingSurveyNumber.Trim();
            entity.SurveyName = GetStringValue(opportunityElement, "survey_name", "surveyName", "title");
            entity.AccountName = GetStringValue(opportunityElement, "account_name", "accountName");
            entity.BuyerId = GetIntValue(opportunityElement, "buyer_id", "buyerId");
            entity.TargetGroupId = GetStringValue(opportunityElement, "target_group_id", "targetGroupId");
            entity.CountryLanguageCode = GetStringValue(opportunityElement, "country_language", "countryLanguage");
            entity.StudyType = GetStringValue(opportunityElement, "study_type", "studyType");
            entity.Industry = GetStringValue(opportunityElement, "industry");
            entity.RevenuePerInterview = GetMonetaryValue(opportunityElement, "revenue_per_interview", "revenuePerInterview");
            entity.RevenueCurrencyCode = GetMonetaryCurrency(opportunityElement, "revenue_per_interview", "revenuePerInterview");
            entity.BidIncidence = GetDecimalValue(opportunityElement, "bid_incidence", "bidIncidence");
            entity.BidLengthOfInterview = GetDecimalValue(opportunityElement, "bid_length_of_interview", "bidLengthOfInterview");
            entity.TotalRemaining = GetIntValue(opportunityElement, "total_remaining", "totalRemaining");
            entity.IsLive = isLive;
            entity.RecontactCount = GetIntValue(opportunityElement, "recontact_count", "recontactCount");
            entity.SurveyGroupIdsJson = GetJsonTextValue(opportunityElement, "survey_group_ids", "surveyGroupIds");
            entity.MessageReason = GetStringValue(opportunityElement, "message_reason", "messageReason");
            entity.LastVendorUpdatedOn = GetDateTimeValue(opportunityElement, "survey_lastSend", "survey_last_send", "last_vendor_updated_on", "updated_at", "lastUpdatedOn");
            entity.LastSyncedOn = now;
            entity.LocalState = localState;
            entity.RawJson = null;
            entity.IsActive = preserveIgnored ? false : (isLive ?? existing?.IsActive ?? true);
            entity.ModifiedDate = now;
            entity.ModifiedBy = userId;

            int opportunityId;
            if (existing == null)
            {
                entity.CreatedDate = now;
                entity.CreatedBy = userId;

                const string insertOpportunitySql = @"
                    INSERT INTO LucidMarketplaceOpportunity
                    (
                        SupplierCode, SurveyId, SurveyNumber, SurveyName, AccountName, BuyerId, TargetGroupId,
                        CountryLanguageCode, StudyType, Industry, RevenuePerInterview, RevenueCurrencyCode,
                        BidIncidence, BidLengthOfInterview, TotalRemaining, IsLive, RecontactCount,
                        SurveyGroupIdsJson, MessageReason, LastVendorUpdatedOn, LastSyncedOn, LocalState,
                        RawJson, IsActive, CreatedDate, ModifiedDate, CreatedBy, ModifiedBy
                    )
                    VALUES
                    (
                        @SupplierCode, @SurveyId, @SurveyNumber, @SurveyName, @AccountName, @BuyerId, @TargetGroupId,
                        @CountryLanguageCode, @StudyType, @Industry, @RevenuePerInterview, @RevenueCurrencyCode,
                        @BidIncidence, @BidLengthOfInterview, @TotalRemaining, @IsLive, @RecontactCount,
                        @SurveyGroupIdsJson, @MessageReason, @LastVendorUpdatedOn, @LastSyncedOn, @LocalState,
                        @RawJson, @IsActive, @CreatedDate, @ModifiedDate, @CreatedBy, @ModifiedBy
                    );
                    SELECT CAST(SCOPE_IDENTITY() AS int);";

                opportunityId = await connection.ExecuteScalarAsync<int>(insertOpportunitySql, entity, transaction);
            }
            else
            {
                entity.LucidMarketplaceOpportunityId = existing.LucidMarketplaceOpportunityId;
                entity.CreatedDate = existing.CreatedDate;
                entity.CreatedBy = existing.CreatedBy;

                const string updateOpportunitySql = @"
                    UPDATE LucidMarketplaceOpportunity
                    SET SupplierCode = @SupplierCode,
                        SurveyId = @SurveyId,
                        SurveyNumber = @SurveyNumber,
                        SurveyName = @SurveyName,
                        AccountName = @AccountName,
                        BuyerId = @BuyerId,
                        TargetGroupId = @TargetGroupId,
                        CountryLanguageCode = @CountryLanguageCode,
                        StudyType = @StudyType,
                        Industry = @Industry,
                        RevenuePerInterview = @RevenuePerInterview,
                        RevenueCurrencyCode = @RevenueCurrencyCode,
                        BidIncidence = @BidIncidence,
                        BidLengthOfInterview = @BidLengthOfInterview,
                        TotalRemaining = @TotalRemaining,
                        IsLive = @IsLive,
                        RecontactCount = @RecontactCount,
                        SurveyGroupIdsJson = @SurveyGroupIdsJson,
                        MessageReason = @MessageReason,
                        LastVendorUpdatedOn = @LastVendorUpdatedOn,
                        LastSyncedOn = @LastSyncedOn,
                        LocalState = @LocalState,
                        RawJson = @RawJson,
                        IsActive = @IsActive,
                        ModifiedDate = @ModifiedDate,
                        ModifiedBy = @ModifiedBy
                    WHERE LucidMarketplaceOpportunityId = @LucidMarketplaceOpportunityId";

                await connection.ExecuteAsync(updateOpportunitySql, entity, transaction);
                opportunityId = existing.LucidMarketplaceOpportunityId;
            }

            transaction.Commit();
            return opportunityId;
        }
        catch
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // ignored
            }

            throw;
        }
    }

    private async Task InsertQualificationsAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        int opportunityId,
        string supplierCode,
        int surveyId,
        JsonElement opportunityElement,
        int? userId)
    {
        if (!TryGetProperty(opportunityElement, out var qualificationsElement, "survey_qualifications", "surveyQualifications") ||
            qualificationsElement.ValueKind != JsonValueKind.Array)
        {
            return;
        }

        const string insertSql = @"
            INSERT INTO LucidMarketplaceOpportunityQualification
            (
                LucidMarketplaceOpportunityId, SupplierCode, SurveyId, QuestionId, QuestionText,
                LogicalOperator, PrecodesJson, RawJson, SortOrder, IsActive, CreatedDate, ModifiedDate, CreatedBy, ModifiedBy
            )
            VALUES
            (
                @LucidMarketplaceOpportunityId, @SupplierCode, @SurveyId, @QuestionId, @QuestionText,
                @LogicalOperator, @PrecodesJson, @RawJson, @SortOrder, @IsActive, @CreatedDate, @ModifiedDate, @CreatedBy, @ModifiedBy
            );";

        var sortOrder = 1;
        foreach (var qualification in qualificationsElement.EnumerateArray())
        {
            var row = new LucidMarketplaceOpportunityQualification
            {
                LucidMarketplaceOpportunityId = opportunityId,
                SupplierCode = supplierCode,
                SurveyId = surveyId,
                QuestionId = GetIntValue(qualification, "question_id", "questionId"),
                QuestionText = GetStringValue(qualification, "question_text", "questionText"),
                LogicalOperator = GetStringValue(qualification, "logical_operator", "logicalOperator"),
                PrecodesJson = GetJsonTextValue(qualification, "precodes", "preCodes"),
                RawJson = qualification.GetRawText(),
                SortOrder = sortOrder++,
                IsActive = true,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                CreatedBy = userId,
                ModifiedBy = userId
            };

            await connection.ExecuteAsync(insertSql, row, transaction);
        }
    }

    private async Task InsertQuotasAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        int opportunityId,
        string supplierCode,
        int surveyId,
        JsonElement opportunityElement,
        int? userId)
    {
        if (!TryGetProperty(opportunityElement, out var quotasElement, "survey_quotas", "surveyQuotas") ||
            quotasElement.ValueKind != JsonValueKind.Array)
        {
            return;
        }

        const string insertSql = @"
            INSERT INTO LucidMarketplaceOpportunityQuota
            (
                LucidMarketplaceOpportunityId, SupplierCode, SurveyId, SurveyQuotaId, SurveyQuotaType,
                Conversion, NumberOfRespondents, QuestionsJson, RawJson, SortOrder, IsActive,
                CreatedDate, ModifiedDate, CreatedBy, ModifiedBy
            )
            VALUES
            (
                @LucidMarketplaceOpportunityId, @SupplierCode, @SurveyId, @SurveyQuotaId, @SurveyQuotaType,
                @Conversion, @NumberOfRespondents, @QuestionsJson, @RawJson, @SortOrder, @IsActive,
                @CreatedDate, @ModifiedDate, @CreatedBy, @ModifiedBy
            );";

        var sortOrder = 1;
        foreach (var quota in quotasElement.EnumerateArray())
        {
            var row = new LucidMarketplaceOpportunityQuota
            {
                LucidMarketplaceOpportunityId = opportunityId,
                SupplierCode = supplierCode,
                SurveyId = surveyId,
                SurveyQuotaId = GetIntValue(quota, "survey_quota_id", "surveyQuotaId", "quota_id", "quotaId"),
                SurveyQuotaType = GetStringValue(quota, "survey_quota_type", "surveyQuotaType", "quota_name", "quotaName"),
                Conversion = GetDecimalValue(quota, "conversion"),
                NumberOfRespondents = GetIntValue(quota, "number_of_respondents", "numberOfRespondents", "remaining"),
                QuestionsJson = GetJsonTextValue(quota, "questions"),
                RawJson = quota.GetRawText(),
                SortOrder = sortOrder++,
                IsActive = true,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                CreatedBy = userId,
                ModifiedBy = userId
            };

            await connection.ExecuteAsync(insertSql, row, transaction);
        }
    }

    private Task InsertLogAsync(LucidMarketplaceSyncLogDTO log)
    {
        // Lucid Marketplace sync logging is intentionally disabled to keep the
        // LucidMarketplaceSyncLog table from growing too quickly.
        return Task.CompletedTask;
    }

    private static bool NormalizeIncludeQuotas(string? subscriptionType, bool includeQuotas)
    {
        return string.Equals(subscriptionType?.Trim(), OpportunitiesSubscriptionType, StringComparison.OrdinalIgnoreCase)
            ? false
            : includeQuotas;
    }

    private async Task<SqlConnection> CreateOpenConnectionAsync()
    {
        var connection = new SqlConnection(_dapperDBSetting.ConnectionString);
        await connection.OpenAsync().ConfigureAwait(false);
        return connection;
    }

    private static List<JsonElement> ExtractOpportunityElements(JsonElement root)
    {
        var items = new List<JsonElement>();

        if (root.ValueKind == JsonValueKind.Array)
        {
            items.AddRange(root.EnumerateArray().Select(x => x.Clone()));
            return items;
        }

        if (root.ValueKind != JsonValueKind.Object)
        {
            return items;
        }

        if (TryGetProperty(root, out var surveysElement, "surveys", "Surveys") && surveysElement.ValueKind == JsonValueKind.Array)
        {
            items.AddRange(surveysElement.EnumerateArray().Select(x => x.Clone()));
            return items;
        }

        if (TryGetProperty(root, out var opportunitiesElement, "opportunities", "Opportunities") &&
            opportunitiesElement.ValueKind == JsonValueKind.Array)
        {
            items.AddRange(opportunitiesElement.EnumerateArray().Select(x => x.Clone()));
            return items;
        }

        if (TryGetProperty(root, out var dataElement, "data", "Data") && dataElement.ValueKind == JsonValueKind.Array)
        {
            items.AddRange(dataElement.EnumerateArray().Select(x => x.Clone()));
            return items;
        }

        if (HasSurveyIdentity(root))
        {
            items.Add(root.Clone());
        }

        return items;
    }

    private static List<JsonElement> ExtractOutcomeElements(JsonElement root)
    {
        var items = new List<JsonElement>();

        if (root.ValueKind == JsonValueKind.Array)
        {
            items.AddRange(root.EnumerateArray().Select(x => x.Clone()));
            return items;
        }

        if (root.ValueKind != JsonValueKind.Object)
        {
            return items;
        }

        if (TryGetProperty(root, out var outcomesElement, "outcomes", "Outcomes") && outcomesElement.ValueKind == JsonValueKind.Array)
        {
            items.AddRange(outcomesElement.EnumerateArray().Select(x => x.Clone()));
            return items;
        }

        if (TryGetProperty(root, out var respondentsElement, "respondents", "Respondents") && respondentsElement.ValueKind == JsonValueKind.Array)
        {
            items.AddRange(respondentsElement.EnumerateArray().Select(x => x.Clone()));
            return items;
        }

        if (TryGetProperty(root, out var dataElement, "data", "Data") && dataElement.ValueKind == JsonValueKind.Array)
        {
            items.AddRange(dataElement.EnumerateArray().Select(x => x.Clone()));
            return items;
        }

        if (HasOutcomeIdentity(root))
        {
            items.Add(root.Clone());
        }

        return items;
    }

    private static bool HasSurveyIdentity(JsonElement element)
    {
        return TryGetProperty(element, out _, "survey_id", "surveyId");
    }

    private static bool HasOutcomeIdentity(JsonElement element)
    {
        return TryGetProperty(element, out _, "respondent_id", "respondentId") ||
               TryGetProperty(element, out _, "session_id", "sessionId") ||
               TryGetProperty(element, out _, "marketplace_status", "marketplaceStatus") ||
               TryGetProperty(element, out _, "client_status", "clientStatus");
    }

    private static bool TryGetRequiredSurveyId(JsonElement element, out int surveyId)
    {
        surveyId = 0;
        var value = GetIntValue(element, "survey_id", "surveyId");
        if (!value.HasValue || value.Value <= 0)
        {
            return false;
        }

        surveyId = value.Value;
        return true;
    }

    private static bool TryGetProperty(JsonElement element, out JsonElement value, params string[] names)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            value = default;
            return false;
        }

        foreach (var property in element.EnumerateObject())
        {
            var propertyName = NormalizeJsonName(property.Name);
            if (names.Any(name => propertyName == NormalizeJsonName(name)))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    private static string NormalizeJsonName(string name)
    {
        return new string(name.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
    }

    private static string? GetStringValue(JsonElement element, params string[] names)
    {
        if (!TryGetProperty(element, out var value, names))
        {
            return null;
        }

        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Number => value.ToString(),
            JsonValueKind.True => bool.TrueString,
            JsonValueKind.False => bool.FalseString,
            _ => value.ValueKind == JsonValueKind.Null ? null : value.GetRawText()
        };
    }

    private static int? GetIntValue(JsonElement element, params string[] names)
    {
        if (!TryGetProperty(element, out var value, names))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var intValue))
        {
            return intValue;
        }

        if (value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out intValue))
        {
            return intValue;
        }

        return null;
    }

    private static decimal? GetDecimalValue(JsonElement element, params string[] names)
    {
        if (!TryGetProperty(element, out var value, names))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out var decimalValue))
        {
            return decimalValue;
        }

        if (value.ValueKind == JsonValueKind.String && decimal.TryParse(value.GetString(), out decimalValue))
        {
            return decimalValue;
        }

        return null;
    }

    private static bool? GetBoolValue(JsonElement element, params string[] names)
    {
        if (!TryGetProperty(element, out var value, names))
        {
            return null;
        }

        return value.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String when bool.TryParse(value.GetString(), out var boolValue) => boolValue,
            _ => null
        };
    }

    private static DateTime? GetDateTimeValue(JsonElement element, params string[] names)
    {
        var raw = GetStringValue(element, names);
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        if (DateTime.TryParse(raw, out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private static decimal? GetMonetaryValue(JsonElement element, params string[] names)
    {
        if (!TryGetProperty(element, out var value, names))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Object &&
            TryGetProperty(value, out var amountElement, "value", "Value"))
        {
            if (amountElement.ValueKind == JsonValueKind.Number && amountElement.TryGetDecimal(out var decimalValue))
            {
                return decimalValue;
            }

            if (amountElement.ValueKind == JsonValueKind.String && decimal.TryParse(amountElement.GetString(), out decimalValue))
            {
                return decimalValue;
            }
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out var directValue))
        {
            return directValue;
        }

        return null;
    }

    private static string? GetMonetaryCurrency(JsonElement element, params string[] names)
    {
        if (!TryGetProperty(element, out var value, names) || value.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        return GetStringValue(value, "currency_code", "currencyCode");
    }

    private static string? GetJsonTextValue(JsonElement element, params string[] names)
    {
        if (!TryGetProperty(element, out var value, names))
        {
            return null;
        }

        return value.ValueKind == JsonValueKind.Null ? null : value.GetRawText();
    }

    private static string? Truncate(string? input, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Length <= maxLength)
        {
            return input;
        }

        return input.Substring(0, maxLength);
    }

    private sealed class LucidMarketplaceOutcomeRelationshipContext
    {
        public int? LucidMarketplaceOpportunityId { get; set; }

        public int? InternalProjectId { get; set; }
    }

    private sealed class LucidMarketplaceOutcomeGroup
    {
        public LucidMarketplaceRespondentOutcomeDTO LatestOutcome { get; set; } = new();

        public bool OutcomeChangedOverTime { get; set; }

        public string? MatchType { get; set; }
    }

    private sealed class LucidMarketplaceReconciliationBuildResult
    {
        public LucidMarketplaceReconciliationRunDTO Run { get; set; } = new();

        public List<LucidMarketplaceReconciliationItemDTO> Items { get; set; } = new();
    }

    private sealed class LucidMarketplaceOutcomeSaveContext
    {
        public int OutcomeId { get; set; }

        public int? AttemptId { get; set; }

        public int? RelatedOpportunityId { get; set; }

        public string? SupplierCode { get; set; }

        public int? SurveyId { get; set; }

        public string? FinalStatus { get; set; }

        public string? MatchType { get; set; }

        public string? InternalRespondentUid { get; set; }

        public string? LegacyProjectStatus { get; set; }

        public bool IsLatestOutcome { get; set; }
    }
}
