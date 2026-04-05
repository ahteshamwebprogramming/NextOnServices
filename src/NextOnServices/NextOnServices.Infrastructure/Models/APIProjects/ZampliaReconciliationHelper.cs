namespace NextOnServices.Infrastructure.Models.APIProjects;

public sealed class ZampliaResolvedStatusInfo
{
    public string NormalizedStatus { get; init; } = "Unknown";

    public string ProjectStatus { get; init; } = "SEC_TERM";

    public bool IsCompleted { get; init; }

    public bool IsTerminated { get; init; }

    public bool IsOverQuota { get; init; }

    public bool IsQualityTermination { get; init; }

    public bool IsSecurityTermination { get; init; }

    public bool IsDuplicate { get; init; }
}

public static class ZampliaReconciliationHelper
{
    public static string NormalizeReturnStatus(string? status) => ResolveStatusInfo(status).NormalizedStatus;

    public static string MapReturnStatusToProjectStatus(string? status) => ResolveStatusInfo(status).ProjectStatus;

    public static ZampliaResolvedStatusInfo ResolveAttemptFlags(string? status) => ResolveStatusInfo(status);

    public static ZampliaResolvedStatusInfo ResolveStatusInfo(string? status)
    {
        var normalized = NormalizeToken(status);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return BuildStatus("Open", "INCOMPLETE");
        }

        return normalized switch
        {
            "complete" or "completed" or "success" or "approved" => BuildStatus("Complete", "COMPLETE", isCompleted: true),
            "terminate" or "terminated" or "failure" or "failed" or "reject" or "rejected" => BuildStatus("Terminate", "TERMINATE", isTerminated: true),
            "screened" or "screenout" or "screenedout" or "screenoutfailed" => BuildStatus("Screened", "SCREENED", isTerminated: true),
            "quotafull" or "fullquota" or "quotaexhausted" => BuildStatus("QuotaFull", "QUOTAFULL", isTerminated: true, isOverQuota: true),
            "overquota" => BuildStatus("OverQuota", "OVERQUOTA", isTerminated: true, isOverQuota: true),
            "quality" or "qualitytermination" or "qualityfail" or "qualityfailure" => BuildStatus("QualityTermination", "SEC_TERM", isTerminated: true, isQualityTermination: true),
            "security" or "securitytermination" or "secterm" or "securityfail" or "securityfailure" => BuildStatus("SecurityTermination", "SEC_TERM", isTerminated: true, isSecurityTermination: true),
            "fraud" or "frauderror" or "fraudtermination" or "invalid" or "invalidsession" or "invalidtraffic" => BuildStatus("FraudError", "F_ERROR", isTerminated: true),
            "duplicate" or "duplicateentry" or "duplicatetraffic" => BuildStatus("FraudError", "F_ERROR", isTerminated: true, isDuplicate: true),
            "open" or "pending" or "incomplete" => BuildStatus("Open", "INCOMPLETE"),
            _ => BuildStatus("Unknown", "SEC_TERM", isTerminated: true)
        };
    }

    private static ZampliaResolvedStatusInfo BuildStatus(
        string normalizedStatus,
        string projectStatus,
        bool isCompleted = false,
        bool isTerminated = false,
        bool isOverQuota = false,
        bool isQualityTermination = false,
        bool isSecurityTermination = false,
        bool isDuplicate = false)
    {
        return new ZampliaResolvedStatusInfo
        {
            NormalizedStatus = normalizedStatus,
            ProjectStatus = projectStatus,
            IsCompleted = isCompleted,
            IsTerminated = isTerminated,
            IsOverQuota = isOverQuota,
            IsQualityTermination = isQualityTermination,
            IsSecurityTermination = isSecurityTermination,
            IsDuplicate = isDuplicate
        };
    }

    private static string? NormalizeToken(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return null;
        }

        return status.Trim()
            .ToLowerInvariant()
            .Replace(" ", string.Empty)
            .Replace("_", string.Empty)
            .Replace("-", string.Empty);
    }
}

