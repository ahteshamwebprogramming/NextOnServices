using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class UserSubscriptionRepository : GenericRepository<UserSubscription>, IUserSubscriptionRepository
{
    public UserSubscriptionRepository(NextOnServicesDbContext context) : base(context)
    {
    }

    public Task<UserSubscription?> GetActiveSubscriptionAsync(int employerId, DateTime asOfUtc, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<UserSubscription>()
            .Include(subscription => subscription.EmployerCvSearchData)
            .FirstOrDefaultAsync(subscription =>
                subscription.UserId == employerId &&
                subscription.IsActive &&
                subscription.ValidFrom <= asOfUtc &&
                (!subscription.ValidTo.HasValue || subscription.ValidTo.Value >= asOfUtc),
                cancellationToken);
    }
}
