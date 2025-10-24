using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public interface IEmployerSubscriptionService
{
    Task<SubscriptionQuotaSnapshot?> GetQuotaAsync(int employerId, CancellationToken cancellationToken = default);

    Task<UnlockCvResult> TryUnlockCvAsync(int employerId, int jobseekerId, CancellationToken cancellationToken = default);
}

public class EmployerSubscriptionService : IEmployerSubscriptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly NextOnServicesDbContext _dbContext;

    public EmployerSubscriptionService(IUnitOfWork unitOfWork, NextOnServicesDbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
    }

    public async Task<SubscriptionQuotaSnapshot?> GetQuotaAsync(int employerId, CancellationToken cancellationToken = default)
    {
        var subscription = await _unitOfWork.UserSubscription.GetActiveSubscriptionAsync(employerId, DateTime.UtcNow, cancellationToken);
        if (subscription == null)
        {
            return null;
        }

        var remaining = subscription.PlanQuota.HasValue
            ? Math.Max(subscription.PlanQuota.Value - subscription.UnlockCv, 0)
            : (int?)null;

        return new SubscriptionQuotaSnapshot(
            subscription.UserSubscriptionId,
            subscription.UserId,
            subscription.PlanName,
            subscription.PlanCode,
            subscription.PlanType,
            subscription.PlanDescription,
            subscription.PlanMetadata,
            subscription.PlanQuota,
            subscription.PlanPrice,
            subscription.UnlockCv,
            remaining,
            subscription.ValidFrom,
            subscription.ValidTo);
    }

    public async Task<UnlockCvResult> TryUnlockCvAsync(int employerId, int jobseekerId, CancellationToken cancellationToken = default)
    {
        var subscription = await _unitOfWork.UserSubscription.GetActiveSubscriptionAsync(employerId, DateTime.UtcNow, cancellationToken);
        if (subscription == null)
        {
            return UnlockCvResult.Failed("No active subscription is available for your account.");
        }

        if (subscription.PlanQuota.HasValue && subscription.UnlockCv >= subscription.PlanQuota.Value)
        {
            var remaining = Math.Max(subscription.PlanQuota.Value - subscription.UnlockCv, 0);
            return UnlockCvResult.Failed("You have reached the maximum number of CV unlocks for your current plan.", remaining);
        }

        subscription.UnlockCv += 1;
        subscription.UpdatedOn = DateTime.UtcNow;

        _dbContext.UserSubscriptions.Update(subscription);

        await _unitOfWork.EmployerCvSearchData.AddEntryAsync(new EmployerCvSearchDatum
        {
            SubscriptionId = subscription.UserSubscriptionId,
            JobseekerId = jobseekerId,
            CreatedOn = DateTime.UtcNow
        }, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var remainingQuota = subscription.PlanQuota.HasValue
            ? Math.Max(subscription.PlanQuota.Value - subscription.UnlockCv, 0)
            : (int?)null;

        return UnlockCvResult.Success(subscription.UnlockCv, remainingQuota);
    }
}

public record SubscriptionQuotaSnapshot(
    int SubscriptionId,
    int EmployerId,
    string? PlanName,
    string? PlanCode,
    string? PlanType,
    string? PlanDescription,
    string? PlanMetadata,
    int? PlanQuota,
    decimal? PlanPrice,
    int UnlockCount,
    int? Remaining,
    DateTime ValidFrom,
    DateTime? ValidTo);

public class UnlockCvResult
{
    private UnlockCvResult(bool success, string? errorMessage, int unlockCount, int? remainingQuota)
    {
        Success = success;
        ErrorMessage = errorMessage;
        UnlockCount = unlockCount;
        RemainingQuota = remainingQuota;
    }

    public bool Success { get; }

    public string? ErrorMessage { get; }

    public int UnlockCount { get; }

    public int? RemainingQuota { get; }

    public static UnlockCvResult Success(int unlockCount, int? remainingQuota) => new(true, null, unlockCount, remainingQuota);

    public static UnlockCvResult Failed(string message, int? remainingQuota = null) => new(false, message, 0, remainingQuota);
}
