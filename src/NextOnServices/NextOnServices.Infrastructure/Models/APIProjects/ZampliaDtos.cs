using System.ComponentModel.DataAnnotations;

namespace NextOnServices.Infrastructure.Models.APIProjects;

public class ZampliaSettingDTO
{
    public int ZampliaSettingId { get; set; }
    [Required]
    public string? BaseUrl { get; set; } = "https://surveysupplysandbox.zamplia.com";
    [Required]
    public string? ApiKey { get; set; }
    public string? ExitHmacKey { get; set; }
    public bool UseConsultingsBridge { get; set; } = true;
    public bool AutoSyncEnabled { get; set; }
    public int? SyncIntervalMinutes { get; set; }
    public int? DefaultClientId { get; set; }
    public int? DefaultCountryId { get; set; }
    public int? DefaultSupplierId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? CreatedBy { get; set; }
    public int? ModifiedBy { get; set; }
}

public class ZampliaSyncLogDTO
{
    public int ZampliaSyncLogId { get; set; }
    public string? ModuleName { get; set; }
    public string? ActionName { get; set; }
    public string? RequestUrl { get; set; }
    public string? RequestBodySnapshot { get; set; }
    public int? ResponseStatusCode { get; set; }
    public string? ResponseBodySnapshot { get; set; }
    public string? Source { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorText { get; set; }
    public int? RelatedEntityId { get; set; }
    public long? RelatedSurveyId { get; set; }
    public DateTime? StartedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public DateTime? CreatedDate { get; set; }
    public int? CreatedBy { get; set; }
}

public class ZampliaSurveyDTO
{
    public int ZampliaSurveyId { get; set; }
    public long SurveyId { get; set; }
    public string? SurveyName { get; set; }
    public decimal? CPI { get; set; }
    public int? LOI { get; set; }
    public decimal? IR { get; set; }
    public string? LanguageCode { get; set; }
    public int? LanguageId { get; set; }
    public DateTime? SurveyEndDate { get; set; }
    public string? Device { get; set; }
    public string? IndustryId { get; set; }
    public string? StudyTypes { get; set; }
    public bool? IsRecontactSurvey { get; set; }
    public bool? CollectPII { get; set; }
    public decimal? Conversion { get; set; }
    public int? TotalCompleteRequired { get; set; }
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
    public int QualificationCount { get; set; }
    public int QuotaCount { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? CreatedBy { get; set; }
    public int? ModifiedBy { get; set; }
}

public class ZampliaSurveyQualificationDTO
{
    public int Id { get; set; }
    public int ZampliaSurveyId { get; set; }
    public long SurveyId { get; set; }
    public int? QuestionId { get; set; }
    public string? QuestionText { get; set; }
    public string? QuestionType { get; set; }
    public string? LogicalOperator { get; set; }
    public string? AnswerCodesJson { get; set; }
    public string? RawJson { get; set; }
    public int? SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedDate { get; set; }
}

public class ZampliaSurveyQuotaDTO
{
    public int Id { get; set; }
    public int ZampliaSurveyId { get; set; }
    public long SurveyId { get; set; }
    public long? QuotaId { get; set; }
    public string? QuotaName { get; set; }
    public int? NumberOfRespondents { get; set; }
    public decimal? Conversion { get; set; }
    public string? QuestionsJson { get; set; }
    public string? RawJson { get; set; }
    public int? SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedDate { get; set; }
}

public class ZampliaProjectMapDTO
{
    public int Id { get; set; }
    public int ZampliaSurveyId { get; set; }
    public long SurveyId { get; set; }
    public int? InternalProjectId { get; set; }
    public int? InternalProjectUrlId { get; set; }
    public int? InternalProjectMappingId { get; set; }
    public int? AddedBy { get; set; }
    public DateTime? AddedOn { get; set; }
    public bool IsActive { get; set; }
    public string? RawJson { get; set; }
}

public class ZampliaEntryLinkDTO
{
    public int Id { get; set; }
    public int ZampliaSurveyId { get; set; }
    public long SurveyId { get; set; }
    public string? TransactionId { get; set; }
    public string? VendorLink { get; set; }
    public string? InternalLaunchUrl { get; set; }
    public bool HashApplied { get; set; }
    public string? RawJson { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? CreatedBy { get; set; }
    public int? ModifiedBy { get; set; }
}

public class ZampliaRespondentAttemptDTO
{
    public int Id { get; set; }
    public int ZampliaSurveyId { get; set; }
    public long SurveyId { get; set; }
    public int? InternalProjectId { get; set; }
    public int? InternalProjectUrlId { get; set; }
    public int? InternalProjectMappingId { get; set; }
    public string? RespondentId { get; set; }
    public string? TransactionId { get; set; }
    public string? SessionId { get; set; }
    public string? IpAddress { get; set; }
    public string? LaunchUrl { get; set; }
    public string? VendorLaunchUrl { get; set; }
    public string? ReturnUrl { get; set; }
    public string? ReturnRawQuery { get; set; }
    public string? ReturnCode { get; set; }
    public string? ReturnStatus { get; set; }
    public string? FinalStatus { get; set; }
    public string? FinalStatusSource { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsTerminated { get; set; }
    public bool IsOverQuota { get; set; }
    public bool IsQualityTermination { get; set; }
    public bool IsSecurityTermination { get; set; }
    public bool IsDuplicate { get; set; }
    public string? HmacReceived { get; set; }
    public string? HmacCalculated { get; set; }
    public bool? HmacValid { get; set; }
    public DateTime? AttemptedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public string? RawJson { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? CreatedBy { get; set; }
    public int? ModifiedBy { get; set; }
}

public class ZampliaReconciliationRunDTO
{
    public int Id { get; set; }
    public string? RunType { get; set; }
    public long? SurveyId { get; set; }
    public int? InternalProjectId { get; set; }
    public string? TransactionId { get; set; }
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
    public int SecurityTerminationCount { get; set; }
    public int OpenCount { get; set; }
    public int UnknownCount { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
}

public class ZampliaReconciliationItemDTO
{
    public int Id { get; set; }
    public int ReconciliationRunId { get; set; }
    public int? ZampliaRespondentAttemptId { get; set; }
    public long? SurveyId { get; set; }
    public int? InternalProjectId { get; set; }
    public string? TransactionId { get; set; }
    public string? RespondentId { get; set; }
    public string? SessionId { get; set; }
    public string? LocalStatus { get; set; }
    public string? VendorStatus { get; set; }
    public string? FinalStatus { get; set; }
    public string? FinalStatusSource { get; set; }
    public bool IsMismatch { get; set; }
    public string? MismatchType { get; set; }
    public string? Notes { get; set; }
    public string? RawSnapshotJson { get; set; }
    public DateTime? CreatedDate { get; set; }
}

public class ZampliaLaunchContextDTO
{
    public int ZampliaSurveyId { get; set; }
    public long SurveyId { get; set; }
    public string? SurveyName { get; set; }
    public int? LanguageId { get; set; }
    public string? SurveyRawJson { get; set; }
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
    public string? ExitHmacKey { get; set; }
    public bool UseConsultingsBridge { get; set; }
    public string? ExistingProjectUrl { get; set; }
    public string? ExistingMaskingUrl { get; set; }
}
