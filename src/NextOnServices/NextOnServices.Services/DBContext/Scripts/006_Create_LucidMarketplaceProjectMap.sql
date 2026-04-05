Use NextOnServicesCore_Live
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceProjectMap]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LucidMarketplaceProjectMap](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [LucidMarketplaceOpportunityId] [int] NOT NULL,
        [LucidSurveyId] [int] NOT NULL,
        [InternalProjectId] [int] NULL,
        [InternalProjectUrlId] [int] NULL,
        [InternalProjectMappingId] [int] NULL,
        [SupplierCode] [nvarchar](100) NULL,
        [AddedBy] [int] NULL,
        [AddedOn] [datetime] NULL CONSTRAINT [DF_LucidMarketplaceProjectMap_AddedOn] DEFAULT(getdate()),
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_LucidMarketplaceProjectMap_IsActive] DEFAULT((1)),
        [RawJson] [nvarchar](max) NULL,
        CONSTRAINT [PK_LucidMarketplaceProjectMap] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_LucidMarketplaceProjectMap_OpportunityId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceProjectMap]'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [UX_LucidMarketplaceProjectMap_OpportunityId]
        ON [dbo].[LucidMarketplaceProjectMap] ([LucidMarketplaceOpportunityId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceProjectMap_SurveyId_SupplierCode' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceProjectMap]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceProjectMap_SurveyId_SupplierCode]
        ON [dbo].[LucidMarketplaceProjectMap] ([LucidSurveyId], [SupplierCode], [IsActive]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LucidMarketplaceProjectMap_InternalProjectId' AND object_id = OBJECT_ID(N'[dbo].[LucidMarketplaceProjectMap]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_LucidMarketplaceProjectMap_InternalProjectId]
        ON [dbo].[LucidMarketplaceProjectMap] ([InternalProjectId]);
END
GO
