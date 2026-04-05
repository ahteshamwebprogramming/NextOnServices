namespace NextOnServices.Infrastructure.Models.APIProjects;

public class LucidMarketplaceSubscriptionDTO
{
    public int LucidMarketplaceSubscriptionId { get; set; }

    public string? SubscriptionType { get; set; }

    public string? SupplierCode { get; set; }

    public string? CallbackUrl { get; set; }

    public bool IncludeQuotas { get; set; }

    public string? RemoteSubscriptionId { get; set; }

    public string? LastStatus { get; set; }

    public string? RequestPayloadSnapshot { get; set; }

    public string? ResponsePayloadSnapshot { get; set; }

    public string? WebhookKeyId { get; set; }

    public string? WebhookKeyIdFull { get; set; }

    public string? WebhookPublicKey { get; set; }

    public string? WebhookSecuritySnapshot { get; set; }

    public bool IsActive { get; set; }

    public DateTime? LastValidatedOn { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}
