using Microsoft.AspNetCore.Mvc;
using Consultings.WebUI.Models;
using Consultings.WebUI.Services;

namespace Consultings.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CRSDataController : ControllerBase
{
    private readonly ICRSDataService _dataService;
    private readonly ILogger<CRSDataController> _logger;

    public CRSDataController(ICRSDataService dataService, ILogger<CRSDataController> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    /// <summary>
    /// Creates or updates a user record
    /// POST /api/CRSData/Users
    /// </summary>
    [HttpPost("Users")]
    public async Task<IActionResult> CreateOrUpdateUser([FromBody] CRS_Users user)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _dataService.GetUserByEmailAsync(user.Email);

            if (existingUser != null)
            {
                // Update existing user
                user.Id = existingUser.Id;
                user.ModifiedDate = DateTime.UtcNow;
                
                var id = await _dataService.UpdateUserAsync(user);

                _logger.LogInformation("User updated: {Email}", user.Email);
                return Ok(new { message = "User updated successfully", id = id });
            }
            else
            {
                // Create new user
                if (user.CreatedDate == default)
                {
                    user.CreatedDate = DateTime.UtcNow;
                }

                var newId = await _dataService.CreateUserAsync(user);
                user.Id = newId;

                _logger.LogInformation("User created: {Email}", user.Email);
                return CreatedAtAction(nameof(CreateOrUpdateUser), new { id = newId }, user);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing user: {Email}", user.Email);
            return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new guest booking record
    /// POST /api/CRSData/GuestBooking
    /// Accepts nested JSON structure: { "booking": { ... } }
    /// </summary>
    [HttpPost("GuestBooking")]
    public async Task<IActionResult> CreateGuestBooking([FromBody] GuestBookingRequest request)
    {
        try
        {
            if (request?.Booking == null)
            {
                return BadRequest(new { message = "Booking data is required. Expected format: { \"booking\": { ... } }" });
            }

            var booking = request.Booking;

            // Validate the booking model
            if (!TryValidateModel(booking))
            {
                return BadRequest(ModelState);
            }

            // Set default dates if not provided
            if (booking.BookingCreatedDate == null || booking.BookingCreatedDate == default)
            {
                booking.BookingCreatedDate = DateTime.UtcNow;
            }

            if (booking.BookingModifiedDate == null || booking.BookingModifiedDate == default)
            {
                booking.BookingModifiedDate = DateTime.UtcNow;
            }

            var newId = await _dataService.CreateGuestBookingAsync(booking);
            booking.Id = newId;

            _logger.LogInformation("Guest booking created: {BookingId} with Id: {Id}", booking.BookingId, newId);
            
            // Return 201 Created with the nested format matching the request
            return CreatedAtAction(
                nameof(CreateGuestBooking), 
                new { id = newId }, 
                new { booking = booking });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing guest booking: {BookingId}", request?.Booking?.BookingId);
            return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new visitors session log record
    /// POST /api/CRSData/VisitorsSessionLog
    /// </summary>
    [HttpPost("VisitorsSessionLog")]
    public async Task<IActionResult> CreateVisitorsSessionLog([FromBody] CRS_VisitorsSessionLog sessionLog)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (sessionLog.CreatedAtUtc == default)
            {
                sessionLog.CreatedAtUtc = DateTime.UtcNow;
            }

            var newId = await _dataService.CreateVisitorsSessionLogAsync(sessionLog);
            sessionLog.Id = newId;

            _logger.LogInformation("Visitors session log created: {SessionId}", sessionLog.SessionId);
            return CreatedAtAction(nameof(CreateVisitorsSessionLog), new { id = newId }, sessionLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing visitors session log: {SessionId}", sessionLog.SessionId);
            return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets all users
    /// GET /api/CRSData/Users
    /// </summary>
    [HttpGet("Users")]
    public async Task<ActionResult<IEnumerable<CRS_Users>>> GetUsers()
    {
        try
        {
            var users = await _dataService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, new { message = "An error occurred while retrieving users", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets all guest bookings
    /// GET /api/CRSData/GuestBooking
    /// </summary>
    [HttpGet("GuestBooking")]
    public async Task<ActionResult<IEnumerable<CRS_GuestBooking>>> GetGuestBookings()
    {
        try
        {
            var bookings = await _dataService.GetAllGuestBookingsAsync();
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving guest bookings");
            return StatusCode(500, new { message = "An error occurred while retrieving guest bookings", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets all visitors session logs
    /// GET /api/CRSData/VisitorsSessionLog
    /// </summary>
    [HttpGet("VisitorsSessionLog")]
    public async Task<ActionResult<IEnumerable<CRS_VisitorsSessionLog>>> GetVisitorsSessionLogs()
    {
        try
        {
            var logs = await _dataService.GetAllVisitorsSessionLogsAsync();
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving visitors session logs");
            return StatusCode(500, new { message = "An error occurred while retrieving visitors session logs", error = ex.Message });
        }
    }
}
