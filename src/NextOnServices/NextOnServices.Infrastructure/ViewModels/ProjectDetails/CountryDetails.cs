using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.ProjectDetails;

public class CountryDetails
{
    public int? Id { get; set; }
    public int? CountryId { get; set; }
    public string? Country { get; set; }
    public int? Total { get; set; }
    public int? Complete { get; set; }
    public int? Incomplete { get; set; }
    public int? Screened { get; set; }
    public int? Terminate { get; set; }
    public int? Overquota { get; set; }
    public int? Sec_Term { get; set; }
    public int? F_Error { get; set; }
    public int? Cancelled { get; set; }
}
