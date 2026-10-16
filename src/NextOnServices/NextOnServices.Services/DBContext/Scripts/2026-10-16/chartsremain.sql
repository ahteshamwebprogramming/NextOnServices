CREATE OR ALTER PROC [dbo].[chartsremain]
    @countryid INT = 0,
    @supplierid INT = 0,
    @clientid INT = 0,
    @startdate DATETIME = NULL,
    @enddate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF (@startdate IS NULL OR @enddate IS NULL)
    BEGIN
        SET @enddate = GETDATE();
        SET @startdate = DATEADD(MONTH, -2, DATEFROMPARTS(YEAR(@enddate), MONTH(@enddate), 1));
    END

    DECLARE @rangeStart DATETIME = @startdate;
    DECLARE @rangeEnd DATETIME = @enddate;

    DECLARE @bartable AS TABLE (id INT IDENTITY(1,1), Months VARCHAR(20), Completes INT);
    DECLARE @cursor DATE = DATEFROMPARTS(YEAR(@rangeStart), MONTH(@rangeStart), 1);
    DECLARE @lastMonth DATE = DATEFROMPARTS(YEAR(@rangeEnd), MONTH(@rangeEnd), 1);

    WHILE @cursor <= @lastMonth
    BEGIN
        DECLARE @m INT = MONTH(@cursor);
        DECLARE @y INT = YEAR(@cursor);
        DECLARE @label VARCHAR(20) = LEFT(DATENAME(MONTH, @cursor), 3);

        INSERT INTO @bartable (Months, Completes)
        SELECT @label,
               COUNT(*)
        FROM dbo.SupplierProjects sp
        LEFT JOIN dbo.ProjectMapping pm ON sp.SID = pm.SID
        LEFT JOIN dbo.Projects p ON pm.ProjectID = p.ProjectId
        WHERE sp.Status = 'Complete'
          AND sp.StartDate IS NOT NULL
          AND sp.EndDate IS NOT NULL
          AND sp.StartDate >= @rangeStart
          AND sp.EndDate <= @rangeEnd
          AND MONTH(sp.EndDate) = @m
          AND YEAR(sp.EndDate) = @y
          AND (@countryid = 0 OR pm.CountryID = @countryid)
          AND (@supplierid = 0 OR pm.SUpplierID = @supplierid)
          AND (@clientid = 0 OR p.ClientID = @clientid);

        SET @cursor = DATEADD(MONTH, 1, @cursor);
    END

    DECLARE @completedata AS TABLE (id INT IDENTITY(1,1), CPI INT);

    INSERT INTO @completedata (CPI)
    SELECT p.CPI
    FROM dbo.Projects p
    WHERE p.Status = 1
      AND TRY_CONVERT(DATETIME, p.CreationDate) IS NOT NULL
      AND TRY_CONVERT(DATETIME, p.CreationDate) >= @rangeStart
      AND TRY_CONVERT(DATETIME, p.CreationDate) <= @rangeEnd
      AND (@countryid = 0 OR p.CountryId = @countryid)
      AND (@clientid = 0 OR p.ClientID = @clientid);

    SELECT * FROM @bartable;

    DECLARE @pietable AS TABLE (id INT IDENTITY(1,1), Label VARCHAR(20), Minn INT, Maxx INT, Mean INT);

    DECLARE @min INT = (SELECT CASE WHEN MIN(CPI) IS NULL THEN 0 ELSE MIN(CPI) END FROM @completedata);
    DECLARE @max INT = (SELECT CASE WHEN MAX(CPI) IS NULL THEN 0 ELSE MAX(CPI) END FROM @completedata);
    DECLARE @avg INT = (SELECT CASE WHEN AVG(CPI) IS NULL THEN 0 ELSE AVG(CPI) END FROM @completedata);
    DECLARE @total INT = (@min + @max + @avg);

    INSERT INTO @pietable (Label, Minn, Maxx, Mean)
    VALUES (
        'Percent',
        CASE WHEN @total = 0 THEN 0 ELSE (@min * 100 / @total) END,
        CASE WHEN @total = 0 THEN 0 ELSE (@max * 100 / @total) END,
        CASE WHEN @total = 0 THEN 0 ELSE (@avg * 100 / @total) END
    );

    INSERT INTO @pietable (Label, Minn, Maxx, Mean)
    VALUES ('CPI', @min, @max, @avg);

    SELECT * FROM @pietable;
END
GO
