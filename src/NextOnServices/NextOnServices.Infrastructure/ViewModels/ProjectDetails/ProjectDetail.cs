using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.ProjectDetails;

public class ProjectDetail
{
    public string? PName { get; set; }
    public string? Company { get; set; }
    public string? UserName { get; set; }
    public string? Notes { get; set; }
    public string? PID { get; set; }
    public string? Country { get; set; }
    public float? LOI { get; set; }
    public float? CPI { get; set; }
    public int? Status { get; set; }
    public string? StatusName { get; set; }
    public DateTime? edate { get; set; }
    public string? BlockDevice { get; set; }
}
