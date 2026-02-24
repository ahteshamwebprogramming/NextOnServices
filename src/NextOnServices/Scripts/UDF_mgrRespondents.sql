-- =============================================
-- Table-Valued Function: UDF_mgrRespondents
-- Replaces stored procedure USP_mgrRespondents for ID Search.
-- Usage: SELECT * FROM dbo.UDF_mgrRespondents(@id, @type)
--   @type = '1' => search by UID (Unique Id)
--   @type = '2' => search by PMID (Supplier Id)
-- =============================================
USE [NextOnServicesCore_Live]
GO

IF OBJECT_ID(N'dbo.UDF_mgrRespondents', N'IF') IS NOT NULL
    DROP FUNCTION dbo.UDF_mgrRespondents;
GO

CREATE FUNCTION dbo.UDF_mgrRespondents
(
    @id   varchar(200) = '',
    @type varchar(200) = ''
)
RETURNS TABLE
AS
RETURN
(
    -- type='1': search by UID (Unique Id), join CountryMaster
    SELECT
        s.Name       AS [Supplier Name],
        pm.SID       AS SID,
        sp.UID       AS UID,
        c.Country    AS Country,
        sp.PMID      AS [Supplier ID],
        sp.Status    AS [Status],
        sp.StartDate AS StartDate,
        sp.EndDate   AS EndDate,
        DATEDIFF(MI, sp.StartDate, sp.EndDate) AS Duration,
        sp.ClientBrowser,
        sp.ClientIP,
        sp.Device
    FROM ProjectMapping pm
    LEFT JOIN Suppliers s ON pm.SupplierID = s.ID
    RIGHT JOIN SupplierProjects sp ON pm.SID = sp.SID
    LEFT JOIN CountryMaster c ON pm.CountryID = c.CountryId
    WHERE @id <> ''
      AND @type = '1'
      AND sp.UID LIKE '%' + @id + '%'

    UNION ALL

    -- type='2': search by PMID (Supplier Id), join TblCountries
    SELECT
        s.Name       AS [Supplier Name],
        pm.SID       AS SID,
        sp.UID       AS UID,
        c.Country    AS Country,
        sp.PMID      AS [Supplier ID],
        sp.Status    AS [Status],
        sp.StartDate AS StartDate,
        sp.EndDate   AS EndDate,
        DATEDIFF(MI, sp.StartDate, sp.EndDate) AS Duration,
        sp.ClientBrowser,
        sp.ClientIP,
        sp.Device
    FROM ProjectMapping pm
    LEFT JOIN Suppliers s ON pm.SupplierID = s.ID
    RIGHT JOIN SupplierProjects sp ON pm.SID = sp.SID
    LEFT JOIN TblCountries c ON pm.CountryID = c.Id
    WHERE @id <> ''
      AND @type = '2'
      AND sp.PMID LIKE '%' + @id + '%'
);
GO
