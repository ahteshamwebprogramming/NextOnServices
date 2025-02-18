using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.Models.Supplier;

public class SupplierDTO
{
    public int Id { get; set; }
    public string? encId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Number { get; set; }

    public string? Email { get; set; }

    public string? Psize { get; set; }

    public string? Country { get; set; }

    public string? Notes { get; set; }

    public string? Completes { get; set; }

    public string? Terminate { get; set; }

    public string? Quotafull { get; set; }

    public string? Screened { get; set; }

    public string? Overquota { get; set; }

    public string? Incomplete { get; set; }

    public string? Security { get; set; }

    public string? Fraud { get; set; }

    public DateTime? CreationDate { get; set; }

    public int? Sstatus { get; set; }

    public string? Success { get; set; }

    public string? Default { get; set; }

    public string? Failure { get; set; }

    public string? QualityTermination { get; set; }

    public string? OverQuota1 { get; set; }

    public int? IsActive { get; set; }

    public string? HashingUrl { get; set; }

    public string? CompleteStatus { get; set; }

    public string? OtherThenComplete { get; set; }

    public int? AllowHashing { get; set; }

    public string? AuthorizationKey { get; set; }
    public string? SupplierCode { get; set; }
}
