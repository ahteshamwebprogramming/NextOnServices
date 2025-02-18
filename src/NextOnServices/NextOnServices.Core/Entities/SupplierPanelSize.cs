using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextOnServices.Core.Entities;

[Dapper.Contrib.Extensions.Table("SupplierPanelSize")]
public partial class SupplierPanelSize
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? SupplierId { get; set; }

    public int? CountryId { get; set; }

    public int? Psize { get; set; }
}
