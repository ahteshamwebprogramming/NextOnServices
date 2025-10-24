using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Services;

namespace NextOnServices.WebUI.VT.Controllers;

[Area("VT")]
[Authorize]
public class CvController : Controller
{
    private readonly IEmployerSubscriptionService _subscriptionService;

    public CvController(IEmployerSubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet("/VT/CV")]
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> QuotaSummary(CancellationToken cancellationToken)
    {
        var employerId = GetEmployerId();
        if (employerId == null)
        {
            return Unauthorized();
        }

        var quota = await _subscriptionService.GetQuotaAsync(employerId.Value, cancellationToken);
        if (quota == null)
        {
            return NotFound(new { message = "No active subscription found." });
        }

        return Json(new
        {
            subscriptionId = quota.SubscriptionId,
            planName = quota.PlanName,
            planCode = quota.PlanCode,
            planType = quota.PlanType,
            planDescription = quota.PlanDescription,
            planMetadata = quota.PlanMetadata,
            planQuota = quota.PlanQuota,
            unlocked = quota.UnlockCount,
            remaining = quota.Remaining,
            validFrom = quota.ValidFrom,
            validTo = quota.ValidTo
        });
    }

    [HttpPost]
    public async Task<IActionResult> Unlock([FromBody] UnlockCvRequest? request, CancellationToken cancellationToken)
    {
        if (request == null || request.JobseekerId <= 0)
        {
            return BadRequest(new { message = "A valid jobseeker identifier is required." });
        }

        var employerId = GetEmployerId();
        if (employerId == null)
        {
            return Unauthorized();
        }

        var result = await _subscriptionService.TryUnlockCvAsync(employerId.Value, request.JobseekerId, cancellationToken);
        if (!result.Success)
        {
            return BadRequest(new
            {
                message = result.ErrorMessage,
                remaining = result.RemainingQuota
            });
        }

        return Ok(new
        {
            message = "CV unlocked successfully.",
            unlocked = result.UnlockCount,
            remaining = result.RemainingQuota
        });
    }

    private int? GetEmployerId()
    {
        var claimValue = User?.FindFirst("Id")?.Value;
        if (int.TryParse(claimValue, out var employerId))
        {
            return employerId;
        }

        return null;
    }

    public class UnlockCvRequest
    {
        public int JobseekerId { get; set; }
    }
}
