CREATE proc [dbo].[usp_CalculateRevenue_BK]
@countryid int=0,
@supplierid int=0,
@clientid int=0,
@startdate datetime='2021-08-01',
@enddate datetime='2021-08-28'
--@startdate datetime='2017-11-14',
--@enddate datetime='2017-11-17'
as begin
--with out filter
declare @output_s as table (id int identity(1,1) , parameter varchar(100), revenue float, percentt float)
DECLARE @SupplierTable AS TABLE (id int identity(1,1) , PID int , SID int , CID int , Complete float,CPI float, Cost float )
DECLARE @ClientTable AS TABLE (id int identity(1,1) , PID int , CID int , Complete float , CPI float, Cost float )
DECLARE @CountryTable AS TABLE (id int IDENTITY(1,1) , PID int , CID int , Complete float, CPI float , Value float )
declare @output_cl as table (id int identity(1,1) , parameter varchar(100), revenue float, percentt decimal(5,0))
declare @output_co as table (id int identity(1,1) , parameter varchar(100), revenue float, percentt decimal(5,0))
declare @totalrevenue float, @min int, @max int, @revenue_s float
--without filter
if(@countryid=0 and @supplierid=0 and @clientid=0)
begin
if(@startdate='' and @enddate= '')
begin
--without filter supplier wise start
INSERT INTO @SupplierTable
SELECT pm.projectid,pm.SUpplierID,pm.Countryid
,count(pm.SID)Completes
,round((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID),3)CPI
,(round(((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID)*count(pm.SID)),2))Cost
FROM dbo.ProjectMapping pm
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete'
GROUP BY pm.ProjectID,pm.SUpplierID,pm.CountryID
SET @totalrevenue=(SELECT sum(cost) from @SupplierTable st)
INSERT INTO @output_s
SELECT --st.sid ID
(SELECT s.Name FROM dbo.Suppliers s WHERE s.id=st.sid) Parameter
,(SELECT sum(st2.cost) FROM @SupplierTable st2 WHERE st2.sid=st.sid) revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(st2.cost),0)*100/@totalrevenue) END FROM @SupplierTable st2 WHERE st2.sid=st.sid),2)percentt
 FROM @SupplierTable st GROUP BY st.SID
select top(10)* from @output_s order by revenue desc
--with out filter supplier wise end
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--without filter client wise start
INSERT INTO @ClientTable
SELECT pm.ProjectID PID,(SELECT p.ClientID FROM dbo.Projects p WHERE p.ProjectId=pm.projectid)CID,count(pm.sid)Complete,(SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)CPI,round(((SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)*count(pm.sid)),2)Cost
FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete'
GROUP BY pm.ProjectID
SET @totalrevenue= (SELECT sum(cost) FROM @ClientTable ct)
INSERT INTO @output_cl
SELECT (SELECT c.Company FROM dbo.Clients c WHERE c.ClientId=ct.CID)Parameter
,(SELECT sum(cost) FROM @ClientTable ct2 WHERE ct2.CID=ct.CID )revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct2.cost),0)*100/@totalrevenue) END FROM @ClientTable ct2 WHERE ct2.CID=ct.CID),2)percentt
FROM @ClientTable ct GROUP BY ct.CID

select top(10)* from @output_cl order by revenue desc
--without filter client wise end

--without flter country wise start
INSERT INTO @CountryTable
SELECT pm.ProjectID,pm.CountryID
,count(pm.SID)Completes
,(SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID ) CPI
,((SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID )*count(pm.SID))Value
FROM dbo.ProjectMapping pm
JOIN dbo.SupplierProjects sp ON pm.SID = sp.SID
WHERE sp.Status='Complete'
GROUP BY pm.ProjectID,pm.CountryID
set @totalrevenue = (SELECT sum(ct.[Value]) FROM @CountryTable ct)
insert into @output_co 
SELECT (SELECT tc.Country FROM dbo.CountryMaster tc WHERE tc.CountryId=ct.CID)
,(SELECT sum(value) FROM @CountryTable ct2 WHERE ct2.CID=ct.CID)Value
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct3.[Value]),0)*100/@totalrevenue) END FROM @CountryTable ct3 WHERE ct3.CID=ct.CID),2)percentt
FROM @CountryTable ct GROUP BY ct.CID

select top(10)* from @output_co order by revenue desc
--without filter country end
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
end
else
begin
--date filter supplier wise start
INSERT INTO @SupplierTable
SELECT pm.projectid,pm.SUpplierID,pm.Countryid
,count(pm.SID)Completes
,round((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID),3)CPI
,(round(((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID)*count(pm.SID)),2))Cost
FROM dbo.ProjectMapping pm
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID,pm.SUpplierID,pm.CountryID
SET @totalrevenue=(SELECT sum(cost) from @SupplierTable st)
INSERT INTO @output_s
SELECT --st.sid ID
(SELECT s.Name FROM dbo.Suppliers s WHERE s.id=st.sid) Parameter
,(SELECT sum(st2.cost) FROM @SupplierTable st2 WHERE st2.sid=st.sid) revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(st2.cost),0)*100/@totalrevenue) END FROM @SupplierTable st2 WHERE st2.sid=st.sid),2)percentt
 FROM @SupplierTable st GROUP BY st.SID
select top(10)* from @output_s order by revenue desc

--date filter client wise start
INSERT INTO @ClientTable
SELECT pm.ProjectID PID,(SELECT p.ClientID FROM dbo.Projects p WHERE p.ProjectId=pm.projectid)CID,count(pm.sid)Complete,(SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)CPI,round(((SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)*count(pm.sid)),2)Cost
FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID
SET @totalrevenue= (SELECT sum(cost) FROM @ClientTable ct)
INSERT INTO @output_cl
SELECT (SELECT c.Company FROM dbo.Clients c WHERE c.ClientId=ct.CID)Parameter
,(SELECT sum(cost) FROM @ClientTable ct2 WHERE ct2.CID=ct.CID )revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct2.cost),0)*100/@totalrevenue) END FROM @ClientTable ct2 WHERE ct2.CID=ct.CID),2)percentt
FROM @ClientTable ct GROUP BY ct.CID
select top(10)* from @output_cl order by revenue desc
--date filter client wise stop

