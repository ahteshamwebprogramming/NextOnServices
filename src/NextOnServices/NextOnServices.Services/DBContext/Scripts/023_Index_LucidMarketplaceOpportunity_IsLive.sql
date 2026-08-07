/*
Index to speed Not Live opportunity cleanup filters.
*/

IF OBJECT_ID('dbo.LucidMarketplaceOpportunity', 'U') IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM sys.indexes
       WHERE name = 'IX_LucidMarketplaceOpportunity_IsLive'
         AND object_id = OBJECT_ID('dbo.LucidMarketplaceOpportunity')
   )
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceOpportunity_IsLive]
        ON [dbo].[LucidMarketplaceOpportunity] ([IsLive])
        INCLUDE ([LucidMarketplaceOpportunityId], [SupplierCode], [SurveyId]);
END
GO
