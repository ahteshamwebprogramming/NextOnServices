namespace Consultings.WebUI.Models;

/// <summary>
/// Request DTO for GuestBooking API that wraps the booking object
/// </summary>
public class GuestBookingRequest
{
    public CRS_GuestBooking Booking { get; set; } = new();
}

