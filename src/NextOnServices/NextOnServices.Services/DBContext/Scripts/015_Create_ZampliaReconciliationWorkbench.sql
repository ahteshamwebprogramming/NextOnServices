Use NextOnServicesCore_Live
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZampliaReconciliationRun]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ZampliaReconciliationRun](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [RunType] [nvarchar](50) NULL,
        [SurveyId] [bigint] NULL,
        [InternalProjectId] [int] NULL,
        [TransactionId] [nvarchar](150) NULL,
        [RunScopeJson] [nvarchar](max) NULL,
        [StartedOn] [datetime] NULL,
        [CompletedOn] [datetime] NULL,
        [Success] [bit] NOT NULL CONSTRAINT [DF_ZampliaReconciliationRun_Success] DEFAULT((0)),
        [Notes] [nvarchar](1000) NULL,
        [TotalReviewed] [int] NOT NULL CONSTRAINT [DF_ZampliaReconciliationRun_TotalReviewed] DEFAULT((0)),
        [TotalMatched] [int] NOT NULL CONSTRAINT [DF_ZampliaReconciliationRun_TotalMatched] DEFAULT((0)),
        [TotalMismatched] [int] NOT NULL CONSTRAINT [DF_ZampliaReconciliationRun_TotalMismatched] DEFAULT((0)),
        [CompleteCount] [int] NOT NULL CONSTRAINT [DF_ZampliaReconciliationRun_CompleteCount] DEFAULT((0)),
        [TerminateCount] [int] NOT NULL CONSTRAINT [DF_ZampliaReconciliationRun_TerminateCount] DEFAULT((0)),
        [OverQuotaCount] [int] NOT NULL CONSTRAINT [DF_ZampliaReconciliationRun_OverQuotaCount] DEFAULT((0)),
        [QualityTerminationCount] [int] NOT NULL CONSTRAINT [DF_ZampliaReconciliationRun_QualityTerminationCount] DEFAULT((0)),
        [SecurityTerminationCount] [int] NOT NULL CONSTRAINT [DF_ZampliaReconciliationRun_SecurityTerminationCount] DEFAULT((0)),
        [OpenCount] [int] NOT NULL CONSTRAINT [DF_ZampliaReconciliationRun_OpenCount] DEFAULT((0)),
        [UnknownCount] [int] NOT NULL CONSTRAINT [DF_ZampliaReconciliationRun_UnknownCount] DEFAULT((0)),
        [CreatedBy] [int] NULL,
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_ZampliaReconciliationRun_CreatedDate] DEFAULT(getdate()),
        CONSTRAINT [PK_ZampliaReconciliationRun] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZampliaReconciliationItem]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ZampliaReconciliationItem](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [ReconciliationRunId] [int] NOT NULL,
        [ZampliaRespondentAttemptId] [int] NULL,
        [SurveyId] [bigint] NULL,
        [InternalProjectId] [int] NULL,
        [TransactionId] [nvarchar](150) NULL,
        [RespondentId] [nvarchar](150) NULL,
        [SessionId] [nvarchar](150) NULL,
        [LocalStatus] [nvarchar](100) NULL,
        [VendorStatus] [nvarchar](100) NULL,
        [FinalStatus] [nvarchar](100) NULL,
        [FinalStatusSource] [nvarchar](50) NULL,
        [IsMismatch] [bit] NOT NULL CONSTRAINT [DF_ZampliaReconciliationItem_IsMismatch] DEFAULT((0)),
        [MismatchType] [nvarchar](100) NULL,
        [Notes] [nvarchar](1000) NULL,
        [RawSnapshotJson] [nvarchar](max) NULL,
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_ZampliaReconciliationItem_CreatedDate] DEFAULT(getdate()),
        CONSTRAINT [PK_ZampliaReconciliationItem] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO
