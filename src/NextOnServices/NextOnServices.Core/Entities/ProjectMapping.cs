using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextOnServices.Core.Entities;

[Dapper.Contrib.Extensions.Table("ProjectMapping")]
public class ProjectMapping
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? ProjectId { get; set; }

    public int? CountryId { get; set; }

    public int? SupplierId { get; set; }

    public string? Olink { get; set; }

    public double? Cpi { get; set; }

    public string? Mlink { get; set; }

    public string? Sid { get; set; }

    public string? Code { get; set; }

    public string? Scode { get; set; }

    public string? Nid { get; set; }

    public int? IsUsed { get; set; }

    public string? Status { get; set; }

    public string? ClientIp { get; set; }

    public string? ClientBrowser { get; set; }

    public DateTime? CreationDate { get; set; }

    public int? Respondants { get; set; }

    public int? IsSent { get; set; }

    public string? Notes { get; set; }

    public int? IsChecked { get; set; }

    public string? Completes { get; set; }

    public string? Terminate { get; set; }

    public string? Quotafull { get; set; }

    public string? Screened { get; set; }

    public string? Overquota { get; set; }

    public string? Incomplete { get; set; }

    public string? Security { get; set; }

    public string? Fraud { get; set; }

    public string? Success { get; set; }

    public string? Default { get; set; }

    public string? Failure { get; set; }

    public string? QualityTermination { get; set; }

    public string? OverQuota1 { get; set; }

    public int? IsActive { get; set; }

    public int? Block { get; set; }

    public int? TrackingType { get; set; }

    public int? Prescreening { get; set; }

    public int? Rc { get; set; }

    public int? AddHashing { get; set; }

    public string? ParameterName { get; set; }

    public string? HashingType { get; set; }
}
