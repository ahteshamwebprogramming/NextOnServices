-- Add Mode column to tblIPMapping table to store Include/Exclude mode
-- This allows us to store only selected countries instead of mapping all countries

-- Check if column already exists, if not add it
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[tblIPMapping]') 
    AND name = 'MappingMode'
)
BEGIN
    ALTER TABLE [dbo].[tblIPMapping]
    ADD [MappingMode] VARCHAR(10) NULL;
    
    -- Set default value for existing records
    UPDATE [dbo].[tblIPMapping]
    SET [MappingMode] = CASE 
        WHEN [stat] = 1 THEN 'Exclude'
        ELSE 'Include'
    END
    WHERE [MappingMode] IS NULL;
    
    -- Set default constraint for new records
    ALTER TABLE [dbo].[tblIPMapping]
    ADD CONSTRAINT DF_tblIPMapping_MappingMode DEFAULT 'Include' FOR [MappingMode];
    
    PRINT 'MappingMode column added successfully to tblIPMapping table.';
END
ELSE
BEGIN
    PRINT 'MappingMode column already exists in tblIPMapping table.';
END
GO

