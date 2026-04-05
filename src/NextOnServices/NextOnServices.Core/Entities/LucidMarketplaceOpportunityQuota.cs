using System.ComponentModel.DataAnnotations.Schema;

namespace NextOnServices.Core.Entities;

[Dapper.Contrib.Extensions.Table("LucidMarketplaceOpportunityQuota")]
public class LucidMarketplaceOpportunityQuota
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}
