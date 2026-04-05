Use NextOnServicesCore_Live
IF COL_LENGTH('dbo.LucidMarketplaceSubscription', 'WebhookKeyId') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSubscription]
        ADD [WebhookKeyId] [nvarchar](100) NULL;
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceSubscription', 'WebhookKeyIdFull') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSubscription]
        ADD [WebhookKeyIdFull] [nvarchar](200) NULL;
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceSubscription', 'WebhookPublicKey') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSubscription]
        ADD [WebhookPublicKey] [nvarchar](max) NULL;
END
GO

IF COL_LENGTH('dbo.LucidMarketplaceSubscription', 'WebhookSecuritySnapshot') IS NULL
BEGIN
    ALTER TABLE [dbo].[LucidMarketplaceSubscription]
        ADD [WebhookSecuritySnapshot] [nvarchar](max) NULL;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_LucidMarketplaceSubscription_TypeSupplierActive'
      AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceSubscription]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceSubscription_TypeSupplierActive]
        ON [dbo].[LucidMarketplaceSubscription] ([SubscriptionType], [SupplierCode], [IsActive])
        INCLUDE ([WebhookKeyId], [LastValidatedOn], [ModifiedDate], [CreatedDate]);
END
GO
