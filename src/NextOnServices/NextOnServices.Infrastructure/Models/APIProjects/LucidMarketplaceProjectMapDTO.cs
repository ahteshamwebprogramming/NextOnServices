namespace NextOnServices.Infrastructure.Models.APIProjects;

public class LucidMarketplaceProjectMapDTO
{
    public int Id { get; set; }

    public int LucidMarketplaceOpportunityId { get; set; }

    public int LucidSurveyId { get; set; }

    public int? InternalProjectId { get; set; }

    public int? InternalProjectUrlId { get; set; }

    public int? InternalProjectMappingId { get; set; }

    public string? SupplierCode { get; set; }

    public int? AddedBy { get; set; }

    public DateTime? AddedOn { get; set; }

    public bool IsActive { get; set; }

    public string? RawJson { get; set; }
}
