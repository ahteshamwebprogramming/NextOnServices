namespace NextOnServices.Infrastructure.Models.APIProjects;

public class LucidMarketplaceLaunchContextDTO
{
    public int LucidMarketplaceOpportunityId { get; set; }

    public int LucidSurveyId { get; set; }

    public string? SurveyNumber { get; set; }

    public string? SupplierCode { get; set; }

    public string? SurveyName { get; set; }

    public string? OpportunityRawJson { get; set; }

    public int? InternalProjectId { get; set; }

    public int? InternalProjectUrlId { get; set; }

    public int? InternalProjectMappingId { get; set; }

    public string? ProjectMappingSid { get; set; }

    public string? ProjectMappingCode { get; set; }

    public string? ProjectFrom { get; set; }

    public int? CountryId { get; set; }

    public int? SupplierId { get; set; }

    public string? BaseUrl { get; set; }

    public string? ApiKey { get; set; }

    public bool UseConsultingsBridge { get; set; }

    public string? SupplierLinkTypeCode { get; set; }

    public string? TrackingTypeCode { get; set; }

    public string? ExistingProjectUrl { get; set; }

    public string? ExistingMaskingUrl { get; set; }
}
