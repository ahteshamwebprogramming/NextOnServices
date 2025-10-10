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
}

public class SupplierProjectMessageListItemDto : SupplierProjectMessageDto
{
}
