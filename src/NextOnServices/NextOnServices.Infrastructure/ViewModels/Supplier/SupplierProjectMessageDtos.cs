using System;

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

    public DateTime? CreatedUtc { get; set; }

    public bool FromSupplier { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadUtc { get; set; }

    public SupplierChatAttachmentDto? Attachment { get; set; }
}

public class SupplierProjectMessageListItemDto : SupplierProjectMessageDto
{
}

public class SupplierChatAttachmentDto
{
    public string? FileName { get; set; }

    public string? FileUrl { get; set; }

    public string? Caption { get; set; }

    public string? ContentType { get; set; }

    public long? Length { get; set; }

    public bool IsImage { get; set; }
}
