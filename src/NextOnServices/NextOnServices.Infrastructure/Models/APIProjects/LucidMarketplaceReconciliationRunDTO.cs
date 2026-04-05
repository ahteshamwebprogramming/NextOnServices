namespace NextOnServices.Infrastructure.Models.APIProjects;

public class LucidMarketplaceReconciliationRunDTO
{
    public int Id { get; set; }

    public string? RunType { get; set; }

    public string? SupplierCode { get; set; }

    public int? LucidSurveyId { get; set; }

    public int? InternalProjectId { get; set; }

    public string? RunScopeJson { get; set; }

    public DateTime? StartedOn { get; set; }

    public DateTime? CompletedOn { get; set; }

    public bool Success { get; set; }

    public string? Notes { get; set; }

    public int TotalReviewed { get; set; }

    public int TotalMatched { get; set; }

    public int TotalMismatched { get; set; }

    public int CompleteCount { get; set; }

    public int TerminateCount { get; set; }

    public int OverQuotaCount { get; set; }

    public int QualityTerminationCount { get; set; }

    public int DuplicateCount { get; set; }

    public int SecurityTerminationCount { get; set; }

    public int OpenCount { get; set; }

    public int UnknownCount { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }
}
