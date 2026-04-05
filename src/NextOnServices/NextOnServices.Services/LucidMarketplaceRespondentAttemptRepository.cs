using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class LucidMarketplaceRespondentAttemptRepository : DapperGenericRepository<LucidMarketplaceRespondentAttempt>, ILucidMarketplaceRespondentAttemptRepository
{
    public LucidMarketplaceRespondentAttemptRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}
