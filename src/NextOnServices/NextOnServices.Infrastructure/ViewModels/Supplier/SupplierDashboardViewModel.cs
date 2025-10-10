using NextOnServices.Infrastructure.Models.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.Supplier;

public class SupplierDashboardViewModel
{
    public SupplierDTO? Supplier { get; set; }
    public List<SupplierProjectsDTO>? SupplierProjects { get; set; }

    public ProjectDashboardCardsDTO? ProjectDashboardCards { get; set; }
}
