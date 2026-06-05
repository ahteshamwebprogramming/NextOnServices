using System.ComponentModel.DataAnnotations.Schema;

namespace NextOnServices.Core.Entities;

[Dapper.Contrib.Extensions.Table("TorfacMarketplaceSetting")]
public class TorfacMarketplaceSetting
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TorfacMarketplaceSettingId { get; set; }

    public string? SurveysUrl { get; set; }

    public string? SecretKey { get; set; }

    public string? DefaultSupplierIds { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}
