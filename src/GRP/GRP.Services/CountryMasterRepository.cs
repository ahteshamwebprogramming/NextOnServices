using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRP.Core.Entities;
using GRP.Core.Repository;
using GRP.Services.DBContext;

namespace GRP.Services;

public class CountryMasterRepository : DapperGenericRepository<CountryMaster>, ICountryMasterRepository
{
    public CountryMasterRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {

    }
}
