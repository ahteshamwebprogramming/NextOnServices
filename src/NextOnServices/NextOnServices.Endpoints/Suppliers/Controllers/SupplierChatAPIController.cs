using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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

    private const long MaxAttachmentSizeBytes = 10 * 1024 * 1024; // 10 MB

    private static readonly string[] AllowedAttachmentMimePrefixes = new[]
    {
        "image/"
    };

    private static readonly HashSet<string> AllowedAttachmentMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "text/plain"
    };

    private static readonly HashSet<string> AllowedAttachmentExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".pdf",
        ".doc",
        ".docx",
        ".xls",
        ".xlsx",
        ".txt"
    };

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
    public async Task<IActionResult> Send([FromForm] SupplierChatSendRequest request)
    {
        if (request == null || request.ProjectMappingId <= 0)
        {
            return BadRequest(new { message = "A valid project mapping identifier is required." });
        }

        var trimmedMessage = request.Message?.Trim();
        var hasAttachment = request.Attachment != null && request.Attachment.Length > 0;

        if (!string.IsNullOrWhiteSpace(trimmedMessage) && trimmedMessage.Length > 4000)
        {
            trimmedMessage = trimmedMessage[..4000];
        }

        if (string.IsNullOrWhiteSpace(trimmedMessage) && !hasAttachment)
        {
            return BadRequest(new { message = "A message or attachment is required." });
        }

        var attachmentValidationError = ValidateAttachment(request.Attachment);
        if (attachmentValidationError != null)
        {
            return BadRequest(new { message = attachmentValidationError });
        }

        string? storedFileName = null;
        string? storedRelativePath = null;
        string? originalFileName = null;
        string? normalizedMimeType = null;
        long? attachmentSize = null;

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
            var sanitizedMessage = string.IsNullOrWhiteSpace(trimmedMessage) ? null : trimmedMessage;

            if (hasAttachment && request.Attachment != null)
            {
                normalizedMimeType = NormalizeContentType(request.Attachment);
                originalFileName = Path.GetFileName(request.Attachment.FileName);
                attachmentSize = request.Attachment.Length;

                var saveResult = await SaveAttachmentAsync(request.Attachment, HttpContext.RequestAborted);
                storedFileName = saveResult.StoredFileName;
                storedRelativePath = saveResult.RelativePath;
            }

            var entity = new SupplierProjectMessage
            {
                ProjectMappingId = request.ProjectMappingId,
                ProjectId = request.ProjectId ?? mapping.ProjectId,
                SupplierId = effectiveSupplierId,
                Message = sanitizedMessage,
                CreatedBy = createdBy,
                CreatedByName = senderName,
                CreatedUtc = utcNow,
                FromSupplier = isSupplierUser,
                IsRead = false,
                ReadUtc = null,
                AttachmentFileName = storedFileName,
                AttachmentOriginalFileName = originalFileName,
                AttachmentStoragePath = storedRelativePath,
                AttachmentMimeType = normalizedMimeType,
                AttachmentSizeBytes = attachmentSize
            };

            try
            {
                var newId = await _unitOfWork.SupplierProjectMessages.AddAsync(entity);
                entity.Id = newId;
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(storedRelativePath))
                {
                    TryDeleteAttachment(storedRelativePath);
                }

                throw;
            }

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
                AttachmentFileName = entity.AttachmentFileName,
                AttachmentOriginalFileName = entity.AttachmentOriginalFileName,
                AttachmentStoragePath = entity.AttachmentStoragePath,
                AttachmentMimeType = entity.AttachmentMimeType,
                AttachmentSizeBytes = entity.AttachmentSizeBytes,
                AttachmentUrl = BuildAttachmentUrl(entity.AttachmentStoragePath)
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrWhiteSpace(storedRelativePath))
            {
                TryDeleteAttachment(storedRelativePath);
            }

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
                                ReadUtc,
                                AttachmentFileName,
                                AttachmentOriginalFileName,
                                AttachmentStoragePath,
                                AttachmentMimeType,
                                AttachmentSizeBytes
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
                                ReadUtc,
                                AttachmentFileName,
                                AttachmentOriginalFileName,
                                AttachmentStoragePath,
                                AttachmentMimeType,
                                AttachmentSizeBytes
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
                row.AttachmentUrl = BuildAttachmentUrl(row.AttachmentStoragePath);
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

    private string? ValidateAttachment(IFormFile? attachment)
    {
        if (attachment == null)
        {
            return null;
        }

        if (attachment.Length <= 0)
        {
            return "The selected attachment is empty.";
        }

        if (attachment.Length > MaxAttachmentSizeBytes)
        {
            var maxSizeMb = MaxAttachmentSizeBytes / (1024 * 1024);
            return $"Attachments must be smaller than {maxSizeMb} MB.";
        }

        var contentType = attachment.ContentType ?? string.Empty;
        var extension = Path.GetExtension(attachment.FileName) ?? string.Empty;

        if (!IsAllowedAttachment(contentType, extension))
        {
            return "Attachments must be an image, PDF, text, or Office document.";
        }

        return null;
    }

    private static bool IsAllowedAttachment(string contentType, string extension)
    {
        if (!string.IsNullOrWhiteSpace(contentType))
        {
            if (AllowedAttachmentMimeTypes.Contains(contentType))
            {
                return true;
            }

            foreach (var prefix in AllowedAttachmentMimePrefixes)
            {
                if (contentType.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(extension))
        {
            var normalised = extension.StartsWith('.') ? extension : $".{extension}";
            normalised = normalised.ToLowerInvariant();
            if (AllowedAttachmentExtensions.Contains(normalised))
            {
                return true;
            }
        }

        return false;
    }

    private async Task<(string StoredFileName, string RelativePath)> SaveAttachmentAsync(IFormFile attachment, CancellationToken cancellationToken)
    {
        var attachmentRoot = EnsureAttachmentRoot();
        var extension = Path.GetExtension(attachment.FileName);
        if (string.IsNullOrWhiteSpace(extension) || extension.Length > 10)
        {
            extension = ".bin";
        }

        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(attachmentRoot, storedFileName);

        await using (var stream = System.IO.File.Create(absolutePath))
        {
            await attachment.CopyToAsync(stream, cancellationToken);
        }

        var relativePath = Path.Combine("uploads", "supplier-chat", storedFileName)
            .Replace(Path.DirectorySeparatorChar, '/');

        return (storedFileName, relativePath);
    }

    private string EnsureAttachmentRoot()
    {
        var root = GetWebRootPath();
        var attachmentRoot = Path.Combine(root, "uploads", "supplier-chat");
        Directory.CreateDirectory(attachmentRoot);
        return attachmentRoot;
    }

    private string GetWebRootPath()
    {
        if (!string.IsNullOrWhiteSpace(_environment.WebRootPath))
        {
            return _environment.WebRootPath!;
        }

        var fallback = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        Directory.CreateDirectory(fallback);
        return fallback;
    }

    private void TryDeleteAttachment(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return;
        }

        try
        {
            var normalised = relativePath.Replace('/', Path.DirectorySeparatorChar)
                .TrimStart(Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(GetWebRootPath(), normalised);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
        catch (Exception cleanupEx)
        {
            _logger.LogWarning(cleanupEx, "Failed to clean up attachment {AttachmentPath}", relativePath);
        }
    }

    private string NormalizeContentType(IFormFile attachment)
    {
        if (!string.IsNullOrWhiteSpace(attachment.ContentType))
        {
            return attachment.ContentType;
        }

        var extension = Path.GetExtension(attachment.FileName)?.ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };
    }

    private string? BuildAttachmentUrl(string? storagePath)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
        {
            return null;
        }

        var normalised = storagePath.Replace('\', '/').TrimStart('/');

        if (Url == null)
        {
            return $"/{normalised}";
        }

        return Url.Content($"~/{normalised}");
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
