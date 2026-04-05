Use NextOnServicesCore_Live
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceSetting]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LucidMarketplaceSetting](
        [LucidMarketplaceSettingId] [int] IDENTITY(1,1) NOT NULL,
        [BaseUrl] [nvarchar](500) NULL,
        [ApiKey] [nvarchar](500) NULL,
        [SupplierCode] [nvarchar](100) NULL,
        [OpportunitiesCallbackUrl] [nvarchar](500) NULL,
        [OutcomesCallbackUrl] [nvarchar](500) NULL,
        [UseConsultingsBridge] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceSetting_UseConsultingsBridge] DEFAULT((1)),
        [AutoSyncEnabled] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceSetting_AutoSyncEnabled] DEFAULT((0)),
        [SyncIntervalMinutes] [int] NULL,
        [DefaultClientId] [int] NULL,
        [DefaultCountryId] [int] NULL,
        [DefaultSupplierId] [int] NULL,
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceSetting_IsActive] DEFAULT((1)),
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_LucidMarketplaceSetting_CreatedDate] DEFAULT(getdate()),
        [ModifiedDate] [datetime] NULL,
        [CreatedBy] [int] NULL,
        [ModifiedBy] [int] NULL,
        CONSTRAINT [PK_LucidMarketplaceSetting] PRIMARY KEY CLUSTERED ([LucidMarketplaceSettingId] ASC)
    );

    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceSetting_IsActive]
        ON [dbo].[LucidMarketplaceSetting] ([IsActive]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceSubscription]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LucidMarketplaceSubscription](
        [LucidMarketplaceSubscriptionId] [int] IDENTITY(1,1) NOT NULL,
        [SubscriptionType] [nvarchar](50) NULL,
        [SupplierCode] [nvarchar](100) NULL,
        [CallbackUrl] [nvarchar](500) NULL,
        [IncludeQuotas] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceSubscription_IncludeQuotas] DEFAULT((0)),
        [RemoteSubscriptionId] [nvarchar](200) NULL,
        [LastStatus] [nvarchar](200) NULL,
        [RequestPayloadSnapshot] [nvarchar](max) NULL,
        [ResponsePayloadSnapshot] [nvarchar](max) NULL,
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceSubscription_IsActive] DEFAULT((0)),
        [LastValidatedOn] [datetime] NULL,
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_LucidMarketplaceSubscription_CreatedDate] DEFAULT(getdate()),
        [ModifiedDate] [datetime] NULL,
        [CreatedBy] [int] NULL,
        [ModifiedBy] [int] NULL,
        CONSTRAINT [PK_LucidMarketplaceSubscription] PRIMARY KEY CLUSTERED ([LucidMarketplaceSubscriptionId] ASC)
    );

    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceSubscription_SubscriptionType]
        ON [dbo].[LucidMarketplaceSubscription] ([SubscriptionType]);

    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceSubscription_IsActive]
        ON [dbo].[LucidMarketplaceSubscription] ([IsActive]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceSyncLog]') AND type in (N'U'))
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
        [IsSuccess] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceSyncLog_IsSuccess] DEFAULT((0)),
        [ErrorText] [nvarchar](max) NULL,
        [StartedOn] [datetime] NULL,
        [CompletedOn] [datetime] NULL,
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_LucidMarketplaceSyncLog_CreatedDate] DEFAULT(getdate()),
        [CreatedBy] [int] NULL,
        CONSTRAINT [PK_LucidMarketplaceSyncLog] PRIMARY KEY CLUSTERED ([LucidMarketplaceSyncLogId] ASC)
    );

    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceSyncLog_ActionName]
        ON [dbo].[LucidMarketplaceSyncLog] ([ActionName]);

    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceSyncLog_IsSuccess]
        ON [dbo].[LucidMarketplaceSyncLog] ([IsSuccess]);

    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceSyncLog_CreatedDate]
        ON [dbo].[LucidMarketplaceSyncLog] ([CreatedDate]);
END
GO
