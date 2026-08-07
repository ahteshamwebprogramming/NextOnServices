IF COL_LENGTH(N'dbo.LucidMarketplaceSetting', N'DefaultSupplierIds') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSetting]
    ADD [DefaultSupplierIds] [nvarchar](1000) NULL;
END;
GO

IF COL_LENGTH(N'dbo.LucidMarketplaceSetting', N'SupplierCpiPercent') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSetting]
    ADD [SupplierCpiPercent] [decimal](9, 4) NULL;
END;
GO

-- Seed multi-supplier list from legacy single DefaultSupplierId where needed.
UPDATE s
SET s.[DefaultSupplierIds] = CAST(s.[DefaultSupplierId] AS nvarchar(20))
FROM [dbo].[LucidMarketplaceSetting] s
WHERE s.[DefaultSupplierId] IS NOT NULL
  AND s.[DefaultSupplierId] > 0
  AND NULLIF(LTRIM(RTRIM(ISNULL(s.[DefaultSupplierIds], N''))), N'') IS NULL;
GO
