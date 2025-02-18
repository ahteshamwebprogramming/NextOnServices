using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.ProjectDetails;

public class SurveySpecs
{
    public int? Id { get; set; }
    public int? PUID { get; set; }
    public string? Country { get; set; }
    public int? SurveyQuota { get; set; }
    public float? LOI { get; set; }
    public float? CPI { get; set; }
    public float? IRate { get; set; }
    public int? CountryId { get; set; }
    public int? Status { get; set; }
    public string? StatusName { get; set; }
    public int? Total { get; set; }
    public int? Complete { get; set; }
    public float? CompletePercent { get; set; }
    public float? IRPercent { get; set; }
    public float? ActLOI { get; set; }
    public float? LOIPercentage { get; set; }
    public string? Notes { get; set; }
}
