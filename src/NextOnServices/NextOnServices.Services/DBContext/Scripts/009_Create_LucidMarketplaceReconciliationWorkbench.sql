Use NextOnServicesCore_Live
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceReconciliationRun]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LucidMarketplaceReconciliationRun](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [RunType] [nvarchar](50) NULL,
        [SupplierCode] [nvarchar](100) NULL,
        [LucidSurveyId] [int] NULL,
        [InternalProjectId] [int] NULL,
        [RunScopeJson] [nvarchar](max) NULL,
        [StartedOn] [datetime] NULL,
        [CompletedOn] [datetime] NULL,
        [Success] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceReconciliationRun_Success] DEFAULT((0)),
        [Notes] [nvarchar](1000) NULL,
        [TotalReviewed] [int] NOT NULL CONSTRAINT [DF_LucidMarketplaceReconciliationRun_TotalReviewed] DEFAULT((0)),
        [TotalMatched] [int] NOT NULL CONSTRAINT [DF_LucidMarketplaceReconciliationRun_TotalMatched] DEFAULT((0)),
        [TotalMismatched] [int] NOT NULL CONSTRAINT [DF_LucidMarketplaceReconciliationRun_TotalMismatched] DEFAULT((0)),
        [CompleteCount] [int] NOT NULL CONSTRAINT [DF_LucidMarketplaceReconciliationRun_CompleteCount] DEFAULT((0)),
        [TerminateCount] [int] NOT NULL CONSTRAINT [DF_LucidMarketplaceReconciliationRun_TerminateCount] DEFAULT((0)),
        [OverQuotaCount] [int] NOT NULL CONSTRAINT [DF_LucidMarketplaceReconciliationRun_OverQuotaCount] DEFAULT((0)),
        [QualityTerminationCount] [int] NOT NULL CONSTRAINT [DF_LucidMarketplaceReconciliationRun_QualityTerminationCount] DEFAULT((0)),
        [DuplicateCount] [int] NOT NULL CONSTRAINT [DF_LucidMarketplaceReconciliationRun_DuplicateCount] DEFAULT((0)),
        [SecurityTerminationCount] [int] NOT NULL CONSTRAINT [DF_LucidMarketplaceReconciliationRun_SecurityTerminationCount] DEFAULT((0)),
        [OpenCount] [int] NOT NULL CONSTRAINT [DF_LucidMarketplaceReconciliationRun_OpenCount] DEFAULT((0)),
        [UnknownCount] [int] NOT NULL CONSTRAINT [DF_LucidMarketplaceReconciliationRun_UnknownCount] DEFAULT((0)),
        [CreatedBy] [int] NULL,
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_LucidMarketplaceReconciliationRun_CreatedDate] DEFAULT(getdate()),
        CONSTRAINT [PK_LucidMarketplaceReconciliationRun] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceReconciliationItem]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LucidMarketplaceReconciliationItem](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [ReconciliationRunId] [int] NOT NULL,
        [LucidMarketplaceRespondentAttemptId] [int] NULL,
        [LucidMarketplaceRespondentOutcomeId] [int] NULL,
        [LucidSurveyId] [int] NULL,
        [InternalProjectId] [int] NULL,
        [SessionId] [nvarchar](150) NULL,
        [RespondentId] [nvarchar](150) NULL,
        [PanelistId] [nvarchar](150) NULL,
        [RedirectStatus] [nvarchar](50) NULL,
        [RedirectCode] [nvarchar](50) NULL,
        [OutcomeMarketplaceStatus] [int] NULL,
        [OutcomeClientStatus] [int] NULL,
        [FinalStatus] [nvarchar](50) NULL,
        [FinalStatusSource] [nvarchar](50) NULL,
        [IsMismatch] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceReconciliationItem_IsMismatch] DEFAULT((0)),
        [MismatchType] [nvarchar](100) NULL,
        [RevenueValue] [decimal](18,4) NULL,
        [RevenueCurrencyCode] [nvarchar](10) NULL,
        [Notes] [nvarchar](1000) NULL,
        [RawSnapshotJson] [nvarchar](max) NULL,
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_LucidMarketplaceReconciliationItem_CreatedDate] DEFAULT(getdate()),
        CONSTRAINT [PK_LucidMarketplaceReconciliationItem] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceReconciliationRun_Scope' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceReconciliationRun]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceReconciliationRun_Scope]
        ON [dbo].[LucidMarketplaceReconciliationRun] ([SupplierCode], [LucidSurveyId], [InternalProjectId], [StartedOn]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceReconciliationRun_CompletedOn' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceReconciliationRun]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceReconciliationRun_CompletedOn]
        ON [dbo].[LucidMarketplaceReconciliationRun] ([CompletedOn], [Success]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceReconciliationItem_RunId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceReconciliationItem]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceReconciliationItem_RunId]
        ON [dbo].[LucidMarketplaceReconciliationItem] ([ReconciliationRunId], [IsMismatch], [FinalStatus]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceReconciliationItem_SessionId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceReconciliationItem]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceReconciliationItem_SessionId]
        ON [dbo].[LucidMarketplaceReconciliationItem] ([SessionId], [LucidSurveyId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceReconciliationItem_RespondentId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceReconciliationItem]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceReconciliationItem_RespondentId]
        ON [dbo].[LucidMarketplaceReconciliationItem] ([RespondentId], [LucidSurveyId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceReconciliationItem_Survey_Project' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceReconciliationItem]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceReconciliationItem_Survey_Project]
        ON [dbo].[LucidMarketplaceReconciliationItem] ([LucidSurveyId], [InternalProjectId], [FinalStatus]);
END
GO
