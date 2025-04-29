using Dapper.Contrib.Extensions;
using NextOnServices.Infrastructure.Models.Account;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.Models.Projects;

public class ProjectDTO
{
    [ExplicitKey]
    public int ProjectId { get; set; }

    public string? Pname { get; set; }

    public int? Flag { get; set; }
    public string? Descriptions { get; set; }

    public int? ClientId { get; set; }

    public string? Pid { get; set; }

    public int? Pmanager { get; set; }

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
    public ClientDTO Client { get; set; } = null!;
    public CountryMasterDTO Country { get; set; } = null!;
    
}

public class SaveMapIPRequest
{
    public int Id { get; set; }
    public List<string> Countries { get; set; }
}

public class DeleteMapIPRequest
{
    public int ProjectURLID { get; set; }
    public int CountryID { get; set; }
}
public class MappedCountryResponse
{
    public int Id { get; set; }
    public string Country { get; set; }
}
public class ProUrlIDModel
{
    public int ProUrlID { get; set; }
}