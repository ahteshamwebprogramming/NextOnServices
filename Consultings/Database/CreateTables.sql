-- =============================================
-- Script to Create Tables in 08Consultings Database
-- =============================================
-- This script creates all required tables with CRS_ prefix
-- Make sure to run CreateDatabase.sql first

USE [08Consultings]
GO

-- =============================================
-- Table: CRS_Users
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CRS_Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CRS_Users] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [FullName] NVARCHAR(255) NOT NULL,
        [Email] NVARCHAR(255) NOT NULL,
        [PhoneNumber] NVARCHAR(50) NULL,
        [Address] NVARCHAR(500) NULL,
        [City] NVARCHAR(100) NULL,
        [State] NVARCHAR(100) NULL,
        [Country] NVARCHAR(100) NULL,
        [ZipCode] NVARCHAR(20) NULL,
        [IsEmailVerified] BIT NOT NULL DEFAULT 0,
        [IsSubscribed] BIT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [Role] NVARCHAR(50) NULL,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ModifiedDate] DATETIME2 NULL,
        [LastLoginDate] DATETIME2 NULL,
        CONSTRAINT [UQ_CRS_Users_Email] UNIQUE ([Email])
    )
    
    -- Create index on Email for faster lookups
    CREATE UNIQUE NONCLUSTERED INDEX [IX_CRS_Users_Email] 
    ON [dbo].[CRS_Users] ([Email])
    
    PRINT 'Table CRS_Users created successfully.'
END
ELSE
BEGIN
    PRINT 'Table CRS_Users already exists.'
END
GO

-- =============================================
-- Table: CRS_GuestBooking
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CRS_GuestBooking]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CRS_GuestBooking] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Email] NVARCHAR(255) NULL,
        [PhoneNumber] NVARCHAR(50) NULL,
        [Address] NVARCHAR(500) NULL,
        [Country] NVARCHAR(100) NULL,
        [City] NVARCHAR(100) NULL,
        [ZipCode] NVARCHAR(20) NULL,
        [CountryId] INT NULL,
        [CityId] INT NULL,
        [CheckInDate] DATETIME2 NULL,
        [CheckOutDate] DATETIME2 NULL,
        [NoOfNights] INT NULL,
        [TotalAdults] INT NULL,
        [TotalChildren] INT NULL,
        [TotalRooms] INT NULL,
        [TotalPrice] DECIMAL(18,2) NULL,
        [RoomDetails] NVARCHAR(MAX) NULL,
        [GuestDetails] NVARCHAR(MAX) NULL,
        [IsPaymentDone] BIT NOT NULL DEFAULT 0,
        [PaymentAmount] DECIMAL(18,2) NULL,
        [PaymentMethod] NVARCHAR(100) NULL,
        [PaymentTransactionId] NVARCHAR(255) NULL,
        [PaymentStatus] NVARCHAR(50) NULL,
        [PaymentDate] DATETIME2 NULL,
        [PaymentRemarks] NVARCHAR(MAX) NULL,
        [IsPostedToPms] BIT NOT NULL DEFAULT 0,
        [PmsPostedDate] DATETIME2 NULL,
        [PmsGroupId] NVARCHAR(100) NULL,
        [PmsGuestIds] NVARCHAR(500) NULL,
        [PmsResponse] NVARCHAR(MAX) NULL,
        [PmsError] NVARCHAR(MAX) NULL,
        [BookingCreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [BookingModifiedDate] DATETIME2 NULL,
        [BookingId] NVARCHAR(100) NULL,
        [Remarks] NVARCHAR(MAX) NULL,
        [Status] INT NULL,
        [SessionId] NVARCHAR(255) NULL,
        [UserId] INT NULL
    )
    
    -- Create indexes for better query performance
    CREATE NONCLUSTERED INDEX [IX_CRS_GuestBooking_BookingId] 
    ON [dbo].[CRS_GuestBooking] ([BookingId])
    
    CREATE NONCLUSTERED INDEX [IX_CRS_GuestBooking_SessionId] 
    ON [dbo].[CRS_GuestBooking] ([SessionId])
    
    CREATE NONCLUSTERED INDEX [IX_CRS_GuestBooking_BookingCreatedDate] 
    ON [dbo].[CRS_GuestBooking] ([BookingCreatedDate] DESC)
    
    PRINT 'Table CRS_GuestBooking created successfully.'
END
ELSE
BEGIN
    PRINT 'Table CRS_GuestBooking already exists.'
END
GO

-- =============================================
-- Table: CRS_VisitorsSessionLog
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CRS_VisitorsSessionLog]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CRS_VisitorsSessionLog] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [SessionId] NVARCHAR(255) NOT NULL,
        [IpAddress] NVARCHAR(50) NULL,
        [Browser] NVARCHAR(100) NULL,
        [BrowserVersion] NVARCHAR(50) NULL,
        [OperatingSystem] NVARCHAR(100) NULL,
        [DeviceType] NVARCHAR(50) NULL,
        [UserAgent] NVARCHAR(MAX) NULL,
        [Referrer] NVARCHAR(500) NULL,
        [Country] NVARCHAR(100) NULL,
        [Region] NVARCHAR(100) NULL,
        [City] NVARCHAR(100) NULL,
        [Latitude] DECIMAL(18,6) NULL,
        [Longitude] DECIMAL(18,6) NULL,
        [CreatedAtUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    )
    
    -- Create indexes for better query performance
    CREATE NONCLUSTERED INDEX [IX_CRS_VisitorsSessionLog_SessionId] 
    ON [dbo].[CRS_VisitorsSessionLog] ([SessionId])
    
    CREATE NONCLUSTERED INDEX [IX_CRS_VisitorsSessionLog_CreatedAtUtc] 
    ON [dbo].[CRS_VisitorsSessionLog] ([CreatedAtUtc] DESC)
    
    PRINT 'Table CRS_VisitorsSessionLog created successfully.'
END
ELSE
BEGIN
    PRINT 'Table CRS_VisitorsSessionLog already exists.'
END
GO

PRINT 'All tables created successfully!'
GO

