# Database Setup Scripts

This folder contains SQL scripts to create the 08Consultings database and all required tables.

## Scripts

### 1. CreateDatabase.sql
Creates the `08Consultings` database if it doesn't already exist.

**Usage:**
```sql
-- Run this script first
-- Execute against master database or with sufficient permissions
```

### 2. CreateTables.sql
Creates all required tables with `CRS_` prefix:
- `CRS_Users`
- `CRS_GuestBooking`
- `CRS_VisitorsSessionLog`

**Usage:**
```sql
-- Run this after CreateDatabase.sql
-- Execute against the 08Consultings database
```

## Execution Order

1. **First**, run `CreateDatabase.sql` to create the database
2. **Then**, run `CreateTables.sql` to create all tables

## Connection Details

The connection string configured in `appsettings.json`:
```
Data Source=182.18.138.110;Initial Catalog=08Consultings;User ID=sa;Password=CG$sBK9%!8P4c$;Encrypt=False;
```

## Tables Created

### CRS_Users
Stores user registration and profile information.

### CRS_GuestBooking
Stores guest booking information including payment and PMS integration details.

### CRS_VisitorsSessionLog
Stores visitor session analytics and tracking information.

## Indexes

All tables include appropriate indexes for optimal query performance:
- Unique index on `CRS_Users.Email`
- Indexes on `CRS_GuestBooking.BookingId`, `SessionId`, and `BookingCreatedDate`
- Indexes on `CRS_VisitorsSessionLog.SessionId` and `CreatedAtUtc`

## Notes

- All scripts are idempotent (can be run multiple times safely)
- Tables check for existence before creation
- Default values are set for required fields
- UTC timestamps are used for date fields
- Identity columns are used for primary keys

