/*
Lean LucidMarketplaceOpportunity: drop unused columns.
Safe / idempotent. Run after lean upsert code is deployed.
*/

IF COL_LENGTH('dbo.LucidMarketplaceOpportunity', 'AccountName') IS NOT NULL
    ALTER TABLE dbo.LucidMarketplaceOpportunity DROP COLUMN AccountName;
GO

IF COL_LENGTH('dbo.LucidMarketplaceOpportunity', 'BuyerId') IS NOT NULL
    ALTER TABLE dbo.LucidMarketplaceOpportunity DROP COLUMN BuyerId;
GO

IF COL_LENGTH('dbo.LucidMarketplaceOpportunity', 'TargetGroupId') IS NOT NULL
    ALTER TABLE dbo.LucidMarketplaceOpportunity DROP COLUMN TargetGroupId;
GO

IF COL_LENGTH('dbo.LucidMarketplaceOpportunity', 'RevenueCurrencyCode') IS NOT NULL
    ALTER TABLE dbo.LucidMarketplaceOpportunity DROP COLUMN RevenueCurrencyCode;
GO

IF COL_LENGTH('dbo.LucidMarketplaceOpportunity', 'RecontactCount') IS NOT NULL
    ALTER TABLE dbo.LucidMarketplaceOpportunity DROP COLUMN RecontactCount;
GO

IF COL_LENGTH('dbo.LucidMarketplaceOpportunity', 'SurveyGroupIdsJson') IS NOT NULL
    ALTER TABLE dbo.LucidMarketplaceOpportunity DROP COLUMN SurveyGroupIdsJson;
GO

IF COL_LENGTH('dbo.LucidMarketplaceOpportunity', 'LastVendorUpdatedOn') IS NOT NULL
    ALTER TABLE dbo.LucidMarketplaceOpportunity DROP COLUMN LastVendorUpdatedOn;
GO

IF COL_LENGTH('dbo.LucidMarketplaceOpportunity', 'RawJson') IS NOT NULL
    ALTER TABLE dbo.LucidMarketplaceOpportunity DROP COLUMN RawJson;
GO
