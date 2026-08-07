IF COL_LENGTH(N'dbo.LucidMarketplaceSetting', N'RespondentIdUrlReplaceWith') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSetting]
    ADD [RespondentIdUrlReplaceWith] [nvarchar](500) NULL;
END
GO
