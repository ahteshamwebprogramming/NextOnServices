using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
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

    private static readonly HashSet<string> PreviewableAttachmentExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".bmp",
        ".webp"
    };

    //private static readonly JsonSerializerOptions AttachmentSerializerOptions = new(JsonSerializerDefaults.Web);
    private static readonly JsonSerializerOptions AttachmentSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SupplierChatAPIController> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly IUrlHelperFactory _urlHelperFactory;

    public SupplierChatAPIController(IUnitOfWork unitOfWork, ILogger<SupplierChatAPIController> logger, IWebHostEnvironment environment, IUrlHelperFactory urlHelperFactory)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _environment = environment;
        _urlHelperFactory = urlHelperFactory;
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
                storedAttachments = await StoreAttachmentsAsync(request, utcNow);
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
                Attachments = storedAttachments.Count > 0
                    ? SerializeAttachments(storedAttachments.Select(a => a.Descriptor.Id))
                    : null
            };

            try
            {
                var newId = await _unitOfWork.SupplierProjectMessages.AddAsync(entity);
                entity.Id = newId;

                if (storedAttachments.Count > 0)
                {
                    foreach (var attachment in storedAttachments)
                    {
                        attachment.Entity.MessageId = entity.Id;
                    }

                    try
                    {
                        await _unitOfWork.SupplierProjectMessageAttachments.AddRangeAsync(
                            storedAttachments.Select(a => a.Entity));
                    }
                    catch
                    {
                        await _unitOfWork.SupplierProjectMessages.ExecuteQueryAsync(
                            "DELETE FROM SupplierProjectMessages WHERE Id = @Id",
                            new { Id = entity.Id });
                        throw;
                    }
                }
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
                //Attachments = DeserializeAttachments(entity.Attachments)
            };

            PopulateAttachmentUrls(dto.ProjectMappingId, dto.Attachments);

            CleanupStoredAttachments(storedAttachments);

            return Ok(dto);
        }
        catch (Exception ex)
        {
            CleanupStoredAttachments(storedAttachments);
            _logger.LogError(ex, "Error sending supplier chat message for mapping {ProjectMappingId}", request.ProjectMappingId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while sending the message." });
        }
    }

    private static string SerializeAttachments(IEnumerable<string> attachmentIds)
    {
        return JsonSerializer.Serialize(attachmentIds, AttachmentSerializerOptions);
    }

    private AttachmentMetadata DeserializeAttachmentMetadata(string? serialized)
    {
        if (string.IsNullOrWhiteSpace(serialized))
        {
            return new AttachmentMetadata(new List<string>(), new List<SupplierProjectMessageAttachmentDto>());
        }

        var identifiers = new List<string>();
        var legacyDescriptors = new List<SupplierProjectMessageAttachmentDto>();

        try
        {
            using var document = JsonDocument.Parse(serialized);
            if (document.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in document.RootElement.EnumerateArray())
                {
                    if (element.ValueKind == JsonValueKind.String)
                    {
                        var id = element.GetString();
                        if (!string.IsNullOrWhiteSpace(id))
                        {
                            identifiers.Add(id);
                        }
                    }
                    else if (element.ValueKind == JsonValueKind.Object)
                    {
                        var descriptor = element.Deserialize<SupplierProjectMessageAttachmentDto>(AttachmentSerializerOptions);
                        if (descriptor != null)
                        {
                            if (!string.IsNullOrWhiteSpace(descriptor.Id))
                            {
                                identifiers.Add(descriptor.Id);
                            }

                            legacyDescriptors.Add(descriptor);
                        }
                    }
                }
            }
        }
        catch (JsonException jsonException)
        {
            _logger.LogWarning(jsonException, "Failed to deserialize supplier chat attachment payload.");

            try
            {
                var attachments = JsonSerializer.Deserialize<List<SupplierProjectMessageAttachmentDto>>(serialized, AttachmentSerializerOptions);
                if (attachments != null)
                {
                    foreach (var descriptor in attachments)
                    {
                        if (descriptor == null)
                        {
                            continue;
                        }

                        if (!string.IsNullOrWhiteSpace(descriptor.Id))
                        {
                            identifiers.Add(descriptor.Id);
                        }

                        legacyDescriptors.Add(descriptor);
                    }
                }
            }
            catch (JsonException secondaryException)
            {
                _logger.LogWarning(secondaryException, "Failed to deserialize supplier chat attachment payload via legacy path.");
            }
        }

        return new AttachmentMetadata(identifiers, legacyDescriptors);
    }

    private async Task PrepareMessagesForResponseAsync(ICollection<SupplierProjectMessageDto> messages)
    {
        if (messages == null || messages.Count == 0)
        {
            return;
        }

        var messageList = messages.ToList();
        var metadataByMessage = new Dictionary<int, AttachmentMetadata>();

        foreach (var message in messageList)
        {
            var metadata = DeserializeAttachmentMetadata(message.AttachmentsSerialized);
            metadataByMessage[message.Id] = metadata;

            message.Attachments = new List<SupplierProjectMessageAttachmentDto>();
            message.AttachmentsSerialized = null;
            if (message is SupplierProjectMessageListItemDto listItem)
            {
                listItem.AttachmentsPayload = null;
            }
            message.CreatedUtc = NormalizeUtc(message.CreatedUtc);
            message.ReadUtc = NormalizeUtc(message.ReadUtc);
        }

        var messageIdsWithAttachments = metadataByMessage
            .Where(kvp => kvp.Value.AttachmentIds.Count > 0)
            .Select(kvp => kvp.Key)
            .ToList();

        IReadOnlyDictionary<Guid, SupplierProjectMessageAttachment>? attachmentsLookup = null;

        if (messageIdsWithAttachments.Count > 0)
        {
            var attachments = await _unitOfWork.SupplierProjectMessageAttachments.GetByMessageIdsAsync(messageIdsWithAttachments);
            attachmentsLookup = attachments.ToDictionary(a => a.Id, a => a);
        }

        foreach (var message in messageList)
        {
            var metadata = metadataByMessage[message.Id];

            if (metadata.AttachmentIds.Count > 0 && attachmentsLookup != null)
            {
                foreach (var rawId in metadata.AttachmentIds)
                {
                    if (!Guid.TryParse(rawId, out var attachmentId) || !attachmentsLookup.TryGetValue(attachmentId, out var entity))
                    {
                        var legacy = metadata.LegacyDescriptors.FirstOrDefault(l => string.Equals(l.Id, rawId, StringComparison.OrdinalIgnoreCase));
                        if (legacy != null)
                        {
                            message.Attachments.Add(legacy);
                        }

                        continue;
                    }

                    var descriptor = new SupplierProjectMessageAttachmentDto
                    {
                        Id = entity.Id.ToString("N"),
                        ClientId = entity.ClientId,
                        FileName = entity.FileName,
                        ContentType = string.IsNullOrWhiteSpace(entity.ContentType) ? "application/octet-stream" : entity.ContentType,
                        FileSize = entity.FileSize,
                        UploadedUtc = NormalizeUtc(entity.UploadedUtc)
                    };

                    message.Attachments.Add(descriptor);
                }
            }
            else if (metadata.LegacyDescriptors.Count > 0)
            {
                message.Attachments.AddRange(metadata.LegacyDescriptors);
            }

            PopulateAttachmentUrls(message.ProjectMappingId, message.Attachments);
        }
    }

    private void PopulateAttachmentUrls(int projectMappingId, ICollection<SupplierProjectMessageAttachmentDto>? attachments)
    {
        if (attachments == null || attachments.Count == 0)
        {
            return;
        }

        var urlHelper = GetUrlHelper();

        if (urlHelper == null)
        {
            return;
        }

        foreach (var attachment in attachments)
        {
            if (attachment == null || string.IsNullOrWhiteSpace(attachment.Id))
            {
                continue;
            }

            var url = urlHelper.Action(nameof(DownloadAttachment), values: new
            {
                projectMappingId,
                attachmentId = attachment.Id
            });

            if (!string.IsNullOrWhiteSpace(url))
            {
                attachment.Url = url;
                if (SupportsInlinePreview(attachment))
                {
                    attachment.PreviewUrl = QueryHelpers.AddQueryString(url, "preview", "true");
                }
                else
                {
                    attachment.PreviewUrl = null;
                }
            }
            else
            {
                attachment.PreviewUrl = null;
            }
        }
    }

    private static bool SupportsInlinePreview(SupplierProjectMessageAttachmentDto attachment)
    {
        if (attachment == null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(attachment.ContentType) &&
            attachment.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var candidate = attachment.FileName;

        if (string.IsNullOrWhiteSpace(candidate) && !string.IsNullOrWhiteSpace(attachment.Url))
        {
            candidate = attachment.Url;
        }

        if (string.IsNullOrWhiteSpace(candidate) && !string.IsNullOrWhiteSpace(attachment.StoragePath))
        {
            candidate = attachment.StoragePath;
        }

        if (string.IsNullOrWhiteSpace(candidate))
        {
            return false;
        }

        var sanitized = candidate;
        var queryIndex = sanitized.IndexOf('?');
        if (queryIndex >= 0)
        {
            sanitized = sanitized[..queryIndex];
        }

        var fragmentIndex = sanitized.IndexOf('#');
        if (fragmentIndex >= 0)
        {
            sanitized = sanitized[..fragmentIndex];
        }

        var extension = Path.GetExtension(sanitized);

        if (string.IsNullOrWhiteSpace(extension))
        {
            return false;
        }

        return PreviewableAttachmentExtensions.Contains(extension);
    }

    private void ApplyContentDisposition(FileResult fileResult, string fileName, bool preview)
    {
        if (fileResult == null)
        {
            return;
        }

        if (preview)
        {
            fileResult.FileDownloadName = null;
            SetInlineContentDisposition(fileName);
            return;
        }

        fileResult.FileDownloadName = fileName;
    }

    private void SetInlineContentDisposition(string fileName)
    {
        var contentDisposition = new ContentDispositionHeaderValue("inline");

        if (!string.IsNullOrWhiteSpace(fileName))
        {
            contentDisposition.SetHttpFileName(fileName);
        }

        Response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();
    }

    private IUrlHelper? GetUrlHelper()
    {
        if (Url != null)
        {
            return Url;
        }

        try
        {
            return _urlHelperFactory.GetUrlHelper(ControllerContext);
        }
        catch
        {
            return null;
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
                                Attachments AS AttachmentsPayload,
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
                                Attachments AS AttachmentsPayload,
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

            rows ??= new List<SupplierProjectMessageListItemDto>();

            await PrepareMessagesForResponseAsync(rows.Cast<SupplierProjectMessageDto>().ToList());
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

    private async Task<List<StoredAttachment>> StoreAttachmentsAsync(SupplierChatSendRequest request, DateTime utcNow)
    {
        var storedAttachments = new List<StoredAttachment>();

        if (request.Attachments == null || request.Attachments.Count == 0)
        {
            return storedAttachments;
        }

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

                await using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var attachmentData = memoryStream.ToArray();

                var attachmentId = Guid.NewGuid();
                var contentType = string.IsNullOrWhiteSpace(file.ContentType)
                    ? "application/octet-stream"
                    : file.ContentType;

                var descriptor = new SupplierProjectMessageAttachmentDto
                {
                    Id = attachmentId.ToString("N"),
                    ClientId = index < clientIds.Count ? clientIds[index] : null,
                    FileName = Path.GetFileName(file.FileName),
                    ContentType = contentType,
                    FileSize = file.Length,
                    UploadedUtc = uploadedUtc
                };

                var entity = new SupplierProjectMessageAttachment
                {
                    Id = attachmentId,
                    ClientId = descriptor.ClientId,
                    FileName = descriptor.FileName,
                    ContentType = contentType,
                    FileSize = file.Length,
                    FileData = attachmentData,
                    UploadedUtc = uploadedUtc
                };

                storedAttachments.Add(new StoredAttachment(entity, descriptor));
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
        foreach (var attachment in storedAttachments)
        {
            if (attachment?.Entity?.FileData == null)
            {
                continue;
            }

            if (attachment.Entity.FileData.Length > 0)
            {
                Array.Clear(attachment.Entity.FileData, 0, attachment.Entity.FileData.Length);
            }

            attachment.Entity.FileData = Array.Empty<byte>();
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

    [HttpGet("attachments/{projectMappingId:int}/{attachmentId}")]
    public async Task<IActionResult> DownloadAttachment(int projectMappingId, string attachmentId, [FromQuery] bool preview = false)
    {
        if (projectMappingId <= 0 || string.IsNullOrWhiteSpace(attachmentId))
        {
            return BadRequest(new { message = "A valid attachment identifier is required." });
        }

        SupplierProjectMessageAttachment? attachmentEntity = null;
        SupplierProjectMessageAttachmentDto? legacyAttachment = null;

        try
        {
            var mapping = await _unitOfWork.ProjectMapping.GetEntityData<ProjectMapping>(
                "SELECT TOP 1 * FROM ProjectMapping WHERE Id=@Id",
                new { Id = projectMappingId });

            if (mapping == null)
            {
                return NotFound(new { message = "Project mapping could not be located." });
            }

            if (!TryResolveSupplierContext(null, out var supplierId, out var isSupplierUser, out var failureResult))
            {
                return failureResult!;
            }

            if (isSupplierUser && mapping.SupplierId.HasValue && mapping.SupplierId != supplierId)
            {
                return Forbid();
            }

            if (Guid.TryParse(attachmentId, out var attachmentGuid))
            {
                attachmentEntity = await _unitOfWork.SupplierProjectMessageAttachments.GetByIdForProjectAsync(attachmentGuid, projectMappingId);
            }

            if (attachmentEntity == null)
            {
                const string attachmentsSql = @"SELECT Attachments FROM SupplierProjectMessages
                                            WHERE ProjectMappingId = @ProjectMappingId
                                              AND Attachments IS NOT NULL";

                var payloads = await _unitOfWork.SupplierProjectMessages.GetTableData<string>(
                    attachmentsSql,
                    new { ProjectMappingId = projectMappingId });

                foreach (var payload in payloads)
                {
                    if (string.IsNullOrWhiteSpace(payload))
                    {
                        continue;
                    }

                    var metadata = DeserializeAttachmentMetadata(payload);
                    legacyAttachment = metadata.LegacyDescriptors.FirstOrDefault(a =>
                        a != null && string.Equals(a.Id, attachmentId, StringComparison.OrdinalIgnoreCase));

                    if (legacyAttachment != null)
                    {
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving supplier chat attachment for mapping {ProjectMappingId}", projectMappingId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while locating the attachment." });
        }

        if (attachmentEntity != null)
        {
            var fileName = string.IsNullOrWhiteSpace(attachmentEntity.FileName)
                ? attachmentEntity.Id.ToString("N")
                : attachmentEntity.FileName;

            var contentType = string.IsNullOrWhiteSpace(attachmentEntity.ContentType)
                ? "application/octet-stream"
                : attachmentEntity.ContentType;

            var fileResult = File(attachmentEntity.FileData, contentType);
            ApplyContentDisposition(fileResult, fileName, preview);
            return fileResult;
        }

        if (legacyAttachment == null)
        {
            return NotFound(new { message = "The requested attachment could not be located." });
        }

        var storagePath = legacyAttachment.StoragePath;
        if (string.IsNullOrWhiteSpace(storagePath))
        {
            return NotFound(new { message = "Attachment storage information is missing." });
        }

        var root = GetAttachmentRootPath();
        var segments = storagePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

        var combinedPath = segments.Aggregate(root, Path.Combine);
        var fullPath = Path.GetFullPath(combinedPath);
        var rootPath = Path.GetFullPath(root);

        if (!fullPath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(fullPath))
        {
            return NotFound(new { message = "The requested attachment could not be located." });
        }

        var downloadFileName = string.IsNullOrWhiteSpace(legacyAttachment.FileName)
            ? Path.GetFileName(fullPath)
            : legacyAttachment.FileName;

        var contentTypeProvider = new FileExtensionContentTypeProvider();
        if (!contentTypeProvider.TryGetContentType(downloadFileName, out var downloadContentType) || string.IsNullOrWhiteSpace(downloadContentType))
        {
            downloadContentType = string.IsNullOrWhiteSpace(legacyAttachment.ContentType)
                ? "application/octet-stream"
                : legacyAttachment.ContentType;
        }

        var physicalFile = PhysicalFile(fullPath, downloadContentType!);
        ApplyContentDisposition(physicalFile, downloadFileName, preview);
        return physicalFile;
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

        var utcDateTime = DateTime.SpecifyKind(value.Value.DateTime, DateTimeKind.Utc);
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
        public StoredAttachment(SupplierProjectMessageAttachment entity, SupplierProjectMessageAttachmentDto descriptor)
        {
            Entity = entity;
            Descriptor = descriptor;
        }

        public SupplierProjectMessageAttachment Entity { get; }

        public SupplierProjectMessageAttachmentDto Descriptor { get; }
    }

    private sealed class AttachmentMetadata
    {
        public AttachmentMetadata(List<string> attachmentIds, List<SupplierProjectMessageAttachmentDto> legacyDescriptors)
        {
            AttachmentIds = attachmentIds;
            LegacyDescriptors = legacyDescriptors;
        }

        public List<string> AttachmentIds { get; }

        public List<SupplierProjectMessageAttachmentDto> LegacyDescriptors { get; }
    }
}
