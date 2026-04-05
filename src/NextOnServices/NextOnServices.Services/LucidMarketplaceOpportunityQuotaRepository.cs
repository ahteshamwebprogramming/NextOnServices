using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class LucidMarketplaceOpportunityQuotaRepository : DapperGenericRepository<LucidMarketplaceOpportunityQuota>, ILucidMarketplaceOpportunityQuotaRepository
{
    public LucidMarketplaceOpportunityQuotaRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}
