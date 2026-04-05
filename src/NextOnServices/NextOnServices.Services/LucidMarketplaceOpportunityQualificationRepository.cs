using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class LucidMarketplaceOpportunityQualificationRepository : DapperGenericRepository<LucidMarketplaceOpportunityQualification>, ILucidMarketplaceOpportunityQualificationRepository
{
    public LucidMarketplaceOpportunityQualificationRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}
