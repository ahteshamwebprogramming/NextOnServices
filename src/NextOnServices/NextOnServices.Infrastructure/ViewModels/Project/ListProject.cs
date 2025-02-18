using Dapper.Contrib.Extensions;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.Project;

public class ListProject
{
    [ExplicitKey]
    public int ProjectId { get; set; }
    public string ProjectIdEnc { get; set; }

    public string? Pname { get; set; }

    public int? Flag { get; set; }
    public string? Descriptions { get; set; }

    public int? ClientId { get; set; }
    public string ClientName { get; set; }

    public string? Pid { get; set; }

    public string? Pmanager { get; set; }

    public string? Loi { get; set; }

    public string? Irate { get; set; }

    public double? Cpi { get; set; }

    public int? Status { get; set; }

    public string? SampleSize { get; set; }

    public string? Quota { get; set; }

    public string? Sdate { get; set; }

    public string? Edate { get; set; }

    public int? CountryId { get; set; }

    public int? Ltype { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreationDate { get; set; }

    public int? IsActive { get; set; }

    public string? BlockDevice { get; set; }

    public int? Ipcount { get; set; }

    public string? ProjectIdFromApi { get; set; }

    public string? ProjectFrom { get; set; }

}
