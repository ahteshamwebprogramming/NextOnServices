using System;

namespace NextOnServices.Infrastructure.Models.Client;

public class ClientProjectSummary
{
    public int ProjectId { get; set; }
    public string? EncProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string? Country { get; set; }
    public string? Quota { get; set; }
    public string? Loi { get; set; }
    public double? Cpi { get; set; }
    public string? Irate { get; set; }
    public int? StatusId { get; set; }
    public string? StatusLabel { get; set; }
}