--date filter countrywise start
INSERT INTO @CountryTable
SELECT pm.ProjectID,pm.CountryID
,count(pm.SID)Completes
,(SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID ) CPI
,((SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID )*count(pm.SID))Value
FROM dbo.ProjectMapping pm
JOIN dbo.SupplierProjects sp ON pm.SID = sp.SID
WHERE sp.Status='Complete' AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID,pm.CountryID
set @totalrevenue = (SELECT sum(ct.[Value]) FROM @CountryTable ct)
insert into @output_co 
SELECT (SELECT tc.Country FROM dbo.CountryMaster tc WHERE tc.CountryId=ct.CID)
,(SELECT sum(value) FROM @CountryTable ct2 WHERE ct2.CID=ct.CID)Value
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct3.[Value]),0)*100/@totalrevenue) END FROM @CountryTable ct3 WHERE ct3.CID=ct.CID),2)percentt
FROM @CountryTable ct GROUP BY ct.CID
select top(10)* from @output_co order by revenue desc
--date filter countrywise stop
end
END
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--countrywise filter start
if(@countryid!=0 and @supplierid=0 and @clientid=0)
begin
if(@startdate='' and @enddate= '')
begin
--country filter supplier wise start
INSERT INTO @SupplierTable
SELECT pm.projectid,pm.SUpplierID,pm.Countryid
,count(pm.SID)Completes
,round((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID),3)CPI
,(round(((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID)*count(pm.SID)),2))Cost
FROM dbo.ProjectMapping pm
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.CountryID=@countryid
GROUP BY pm.ProjectID,pm.SUpplierID,pm.CountryID
SET @totalrevenue=(SELECT sum(cost) from @SupplierTable st)
INSERT INTO @output_s
SELECT --st.sid ID
(SELECT s.Name FROM dbo.Suppliers s WHERE s.id=st.sid) Parameter
,(SELECT sum(st2.cost) FROM @SupplierTable st2 WHERE st2.sid=st.sid) revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(st2.cost),0)*100/@totalrevenue) END FROM @SupplierTable st2 WHERE st2.sid=st.sid),2)percentt
 FROM @SupplierTable st GROUP BY st.SID
select top(10)* from @output_s order by revenue desc
--country filter supplier wise end
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--country filter client wise start
INSERT INTO @ClientTable
SELECT pm.ProjectID PID,(SELECT p.ClientID FROM dbo.Projects p WHERE p.ProjectId=pm.projectid)CID,count(pm.sid)Complete,(SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)CPI,round(((SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)*count(pm.sid)),2)Cost
FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.CountryID=@countryid
GROUP BY pm.ProjectID
SET @totalrevenue= (SELECT sum(cost) FROM @ClientTable ct)
INSERT INTO @output_cl
SELECT (SELECT c.Company FROM dbo.Clients c WHERE c.ClientId=ct.CID)Parameter
,(SELECT sum(cost) FROM @ClientTable ct2 WHERE ct2.CID=ct.CID )revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct2.cost),0)*100/@totalrevenue) END FROM @ClientTable ct2 WHERE ct2.CID=ct.CID),2)percentt
FROM @ClientTable ct GROUP BY ct.CID
select top(10)* from @output_cl order by revenue desc
--country filter client wise end
--country filter country wise start

INSERT INTO @CountryTable
SELECT pm.ProjectID,pm.CountryID
,count(pm.SID)Completes
,(SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID ) CPI
,((SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID )*count(pm.SID))Value
FROM dbo.ProjectMapping pm
JOIN dbo.SupplierProjects sp ON pm.SID = sp.SID
WHERE sp.Status='Complete' AND pm.CountryID=@countryid
GROUP BY pm.ProjectID,pm.CountryID
set @totalrevenue = (SELECT sum(ct.[Value]) FROM @CountryTable ct)
insert into @output_co 
SELECT (SELECT tc.Country FROM dbo.CountryMaster tc WHERE tc.CountryId=ct.CID)
,(SELECT sum(value) FROM @CountryTable ct2 WHERE ct2.CID=ct.CID)Value
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct3.[Value]),0)*100/@totalrevenue) END FROM @CountryTable ct3 WHERE ct3.CID=ct.CID),2)percentt
FROM @CountryTable ct GROUP BY ct.CID
select top(10)* from @output_co order by revenue desc


--country filter country wise end
end
else
begin
--date country filter supplier wise start
----------------------------------------------------------
INSERT INTO @SupplierTable 
SELECT pm.projectid,pm.SUpplierID,pm.Countryid,count(pm.SID)Completes,round((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID),3)CPI
,(round(((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID)*count(pm.SID)),2))Cost FROM dbo.ProjectMapping pm
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate) AND pm.CountryID=@countryid
GROUP BY pm.ProjectID,pm.SUpplierID,pm.CountryID
SET @totalrevenue=(SELECT sum(cost) from @SupplierTable st)
INSERT INTO @output_s
SELECT --st.sid ID
(SELECT s.Name FROM dbo.Suppliers s WHERE s.id=st.sid) Parameter
,(SELECT sum(st2.cost) FROM @SupplierTable st2 WHERE st2.sid=st.sid) revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(st2.cost),0)*100/@totalrevenue) END FROM @SupplierTable st2 WHERE st2.sid=st.sid),2)percentt
 FROM @SupplierTable st GROUP BY st.SID
select top(10)* from @output_s order by revenue desc
--date country filter supplier wise end
-----------------------------------------------------------
--date country filter client wise start
INSERT INTO @ClientTable
SELECT pm.ProjectID PID,(SELECT p.ClientID FROM dbo.Projects p WHERE p.ProjectId=pm.projectid)CID,count(pm.sid)Complete,(SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)CPI,round(((SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)*count(pm.sid)),2)Cost
FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND convert(date,sp.StartDate)<=convert(date,@startdate) AND convert(date,enddate) > = convert(date,@enddate) AND pm.CountryID=@countryid
GROUP BY pm.ProjectID
SET @totalrevenue= (SELECT sum(cost) FROM @ClientTable ct)
INSERT INTO @output_cl
SELECT (SELECT c.Company FROM dbo.Clients c WHERE c.ClientId=ct.CID)Parameter
,(SELECT sum(cost) FROM @ClientTable ct2 WHERE ct2.CID=ct.CID )revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct2.cost),0)*100/@totalrevenue) END FROM @ClientTable ct2 WHERE ct2.CID=ct.CID),2)percentt
FROM @ClientTable ct GROUP BY ct.CID
select top(10)* from @output_cl order by revenue desc
--date country filter client wise end
-----------------------------------------------------------
--date country filter country wise start
INSERT INTO @CountryTable
SELECT pm.ProjectID,pm.CountryID,count(pm.SID)Completes,(SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID ) CPI,((SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID )*count(pm.SID))Value
FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.SID = sp.SID
WHERE sp.Status='Complete' AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate) AND pm.CountryID=@countryid
GROUP BY pm.ProjectID,pm.CountryID
set @totalrevenue = (SELECT sum(ct.[Value]) FROM @CountryTable ct)
insert into @output_co 
SELECT (SELECT tc.Country FROM dbo.CountryMaster tc WHERE tc.CountryId=ct.CID) 
,(SELECT sum(value) FROM @CountryTable ct2 WHERE ct2.CID=ct.CID)Value 
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct3.[Value]),0)*100/@totalrevenue) END FROM @CountryTable ct3 WHERE ct3.CID=ct.CID),2)percentt
FROM @CountryTable ct GROUP BY ct.CID
select top(10)* from @output_co order by revenue desc
--date country filter country wise end
----------------------------------------------------------
end
end
--supplier filter start
if(@countryid=0 and @supplierid!=0 and @clientid=0)
BEGIN
---------------------------------------------------------------------------------
if(@startdate='' and @enddate= '')
begin
--supplier filter supplier wise start
INSERT INTO @SupplierTable
SELECT pm.projectid,pm.SUpplierID,pm.Countryid
,count(pm.SID)Completes
,round((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID),3)CPI
,(round(((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID)*count(pm.SID)),2))Cost
FROM dbo.ProjectMapping pm
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.SUpplierID=@supplierid
GROUP BY pm.ProjectID,pm.SUpplierID,pm.CountryID
SET @totalrevenue=(SELECT sum(cost) from @SupplierTable st)
INSERT INTO @output_s
SELECT --st.sid ID
(SELECT s.Name FROM dbo.Suppliers s WHERE s.id=st.sid) Parameter
,(SELECT sum(st2.cost) FROM @SupplierTable st2 WHERE st2.sid=st.sid) revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(st2.cost),0)*100/@totalrevenue) END FROM @SupplierTable st2 WHERE st2.sid=st.sid),2)percentt
 FROM @SupplierTable st GROUP BY st.SID
select top(10)* from @output_s order by revenue desc
--supplier filter supplier wise end
---------------------------------------------------------------------------------
--supplier filter client wise start
INSERT INTO @ClientTable
SELECT pm.ProjectID PID,(SELECT p.ClientID FROM dbo.Projects p WHERE p.ProjectId=pm.projectid)CID,count(pm.sid)Complete,(SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)CPI,round(((SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)*count(pm.sid)),2)Cost
FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.SUpplierID=@supplierid
GROUP BY pm.ProjectID
SET @totalrevenue= (SELECT sum(cost) FROM @ClientTable ct)
INSERT INTO @output_cl
SELECT (SELECT c.Company FROM dbo.Clients c WHERE c.ClientId=ct.CID)Parameter
,(SELECT sum(cost) FROM @ClientTable ct2 WHERE ct2.CID=ct.CID )revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct2.cost),0)*100/@totalrevenue) END FROM @ClientTable ct2 WHERE ct2.CID=ct.CID),2)percentt
FROM @ClientTable ct GROUP BY ct.CID
select top(10)* from @output_cl order by revenue desc
--supplier filter client wise end
---------------------------------------------------------------------------------
--supplier filter country wise start
INSERT INTO @CountryTable
SELECT pm.ProjectID,pm.CountryID,count(pm.SID)Completes,(SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID ) CPI,((SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID )*count(pm.SID))Value
FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.SID = sp.SID
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid
GROUP BY pm.ProjectID,pm.CountryID
set @totalrevenue = (SELECT sum(ct.[Value]) FROM @CountryTable ct)
insert into @output_co 
SELECT (SELECT tc.Country FROM dbo.CountryMaster tc WHERE tc.CountryId=ct.CID)
,(SELECT sum(value) FROM @CountryTable ct2 WHERE ct2.CID=ct.CID)Value
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct3.[Value]),0)*100/@totalrevenue) END FROM @CountryTable ct3 WHERE ct3.CID=ct.CID),2)percentt
FROM @CountryTable ct GROUP BY ct.CID
select top(10)* from @output_co order by revenue desc
--supplier filter country wise end
----------------------------------------------------------------------------------
end
else
BEGIN
-----------------------------------------------------------------------------------
--date supplier filter supplier wise start
INSERT INTO @SupplierTable
SELECT pm.projectid,pm.SUpplierID,pm.Countryid,count(pm.SID)Completes,round((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID),3)CPI
,(round(((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID)*count(pm.SID)),2))Cost FROM dbo.ProjectMapping pm
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.SUpplierID=@supplierid AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID,pm.SUpplierID,pm.CountryID
SET @totalrevenue=(SELECT sum(cost) from @SupplierTable st)
INSERT INTO @output_s
SELECT --st.sid ID
(SELECT s.Name FROM dbo.Suppliers s WHERE s.id=st.sid) Parameter
,(SELECT sum(st2.cost) FROM @SupplierTable st2 WHERE st2.sid=st.sid) revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(st2.cost),0)*100/@totalrevenue) END FROM @SupplierTable st2 WHERE st2.sid=st.sid),2)percentt
 FROM @SupplierTable st GROUP BY st.SID
select top(10)* from @output_s order by revenue desc
--date supplier filter supplier wise end
-------------------------------------------------------------------------------------
--date supplier filter client wise start
INSERT INTO @ClientTable
SELECT pm.ProjectID PID,(SELECT p.ClientID FROM dbo.Projects p WHERE p.ProjectId=pm.projectid)CID,count(pm.sid)Complete,(SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)CPI,round(((SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)*count(pm.sid)),2)Cost
FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.SUpplierID=@supplierid AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID
SET @totalrevenue= (SELECT sum(cost) FROM @ClientTable ct)
INSERT INTO @output_cl
SELECT (SELECT c.Company FROM dbo.Clients c WHERE c.ClientId=ct.CID)Parameter
,(SELECT sum(cost) FROM @ClientTable ct2 WHERE ct2.CID=ct.CID )revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct2.cost),0)*100/@totalrevenue) END FROM @ClientTable ct2 WHERE ct2.CID=ct.CID),2)percentt
FROM @ClientTable ct GROUP BY ct.CID
select top(10)* from @output_cl order by revenue desc
--date supplier filter client wise end
-------------------------------------------------------------------------------------
--date supplier filter country wise start
INSERT INTO @CountryTable
SELECT pm.ProjectID,pm.CountryID,count(pm.SID)Completes,(SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID ) CPI,((SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID )*count(pm.SID))Value
FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.SID = sp.SID
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID,pm.CountryID
set @totalrevenue = (SELECT sum(ct.[Value]) FROM @CountryTable ct)
insert into @output_co 
SELECT (SELECT tc.Country FROM dbo.CountryMaster tc WHERE tc.CountryId=ct.CID)
,(SELECT sum(value) FROM @CountryTable ct2 WHERE ct2.CID=ct.CID)Value
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct3.[Value]),0)*100/@totalrevenue) END FROM @CountryTable ct3 WHERE ct3.CID=ct.CID),2)percentt
FROM @CountryTable ct GROUP BY ct.CID
select top(10)* from @output_co order by revenue desc
--date supplier filter country wise end
---------------------------------------------------------------------------------------

end
end
--Client Filter start
if(@countryid=0 and @supplierid=0 and @clientid!=0)
begin
if(@startdate='' and @enddate= '')
BEGIN
------------------------------------------------------------------------------------------
--client filter supplier wise start
INSERT INTO @SupplierTable
SELECT pm.projectid,pm.SUpplierID,pm.Countryid,count(pm.SID)Completes,round((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID),3)CPI
,(round(((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID)*count(pm.SID)),2))Cost FROM dbo.ProjectMapping pm
JOIN dbo.Projects p ON pm.ProjectID = p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND p.ClientID=@clientid
GROUP BY pm.ProjectID,pm.SUpplierID,pm.CountryID
SET @totalrevenue=(SELECT sum(cost) from @SupplierTable st)
INSERT INTO @output_s
SELECT --st.sid ID
(SELECT s.Name FROM dbo.Suppliers s WHERE s.id=st.sid) Parameter
,(SELECT sum(st2.cost) FROM @SupplierTable st2 WHERE st2.sid=st.sid) revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(st2.cost),0)*100/@totalrevenue) END FROM @SupplierTable st2 WHERE st2.sid=st.sid),2)percentt
 FROM @SupplierTable st GROUP BY st.SID
