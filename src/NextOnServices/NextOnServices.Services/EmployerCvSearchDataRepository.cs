using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class EmployerCvSearchDataRepository : GenericRepository<EmployerCvSearchDatum>, IEmployerCvSearchDataRepository
{
    public EmployerCvSearchDataRepository(NextOnServicesDbContext context) : base(context)
    {
    }

    public Task AddEntryAsync(EmployerCvSearchDatum entry, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<EmployerCvSearchDatum>().AddAsync(entry, cancellationToken).AsTask();
    }
}
