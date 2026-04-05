IF OBJECT_ID(N'[dbo].[HashingSetting]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[HashingSetting](
        [HashingSettingId] [int] IDENTITY(1,1) NOT NULL,
        [HashingType] [nvarchar](50) NOT NULL,
        [HashingKey] [nvarchar](500) NOT NULL,
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_HashingSetting_IsActive] DEFAULT((1)),
        [CreatedDate] [datetime] NULL CONSTRAINT [DF_HashingSetting_CreatedDate] DEFAULT(getdate()),
        [ModifiedDate] [datetime] NULL,
        [CreatedBy] [int] NULL,
        [ModifiedBy] [int] NULL,
        CONSTRAINT [PK_HashingSetting] PRIMARY KEY CLUSTERED ([HashingSettingId] ASC)
    );

    CREATE UNIQUE NONCLUSTERED INDEX [UX_HashingSetting_HashingType]
        ON [dbo].[HashingSetting] ([HashingType] ASC);
END;
