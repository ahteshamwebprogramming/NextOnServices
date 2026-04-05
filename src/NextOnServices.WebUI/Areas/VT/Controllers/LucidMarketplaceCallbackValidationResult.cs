namespace NextOnServices.WebUI.VT.Controllers;

internal sealed class LucidMarketplaceCallbackValidationResult
{
    public bool IsValid { get; init; }

    public int StatusCode { get; init; }

    public string Message { get; init; } = string.Empty;

    public string? EffectiveSupplierCode { get; init; }

    public string? ReasonCode { get; init; }

    public string? DiagnosticMessage { get; init; }

    public int? SubscriptionId { get; init; }

    public string? KeyId { get; init; }

    public long? Timestamp { get; init; }
}
