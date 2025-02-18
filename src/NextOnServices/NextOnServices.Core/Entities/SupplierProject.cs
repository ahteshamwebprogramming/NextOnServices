using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class SupplierProject
{
    public int Id { get; set; }

    public string? Pmid { get; set; }

    public string? Sid { get; set; }

    public string? Uid { get; set; }

    public string? Status { get; set; }

    public string? ClientIp { get; set; }

    public string? ClientBrowser { get; set; }

    public int? IsSent { get; set; }

    public string? Device { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? CreationDate { get; set; }

    public string? ActStatus { get; set; }

    public string? Enc { get; set; }
}
