namespace NextOnServices.Infrastructure.Models.APIProjects;

public class LucidMarketplaceConnectivityRequest
{
    public string? BaseUrl { get; set; }

    public string? ApiKey { get; set; }

    public string? SupplierCode { get; set; }

    public bool UseConsultingsBridge { get; set; }
}

public class LucidMarketplaceSubscriptionActionRequest
{
    public string? SubscriptionType { get; set; }

    public string? SupplierCode { get; set; }

    public string? CallbackUrl { get; set; }

    public bool IncludeQuotas { get; set; }

    public List<string> CountryLanguageCodes { get; set; } = new();

    public int? PayloadMaxSizeMb { get; set; }

    public int? PayloadMaxSurveyCount { get; set; }

    public int? SendIntervalSeconds { get; set; }

    public string? OutcomeFiltersJson { get; set; }
}

public class LucidMarketplaceDeactivateSubscriptionRequest
{
    public int LucidMarketplaceSubscriptionId { get; set; }
}

public class LucidMarketplaceLogDetailsRequest
{
    public int LucidMarketplaceSyncLogId { get; set; }
}

public class LucidMarketplaceOpportunityStateRequest
{
    public int LucidMarketplaceOpportunityId { get; set; }

    public bool IsActive { get; set; }

    public string? LocalState { get; set; }
}

public class LucidMarketplaceNumericFilter
{
    public string? Op { get; set; }

    public decimal? Value { get; set; }

    public decimal? Min { get; set; }

    public decimal? Max { get; set; }
}

public class LucidMarketplaceOpportunitySearchRequest
{
    public string? View { get; set; }

    public int Draw { get; set; }

    public int Start { get; set; }

    public int Length { get; set; } = 25;

    public string? Search { get; set; }

    public string? OrderBy { get; set; }

    public string? Dir { get; set; }

    public string? CountryLanguageCode { get; set; }

    public string? StudyType { get; set; }

    public LucidMarketplaceNumericFilter? Cpi { get; set; }

    public LucidMarketplaceNumericFilter? Ir { get; set; }

    public LucidMarketplaceNumericFilter? Loi { get; set; }

    public LucidMarketplaceNumericFilter? TotalRemaining { get; set; }
}

public class LucidMarketplaceOpportunityDataTableRequest : NextOnServices.Infrastructure.ViewModels.DataTablesRequest
{
    public string? View { get; set; }

    public string? CountryLanguageCode { get; set; }

    public string? StudyType { get; set; }

    public LucidMarketplaceNumericFilter? Cpi { get; set; }

    public LucidMarketplaceNumericFilter? Ir { get; set; }

    public LucidMarketplaceNumericFilter? Loi { get; set; }

    public LucidMarketplaceNumericFilter? TotalRemaining { get; set; }
}

public class LucidMarketplaceOpportunitySearchResult
{
    public int Draw { get; set; }

    public int RecordsTotal { get; set; }

    public int RecordsFiltered { get; set; }

    public List<LucidMarketplaceOpportunityDTO> Data { get; set; } = new();
}

public class LucidMarketplaceAddProjectRequest
{
    public int LucidMarketplaceOpportunityId { get; set; }
}

public class LucidMarketplaceRemoveProjectRequest
{
    public int LucidMarketplaceOpportunityId { get; set; }
}

public class LucidMarketplaceRemoveAddedProjectRequest
{
    public int LucidMarketplaceOpportunityId { get; set; }

    /// <summary>
    /// "lucid" = hard-delete Lucid Marketplace tables only; keep VT Projects/ProjectsUrl/ProjectMapping unchanged.
    /// "entire" = hard-delete Lucid Marketplace tables and soft-deactivate VT Projects/ProjectsUrl/ProjectMapping.
    /// </summary>
    public string Mode { get; set; } = "lucid";
}

