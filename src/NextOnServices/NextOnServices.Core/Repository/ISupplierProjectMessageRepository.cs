using System;
using System.Threading.Tasks;
using NextOnServices.Core.Entities;

namespace NextOnServices.Core.Repository;

public interface ISupplierProjectMessageRepository : IDapperRepository<SupplierProjectMessage>
{
    Task<int> MarkSupplierMessagesAsReadAsync(int projectMappingId, DateTime readUtc);
    Task<int> MarkMessagesAsReadAsync(int projectMappingId, bool fromSupplier, DateTime readUtc);
}
