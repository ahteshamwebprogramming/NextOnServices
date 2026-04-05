namespace NextOnServices.Infrastructure.Models.APIProjects;

public class LucidMarketplaceRespondentAttemptDTO
{
    public int Id { get; set; }

    public int LucidMarketplaceOpportunityId { get; set; }

    public int? InternalProjectId { get; set; }

    public int? InternalProjectUrlId { get; set; }

    public int? InternalProjectMappingId { get; set; }

    public int LucidSurveyId { get; set; }

    public string? SupplierCode { get; set; }

    public string? RespondentId { get; set; }

    public string? ParentSessionId { get; set; }

    public string? PanelistId { get; set; }

    public string? SessionId { get; set; }

    public string? LaunchUrl { get; set; }

    public string? AttemptType { get; set; }

    public DateTime? AttemptedOn { get; set; }

    public string? ReturnStatus { get; set; }

    public string? ReturnCode { get; set; }

    public string? ReturnRawQuery { get; set; }

    public int? MarketplaceStatus { get; set; }

    public int? ClientStatus { get; set; }

    public string? FinalStatus { get; set; }

    public string? FinalStatusSource { get; set; }

    public DateTime? AsyncLastReceivedOn { get; set; }

    public int? LatestRespondentOutcomeId { get; set; }

    public decimal? RevenueValue { get; set; }

    public string? RevenueCurrencyCode { get; set; }

    public bool IsCompleted { get; set; }

    public bool IsTerminated { get; set; }

    public bool IsOverQuota { get; set; }

    public bool IsQualityTermination { get; set; }

    public bool IsDuplicate { get; set; }

    public bool IsSecurityTermination { get; set; }

    public string? RawJson { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
