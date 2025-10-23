using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.VT.Infrastructure.ViewModels.Supplier;

public class SupplierViewModel
{
    public List<SupplierDTO>? SupplierList { get; set; }
    public SupplierDTO? Supplier { get; set; }
    public SupplierWithChild? SupplierWithChild { get; set; }
    public List<SupplierWithChild>? SupplierWithChildList { get; set; }
    public List<SupplierPanelSizeWithChild>? SupplierPanelSizes { get; set; }
    public List<SupplierDeliverySummary>? SupplierDeliveryDetails { get; set; }
    public List<CountryMasterDTO>? Countries { get; set; }

}
