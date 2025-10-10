using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class SupplierProjectMessageRepository : DapperGenericRepository<SupplierProjectMessage>, ISupplierProjectMessageRepository
{
    public SupplierProjectMessageRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}
