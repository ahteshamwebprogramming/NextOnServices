/*
Run this only after the Lucid Marketplace Phase 1 minimum-storage code is deployed and tested.
Take a database backup first.
*/

IF OBJECT_ID('dbo.LucidMarketplaceSyncLog', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.LucidMarketplaceSyncLog;
END
GO

IF OBJECT_ID('dbo.LucidMarketplaceOpportunityQualification', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.LucidMarketplaceOpportunityQualification;
END
GO

IF OBJECT_ID('dbo.LucidMarketplaceOpportunityQuota', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.LucidMarketplaceOpportunityQuota;
END
GO

IF OBJECT_ID('dbo.LucidMarketplaceOpportunity', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.LucidMarketplaceOpportunity', 'RawJson') IS NOT NULL
    BEGIN
        UPDATE dbo.LucidMarketplaceOpportunity
        SET RawJson = NULL
        WHERE RawJson IS NOT NULL;
    END
END
GO

IF OBJECT_ID('dbo.LucidMarketplaceOpportunity', 'U') IS NOT NULL
BEGIN
    ;WITH d AS
    (
        SELECT
            LucidMarketplaceOpportunityId,
            rn = ROW_NUMBER() OVER
            (
                PARTITION BY SupplierCode, SurveyId
                ORDER BY ISNULL(LastSyncedOn, ISNULL(ModifiedDate, CreatedDate)) DESC,
                         LucidMarketplaceOpportunityId DESC
            )
        FROM dbo.LucidMarketplaceOpportunity
    )
    DELETE FROM d
    WHERE rn > 1;
END
GO

IF OBJECT_ID('dbo.LucidMarketplaceOpportunity', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.LucidMarketplaceOpportunity', 'SupplierCode') IS NOT NULL
   AND COL_LENGTH('dbo.LucidMarketplaceOpportunity', 'SurveyId') IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM sys.indexes
       WHERE name = 'UX_LucidMarketplaceOpportunity_SupplierCode_SurveyId'
         AND object_id = OBJECT_ID('dbo.LucidMarketplaceOpportunity')
   )
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_LucidMarketplaceOpportunity_SupplierCode_SurveyId
    ON dbo.LucidMarketplaceOpportunity(SupplierCode, SurveyId);
END
GO
