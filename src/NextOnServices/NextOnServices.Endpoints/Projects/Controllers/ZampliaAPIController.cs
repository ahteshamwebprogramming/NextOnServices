using System.Data.SqlClient;
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
public class ZampliaAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ZampliaAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly DapperDBSetting _dapperDBSetting;

    public ZampliaAPIController(
        IUnitOfWork unitOfWork,
        ILogger<ZampliaAPIController> logger,
        IMapper mapper,
        DapperDBSetting dapperDBSetting)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dapperDBSetting = dapperDBSetting;
    }

    [HttpGet("Setting")]
    public async Task<IActionResult> GetZampliaSetting()
    {
        var setting = await _unitOfWork.ZampliaSetting.GetEntityData<ZampliaSettingDTO>(
            @"SELECT TOP 1 *
              FROM ZampliaSetting
              ORDER BY IsActive DESC, ZampliaSettingId DESC");

        return Ok(setting ?? new ZampliaSettingDTO());
    }

    [HttpPost("Setting")]
    public async Task<IActionResult> SaveZampliaSetting(ZampliaSettingDTO inputData)
    {
        if (inputData == null)
        {
            return BadRequest("Invalid Zamplia settings payload.");
        }

        var current = await GetCurrentSettingEntityAsync(inputData.ZampliaSettingId);
        if (current == null)
        {
            var entity = _mapper.Map<ZampliaSetting>(inputData);
            entity.BaseUrl = string.IsNullOrWhiteSpace(entity.BaseUrl) ? "https://surveysupplysandbox.zamplia.com" : entity.BaseUrl.Trim();
            entity.CreatedDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;
            entity.ZampliaSettingId = await _unitOfWork.ZampliaSetting.AddAsync(entity);
            if (entity.ZampliaSettingId <= 0)
            {
                return BadRequest("Unable to save Zamplia settings.");
            }

            inputData.ZampliaSettingId = entity.ZampliaSettingId;
            inputData.CreatedDate = entity.CreatedDate;
            inputData.ModifiedDate = entity.ModifiedDate;
            return Ok(inputData);
        }

        current.BaseUrl = string.IsNullOrWhiteSpace(inputData.BaseUrl) ? current.BaseUrl : inputData.BaseUrl.Trim();
        if (!string.IsNullOrWhiteSpace(inputData.ApiKey))
        {
            current.ApiKey = inputData.ApiKey.Trim();
        }
        if (!string.IsNullOrWhiteSpace(inputData.ExitHmacKey))
        {
            current.ExitHmacKey = inputData.ExitHmacKey.Trim();
        }
        current.UseConsultingsBridge = inputData.UseConsultingsBridge;
        current.AutoSyncEnabled = inputData.AutoSyncEnabled;
        current.SyncIntervalMinutes = inputData.SyncIntervalMinutes;
        current.DefaultClientId = inputData.DefaultClientId;
        current.DefaultCountryId = inputData.DefaultCountryId;
        current.DefaultSupplierId = inputData.DefaultSupplierId;
        current.IsActive = inputData.IsActive;
        current.ModifiedDate = DateTime.Now;
        current.ModifiedBy = inputData.ModifiedBy;

        var updated = await _unitOfWork.ZampliaSetting.UpdateAsync(current);
        if (!updated)
        {
            return BadRequest("Unable to update Zamplia settings.");
        }

        inputData.ZampliaSettingId = current.ZampliaSettingId;
        inputData.CreatedDate = current.CreatedDate;
        inputData.ModifiedDate = current.ModifiedDate;
        inputData.CreatedBy = current.CreatedBy;
        return Ok(inputData);
    }

    [HttpGet("Surveys")]
    public async Task<IActionResult> GetZampliaSurveys(int take = 500)
    {
        var safeTake = take <= 0 ? 500 : Math.Min(take, 2000);
        var query = $@"
            SELECT TOP ({safeTake})
                s.*,
                InternalProjectId = COALESCE(pm.InternalProjectId, p.ProjectId),
                InternalProjectUrlId = COALESCE(pm.InternalProjectUrlId, pu.ID),
                InternalProjectMappingId = COALESCE(pm.InternalProjectMappingId, map.ID),
                IsMapped = CAST(CASE WHEN COALESCE(pm.InternalProjectId, p.ProjectId, 0) > 0 THEN 1 ELSE 0 END AS bit),
                MappingStatus = CASE WHEN COALESCE(pm.InternalProjectId, p.ProjectId, 0) > 0 THEN 'Mapped' ELSE ISNULL(NULLIF(s.LocalState, ''), 'New') END,
                QualificationCount = (SELECT COUNT(1) FROM ZampliaSurveyQualification q WHERE q.ZampliaSurveyId = s.ZampliaSurveyId),
                QuotaCount = (SELECT COUNT(1) FROM ZampliaSurveyQuota qt WHERE qt.ZampliaSurveyId = s.ZampliaSurveyId)
            FROM ZampliaSurvey s
            OUTER APPLY (
                SELECT TOP 1 pm.*
                FROM ZampliaProjectMap pm
                INNER JOIN Projects mappedProject
                    ON mappedProject.ProjectId = pm.InternalProjectId
                   AND ISNULL(mappedProject.IsActive, 0) = 1
                WHERE pm.ZampliaSurveyId = s.ZampliaSurveyId
                  AND pm.IsActive = 1
                ORDER BY pm.Id DESC
            ) pm
            OUTER APPLY (
                SELECT TOP 1 p.*
                FROM Projects p
                WHERE p.ProjectFrom = 'Zamplia'
                  AND ISNULL(p.IsActive, 0) = 1
                  AND TRY_CONVERT(bigint, p.ProjectIdFromApi) = s.SurveyId
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
            ORDER BY ISNULL(s.LastSyncedOn, ISNULL(s.ModifiedDate, s.CreatedDate)) DESC, s.ZampliaSurveyId DESC";

        var items = await _unitOfWork.ZampliaSurvey.GetTableData<ZampliaSurveyDTO>(query);
        return Ok(items ?? new List<ZampliaSurveyDTO>());
    }

    [HttpGet("Survey/{id}")]
    public async Task<IActionResult> GetZampliaSurvey(int id)
    {
        var query = @"
            SELECT TOP 1
                s.*,
                InternalProjectId = COALESCE(pm.InternalProjectId, p.ProjectId),
                InternalProjectUrlId = COALESCE(pm.InternalProjectUrlId, pu.ID),
                InternalProjectMappingId = COALESCE(pm.InternalProjectMappingId, map.ID),
                IsMapped = CAST(CASE WHEN COALESCE(pm.InternalProjectId, p.ProjectId, 0) > 0 THEN 1 ELSE 0 END AS bit),
                MappingStatus = CASE WHEN COALESCE(pm.InternalProjectId, p.ProjectId, 0) > 0 THEN 'Mapped' ELSE ISNULL(NULLIF(s.LocalState, ''), 'New') END,
                QualificationCount = (SELECT COUNT(1) FROM ZampliaSurveyQualification q WHERE q.ZampliaSurveyId = s.ZampliaSurveyId),
                QuotaCount = (SELECT COUNT(1) FROM ZampliaSurveyQuota qt WHERE qt.ZampliaSurveyId = s.ZampliaSurveyId)
            FROM ZampliaSurvey s
            OUTER APPLY (
                SELECT TOP 1 pm.*
                FROM ZampliaProjectMap pm
                INNER JOIN Projects mappedProject
                    ON mappedProject.ProjectId = pm.InternalProjectId
                   AND ISNULL(mappedProject.IsActive, 0) = 1
                WHERE pm.ZampliaSurveyId = s.ZampliaSurveyId
                  AND pm.IsActive = 1
                ORDER BY pm.Id DESC
            ) pm
            OUTER APPLY (
                SELECT TOP 1 p.*
                FROM Projects p
                WHERE p.ProjectFrom = 'Zamplia'
                  AND ISNULL(p.IsActive, 0) = 1
                  AND TRY_CONVERT(bigint, p.ProjectIdFromApi) = s.SurveyId
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
            WHERE s.ZampliaSurveyId = @Id";

        var item = await _unitOfWork.ZampliaSurvey.GetEntityData<ZampliaSurveyDTO>(query, new { Id = id });
        return item == null ? NotFound() : Ok(item);
    }

    [HttpGet("Survey/{id}/Qualifications")]
    public async Task<IActionResult> GetZampliaSurveyQualifications(int id)
    {
        var items = await _unitOfWork.ZampliaSurveyQualification.GetTableData<ZampliaSurveyQualificationDTO>(
            @"SELECT *
              FROM ZampliaSurveyQualification
              WHERE ZampliaSurveyId = @Id
              ORDER BY ISNULL(SortOrder, 0), Id",
            new { Id = id });

        return Ok(items ?? new List<ZampliaSurveyQualificationDTO>());
    }

    [HttpGet("Survey/{id}/Quotas")]
    public async Task<IActionResult> GetZampliaSurveyQuotas(int id)
    {
        var items = await _unitOfWork.ZampliaSurveyQuota.GetTableData<ZampliaSurveyQuotaDTO>(
            @"SELECT *
              FROM ZampliaSurveyQuota
              WHERE ZampliaSurveyId = @Id
              ORDER BY ISNULL(SortOrder, 0), Id",
            new { Id = id });

        return Ok(items ?? new List<ZampliaSurveyQuotaDTO>());
    }

    [HttpPost("Survey")]
    public async Task<IActionResult> SaveZampliaSurvey(ZampliaSurveyDTO inputData)
    {
        if (inputData == null || inputData.SurveyId <= 0)
        {
            return BadRequest("Invalid Zamplia survey payload.");
        }

        var entityId = await SaveSurveyEntityAsync(inputData);
        inputData.ZampliaSurveyId = entityId;
        return entityId > 0 ? Ok(inputData) : BadRequest("Unable to save Zamplia survey.");
    }

    [HttpPost("SurveyQualification")]
    public async Task<IActionResult> SaveZampliaSurveyQualification(ZampliaSurveyQualificationDTO inputData)
    {
        if (inputData == null || inputData.ZampliaSurveyId <= 0)
        {
            return BadRequest("Invalid Zamplia survey qualification payload.");
        }

        if (inputData.Id > 0)
        {
            var current = await _unitOfWork.ZampliaSurveyQualification.FindByIdAsync(inputData.Id);
            if (current == null)
            {
                return NotFound();
            }

            current.QuestionId = inputData.QuestionId;
            current.QuestionText = inputData.QuestionText;
            current.QuestionType = inputData.QuestionType;
            current.LogicalOperator = inputData.LogicalOperator;
            current.AnswerCodesJson = inputData.AnswerCodesJson;
            current.RawJson = inputData.RawJson;
            current.SortOrder = inputData.SortOrder;
            current.IsActive = inputData.IsActive;

            return await _unitOfWork.ZampliaSurveyQualification.UpdateAsync(current)
                ? Ok(inputData)
                : BadRequest("Unable to update Zamplia survey qualification.");
        }

        var entity = _mapper.Map<ZampliaSurveyQualification>(inputData);
        entity.CreatedDate = DateTime.Now;
        entity.Id = await _unitOfWork.ZampliaSurveyQualification.AddAsync(entity);
        inputData.Id = entity.Id;
        inputData.CreatedDate = entity.CreatedDate;
        return entity.Id > 0 ? Ok(inputData) : BadRequest("Unable to save Zamplia survey qualification.");
    }

    [HttpPost("SurveyQuota")]
    public async Task<IActionResult> SaveZampliaSurveyQuota(ZampliaSurveyQuotaDTO inputData)
    {
        if (inputData == null || inputData.ZampliaSurveyId <= 0)
        {
            return BadRequest("Invalid Zamplia survey quota payload.");
        }

        if (inputData.Id > 0)
        {
            var current = await _unitOfWork.ZampliaSurveyQuota.FindByIdAsync(inputData.Id);
            if (current == null)
            {
                return NotFound();
            }

            current.QuotaId = inputData.QuotaId;
            current.QuotaName = inputData.QuotaName;
            current.NumberOfRespondents = inputData.NumberOfRespondents;
            current.Conversion = inputData.Conversion;
            current.QuestionsJson = inputData.QuestionsJson;
            current.RawJson = inputData.RawJson;
            current.SortOrder = inputData.SortOrder;
            current.IsActive = inputData.IsActive;

            return await _unitOfWork.ZampliaSurveyQuota.UpdateAsync(current)
                ? Ok(inputData)
                : BadRequest("Unable to update Zamplia survey quota.");
        }

        var entity = _mapper.Map<ZampliaSurveyQuota>(inputData);
        entity.CreatedDate = DateTime.Now;
        entity.Id = await _unitOfWork.ZampliaSurveyQuota.AddAsync(entity);
        inputData.Id = entity.Id;
        inputData.CreatedDate = entity.CreatedDate;
        return entity.Id > 0 ? Ok(inputData) : BadRequest("Unable to save Zamplia survey quota.");
    }

    [HttpGet("Survey/{id}/ProjectMap")]
    public async Task<IActionResult> GetZampliaProjectMap(int id)
    {
        var item = await _unitOfWork.ZampliaProjectMap.GetEntityData<ZampliaProjectMapDTO>(
            @"SELECT TOP 1 *
              FROM ZampliaProjectMap
              WHERE ZampliaSurveyId = @Id
              ORDER BY IsActive DESC, Id DESC",
            new { Id = id });

        return Ok(item ?? new ZampliaProjectMapDTO());
    }

    [HttpPost("ProjectMap")]
    public async Task<IActionResult> SaveZampliaProjectMap(ZampliaProjectMapDTO inputData)
    {
        if (inputData == null || inputData.ZampliaSurveyId <= 0 || inputData.SurveyId <= 0)
        {
            return BadRequest("Invalid Zamplia project mapping payload.");
        }

        var current = inputData.Id > 0
            ? await _unitOfWork.ZampliaProjectMap.FindByIdAsync(inputData.Id)
            : await _unitOfWork.ZampliaProjectMap.GetEntityData<ZampliaProjectMap>(
                @"SELECT TOP 1 *
                  FROM ZampliaProjectMap
                  WHERE ZampliaSurveyId = @ZampliaSurveyId
                  ORDER BY Id DESC",
                new { inputData.ZampliaSurveyId });

        if (current == null)
        {
            var entity = _mapper.Map<ZampliaProjectMap>(inputData);
            entity.AddedOn = inputData.AddedOn ?? DateTime.Now;
            entity.Id = await _unitOfWork.ZampliaProjectMap.AddAsync(entity);
            inputData.Id = entity.Id;
            inputData.AddedOn = entity.AddedOn;
            return entity.Id > 0 ? Ok(inputData) : BadRequest("Unable to save Zamplia project mapping.");
        }

        current.InternalProjectId = inputData.InternalProjectId;
        current.InternalProjectUrlId = inputData.InternalProjectUrlId;
        current.InternalProjectMappingId = inputData.InternalProjectMappingId;
        current.IsActive = inputData.IsActive;
        current.RawJson = inputData.RawJson;
        current.AddedBy ??= inputData.AddedBy;
        current.AddedOn ??= inputData.AddedOn ?? DateTime.Now;

        var updated = await _unitOfWork.ZampliaProjectMap.UpdateAsync(current);
        inputData.Id = current.Id;
        inputData.AddedBy = current.AddedBy;
        inputData.AddedOn = current.AddedOn;
        return updated ? Ok(inputData) : BadRequest("Unable to update Zamplia project mapping.");
    }

    [HttpGet("Survey/{id}/EntryLink")]
    public async Task<IActionResult> GetZampliaEntryLink(int id)
    {
        var item = await _unitOfWork.ZampliaEntryLink.GetEntityData<ZampliaEntryLinkDTO>(
            @"SELECT TOP 1 *
              FROM ZampliaEntryLink
              WHERE ZampliaSurveyId = @Id
              ORDER BY IsActive DESC, ISNULL(ModifiedDate, CreatedDate) DESC, Id DESC",
            new { Id = id });

        return Ok(item ?? new ZampliaEntryLinkDTO());
    }

    [HttpPost("EntryLink")]
    public async Task<IActionResult> SaveZampliaEntryLink(ZampliaEntryLinkDTO inputData)
    {
        if (inputData == null || inputData.ZampliaSurveyId <= 0 || inputData.SurveyId <= 0)
        {
            return BadRequest("Invalid Zamplia entry-link payload.");
        }

        var current = inputData.Id > 0
            ? await _unitOfWork.ZampliaEntryLink.FindByIdAsync(inputData.Id)
            : await _unitOfWork.ZampliaEntryLink.GetEntityData<ZampliaEntryLink>(
                @"SELECT TOP 1 *
                  FROM ZampliaEntryLink
                  WHERE ZampliaSurveyId = @ZampliaSurveyId
                  ORDER BY Id DESC",
                new { inputData.ZampliaSurveyId });

        if (current == null)
        {
            var entity = _mapper.Map<ZampliaEntryLink>(inputData);
            TrimZampliaEntryLinkEntity(entity);
            entity.CreatedDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;
            entity.Id = await _unitOfWork.ZampliaEntryLink.AddAsync(entity);
            inputData.Id = entity.Id;
            inputData.CreatedDate = entity.CreatedDate;
            inputData.ModifiedDate = entity.ModifiedDate;
            return entity.Id > 0 ? Ok(inputData) : BadRequest("Unable to save Zamplia entry link.");
        }

        current.TransactionId = TrimForStorage(inputData.TransactionId, 150);
        current.VendorLink = TrimForStorage(inputData.VendorLink, 2000);
        current.InternalLaunchUrl = TrimForStorage(inputData.InternalLaunchUrl, 2000);
        current.HashApplied = inputData.HashApplied;
        current.RawJson = TrimForStorage(inputData.RawJson, 4000);
        current.IsActive = inputData.IsActive;
        current.ModifiedDate = DateTime.Now;
        current.ModifiedBy = inputData.ModifiedBy;
        current.CreatedBy ??= inputData.CreatedBy;

        var updated = await _unitOfWork.ZampliaEntryLink.UpdateAsync(current);
        inputData.Id = current.Id;
        inputData.CreatedDate = current.CreatedDate;
        inputData.ModifiedDate = current.ModifiedDate;
        inputData.CreatedBy = current.CreatedBy;
        return updated ? Ok(inputData) : BadRequest("Unable to update Zamplia entry link.");
    }

    [HttpGet("Survey/{id}/Attempts")]
    public async Task<IActionResult> GetZampliaRespondentAttemptsBySurvey(int id, int take = 100)
    {
        var safeTake = take <= 0 ? 100 : Math.Min(take, 500);
        var query = $@"SELECT TOP ({safeTake}) *
                       FROM ZampliaRespondentAttempt
                       WHERE ZampliaSurveyId = @Id
                       ORDER BY ISNULL(AttemptedOn, CreatedDate) DESC, Id DESC";

        var items = await _unitOfWork.ZampliaRespondentAttempt.GetTableData<ZampliaRespondentAttemptDTO>(query, new { Id = id });
        return Ok(items ?? new List<ZampliaRespondentAttemptDTO>());
    }

    [HttpGet("RespondentAttempt/{id}")]
    public async Task<IActionResult> GetZampliaRespondentAttempt(int id)
    {
        var entity = await _unitOfWork.ZampliaRespondentAttempt.FindByIdAsync(id);
        return entity == null ? NotFound() : Ok(_mapper.Map<ZampliaRespondentAttemptDTO>(entity));
    }

    [HttpPost("RespondentAttempt")]
    public async Task<IActionResult> SaveZampliaRespondentAttempt(ZampliaRespondentAttemptDTO inputData)
    {
        if (inputData == null || inputData.ZampliaSurveyId <= 0 || inputData.SurveyId <= 0)
        {
            return BadRequest("Invalid Zamplia respondent attempt payload.");
        }

        var current = inputData.Id > 0 ? await _unitOfWork.ZampliaRespondentAttempt.FindByIdAsync(inputData.Id) : null;
        if (current == null)
        {
            var entity = _mapper.Map<ZampliaRespondentAttempt>(inputData);
            TrimZampliaRespondentAttemptEntity(entity);
            entity.CreatedDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;
            entity.Id = await _unitOfWork.ZampliaRespondentAttempt.AddAsync(entity);
            inputData.Id = entity.Id;
            inputData.CreatedDate = entity.CreatedDate;
            inputData.ModifiedDate = entity.ModifiedDate;
            return entity.Id > 0 ? Ok(inputData) : BadRequest("Unable to save Zamplia respondent attempt.");
        }

        current.InternalProjectId = inputData.InternalProjectId;
        current.InternalProjectUrlId = inputData.InternalProjectUrlId;
        current.InternalProjectMappingId = inputData.InternalProjectMappingId;
        current.RespondentId = TrimForStorage(inputData.RespondentId, 150);
        current.TransactionId = TrimForStorage(inputData.TransactionId, 150);
        current.SessionId = TrimForStorage(inputData.SessionId, 150);
        current.IpAddress = TrimForStorage(inputData.IpAddress, 100);
        current.LaunchUrl = TrimForStorage(inputData.LaunchUrl, 2000);
        current.VendorLaunchUrl = TrimForStorage(inputData.VendorLaunchUrl, 2000);
        current.ReturnUrl = TrimForStorage(inputData.ReturnUrl, 2000);
        current.ReturnRawQuery = TrimForStorage(inputData.ReturnRawQuery, 2000);
        current.ReturnCode = TrimForStorage(inputData.ReturnCode, 100);
        current.ReturnStatus = TrimForStorage(inputData.ReturnStatus, 100);
        current.FinalStatus = TrimForStorage(inputData.FinalStatus, 100);
        current.FinalStatusSource = TrimForStorage(inputData.FinalStatusSource, 50);
        current.IsCompleted = inputData.IsCompleted;
        current.IsTerminated = inputData.IsTerminated;
        current.IsOverQuota = inputData.IsOverQuota;
        current.IsQualityTermination = inputData.IsQualityTermination;
        current.IsSecurityTermination = inputData.IsSecurityTermination;
        current.IsDuplicate = inputData.IsDuplicate;
        current.HmacReceived = TrimForStorage(inputData.HmacReceived, 500);
        current.HmacCalculated = TrimForStorage(inputData.HmacCalculated, 500);
        current.HmacValid = inputData.HmacValid;
        current.AttemptedOn = inputData.AttemptedOn;
        current.CompletedOn = inputData.CompletedOn;
        current.RawJson = TrimForStorage(inputData.RawJson, 4000);
        current.ModifiedDate = DateTime.Now;
        current.ModifiedBy = inputData.ModifiedBy;
        current.CreatedBy ??= inputData.CreatedBy;

        var updated = await _unitOfWork.ZampliaRespondentAttempt.UpdateAsync(current);
        inputData.Id = current.Id;
        inputData.CreatedDate = current.CreatedDate;
        inputData.ModifiedDate = current.ModifiedDate;
        inputData.CreatedBy = current.CreatedBy;
        return updated ? Ok(inputData) : BadRequest("Unable to update Zamplia respondent attempt.");
    }

    [HttpGet("ReconciliationRuns")]
    public async Task<IActionResult> GetZampliaReconciliationRuns(int take = 50)
    {
        var safeTake = take <= 0 ? 50 : Math.Min(take, 250);
        var query = $@"SELECT TOP ({safeTake}) *
                       FROM ZampliaReconciliationRun
                       ORDER BY ISNULL(CompletedOn, ISNULL(StartedOn, CreatedDate)) DESC, Id DESC";

        var items = await _unitOfWork.ZampliaReconciliationRun.GetTableData<ZampliaReconciliationRunDTO>(query);
        return Ok(items ?? new List<ZampliaReconciliationRunDTO>());
    }

    [HttpGet("ReconciliationRun/{id}/Items")]
    public async Task<IActionResult> GetZampliaReconciliationItems(int id)
    {
        var items = await _unitOfWork.ZampliaReconciliationItem.GetTableData<ZampliaReconciliationItemDTO>(
            @"SELECT *
              FROM ZampliaReconciliationItem
              WHERE ReconciliationRunId = @Id
              ORDER BY IsMismatch DESC, Id DESC",
            new { Id = id });

        return Ok(items ?? new List<ZampliaReconciliationItemDTO>());
    }

    [HttpGet("Dashboard")]
    public async Task<IActionResult> GetZampliaDashboard()
    {
        const string query = @"
            SELECT
                ActiveSettings = (SELECT COUNT(1) FROM ZampliaSetting WHERE IsActive = 1),
                ActiveSurveys = (SELECT COUNT(1) FROM ZampliaSurvey WHERE IsActive = 1),
                MappedSurveys = (SELECT COUNT(DISTINCT ProjectIdFromApi) FROM Projects WHERE ProjectFrom = 'Zamplia' AND ISNULL(IsActive, 0) = 1),
                TotalLogs = (SELECT COUNT(1) FROM ZampliaSyncLog),
                SuccessfulApiCalls = (SELECT COUNT(1) FROM ZampliaSyncLog WHERE IsSuccess = 1),
                FailedApiCalls = (SELECT COUNT(1) FROM ZampliaSyncLog WHERE IsSuccess = 0),
                TotalAttempts = (SELECT COUNT(1) FROM ZampliaRespondentAttempt),
                LastSyncTime = (SELECT MAX(COALESCE(CompletedOn, StartedOn, CreatedDate)) FROM ZampliaSyncLog),
                LatestReconciliationRunTime = (SELECT MAX(COALESCE(CompletedOn, StartedOn, CreatedDate)) FROM ZampliaReconciliationRun WHERE Success = 1),
                LatestReconciliationMismatchCount = ISNULL((
                    SELECT TOP 1 TotalMismatched
                    FROM ZampliaReconciliationRun
                    WHERE Success = 1
                    ORDER BY ISNULL(CompletedOn, ISNULL(StartedOn, CreatedDate)) DESC, Id DESC
                ), 0)";

        var summary = await _unitOfWork.ZampliaSyncLog.GetEntityData<ZampliaDashboardVM>(query);
        return Ok(summary ?? new ZampliaDashboardVM());
    }

    [HttpGet("Logs")]
    public async Task<IActionResult> GetZampliaLogs(int take = 250)
    {
        var safeTake = take <= 0 ? 250 : Math.Min(take, 1000);
        var query = $@"SELECT TOP ({safeTake}) *
                       FROM ZampliaSyncLog
                       ORDER BY ISNULL(CreatedDate, StartedOn) DESC, ZampliaSyncLogId DESC";

        var items = await _unitOfWork.ZampliaSyncLog.GetTableData<ZampliaSyncLogDTO>(query);
        return Ok(items ?? new List<ZampliaSyncLogDTO>());
    }

    [HttpGet("Log/{id}")]
    public async Task<IActionResult> GetZampliaLog(int id)
    {
        var entity = await _unitOfWork.ZampliaSyncLog.FindByIdAsync(id);
        return entity == null ? NotFound() : Ok(_mapper.Map<ZampliaSyncLogDTO>(entity));
    }

    [HttpPost("Log")]
    public async Task<IActionResult> AddZampliaLog(ZampliaSyncLogDTO inputData)
    {
        if (inputData == null)
        {
            return BadRequest("Invalid Zamplia log payload.");
        }

        var entity = _mapper.Map<ZampliaSyncLog>(inputData);
        TrimZampliaLogEntity(entity);
        entity.CreatedDate ??= DateTime.Now;
        entity.ZampliaSyncLogId = await _unitOfWork.ZampliaSyncLog.AddAsync(entity);
        inputData.ZampliaSyncLogId = entity.ZampliaSyncLogId;
        inputData.CreatedDate = entity.CreatedDate;
        return entity.ZampliaSyncLogId > 0 ? Ok(inputData) : BadRequest("Unable to save Zamplia log.");
    }

    [NonAction]
    public async Task<ZampliaProcessResult> ProcessAllocatedSurveysPayloadAsync(string rawJson, string source, int? userId)
    {
        var result = new ZampliaProcessResult();
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            result.Errors.Add("Allocated surveys payload is empty.");
            return result;
        }

        try
        {
            using var document = JsonDocument.Parse(rawJson);
            var surveyItems = ZampliaJsonHelper.ExtractArrayCandidates(document.RootElement, "surveys", "Surveys", "allocatedSurveys", "AllocatedSurveys");
            if (surveyItems.Count == 0 &&
                document.RootElement.ValueKind == JsonValueKind.Object &&
                ResolveSurveyId(ResolveSurveyElement(document.RootElement)) > 0)
            {
                surveyItems.Add(document.RootElement.Clone());
            }

            var activeSurveyIds = new HashSet<long>();
            var skipInactiveReconciliation = false;
            foreach (var surveyItem in surveyItems)
            {
                try
                {
                    var surveyId = ResolveSurveyId(ResolveSurveyElement(surveyItem));
                    if (surveyId <= 0)
                    {
                        result.SkippedCount++;
                        result.Errors.Add("Allocated survey row did not include a valid survey id.");
                        skipInactiveReconciliation = true;
                        continue;
                    }

                    var isActive = ResolveSurveyIsActive(surveyItem);
                    if (isActive)
                    {
                        activeSurveyIds.Add(surveyId);
                    }

                    var entityId = await UpsertSurveyAsync(surveyItem.GetRawText(), source, userId, isActive);
                    if (entityId > 0)
                    {
                        result.ProcessedCount++;
                        result.SurveyEntityIds.Add(entityId);
                        if (isActive)
                        {
                            result.SurveyIds.Add(surveyId);
                        }
                    }
                    else
                    {
                        result.SkippedCount++;
                    }
                }
                catch (Exception ex)
                {
                    result.SkippedCount++;
                    result.Errors.Add(ex.Message);
                }
            }

            if (!skipInactiveReconciliation)
            {
                var inactiveSurveyIds = await MarkMissingSurveysInactiveAsync(activeSurveyIds, userId, source);
                result.InactivatedCount = inactiveSurveyIds.Count;
                result.InactivatedSurveyIds.AddRange(inactiveSurveyIds);
            }
            else if (surveyItems.Count > 0)
            {
                result.Errors.Add("Existing Zamplia surveys were not fully reconciled because one or more payload rows were missing survey ids.");
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add(ex.Message);
        }

        return result;
    }

    [NonAction]
    public async Task<ZampliaInactiveSurveyCleanupResult> RemoveInactiveSurveysAsync(int? userId, string source)
    {
        var result = new ZampliaInactiveSurveyCleanupResult();
        using var connection = await CreateOpenConnectionAsync();
        using var transaction = connection.BeginTransaction();
        try
        {
            var inactiveSurveys = (await connection.QueryAsync<ZampliaSurveyKeyRow>(
                @"SELECT ZampliaSurveyId, SurveyId
                  FROM ZampliaSurvey
                  WHERE ISNULL(IsActive, 0) = 0",
                transaction: transaction)).ToList();

            if (inactiveSurveys.Count == 0)
            {
                transaction.Commit();
                return result;
            }

            var surveyEntityIds = inactiveSurveys.Select(item => item.ZampliaSurveyId).Distinct().ToList();

            // Leave the VT Projects tables alone and purge only Zamplia-owned survey data.
            await connection.ExecuteAsync(
                @"DELETE FROM ZampliaSurveyQualification WHERE ZampliaSurveyId IN @SurveyEntityIds",
                new { SurveyEntityIds = surveyEntityIds },
                transaction);
            await connection.ExecuteAsync(
                @"DELETE FROM ZampliaSurveyQuota WHERE ZampliaSurveyId IN @SurveyEntityIds",
                new { SurveyEntityIds = surveyEntityIds },
                transaction);
            await connection.ExecuteAsync(
                @"DELETE FROM ZampliaProjectMap WHERE ZampliaSurveyId IN @SurveyEntityIds",
                new { SurveyEntityIds = surveyEntityIds },
                transaction);
            await connection.ExecuteAsync(
                @"DELETE FROM ZampliaEntryLink WHERE ZampliaSurveyId IN @SurveyEntityIds",
                new { SurveyEntityIds = surveyEntityIds },
                transaction);
            await connection.ExecuteAsync(
                @"DELETE FROM ZampliaRespondentAttempt WHERE ZampliaSurveyId IN @SurveyEntityIds",
                new { SurveyEntityIds = surveyEntityIds },
                transaction);
            await connection.ExecuteAsync(
                @"DELETE FROM ZampliaSurvey WHERE ZampliaSurveyId IN @SurveyEntityIds",
                new { SurveyEntityIds = surveyEntityIds },
                transaction);

            transaction.Commit();

            result.RemovedSurveyIds.AddRange(inactiveSurveys.Select(item => item.SurveyId).Distinct());
            result.RemovedCount = result.RemovedSurveyIds.Count;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }

        if (result.RemovedCount > 0)
        {
            await InsertLogAsync(new ZampliaSyncLogDTO
            {
                ModuleName = "Zamplia",
                ActionName = "RemoveInactiveSurveys",
                Source = source,
                ResponseStatusCode = StatusCodes.Status200OK,
                ResponseBodySnapshot = $"Removed {result.RemovedCount} inactive survey(s) permanently from Zamplia working tables.",
                IsSuccess = true,
                RelatedSurveyId = result.RemovedCount == 1 ? result.RemovedSurveyIds[0] : null,
                StartedOn = DateTime.Now,
                CompletedOn = DateTime.Now,
                CreatedBy = userId
            });
        }

        return result;
    }

    [NonAction]
    public async Task<int> UpsertSurveyAsync(string rawJson, string source, int? userId, bool isActive = true)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            return 0;
        }

        using var document = JsonDocument.Parse(rawJson);
        var surveyElement = ResolveSurveyElement(document.RootElement);
        var surveyId = ResolveSurveyId(surveyElement);
        if (surveyId <= 0)
        {
            return 0;
        }

        var dto = new ZampliaSurveyDTO
        {
            SurveyId = surveyId,
            SurveyName = ZampliaJsonHelper.GetJsonString(surveyElement, "SurveyName", "surveyName", "name", "Name", "title", "Title"),
            CPI = ZampliaJsonHelper.GetJsonDecimal(surveyElement, "CPI", "cpi", "reward", "Reward"),
            LOI = ZampliaJsonHelper.GetJsonInt(surveyElement, "LOI", "loi", "LengthOfInterview", "lengthOfInterview"),
            IR = ZampliaJsonHelper.GetJsonDecimal(surveyElement, "IR", "ir", "IncidenceRate", "incidenceRate"),
            LanguageCode = ZampliaJsonHelper.GetJsonString(surveyElement, "LanguageCode", "languageCode", "language", "Language"),
            LanguageId = ZampliaJsonHelper.GetJsonInt(surveyElement, "LanguageId", "languageId"),
            SurveyEndDate = ZampliaJsonHelper.GetJsonDateTime(surveyElement, "SurveyEndDate", "surveyEndDate", "EndDate", "endDate"),
            Device = ZampliaJsonHelper.GetJsonString(surveyElement, "Device", "device"),
            IndustryId = ZampliaJsonHelper.GetJsonString(surveyElement, "IndustryId", "industryId"),
            StudyTypes = ZampliaJsonHelper.GetJsonString(surveyElement, "StudyTypes", "studyTypes", "StudyType", "studyType"),
            IsRecontactSurvey = ZampliaJsonHelper.GetJsonBoolean(surveyElement, "IsRecontactSurvey", "isRecontactSurvey"),
            CollectPII = ZampliaJsonHelper.GetJsonBoolean(surveyElement, "CollectPII", "collectPII"),
            Conversion = ZampliaJsonHelper.GetJsonDecimal(surveyElement, "Conversion", "conversion"),
            TotalCompleteRequired = ZampliaJsonHelper.GetJsonInt(surveyElement, "TotalCompleteRequired", "totalCompleteRequired", "CompletesRequired", "completesRequired"),
            LastVendorUpdatedOn = ZampliaJsonHelper.GetJsonDateTime(surveyElement, "LastVendorUpdatedOn", "lastVendorUpdatedOn", "ModifiedOn", "modifiedOn", "LastUpdateTimeStamp", "lastUpdateTimeStamp"),
            LastSyncedOn = DateTime.Now,
            LocalState = isActive ? "Synced" : "Inactive",
            RawJson = rawJson,
            IsActive = isActive,
            CreatedBy = userId,
            ModifiedBy = userId
        };

        var entityId = await SaveSurveyEntityAsync(dto);
        await InsertLogAsync(new ZampliaSyncLogDTO
        {
            ModuleName = "Zamplia",
            ActionName = "UpsertSurvey",
            Source = source,
            RequestBodySnapshot = rawJson,
            ResponseStatusCode = entityId > 0 ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest,
            ResponseBodySnapshot = entityId > 0
                ? isActive
                    ? $"Survey {surveyId} synced."
                    : $"Survey {surveyId} marked inactive."
                : $"Survey {surveyId} skipped.",
            IsSuccess = entityId > 0,
            RelatedEntityId = entityId > 0 ? entityId : null,
            RelatedSurveyId = surveyId,
            StartedOn = DateTime.Now,
            CompletedOn = DateTime.Now,
            CreatedBy = userId
        });

        return entityId;
    }

    [NonAction]
    public async Task<List<long>> MarkMissingSurveysInactiveAsync(IReadOnlyCollection<long> activeSurveyIds, int? userId, string source)
    {
        using var connection = await CreateOpenConnectionAsync();
        using var transaction = connection.BeginTransaction();
        try
        {
            var surveysToInactivate = (await connection.QueryAsync<ZampliaSurveyKeyRow>(
                activeSurveyIds.Count > 0
                    ? @"SELECT ZampliaSurveyId, SurveyId
                        FROM ZampliaSurvey
                        WHERE IsActive = 1
                          AND SurveyId NOT IN @ActiveSurveyIds"
                    : @"SELECT ZampliaSurveyId, SurveyId
                        FROM ZampliaSurvey
                        WHERE IsActive = 1",
                activeSurveyIds.Count > 0 ? new { ActiveSurveyIds = activeSurveyIds } : null,
                transaction)).ToList();

            if (surveysToInactivate.Count == 0)
            {
                transaction.Commit();
                return new List<long>();
            }

            var surveyEntityIds = surveysToInactivate.Select(item => item.ZampliaSurveyId).Distinct().ToList();
            await connection.ExecuteAsync(
                @"UPDATE ZampliaSurvey
                  SET IsActive = 0,
                      LocalState = @LocalState,
                      LastSyncedOn = @Now,
                      ModifiedDate = @Now,
                      ModifiedBy = @ModifiedBy
                  WHERE ZampliaSurveyId IN @SurveyEntityIds",
                new
                {
                    LocalState = "Inactive",
                    Now = DateTime.Now,
                    ModifiedBy = userId,
                    SurveyEntityIds = surveyEntityIds
                },
                transaction);

            transaction.Commit();

            var inactiveSurveyIds = surveysToInactivate.Select(item => item.SurveyId).Distinct().ToList();
            if (inactiveSurveyIds.Count > 0)
            {
                await InsertLogAsync(new ZampliaSyncLogDTO
                {
                    ModuleName = "Zamplia",
                    ActionName = "MarkInactiveSurveys",
                    Source = source,
                    ResponseStatusCode = StatusCodes.Status200OK,
                    ResponseBodySnapshot = $"Marked {inactiveSurveyIds.Count} survey(s) inactive because they are missing or inactive in the latest allocated payload.",
                    IsSuccess = true,
                    RelatedSurveyId = inactiveSurveyIds.Count == 1 ? inactiveSurveyIds[0] : null,
                    StartedOn = DateTime.Now,
                    CompletedOn = DateTime.Now,
                    CreatedBy = userId
                });
            }

            return inactiveSurveyIds;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    [NonAction]
    public async Task<int> UpsertQualificationsAsync(int zampliaSurveyId, long surveyId, string rawJson, int? userId)
    {
        if (zampliaSurveyId <= 0 || surveyId <= 0)
        {
            return 0;
        }

        using var connection = await CreateOpenConnectionAsync();
        using var transaction = connection.BeginTransaction();
        try
        {
            await connection.ExecuteAsync(
                @"DELETE FROM ZampliaSurveyQualification WHERE ZampliaSurveyId = @ZampliaSurveyId",
                new { ZampliaSurveyId = zampliaSurveyId },
                transaction);

            using var document = JsonDocument.Parse(string.IsNullOrWhiteSpace(rawJson) ? "[]" : rawJson);
            var items = ZampliaJsonHelper.ExtractArrayCandidates(document.RootElement, "Qualifications", "qualifications", "Questions", "questions");
            var inserted = 0;
            var sortOrder = 1;

            foreach (var item in items)
            {
                var row = new ZampliaSurveyQualification
                {
                    ZampliaSurveyId = zampliaSurveyId,
                    SurveyId = surveyId,
                    QuestionId = ZampliaJsonHelper.GetJsonInt(item, "QuestionId", "questionId"),
                    QuestionText = ZampliaJsonHelper.GetJsonString(item, "QuestionText", "questionText"),
                    QuestionType = ZampliaJsonHelper.GetJsonString(item, "QuestionType", "questionType"),
                    LogicalOperator = ZampliaJsonHelper.GetJsonString(item, "LogicalOperator", "logicalOperator", "Operator", "operator"),
                    AnswerCodesJson = ZampliaJsonHelper.GetJsonString(item, "AnswerCodesJson", "answerCodesJson", "AnswerCodes", "answerCodes", "Answers", "answers") ?? item.GetRawText(),
                    RawJson = item.GetRawText(),
                    SortOrder = sortOrder++,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                await connection.ExecuteAsync(
                    @"INSERT INTO ZampliaSurveyQualification
                      (ZampliaSurveyId, SurveyId, QuestionId, QuestionText, QuestionType, LogicalOperator, AnswerCodesJson, RawJson, SortOrder, IsActive, CreatedDate)
                      VALUES
                      (@ZampliaSurveyId, @SurveyId, @QuestionId, @QuestionText, @QuestionType, @LogicalOperator, @AnswerCodesJson, @RawJson, @SortOrder, @IsActive, @CreatedDate)",
                    row,
                    transaction);

                inserted++;
            }

            transaction.Commit();
            return inserted;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    [NonAction]
    public async Task<int> UpsertQuotasAsync(int zampliaSurveyId, long surveyId, string rawJson, int? userId)
    {
        if (zampliaSurveyId <= 0 || surveyId <= 0)
        {
            return 0;
        }

        using var connection = await CreateOpenConnectionAsync();
        using var transaction = connection.BeginTransaction();
        try
        {
            await connection.ExecuteAsync(
                @"DELETE FROM ZampliaSurveyQuota WHERE ZampliaSurveyId = @ZampliaSurveyId",
                new { ZampliaSurveyId = zampliaSurveyId },
                transaction);

            using var document = JsonDocument.Parse(string.IsNullOrWhiteSpace(rawJson) ? "[]" : rawJson);
            var items = ZampliaJsonHelper.ExtractArrayCandidates(document.RootElement, "Quotas", "quotas", "QuotaDetails", "quotaDetails");
            var inserted = 0;
            var sortOrder = 1;

            foreach (var item in items)
            {
                var row = new ZampliaSurveyQuota
                {
                    ZampliaSurveyId = zampliaSurveyId,
                    SurveyId = surveyId,
                    QuotaId = ZampliaJsonHelper.GetJsonLong(item, "QuotaId", "quotaId"),
                    QuotaName = ZampliaJsonHelper.GetJsonString(item, "QuotaName", "quotaName", "Name", "name"),
                    NumberOfRespondents = ZampliaJsonHelper.GetJsonInt(item, "NumberOfRespondents", "numberOfRespondents", "Remaining", "remaining"),
                    Conversion = ZampliaJsonHelper.GetJsonDecimal(item, "Conversion", "conversion"),
                    QuestionsJson = ZampliaJsonHelper.GetJsonString(item, "QuestionsJson", "questionsJson", "Questions", "questions") ?? item.GetRawText(),
                    RawJson = item.GetRawText(),
                    SortOrder = sortOrder++,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                await connection.ExecuteAsync(
                    @"INSERT INTO ZampliaSurveyQuota
                      (ZampliaSurveyId, SurveyId, QuotaId, QuotaName, NumberOfRespondents, Conversion, QuestionsJson, RawJson, SortOrder, IsActive, CreatedDate)
                      VALUES
                      (@ZampliaSurveyId, @SurveyId, @QuotaId, @QuotaName, @NumberOfRespondents, @Conversion, @QuestionsJson, @RawJson, @SortOrder, @IsActive, @CreatedDate)",
                    row,
                    transaction);

                inserted++;
            }

            transaction.Commit();
            return inserted;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    [NonAction]
    public async Task<ZampliaLaunchContextDTO?> GetZampliaLaunchContextAsync(int? zampliaSurveyId = null, int? internalProjectId = null, int? internalProjectMappingId = null, string? projectMappingSid = null)
    {
        const string query = @"
            SELECT TOP 1
                s.ZampliaSurveyId,
                s.SurveyId,
                s.SurveyName,
                s.LanguageId,
                SurveyRawJson = s.RawJson,
                InternalProjectId = COALESCE(pm.InternalProjectId, p.ProjectId, mapBySid.ProjectID),
                InternalProjectUrlId = COALESCE(pm.InternalProjectUrlId, puById.ID, puFallback.ID),
                InternalProjectMappingId = COALESCE(pm.InternalProjectMappingId, mapById.ID, mapBySid.ID, mapFallback.ID),
                ProjectMappingSid = COALESCE(mapById.SID, mapBySid.SID, mapFallback.SID),
                ProjectMappingCode = COALESCE(mapById.Code, mapBySid.Code, mapFallback.Code),
                ProjectFrom = p.ProjectFrom,
                CountryId = p.CountryId,
                SupplierId = COALESCE(mapById.SupplierID, mapBySid.SupplierID, mapFallback.SupplierID, settingRow.DefaultSupplierId),
                settingRow.BaseUrl,
                settingRow.ApiKey,
                settingRow.ExitHmacKey,
                settingRow.UseConsultingsBridge,
                ExistingProjectUrl = COALESCE(puById.Url, puFallback.Url),
                ExistingMaskingUrl = COALESCE(mapById.MLink, mapBySid.MLink, mapFallback.MLink)
            FROM ZampliaSurvey s
            OUTER APPLY (
                SELECT TOP 1 pm.*
                FROM ZampliaProjectMap pm
                INNER JOIN Projects mappedProject
                    ON mappedProject.ProjectId = pm.InternalProjectId
                   AND ISNULL(mappedProject.IsActive, 0) = 1
                WHERE pm.ZampliaSurveyId = s.ZampliaSurveyId
                  AND pm.IsActive = 1
                  AND (@InternalProjectId IS NULL OR pm.InternalProjectId = @InternalProjectId)
                  AND (@InternalProjectMappingId IS NULL OR pm.InternalProjectMappingId = @InternalProjectMappingId)
                ORDER BY pm.Id DESC
            ) pm
            OUTER APPLY (
                SELECT TOP 1 map.*
                FROM ProjectMapping map
                WHERE @ProjectMappingSid IS NOT NULL
                  AND map.SID = @ProjectMappingSid
                ORDER BY map.ID DESC
            ) mapBySid
            OUTER APPLY (
                SELECT TOP 1 p.*
                FROM Projects p
                WHERE p.ProjectFrom = 'Zamplia'
                  AND ISNULL(p.IsActive, 0) = 1
                  AND (@InternalProjectId IS NULL OR p.ProjectId = @InternalProjectId)
                  AND (
                        p.ProjectId = COALESCE(pm.InternalProjectId, mapBySid.ProjectID, 0)
                        OR TRY_CONVERT(bigint, p.ProjectIdFromApi) = s.SurveyId
                      )
                ORDER BY CASE WHEN p.ProjectId = COALESCE(pm.InternalProjectId, mapBySid.ProjectID, 0) THEN 0 ELSE 1 END, p.ProjectId DESC
            ) p
            OUTER APPLY (SELECT TOP 1 * FROM ProjectsUrl pu WHERE pu.ID = pm.InternalProjectUrlId) puById
            OUTER APPLY (
                SELECT TOP 1 *
                FROM ProjectsUrl pu
                WHERE pu.PID = COALESCE(pm.InternalProjectId, p.ProjectId, mapBySid.ProjectID)
                  AND (p.CountryId IS NULL OR pu.CID = p.CountryId)
                ORDER BY CASE WHEN pu.CID = p.CountryId THEN 0 ELSE 1 END, pu.ID DESC
            ) puFallback
            OUTER APPLY (SELECT TOP 1 * FROM ProjectMapping map WHERE map.ID = COALESCE(pm.InternalProjectMappingId, @InternalProjectMappingId)) mapById
            OUTER APPLY (
                SELECT TOP 1 *
                FROM ProjectMapping map
                WHERE map.ProjectID = COALESCE(pm.InternalProjectId, p.ProjectId, mapBySid.ProjectID)
                  AND (@ProjectMappingSid IS NULL OR map.SID = @ProjectMappingSid OR map.ID = COALESCE(pm.InternalProjectMappingId, @InternalProjectMappingId))
                  AND (p.CountryId IS NULL OR map.CountryID = p.CountryId)
                ORDER BY CASE WHEN map.SID = @ProjectMappingSid THEN 0 ELSE 1 END,
                         CASE WHEN map.ID = COALESCE(pm.InternalProjectMappingId, @InternalProjectMappingId) THEN 0 ELSE 1 END,
                         CASE WHEN map.CountryID = p.CountryId THEN 0 ELSE 1 END,
                         map.ID DESC
            ) mapFallback
            OUTER APPLY (SELECT TOP 1 * FROM ZampliaSetting ORDER BY IsActive DESC, ZampliaSettingId DESC) settingRow
            WHERE (@ZampliaSurveyId IS NULL OR s.ZampliaSurveyId = @ZampliaSurveyId)
              AND (@InternalProjectId IS NULL OR COALESCE(pm.InternalProjectId, p.ProjectId, mapBySid.ProjectID) = @InternalProjectId)
              AND (@InternalProjectMappingId IS NULL OR COALESCE(pm.InternalProjectMappingId, mapById.ID, mapBySid.ID, mapFallback.ID) = @InternalProjectMappingId)
              AND (@ProjectMappingSid IS NULL OR COALESCE(mapById.SID, mapBySid.SID, mapFallback.SID) = @ProjectMappingSid)
            ORDER BY CASE WHEN COALESCE(mapById.SID, mapBySid.SID, mapFallback.SID) = @ProjectMappingSid THEN 0 ELSE 1 END,
                     CASE WHEN COALESCE(pm.InternalProjectId, p.ProjectId, mapBySid.ProjectID) = @InternalProjectId THEN 0 ELSE 1 END,
                     s.ZampliaSurveyId DESC";

        return await _unitOfWork.ZampliaProjectMap.GetEntityData<ZampliaLaunchContextDTO>(query, new
        {
            ZampliaSurveyId = zampliaSurveyId,
            InternalProjectId = internalProjectId,
            InternalProjectMappingId = internalProjectMappingId,
            ProjectMappingSid = string.IsNullOrWhiteSpace(projectMappingSid) ? null : projectMappingSid.Trim()
        });
    }

    [NonAction]
    public async Task<bool> SyncZampliaProjectLaunchUrlAsync(int zampliaSurveyId, string launchUrl)
    {
        if (zampliaSurveyId <= 0 || string.IsNullOrWhiteSpace(launchUrl))
        {
            return false;
        }

        var context = await GetZampliaLaunchContextAsync(zampliaSurveyId: zampliaSurveyId);
        if (context == null || !context.InternalProjectId.HasValue || context.InternalProjectId.Value <= 0)
        {
            return false;
        }

        var updated = false;
        if (context.InternalProjectUrlId.HasValue && context.InternalProjectUrlId.Value > 0)
        {
            var projectUrl = await _unitOfWork.ProjectsUrl.FindByIdAsync(context.InternalProjectUrlId.Value);
            if (projectUrl != null)
            {
                projectUrl.Url = launchUrl;
                projectUrl.OriginalUrl ??= launchUrl;
                updated = await _unitOfWork.ProjectsUrl.UpdateAsync(projectUrl) || updated;
            }
        }

        if (context.InternalProjectMappingId.HasValue && context.InternalProjectMappingId.Value > 0)
        {
            var projectMapping = await _unitOfWork.ProjectMapping.FindByIdAsync(context.InternalProjectMappingId.Value);
            if (projectMapping != null)
            {
                projectMapping.Olink = launchUrl;
                updated = await _unitOfWork.ProjectMapping.UpdateAsync(projectMapping) || updated;
            }
        }

        return updated;
    }

    [NonAction]
    public async Task<ZampliaRespondentAttemptDTO?> GetLatestZampliaRespondentAttemptAsync(string? transactionId = null, string? respondentId = null, int? zampliaSurveyId = null)
    {
        const string query = @"
            SELECT TOP 1 *
            FROM ZampliaRespondentAttempt
            WHERE (@TransactionId IS NULL OR TransactionId = @TransactionId)
              AND (@RespondentId IS NULL OR RespondentId = @RespondentId)
              AND (@ZampliaSurveyId IS NULL OR ZampliaSurveyId = @ZampliaSurveyId)
            ORDER BY ISNULL(CompletedOn, ISNULL(AttemptedOn, CreatedDate)) DESC, Id DESC";

        return await _unitOfWork.ZampliaRespondentAttempt.GetEntityData<ZampliaRespondentAttemptDTO>(query, new
        {
            TransactionId = string.IsNullOrWhiteSpace(transactionId) ? null : transactionId.Trim(),
            RespondentId = string.IsNullOrWhiteSpace(respondentId) ? null : respondentId.Trim(),
            ZampliaSurveyId = zampliaSurveyId
        });
    }

    [NonAction]
    public async Task<int> CreateReconciliationRunAsync(ZampliaReconciliationRunDTO run, IReadOnlyCollection<ZampliaReconciliationItemDTO> items)
    {
        using var connection = await CreateOpenConnectionAsync();
        using var transaction = connection.BeginTransaction();
        try
        {
            const string insertRunSql = @"
                INSERT INTO ZampliaReconciliationRun
                (RunType, SurveyId, InternalProjectId, TransactionId, RunScopeJson, StartedOn, CompletedOn, Success, Notes, TotalReviewed, TotalMatched, TotalMismatched, CompleteCount, TerminateCount, OverQuotaCount, QualityTerminationCount, SecurityTerminationCount, OpenCount, UnknownCount, CreatedBy, CreatedDate)
                VALUES
                (@RunType, @SurveyId, @InternalProjectId, @TransactionId, @RunScopeJson, @StartedOn, @CompletedOn, @Success, @Notes, @TotalReviewed, @TotalMatched, @TotalMismatched, @CompleteCount, @TerminateCount, @OverQuotaCount, @QualityTerminationCount, @SecurityTerminationCount, @OpenCount, @UnknownCount, @CreatedBy, @CreatedDate);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            var runId = await connection.ExecuteScalarAsync<int>(insertRunSql, run, transaction);
            const string insertItemSql = @"
                INSERT INTO ZampliaReconciliationItem
                (ReconciliationRunId, ZampliaRespondentAttemptId, SurveyId, InternalProjectId, TransactionId, RespondentId, SessionId, LocalStatus, VendorStatus, FinalStatus, FinalStatusSource, IsMismatch, MismatchType, Notes, RawSnapshotJson, CreatedDate)
                VALUES
                (@ReconciliationRunId, @ZampliaRespondentAttemptId, @SurveyId, @InternalProjectId, @TransactionId, @RespondentId, @SessionId, @LocalStatus, @VendorStatus, @FinalStatus, @FinalStatusSource, @IsMismatch, @MismatchType, @Notes, @RawSnapshotJson, @CreatedDate);";

            foreach (var item in items)
            {
                item.ReconciliationRunId = runId;
                await connection.ExecuteAsync(insertItemSql, item, transaction);
            }

            transaction.Commit();
            return runId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    [NonAction]
    public async Task InsertLogAsync(ZampliaSyncLogDTO log)
    {
        var entity = _mapper.Map<ZampliaSyncLog>(log);
        TrimZampliaLogEntity(entity);
        entity.CreatedDate ??= DateTime.Now;
        await _unitOfWork.ZampliaSyncLog.AddAsync(entity);
    }

    private static void TrimZampliaEntryLinkEntity(ZampliaEntryLink entity)
    {
        entity.TransactionId = TrimForStorage(entity.TransactionId, 150);
        entity.VendorLink = TrimForStorage(entity.VendorLink, 2000);
        entity.InternalLaunchUrl = TrimForStorage(entity.InternalLaunchUrl, 2000);
        entity.RawJson = TrimForStorage(entity.RawJson, 4000);
    }

    private static void TrimZampliaRespondentAttemptEntity(ZampliaRespondentAttempt entity)
    {
        entity.RespondentId = TrimForStorage(entity.RespondentId, 150);
        entity.TransactionId = TrimForStorage(entity.TransactionId, 150);
        entity.SessionId = TrimForStorage(entity.SessionId, 150);
        entity.IpAddress = TrimForStorage(entity.IpAddress, 100);
        entity.LaunchUrl = TrimForStorage(entity.LaunchUrl, 2000);
        entity.VendorLaunchUrl = TrimForStorage(entity.VendorLaunchUrl, 2000);
        entity.ReturnUrl = TrimForStorage(entity.ReturnUrl, 2000);
        entity.ReturnRawQuery = TrimForStorage(entity.ReturnRawQuery, 2000);
        entity.ReturnCode = TrimForStorage(entity.ReturnCode, 100);
        entity.ReturnStatus = TrimForStorage(entity.ReturnStatus, 100);
        entity.FinalStatus = TrimForStorage(entity.FinalStatus, 100);
        entity.FinalStatusSource = TrimForStorage(entity.FinalStatusSource, 50);
        entity.HmacReceived = TrimForStorage(entity.HmacReceived, 500);
        entity.HmacCalculated = TrimForStorage(entity.HmacCalculated, 500);
        entity.RawJson = TrimForStorage(entity.RawJson, 4000);
    }

    private static void TrimZampliaLogEntity(ZampliaSyncLog entity)
    {
        entity.ModuleName = TrimForStorage(entity.ModuleName, 100);
        entity.ActionName = TrimForStorage(entity.ActionName, 100);
        entity.RequestUrl = TrimForStorage(entity.RequestUrl, 1000);
        entity.RequestBodySnapshot = TrimForStorage(entity.RequestBodySnapshot, 4000);
        entity.ResponseBodySnapshot = TrimForStorage(entity.ResponseBodySnapshot, 4000);
        entity.Source = TrimForStorage(entity.Source, 50);
        entity.ErrorText = TrimForStorage(entity.ErrorText, 4000);
    }

    private static string? TrimForStorage(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var trimmed = value.Trim();
        if (trimmed.Length <= maxLength)
        {
            return trimmed;
        }

        return trimmed[..(maxLength - 3)] + "...";
    }

    [NonAction]
    public async Task<ZampliaSetting?> GetCurrentSettingEntityAsync(int requestedId)
    {
        if (requestedId > 0)
        {
            return await _unitOfWork.ZampliaSetting.FindByIdAsync(requestedId);
        }

        return await _unitOfWork.ZampliaSetting.GetEntityData<ZampliaSetting>(
            @"SELECT TOP 1 *
              FROM ZampliaSetting
              ORDER BY IsActive DESC, ZampliaSettingId DESC");
    }

    private async Task<int> SaveSurveyEntityAsync(ZampliaSurveyDTO inputData)
    {
        var current = inputData.ZampliaSurveyId > 0
            ? await _unitOfWork.ZampliaSurvey.FindByIdAsync(inputData.ZampliaSurveyId)
            : await _unitOfWork.ZampliaSurvey.GetEntityData<ZampliaSurvey>(
                @"SELECT TOP 1 *
                  FROM ZampliaSurvey
                  WHERE SurveyId = @SurveyId
                  ORDER BY ZampliaSurveyId DESC",
                new { inputData.SurveyId });

        if (current == null)
        {
            var entity = _mapper.Map<ZampliaSurvey>(inputData);
            entity.CreatedDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;
            entity.ZampliaSurveyId = await _unitOfWork.ZampliaSurvey.AddAsync(entity);
            return entity.ZampliaSurveyId;
        }

        current.SurveyName = inputData.SurveyName;
        current.CPI = inputData.CPI;
        current.LOI = inputData.LOI;
        current.IR = inputData.IR;
        current.LanguageCode = inputData.LanguageCode;
        current.LanguageId = inputData.LanguageId;
        current.SurveyEndDate = inputData.SurveyEndDate;
        current.Device = inputData.Device;
        current.IndustryId = inputData.IndustryId;
        current.StudyTypes = inputData.StudyTypes;
        current.IsRecontactSurvey = inputData.IsRecontactSurvey;
        current.CollectPII = inputData.CollectPII;
        current.Conversion = inputData.Conversion;
        current.TotalCompleteRequired = inputData.TotalCompleteRequired;
        current.LastVendorUpdatedOn = inputData.LastVendorUpdatedOn;
        current.LastSyncedOn = inputData.LastSyncedOn ?? DateTime.Now;
        current.LocalState = inputData.LocalState;
        current.RawJson = inputData.RawJson;
        current.IsActive = inputData.IsActive;
        current.ModifiedDate = DateTime.Now;
        current.ModifiedBy = inputData.ModifiedBy;
        current.CreatedBy ??= inputData.CreatedBy;

        return await _unitOfWork.ZampliaSurvey.UpdateAsync(current) ? current.ZampliaSurveyId : 0;
    }

    private static JsonElement ResolveSurveyElement(JsonElement root)
    {
        var surveyElement = ZampliaJsonHelper.ResolveObjectCandidate(root, "survey", "Survey", "data", "Data");
        if (surveyElement.ValueKind == JsonValueKind.Object)
        {
            return surveyElement;
        }

        return root.Clone();
    }

    private static long ResolveSurveyId(JsonElement surveyElement)
    {
        return ZampliaJsonHelper.GetJsonLong(surveyElement, "SurveyId", "surveyId", "survey_id", "Id", "id") ?? 0;
    }

    private static bool ResolveSurveyIsActive(JsonElement surveyElement)
    {
        var resolvedSurvey = ResolveSurveyElement(surveyElement);

        var activeFlag = ZampliaJsonHelper.GetJsonBoolean(
            resolvedSurvey,
            "IsActive",
            "isActive",
            "Active",
            "active",
            "IsAvailable",
            "isAvailable",
            "Available",
            "available",
            "Enabled",
            "enabled");
        if (activeFlag.HasValue)
        {
            return activeFlag.Value;
        }

        var inactiveFlag = ZampliaJsonHelper.GetJsonBoolean(
            resolvedSurvey,
            "IsInactive",
            "isInactive",
            "Inactive",
            "inactive",
            "IsClosed",
            "isClosed",
            "Closed",
            "closed",
            "Archived",
            "archived",
            "IsRemoved",
            "isRemoved",
            "Removed",
            "removed");
        if (inactiveFlag.HasValue)
        {
            return !inactiveFlag.Value;
        }

        var status = ZampliaJsonHelper.GetJsonString(resolvedSurvey, "Status", "status", "SurveyStatus", "surveyStatus", "State", "state");
        if (string.IsNullOrWhiteSpace(status))
        {
            return true;
        }

        var normalizedStatus = status.Trim().ToLowerInvariant();
        if (normalizedStatus.Contains("inactive") ||
            normalizedStatus.Contains("closed") ||
            normalizedStatus.Contains("archived") ||
            normalizedStatus.Contains("paused") ||
            normalizedStatus.Contains("deleted") ||
            normalizedStatus.Contains("disabled") ||
            normalizedStatus.Contains("unavailable"))
        {
            return false;
        }

        return true;
    }

    private async Task<SqlConnection> CreateOpenConnectionAsync()
    {
        var connection = new SqlConnection(_dapperDBSetting.ConnectionString);
        await connection.OpenAsync();
        return connection;
    }

    private sealed class ZampliaSurveyKeyRow
    {
        public int ZampliaSurveyId { get; set; }
        public long SurveyId { get; set; }
    }
}








