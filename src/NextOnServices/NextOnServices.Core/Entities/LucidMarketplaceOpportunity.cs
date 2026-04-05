using System.ComponentModel.DataAnnotations.Schema;

namespace NextOnServices.Core.Entities;

[Dapper.Contrib.Extensions.Table("LucidMarketplaceOpportunity")]
public class LucidMarketplaceOpportunity
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LucidMarketplaceOpportunityId { get; set; }

    public string? SupplierCode { get; set; }

    public int SurveyId { get; set; }

    public string? SurveyNumber { get; set; }

    public string? SurveyName { get; set; }

    public string? AccountName { get; set; }

    public int? BuyerId { get; set; }

    public string? TargetGroupId { get; set; }

    public string? CountryLanguageCode { get; set; }

    public string? StudyType { get; set; }

    public string? Industry { get; set; }

    public decimal? RevenuePerInterview { get; set; }

    public string? RevenueCurrencyCode { get; set; }

    public decimal? BidIncidence { get; set; }

    public decimal? BidLengthOfInterview { get; set; }

    public int? TotalRemaining { get; set; }

    public bool? IsLive { get; set; }

    public int? RecontactCount { get; set; }

    public string? SurveyGroupIdsJson { get; set; }

    public string? MessageReason { get; set; }

    public DateTime? LastVendorUpdatedOn { get; set; }

    public DateTime? LastSyncedOn { get; set; }

    public string? LocalState { get; set; }

    public string? RawJson { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}
