namespace NextOnServices.Infrastructure.Models.APIProjects;

public sealed class ProjectLaunchRuntimeContextDTO
{
    public int? ProjectId { get; set; }

    public string? ProjectFrom { get; set; }

    public string? ProjectIdFromApi { get; set; }

    public int? ProjectUrlId { get; set; }

    public int? ProjectMappingId { get; set; }

    public string? ProjectMappingSid { get; set; }

    public string? ProjectMappingCode { get; set; }

    public string? ProjectUrl { get; set; }

    public string? ProjectUrlOriginalUrl { get; set; }

    public string? ProjectMappingOlink { get; set; }
}
