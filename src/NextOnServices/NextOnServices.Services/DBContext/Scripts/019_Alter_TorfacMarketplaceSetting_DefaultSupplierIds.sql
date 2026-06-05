IF COL_LENGTH(N'dbo.TorfacMarketplaceSetting', N'DefaultSupplierIds') IS NULL
BEGIN
    ALTER TABLE [dbo].[TorfacMarketplaceSetting]
    ADD [DefaultSupplierIds] [nvarchar](1000) NULL;
END;
