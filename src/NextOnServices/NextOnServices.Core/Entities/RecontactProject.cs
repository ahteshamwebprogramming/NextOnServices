using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class RecontactProject
{
    public int Id { get; set; }

    public string? RecontactProjectId { get; set; }

    public int? ProMapId { get; set; }

    public int? Rccnt { get; set; }

    public int? ProjectId { get; set; }

    public int? CountryId { get; set; }

    public int? SupplierId { get; set; }

    public int? Rcq { get; set; }

    public double? Cpi { get; set; }

    public string? Notes { get; set; }

    public int? Recontactid { get; set; }

    public string? Url { get; set; }

    public string? Sid { get; set; }

    public string? Murl { get; set; }

    public string? Recontactname { get; set; }

    public string? RecontactDescription { get; set; }

    public int? Status { get; set; }

    public double? Loi { get; set; }

    public double? Ir { get; set; }
}
