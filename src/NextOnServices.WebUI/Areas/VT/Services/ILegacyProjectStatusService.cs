namespace NextOnServices.WebUI.VT.Services;

public interface ILegacyProjectStatusService
{
    Task<LegacyProjectStatusExecutionResult> ApplyAsync(string supplierProjectUid, string? rawStatus, int rc = 0, int? pid = null, bool px = false);

    Task<string> NormalizeStatusAsync(string? rawStatus);
}

public sealed class LegacyProjectStatusExecutionResult
{
    public string SupplierProjectUid { get; init; } = string.Empty;

    public string RawStatus { get; init; } = string.Empty;

    public string NormalizedStatus { get; init; } = string.Empty;

    public string UpdateResponse { get; init; } = string.Empty;

    public string? RedirectUrl { get; init; }

    public string? HashCode { get; init; }

    public bool Success => string.Equals(UpdateResponse, "1", StringComparison.OrdinalIgnoreCase);
}
