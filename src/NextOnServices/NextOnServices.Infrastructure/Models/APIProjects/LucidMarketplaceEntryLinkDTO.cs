namespace NextOnServices.Infrastructure.Models.APIProjects;

public class LucidMarketplaceEntryLinkDTO
{
    public int Id { get; set; }

    public int LucidMarketplaceOpportunityId { get; set; }

    public int LucidSurveyId { get; set; }

    public string? SupplierCode { get; set; }

    public int? InternalProjectId { get; set; }

    public int? InternalProjectUrlId { get; set; }

    public int? InternalProjectMappingId { get; set; }

    public string? SupplierLinkTypeCode { get; set; }

    public string? TrackingTypeCode { get; set; }

    public string? DefaultLink { get; set; }

    public string? SuccessLink { get; set; }

    public string? FailureLink { get; set; }

    public string? OverQuotaLink { get; set; }

    public string? QualityTerminationLink { get; set; }

    public string? LiveLink { get; set; }

    public string? TestLink { get; set; }

    public string? SupplierLinkSid { get; set; }

    public decimal? RPIValue { get; set; }

    public string? RPICurrencyCode { get; set; }

    public string? RawJson { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}
