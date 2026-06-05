IF OBJECT_ID(N'[dbo].[TorfacMarketplaceSetting]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TorfacMarketplaceSetting](
        [TorfacMarketplaceSettingId] [int] IDENTITY(1,1) NOT NULL,
        [SurveysUrl] [nvarchar](1000) NULL,
        [SecretKey] [nvarchar](500) NULL,
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_TorfacMarketplaceSetting_IsActive] DEFAULT((1)),
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_TorfacMarketplaceSetting_CreatedDate] DEFAULT(getdate()),
        [ModifiedDate] [datetime] NULL,
        [CreatedBy] [int] NULL,
        [ModifiedBy] [int] NULL,
        CONSTRAINT [PK_TorfacMarketplaceSetting] PRIMARY KEY CLUSTERED ([TorfacMarketplaceSettingId] ASC)
    );
END;
