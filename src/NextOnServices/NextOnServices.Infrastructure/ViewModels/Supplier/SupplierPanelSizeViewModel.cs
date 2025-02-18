using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.VT.Infrastructure.ViewModels.Supplier;

public class SupplierPanelSizeViewModel
{
    public SupplierPanelSizeDTO? SupplierPanelSize { get; set; }
    public List<SupplierPanelSizeDTO>? SupplierPanelSizeList { get; set; }
    public List<SupplierPanelSizeWithChild>? SupplierPanelSizeWithChildren { get; set; }
    public SupplierPanelSizeWithChild? SupplierPanelSizeWithChild { get; set; }
    public List<CountryMasterDTO>? Countries { get; set; }
}
