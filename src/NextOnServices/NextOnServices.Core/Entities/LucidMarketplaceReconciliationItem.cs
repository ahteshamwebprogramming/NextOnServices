using System.ComponentModel.DataAnnotations.Schema;

namespace NextOnServices.Core.Entities;

[Dapper.Contrib.Extensions.Table("LucidMarketplaceReconciliationItem")]
public class LucidMarketplaceReconciliationItem
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int ReconciliationRunId { get; set; }

    public int? LucidMarketplaceRespondentAttemptId { get; set; }

    public int? LucidMarketplaceRespondentOutcomeId { get; set; }

    public int? LucidSurveyId { get; set; }

    public int? InternalProjectId { get; set; }

    public string? SessionId { get; set; }

    public string? RespondentId { get; set; }

    public string? PanelistId { get; set; }

    public string? RedirectStatus { get; set; }

    public string? RedirectCode { get; set; }

    public int? OutcomeMarketplaceStatus { get; set; }

    public int? OutcomeClientStatus { get; set; }

    public string? FinalStatus { get; set; }

    public string? FinalStatusSource { get; set; }

    public bool IsMismatch { get; set; }

    public string? MismatchType { get; set; }

    public decimal? RevenueValue { get; set; }

    public string? RevenueCurrencyCode { get; set; }

    public string? Notes { get; set; }

    public string? RawSnapshotJson { get; set; }

    public DateTime? CreatedDate { get; set; }
}
