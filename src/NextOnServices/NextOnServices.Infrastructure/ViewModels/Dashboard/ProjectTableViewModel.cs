using NextOnServices.Infrastructure.Models.Account;
using NextOnServices.Infrastructure.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.Dashboard;

public class ProjectTableViewModel
{
    public int ID { get; set; }
    public int ProjectId { get; set; }

    public string? PNO { get; set; }
    public string? PNAME { get; set; }

    public string? Descriptions { get; set; }

    public string? CLIENT { get; set; }

    public string? PM { get; set; }
    public string? COUNTRY { get; set; }

    public int? LOI { get; set; }
    public double? CPI { get; set; }
    public double? IRate { get; set; }
    public string? Date { get; set; }
    public int? STATUS { get; set; }
    public string? startdate { get; set; }
    public int? ClientID { get; set; }
    public int? SampleSize { get; set; }
    public int? TOTAL { get; set; }
    public int? CO { get; set; }
    public int? TR { get; set; }
    public int? OQ { get; set; }
    public int? ST { get; set; }
    public int? FE { get; set; }
    public int? IC { get; set; }
    public int? ActIR { get; set; }
    public int? ActLOI { get; set; }
    public string? Flag { get; set; }
}
public class DashboardProjectCountSummaryViewModel
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int InactiveProjects { get; set; }
    public int ArchivedProjects { get; set; }
}