namespace NextOnServices.Infrastructure.Models.APIProjects;

public class LucidMarketplaceSyncLogDTO
{
    public int LucidMarketplaceSyncLogId { get; set; }

    public string? ModuleName { get; set; }

    public string? ActionName { get; set; }

    public string? RequestUrl { get; set; }

    public string? RequestBodySnapshot { get; set; }

    public int? ResponseStatusCode { get; set; }

    public string? ResponseBodySnapshot { get; set; }

    public string? Source { get; set; }

    public string? SupplierCode { get; set; }

    public bool IsSuccess { get; set; }

    public string? ErrorText { get; set; }

    public int? RelatedEntityId { get; set; }

    public int? RelatedSurveyId { get; set; }

    public DateTime? StartedOn { get; set; }

    public DateTime? CompletedOn { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }
}
