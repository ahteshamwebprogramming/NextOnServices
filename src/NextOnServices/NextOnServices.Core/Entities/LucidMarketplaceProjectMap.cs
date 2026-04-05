using System.ComponentModel.DataAnnotations.Schema;

namespace NextOnServices.Core.Entities;

[Dapper.Contrib.Extensions.Table("LucidMarketplaceProjectMap")]
public class LucidMarketplaceProjectMap
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int LucidMarketplaceOpportunityId { get; set; }

    public int LucidSurveyId { get; set; }

    public int? InternalProjectId { get; set; }

    public int? InternalProjectUrlId { get; set; }

    public int? InternalProjectMappingId { get; set; }

    public string? SupplierCode { get; set; }

    public int? AddedBy { get; set; }

    public DateTime? AddedOn { get; set; }

    public bool IsActive { get; set; }

    public string? RawJson { get; set; }
}
