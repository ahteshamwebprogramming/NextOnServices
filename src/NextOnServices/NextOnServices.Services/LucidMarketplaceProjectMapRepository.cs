using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class LucidMarketplaceProjectMapRepository : DapperGenericRepository<LucidMarketplaceProjectMap>, ILucidMarketplaceProjectMapRepository
{
    public LucidMarketplaceProjectMapRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}