select top(10)* from @output_s order by revenue desc
--client filter supplier wise end
------------------------------------------------------------------------------------------
--client filter client wise start
INSERT INTO @ClientTable
SELECT pm.ProjectID PID,(SELECT p.ClientID FROM dbo.Projects p WHERE p.ProjectId=pm.projectid)CID,count(pm.sid)Complete,(SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)CPI,round(((SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)*count(pm.sid)),2)Cost
FROM dbo.ProjectMapping pm JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND p.ClientID=@clientid
GROUP BY pm.ProjectID
SET @totalrevenue= (SELECT sum(cost) FROM @ClientTable ct)
INSERT INTO @output_cl
SELECT (SELECT c.Company FROM dbo.Clients c WHERE c.ClientId=ct.CID)Parameter
,(SELECT sum(cost) FROM @ClientTable ct2 WHERE ct2.CID=ct.CID )revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct2.cost),0)*100/@totalrevenue) END FROM @ClientTable ct2 WHERE ct2.CID=ct.CID),2)percentt
FROM @ClientTable ct GROUP BY ct.CID
select top(10)* from @output_cl order by revenue desc
--client filter client wise end

----------------------------------------------------------------------------------------------

--client filter country wise start
INSERT INTO @CountryTable
SELECT pm.ProjectID,pm.CountryID,count(pm.SID)Completes,(SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID ) CPI
,((SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID )*count(pm.SID))Value
FROM dbo.ProjectMapping pm JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId JOIN dbo.SupplierProjects sp ON pm.SID = sp.SID
WHERE sp.Status='Complete' AND p.ClientID=@clientid
GROUP BY pm.ProjectID,pm.CountryID
set @totalrevenue = (SELECT sum(ct.[Value]) FROM @CountryTable ct)
insert into @output_co 
SELECT (SELECT tc.Country FROM dbo.CountryMaster tc WHERE tc.CountryId=ct.CID)
,(SELECT sum(value) FROM @CountryTable ct2 WHERE ct2.CID=ct.CID)Value
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct3.[Value]),0)*100/@totalrevenue) END FROM @CountryTable ct3 WHERE ct3.CID=ct.CID),2)percentt
FROM @CountryTable ct GROUP BY ct.CID
select top(10)* from @output_co order by revenue desc
--client filter country wise end

-----------------------------------------------------------------------------------------------
end
else
BEGIN
-----------------------------------------------------------------------------------------------
--date client filter supplier wise start
INSERT INTO @SupplierTable
SELECT pm.projectid,pm.SUpplierID,pm.Countryid ,count(pm.SID)Completes,round((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID),3)CPI
,(round(((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID)*count(pm.SID)),2))Cost FROM dbo.ProjectMapping pm
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND p.ClientID=@clientid AND convert(date,sp.StartDate)<=convert(date,@startdate) AND convert(date,enddate) > = convert(date,@enddate)
GROUP BY pm.ProjectID,pm.SUpplierID,pm.CountryID
SET @totalrevenue=(SELECT sum(cost) from @SupplierTable st)
INSERT INTO @output_s
SELECT --st.sid ID
(SELECT s.Name FROM dbo.Suppliers s WHERE s.id=st.sid) Parameter
,(SELECT sum(st2.cost) FROM @SupplierTable st2 WHERE st2.sid=st.sid) revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(st2.cost),0)*100/@totalrevenue) END FROM @SupplierTable st2 WHERE st2.sid=st.sid),2)percentt
 FROM @SupplierTable st GROUP BY st.SID
select top(10)* from @output_s order by revenue desc
--date client filter supplier wise end
-----------------------------------------------------------------------------------------------
--date client filter client wise start
INSERT INTO @ClientTable
SELECT pm.ProjectID PID,(SELECT p.ClientID FROM dbo.Projects p WHERE p.ProjectId=pm.projectid)CID,count(pm.sid)Complete,(SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)CPI,round(((SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)*count(pm.sid)),2)Cost
FROM dbo.ProjectMapping pm 
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND p.ClientID=@clientid AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID
SET @totalrevenue= (SELECT sum(cost) FROM @ClientTable ct)
INSERT INTO @output_cl
SELECT (SELECT c.Company FROM dbo.Clients c WHERE c.ClientId=ct.CID)Parameter
,(SELECT sum(cost) FROM @ClientTable ct2 WHERE ct2.CID=ct.CID )revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct2.cost),0)*100/@totalrevenue) END FROM @ClientTable ct2 WHERE ct2.CID=ct.CID),2)percentt
FROM @ClientTable ct GROUP BY ct.CID
select top(10)* from @output_cl order by revenue desc
--date client filter client wise end
------------------------------------------------------------------------------------------------
--date client filter country wise start
INSERT INTO @CountryTable
SELECT pm.ProjectID,pm.CountryID,count(pm.SID)Completes,(SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID ) CPI
,((SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID )*count(pm.SID))Value FROM dbo.ProjectMapping pm
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.SID = sp.SID
WHERE sp.Status='Complete' AND p.ClientID=@clientid AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID,pm.CountryID
set @totalrevenue = (SELECT sum(ct.[Value]) FROM @CountryTable ct)
insert into @output_co 
SELECT (SELECT tc.Country FROM dbo.CountryMaster tc WHERE tc.CountryId=ct.CID)
,(SELECT sum(value) FROM @CountryTable ct2 WHERE ct2.CID=ct.CID)Value
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct3.[Value]),0)*100/@totalrevenue) END FROM @CountryTable ct3 WHERE ct3.CID=ct.CID),2)percentt
FROM @CountryTable ct GROUP BY ct.CID
select top(10)* from @output_co order by revenue desc
--date client filter country wise end
------------------------------------------------------------------------------------------------

end
end
--country supplier Filter start
if(@countryid!=0 and @supplierid!=0 and @clientid=0)
begin
if(@startdate='' and @enddate= '')
BEGIN
-------------------------------------------------------------------------------------
--country supplier filter supplier wise start
INSERT INTO @SupplierTable
SELECT pm.projectid,pm.SUpplierID,pm.Countryid,count(pm.SID)Completes,round((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID),3)CPI
,(round(((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID)*count(pm.SID)),2))Cost FROM dbo.ProjectMapping pm
--JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.CountryID=@countryid AND pm.SUpplierID=@supplierid
GROUP BY pm.ProjectID,pm.SUpplierID,pm.CountryID
SET @totalrevenue=(SELECT sum(cost) from @SupplierTable st)
INSERT INTO @output_s
SELECT --st.sid ID
(SELECT s.Name FROM dbo.Suppliers s WHERE s.id=st.sid) Parameter
,(SELECT sum(st2.cost) FROM @SupplierTable st2 WHERE st2.sid=st.sid) revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(st2.cost),0)*100/@totalrevenue) END FROM @SupplierTable st2 WHERE st2.sid=st.sid),2)percentt
 FROM @SupplierTable st GROUP BY st.SID
