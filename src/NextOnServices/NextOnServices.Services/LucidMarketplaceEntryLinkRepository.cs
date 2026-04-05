using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class LucidMarketplaceEntryLinkRepository : DapperGenericRepository<LucidMarketplaceEntryLink>, ILucidMarketplaceEntryLinkRepository
{
    public LucidMarketplaceEntryLinkRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}
