IF COL_LENGTH(N'dbo.TorfacMarketplaceSetting', N'DefaultClientId') IS NULL
BEGIN
    ALTER TABLE [dbo].[TorfacMarketplaceSetting]
        ADD [DefaultClientId] [int] NULL;
END;
