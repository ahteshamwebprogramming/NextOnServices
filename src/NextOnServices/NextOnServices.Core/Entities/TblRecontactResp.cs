using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class TblRecontactResp
{
    public int Id { get; set; }

    public string? Uid { get; set; }

    public string? RedirectLink { get; set; }

    public string? MaskingUrl { get; set; }

    public string? Pmid { get; set; }

    public string? NewUid { get; set; }

    public string? Status { get; set; }

    public string? StartDate { get; set; }

    public string? EndDate { get; set; }

    public int? Rpi { get; set; }

    public string? Sid { get; set; }

    public string? ClientIp { get; set; }

    public string? ClientBrowser { get; set; }

    public string? Device { get; set; }

    public int? IsSent { get; set; }

    public string? VarVal1 { get; set; }

    public string? VarVal2 { get; set; }

    public string? VarVal3 { get; set; }

    public string? VarVal4 { get; set; }

    public string? VarVal5 { get; set; }

    public string? NewPmid { get; set; }

    public string? VarName1 { get; set; }

    public string? VarName2 { get; set; }

    public string? VarName3 { get; set; }

    public string? VarName4 { get; set; }

    public string? VarName5 { get; set; }

    public int? RecontactFrequency { get; set; }

    public int? ProjectMappingId { get; set; }

    public int? ProjectId { get; set; }

    public int? CountryId { get; set; }

    public int? SupplierId { get; set; }

    public string? ActStatus { get; set; }
}