select top(10)* from @output_s order by revenue desc
--country supplier filter supplier wise end
-------------------------------------------------------------------------------------
--country supplier filter client wise start
INSERT INTO @ClientTable
SELECT pm.ProjectID PID,(SELECT p.ClientID FROM dbo.Projects p WHERE p.ProjectId=pm.projectid)CID,count(pm.sid)Complete,(SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)CPI,round(((SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)*count(pm.sid)),2)Cost
FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.CountryID=@countryid AND pm.SUpplierID=@supplierid
GROUP BY pm.ProjectID
SET @totalrevenue= (SELECT sum(cost) FROM @ClientTable ct)
INSERT INTO @output_cl
SELECT (SELECT c.Company FROM dbo.Clients c WHERE c.ClientId=ct.CID)Parameter
,(SELECT sum(cost) FROM @ClientTable ct2 WHERE ct2.CID=ct.CID )revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct2.cost),0)*100/@totalrevenue) END FROM @ClientTable ct2 WHERE ct2.CID=ct.CID),2)percentt
FROM @ClientTable ct GROUP BY ct.CID
select top(10)* from @output_cl order by revenue desc
--country supplier filter client wise end
----------------------------------------------------------------------------------------
--country supplier filter country wise start
INSERT INTO @CountryTable
SELECT pm.ProjectID,pm.CountryID,count(pm.SID)Completes,(SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID ) CPI
,((SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID )*count(pm.SID))Value FROM dbo.ProjectMapping pm
JOIN dbo.SupplierProjects sp ON pm.SID = sp.SID
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND pm.CountryID=@countryid
GROUP BY pm.ProjectID,pm.CountryID
set @totalrevenue = (SELECT sum(ct.[Value]) FROM @CountryTable ct)
insert into @output_co 
SELECT (SELECT tc.Country FROM dbo.CountryMaster tc WHERE tc.CountryId=ct.CID)
,(SELECT sum(value) FROM @CountryTable ct2 WHERE ct2.CID=ct.CID)Value
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct3.[Value]),0)*100/@totalrevenue) END FROM @CountryTable ct3 WHERE ct3.CID=ct.CID),2)percentt
FROM @CountryTable ct GROUP BY ct.CID
select top(10)* from @output_co order by revenue desc
--country supplier filter country wise end
----------------------------------------------------------------------------------------
end
else
BEGIN
-----------------------------------------------------------------------------------------
--date country supplier filter supplier wise start
INSERT INTO @SupplierTable
SELECT pm.projectid,pm.SUpplierID,pm.Countryid
,count(pm.SID)Completes
,round((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID),3)CPI
,(round(((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID)*count(pm.SID)),2))Cost
FROM dbo.ProjectMapping pm
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.CountryID=@countryid AND pm.SUpplierID=@supplierid AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID,pm.SUpplierID,pm.CountryID
SET @totalrevenue=(SELECT sum(cost) from @SupplierTable st)
INSERT INTO @output_s
SELECT --st.sid ID
(SELECT s.Name FROM dbo.Suppliers s WHERE s.id=st.sid) Parameter
,(SELECT sum(st2.cost) FROM @SupplierTable st2 WHERE st2.sid=st.sid) revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(st2.cost),0)*100/@totalrevenue) END FROM @SupplierTable st2 WHERE st2.sid=st.sid),2)percentt
 FROM @SupplierTable st GROUP BY st.SID
select top(10)* from @output_s order by revenue desc
--date country supplier filter supplier wise end
----------------------------------------------------------------------------------------
--date country supplier filter client wise start
INSERT INTO @ClientTable
SELECT pm.ProjectID PID,(SELECT p.ClientID FROM dbo.Projects p WHERE p.ProjectId=pm.projectid)CID,count(pm.sid)Complete,(SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)CPI,round(((SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)*count(pm.sid)),2)Cost
FROM dbo.ProjectMapping pm JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.CountryID=@countryid AND pm.SUpplierID=@supplierid AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID
SET @totalrevenue= (SELECT sum(cost) FROM @ClientTable ct)
INSERT INTO @output_cl
SELECT (SELECT c.Company FROM dbo.Clients c WHERE c.ClientId=ct.CID)Parameter
,(SELECT sum(cost) FROM @ClientTable ct2 WHERE ct2.CID=ct.CID )revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct2.cost),0)*100/@totalrevenue) END FROM @ClientTable ct2 WHERE ct2.CID=ct.CID),2)percentt
FROM @ClientTable ct GROUP BY ct.CID
select top(10)* from @output_cl order by revenue desc
--date country supplier filter client wise end
----------------------------------------------------------------------------------------
--date country supplier filter country wise start
INSERT INTO @CountryTable
SELECT pm.ProjectID,pm.CountryID
,count(pm.SID)Completes
,(SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID ) CPI
,((SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID )*count(pm.SID))Value
FROM dbo.ProjectMapping pm
JOIN dbo.SupplierProjects sp ON pm.SID = sp.SID
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND pm.CountryID=@countryid AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID,pm.CountryID
set @totalrevenue = (SELECT sum(ct.[Value]) FROM @CountryTable ct)
insert into @output_co 
SELECT (SELECT tc.Country FROM dbo.CountryMaster tc WHERE tc.CountryId=ct.CID)
,(SELECT sum(value) FROM @CountryTable ct2 WHERE ct2.CID=ct.CID)Value
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct3.[Value]),0)*100/@totalrevenue) END FROM @CountryTable ct3 WHERE ct3.CID=ct.CID),2)percentt
FROM @CountryTable ct GROUP BY ct.CID
select top(10)* from @output_co order by revenue desc
--date country supplier filter country wise end
-----------------------------------------------------------------------------------------

end
end
--supplier client Filter start
if(@countryid=0 and @supplierid!=0 and @clientid!=0)
begin
if(@startdate='' and @enddate= '')
BEGIN
-------------------------------------------------------------------------------------------------
--supplier client filter supplier wise start
INSERT INTO @SupplierTable
SELECT pm.projectid,pm.SUpplierID,pm.Countryid
,count(pm.SID)Completes
,round((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID),3)CPI
,(round(((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID)*count(pm.SID)),2))Cost
FROM dbo.ProjectMapping pm
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.SUpplierID=@supplierid AND p.ClientID=@clientid
GROUP BY pm.ProjectID,pm.SUpplierID,pm.CountryID
SET @totalrevenue=(SELECT sum(cost) from @SupplierTable st)
INSERT INTO @output_s
SELECT --st.sid ID
(SELECT s.Name FROM dbo.Suppliers s WHERE s.id=st.sid) Parameter
,(SELECT sum(st2.cost) FROM @SupplierTable st2 WHERE st2.sid=st.sid) revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(st2.cost),0)*100/@totalrevenue) END FROM @SupplierTable st2 WHERE st2.sid=st.sid),2)percentt
 FROM @SupplierTable st GROUP BY st.SID
