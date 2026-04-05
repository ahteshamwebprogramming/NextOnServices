using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class LucidMarketplaceReconciliationRunRepository : DapperGenericRepository<LucidMarketplaceReconciliationRun>, ILucidMarketplaceReconciliationRunRepository
{
    public LucidMarketplaceReconciliationRunRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}
