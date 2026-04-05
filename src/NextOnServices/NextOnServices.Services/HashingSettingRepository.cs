using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class HashingSettingRepository : DapperGenericRepository<HashingSetting>, IHashingSettingRepository
{
    public HashingSettingRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}
