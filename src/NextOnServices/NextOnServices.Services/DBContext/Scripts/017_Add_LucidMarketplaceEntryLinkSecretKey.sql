IF COL_LENGTH(N'dbo.LucidMarketplaceSetting', N'EntryLinkSecretKey') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSetting]
    ADD [EntryLinkSecretKey] [nvarchar](500) NULL;
END;
