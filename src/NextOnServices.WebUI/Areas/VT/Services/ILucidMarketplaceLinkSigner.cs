using System.Collections.Generic;

namespace NextOnServices.WebUI.VT.Services;

public interface ILucidMarketplaceLinkSigner
{
    LucidMarketplaceSignedLinkResult BuildSignedEntryLink(LucidMarketplaceLinkSignRequest request);
    string ComputeUrlSafeHmacSha1(string secretKey, string unsignedUrl);
    string ComputeCintEmailHash(string rawEmail);
}

public sealed class LucidMarketplaceLinkSignRequest
{
    public string? BaseLink { get; init; }
    public string? SecretKey { get; init; }
    public string? RespondentId { get; init; }
    public string? SessionId { get; init; }
    public string? CountryLanguageId { get; init; }
    public string? MarketplaceAttemptId { get; init; }
    public string? CintEmailRaw { get; init; }
    public IReadOnlyDictionary<string, string?>? ExtraParameters { get; init; }
    public int? OpportunityId { get; init; }
    public int? InternalProjectId { get; init; }
    public int? LucidSurveyId { get; init; }
    public string? AttemptType { get; init; }
}

public sealed class LucidMarketplaceSignedLinkResult
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public string UnsignedUrl { get; init; } = string.Empty;
    public string SignedUrl { get; init; } = string.Empty;
    public string HashValue { get; init; } = string.Empty;
    public bool IncludedCintEmail { get; init; }
}
