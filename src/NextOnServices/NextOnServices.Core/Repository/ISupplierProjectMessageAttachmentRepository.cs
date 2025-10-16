using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NextOnServices.Core.Entities;

namespace NextOnServices.Core.Repository;

public interface ISupplierProjectMessageAttachmentRepository
{
    Task AddRangeAsync(IEnumerable<SupplierProjectMessageAttachment> attachments);

    Task<SupplierProjectMessageAttachment?> GetByIdAsync(Guid id);

    Task<SupplierProjectMessageAttachment?> GetByIdForProjectAsync(Guid id, int projectMappingId);

    Task<IReadOnlyList<SupplierProjectMessageAttachment>> GetByMessageIdsAsync(IEnumerable<int> messageIds);
}
