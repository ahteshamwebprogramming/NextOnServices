namespace NextOnServices.Infrastructure.Models.APIProjects;

public class LucidMarketplaceOpportunityQuotaDTO
{
    public int LucidMarketplaceOpportunityQuotaId { get; set; }

    public int LucidMarketplaceOpportunityId { get; set; }

    public string? SupplierCode { get; set; }

    public int SurveyId { get; set; }

    public int? SurveyQuotaId { get; set; }

    public string? SurveyQuotaType { get; set; }

    public decimal? Conversion { get; set; }

    public int? NumberOfRespondents { get; set; }

    public string? QuestionsJson { get; set; }

    public string? RawJson { get; set; }

    public int? SortOrder { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
