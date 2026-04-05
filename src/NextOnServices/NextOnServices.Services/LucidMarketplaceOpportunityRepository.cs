using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class LucidMarketplaceOpportunityRepository : DapperGenericRepository<LucidMarketplaceOpportunity>, ILucidMarketplaceOpportunityRepository
{
    public LucidMarketplaceOpportunityRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}
