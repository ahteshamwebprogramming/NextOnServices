Use NextOnServicesCore_Live
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ZampliaProjectMap]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ZampliaProjectMap](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [ZampliaSurveyId] [int] NOT NULL,
        [SurveyId] [bigint] NOT NULL,
        [InternalProjectId] [int] NULL,
        [InternalProjectUrlId] [int] NULL,
        [InternalProjectMappingId] [int] NULL,
        [AddedBy] [int] NULL,
        [AddedOn] [datetime] NULL CONSTRAINT [DF_ZampliaProjectMap_AddedOn] DEFAULT(getdate()),
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_ZampliaProjectMap_IsActive] DEFAULT((1)),
        [RawJson] [nvarchar](max) NULL,
        CONSTRAINT [PK_ZampliaProjectMap] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO
