IF COL_LENGTH(N'dbo.LucidMarketplaceSetting', N'EnableHashing') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSetting]
    ADD [EnableHashing] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceSetting_EnableHashing] DEFAULT ((0));
END
GO

IF COL_LENGTH(N'dbo.LucidMarketplaceSetting', N'HashingParameter') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSetting]
    ADD [HashingParameter] [nvarchar](50) NULL;
END
GO

IF COL_LENGTH(N'dbo.LucidMarketplaceSetting', N'HashingType') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSetting]
    ADD [HashingType] [nvarchar](50) NULL;
END
GO

IF COL_LENGTH(N'dbo.LucidMarketplaceSetting', N'RespondentIdUrlParts') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSetting]
    ADD [RespondentIdUrlParts] [nvarchar](2000) NULL;
END
GO
