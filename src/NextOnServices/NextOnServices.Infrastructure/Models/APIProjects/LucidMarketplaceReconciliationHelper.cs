namespace NextOnServices.Infrastructure.Models.APIProjects;

public static class LucidMarketplaceMismatchTypes
{
    public const string None = "None";
    public const string RedirectCompleteOutcomeTerminate = "RedirectComplete_OutcomeTerminate";
    public const string RedirectTerminateOutcomeComplete = "RedirectTerminate_OutcomeComplete";
    public const string RedirectOverQuotaOutcomeComplete = "RedirectOverQuota_OutcomeComplete";
    public const string RedirectMissingOutcomeExists = "RedirectMissing_OutcomeExists";
    public const string RedirectExistsOutcomeMissing = "RedirectExists_OutcomeMissing";
    public const string OutcomeChangedOverTime = "OutcomeChangedOverTime";
    public const string UnknownDifference = "UnknownDifference";
}

public sealed class LucidMarketplaceResolvedStatusInfo
{
    public string FinalStatus { get; init; } = "Unknown";

    public bool IsCompleted { get; init; }

    public bool IsTerminated { get; init; }

    public bool IsOverQuota { get; init; }

    public bool IsQualityTermination { get; init; }

    public bool IsDuplicate { get; init; }

    public bool IsSecurityTermination { get; init; }
}

public sealed class LucidMarketplaceReconciliationStatusResolution
{
    public string FinalStatus { get; init; } = "Unknown";

    public string FinalStatusSource { get; init; } = "None";

    public string? RedirectFinalStatus { get; init; }

    public string? OutcomeFinalStatus { get; init; }

    public bool HasRedirectResult { get; init; }

    public bool HasOutcomeResult { get; init; }
}

public sealed class LucidMarketplaceMismatchResult
{
    public bool IsMismatch { get; init; }

    public string MismatchType { get; init; } = LucidMarketplaceMismatchTypes.None;

    public string? Notes { get; init; }
}

public static class LucidMarketplaceReconciliationHelper
{
    private static readonly TimeSpan DefaultMissingOutcomeGrace = TimeSpan.FromMinutes(30);

    public static string? NormalizeRedirectStatus(string? status)
    {
        var normalized = status?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return null;
        }

