using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextOnServices.Core.Entities;
[Dapper.Contrib.Extensions.Table("ProjectsUrl")]
public class ProjectsUrl
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? Pid { get; set; }

    public int? Cid { get; set; }

    public string? Url { get; set; }

    public DateTime? CreationDate { get; set; }

    public string? Notes { get; set; }

    public double? Cpi { get; set; }

    public string? Quota { get; set; }

    public int? Status { get; set; }

    public int? Token { get; set; }

    public string? ParameterName { get; set; }

    public string? ParameterValue { get; set; }

    public int? ApplytoSupplier { get; set; }

    public string? OriginalUrl { get; set; }

    public int? ApplyToSupplier1 { get; set; }
}
