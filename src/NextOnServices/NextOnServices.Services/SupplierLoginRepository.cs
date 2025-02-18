using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Services;

public class SupplierLoginRepository : DapperGenericRepository<SupplierLogin>, ISupplierLoginRepository
{
    public SupplierLoginRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {

    }
}
