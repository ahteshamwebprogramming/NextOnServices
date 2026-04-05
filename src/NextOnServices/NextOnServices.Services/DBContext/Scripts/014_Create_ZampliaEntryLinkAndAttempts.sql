Use NextOnServicesCore_Live
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZampliaEntryLink]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ZampliaEntryLink](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [ZampliaSurveyId] [int] NOT NULL,
        [SurveyId] [bigint] NOT NULL,
        [TransactionId] [nvarchar](150) NULL,
        [VendorLink] [nvarchar](max) NULL,
        [InternalLaunchUrl] [nvarchar](max) NULL,
        [HashApplied] [bit] NOT NULL CONSTRAINT [DF_ZampliaEntryLink_HashApplied] DEFAULT((0)),
        [RawJson] [nvarchar](max) NULL,
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_ZampliaEntryLink_IsActive] DEFAULT((1)),
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_ZampliaEntryLink_CreatedDate] DEFAULT(getdate()),
        [ModifiedDate] [datetime] NULL,
        [CreatedBy] [int] NULL,
        [ModifiedBy] [int] NULL,
        CONSTRAINT [PK_ZampliaEntryLink] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZampliaRespondentAttempt]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ZampliaRespondentAttempt](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [ZampliaSurveyId] [int] NOT NULL,
        [SurveyId] [bigint] NOT NULL,
        [InternalProjectId] [int] NULL,
        [InternalProjectUrlId] [int] NULL,
        [InternalProjectMappingId] [int] NULL,
        [RespondentId] [nvarchar](150) NULL,
        [TransactionId] [nvarchar](150) NULL,
        [SessionId] [nvarchar](150) NULL,
        [IpAddress] [nvarchar](100) NULL,
        [LaunchUrl] [nvarchar](max) NULL,
        [VendorLaunchUrl] [nvarchar](max) NULL,
        [ReturnUrl] [nvarchar](max) NULL,
        [ReturnRawQuery] [nvarchar](max) NULL,
        [ReturnCode] [nvarchar](100) NULL,
        [ReturnStatus] [nvarchar](100) NULL,
        [FinalStatus] [nvarchar](100) NULL,
        [FinalStatusSource] [nvarchar](50) NULL,
        [IsCompleted] [bit] NOT NULL CONSTRAINT [DF_ZampliaRespondentAttempt_IsCompleted] DEFAULT((0)),
        [IsTerminated] [bit] NOT NULL CONSTRAINT [DF_ZampliaRespondentAttempt_IsTerminated] DEFAULT((0)),
        [IsOverQuota] [bit] NOT NULL CONSTRAINT [DF_ZampliaRespondentAttempt_IsOverQuota] DEFAULT((0)),
        [IsQualityTermination] [bit] NOT NULL CONSTRAINT [DF_ZampliaRespondentAttempt_IsQualityTermination] DEFAULT((0)),
        [IsSecurityTermination] [bit] NOT NULL CONSTRAINT [DF_ZampliaRespondentAttempt_IsSecurityTermination] DEFAULT((0)),
        [IsDuplicate] [bit] NOT NULL CONSTRAINT [DF_ZampliaRespondentAttempt_IsDuplicate] DEFAULT((0)),
        [HmacReceived] [nvarchar](500) NULL,
        [HmacCalculated] [nvarchar](500) NULL,
        [HmacValid] [bit] NULL,
        [AttemptedOn] [datetime] NULL,
        [CompletedOn] [datetime] NULL,
        [RawJson] [nvarchar](max) NULL,
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_ZampliaRespondentAttempt_CreatedDate] DEFAULT(getdate()),
        [ModifiedDate] [datetime] NULL,
        [CreatedBy] [int] NULL,
        [ModifiedBy] [int] NULL,
        CONSTRAINT [PK_ZampliaRespondentAttempt] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO
