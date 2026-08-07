IF COL_LENGTH(N'dbo.HashingSetting', N'HashingType') IS NOT NULL
BEGIN
    UPDATE [dbo].[HashingSetting]
    SET [HashingType] = N'HMACSHA256_Zamplia'
    WHERE UPPER(LTRIM(RTRIM([HashingType]))) = N'HMACSHA256';
END
GO

IF COL_LENGTH(N'dbo.ProjectMapping', N'HashingType') IS NOT NULL
BEGIN
    UPDATE [dbo].[ProjectMapping]
    SET [HashingType] = N'HMACSHA256_Zamplia'
    WHERE UPPER(LTRIM(RTRIM([HashingType]))) = N'HMACSHA256';
END
GO
