using System.Threading;
using System.Threading.Tasks;
using NextOnServices.Core.Entities;

namespace NextOnServices.Core.Repository;

public interface IEmployerCvSearchDataRepository : IGenericRepository<EmployerCvSearchDatum>
{
    Task AddEntryAsync(EmployerCvSearchDatum entry, CancellationToken cancellationToken = default);
}
