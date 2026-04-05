namespace NextOnServices.Infrastructure.Models.APIProjects;

public class ZampliaConnectivityRequest
{
    public string? BaseUrl { get; set; }
    public string? ApiKey { get; set; }
    public bool UseConsultingsBridge { get; set; }
}

public class ZampliaSaveSurveyRequest
{
    public ZampliaSurveyDTO Survey { get; set; } = new();
    public List<ZampliaSurveyQualificationDTO> Qualifications { get; set; } = new();
    public List<ZampliaSurveyQuotaDTO> Quotas { get; set; } = new();
}

public class ZampliaAddProjectRequest
{
    public int ZampliaSurveyId { get; set; }
}

public class ZampliaGenerateEntryLinkRequest
{
    public int? ZampliaSurveyId { get; set; }
    public int? InternalProjectId { get; set; }
    public int? InternalProjectUrlId { get; set; }
    public int? InternalProjectMappingId { get; set; }
    public bool ForceRefresh { get; set; }
}

public class ZampliaReconciliationRunRequest
{
    public string? RunType { get; set; }
    public long? SurveyId { get; set; }
    public int? InternalProjectId { get; set; }
    public string? TransactionId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class ZampliaReconciliationRunResult
{
    public int ReconciliationRunId { get; set; }
    public int TotalReviewed { get; set; }
    public int TotalMismatched { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class ZampliaProcessResult
{
    public int ProcessedCount { get; set; }
    public int SkippedCount { get; set; }
    public int InactivatedCount { get; set; }
    public List<int> SurveyEntityIds { get; set; } = new();
    public List<long> SurveyIds { get; set; } = new();
    public List<long> InactivatedSurveyIds { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}

public class ZampliaInactiveSurveyCleanupResult
{
    public int RemovedCount { get; set; }
    public List<long> RemovedSurveyIds { get; set; } = new();
}

public class ZampliaProxyRequest
{
    public ZampliaSettingDTO? Setting { get; set; }
    public long? SurveyId { get; set; }
    public int? LanguageId { get; set; }
    public string? IpAddress { get; set; }
    public string? TransactionId { get; set; }
    public Dictionary<string, string?> QueryParameters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public class ZampliaProxyResponse
{
    public bool Result { get; set; }
    public bool IsStub { get; set; }
    public int? StatusCode { get; set; }
    public string? Message { get; set; }
    public string? RequestUrl { get; set; }
    public string? ResponseBody { get; set; }
}

public class ZampliaHmacValidationResult
{
    public string? NormalizedUrl { get; set; }
    public string? CalculatedHash { get; set; }
    public string? ReceivedHash { get; set; }
    public bool IsValid { get; set; }
}

public class ZampliaSessionEventsRequest
{
    public long SurveyId { get; set; }
    public string? TransactionId { get; set; }
}
