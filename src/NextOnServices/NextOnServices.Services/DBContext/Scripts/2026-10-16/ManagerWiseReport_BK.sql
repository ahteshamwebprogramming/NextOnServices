CREATE OR ALTER PROC [dbo].[ManagerWiseReport_BK] @countryid int=0,@supplierid int=0,@clientid int=0,@report int=0,@opt int=0,@id int=5,@startdate datetime ='2021-08-01',@enddate datetime= '2021-08-28'
AS BEGIN
if(@report=0)
BEGIN
if(@opt=0)
BEGIN
if(@countryid=0 and @supplierid=0 and @clientid=0)
begin
if(@startdate='' and @enddate= '')
BEGIN
DECLARE @ManagerTable AS TABLE (id int identity(1,1), Manager int , Total float , Complete float , CPI float , Revenue float )
DECLARE @output AS TABLE (id int IDENTITY(1,1),username varchar(50),total int,complete int,revenue float,percentagetotal float,percentrev float, percentcomple float)
DECLARE @sumTotal float,@sumComplete float, @sumRevenue float

INSERT INTO @ManagerTable
SELECT p.PManager
,(SELECT count(*) FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid WHERE pm.ProjectID=p.ProjectId)Total
,(SELECT count(*) FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid WHERE pm.ProjectID=p.ProjectId AND sp.Status='Complete')Complete
,(SELECT p2.CPI FROM dbo.Projects p2 WHERE p2.ProjectId=p.ProjectId) CPI
,((SELECT count(*) FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid WHERE pm.ProjectID=p.ProjectId AND sp.Status='Complete')*(SELECT p2.CPI FROM dbo.Projects p2 WHERE p2.ProjectId=p.ProjectId))Revenue
FROM dbo.Projects p 
SET @sumTotal = (SELECT sum(Total) FROM @ManagerTable mt)
SET @sumComplete = (SELECT sum(mt.Complete) FROM @ManagerTable mt)
SET @sumRevenue = (SELECT sum(mt.Revenue) FROM @ManagerTable mt)
INSERT INTO @output
SELECT (SELECT u.UserName FROM dbo.Users u WHERE u.UserId=mt.manager) username
,sum(mt.total) Total
,sum(mt.Complete)Complete
,sum(mt.Revenue)Revenue
,round(case when @sumtotal=0 THEN 0 ELSE (sum(mt.total)*100/@sumTotal) end,2)percentagetotal
,round(case when @sumComplete=0 THEN 0 ELSE (sum(mt.complete)*100/@sumComplete) end,2)percentagecomplete
,round(case when @sumRevenue=0 THEN 0 ELSE (sum(mt.Revenue)*100/@sumRevenue) end,2)percentagerevenue
FROM @ManagerTable mt GROUP BY mt.Manager
SELECT * FROM @output o

END
--no filter no date end
else
BEGIN
INSERT INTO @ManagerTable
SELECT p.PManager
,(SELECT count(*) FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid WHERE pm.ProjectID=p.ProjectId AND sp.StartDate >= @startdate AND sp.EndDate <= @enddate)Total
,(SELECT count(*) FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid WHERE pm.ProjectID=p.ProjectId AND sp.Status='Complete' AND sp.StartDate >= @startdate AND sp.EndDate <= @enddate)Complete
,(SELECT p2.CPI FROM dbo.Projects p2 WHERE p2.ProjectId=p.ProjectId) CPI
,((SELECT count(*) FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid WHERE pm.ProjectID=p.ProjectId AND sp.Status='Complete' AND sp.StartDate >= @startdate AND sp.EndDate <= @enddate)*(SELECT p2.CPI FROM dbo.Projects p2 WHERE p2.ProjectId=p.ProjectId))Revenue
FROM dbo.Projects p 
SET @sumTotal = (SELECT sum(Total) FROM @ManagerTable mt)
SET @sumComplete = (SELECT sum(mt.Complete) FROM @ManagerTable mt)
SET @sumRevenue = (SELECT sum(mt.Revenue) FROM @ManagerTable mt)
INSERT INTO @output
SELECT (SELECT u.UserName FROM dbo.Users u WHERE u.UserId=mt.manager) username
,sum(mt.total) Total
,sum(mt.Complete)Complete
,sum(mt.Revenue)Revenue
,round(case when @sumtotal=0 THEN 0 ELSE (sum(mt.total)*100/@sumTotal) end,2)percentagetotal
,round(case when @sumComplete=0 THEN 0 ELSE (sum(mt.complete)*100/@sumComplete) end,2)percentagecomplete
,round(case when @sumRevenue=0 THEN 0 ELSE (sum(mt.Revenue)*100/@sumRevenue) end,2)percentagerevenue
FROM @ManagerTable mt GROUP BY mt.Manager
SELECT * FROM @output o
END
--no filter with date end
END
--no filter end
END
END
END
