using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextOnServices.Core.Entities;
[Dapper.Contrib.Extensions.Table("SupplierProjects")]
public partial class SupplierProjects
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    public string? PMID { get; set; }

    public string? SID { get; set; }

    public string? UID { get; set; }

    public string? Status { get; set; }

    public string? ClientIP { get; set; }

    public string? ClientBrowser { get; set; }

    public int? IsSent { get; set; }

    public string? Device { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? CreationDate { get; set; }

    public string? ActStatus { get; set; }

    public string? ENC { get; set; }
}
