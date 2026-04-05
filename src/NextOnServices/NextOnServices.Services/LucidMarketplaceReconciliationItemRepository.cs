using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class LucidMarketplaceReconciliationItemRepository : DapperGenericRepository<LucidMarketplaceReconciliationItem>, ILucidMarketplaceReconciliationItemRepository
{
    public LucidMarketplaceReconciliationItemRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}
