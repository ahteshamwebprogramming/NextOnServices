using System;
using System.Threading.Tasks;
using NextOnServices.Core.Entities;

namespace NextOnServices.Core.Repository;

public interface ISupplierProjectMessageRepository : IDapperRepository<SupplierProjectMessage>
{
    Task<int> MarkMessagesAsReadAsync(int projectMappingId, bool fromSupplier, DateTime readUtc);
}
