using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class SupplierProjectMessagesRepository : DapperGenericRepository<SupplierProjectMessage>, ISupplierProjectMessagesRepository
{
    public SupplierProjectMessagesRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }
}
