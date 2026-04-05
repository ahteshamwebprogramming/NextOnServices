namespace NextOnServices.Infrastructure.Models.APIProjects;

public class LucidMarketplaceRespondentOutcomeDTO
{
    public int Id { get; set; }

    public string? SupplierCode { get; set; }

    public string? RespondentId { get; set; }

    public string? ParentSessionId { get; set; }

    public string? PanelistId { get; set; }

    public string? SessionId { get; set; }

    public int? MarketplaceStatus { get; set; }

    public int? ClientStatus { get; set; }

    public DateTime? EntryDate { get; set; }

    public DateTime? LastDate { get; set; }

    public int? SurveyId { get; set; }

    public decimal? RevenueValue { get; set; }

    public string? RevenueCurrencyCode { get; set; }

    public string? StudyType { get; set; }

    public int? BuyerId { get; set; }

    public decimal? ProofCostPerInterview { get; set; }

    public string? FinalStatus { get; set; }

    public string? RawJson { get; set; }

    public string? Source { get; set; }

    public DateTime? ReceivedOn { get; set; }

    public bool IsLatest { get; set; }

    public int? RelatedAttemptId { get; set; }

    public int? RelatedOpportunityId { get; set; }

    public int? RelatedInternalProjectId { get; set; }

    public bool AttemptMatched { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
