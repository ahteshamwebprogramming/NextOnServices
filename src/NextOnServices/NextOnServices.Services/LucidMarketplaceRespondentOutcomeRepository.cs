using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class LucidMarketplaceRespondentOutcomeRepository : DapperGenericRepository<LucidMarketplaceRespondentOutcome>, ILucidMarketplaceRespondentOutcomeRepository
{
    public LucidMarketplaceRespondentOutcomeRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}
