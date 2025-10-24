using System;

namespace NextOnServices.Core.Entities;

public partial class EmployerCvSearchDatum
{
    public int EmployerCvSearchDataId { get; set; }

    public int SubscriptionId { get; set; }

    public int JobseekerId { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual UserSubscription Subscription { get; set; } = null!;
}
