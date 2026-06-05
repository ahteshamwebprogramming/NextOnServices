using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class TorfacMarketplaceSettingRepository : DapperGenericRepository<TorfacMarketplaceSetting>, ITorfacMarketplaceSettingRepository
{
    public TorfacMarketplaceSettingRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}
