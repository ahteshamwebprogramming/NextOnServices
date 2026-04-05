using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class LucidMarketplaceSubscriptionRepository : DapperGenericRepository<LucidMarketplaceSubscription>, ILucidMarketplaceSubscriptionRepository
{
    public LucidMarketplaceSubscriptionRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}
