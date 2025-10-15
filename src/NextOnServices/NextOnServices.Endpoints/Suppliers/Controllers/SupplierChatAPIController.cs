using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
    private const long MaxAttachmentSizeBytes = 20 * 1024 * 1024; // 20 MB

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
        ".ppt",
        ".pptx",
        ".txt",
        ".csv"
    };

    private static readonly JsonSerializerOptions AttachmentSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SupplierChatAPIController> _logger;
    private readonly IWebHostEnvironment _environment;

    public SupplierChatAPIController(IUnitOfWork unitOfWork, ILogger<SupplierChatAPIController> logger, IWebHostEnvironment environment)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _environment = environment;
    }

    private static string? ValidateAttachments(IFormFileCollection? attachments)
    {
        if (attachments == null || attachments.Count == 0)
        {
            return null;
        }

        foreach (var file in attachments)
        {
            if (file == null)
            {
                continue;
            }

            if (file.Length <= 0)
            {
                return $"Attachment '{file.FileName}' is empty.";
            }

            if (file.Length > MaxAttachmentSizeBytes)
            {
                var maxSizeMb = MaxAttachmentSizeBytes / (1024 * 1024);
                return $"Attachment '{file.FileName}' exceeds the maximum size of {maxSizeMb} MB.";
            }

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedAttachmentExtensions.Contains(extension))
            {
                return $"Attachment '{file.FileName}' has an unsupported file type.";
            }
        }

        return null;
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
        var hasMessage = !string.IsNullOrWhiteSpace(trimmedMessage);
        var hasAttachments = request.Attachments != null && request.Attachments.Count > 0;

        if (!hasMessage && !hasAttachments)
        {
            return BadRequest(new { message = "Either message text or at least one attachment is required." });
        }

        if (hasMessage && trimmedMessage!.Length > 4000)
        {
            trimmedMessage = trimmedMessage[..4000];
        }

        var attachmentValidationError = ValidateAttachments(request.Attachments);
        if (!string.IsNullOrEmpty(attachmentValidationError))
        {
            return BadRequest(new { message = attachmentValidationError });
        }

        var storedAttachments = new List<StoredAttachment>();

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

            if (hasAttachments)
            {
                storedAttachments = await StoreAttachmentsAsync(request, request.ProjectMappingId, utcNow);
            }

            var entity = new SupplierProjectMessage
            {
                ProjectMappingId = request.ProjectMappingId,
                ProjectId = request.ProjectId ?? mapping.ProjectId,
                SupplierId = effectiveSupplierId,
                Message = hasMessage ? trimmedMessage : null,
                CreatedBy = createdBy,
                CreatedByName = senderName,
                CreatedUtc = utcNow,
                FromSupplier = isSupplierUser,
                IsRead = false,
                ReadUtc = null,
                Attachments = storedAttachments.Count > 0 ? SerializeAttachments(storedAttachments.Select(a => a.Descriptor)) : null
            };

            try
            {
                var newId = await _unitOfWork.SupplierProjectMessages.AddAsync(entity);
                entity.Id = newId;
            }
            catch
            {
                CleanupStoredAttachments(storedAttachments);
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
                CreatedUtc = NormalizeUtc(entity.CreatedUtc),
                FromSupplier = entity.FromSupplier,
                IsRead = entity.IsRead,
                ReadUtc = NormalizeUtc(entity.ReadUtc),
                Attachments = storedAttachments.Select(a => a.Descriptor).ToList(),
                AttachmentsSerialized = entity.Attachments
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            CleanupStoredAttachments(storedAttachments);
            _logger.LogError(ex, "Error sending supplier chat message for mapping {ProjectMappingId}", request.ProjectMappingId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while sending the message." });
        }
    }

    private static string SerializeAttachments(IEnumerable<SupplierProjectMessageAttachmentDto> attachments)
    {
        return JsonSerializer.Serialize(attachments, AttachmentSerializerOptions);
    }

    private List<SupplierProjectMessageAttachmentDto> DeserializeAttachments(string? serialized)
    {
        if (string.IsNullOrWhiteSpace(serialized))
        {
            return new List<SupplierProjectMessageAttachmentDto>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<SupplierProjectMessageAttachmentDto>>(serialized, AttachmentSerializerOptions) ?? new List<SupplierProjectMessageAttachmentDto>();
        }
        catch (JsonException jsonException)
        {
            _logger.LogWarning(jsonException, "Failed to deserialize supplier chat attachment payload.");
            return new List<SupplierProjectMessageAttachmentDto>();
        }
    }

    private void PrepareMessagesForResponse(IEnumerable<SupplierProjectMessageDto> messages)
    {
        foreach (var message in messages)
        {
            message.Attachments = DeserializeAttachments(message.AttachmentsSerialized);
            message.AttachmentsSerialized = null;
            message.CreatedUtc = NormalizeUtc(message.CreatedUtc);
            message.ReadUtc = NormalizeUtc(message.ReadUtc);
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
                                Attachments AS AttachmentsSerialized
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
                                Attachments AS AttachmentsSerialized
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

            PrepareMessagesForResponse(rows);
            if (!isSupplierUser)
            {
                var readUtc = DateTime.UtcNow;
                var updatedCount = await _unitOfWork.SupplierProjectMessages.MarkSupplierMessagesAsReadAsync(request.ProjectMappingId, readUtc);
                if (updatedCount > 0)
                {
                    foreach (var message in rows.Where(m => m.FromSupplier && !m.IsRead))
                    {
                        message.IsRead = true;
                        message.ReadUtc = NormalizeUtc(readUtc);
                    }
                }
            }


            var nextCursorDate = rows.LastOrDefault()?.CreatedUtc;
            DateTimeOffset? nextCursor = NormalizeUtc(nextCursorDate);

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

    private async Task<List<StoredAttachment>> StoreAttachmentsAsync(SupplierChatSendRequest request, int projectMappingId, DateTime utcNow)
    {
        var storedAttachments = new List<StoredAttachment>();

        if (request.Attachments == null || request.Attachments.Count == 0)
        {
            return storedAttachments;
        }

        var rootPath = GetAttachmentRootPath();
        var mappingFolder = Path.Combine(rootPath, projectMappingId.ToString());

        Directory.CreateDirectory(mappingFolder);

        var clientIds = request.AttachmentClientIds ?? new List<string>();
        var uploadedUtc = NormalizeUtc(utcNow);

        try
        {
            for (var index = 0; index < request.Attachments.Count; index++)
            {
                var file = request.Attachments[index];
                if (file == null)
                {
                    continue;
                }

                var extension = Path.GetExtension(file.FileName);
                var sanitizedExtension = string.IsNullOrWhiteSpace(extension) ? string.Empty : extension.ToLowerInvariant();
                var attachmentId = Guid.NewGuid().ToString("N");
                var uniqueFileName = $"{attachmentId}{sanitizedExtension}";
                var physicalPath = Path.Combine(mappingFolder, uniqueFileName);

                await using (var targetStream = new FileStream(physicalPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(targetStream);
                }

                var descriptor = new SupplierProjectMessageAttachmentDto
                {
                    Id = attachmentId,
                    ClientId = index < clientIds.Count ? clientIds[index] : null,
                    FileName = Path.GetFileName(file.FileName),
                    ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                    Length = file.Length,
                    StoragePath = Path.Combine(projectMappingId.ToString(), uniqueFileName).Replace('\\', '/'),
                    UploadedUtc = uploadedUtc
                };

                storedAttachments.Add(new StoredAttachment(physicalPath, descriptor));
            }

            return storedAttachments;
        }
        catch
        {
            CleanupStoredAttachments(storedAttachments);
            throw;
        }
    }

    private string GetAttachmentRootPath()
    {
        var root = Path.Combine(_environment.ContentRootPath, "App_Data", "supplier-chat");
        Directory.CreateDirectory(root);
        return root;
    }

    private void CleanupStoredAttachments(IEnumerable<StoredAttachment> storedAttachments)
    {
        var processedDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var attachment in storedAttachments)
        {
            if (string.IsNullOrWhiteSpace(attachment.PhysicalPath))
            {
                continue;
            }

            try
            {
                if (System.IO.File.Exists(attachment.PhysicalPath))
                {
                    System.IO.File.Delete(attachment.PhysicalPath);
                }

                var directory = Path.GetDirectoryName(attachment.PhysicalPath);
                if (!string.IsNullOrEmpty(directory) && processedDirectories.Add(directory) && Directory.Exists(directory))
                {
                    if (!Directory.EnumerateFileSystemEntries(directory).Any())
                    {
                        Directory.Delete(directory, false);
                    }
                }
            }
            catch (Exception cleanupException)
            {
                _logger.LogWarning(cleanupException, "Failed to clean up supplier chat attachment located at {AttachmentPath}.", attachment.PhysicalPath);
            }
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

    private static DateTimeOffset NormalizeUtc(DateTime value)
    {
        return new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc));
    }

    private static DateTimeOffset? NormalizeUtc(DateTime? value)
    {
        return value.HasValue ? NormalizeUtc(value.Value) : null;
    }

    private static DateTimeOffset? NormalizeUtc(DateTimeOffset? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        var utcDateTime = DateTime.SpecifyKind(value.Value.UtcDateTime, DateTimeKind.Utc);
        return new DateTimeOffset(utcDateTime);
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

    private sealed class StoredAttachment
    {
        public StoredAttachment(string physicalPath, SupplierProjectMessageAttachmentDto descriptor)
        {
            PhysicalPath = physicalPath;
            Descriptor = descriptor;
        }

        public string PhysicalPath { get; }

        public SupplierProjectMessageAttachmentDto Descriptor { get; }
    }
}