        return normalized switch
        {
            "complete" or "completed" or "success" => "Complete",
            "screened" or "screenout" or "screenedout" or "screen_out" => "Screened",
            "overquota" or "over_quota" => "OverQuota",
            "quotafull" or "quotafull" or "quota_full" => "QuotaFull",
            "quality" or "qualitytermination" or "quality_termination" or "sec_term" => "QualityTermination",
            "security" or "securitytermination" or "security_termination" => "SecurityTermination",
            "duplicate" => "Duplicate",
            "invalid" or "f_error" => "Invalid",
            "failure" or "terminate" or "terminated" => "Failure",
            "open" or "incomplete" or "pending" => "Open",
            "default" => "Default",
            _ => "Default"
        };
    }

    public static string? NormalizeFinalStatus(string? finalStatus)
    {
        var normalized = finalStatus?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return null;
        }

        return normalized switch
        {
            "complete" or "completed" or "success" => "Complete",
            "terminate" or "terminated" or "failure" => "Terminate",
            "screened" or "screenout" or "screenedout" or "screen_out" => "Screened",
            "overquota" or "over_quota" => "OverQuota",
            "quotafull" or "quotafull" or "quota_full" => "QuotaFull",
            "qualitytermination" or "quality_termination" or "quality" => "QualityTermination",
            "securitytermination" or "security_termination" or "security" => "SecurityTermination",
            "duplicate" => "Duplicate",
            "invalid" => "Invalid",
            "open" or "incomplete" or "pending" => "Open",
            "unknown" or "default" => "Unknown",
            _ => "Unknown"
        };
    }

    public static string? MapLucidFinalStatusToInternalStatus(string? finalStatus)
    {
        var normalized = NormalizeFinalStatus(finalStatus);
        return normalized switch
        {
            "Complete" => "COMPLETE",
            "Terminate" => "TERMINATE",
            "Screened" => "SCREENED",
            "QuotaFull" => "QUOTAFULL",
            "OverQuota" => "OVERQUOTA",
            "QualityTermination" or "SecurityTermination" => "SEC_TERM",
            "Duplicate" or "Invalid" => "F_ERROR",
            _ => null
        };
    }

    public static LucidMarketplaceResolvedStatusInfo ResolveRedirectStatusInfo(string? status)
    {
        var normalized = NormalizeRedirectStatus(status);
        return normalized switch
        {
            "Complete" => BuildStatus("Complete", isCompleted: true),
            "Screened" => BuildStatus("Screened", isTerminated: true),
            "QuotaFull" => BuildStatus("QuotaFull", isTerminated: true, isOverQuota: true),
            "OverQuota" => BuildStatus("OverQuota", isTerminated: true, isOverQuota: true),
            "QualityTermination" => BuildStatus("QualityTermination", isTerminated: true, isQualityTermination: true),
            "SecurityTermination" => BuildStatus("SecurityTermination", isTerminated: true, isQualityTermination: true, isSecurityTermination: true),
            "Duplicate" => BuildStatus("Duplicate", isTerminated: true, isQualityTermination: true, isDuplicate: true),
            "Invalid" => BuildStatus("Invalid", isTerminated: true),
            "Failure" => BuildStatus("Terminate", isTerminated: true),
            "Open" => BuildStatus("Open"),
            null => BuildStatus("Open"),
            _ => BuildStatus("Unknown", isTerminated: true)
        };
    }

    public static string MapRedirectStatusToProjectStatus(string? status)
    {
        var finalStatus = ResolveRedirectStatusInfo(status).FinalStatus;
        return MapLucidFinalStatusToInternalStatus(finalStatus) ?? "F_ERROR";
    }

    public static LucidMarketplaceReconciliationStatusResolution ResolveFinalStatus(
        LucidMarketplaceRespondentAttemptDTO? attempt,
        LucidMarketplaceRespondentOutcomeDTO? latestOutcome)
    {
        var redirectFinalStatus = HasRedirectResult(attempt)
            ? ResolveRedirectStatusInfo(attempt?.ReturnStatus).FinalStatus
            : null;
        var outcomeFinalStatus = !string.IsNullOrWhiteSpace(latestOutcome?.FinalStatus)
            ? latestOutcome!.FinalStatus!.Trim()
            : null;

        if (!string.IsNullOrWhiteSpace(outcomeFinalStatus))
        {
            return new LucidMarketplaceReconciliationStatusResolution
            {
                FinalStatus = outcomeFinalStatus,
                FinalStatusSource = "OutcomesFeed",
                RedirectFinalStatus = redirectFinalStatus,
                OutcomeFinalStatus = outcomeFinalStatus,
                HasRedirectResult = HasRedirectResult(attempt),
                HasOutcomeResult = true
            };
        }

        if (!string.IsNullOrWhiteSpace(redirectFinalStatus) &&
            !string.Equals(redirectFinalStatus, "Open", StringComparison.OrdinalIgnoreCase))
        {
            return new LucidMarketplaceReconciliationStatusResolution
            {
                FinalStatus = redirectFinalStatus,
                FinalStatusSource = "RedirectReturn",
                RedirectFinalStatus = redirectFinalStatus,
                OutcomeFinalStatus = null,
                HasRedirectResult = true,
                HasOutcomeResult = false
            };
        }

        return new LucidMarketplaceReconciliationStatusResolution
        {
            FinalStatus = attempt != null || latestOutcome != null ? "Open" : "Unknown",
            FinalStatusSource = "None",
            RedirectFinalStatus = redirectFinalStatus,
            OutcomeFinalStatus = outcomeFinalStatus,
            HasRedirectResult = HasRedirectResult(attempt),
            HasOutcomeResult = !string.IsNullOrWhiteSpace(outcomeFinalStatus)
        };
    }

    public static LucidMarketplaceMismatchResult DetermineMismatch(
        LucidMarketplaceRespondentAttemptDTO? attempt,
        LucidMarketplaceRespondentOutcomeDTO? latestOutcome,
        bool outcomeChangedOverTime,
        DateTime? now = null,
        TimeSpan? missingOutcomeGrace = null)
    {
        var resolved = ResolveFinalStatus(attempt, latestOutcome);
        var redirectFinalStatus = resolved.RedirectFinalStatus;
        var outcomeFinalStatus = resolved.OutcomeFinalStatus;
        var hasRedirect = resolved.HasRedirectResult;
        var hasOutcome = resolved.HasOutcomeResult;

        if (outcomeChangedOverTime)
        {
            return new LucidMarketplaceMismatchResult
            {
                IsMismatch = true,
                MismatchType = LucidMarketplaceMismatchTypes.OutcomeChangedOverTime,
                Notes = "Multiple async outcome statuses were observed for this respondent/session before the latest value settled."
            };
        }

        if (!hasRedirect && hasOutcome)
        {
            return new LucidMarketplaceMismatchResult
            {
                IsMismatch = true,
                MismatchType = LucidMarketplaceMismatchTypes.RedirectMissingOutcomeExists,
                Notes = "An async outcome exists, but no browser redirect-return was captured."
            };
        }

        if (hasRedirect && !hasOutcome)
        {
            var grace = missingOutcomeGrace ?? DefaultMissingOutcomeGrace;
            var attemptedOn = attempt?.AttemptedOn ?? attempt?.ModifiedDate ?? attempt?.CreatedDate;
            if (!attemptedOn.HasValue || attemptedOn.Value <= (now ?? DateTime.Now).Add(-grace))
            {
                return new LucidMarketplaceMismatchResult
                {
                    IsMismatch = true,
                    MismatchType = LucidMarketplaceMismatchTypes.RedirectExistsOutcomeMissing,
                    Notes = "A redirect-return exists, but no async outcome has been received within the review window."
                };
            }

            return new LucidMarketplaceMismatchResult
            {
                IsMismatch = false,
                MismatchType = LucidMarketplaceMismatchTypes.None,
                Notes = "Awaiting async outcome within the review window."
            };
        }

        if (!hasRedirect || !hasOutcome || string.Equals(redirectFinalStatus, outcomeFinalStatus, StringComparison.OrdinalIgnoreCase))
        {
            return new LucidMarketplaceMismatchResult
            {
                IsMismatch = false,
                MismatchType = LucidMarketplaceMismatchTypes.None
            };
        }

        if (string.Equals(redirectFinalStatus, "OverQuota", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(outcomeFinalStatus, "Complete", StringComparison.OrdinalIgnoreCase))
        {
            return new LucidMarketplaceMismatchResult
            {
                IsMismatch = true,
                MismatchType = LucidMarketplaceMismatchTypes.RedirectOverQuotaOutcomeComplete,
                Notes = "Redirect-return marked the respondent over quota, but the async outcomes feed finalized the session as complete."
            };
        }

        if (string.Equals(redirectFinalStatus, "Complete", StringComparison.OrdinalIgnoreCase) &&
            IsTerminateFamily(outcomeFinalStatus))
        {
            return new LucidMarketplaceMismatchResult
            {
                IsMismatch = true,
                MismatchType = LucidMarketplaceMismatchTypes.RedirectCompleteOutcomeTerminate,
                Notes = "Redirect-return marked the respondent complete, but the async outcomes feed finalized the session as non-complete."
            };
        }

        if (IsTerminateFamily(redirectFinalStatus) &&
            string.Equals(outcomeFinalStatus, "Complete", StringComparison.OrdinalIgnoreCase))
        {
            return new LucidMarketplaceMismatchResult
            {
                IsMismatch = true,
                MismatchType = LucidMarketplaceMismatchTypes.RedirectTerminateOutcomeComplete,
                Notes = "Redirect-return marked the respondent terminated, but the async outcomes feed finalized the session as complete."
            };
        }

        return new LucidMarketplaceMismatchResult
        {
            IsMismatch = true,
            MismatchType = LucidMarketplaceMismatchTypes.UnknownDifference,
            Notes = "Redirect-return and async outcomes feed disagree on the final respondent status."
        };
    }

    private static LucidMarketplaceResolvedStatusInfo BuildStatus(
        string finalStatus,
        bool isCompleted = false,
        bool isTerminated = false,
        bool isOverQuota = false,
        bool isQualityTermination = false,
        bool isDuplicate = false,
        bool isSecurityTermination = false)
    {
        return new LucidMarketplaceResolvedStatusInfo
        {
            FinalStatus = finalStatus,
            IsCompleted = isCompleted,
            IsTerminated = isTerminated,
            IsOverQuota = isOverQuota,
            IsQualityTermination = isQualityTermination,
            IsDuplicate = isDuplicate,
            IsSecurityTermination = isSecurityTermination
        };
    }

    private static bool HasRedirectResult(LucidMarketplaceRespondentAttemptDTO? attempt)
    {
        return !string.IsNullOrWhiteSpace(attempt?.ReturnStatus) ||
               !string.IsNullOrWhiteSpace(attempt?.ReturnCode) ||
               !string.IsNullOrWhiteSpace(attempt?.ReturnRawQuery);
    }

    private static bool IsTerminateFamily(string? finalStatus)
    {
        return finalStatus is "Terminate" or "OverQuota" or "QualityTermination" or "Duplicate" or "SecurityTermination";
    }
}
