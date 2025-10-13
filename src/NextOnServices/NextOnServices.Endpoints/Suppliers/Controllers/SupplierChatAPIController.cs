using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.ViewModels.Supplier;
using Newtonsoft.Json;

namespace NextOnServices.Endpoints.Suppliers;

[Route("api/supplier-chat")]
[ApiController]
[Authorize]
public class SupplierChatAPIController : ControllerBase
{
    private const int DefaultPageSize = 50;
    private const int DefaultPollPageSize = 20;
    private const int MaxPageSize = 200;
    private const string AttachmentPrefix = "ATTACHMENT::";

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SupplierChatAPIController> _logger;
    private readonly IWebHostEnvironment _environment;

    public SupplierChatAPIController(IUnitOfWork unitOfWork, ILogger<SupplierChatAPIController> logger, IWebHostEnvironment environment)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _environment = environment;
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
            var (context, errorResult) = await PrepareMessageContextAsync(request);
            if (errorResult != null)
            {
                return errorResult;
            }

            request.Message = trimmedMessage;
            return await PersistMessageAsync(request, context!, trimmedMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending supplier chat message for mapping {ProjectMappingId}", request.ProjectMappingId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while sending the message." });
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] SupplierChatAttachmentRequest request)
    {
        if (request == null || request.ProjectMappingId <= 0)
        {
            return BadRequest(new { message = "A valid project mapping identifier is required." });
        }

        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest(new { message = "An attachment file is required." });
        }

        try
        {
            var (context, errorResult) = await PrepareMessageContextAsync(request);
            if (errorResult != null)
            {
                return errorResult;
            }

            var attachment = await SaveAttachmentAsync(request);
            if (attachment == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Unable to store the attachment. Please try again later." });
            }

            var payloadJson = JsonConvert.SerializeObject(new SupplierChatAttachmentPayload
            {
                FileName = attachment.FileName,
                FileUrl = attachment.FileUrl,
                Caption = attachment.Caption,
                ContentType = attachment.ContentType,
                Length = attachment.Length ?? 0
            });

            var messagePayload = $"{AttachmentPrefix}{payloadJson}";
            request.Message = messagePayload;

            return await PersistMessageAsync(request, context!, messagePayload, attachment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving chat attachment for mapping {ProjectMappingId}", request.ProjectMappingId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while uploading the attachment." });
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

            foreach (var row in rows)
            {
                EnsureAttachmentMetadata(row);
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

    private async Task<(MessageContext? Context, IActionResult? ErrorResult)> PrepareMessageContextAsync(SupplierChatSendRequest request)
    {
        var mapping = await _unitOfWork.ProjectMapping.GetEntityData<ProjectMapping>(
            "SELECT TOP 1 * FROM ProjectMapping WHERE Id=@Id",
            new { Id = request.ProjectMappingId });

        if (mapping == null)
        {
            return (null, NotFound(new { message = "Project mapping could not be located." }));
        }

        if (!TryResolveSupplierContext(request.SupplierId, out var supplierId, out var isSupplierUser, out var failureResult))
        {
            return (null, failureResult);
        }

        if (isSupplierUser && mapping.SupplierId.HasValue && mapping.SupplierId != supplierId)
        {
            return (null, Forbid());
        }

        var effectiveSupplierId = supplierId ?? mapping.SupplierId;
        if (!effectiveSupplierId.HasValue)
        {
            return (null, BadRequest(new { message = "A supplier association is required for chat messages." }));
        }

        return (new MessageContext(mapping, effectiveSupplierId.Value, isSupplierUser), null);
    }

    private async Task<IActionResult> PersistMessageAsync(SupplierChatSendRequest request, MessageContext context, string messageText, SupplierChatAttachmentDto? attachment = null)
    {
        var senderIdClaim = User.FindFirst("Id")?.Value;
        int? createdBy = null;
        if (int.TryParse(senderIdClaim, out var parsedSenderId))
        {
            createdBy = parsedSenderId;
        }

        var senderName = await ResolveSenderNameAsync(context);
        var utcNow = DateTime.UtcNow;

        var entity = new SupplierProjectMessage
        {
            ProjectMappingId = request.ProjectMappingId,
            ProjectId = request.ProjectId ?? context.Mapping.ProjectId,
            SupplierId = context.EffectiveSupplierId,
            Message = messageText,
            CreatedBy = createdBy,
            CreatedByName = senderName,
            CreatedUtc = utcNow,
            FromSupplier = context.IsSupplierUser,
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
            ReadUtc = entity.ReadUtc,
            Attachment = attachment
        };

        EnsureAttachmentMetadata(dto);

        return Ok(dto);
    }

    private async Task<string> ResolveSenderNameAsync(MessageContext context)
    {
        if (context.IsSupplierUser)
        {
            try
            {
                var supplier = await _unitOfWork.Suppliers.GetEntityData<Supplier>(
                    "SELECT TOP 1 Name FROM Suppliers WHERE Id=@Id",
                    new { Id = context.EffectiveSupplierId });

                if (!string.IsNullOrWhiteSpace(supplier?.Name))
                {
                    return supplier.Name!.Trim();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to resolve supplier name for mapping {ProjectMappingId}", context.Mapping.Id);
            }
        }

        var fallback = User.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(fallback))
        {
            return fallback!;
        }

        return "System";
    }

    private void EnsureAttachmentMetadata(SupplierProjectMessageDto message)
    {
        if (message == null)
        {
            return;
        }

        var attachment = message.Attachment ?? TryParseAttachmentPayload(message.Message);
        if (attachment == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(attachment.Caption))
        {
            attachment.Caption = attachment.FileName;
        }

        if (!attachment.IsImage && !string.IsNullOrWhiteSpace(attachment.ContentType))
        {
            attachment.IsImage = attachment.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        }

        message.Attachment = attachment;
    }

    private SupplierChatAttachmentDto? TryParseAttachmentPayload(string? messageText)
    {
        if (string.IsNullOrWhiteSpace(messageText))
        {
            return null;
        }

        if (!messageText.StartsWith(AttachmentPrefix, StringComparison.Ordinal))
        {
            return null;
        }

        var payloadJson = messageText[AttachmentPrefix.Length..];
        try
        {
            var payload = JsonConvert.DeserializeObject<SupplierChatAttachmentPayload>(payloadJson);
            if (payload == null)
            {
                return null;
            }

            return new SupplierChatAttachmentDto
            {
                FileName = payload.FileName,
                FileUrl = payload.FileUrl,
                Caption = payload.Caption,
                ContentType = payload.ContentType,
                Length = payload.Length,
                IsImage = !string.IsNullOrWhiteSpace(payload.ContentType) && payload.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
            };
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Unable to parse chat attachment payload.");
            return null;
        }
    }

    private async Task<SupplierChatAttachmentDto?> SaveAttachmentAsync(SupplierChatAttachmentRequest request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return null;
        }

        var uploadsDirectory = EnsureUploadsDirectory();
        var originalName = Path.GetFileName(request.File.FileName);
        var extension = Path.GetExtension(originalName);
        var generatedName = $"{Guid.NewGuid():N}{extension}";
        var physicalPath = Path.Combine(uploadsDirectory, generatedName);

        Directory.CreateDirectory(uploadsDirectory);

        await using (var stream = System.IO.File.Create(physicalPath))
        {
            await request.File.CopyToAsync(stream);
        }

        var relativePath = Path.Combine("uploads", "chat", generatedName).Replace('\\', '/');
        var captionSource = !string.IsNullOrWhiteSpace(request.Caption) ? request.Caption : request.Message;
        var caption = captionSource?.Trim();
        if (string.IsNullOrWhiteSpace(caption))
        {
            caption = originalName;
        }

        var contentType = request.File.ContentType;
        var attachment = new SupplierChatAttachmentDto
        {
            FileName = originalName,
            FileUrl = "/" + relativePath.TrimStart('/'),
            Caption = caption,
            ContentType = contentType,
            Length = request.File.Length,
            IsImage = !string.IsNullOrWhiteSpace(contentType) && contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
        };

        return attachment;
    }

    private string EnsureUploadsDirectory()
    {
        var root = _environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(root))
        {
            root = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        }

        var uploadsDirectory = Path.Combine(root, "uploads", "chat");
        Directory.CreateDirectory(uploadsDirectory);
        return uploadsDirectory;
    }

    private sealed record MessageContext(ProjectMapping Mapping, int EffectiveSupplierId, bool IsSupplierUser);

    private sealed class SupplierChatAttachmentPayload
    {
        public string? FileName { get; set; }

        public string? FileUrl { get; set; }

        public string? Caption { get; set; }

        public string? ContentType { get; set; }

        public long Length { get; set; }
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