select top(10)* from @output_s order by revenue desc
--supplier client filter supplier wise end
-----------------------------------------------------------------------------------------------
--supplier client filter client wise start
INSERT INTO @ClientTable
SELECT pm.ProjectID PID,(SELECT p.ClientID FROM dbo.Projects p WHERE p.ProjectId=pm.projectid)CID,count(pm.sid)Complete,(SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)CPI,round(((SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)*count(pm.sid)),2)Cost
FROM dbo.ProjectMapping pm 
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.SUpplierID=@supplierid AND p.ClientID=@clientid
GROUP BY pm.ProjectID
SET @totalrevenue= (SELECT sum(cost) FROM @ClientTable ct)
INSERT INTO @output_cl
SELECT (SELECT c.Company FROM dbo.Clients c WHERE c.ClientId=ct.CID)Parameter
,(SELECT sum(cost) FROM @ClientTable ct2 WHERE ct2.CID=ct.CID )revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct2.cost),0)*100/@totalrevenue) END FROM @ClientTable ct2 WHERE ct2.CID=ct.CID),2)percentt
FROM @ClientTable ct GROUP BY ct.CID
select top(10)* from @output_cl order by revenue desc
--supplier client filter client wise end
------------------------------------------------------------------------------------------------
--supplier client filter country wise start
INSERT INTO @CountryTable
SELECT pm.ProjectID,pm.CountryID,count(pm.SID)Completes,(SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID ) CPI
,((SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID )*count(pm.SID))Value
FROM dbo.ProjectMapping pm
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.SID = sp.SID
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid and p.ClientID=@clientid
GROUP BY pm.ProjectID,pm.CountryID
set @totalrevenue = (SELECT sum(ct.[Value]) FROM @CountryTable ct)
insert into @output_co 
SELECT (SELECT tc.Country FROM dbo.CountryMaster tc WHERE tc.CountryId=ct.CID)
,(SELECT sum(value) FROM @CountryTable ct2 WHERE ct2.CID=ct.CID)Value
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct3.[Value]),0)*100/@totalrevenue) END FROM @CountryTable ct3 WHERE ct3.CID=ct.CID),2)percentt
FROM @CountryTable ct GROUP BY ct.CID
select top(10)* from @output_co order by revenue desc
--supplier client filter country wise end
-------------------------------------------------------------------------------------------------
end
else
BEGIN
-------------------------------------------------------------------------------------------
--date supplier client filter supplier wise start
INSERT INTO @SupplierTable
SELECT pm.projectid,pm.SUpplierID,pm.Countryid,count(pm.SID)Completes,round((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID),3)CPI
,(round(((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID)*count(pm.SID)),2))Cost FROM dbo.ProjectMapping pm
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.SUpplierID=@supplierid AND p.ClientID=@clientid AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID,pm.SUpplierID,pm.CountryID
SET @totalrevenue=(SELECT sum(cost) from @SupplierTable st)
INSERT INTO @output_s
SELECT --st.sid ID
(SELECT s.Name FROM dbo.Suppliers s WHERE s.id=st.sid) Parameter
,(SELECT sum(st2.cost) FROM @SupplierTable st2 WHERE st2.sid=st.sid) revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(st2.cost),0)*100/@totalrevenue) END FROM @SupplierTable st2 WHERE st2.sid=st.sid),2)percentt
 FROM @SupplierTable st GROUP BY st.SID
select top(10)* from @output_s order by revenue desc
--date supplier client filter supplier wise end
----------------------------------------------------------------------------------------------
--date supplier client filter client wise start
INSERT INTO @ClientTable
SELECT pm.ProjectID PID,(SELECT p.ClientID FROM dbo.Projects p WHERE p.ProjectId=pm.projectid)CID,count(pm.sid)Complete,(SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)CPI,round(((SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)*count(pm.sid)),2)Cost
FROM dbo.ProjectMapping pm JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.SUpplierID=@supplierid AND p.ClientID=@clientid AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID
SET @totalrevenue= (SELECT sum(cost) FROM @ClientTable ct)
INSERT INTO @output_cl
SELECT (SELECT c.Company FROM dbo.Clients c WHERE c.ClientId=ct.CID)Parameter
,(SELECT sum(cost) FROM @ClientTable ct2 WHERE ct2.CID=ct.CID )revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct2.cost),0)*100/@totalrevenue) END FROM @ClientTable ct2 WHERE ct2.CID=ct.CID),2)percentt
FROM @ClientTable ct GROUP BY ct.CID
select top(10)* from @output_cl order by revenue desc
--date supplier client filter client wise end
---------------------------------------------------------------------------------------------------------
--date supplier client filter country wise start
INSERT INTO @CountryTable
SELECT pm.ProjectID,pm.CountryID,count(pm.SID)Completes,(SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID ) CPI,((SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID )*count(pm.SID))Value
FROM dbo.ProjectMapping pm JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId JOIN dbo.SupplierProjects sp ON pm.SID = sp.SID
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND p.ClientID=@clientid AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID,pm.CountryID
set @totalrevenue = (SELECT sum(ct.[Value]) FROM @CountryTable ct)
insert into @output_co 
SELECT (SELECT tc.Country FROM dbo.CountryMaster tc WHERE tc.CountryId=ct.CID)
,(SELECT sum(value) FROM @CountryTable ct2 WHERE ct2.CID=ct.CID)Value
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct3.[Value]),0)*100/@totalrevenue) END FROM @CountryTable ct3 WHERE ct3.CID=ct.CID),2)percentt
FROM @CountryTable ct GROUP BY ct.CID
select top(10)* from @output_co order by revenue desc
--date supplier client filter country wise end
------------------------------------------------------------------------------------------------------------

end
end
--country client Filter start
if(@countryid!=0 and @supplierid=0 and @clientid!=0)
begin
if(@startdate='' and @enddate= '')
BEGIN
-------------------------------------------------------------------------------------------------
--country client filter supplier wise start
INSERT INTO @SupplierTable
SELECT pm.projectid,pm.SUpplierID,pm.Countryid
,count(pm.SID)Completes
,round((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID),3)CPI
,(round(((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID)*count(pm.SID)),2))Cost
FROM dbo.ProjectMapping pm
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.CountryID=@countryid AND p.ClientID=@clientid
GROUP BY pm.ProjectID,pm.SUpplierID,pm.CountryID
SET @totalrevenue=(SELECT sum(cost) from @SupplierTable st)
INSERT INTO @output_s
SELECT --st.sid ID
(SELECT s.Name FROM dbo.Suppliers s WHERE s.id=st.sid) Parameter
,(SELECT sum(st2.cost) FROM @SupplierTable st2 WHERE st2.sid=st.sid) revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(st2.cost),0)*100/@totalrevenue) END FROM @SupplierTable st2 WHERE st2.sid=st.sid),2)percentt
 FROM @SupplierTable st GROUP BY st.SID
