using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class UserSubscription
{
    public int UserSubscriptionId { get; set; }

    public int UserId { get; set; }

    public string? PlanCode { get; set; }

    public string? PlanName { get; set; }

    public string? PlanDescription { get; set; }

    public string? PlanType { get; set; }

    public string? PlanMetadata { get; set; }

    public int? PlanQuota { get; set; }

    public decimal? PlanPrice { get; set; }

    public int UnlockCv { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual ICollection<EmployerCvSearchDatum> EmployerCvSearchData { get; set; } = new List<EmployerCvSearchDatum>();
}
