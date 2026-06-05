IF COL_LENGTH(N'dbo.TorfacMarketplaceSetting', N'RespondentIdUrlParts') IS NULL
BEGIN
    ALTER TABLE [dbo].[TorfacMarketplaceSetting]
        ADD [RespondentIdUrlParts] [nvarchar](2000) NULL;
END;

IF COL_LENGTH(N'dbo.TorfacMarketplaceSetting', N'RespondentPanelistIdUrlParts') IS NULL
BEGIN
    ALTER TABLE [dbo].[TorfacMarketplaceSetting]
        ADD [RespondentPanelistIdUrlParts] [nvarchar](2000) NULL;
END;