select top(10)* from @output_s order by revenue desc
--country client filter supplier wise end
--------------------------------------------------------------------------------------------------
--country client filter client wise start
INSERT INTO @ClientTable
SELECT pm.ProjectID PID,(SELECT p.ClientID FROM dbo.Projects p WHERE p.ProjectId=pm.projectid)CID,count(pm.sid)Complete,(SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)CPI,round(((SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)*count(pm.sid)),2)Cost
FROM dbo.ProjectMapping pm 
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.CountryID=@countryid AND p.ClientID=@clientid
GROUP BY pm.ProjectID
SET @totalrevenue= (SELECT sum(cost) FROM @ClientTable ct)
INSERT INTO @output_cl
SELECT (SELECT c.Company FROM dbo.Clients c WHERE c.ClientId=ct.CID)Parameter
,(SELECT sum(cost) FROM @ClientTable ct2 WHERE ct2.CID=ct.CID )revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct2.cost),0)*100/@totalrevenue) END FROM @ClientTable ct2 WHERE ct2.CID=ct.CID),2)percentt
FROM @ClientTable ct GROUP BY ct.CID
select top(10)* from @output_cl order by revenue desc
--country client filter client wise end
-------------------------------------------------------------------------------------------------
--country client filter country wise start
INSERT INTO @CountryTable
SELECT pm.ProjectID,pm.CountryID,count(pm.SID)Completes,(SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID ) CPI,((SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID )*count(pm.SID))Value
FROM dbo.ProjectMapping pm
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.SID = sp.SID AND pm.CountryID=@countryid AND p.ClientID=@clientid
WHERE sp.Status='Complete'
GROUP BY pm.ProjectID,pm.CountryID
set @totalrevenue = (SELECT sum(ct.[Value]) FROM @CountryTable ct)
insert into @output_co 
SELECT (SELECT tc.Country FROM dbo.CountryMaster tc WHERE tc.CountryId=ct.CID)
,(SELECT sum(value) FROM @CountryTable ct2 WHERE ct2.CID=ct.CID)Value
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct3.[Value]),0)*100/@totalrevenue) END FROM @CountryTable ct3 WHERE ct3.CID=ct.CID),2)percentt
FROM @CountryTable ct GROUP BY ct.CID
select top(10)* from @output_co order by revenue desc
--country client filter country wise end
---------------------------------------------------------------------------------------------------------
end
else
BEGIN
----------------------------------------------------------------------------------------------
--date country client filter supplier wise start
INSERT INTO @SupplierTable
SELECT pm.projectid,pm.SUpplierID,pm.Countryid,count(pm.SID)Completes,round((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID),3)CPI
,(round(((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID)*count(pm.SID)),2))Cost
FROM dbo.ProjectMapping pm
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.CountryID=@countryid AND p.ClientID=@clientid AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID,pm.SUpplierID,pm.CountryID
SET @totalrevenue=(SELECT sum(cost) from @SupplierTable st)
INSERT INTO @output_s
SELECT --st.sid ID
(SELECT s.Name FROM dbo.Suppliers s WHERE s.id=st.sid) Parameter
,(SELECT sum(st2.cost) FROM @SupplierTable st2 WHERE st2.sid=st.sid) revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(st2.cost),0)*100/@totalrevenue) END FROM @SupplierTable st2 WHERE st2.sid=st.sid),2)percentt
 FROM @SupplierTable st GROUP BY st.SID
select top(10)* from @output_s order by revenue desc
--date country client filter supplier wise end
-----------------------------------------------------------------------------------------------
--date country client filter client wise start
INSERT INTO @ClientTable
SELECT pm.ProjectID PID,(SELECT p.ClientID FROM dbo.Projects p WHERE p.ProjectId=pm.projectid)CID,count(pm.sid)Complete,(SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)CPI,round(((SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)*count(pm.sid)),2)Cost
FROM dbo.ProjectMapping pm 
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.CountryID=@countryid AND p.ClientID=@clientid AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID
SET @totalrevenue= (SELECT sum(cost) FROM @ClientTable ct)
INSERT INTO @output_cl
SELECT (SELECT c.Company FROM dbo.Clients c WHERE c.ClientId=ct.CID)Parameter
,(SELECT sum(cost) FROM @ClientTable ct2 WHERE ct2.CID=ct.CID )revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct2.cost),0)*100/@totalrevenue) END FROM @ClientTable ct2 WHERE ct2.CID=ct.CID),2)percentt
FROM @ClientTable ct GROUP BY ct.CID
select top(10)* from @output_cl order by revenue desc
--date country client filter client wise end
------------------------------------------------------------------------------------------------
--date country client filter country wise start
INSERT INTO @CountryTable
SELECT pm.ProjectID,pm.CountryID
,count(pm.SID)Completes
,(SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID ) CPI
,((SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID )*count(pm.SID))Value
FROM dbo.ProjectMapping pm
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.SID = sp.SID
WHERE sp.Status='Complete' AND pm.CountryID=@countryid AND p.ClientID=@clientid AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID,pm.CountryID
set @totalrevenue = (SELECT sum(ct.[Value]) FROM @CountryTable ct)
insert into @output_co 
SELECT (SELECT tc.Country FROM dbo.CountryMaster tc WHERE tc.CountryId=ct.CID)
,(SELECT sum(value) FROM @CountryTable ct2 WHERE ct2.CID=ct.CID)Value
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct3.[Value]),0)*100/@totalrevenue) END FROM @CountryTable ct3 WHERE ct3.CID=ct.CID),2)percentt
FROM @CountryTable ct GROUP BY ct.CID
select top(10)* from @output_co order by revenue desc
--date country client filter country wise end
------------------------------------------------------------------------------------------------

end
end
--country supplier client Filter start
if(@countryid!=0 and @supplierid!=0 and @clientid!=0)
begin
if(@startdate='' and @enddate= '')
BEGIN
---------------------------------------------------------------------------------------------
--country supplier client filter supplier wise start
INSERT INTO @SupplierTable
SELECT pm.projectid,pm.SUpplierID,pm.Countryid,count(pm.SID)Completes,round((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID),3)CPI
,(round(((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID)*count(pm.SID)),2))Cost
FROM dbo.ProjectMapping pm
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.CountryID=@countryid AND p.ClientID=@clientid AND pm.SUpplierID=@supplierid
GROUP BY pm.ProjectID,pm.SUpplierID,pm.CountryID
SET @totalrevenue=(SELECT sum(cost) from @SupplierTable st)
INSERT INTO @output_s
SELECT --st.sid ID
(SELECT s.Name FROM dbo.Suppliers s WHERE s.id=st.sid) Parameter
,(SELECT sum(st2.cost) FROM @SupplierTable st2 WHERE st2.sid=st.sid) revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(st2.cost),0)*100/@totalrevenue) END FROM @SupplierTable st2 WHERE st2.sid=st.sid),2)percentt
 FROM @SupplierTable st GROUP BY st.SID
