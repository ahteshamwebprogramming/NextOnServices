using NextOnServices.Infrastructure.Models.APIProjects;

namespace NextOnServices.WebUI.VT.Services;

public interface IZampliaLaunchService
{
    Task<ZampliaLaunchResolution> TryResolveVendorLaunchAsync(ZampliaLaunchServiceRequest request);
}

public sealed class ZampliaLaunchServiceRequest
{
    public int? ZampliaSurveyId { get; init; }
    public string? ProjectMappingSid { get; init; }
    public string? ProjectMappingCode { get; init; }
    public string? SupplierProjectUid { get; init; }
    public string? SourceRespondentId { get; init; }
    public string? ClientIp { get; init; }
    public string? ClientBrowser { get; init; }
    public string? ClientDevice { get; init; }
    public string? Enc { get; init; }
    public string? LaunchRequestUrl { get; init; }
    public int? UserId { get; init; }
}

public sealed class ZampliaLaunchResolution
{
    public bool IsZampliaProject { get; init; }
    public bool Success { get; init; }
    public string? Message { get; init; }
    public string? VendorLaunchUrl { get; init; }
    public string? TransactionId { get; init; }
    public int? AttemptId { get; init; }
    public int? ZampliaSurveyId { get; init; }
    public string? SupplierProjectUid { get; init; }
    public string? SourceRespondentId { get; init; }
    public bool CreatedSupplierProject { get; init; }
    public ZampliaLaunchContextDTO? Context { get; init; }
}
