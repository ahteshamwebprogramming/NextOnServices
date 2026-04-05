namespace NextOnServices.Infrastructure.Models.APIProjects;

public sealed class LucidMarketplaceOutcomeStatusInfo
{
    public string FinalStatus { get; init; } = "Unknown";

    public bool IsFinal { get; init; }

    public bool IsCompleted { get; init; }

    public bool IsTerminated { get; init; }

    public bool IsOverQuota { get; init; }

    public bool IsQualityTermination { get; init; }

    public bool IsDuplicate { get; init; }

    public bool IsSecurityTermination { get; init; }
}

public static class LucidMarketplaceOutcomeStatusHelper
{
    private static readonly HashSet<int> OpenMarketplaceStatuses = new() { -1, 1, 3 };
    private static readonly HashSet<int> TerminateMarketplaceStatuses = new() { 21, 23, 24, 50, 98, 120, 122, 123, 124, 125, 126, 301, 302, 303 };
    private static readonly HashSet<int> OverQuotaMarketplaceStatuses = new() { 40, 41, 42 };
    private static readonly HashSet<int> DuplicateMarketplaceStatuses = new() { 35, 36, 138, 139, 230 };
    private static readonly HashSet<int> SecurityMarketplaceStatuses = new() { 30, 32, 37, 131, 132, 133, 234, 236, 237, 238, 240, 241, 242, 243 };
    private static readonly HashSet<int> QualityMarketplaceStatuses = new() { 30, 32, 35, 36, 37, 131, 132, 133, 138, 139, 230, 234, 236, 237, 238, 240, 241, 242, 243 };

    private static readonly HashSet<int> OpenClientStatuses = new() { -1, 1, 110 };
    private static readonly HashSet<int> CompleteClientStatuses = new() { 10, 11, 70 };
    private static readonly HashSet<int> TerminateClientStatuses = new() { 20, 26, 28, 80 };
    private static readonly HashSet<int> OverQuotaClientStatuses = new() { 40 };
    private static readonly HashSet<int> SecurityClientStatuses = new() { 33, 34, 35, 134, 135, 136, 137, 233, 235, 335 };
    private static readonly HashSet<int> QualityClientStatuses = new() { 30, 33, 34, 35, 38, 134, 135, 136, 137, 233, 235, 335 };

    public static LucidMarketplaceOutcomeStatusInfo Resolve(int? marketplaceStatus, int? clientStatus)
    {
        if (clientStatus.HasValue && CompleteClientStatuses.Contains(clientStatus.Value))
        {
            return Build("Complete", isFinal: true, isCompleted: true);
        }

        if ((clientStatus.HasValue && OverQuotaClientStatuses.Contains(clientStatus.Value)) ||
            (marketplaceStatus.HasValue && OverQuotaMarketplaceStatuses.Contains(marketplaceStatus.Value)))
        {
            return Build("OverQuota", isFinal: true, isTerminated: true, isOverQuota: true);
        }

        if (marketplaceStatus.HasValue && DuplicateMarketplaceStatuses.Contains(marketplaceStatus.Value))
        {
            return Build("Duplicate", isFinal: true, isTerminated: true, isQualityTermination: true, isDuplicate: true);
        }

        if ((clientStatus.HasValue && SecurityClientStatuses.Contains(clientStatus.Value)) ||
            (marketplaceStatus.HasValue && SecurityMarketplaceStatuses.Contains(marketplaceStatus.Value)))
        {
            return Build("SecurityTermination", isFinal: true, isTerminated: true, isQualityTermination: true, isSecurityTermination: true);
        }

        if ((clientStatus.HasValue && QualityClientStatuses.Contains(clientStatus.Value)) ||
            (marketplaceStatus.HasValue && QualityMarketplaceStatuses.Contains(marketplaceStatus.Value)))
        {
            return Build("QualityTermination", isFinal: true, isTerminated: true, isQualityTermination: true);
        }

        if ((clientStatus.HasValue && TerminateClientStatuses.Contains(clientStatus.Value)) ||
            (marketplaceStatus.HasValue && TerminateMarketplaceStatuses.Contains(marketplaceStatus.Value)))
        {
            return Build("Terminate", isFinal: true, isTerminated: true);
        }

        if ((clientStatus.HasValue && OpenClientStatuses.Contains(clientStatus.Value)) ||
            (marketplaceStatus.HasValue && OpenMarketplaceStatuses.Contains(marketplaceStatus.Value)))
        {
            return Build("Open", isFinal: false);
        }

        return Build("Unknown", isFinal: false);
    }

    private static LucidMarketplaceOutcomeStatusInfo Build(
        string finalStatus,
        bool isFinal,
        bool isCompleted = false,
        bool isTerminated = false,
        bool isOverQuota = false,
        bool isQualityTermination = false,
        bool isDuplicate = false,
        bool isSecurityTermination = false)
    {
        return new LucidMarketplaceOutcomeStatusInfo
        {
            FinalStatus = finalStatus,
            IsFinal = isFinal,
            IsCompleted = isCompleted,
            IsTerminated = isTerminated,
            IsOverQuota = isOverQuota,
            IsQualityTermination = isQualityTermination,
            IsDuplicate = isDuplicate,
            IsSecurityTermination = isSecurityTermination
        };
    }
}
