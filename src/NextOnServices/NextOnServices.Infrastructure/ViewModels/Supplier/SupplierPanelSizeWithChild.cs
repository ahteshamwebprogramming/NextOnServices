using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.VT.Infrastructure.ViewModels.Supplier;

public class SupplierPanelSizeWithChild
{
    public int Id { get; set; }
    public string? encId { get; set; }

    public int? SupplierId { get; set; }
    public string? Supplier { get; set; }

    public int? CountryId { get; set; }
    public string? Country { get; set; }

    public int? Psize { get; set; }
}
