# CRSDataAPI Configuration Explanation

## Overview

The `CRSDataAPI` section in `appsettings.json` contains configuration URLs for the API endpoints that will be called from **another external project**. This is a reference configuration that documents where these APIs are hosted.

## Configuration in appsettings.json

```json
"CRSDataAPI": {
    "UsersUrl": "https://08-Consultings.com/api/CRSData/Users",
    "GuestBookingUrl": "https://08-Consultings.com/api/CRSData/GuestBooking",
    "VisitorsSessionLogUrl": "https://08-Consultings.com/api/CRSData/VisitorsSessionLog"
}
```

## Why This Configuration Exists

### 1. **External Project Integration**
   - These URLs are **not used by this project** to call itself
   - They are **documentation/reference** for another project that needs to call these APIs
   - The external project (likely a CRS - Central Reservation System) will use these URLs to POST data to this API

### 2. **Centralized Configuration**
   - Having URLs in configuration makes it easy to:
     - Change URLs without code changes
     - Use different URLs for different environments (dev, staging, production)
     - Document the API endpoints for other developers/projects

### 3. **Documentation Purpose**
   - Serves as a reference for what endpoints are available
   - Shows the expected base URL structure
   - Helps other developers understand the API structure

## How It Works

1. **This Project (08-Consultings)**: 
   - **Receives** data via POST requests at these endpoints
   - Stores the data in the `08Consultings` database
   - Acts as a **data receiver/storage API**

2. **External Project (CRS System)**:
   - **Sends** data via POST requests to these URLs
   - Uses these configuration URLs to know where to send data
   - Acts as a **data sender/producer**

## Example Usage in External Project

If another project needs to call these APIs, it would:

```csharp
// In the external project's configuration
var usersUrl = configuration["CRSDataAPI:UsersUrl"];
// usersUrl = "https://08-Consultings.com/api/CRSData/Users"

// Then make HTTP POST request
var httpClient = new HttpClient();
var response = await httpClient.PostAsJsonAsync(usersUrl, userData);
```

## API Endpoints

### POST /api/CRSData/Users
- **Purpose**: Create or update user records
- **Data Format**: camelCase JSON
- **Content-Type**: application/json

### POST /api/CRSData/GuestBooking
- **Purpose**: Create guest booking records
- **Data Format**: camelCase JSON
- **Content-Type**: application/json

### POST /api/CRSData/VisitorsSessionLog
- **Purpose**: Create visitor session log records
- **Data Format**: camelCase JSON
- **Content-Type**: application/json

## Important Notes

1. **These URLs are for reference only** - This project doesn't use them internally
2. **The actual API endpoints** are implemented in `CRSDataController.cs`
3. **CORS is configured** to allow cross-origin requests from other projects
4. **All data is in camelCase** JSON format as specified
5. **The base URL** (https://08-Consultings.com) should match your actual deployment URL

## Environment-Specific Configuration

You can override these URLs in environment-specific configuration files:

- `appsettings.Development.json` - For local development
- `appsettings.Production.json` - For production deployment

Example:
```json
{
  "CRSDataAPI": {
    "UsersUrl": "https://localhost:5001/api/CRSData/Users",
    "GuestBookingUrl": "https://localhost:5001/api/CRSData/GuestBooking",
    "VisitorsSessionLogUrl": "https://localhost:5001/api/CRSData/VisitorsSessionLog"
  }
}
```

