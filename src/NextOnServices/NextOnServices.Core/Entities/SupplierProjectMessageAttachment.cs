using System;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;

namespace NextOnServices.Core.Entities;

[Table("SupplierProjectMessageAttachments")]
public class SupplierProjectMessageAttachment
{
    [ExplicitKey]
    public Guid Id { get; set; }

    public int MessageId { get; set; }

    public string? ClientId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public byte[] FileData { get; set; } = Array.Empty<byte>();

    public DateTimeOffset UploadedUtc { get; set; }
}
