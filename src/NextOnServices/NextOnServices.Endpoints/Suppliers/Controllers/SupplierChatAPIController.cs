using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.ViewModels.Supplier;

namespace NextOnServices.Endpoints.Suppliers;

[Route("api/supplier-chat")]
[ApiController]
[Authorize]
public class SupplierChatAPIController : ControllerBase
{
    private const int DefaultPageSize = 50;
    private const int DefaultPollPageSize = 20;
    private const int MaxPageSize = 200;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SupplierChatAPIController> _logger;

    public SupplierChatAPIController(IUnitOfWork unitOfWork, ILogger<SupplierChatAPIController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet("history")]
    public Task<IActionResult> GetHistory([FromQuery] SupplierChatHistoryRequest request)
    {
        request ??= new SupplierChatHistoryRequest();
        if (request.PageSize <= 0)
        {
            request.PageSize = DefaultPageSize;
        }

        return FetchHistoryAsync(request);
    }

    [HttpGet("poll")]
    public Task<IActionResult> Poll([FromQuery] SupplierChatHistoryRequest request, [FromQuery(Name = "after")] DateTimeOffset? after)
    {
        request ??= new SupplierChatHistoryRequest();
        if (after.HasValue)
        {
            request.SinceCursor = after;
        }

        if (request.PageSize <= 0)
        {
            request.PageSize = DefaultPollPageSize;
        }

        return FetchHistoryAsync(request);
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SupplierChatSendRequest request)
    {
        if (request == null || request.ProjectMappingId <= 0)
        {
            return BadRequest(new { message = "A valid project mapping identifier is required." });
        }

        var trimmedMessage = request.Message?.Trim();
        if (string.IsNullOrWhiteSpace(trimmedMessage))
        {
            return BadRequest(new { message = "Message text is required." });
        }

        if (trimmedMessage.Length > 4000)
        {
            trimmedMessage = trimmedMessage[..4000];
        }

        try
        {
            var mapping = await _unitOfWork.ProjectMapping.GetEntityData<ProjectMapping>(
                "SELECT TOP 1 * FROM ProjectMapping WHERE Id=@Id",
                new { Id = request.ProjectMappingId });

            if (mapping == null)
            {
                return NotFound(new { message = "Project mapping could not be located." });
            }

            if (!TryResolveSupplierContext(request.SupplierId, out var supplierId, out var isSupplierUser, out var failureResult))
            {
                return failureResult!;
            }

            if (isSupplierUser && mapping.SupplierId.HasValue && mapping.SupplierId != supplierId)
            {
                return Forbid();
            }

            var effectiveSupplierId = supplierId ?? mapping.SupplierId;
            if (!effectiveSupplierId.HasValue)
            {
                return BadRequest(new { message = "A supplier association is required for chat messages." });
            }

            var senderIdClaim = User.FindFirst("Id")?.Value;
            int? createdBy = null;
            if (int.TryParse(senderIdClaim, out var parsedSenderId))
            {
                createdBy = parsedSenderId;
            }

            var senderName = User.Identity?.Name ?? "System";
            var utcNow = DateTime.UtcNow;

            var entity = new SupplierProjectMessage
            {
                ProjectMappingId = request.ProjectMappingId,
                ProjectId = request.ProjectId ?? mapping.ProjectId,
                SupplierId = effectiveSupplierId,
                Message = trimmedMessage,
                CreatedBy = createdBy,
                CreatedByName = senderName,
                CreatedUtc = utcNow,
                FromSupplier = isSupplierUser,
                IsRead = false,
                ReadUtc = null
            };

            var newId = await _unitOfWork.SupplierProjectMessages.AddAsync(entity);
            entity.Id = newId;

            var dto = new SupplierProjectMessageDto
            {
                Id = entity.Id,
                ProjectMappingId = entity.ProjectMappingId,
                ProjectId = entity.ProjectId,
                SupplierId = entity.SupplierId,
                Message = entity.Message,
                CreatedBy = entity.CreatedBy,
                CreatedByName = entity.CreatedByName,
                CreatedUtc = entity.CreatedUtc,
                FromSupplier = entity.FromSupplier,
                IsRead = entity.IsRead,
                ReadUtc = entity.ReadUtc
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending supplier chat message for mapping {ProjectMappingId}", request.ProjectMappingId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while sending the message." });
        }
    }

    private async Task<IActionResult> FetchHistoryAsync(SupplierChatHistoryRequest request)
    {
        if (request.ProjectMappingId <= 0)
        {
            return BadRequest(new { message = "A valid project mapping identifier is required." });
        }

        try
        {
            var mapping = await _unitOfWork.ProjectMapping.GetEntityData<ProjectMapping>(
                "SELECT TOP 1 * FROM ProjectMapping WHERE Id=@Id",
                new { Id = request.ProjectMappingId });

            if (mapping == null)
            {
                return NotFound(new { message = "Project mapping could not be located." });
            }

            if (!TryResolveSupplierContext(request.SupplierId, out var supplierId, out var isSupplierUser, out var failureResult))
            {
                return failureResult!;
            }

            if (isSupplierUser && mapping.SupplierId.HasValue && mapping.SupplierId != supplierId)
            {
                return Forbid();
            }

            var effectiveSupplierId = supplierId ?? mapping.SupplierId;
            var pageSize = GetPageSize(request.PageSize);
            var take = pageSize + 1;
            var since = request.SinceCursor?.UtcDateTime;

            List<SupplierProjectMessageListItemDto> rows;
            bool hasMore;

            if (since.HasValue)
            {
                var sql = @"SELECT TOP (@Take)
                                Id,
                                ProjectMappingId,
                                ProjectId,
                                SupplierId,
                                Message,
                                CreatedBy,
                                CreatedByName,
                                CreatedUtc,
                                FromSupplier,
                                IsRead,
                                ReadUtc
                            FROM SupplierProjectMessages
                            WHERE ProjectMappingId = @ProjectMappingId
                              AND (@SupplierId IS NULL OR SupplierId = @SupplierId)
                              AND (@UnreadOnly = 0 OR IsRead = 0)
                              AND CreatedUtc > @SinceCursor
                            ORDER BY CreatedUtc ASC, Id ASC;";

                var parameters = new
                {
                    ProjectMappingId = request.ProjectMappingId,
                    SupplierId = effectiveSupplierId,
                    SinceCursor = since.Value,
                    Take = take,
                    UnreadOnly = request.UnreadOnly ? 1 : 0
                };

                rows = await _unitOfWork.SupplierProjectMessages.GetTableData<SupplierProjectMessageListItemDto>(sql, parameters);
                hasMore = rows.Count > pageSize;
                if (hasMore)
                {
                    rows = rows.Take(pageSize).ToList();
                }
            }
            else
            {
                var sql = @"SELECT TOP (@Take)
                                Id,
                                ProjectMappingId,
                                ProjectId,
                                SupplierId,
                                Message,
                                CreatedBy,
                                CreatedByName,
                                CreatedUtc,
                                FromSupplier,
                                IsRead,
                                ReadUtc
                            FROM SupplierProjectMessages
                            WHERE ProjectMappingId = @ProjectMappingId
                              AND (@SupplierId IS NULL OR SupplierId = @SupplierId)
                              AND (@UnreadOnly = 0 OR IsRead = 0)
                            ORDER BY CreatedUtc DESC, Id DESC;";

                var parameters = new
                {
                    ProjectMappingId = request.ProjectMappingId,
                    SupplierId = effectiveSupplierId,
                    Take = take,
                    UnreadOnly = request.UnreadOnly ? 1 : 0
                };

                rows = await _unitOfWork.SupplierProjectMessages.GetTableData<SupplierProjectMessageListItemDto>(sql, parameters);
                hasMore = rows.Count > pageSize;
                if (hasMore)
                {
                    rows = rows.Take(pageSize).ToList();
                }

                rows = rows
                    .OrderBy(m => m.CreatedUtc)
                    .ThenBy(m => m.Id)
                    .ToList();
            }

            if (!isSupplierUser)
            {
                var readUtc = DateTime.UtcNow;
                var updatedCount = await _unitOfWork.SupplierProjectMessages.MarkSupplierMessagesAsReadAsync(request.ProjectMappingId, readUtc);
                if (updatedCount > 0)
                {
                    foreach (var message in rows.Where(m => m.FromSupplier && !m.IsRead))
                    {
                        message.IsRead = true;
                        message.ReadUtc = readUtc;
                    }
                }
            }

            var nextCursorDate = rows.LastOrDefault()?.CreatedUtc;
            DateTimeOffset? nextCursor = nextCursorDate.HasValue
                ? new DateTimeOffset(DateTime.SpecifyKind(nextCursorDate.Value, DateTimeKind.Utc))
                : null;

            var response = new SupplierChatHistoryResponse
            {
                Messages = rows,
                NextCursor = nextCursor,
                HasMore = hasMore
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving supplier chat history for mapping {ProjectMappingId}", request.ProjectMappingId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while loading the chat history." });
        }
    }

    private static int GetPageSize(int? requested)
    {
        var pageSize = requested.GetValueOrDefault(DefaultPageSize);
        if (pageSize <= 0)
        {
            pageSize = DefaultPageSize;
        }

        return Math.Min(pageSize, MaxPageSize);
    }

    private bool TryResolveSupplierContext(int? requestedSupplierId, out int? supplierId, out bool isSupplierUser, out IActionResult? failureResult)
    {
        supplierId = null;
        isSupplierUser = false;
        failureResult = null;

        var loginSource = User.FindFirst("LoginSource")?.Value;
        if (string.Equals(loginSource, "SupplierLogin", StringComparison.OrdinalIgnoreCase))
        {
            var supplierClaim = User.FindFirst("Id")?.Value;
            if (!int.TryParse(supplierClaim, out var parsedSupplierId))
            {
                failureResult = StatusCode(StatusCodes.Status403Forbidden, new { message = "Supplier identity could not be determined from the current session." });
                return false;
            }

            supplierId = parsedSupplierId;
            isSupplierUser = true;
            return true;
        }

        supplierId = requestedSupplierId;
        return true;
    }
}