select top(10)* from @output_s order by revenue desc
--country supplier client filter supplier wise end
--------------------------------------------------------------------------------------------
--country supplier client filter client wise start
INSERT INTO @ClientTable
SELECT pm.ProjectID PID,(SELECT p.ClientID FROM dbo.Projects p WHERE p.ProjectId=pm.projectid)CID,count(pm.sid)Complete,(SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)CPI,round(((SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)*count(pm.sid)),2)Cost
FROM dbo.ProjectMapping pm 
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.CountryID=@countryid AND p.ClientID=@clientid AND pm.SUpplierID=@supplierid
GROUP BY pm.ProjectID
SET @totalrevenue= (SELECT sum(cost) FROM @ClientTable ct)
INSERT INTO @output_cl
SELECT (SELECT c.Company FROM dbo.Clients c WHERE c.ClientId=ct.CID)Parameter
,(SELECT sum(cost) FROM @ClientTable ct2 WHERE ct2.CID=ct.CID )revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct2.cost),0)*100/@totalrevenue) END FROM @ClientTable ct2 WHERE ct2.CID=ct.CID),2)percentt
FROM @ClientTable ct GROUP BY ct.CID
select top(10)* from @output_cl order by revenue desc
--country supplier client filter client wise end
-------------------------------------------------------------------------------------------
--country supplier client filter country wise start
INSERT INTO @CountryTable
SELECT pm.ProjectID,pm.CountryID,count(pm.SID)Completes,(SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID ) CPI,((SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID )*count(pm.SID))Value
FROM dbo.ProjectMapping pm
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.SID = sp.SID
WHERE sp.Status='Complete' AND pm.CountryID=@countryid AND pm.SUpplierID=@supplierid AND p.ClientID=@clientid
GROUP BY pm.ProjectID,pm.CountryID
set @totalrevenue = (SELECT sum(ct.[Value]) FROM @CountryTable ct)
insert into @output_co 
SELECT (SELECT tc.Country FROM dbo.CountryMaster tc WHERE tc.CountryId=ct.CID)
,(SELECT sum(value) FROM @CountryTable ct2 WHERE ct2.CID=ct.CID)Value
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct3.[Value]),0)*100/@totalrevenue) END FROM @CountryTable ct3 WHERE ct3.CID=ct.CID),2)percentt
FROM @CountryTable ct GROUP BY ct.CID
select top(10)* from @output_co order by revenue desc
--country supplier client filter country wise end
----------------------------------------------------------------------------------------------
end
else
BEGIN
----------------------------------------------------------------------------------------------
--date country supplier client filter supplier wise start
INSERT INTO @SupplierTable
SELECT pm.projectid,pm.SUpplierID,pm.Countryid,count(pm.SID)Completes,round((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID),3)CPI
,(round(((SELECT pm2.CPI FROM dbo.ProjectMapping pm2 WHERE pm2.ProjectID=pm.ProjectID AND pm2.SUpplierID=pm.SUpplierID AND pm2.CountryID=pm.CountryID)*count(pm.SID)),2))Cost
FROM dbo.ProjectMapping pm
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.CountryID=@countryid AND pm.SUpplierID=@supplierid AND p.ClientID=@clientid AND convert(date,sp.StartDate)<=convert(date,@startdate) AND convert(date,enddate) > = convert(date,@enddate)
GROUP BY pm.ProjectID,pm.SUpplierID,pm.CountryID
SET @totalrevenue=(SELECT sum(cost) from @SupplierTable st)
INSERT INTO @output_s
SELECT --st.sid ID
(SELECT s.Name FROM dbo.Suppliers s WHERE s.id=st.sid) Parameter
,(SELECT sum(st2.cost) FROM @SupplierTable st2 WHERE st2.sid=st.sid) revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(st2.cost),0)*100/@totalrevenue) END FROM @SupplierTable st2 WHERE st2.sid=st.sid),2)percentt
 FROM @SupplierTable st GROUP BY st.SID
select top(10)* from @output_s order by revenue desc
--date country supplier client filter supplier wise end
----------------------------------------------------------------------------------------------
--date country supplier client filter client wise start
INSERT INTO @ClientTable
SELECT pm.ProjectID PID,(SELECT p.ClientID FROM dbo.Projects p WHERE p.ProjectId=pm.projectid)CID,count(pm.sid)Complete,(SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)CPI,round(((SELECT p.CPI FROM dbo.Projects p WHERE p.ProjectId=pm.ProjectID)*count(pm.sid)),2)Cost
FROM dbo.ProjectMapping pm 
JOIN dbo.Projects p ON pm.ProjectID = p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID WHERE sp.status='Complete' AND pm.SUpplierID=@supplierid AND pm.CountryID=@countryid AND p.ClientID=@clientid AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID
SET @totalrevenue= (SELECT sum(cost) FROM @ClientTable ct)
INSERT INTO @output_cl
SELECT (SELECT c.Company FROM dbo.Clients c WHERE c.ClientId=ct.CID)Parameter
,(SELECT sum(cost) FROM @ClientTable ct2 WHERE ct2.CID=ct.CID )revenue
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct2.cost),0)*100/@totalrevenue) END FROM @ClientTable ct2 WHERE ct2.CID=ct.CID),2)percentt
FROM @ClientTable ct GROUP BY ct.CID
select top(10)* from @output_cl order by revenue desc
--date country supplier client filter client wise end
----------------------------------------------------------------------------------------------
--date country supplier client filter country wise start
INSERT INTO @CountryTable
SELECT pm.ProjectID,pm.CountryID,count(pm.SID)Completes,(SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID ) CPI,((SELECT pu.CPI FROM dbo.ProjectsUrl pu WHERE pu.PID=pm.ProjectID AND pu.CID=pm.CountryID )*count(pm.SID))Value
FROM dbo.ProjectMapping pm
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
JOIN dbo.SupplierProjects sp ON pm.SID = sp.SID
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND pm.CountryID=@countryid AND p.ClientID=@clientid AND convert(date,sp.StartDate)>=convert(date,@startdate) AND convert(date,enddate) <= convert(date,@enddate)
GROUP BY pm.ProjectID,pm.CountryID
set @totalrevenue = (SELECT sum(ct.[Value]) FROM @CountryTable ct)
insert into @output_co 
SELECT (SELECT tc.Country FROM dbo.CountryMaster tc WHERE tc.CountryId=ct.CID)
,(SELECT sum(value) FROM @CountryTable ct2 WHERE ct2.CID=ct.CID)Value
,round((SELECT case WHEN @totalrevenue=0 THEN 0 ELSE (isnull(sum(ct3.[Value]),0)*100/@totalrevenue) END FROM @CountryTable ct3 WHERE ct3.CID=ct.CID),2)percentt
FROM @CountryTable ct GROUP BY ct.CID
select top(10)* from @output_co order by revenue desc
--date country supplier client filter country wise end
-----------------------------------------------------------------------------------------------

end
end
end






