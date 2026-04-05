Use NextOnServicesCore_Live
IF COL_LENGTH('dbo.LucidMarketplaceRespondentAttempt', 'ParentSessionId') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceRespondentAttempt]
        ADD [ParentSessionId] [nvarchar](150) NULL;
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceRespondentAttempt', 'PanelistId') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceRespondentAttempt]
        ADD [PanelistId] [nvarchar](150) NULL;
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceRespondentAttempt', 'MarketplaceStatus') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceRespondentAttempt]
        ADD [MarketplaceStatus] [int] NULL;
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceRespondentAttempt', 'ClientStatus') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceRespondentAttempt]
        ADD [ClientStatus] [int] NULL;
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceRespondentAttempt', 'FinalStatus') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceRespondentAttempt]
        ADD [FinalStatus] [nvarchar](50) NULL;
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceRespondentAttempt', 'FinalStatusSource') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceRespondentAttempt]
        ADD [FinalStatusSource] [nvarchar](50) NULL;
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceRespondentAttempt', 'AsyncLastReceivedOn') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceRespondentAttempt]
        ADD [AsyncLastReceivedOn] [datetime] NULL;
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceRespondentAttempt', 'LatestRespondentOutcomeId') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceRespondentAttempt]
        ADD [LatestRespondentOutcomeId] [int] NULL;
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceRespondentAttempt', 'RevenueValue') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceRespondentAttempt]
        ADD [RevenueValue] [decimal](18,4) NULL;
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceRespondentAttempt', 'RevenueCurrencyCode') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceRespondentAttempt]
        ADD [RevenueCurrencyCode] [nvarchar](10) NULL;
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceRespondentAttempt', 'IsDuplicate') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceRespondentAttempt]
        ADD [IsDuplicate] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceRespondentAttempt_IsDuplicate] DEFAULT((0));
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceRespondentAttempt', 'IsSecurityTermination') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceRespondentAttempt]
        ADD [IsSecurityTermination] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceRespondentAttempt_IsSecurityTermination] DEFAULT((0));
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceRespondentAttempt_RespondentId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceRespondentAttempt]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceRespondentAttempt_RespondentId]
        ON [dbo].[LucidMarketplaceRespondentAttempt] ([RespondentId], [AttemptedOn]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceRespondentAttempt_PanelistId_SurveyId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceRespondentAttempt]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceRespondentAttempt_PanelistId_SurveyId]
        ON [dbo].[LucidMarketplaceRespondentAttempt] ([PanelistId], [LucidSurveyId], [AttemptedOn]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceRespondentOutcome]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LucidMarketplaceRespondentOutcome](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [SupplierCode] [nvarchar](100) NULL,
        [RespondentId] [nvarchar](150) NULL,
        [ParentSessionId] [nvarchar](150) NULL,
        [PanelistId] [nvarchar](150) NULL,
        [SessionId] [nvarchar](150) NULL,
        [MarketplaceStatus] [int] NULL,
        [ClientStatus] [int] NULL,
        [EntryDate] [datetime] NULL,
        [LastDate] [datetime] NULL,
        [SurveyId] [int] NULL,
        [RevenueValue] [decimal](18,4) NULL,
        [RevenueCurrencyCode] [nvarchar](10) NULL,
        [StudyType] [nvarchar](100) NULL,
        [BuyerId] [int] NULL,
        [ProofCostPerInterview] [decimal](18,4) NULL,
        [FinalStatus] [nvarchar](50) NULL,
        [RawJson] [nvarchar](max) NULL,
        [Source] [nvarchar](50) NULL,
        [ReceivedOn] [datetime] NULL CONSTRAINT [DF_LucidMarketplaceRespondentOutcome_ReceivedOn] DEFAULT(getdate()),
        [IsLatest] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceRespondentOutcome_IsLatest] DEFAULT((1)),
        [RelatedAttemptId] [int] NULL,
        [RelatedOpportunityId] [int] NULL,
        [RelatedInternalProjectId] [int] NULL,
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_LucidMarketplaceRespondentOutcome_CreatedDate] DEFAULT(getdate()),
        [ModifiedDate] [datetime] NULL,
        CONSTRAINT [PK_LucidMarketplaceRespondentOutcome] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceRespondentOutcome_SupplierCode_SessionId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceRespondentOutcome]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceRespondentOutcome_SupplierCode_SessionId]
        ON [dbo].[LucidMarketplaceRespondentOutcome] ([SupplierCode], [SessionId], [IsLatest], [ReceivedOn]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceRespondentOutcome_SupplierCode_RespondentId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceRespondentOutcome]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceRespondentOutcome_SupplierCode_RespondentId]
        ON [dbo].[LucidMarketplaceRespondentOutcome] ([SupplierCode], [RespondentId], [ReceivedOn]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceRespondentOutcome_SurveyId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceRespondentOutcome]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceRespondentOutcome_SurveyId]
        ON [dbo].[LucidMarketplaceRespondentOutcome] ([SurveyId], [ReceivedOn]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceRespondentOutcome_RelatedAttemptId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceRespondentOutcome]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceRespondentOutcome_RelatedAttemptId]
        ON [dbo].[LucidMarketplaceRespondentOutcome] ([RelatedAttemptId], [ReceivedOn]);
END
GO
