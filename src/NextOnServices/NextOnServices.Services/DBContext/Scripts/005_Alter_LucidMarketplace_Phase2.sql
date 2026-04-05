Use NextOnServicesCore_Live
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

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceOpportunity]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LucidMarketplaceOpportunity](
        [LucidMarketplaceOpportunityId] [int] IDENTITY(1,1) NOT NULL,
        [SupplierCode] [nvarchar](100) NOT NULL,
        [SurveyId] [int] NOT NULL,
        [SurveyNumber] [nvarchar](100) NULL,
        [SurveyName] [nvarchar](500) NULL,
        [AccountName] [nvarchar](200) NULL,
        [BuyerId] [int] NULL,
        [TargetGroupId] [nvarchar](100) NULL,
        [CountryLanguageCode] [nvarchar](50) NULL,
        [StudyType] [nvarchar](100) NULL,
        [Industry] [nvarchar](100) NULL,
        [RevenuePerInterview] [decimal](18,4) NULL,
        [RevenueCurrencyCode] [nvarchar](10) NULL,
        [BidIncidence] [decimal](18,4) NULL,
        [BidLengthOfInterview] [decimal](18,4) NULL,
        [TotalRemaining] [int] NULL,
        [IsLive] [bit] NULL,
        [RecontactCount] [int] NULL,
        [SurveyGroupIdsJson] [nvarchar](max) NULL,
        [MessageReason] [nvarchar](100) NULL,
        [LastVendorUpdatedOn] [datetime] NULL,
        [LastSyncedOn] [datetime] NULL,
        [LocalState] [nvarchar](50) NULL,
        [RawJson] [nvarchar](max) NULL,
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceOpportunity_IsActive] DEFAULT((1)),
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_LucidMarketplaceOpportunity_CreatedDate] DEFAULT(getdate()),
        [ModifiedDate] [datetime] NULL,
        [CreatedBy] [int] NULL,
        [ModifiedBy] [int] NULL,
        CONSTRAINT [PK_LucidMarketplaceOpportunity] PRIMARY KEY CLUSTERED ([LucidMarketplaceOpportunityId] ASC),
        CONSTRAINT [UQ_LucidMarketplaceOpportunity_SupplierCode_SurveyId] UNIQUE NONCLUSTERED ([SupplierCode] ASC, [SurveyId] ASC)
    );

    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceOpportunity_SupplierCode_IsActive]
        ON [dbo].[LucidMarketplaceOpportunity] ([SupplierCode], [IsActive]);

    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceOpportunity_SurveyId]
        ON [dbo].[LucidMarketplaceOpportunity] ([SurveyId]);

    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceOpportunity_LastSyncedOn]
        ON [dbo].[LucidMarketplaceOpportunity] ([LastSyncedOn]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceOpportunityQualification]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LucidMarketplaceOpportunityQualification](
        [LucidMarketplaceOpportunityQualificationId] [int] IDENTITY(1,1) NOT NULL,
        [LucidMarketplaceOpportunityId] [int] NOT NULL,
        [SupplierCode] [nvarchar](100) NULL,
        [SurveyId] [int] NOT NULL,
        [QuestionId] [int] NULL,
        [QuestionText] [nvarchar](500) NULL,
        [LogicalOperator] [nvarchar](50) NULL,
        [PrecodesJson] [nvarchar](max) NULL,
        [RawJson] [nvarchar](max) NULL,
        [SortOrder] [int] NULL,
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceOpportunityQualification_IsActive] DEFAULT((1)),
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_LucidMarketplaceOpportunityQualification_CreatedDate] DEFAULT(getdate()),
        [ModifiedDate] [datetime] NULL,
        [CreatedBy] [int] NULL,
        [ModifiedBy] [int] NULL,
        CONSTRAINT [PK_LucidMarketplaceOpportunityQualification] PRIMARY KEY CLUSTERED ([LucidMarketplaceOpportunityQualificationId] ASC)
    );

    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceOpportunityQualification_OpportunityId]
        ON [dbo].[LucidMarketplaceOpportunityQualification] ([LucidMarketplaceOpportunityId]);

    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceOpportunityQualification_SurveyId]
        ON [dbo].[LucidMarketplaceOpportunityQualification] ([SurveyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceOpportunityQuota]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LucidMarketplaceOpportunityQuota](
        [LucidMarketplaceOpportunityQuotaId] [int] IDENTITY(1,1) NOT NULL,
        [LucidMarketplaceOpportunityId] [int] NOT NULL,
        [SupplierCode] [nvarchar](100) NULL,
        [SurveyId] [int] NOT NULL,
        [SurveyQuotaId] [int] NULL,
        [SurveyQuotaType] [nvarchar](100) NULL,
        [Conversion] [decimal](18,4) NULL,
        [NumberOfRespondents] [int] NULL,
        [QuestionsJson] [nvarchar](max) NULL,
        [RawJson] [nvarchar](max) NULL,
        [SortOrder] [int] NULL,
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceOpportunityQuota_IsActive] DEFAULT((1)),
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_LucidMarketplaceOpportunityQuota_CreatedDate] DEFAULT(getdate()),
        [ModifiedDate] [datetime] NULL,
        [CreatedBy] [int] NULL,
        [ModifiedBy] [int] NULL,
        CONSTRAINT [PK_LucidMarketplaceOpportunityQuota] PRIMARY KEY CLUSTERED ([LucidMarketplaceOpportunityQuotaId] ASC)
    );

    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceOpportunityQuota_OpportunityId]
        ON [dbo].[LucidMarketplaceOpportunityQuota] ([LucidMarketplaceOpportunityId]);

    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceOpportunityQuota_SurveyId]
        ON [dbo].[LucidMarketplaceOpportunityQuota] ([SurveyId]);
END
GO
