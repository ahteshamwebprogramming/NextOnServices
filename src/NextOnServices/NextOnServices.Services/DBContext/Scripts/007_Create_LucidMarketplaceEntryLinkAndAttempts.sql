Use NextOnServicesCore_Live
IF COL_LENGTH('dbo.LucidMarketplaceSetting', 'SupplierLinkTypeCode') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSetting]
        ADD [SupplierLinkTypeCode] [nvarchar](20) NULL;
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceSetting', 'TrackingTypeCode') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSetting]
        ADD [TrackingTypeCode] [nvarchar](20) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceEntryLink]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LucidMarketplaceEntryLink](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LucidMarketplaceOpportunityId] [int] NOT NULL,
        [LucidSurveyId] [int] NOT NULL,
        [SupplierCode] [nvarchar](100) NULL,
        [InternalProjectId] [int] NULL,
        [InternalProjectUrlId] [int] NULL,
        [InternalProjectMappingId] [int] NULL,
        [SupplierLinkTypeCode] [nvarchar](20) NULL,
        [TrackingTypeCode] [nvarchar](20) NULL,
        [DefaultLink] [nvarchar](max) NULL,
        [SuccessLink] [nvarchar](max) NULL,
        [FailureLink] [nvarchar](max) NULL,
        [OverQuotaLink] [nvarchar](max) NULL,
        [QualityTerminationLink] [nvarchar](max) NULL,
        [LiveLink] [nvarchar](max) NULL,
        [TestLink] [nvarchar](max) NULL,
        [SupplierLinkSid] [nvarchar](150) NULL,
        [RPIValue] [decimal](18,4) NULL,
        [RPICurrencyCode] [nvarchar](10) NULL,
        [RawJson] [nvarchar](max) NULL,
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceEntryLink_IsActive] DEFAULT((1)),
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_LucidMarketplaceEntryLink_CreatedDate] DEFAULT(getdate()),
        [ModifiedDate] [datetime] NULL,
        [CreatedBy] [int] NULL,
        [ModifiedBy] [int] NULL,
        CONSTRAINT [PK_LucidMarketplaceEntryLink] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_LucidMarketplaceEntryLink_OpportunityId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceEntryLink]'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [UX_LucidMarketplaceEntryLink_OpportunityId]
        ON [dbo].[LucidMarketplaceEntryLink] ([LucidMarketplaceOpportunityId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceEntryLink_SurveyId_SupplierCode' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceEntryLink]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceEntryLink_SurveyId_SupplierCode]
        ON [dbo].[LucidMarketplaceEntryLink] ([LucidSurveyId], [SupplierCode], [IsActive]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceEntryLink_InternalProjectId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceEntryLink]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceEntryLink_InternalProjectId]
        ON [dbo].[LucidMarketplaceEntryLink] ([InternalProjectId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceRespondentAttempt]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LucidMarketplaceRespondentAttempt](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LucidMarketplaceOpportunityId] [int] NOT NULL,
        [InternalProjectId] [int] NULL,
        [InternalProjectUrlId] [int] NULL,
        [InternalProjectMappingId] [int] NULL,
        [LucidSurveyId] [int] NOT NULL,
        [SupplierCode] [nvarchar](100) NULL,
        [RespondentId] [nvarchar](100) NULL,
        [SessionId] [nvarchar](100) NULL,
        [LaunchUrl] [nvarchar](max) NULL,
        [AttemptType] [nvarchar](20) NULL,
        [AttemptedOn] [datetime] NULL,
        [ReturnStatus] [nvarchar](50) NULL,
        [ReturnCode] [nvarchar](50) NULL,
        [ReturnRawQuery] [nvarchar](max) NULL,
        [IsCompleted] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceRespondentAttempt_IsCompleted] DEFAULT((0)),
        [IsTerminated] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceRespondentAttempt_IsTerminated] DEFAULT((0)),
        [IsOverQuota] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceRespondentAttempt_IsOverQuota] DEFAULT((0)),
        [IsQualityTermination] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceRespondentAttempt_IsQualityTermination] DEFAULT((0)),
        [RawJson] [nvarchar](max) NULL,
        [Notes] [nvarchar](max) NULL,
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_LucidMarketplaceRespondentAttempt_CreatedDate] DEFAULT(getdate()),
        [ModifiedDate] [datetime] NULL,
        CONSTRAINT [PK_LucidMarketplaceRespondentAttempt] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceRespondentAttempt_OpportunityId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceRespondentAttempt]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceRespondentAttempt_OpportunityId]
        ON [dbo].[LucidMarketplaceRespondentAttempt] ([LucidMarketplaceOpportunityId], [AttemptedOn]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceRespondentAttempt_ProjectMappingId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceRespondentAttempt]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceRespondentAttempt_ProjectMappingId]
        ON [dbo].[LucidMarketplaceRespondentAttempt] ([InternalProjectMappingId], [AttemptedOn]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceRespondentAttempt_SessionId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceRespondentAttempt]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceRespondentAttempt_SessionId]
        ON [dbo].[LucidMarketplaceRespondentAttempt] ([SessionId]);
END
GO
