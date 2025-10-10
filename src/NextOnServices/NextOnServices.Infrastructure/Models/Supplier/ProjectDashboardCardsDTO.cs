using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.Models.Supplier
{
    public class ProjectDashboardCardsDTO
    {
        public string? ProjectBid { get; set; }

        public int? ProjectsCompleted { get; set; }

        public string? AverageDuration { get; set; }

        public string? AverageProjectValue { get; set; }

        public int? CountriesCovered { get; set; }
        public string? SupplierSince { get; set; }
        
    }
}
