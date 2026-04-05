using NextOnServices.Infrastructure.Models.APIProjects;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Supplier;

namespace NextOnServices.VT.Infrastructure.ViewModels.Project;

public class LucidMarketplaceDashboardVM
{
    public int ActiveSettings { get; set; }

    public int ActiveSubscriptions { get; set; }

    public int ActiveOpportunities { get; set; }

    public DateTime? LastSyncTime { get; set; }

    public int TotalLogs { get; set; }

    public int SuccessfulApiCalls { get; set; }

    public int FailedApiCalls { get; set; }

    public int LatestOutcomesReceivedCount { get; set; }

    public DateTime? LatestReconciliationRunTime { get; set; }

    public int LatestReconciliationMismatchCount { get; set; }

    public int LatestReconciliationCompleteCount { get; set; }

    public int LatestReconciliationTerminateCount { get; set; }
}

public class LucidMarketplaceViewModel
{
    public const string OpportunityViewRecent = "recent";

    public const string OpportunityViewAdded = "added";

    public const string OpportunityViewAll = "all";

    public LucidMarketplaceDashboardVM Dashboard { get; set; } = new();

    public LucidMarketplaceSettingDTO Setting { get; set; } = new();

    public List<LucidMarketplaceSubscriptionDTO> Subscriptions { get; set; } = new();

    public List<LucidMarketplaceSyncLogDTO> Logs { get; set; } = new();

    public List<LucidMarketplaceOpportunityDTO> Opportunities { get; set; } = new();

    public string OpportunityView { get; set; } = OpportunityViewRecent;

    public int OpportunityCount { get; set; }

    public LucidMarketplaceOpportunityDTO Opportunity { get; set; } = new();

    public List<LucidMarketplaceOpportunityQualificationDTO> OpportunityQualifications { get; set; } = new();

    public List<LucidMarketplaceOpportunityQuotaDTO> OpportunityQuotas { get; set; } = new();

    public List<LucidMarketplaceSyncLogDTO> OpportunityLogs { get; set; } = new();

    public LucidMarketplaceEntryLinkDTO EntryLink { get; set; } = new();

    public List<LucidMarketplaceRespondentAttemptDTO> RespondentAttempts { get; set; } = new();

    public List<LucidMarketplaceRespondentOutcomeDTO> Outcomes { get; set; } = new();

    public LucidMarketplaceRespondentOutcomeDTO Outcome { get; set; } = new();

    public List<LucidMarketplaceRespondentOutcomeDTO> OutcomeHistory { get; set; } = new();

    public List<LucidMarketplaceReconciliationRunDTO> ReconciliationRuns { get; set; } = new();

    public LucidMarketplaceReconciliationRunDTO ReconciliationRun { get; set; } = new();

    public List<LucidMarketplaceReconciliationItemDTO> ReconciliationItems { get; set; } = new();

    public LucidMarketplaceReconciliationRunRequest ReconciliationFilter { get; set; } = new();

    public List<ClientDTO> Clients { get; set; } = new();

    public List<CountryMasterDTO> Countries { get; set; } = new();

    public List<SupplierDTO> Suppliers { get; set; } = new();

    public string? PlaceholderTitle { get; set; }

    public string? PlaceholderMessage { get; set; }
}
