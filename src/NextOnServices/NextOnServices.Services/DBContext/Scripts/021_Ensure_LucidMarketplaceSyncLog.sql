/*
Ensures LucidMarketplaceSyncLog exists for failure-only logging.
Safe to re-run. Does NOT drop existing data.
*/

IF OBJECT_ID('dbo.LucidMarketplaceSyncLog', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[LucidMarketplaceSyncLog](
        [LucidMarketplaceSyncLogId] [int] IDENTITY(1,1) NOT NULL,
        [ModuleName] [nvarchar](100) NULL,
        [ActionName] [nvarchar](100) NULL,
        [RequestUrl] [nvarchar](1000) NULL,
        [RequestBodySnapshot] [nvarchar](max) NULL,
        [ResponseStatusCode] [int] NULL,
        [ResponseBodySnapshot] [nvarchar](max) NULL,
        [Source] [nvarchar](50) NULL,
        [SupplierCode] [nvarchar](100) NULL,
        [IsSuccess] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceSyncLog_IsSuccess] DEFAULT((0)),
        [ErrorText] [nvarchar](max) NULL,
        [RelatedEntityId] [int] NULL,
        [RelatedSurveyId] [int] NULL,
        [StartedOn] [datetime] NULL,
        [CompletedOn] [datetime] NULL,
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_LucidMarketplaceSyncLog_CreatedDate] DEFAULT(getdate()),
        [CreatedBy] [int] NULL,
        CONSTRAINT [PK_LucidMarketplaceSyncLog] PRIMARY KEY CLUSTERED ([LucidMarketplaceSyncLogId] ASC)
    );
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceSyncLog', 'SupplierCode') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSyncLog]
        ADD [SupplierCode] [nvarchar](100) NULL;
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceSyncLog', 'RelatedEntityId') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSyncLog]
        ADD [RelatedEntityId] [int] NULL;
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceSyncLog', 'RelatedSurveyId') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSyncLog]
        ADD [RelatedSurveyId] [int] NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceSyncLog_ActionName' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceSyncLog]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceSyncLog_ActionName]
        ON [dbo].[LucidMarketplaceSyncLog] ([ActionName]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceSyncLog_IsSuccess' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceSyncLog]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceSyncLog_IsSuccess]
        ON [dbo].[LucidMarketplaceSyncLog] ([IsSuccess]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceSyncLog_CreatedDate' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceSyncLog]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceSyncLog_CreatedDate]
        ON [dbo].[LucidMarketplaceSyncLog] ([CreatedDate]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceSyncLog_SupplierCode' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceSyncLog]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceSyncLog_SupplierCode]
        ON [dbo].[LucidMarketplaceSyncLog] ([SupplierCode]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceSyncLog_RelatedSurveyId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceSyncLog]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceSyncLog_RelatedSurveyId]
        ON [dbo].[LucidMarketplaceSyncLog] ([RelatedSurveyId]);
END
GO
