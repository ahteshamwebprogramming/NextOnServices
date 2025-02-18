using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.ProjectDetails;

public class SurveyLinks
{
    public int? Id { get; set; }
    public string? Country { get; set; }
    public string? Suppliers { get; set; }
    public string? OLink { get; set; }
    public string? MLink { get; set; }
    public string? StatusLink { get; set; }
    public string? RedirectLink { get; set; }
}
