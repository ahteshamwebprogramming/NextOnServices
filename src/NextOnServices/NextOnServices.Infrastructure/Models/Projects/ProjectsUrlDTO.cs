using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.Models.Projects;

public class ProjectsUrlDTO
{
    public int Id { get; set; }

    public int? Pid { get; set; }
    public string? encProjectId { get; set; }

    public int? Cid { get; set; }

    public string? Url { get; set; }

    public DateTime? CreationDate { get; set; }

    public string? Notes { get; set; }

    public double? Cpi { get; set; }

    public string? Quota { get; set; }

    public int? Status { get; set; }

    public int? Token { get; set; }
    public bool TokenBool { get; set; }
    public string? TokenRaw { get; set; }

    public string? ParameterName { get; set; }

    public string? ParameterValue { get; set; }

    public int? ApplytoSupplier { get; set; }

    public string? OriginalUrl { get; set; }

    public int? ApplyToSupplier1 { get; set; }
}
