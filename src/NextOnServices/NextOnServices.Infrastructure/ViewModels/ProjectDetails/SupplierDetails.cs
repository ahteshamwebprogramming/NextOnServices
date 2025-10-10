using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.ProjectDetails;

public class SupplierDetails
{
    public int? Id { get; set; }
    public int? SupplierId { get; set; }
    public int? CountryId { get; set; }
    public string? Supplier { get; set; }
    public string? Country { get; set; }
    public string? Name { get; set; }
    public float? CPI { get; set; }
    public float? IR { get; set; }
    public float? LOI { get; set; }
    public string? SID { get; set; }
    public bool? TrackingType { get; set; }
    public bool? Block { get; set; }
    public bool? PreScreening { get; set; }
    public string? Quota { get; set; }
    public float? Total { get; set; }
    public float? Complete { get; set; }
    public float? Incomplete { get; set; }
    public float? Screened { get; set; }
    public float? Terminate { get; set; }
    public float? OverQuota { get; set; }
    public float? SecurityTerm { get; set; }
    public float? FraudError { get; set; }
    public int? UnreadCount { get; set; }
    public DateTime? LastMessageUtc { get; set; }
}
