using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.Supplier;

public class SupplierProjectsDTO
{
    public int? ProjectMappingId { get; set; }
    public int? ProjectId { get; set; }
    public int? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public string? ProjectName { get; set; }
    public string? PID { get; set; }
    public string? Country { get; set; }
    public int? CPI { get; set; }
    public int? Respondants { get; set; }
    public int? Total { get; set; }
    public int? Complete { get; set; }
    public int? Terminate { get; set; }
    public int? Overquota { get; set; }
    public int? SecurityTerm { get; set; }
    public int? FraudError { get; set; }
    public int? Incomplete { get; set; }
    public decimal? LOI { get; set; }
    public decimal? IRPercent { get; set; }
    public int? ProjectMappingChecked { get; set; }
    public int? Status { get; set; }
    public string? Notes { get; set; }
    public string? OLink { get; set; }
    public string? MLink { get; set; }

}
