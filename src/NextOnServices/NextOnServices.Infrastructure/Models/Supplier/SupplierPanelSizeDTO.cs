using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.Models.Supplier;

public class SupplierPanelSizeDTO
{
    public int Id { get; set; }

    public int? SupplierId { get; set; }
    public string? encSupplierId { get; set; }

    public int? CountryId { get; set; }

    public int? Psize { get; set; }
}
