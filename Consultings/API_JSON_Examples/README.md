# CRS Data API JSON Examples

This folder contains JSON examples for the three external APIs that receive data from the CRS system.

## APIs

1. **Users API**: `https://08-Consultings.com/api/CRSData/Users`
   - Receives user registration data
   - Example: `Users_API_Example.json`

2. **GuestBooking API**: `https://08-Consultings.com/api/CRSData/GuestBooking`
   - Receives guest booking data
   - Example: `GuestBooking_API_Example.json`

3. **VisitorsSessionLog API**: `https://08-Consultings.com/api/CRSData/VisitorsSessionLog`
   - Receives visitor session log data
   - Example: `VisitorsSessionLog_API_Example.json`

## Implementation Notes

- All API calls are **fire-and-forget** (asynchronous, non-blocking)
- No response validation is performed
- The system does not wait for API responses
- API failures are logged but do not affect the main application flow
- All data is posted in **camelCase** JSON format

## Configuration

API URLs can be configured in `appsettings.json`:

```json
{
  "CRSDataAPI": {
    "UsersUrl": "https://08-Consultings.com/api/CRSData/Users",
    "GuestBookingUrl": "https://08-Consultings.com/api/CRSData/GuestBooking",
    "VisitorsSessionLogUrl": "https://08-Consultings.com/api/CRSData/VisitorsSessionLog"
  }
}
```

If not configured, the default URLs shown above will be used.

## Data Posting Triggers

- **Users**: Posted when a new user is registered (`AccountController.Register`)
- **GuestBooking**: Posted when a booking is saved (`HomeController.SaveGuestBookingAsync`)
- **VisitorsSessionLog**: Posted when a visitor session is logged (`VisitorAnalyticsService.LogVisitorAsync`)

