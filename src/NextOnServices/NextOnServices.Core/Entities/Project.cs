using Dapper.Contrib.Extensions;
using NextOnServices.Core.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextOnServices.Core.Entities;

[Dapper.Contrib.Extensions.Table("Projects")]
public class Project
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProjectId { get; set; }

    public string? Pname { get; set; }

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

    //[NavigationProperty]
    //public virtual Client Client { get; set; } = null!;
    //[NavigationProperty]
    //public virtual CountryMaster Country { get; set; } = null!;

}
