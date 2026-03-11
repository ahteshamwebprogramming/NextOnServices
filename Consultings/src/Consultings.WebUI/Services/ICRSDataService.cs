using Consultings.WebUI.Models;

namespace Consultings.WebUI.Services;

public interface ICRSDataService
{
    Task<CRS_Users?> GetUserByEmailAsync(string email);
    Task<int> CreateUserAsync(CRS_Users user);
    Task<int> UpdateUserAsync(CRS_Users user);
    Task<IEnumerable<CRS_Users>> GetAllUsersAsync();

    Task<int> CreateGuestBookingAsync(CRS_GuestBooking booking);
    Task<IEnumerable<CRS_GuestBooking>> GetAllGuestBookingsAsync();

    Task<int> CreateVisitorsSessionLogAsync(CRS_VisitorsSessionLog sessionLog);
    Task<IEnumerable<CRS_VisitorsSessionLog>> GetAllVisitorsSessionLogsAsync();
}

