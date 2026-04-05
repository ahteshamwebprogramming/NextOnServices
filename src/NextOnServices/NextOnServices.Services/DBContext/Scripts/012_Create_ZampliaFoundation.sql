Use NextOnServicesCore_Live
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZampliaSetting]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ZampliaSetting](
        [ZampliaSettingId] [int] IDENTITY(1,1) NOT NULL,
        [BaseUrl] [nvarchar](500) NULL,
        [ApiKey] [nvarchar](500) NULL,
        [ExitHmacKey] [nvarchar](500) NULL,
        [UseConsultingsBridge] [bit] NOT NULL CONSTRAINT [DF_ZampliaSetting_UseConsultingsBridge] DEFAULT((1)),
        [AutoSyncEnabled] [bit] NOT NULL CONSTRAINT [DF_ZampliaSetting_AutoSyncEnabled] DEFAULT((0)),
        [SyncIntervalMinutes] [int] NULL,
        [DefaultClientId] [int] NULL,
        [DefaultCountryId] [int] NULL,
        [DefaultSupplierId] [int] NULL,
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_ZampliaSetting_IsActive] DEFAULT((1)),
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_ZampliaSetting_CreatedDate] DEFAULT(getdate()),
        [ModifiedDate] [datetime] NULL,
        [CreatedBy] [int] NULL,
        [ModifiedBy] [int] NULL,
        CONSTRAINT [PK_ZampliaSetting] PRIMARY KEY CLUSTERED ([ZampliaSettingId] ASC)
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZampliaSyncLog]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ZampliaSyncLog](
        [ZampliaSyncLogId] [int] IDENTITY(1,1) NOT NULL,
        [ModuleName] [nvarchar](100) NULL,
        [ActionName] [nvarchar](100) NULL,
        [RequestUrl] [nvarchar](1000) NULL,
        [RequestBodySnapshot] [nvarchar](max) NULL,
        [ResponseStatusCode] [int] NULL,
        [ResponseBodySnapshot] [nvarchar](max) NULL,
        [Source] [nvarchar](50) NULL,
        [RelatedEntityId] [int] NULL,
        [RelatedSurveyId] [bigint] NULL,
        [IsSuccess] [bit] NOT NULL CONSTRAINT [DF_ZampliaSyncLog_IsSuccess] DEFAULT((0)),
        [ErrorText] [nvarchar](max) NULL,
        [StartedOn] [datetime] NULL,
        [CompletedOn] [datetime] NULL,
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_ZampliaSyncLog_CreatedDate] DEFAULT(getdate()),
        [CreatedBy] [int] NULL,
        CONSTRAINT [PK_ZampliaSyncLog] PRIMARY KEY CLUSTERED ([ZampliaSyncLogId] ASC)
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZampliaSurvey]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ZampliaSurvey](
        [ZampliaSurveyId] [int] IDENTITY(1,1) NOT NULL,
        [SurveyId] [bigint] NOT NULL,
        [SurveyName] [nvarchar](500) NULL,
        [CPI] [decimal](18,4) NULL,
        [LOI] [int] NULL,
        [IR] [decimal](18,4) NULL,
        [LanguageCode] [nvarchar](50) NULL,
        [LanguageId] [int] NULL,
        [SurveyEndDate] [datetime] NULL,
        [Device] [nvarchar](50) NULL,
        [IndustryId] [nvarchar](100) NULL,
        [StudyTypes] [nvarchar](200) NULL,
        [IsRecontactSurvey] [bit] NULL,
        [CollectPII] [bit] NULL,
        [Conversion] [decimal](18,4) NULL,
        [TotalCompleteRequired] [int] NULL,
        [LastVendorUpdatedOn] [datetime] NULL,
        [LastSyncedOn] [datetime] NULL,
        [LocalState] [nvarchar](50) NULL,
        [RawJson] [nvarchar](max) NULL,
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_ZampliaSurvey_IsActive] DEFAULT((1)),
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_ZampliaSurvey_CreatedDate] DEFAULT(getdate()),
        [ModifiedDate] [datetime] NULL,
        [CreatedBy] [int] NULL,
        [ModifiedBy] [int] NULL,
        CONSTRAINT [PK_ZampliaSurvey] PRIMARY KEY CLUSTERED ([ZampliaSurveyId] ASC)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_ZampliaSurvey_SurveyId' AND object_id = OBJECT_ID(N'[dbo].[ZampliaSurvey]'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [UX_ZampliaSurvey_SurveyId]
        ON [dbo].[ZampliaSurvey] ([SurveyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZampliaSurveyQualification]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ZampliaSurveyQualification](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [ZampliaSurveyId] [int] NOT NULL,
        [SurveyId] [bigint] NOT NULL,
        [QuestionId] [int] NULL,
        [QuestionText] [nvarchar](500) NULL,
        [QuestionType] [nvarchar](100) NULL,
        [LogicalOperator] [nvarchar](50) NULL,
        [AnswerCodesJson] [nvarchar](max) NULL,
        [RawJson] [nvarchar](max) NULL,
        [SortOrder] [int] NULL,
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_ZampliaSurveyQualification_IsActive] DEFAULT((1)),
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_ZampliaSurveyQualification_CreatedDate] DEFAULT(getdate()),
        CONSTRAINT [PK_ZampliaSurveyQualification] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZampliaSurveyQuota]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ZampliaSurveyQuota](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [ZampliaSurveyId] [int] NOT NULL,
        [SurveyId] [bigint] NOT NULL,
        [QuotaId] [bigint] NULL,
        [QuotaName] [nvarchar](500) NULL,
        [NumberOfRespondents] [int] NULL,
        [Conversion] [decimal](18,4) NULL,
        [QuestionsJson] [nvarchar](max) NULL,
        [RawJson] [nvarchar](max) NULL,
        [SortOrder] [int] NULL,
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_ZampliaSurveyQuota_IsActive] DEFAULT((1)),
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_ZampliaSurveyQuota_CreatedDate] DEFAULT(getdate()),
        CONSTRAINT [PK_ZampliaSurveyQuota] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO
