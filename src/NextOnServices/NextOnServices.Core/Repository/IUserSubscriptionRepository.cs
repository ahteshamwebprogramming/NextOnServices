using System;
using System.Threading;
using System.Threading.Tasks;
using NextOnServices.Core.Entities;

namespace NextOnServices.Core.Repository;

public interface IUserSubscriptionRepository : IGenericRepository<UserSubscription>
{
    Task<UserSubscription?> GetActiveSubscriptionAsync(int employerId, DateTime asOfUtc, CancellationToken cancellationToken = default);
}
