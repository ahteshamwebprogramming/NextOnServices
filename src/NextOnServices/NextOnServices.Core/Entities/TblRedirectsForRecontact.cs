using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class TblRedirectsForRecontact
{
    public int Id { get; set; }

    public int? Rpid { get; set; }

    public string? Complete { get; set; }

    public string? Terminate { get; set; }

    public string? Overquota { get; set; }

    public string? STerm { get; set; }

    public string? FError { get; set; }

    public string? Variable1 { get; set; }

    public string? Variable2 { get; set; }

    public string? Notes { get; set; }
}
