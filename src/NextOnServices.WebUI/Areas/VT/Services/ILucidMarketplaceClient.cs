using NextOnServices.Infrastructure.Models.APIProjects;

namespace NextOnServices.WebUI.VT.Services;

public interface ILucidMarketplaceClient
{
    Task<LucidMarketplaceProxyResponse> TestConnectivityAsync(LucidMarketplaceProxyRequest request);

    Task<LucidMarketplaceProxyResponse> CreateSubscriptionAsync(LucidMarketplaceProxyRequest request);

    Task<LucidMarketplaceProxyResponse> GetSubscriptionsAsync(LucidMarketplaceProxyRequest request);

    Task<LucidMarketplaceProxyResponse> DeleteSubscriptionAsync(LucidMarketplaceProxyRequest request);

    Task<LucidMarketplaceProxyResponse> CreateSupplierLinkAsync(LucidMarketplaceProxyRequest request);

    Task<LucidMarketplaceProxyResponse> GetSupplierLinkAsync(LucidMarketplaceProxyRequest request);

    Task<LucidMarketplaceProxyResponse> UpdateSupplierLinkAsync(LucidMarketplaceProxyRequest request);
}