public class LucidMarketplaceRemoveNonLiveAddedProjectsRequest
{
    /// <summary>
    /// "lucid" = hard-delete Lucid Marketplace tables only; keep VT Projects/ProjectsUrl/ProjectMapping unchanged.
    /// "entire" = hard-delete Lucid Marketplace tables and soft-deactivate VT Projects/ProjectsUrl/ProjectMapping.
    /// </summary>
    public string Mode { get; set; } = "lucid";
}

public class LucidMarketplaceGenerateEntryLinkRequest
{
    public int? LucidMarketplaceOpportunityId { get; set; }

    public int? InternalProjectId { get; set; }

    public int? InternalProjectUrlId { get; set; }

    public int? InternalProjectMappingId { get; set; }

    public string AttemptType { get; set; } = "Live";

    public bool ForceRefresh { get; set; }
}

public class LucidMarketplaceReconciliationRunRequest
{
    public string? RunType { get; set; }

    public string? SupplierCode { get; set; }

    public int? LucidSurveyId { get; set; }

    public int? InternalProjectId { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }
}

public class LucidMarketplaceReconciliationRunResult
{
    public int ReconciliationRunId { get; set; }

    public int TotalReviewed { get; set; }

    public int TotalMismatched { get; set; }

    public bool Success { get; set; }

    public string? Message { get; set; }
}

public class LucidMarketplaceOpportunityProcessResult
{
    public int ProcessedCount { get; set; }

    public int SkippedCount { get; set; }

    public List<int> OpportunityIds { get; set; } = new();

    public List<int> SurveyIds { get; set; } = new();

    public List<string> Errors { get; set; } = new();
}

public class LucidMarketplaceOutcomeProcessResult
{
    public int ProcessedCount { get; set; }

    public int SkippedCount { get; set; }

    public int MatchedAttemptCount { get; set; }

    public int UpdatedAttemptCount { get; set; }

    public List<int> OutcomeIds { get; set; } = new();

    public List<int> AttemptIds { get; set; } = new();

    public List<int> SurveyIds { get; set; } = new();

    public int LegacyCompletionAppliedCount { get; set; }

    public int LegacyCompletionSkippedCount { get; set; }

    public List<string> Errors { get; set; } = new();

    public List<LucidMarketplaceLegacyCompletionCandidate> LegacyCompletionCandidates { get; set; } = new();
}

public class LucidMarketplaceLegacyCompletionCandidate
{
    public int OutcomeId { get; set; }

    public int? AttemptId { get; set; }

    public int? RelatedOpportunityId { get; set; }

    public int? SurveyId { get; set; }

    public string? InternalRespondentUid { get; set; }

    public string? FinalStatus { get; set; }

    public string? LegacyProjectStatus { get; set; }
}

public class LucidMarketplaceProxyRequest
{
    public LucidMarketplaceSettingDTO? Setting { get; set; }

    public LucidMarketplaceSubscriptionDTO? Subscription { get; set; }

    public LucidMarketplaceEntryLinkProxyRequest? EntryLink { get; set; }

    public int? SurveyNumber { get; set; }

    public int? CountryLanguageId { get; set; }

    public int? QuestionId { get; set; }
}

public class LucidMarketplaceProxyResponse
{
    public bool Result { get; set; }

    public bool IsStub { get; set; }

    public int? StatusCode { get; set; }

    public string? Message { get; set; }

    public string? RemoteSubscriptionId { get; set; }

    public string? RequestUrl { get; set; }

    public string? ResponseBody { get; set; }
}

public class LucidMarketplaceEntryLinkProxyRequest
{
    public int SurveyNumber { get; set; }

    public string? SupplierCode { get; set; }

    public string? SupplierLinkTypeCode { get; set; }

    public string? TrackingTypeCode { get; set; }

    public string? DefaultLink { get; set; }

    public string? SuccessLink { get; set; }

    public string? FailureLink { get; set; }

    public string? OverQuotaLink { get; set; }

    public string? QualityTerminationLink { get; set; }
}
