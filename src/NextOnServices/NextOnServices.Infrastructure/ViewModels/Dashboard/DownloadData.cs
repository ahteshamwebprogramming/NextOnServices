using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.Dashboard
{
    public class DownloadData
    {
        public string? SupplierName { get; set; }
        public string? SupplierCode{ get; set; }
        public string?  SID { get; set; }
        public string? UID { get; set; }
        public string? PID { get; set; }
        public string? Country { get; set; }
        public string? SupplierID { get; set; }
        public string? Status { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Duration { get; set; }
        public string? ClientBrowser { get; set; }
        public string? ClientIP { get; set; }
        public string? Device { get; set; }
        public string? Token { get; set; }
        public string? Notes { get; set; }
        
    }
}
