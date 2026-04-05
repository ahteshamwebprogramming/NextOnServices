using System.ComponentModel.DataAnnotations;

namespace NextOnServices.Infrastructure.Models.APIProjects;

public class LucidMarketplaceSettingDTO
{
    public int LucidMarketplaceSettingId { get; set; }

    [Required]
    public string? BaseUrl { get; set; }

    [Required]
    public string? ApiKey { get; set; }

    [Required]
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

    public bool IsActive { get; set; } = true;

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}
