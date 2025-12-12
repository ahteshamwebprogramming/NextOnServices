-- SQL Script to create tblCountryExclusion table for Country Exclusion feature
-- This table stores countries that should be excluded from survey responses

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblCountryExclusion]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[tblCountryExclusion](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [prourlid] [int] NULL,
        [countryid] [int] NULL,
        [stat] [int] NULL,
        [isactive] [int] NULL DEFAULT(1),
        CONSTRAINT [PK_tblCountryExclusion] PRIMARY KEY CLUSTERED ([Id] ASC)
    ) ON [PRIMARY]
    
    -- Add foreign key constraint to CountryMaster if needed
    -- ALTER TABLE [dbo].[tblCountryExclusion] 
    -- ADD CONSTRAINT [FK_tblCountryExclusion_CountryMaster] 
    -- FOREIGN KEY([countryid]) REFERENCES [dbo].[CountryMaster] ([CountryId])
    
    -- Add index for better query performance
    CREATE NONCLUSTERED INDEX [IX_tblCountryExclusion_prourlid] 
    ON [dbo].[tblCountryExclusion] ([prourlid])
    
    CREATE NONCLUSTERED INDEX [IX_tblCountryExclusion_countryid] 
    ON [dbo].[tblCountryExclusion] ([countryid])
    
    CREATE NONCLUSTERED INDEX [IX_tblCountryExclusion_isactive] 
    ON [dbo].[tblCountryExclusion] ([isactive])
    
    PRINT 'Table tblCountryExclusion created successfully'
END
ELSE
BEGIN
    PRINT 'Table tblCountryExclusion already exists'
END
GO

