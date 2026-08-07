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

    /// <summary>Comma-separated supplier ids (Torfac-style multi default suppliers).</summary>
    public string? DefaultSupplierIds { get; set; }

    /// <summary>Percent of Lucid CPI stored on ProjectMapping (e.g. 40 = 40%). Null means 100 at add time.</summary>
    public decimal? SupplierCpiPercent { get; set; }

    /// <summary>When true, Add Project sets ProjectMapping hashing and may rewrite LiveLink URL parts.</summary>
    public bool EnableHashing { get; set; }

    /// <summary>ProjectMapping ParameterName: enc, hash, or token.</summary>
    public string? HashingParameter { get; set; }

    /// <summary>ProjectMapping HashingType (must match an active HashingSetting row).</summary>
    public string? HashingType { get; set; }

    /// <summary>Comma-separated LiveLink substrings to find when EnableHashing is on.</summary>
    public string? RespondentIdUrlParts { get; set; }

    /// <summary>Replacement for each find token (e.g. PID=[respondentID]). Defaults to [respondentID] when blank.</summary>
    public string? RespondentIdUrlReplaceWith { get; set; }

    public string? SupplierLinkTypeCode { get; set; }

    public string? TrackingTypeCode { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}

