using System.ComponentModel.DataAnnotations.Schema;

namespace NextOnServices.Core.Entities;

[Dapper.Contrib.Extensions.Table("LucidMarketplaceSetting")]
public class LucidMarketplaceSetting
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LucidMarketplaceSettingId { get; set; }

    public string? BaseUrl { get; set; }

    public string? ApiKey { get; set; }

    public string? EntryLinkSecretKey { get; set; }

    public string? SupplierCode { get; set; }

    public string? OpportunitiesCallbackUrl { get; set; }

    public string? OutcomesCallbackUrl { get; set; }

    public bool UseConsultingsBridge { get; set; }

    public bool AutoSyncEnabled { get; set; }

    public int? SyncIntervalMinutes { get; set; }

    public int? DefaultClientId { get; set; }

    public int? DefaultCountryId { get; set; }

    public int? DefaultSupplierId { get; set; }

    public string? SupplierLinkTypeCode { get; set; }

    public string? TrackingTypeCode { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}

