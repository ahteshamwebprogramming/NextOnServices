using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NextOnServices.Infrastructure.ViewModels.Supplier;

public class SupplierProjectMessageDto
{
    public int Id { get; set; }

    public int ProjectMappingId { get; set; }

    public int? ProjectId { get; set; }

    public int? SupplierId { get; set; }

    public string? Message { get; set; }

    public string? CreatedByName { get; set; }

    public int? CreatedBy { get; set; }

    public DateTimeOffset? CreatedUtc { get; set; }

    public bool FromSupplier { get; set; }

    public bool IsRead { get; set; }

    public DateTimeOffset? ReadUtc { get; set; }

    [JsonIgnore]
    public string? AttachmentsSerialized { get; set; }

    public List<SupplierProjectMessageAttachmentDto> Attachments { get; set; } = new();
}

public class SupplierProjectMessageListItemDto : SupplierProjectMessageDto
{
    [JsonIgnore]
    public string? AttachmentsPayload { get; set; }
}

public class SupplierProjectMessageAttachmentDto
{
    private long _fileSize;

    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public string? ClientId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string? Url { get; set; }

    public string ContentType { get; set; } = string.Empty;

    public long FileSize
    {
        get => _fileSize;
        set => _fileSize = value;
    }

    public long Length
    {
        get => _fileSize;
        set => _fileSize = value;
    }

    public long? Size
    {
        get => _fileSize;
        set
        {
            if (value.HasValue)
            {
                _fileSize = value.Value;
            }
        }
    }

    public string? StoragePath { get; set; }

    public DateTimeOffset? UploadedUtc { get; set; }
}
