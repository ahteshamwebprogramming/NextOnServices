using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class LucidMarketplaceSyncLogRepository : DapperGenericRepository<LucidMarketplaceSyncLog>, ILucidMarketplaceSyncLogRepository
{
    public LucidMarketplaceSyncLogRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}
