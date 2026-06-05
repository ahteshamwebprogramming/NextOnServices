using NextOnServices.Infrastructure.Models.APIProjects;
using NextOnServices.Infrastructure.Models.Supplier;

namespace NextOnServices.VT.Infrastructure.ViewModels.Project;

public class TorfacMarketplaceViewModel
{
    public TorfacMarketplaceSettingDTO Setting { get; set; } = new();
    public List<SupplierDTO> Suppliers { get; set; } = new();
}
