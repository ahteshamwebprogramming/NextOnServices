using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace NextOnServices.Infrastructure.ViewModels.Supplier;

public class SupplierChatHistoryRequest
{
    public int ProjectMappingId { get; set; }

    public int? ProjectId { get; set; }

    public int? SupplierId { get; set; }

    public string? Pid { get; set; }

    public int PageSize { get; set; } = 50;

    public DateTimeOffset? SinceCursor { get; set; }

    public bool UnreadOnly { get; set; }
}

public class SupplierChatSendRequest
{
    public int ProjectMappingId { get; set; }

    public int? ProjectId { get; set; }

    public int? SupplierId { get; set; }

    public string? Pid { get; set; }

    public string? Message { get; set; }

    public IFormFile? Attachment { get; set; }
}

public class SupplierChatHistoryResponse
{
    public List<SupplierProjectMessageListItemDto> Messages { get; set; } = new();

    public DateTimeOffset? NextCursor { get; set; }

    public bool HasMore { get; set; }
}
