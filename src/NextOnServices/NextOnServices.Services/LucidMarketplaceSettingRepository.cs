using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class LucidMarketplaceSettingRepository : DapperGenericRepository<LucidMarketplaceSetting>, ILucidMarketplaceSettingRepository
{
    public LucidMarketplaceSettingRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}
