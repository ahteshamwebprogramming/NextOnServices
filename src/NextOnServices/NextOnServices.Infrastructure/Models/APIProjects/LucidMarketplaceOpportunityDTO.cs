namespace NextOnServices.Infrastructure.Models.APIProjects;

public class LucidMarketplaceOpportunityDTO
{
    public int LucidMarketplaceOpportunityId { get; set; }

    public int? LucidMarketplaceProjectMapId { get; set; }

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

    public bool IsMapped { get; set; }

    public string? MappingStatus { get; set; }

    public int? InternalProjectId { get; set; }

    public int? InternalProjectUrlId { get; set; }

    public int? InternalProjectMappingId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int QualificationCount { get; set; }

    public int QuotaCount { get; set; }
}
