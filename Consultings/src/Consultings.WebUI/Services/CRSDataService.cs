using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using Consultings.WebUI.Models;

namespace Consultings.WebUI.Services;

public class CRSDataService : ICRSDataService
{
    private readonly string _connectionString;
    private readonly ILogger<CRSDataService> _logger;

    public CRSDataService(IConfiguration configuration, ILogger<CRSDataService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _logger = logger;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    #region Users

    public async Task<CRS_Users?> GetUserByEmailAsync(string email)
    {
        const string sql = @"
            SELECT Id, FullName, Email, PhoneNumber, Address, City, State, Country, ZipCode,
                   IsEmailVerified, IsSubscribed, IsActive, Role, CreatedDate, ModifiedDate, LastLoginDate
            FROM CRS_Users
            WHERE Email = @Email";

        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<CRS_Users>(sql, new { Email = email });
    }

    public async Task<int> CreateUserAsync(CRS_Users user)
    {
        const string sql = @"
            INSERT INTO CRS_Users (FullName, Email, PhoneNumber, Address, City, State, Country, ZipCode,
                                   IsEmailVerified, IsSubscribed, IsActive, Role, CreatedDate, ModifiedDate, LastLoginDate)
            VALUES (@FullName, @Email, @PhoneNumber, @Address, @City, @State, @Country, @ZipCode,
                    @IsEmailVerified, @IsSubscribed, @IsActive, @Role, @CreatedDate, @ModifiedDate, @LastLoginDate);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        using var connection = CreateConnection();
        return await connection.QuerySingleAsync<int>(sql, user);
    }

    public async Task<int> UpdateUserAsync(CRS_Users user)
    {
        const string sql = @"
            UPDATE CRS_Users
            SET FullName = @FullName,
                PhoneNumber = @PhoneNumber,
                Address = @Address,
                City = @City,
                State = @State,
                Country = @Country,
                ZipCode = @ZipCode,
                IsEmailVerified = @IsEmailVerified,
                IsSubscribed = @IsSubscribed,
                IsActive = @IsActive,
                Role = @Role,
                ModifiedDate = @ModifiedDate,
                LastLoginDate = @LastLoginDate
            WHERE Email = @Email;";

        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, user);
        
        // Return the user's ID (should already be set from existing user)
        return user.Id;
    }

    public async Task<IEnumerable<CRS_Users>> GetAllUsersAsync()
    {
        const string sql = @"
            SELECT Id, FullName, Email, PhoneNumber, Address, City, State, Country, ZipCode,
                   IsEmailVerified, IsSubscribed, IsActive, Role, CreatedDate, ModifiedDate, LastLoginDate
            FROM CRS_Users
            ORDER BY CreatedDate DESC";

        using var connection = CreateConnection();
        return await connection.QueryAsync<CRS_Users>(sql);
    }

    #endregion

    #region GuestBooking

    public async Task<int> CreateGuestBookingAsync(CRS_GuestBooking booking)
    {
        const string sql = @"
            INSERT INTO CRS_GuestBooking (
                Email, PhoneNumber, Address, Country, City, ZipCode, CountryId, CityId,
                CheckInDate, CheckOutDate, NoOfNights, TotalAdults, TotalChildren, TotalRooms, TotalPrice,
                RoomDetails, GuestDetails, IsPaymentDone, PaymentAmount, PaymentMethod, PaymentTransactionId,
                PaymentStatus, PaymentDate, PaymentRemarks, IsPostedToPms, PmsPostedDate, PmsGroupId,
                PmsGuestIds, PmsResponse, PmsError, BookingCreatedDate, BookingModifiedDate, BookingId,
                Remarks, Status, SessionId, UserId
            )
            VALUES (
                @Email, @PhoneNumber, @Address, @Country, @City, @ZipCode, @CountryId, @CityId,
                @CheckInDate, @CheckOutDate, @NoOfNights, @TotalAdults, @TotalChildren, @TotalRooms, @TotalPrice,
                @RoomDetails, @GuestDetails, @IsPaymentDone, @PaymentAmount, @PaymentMethod, @PaymentTransactionId,
                @PaymentStatus, @PaymentDate, @PaymentRemarks, @IsPostedToPms, @PmsPostedDate, @PmsGroupId,
                @PmsGuestIds, @PmsResponse, @PmsError, @BookingCreatedDate, @BookingModifiedDate, @BookingId,
                @Remarks, @Status, @SessionId, @UserId
            );
            SELECT CAST(SCOPE_IDENTITY() as int);";

        using var connection = CreateConnection();
        return await connection.QuerySingleAsync<int>(sql, booking);
    }

    public async Task<IEnumerable<CRS_GuestBooking>> GetAllGuestBookingsAsync()
    {
        const string sql = @"
            SELECT Id, Email, PhoneNumber, Address, Country, City, ZipCode, CountryId, CityId,
                   CheckInDate, CheckOutDate, NoOfNights, TotalAdults, TotalChildren, TotalRooms, TotalPrice,
                   RoomDetails, GuestDetails, IsPaymentDone, PaymentAmount, PaymentMethod, PaymentTransactionId,
                   PaymentStatus, PaymentDate, PaymentRemarks, IsPostedToPms, PmsPostedDate, PmsGroupId,
                   PmsGuestIds, PmsResponse, PmsError, BookingCreatedDate, BookingModifiedDate, BookingId,
                   Remarks, Status, SessionId, UserId
            FROM CRS_GuestBooking
            ORDER BY BookingCreatedDate DESC";

        using var connection = CreateConnection();
        return await connection.QueryAsync<CRS_GuestBooking>(sql);
    }

    #endregion

    #region VisitorsSessionLog

    public async Task<int> CreateVisitorsSessionLogAsync(CRS_VisitorsSessionLog sessionLog)
    {
        const string sql = @"
            INSERT INTO CRS_VisitorsSessionLog (
                SessionId, IpAddress, Browser, BrowserVersion, OperatingSystem, DeviceType,
                UserAgent, Referrer, Country, Region, City, Latitude, Longitude, CreatedAtUtc
            )
            VALUES (
                @SessionId, @IpAddress, @Browser, @BrowserVersion, @OperatingSystem, @DeviceType,
                @UserAgent, @Referrer, @Country, @Region, @City, @Latitude, @Longitude, @CreatedAtUtc
            );
            SELECT CAST(SCOPE_IDENTITY() as int);";

        using var connection = CreateConnection();
        return await connection.QuerySingleAsync<int>(sql, sessionLog);
    }

    public async Task<IEnumerable<CRS_VisitorsSessionLog>> GetAllVisitorsSessionLogsAsync()
    {
        const string sql = @"
            SELECT Id, SessionId, IpAddress, Browser, BrowserVersion, OperatingSystem, DeviceType,
                   UserAgent, Referrer, Country, Region, City, Latitude, Longitude, CreatedAtUtc
            FROM CRS_VisitorsSessionLog
            ORDER BY CreatedAtUtc DESC";

        using var connection = CreateConnection();
        return await connection.QueryAsync<CRS_VisitorsSessionLog>(sql);
    }

    #endregion
}

