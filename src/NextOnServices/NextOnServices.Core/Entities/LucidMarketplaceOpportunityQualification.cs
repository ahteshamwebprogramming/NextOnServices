using System.ComponentModel.DataAnnotations.Schema;

namespace NextOnServices.Core.Entities;

[Dapper.Contrib.Extensions.Table("LucidMarketplaceOpportunityQualification")]
public class LucidMarketplaceOpportunityQualification
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LucidMarketplaceOpportunityQualificationId { get; set; }

    public int LucidMarketplaceOpportunityId { get; set; }

    public string? SupplierCode { get; set; }

    public int SurveyId { get; set; }

    public int? QuestionId { get; set; }

    public string? QuestionText { get; set; }

    public string? LogicalOperator { get; set; }

    public string? PrecodesJson { get; set; }

    public string? RawJson { get; set; }

    public int? SortOrder { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}
