/*
  Legacy AddCountry referenced dbo.tblcountries (renamed to CountryMaster).
  Deploy on databases where EXEC AddCountry fails with invalid object name 'tblcountries'.
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.AddCountry
    @item NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @name NVARCHAR(255) = LTRIM(RTRIM(@item));
    DECLARE @existingId INT;

    SELECT @existingId = CountryId
    FROM dbo.CountryMaster
    WHERE LTRIM(RTRIM(Country)) = @name;

    IF @existingId IS NOT NULL
    BEGIN
        SELECT @existingId AS CountryId;
        RETURN;
    END

    INSERT INTO dbo.CountryMaster (Country, ALPHA2, ALPHA3, CType)
    VALUES (@name, @name, @name, 1);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS CountryId;
END
GO
