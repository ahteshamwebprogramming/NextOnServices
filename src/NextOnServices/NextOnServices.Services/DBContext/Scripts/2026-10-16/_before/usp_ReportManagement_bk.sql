CREATE proc [dbo].[usp_ReportManagement_BK] @countryid int=0,
@supplierid int=0,
@clientid int=0,
@report int=3,
@opt int=0,
@id int=0,
@startdate datetime = ''--'8/1/2018 12:00:00 AM'
,@enddate datetime= ''-- '8/31/2018 12:00:00 AM'
as
begin
--opt 1 for select
--opt 2 for insert
--report 1 for client
--report 2 for supplier
--report 3 for overall
if(@report=1)

begin

if(@opt=1)

begin

if(@id=0)

begin

select * from Clients

end

else

begin

select * from Clients where ClientId=@id

end

end

end

if(@report=2)

begin

if(@opt=1)

begin 

if(@id=0)

begin

select * from Suppliers

end

else

begin

select * from Suppliers where ID=@id

end

end

end

if(@report=3)
begin
if(@countryid=0 and @supplierid=0 and @clientid=0)
begin
if(@startdate='' and @enddate= '')
begin
--#######################without filter supplier wise start#############################
declare @temp3_ts as table(id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @temp1_ts as table (id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
INSERT INTO @temp1_ts ( completes, supplierID , suppname )
SELECT count(pm.SUpplierID) Completes , pm.SUpplierID , (SELECT s2.Name FROM dbo.Suppliers s2 WHERE id=pm.SUpplierID) SupplierName FROM dbo.Suppliers s
JOIN dbo.ProjectMapping pm ON s.id=pm.SUpplierID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
WHERE sp.Status='Complete' GROUP BY pm.SUpplierID

declare @min_ts int=1, @max_ts int,@grandtotal_ts float=0

SET @grandtotal_ts = (SELECT sum(completes) FROM @temp1_ts)
INSERT INTO @temp3_ts (supplierID , suppname , completes , percentage )
SELECT supplierID , suppname , completes , round (CASE WHEN(@grandtotal_ts = 0) THEN 0 ELSE ( isnull(completes,0)*100/isnull(@grandtotal_ts,0))end,0) FROM @temp1_ts
select top 10 * from @temp3_ts order by completes desc

--########################without filter supplier wise end########################################

--##################without filter client wise start##################################

declare @grantotal_tcl int=0
declare @finaltable_tcl as table(id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
declare @clienttable_tcl as table (id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
insert into @clienttable_tcl ( ClientID ,clieentname, complete )
SELECT c.ClientId AS ClientID ,(SELECT c2.Company FROM dbo.Clients c2 WHERE ClientId=c.ClientId) Name,count(c.ClientId) Completes  FROM dbo.Clients c JOIN dbo.Projects p ON c.ClientId=p.ClientID
JOIN dbo.ProjectMapping pm ON p.ProjectId=pm.ProjectID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID
WHERE sp.Status='Complete' GROUP BY c.ClientId

set @grantotal_tcl = (SELECT sum(complete) FROM @clienttable_tcl)
INSERT INTO @finaltable_tcl (ClientID ,clieentname, complete, percentage )
SELECT ClientID , clieentname , complete , round (CASE WHEN(@grantotal_tcl = 0) THEN 0 ELSE ( isnull(complete,0)*100/isnull(@grantotal_tcl,0))end,0) FROM @clienttable_tcl

select top 10 * from @finaltable_tcl order by complete desc

--###################without filter client wise stop###################################

--#################without filter country wise start#################################

declare @tblcountry_tco as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
declare @max_tco int,@min_tco int=1,@grandtotal_tco int=0
declare @tblCountryid_tco as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
insert into @tblCountryid_tco ( countryid,countryname,total)
SELECT tc.CountryId CountryId , (SELECT tc2.Country FROM dbo.CountryMaster tc2 WHERE CountryId=tc.CountryId ) Name , count(tc.CountryId) Complete FROM dbo.CountryMaster tc JOIN dbo.ProjectMapping pm ON tc.CountryId=pm.CountryID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
WHERE sp.Status='Complete' GROUP BY tc.CountryId
SET @grandtotal_tco  = (SELECT sum(total) FROM  @tblCountryid_tco)

INSERT INTO @tblcountry_tco (countryid ,countryname, total, percentage )
SELECT countryid , countryname , total , round (CASE WHEN(@grandtotal_tco = 0) THEN 0 ELSE ( isnull(total,0)*100/isnull(@grandtotal_tco,0))end,0) FROM @tblCountryid_tco

 select top 10 * from @tblcountry_tco order by total desc

--#######################without filter country wise stop######################################

--########################total start#######################

declare @output_tprojects1 as table (Id int identity(1,1) , Total int , Closed int, InProgress int, Onhold int, Cancelled int)

declare @total_tprojects1 int, @closed_tprojects1 int,@inprogress_tprojects1 int,@onholed_tprojects1 int,@cancelled_tprojects1 int

set @total_tprojects1=(select count(*) from Projects where not (status=5 or Status=6))

set @closed_tprojects1=(select count(*) from Projects where Status=1)

set @inprogress_tprojects1=(select count(*) from Projects where Status=2)

set @onholed_tprojects1=(select count(*) from Projects where Status=3)

set @cancelled_tprojects1=(select count(*) from Projects where Status=4)

insert into @output_tprojects1 (Total,Closed,InProgress,Onhold,Cancelled) values(@total_tprojects1,@closed_tprojects1,@inprogress_tprojects1,@onholed_tprojects1,@cancelled_tprojects1)

select * from @output_tprojects1 

--#######################total end#################################

end

else

begin

--for supplier table start

--#######################without filter supplier wise start#############################

declare @temp3_tsd3 as table(id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @temp1_tsd3 as table (id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @min_tsd3 int=1, @max_tsd3 int,@grandtotal_tsd3 int=0
insert into @temp1_tsd3 (completes , supplierID , suppname )
SELECT count(pm.SUpplierID) Completes , pm.SUpplierID , (SELECT s2.Name FROM dbo.Suppliers s2 WHERE id=pm.SUpplierID) SupplierName FROM dbo.Suppliers s
JOIN dbo.ProjectMapping pm ON s.id=pm.SUpplierID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
WHERE sp.Status='Complete' AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY pm.SUpplierID
SET @grandtotal_tsd3= (SELECT sum(completes) FROM @temp1_tsd3)
INSERT INTO @temp3_tsd3 ( supplierID ,suppname,completes,percentage)
SELECT supplierID,suppname,completes,round (CASE WHEN(@grandtotal_tsd3 = 0) THEN 0 ELSE ( isnull(completes,0)*100/isnull(@grandtotal_tsd3,0))end,0) FROM @temp1_tsd3
select top 10 * from @temp3_tsd3 order by completes desc

--########################without filter supplier wise end########################################

--##################without filter client wise start##################################

declare @grantotal_tcld3 int=0
declare @finaltable_tcld3 as table(id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
declare @clienttable_tcld3 as table (id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
insert into @clienttable_tcld3 (ClientID,clieentname,complete)
SELECT c.ClientId ,(SELECT c2.Company FROM dbo.Clients c2 WHERE c2.ClientId=c.ClientId) Name,count(c.ClientId) Completes  FROM dbo.Clients c JOIN dbo.Projects p ON c.ClientId=p.ClientID
JOIN dbo.ProjectMapping pm ON p.ProjectID=pm.ProjectID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID
WHERE sp.Status='Complete' AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY c.ClientId
SET @grantotal_tcld3 = (SELECT sum(complete) FROM @clienttable_tcld3)
INSERT INTO @finaltable_tcld3(ClientID,clieentname,complete,percentage)
SELECT ClientID,clieentname,complete,round (CASE WHEN(@grantotal_tcld3 = 0) THEN 0 ELSE ( isnull(complete,0)*100/isnull(@grantotal_tcld3,0))end,0) FROM @clienttable_tcld3

select top 10 * from @finaltable_tcld3 order by complete desc

--###################without filter client wise stop###################################
--#################without filter country wise start#################################
declare @max_tcod3 int,@min_tcod3 int=1,@grandtotal_tcod3 int=0
declare @tblcountry_tcod3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
declare @tblCountryid_tcod3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
insert into @tblCountryid_tcod3(countryid,countryname,total)
SELECT tc.CountryId CountryId , (SELECT tc2.Country FROM dbo.CountryMaster tc2 WHERE CountryId=tc.CountryId ) Name , count(tc.CountryId) Complete FROM dbo.CountryMaster tc JOIN dbo.ProjectMapping pm ON tc.CountryId=pm.CountryID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
WHERE sp.Status='Complete' AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
 GROUP BY tc.CountryId
 SET @grandtotal_tcod3 = (SELECT sum(total) FROM @tblCountryid_tcod3)
 INSERT INTO @tblcountry_tcod3( countryid,countryname,total,percentage )
SELECT countryid , countryname , total , round (CASE WHEN(@grandtotal_tcod3 = 0) THEN 0 ELSE ( isnull(total,0)*100/isnull(@grandtotal_tcod3,0))end,0) FROM @tblCountryid_tcod3

 select top 10 * from @tblcountry_tcod3 order by total desc
--#######################without filter country wise stop######################################

--########################without filter total start###############################

declare @output_tprojects1d1 as table (Id int identity(1,1) , Total int , Closed int, InProgress int, Onhold int, Cancelled int)

declare @total_tprojects1d1 int, @closed_tprojects1d1 int,@inprogress_tprojects1d1 int,@onholed_tprojects1d1 int,@cancelled_tprojects1d1 int

set @total_tprojects1d1=(select count(*) from Projects where not (status=5 or status=6) and convert(datetime,(SDate))>=@startdate and convert(datetime,(EDate))<=@enddate)

set @closed_tprojects1d1=(select count(*) from Projects where Status=1 and convert(datetime,(SDate))>=@startdate and convert(datetime,(EDate))<=@enddate)

set @inprogress_tprojects1d1=(select count(*) from Projects where Status=2 and convert(datetime,(SDate))>=@startdate and convert(datetime,(EDate))<=@enddate)

set @onholed_tprojects1d1=(select count(*) from Projects where Status=3 and convert(datetime,(SDate))>=@startdate and convert(datetime,(EDate))<=@enddate)

set @cancelled_tprojects1d1=(select count(*) from Projects where Status=0 and convert(datetime,(SDate))>=@startdate and convert(datetime,(EDate))<=@enddate)

insert into @output_tprojects1d1 (Total,Closed,InProgress,Onhold,Cancelled) values(@total_tprojects1d1,@closed_tprojects1d1,@inprogress_tprojects1d1,@onholed_tprojects1d1,@cancelled_tprojects1d1)

select * from @output_tprojects1d1 

--########################without filter total end########################################



end

end
--country filter
if(@countryid!=0 and @supplierid=0 and @clientid=0)
begin
if(@startdate='' and @enddate='')
begin
--######### country filter supplier wise start############
declare @temp3_ts1 as table(id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @temp1_ts1 as table (id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @min_ts1 int=1, @max_ts1 int,@grandtotal_ts1 int=0
insert into @temp1_ts1(completes,supplierID,suppname)
SELECT count(pm.SUpplierID) Completes , pm.SUpplierID , (SELECT s2.Name FROM dbo.Suppliers s2 WHERE id=pm.SUpplierID) SupplierName FROM dbo.Suppliers s
JOIN dbo.ProjectMapping pm ON s.id=pm.SUpplierID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
WHERE sp.Status='Complete' AND pm.CountryID=@countryid   
GROUP BY pm.SUpplierID
SET @grandtotal_ts1= (SELECT sum(completes) FROM @temp1_ts1)
INSERT INTO @temp3_ts1(supplierID,suppname,completes,percentage)
SELECT supplierID,suppname,completes,round (CASE WHEN(@grandtotal_ts1 = 0) THEN 0 ELSE ( isnull(completes,0)*100/isnull(@grandtotal_ts1,0))end,0) FROM @temp1_ts1
select top 10 * from @temp3_ts1 order by completes desc
--############### country filetr supplier wise end################
--#################client wise start###############################
declare @grantotal_tcl1 float=0
declare @finaltable_tcl1 as table(id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
declare @clienttable_tcl1 as table (id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
insert into @clienttable_tcl1 (ClientID,clieentname,complete)
SELECT c.ClientId ,(SELECT c2.Company FROM dbo.Clients c2 WHERE c2.ClientId=c.ClientId) Name,count(c.ClientId) Completes  FROM dbo.Clients c JOIN dbo.Projects p ON c.ClientId=p.ClientID
JOIN dbo.ProjectMapping pm ON p.ProjectID=pm.ProjectID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID
WHERE sp.Status='Complete' AND pm.CountryID=@countryid 
GROUP BY c.ClientId
SET @grantotal_tcl1  = (SELECT sum(complete) FROM @clienttable_tcl1)
INSERT INTO @finaltable_tcl1 (ClientID,clieentname,complete,percentage)
SELECT ClientID,clieentname,complete,round (CASE WHEN(@grantotal_tcl1 = 0) THEN 0 ELSE ( isnull(complete,0)*100/isnull(@grantotal_tcl1,0))end,0) FROM  @clienttable_tcl1
select top 10 * from @finaltable_tcl1 order by complete desc
--#################client wise end####################
--#########country wise start###########
declare @tblcountry_tco1 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
declare @max_tco1 int,@min_tco1 int=1,@grandtotal_tco1 float=0
declare @tblCountryid_tco1 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
insert into @tblCountryid_tco1(countryid,countryname,total)
SELECT tc.CountryId CountryId , (SELECT tc2.Country FROM dbo.CountryMaster tc2 WHERE CountryId=tc.CountryId ) Name , count(tc.CountryId) Complete FROM dbo.CountryMaster tc JOIN dbo.ProjectMapping pm ON tc.CountryId=pm.CountryID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
WHERE sp.Status='Complete' AND pm.CountryID=@countryid
 GROUP BY tc.CountryId
SET @grandtotal_tco1 = (SELECT sum(total) FROM @tblCountryid_tco1)

INSERT INTO @tblcountry_tco1 ( countryid,countryname,total,percentage )
SELECT countryid,countryname,total,round (CASE WHEN(@grandtotal_tco1 = 0) THEN 0 ELSE ( isnull(total,0)*100/isnull(@grandtotal_tco1,0))end,0) FROM @tblCountryid_tco1
 select top 10 * from @tblcountry_tco1 order by total desc
--###########country wise end#################

--############total#####################

declare @output_tprojects2 as table (Id int identity(1,1) , Total int , Closed int, InProgress int, Onhold int, Cancelled int)

declare @total_tprojects2 int, @closed_tprojects2 int,@inprogress_tprojects2 int,@onholed_tprojects2 int,@cancelled_tprojects2 int

set @total_tprojects2=(select count(*) from Projects where not (status=5 or status=6) and CountryId=@countryid)

set @closed_tprojects2=(select count(*) from Projects where Status=1 and CountryId=@countryid)

set @inprogress_tprojects2=(select count(*) from Projects where Status=2 and CountryId=@countryid)

set @onholed_tprojects2=(select count(*) from Projects where Status=3 and CountryId=@countryid)

set @cancelled_tprojects2=(select count(*) from Projects where Status=0 and CountryId=@countryid)

insert into @output_tprojects2 (Total,Closed,InProgress,Onhold,Cancelled) values(@total_tprojects2,@closed_tprojects2,@inprogress_tprojects2,@onholed_tprojects2,@cancelled_tprojects2)

select * from @output_tprojects2  

--############total######################

end

else

 begin
--######### country filter supplier wise start############
declare @temp3_ts1d3 as table(id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @temp1_ts1d3 as table (id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @min_ts1d3 int=1, @max_ts1d3 int,@grandtotal_ts1d3 float=0
insert into @temp1_ts1d3(completes,supplierID,suppname)
SELECT count(pm.SUpplierID) Completes , pm.SUpplierID , (SELECT s2.Name FROM dbo.Suppliers s2 WHERE id=pm.SUpplierID) SupplierName FROM dbo.Suppliers s
JOIN dbo.ProjectMapping pm ON s.id=pm.SUpplierID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
WHERE sp.Status='Complete' AND pm.CountryID=@countryid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY pm.SUpplierID
SET @grandtotal_ts1d3 = (SELECT sum(completes) FROM @temp1_ts1d3)
INSERT INTO @temp3_ts1d3(supplierID,suppname,completes,percentage)
SELECT supplierID,suppname,completes,round (CASE WHEN(@grandtotal_ts1d3 = 0) THEN 0 ELSE ( isnull(completes,0)*100/isnull(@grandtotal_ts1d3,0))end,0) FROM @temp1_ts1d3
select top 10 * from @temp3_ts1d3 order by completes desc

--############### country filetr supplier wise end################
--#################client wise start###############################
declare @grantotal_tcl1d3 float=0
declare @finaltable_tcl1d3 as table(id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
declare @clienttable_tcl1d3 as table (id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
insert into @clienttable_tcl1d3(ClientID,clieentname,complete)
SELECT c.ClientId ,(SELECT c2.Company FROM dbo.Clients c2 WHERE ClientId=c.ClientId) Name,count(c.ClientId) Completes  FROM dbo.Clients c JOIN dbo.Projects p ON c.ClientId=p.ClientID
JOIN dbo.ProjectMapping pm ON p.ProjectID=pm.ProjectID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID
WHERE sp.Status='Complete' AND pm.CountryID=@countryid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY c.ClientId
SET @grantotal_tcl1d3=(SELECT sum(complete) FROM @clienttable_tcl1d3)
INSERT INTO @finaltable_tcl1d3(ClientID  , clieentname , complete , percentage )
SELECT ClientID  , clieentname , complete , round (CASE WHEN(@grantotal_tcl1d3 = 0) THEN 0 ELSE ( isnull(complete,0)*100/isnull(@grantotal_tcl1d3,0))end,0) FROM @clienttable_tcl1d3
select top 10 * from @finaltable_tcl1d3 order by complete desc
--#################client wise end####################
--#########country wise start###########
declare @tblcountry_tco1d3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
declare @max_tco1d3 int,@min_tco1d3 int=1,@grandtotal_tco1d3 float=0
declare @tblCountryid_tco1d3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
insert into @tblCountryid_tco1d3(countryid,countryname,total)
SELECT tc.CountryId CountryId , (SELECT tc2.Country FROM dbo.CountryMaster tc2 WHERE Countryid=tc.Countryid ) Name , count(tc.CountryId) Complete FROM dbo.CountryMaster tc JOIN dbo.ProjectMapping pm ON tc.CountryId=pm.CountryID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
WHERE sp.Status='Complete' AND pm.CountryID=@countryid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
 GROUP BY tc.CountryId
 SET @grandtotal_tco1d3 = (SELECT sum(total) FROM @tblCountryid_tco1d3 )
 INSERT INTO @tblcountry_tco1d3(countryid,countryname,total,percentage)
 SELECT countryid,countryname,total,round (CASE WHEN(@grandtotal_tco1d3 = 0) THEN 0 ELSE ( isnull(total,0)*100/isnull(@grandtotal_tco1d3,0))end,0) FROM @tblCountryid_tco1d3
 select top 10 * from @tblcountry_tco1d3 order by total desc
--###########country wise end#################

--#############total start############################
declare @output_tprojects2d1 as table (Id int identity(1,1) , Total int , Closed int, InProgress int, Onhold int, Cancelled int)
declare @total_tprojects2d1 int, @closed_tprojects2d1 int,@inprogress_tprojects2d1 int,@onholed_tprojects2d1 int,@cancelled_tprojects2d1 int
set @total_tprojects2d1=(select count(*) from Projects where not (status=5 or status=6) and CountryId=@countryid and (convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @closed_tprojects2d1=(select count(*) from Projects where Status=1 and CountryId=@countryid and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @inprogress_tprojects2d1=(select count(*) from Projects where Status=2 and CountryId=@countryid and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @onholed_tprojects2d1=(select count(*) from Projects where Status=3 and CountryId=@countryid and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @cancelled_tprojects2d1=(select count(*) from Projects where Status=0 and CountryId=@countryid and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

insert into @output_tprojects2d1 (Total,Closed,InProgress,Onhold,Cancelled) values(@total_tprojects2d1,@closed_tprojects2d1,@inprogress_tprojects2d1,@onholed_tprojects2d1,@cancelled_tprojects2d1)

select * from @output_tprojects2d1  
--###############total end##################################

 end

end

--supplier filter

if(@countryid=0 and @supplierid!=0 and @clientid=0)

begin

if(@startdate='' and @enddate='')

begin

--######################supplier filter supplier wise start#######################################

declare @temp3_ts2 as table(id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @temp1_ts2 as table (id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @min_ts2 int=1, @max_ts2 int,@grandtotal_ts2 int=0
insert into @temp1_ts2( completes,supplierID,suppname )
SELECT count(pm.SUpplierID) Completes , pm.SUpplierID , (SELECT s2.Name FROM dbo.Suppliers s2 WHERE id=pm.SUpplierID) SupplierName FROM dbo.Suppliers s
JOIN dbo.ProjectMapping pm ON s.id=pm.SUpplierID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid
GROUP BY pm.SUpplierID
SET @grandtotal_ts2=(SELECT sum(completes) FROM @temp1_ts2 )
INSERT INTO @temp3_ts2 (supplierID,suppname,completes,percentage)
SELECT supplierID,suppname,completes,round (CASE WHEN(@grandtotal_ts2 = 0) THEN 0 ELSE ( isnull(completes,0)*100/isnull(@grandtotal_ts2,0))end,0) FROM @temp1_ts2
select top 10 * from @temp3_ts2 order by completes desc
--######################supplier filter supplier wise stop###############################
--#####################client wise start#####################################
declare @grantotal_tcl2 float=0
declare @finaltable_tcl2 as table(id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
declare @clienttable_tcl2 as table (id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
insert into @clienttable_tcl2(ClientID,clieentname,complete)
SELECT c.ClientId ,(SELECT c2.Company FROM dbo.Clients c2 WHERE ClientId=c.ClientId) Name,count(c.ClientId) Completes  FROM dbo.Clients c JOIN dbo.Projects p ON c.ClientId=p.ClientID
JOIN dbo.ProjectMapping pm ON p.ProjectID=pm.ProjectID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid
GROUP BY c.ClientId
SET @grantotal_tcl2 = ( SELECT sum(complete) from @clienttable_tcl2 )
INSERT INTO @finaltable_tcl2 (ClientID,clieentname,complete,percentage)
SELECT ClientID,clieentname,complete,round (CASE WHEN(@grantotal_tcl2 = 0) THEN 0 ELSE ( isnull(complete,0)*100/isnull(@grantotal_tcl2,0))end,0) FROM @clienttable_tcl2
select top 10 * from @finaltable_tcl2 order by complete desc
--#####################client wise stop #################
--############country wise start#########################
declare @tblcountry_tco2 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
declare @max_tco2 int,@min_tco2 int=1,@grandtotal_tco2 float=0
declare @tblCountryid_tco2 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
insert into @tblCountryid_tco2(countryid,countryname,total)
SELECT tc.CountryId CountryId , (SELECT tc2.Country FROM dbo.CountryMaster tc2 WHERE CountryId=tc.CountryId ) Name , count(tc.CountryId) Complete FROM dbo.CountryMaster tc JOIN dbo.ProjectMapping pm ON tc.CountryId=pm.CountryID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid
 GROUP BY tc.CountryId
 SET @grandtotal_tco2= (SELECT sum(total) FROM @tblCountryid_tco2)
 INSERT INTO @tblcountry_tco2(countryid,countryname,total,percentage)
 SELECT countryid,countryname,total,round (CASE WHEN(@grandtotal_tco2 = 0) THEN 0 ELSE ( isnull(total,0)*100/isnull(@grandtotal_tco2,0))end,0) FROM @tblCountryid_tco2
 select top 10 * from @tblcountry_tco2 order by total desc
--###############country wise stop#########################
--###############total#################
declare @output_tprojects3 as table (Id int identity(1,1) , Total int , Closed int, InProgress int, Onhold int, Cancelled int)
declare @total_tprojects3 int=0, @closed_tprojects3 int=0,@inprogress_tprojects3 int=0,@onholed_tprojects3 int=0,@cancelled_tprojects3 int=0
--select * from ProjectMapping
declare @projecttable_tprojects3 as table(Id int identity(1,1), ProjectId int,CountryId int)

insert into @projecttable_tprojects3

select ProjectID,CountryID from ProjectMapping where SUpplierID=@supplierid

declare @max_tprojects3 int=(select count(*) from @projecttable_tprojects3),@min__tprojects3 int=1

while(@min__tprojects3<=@max_tprojects3)

begin

declare @pid__tprojects3 int

set @pid__tprojects3=(select ProjectId from @projecttable_tprojects3 where Id=@min__tprojects3)

set @total_tprojects3= @total_tprojects3+(select count(*) from Projects where not (status=5 or status=6) and ProjectId=@pid__tprojects3)

set @closed_tprojects3= @closed_tprojects3+ (select count(*) from Projects where Status=1 and ProjectId=@pid__tprojects3)

set @inprogress_tprojects3= @inprogress_tprojects3+ (select count(*) from Projects where Status=2 and ProjectId=@pid__tprojects3)

set @onholed_tprojects3= @onholed_tprojects3+ (select count(*) from Projects where Status=3 and ProjectId=@pid__tprojects3)

set @cancelled_tprojects3=@cancelled_tprojects3+ (select count(*) from Projects where Status=0 and ProjectId=@pid__tprojects3)

set @min__tprojects3=@min__tprojects3+1

end

insert into @output_tprojects3 (Total,Closed,InProgress,Onhold,Cancelled) values(@total_tprojects3,@closed_tprojects3,@inprogress_tprojects3,@onholed_tprojects3,@cancelled_tprojects3)

select * from @output_tprojects3 

--#############total########################## 











end

else

begin

--######################supplier filter supplier wise start#######################################

declare @temp3_ts2d3 as table(id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @temp1_ts2d3 as table (id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @min_ts2d3 int=1, @max_ts2d3 int,@grandtotal_ts2d3 float=0
insert into @temp1_ts2d3(completes,supplierID,suppname)
SELECT count(pm.SUpplierID) Completes , pm.SUpplierID , (SELECT s2.Name FROM dbo.Suppliers s2 WHERE id=pm.SUpplierID) SupplierName FROM dbo.Suppliers s
JOIN dbo.ProjectMapping pm ON s.id=pm.SUpplierID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY pm.SUpplierID
SET @grandtotal_ts2d3= (SELECT sum(completes) FROM @temp1_ts2d3)
INSERT INTO @temp3_ts2d3(supplierID,suppname,completes,percentage)
SELECT supplierID,suppname,completes,round (CASE WHEN(@grandtotal_ts2d3 = 0) THEN 0 ELSE ( isnull(completes,0)*100/isnull(@grandtotal_ts2d3,0))end,0) FROM @temp1_ts2d3

select top 10 * from @temp3_ts2d3 order by completes desc
--######################supplier filter supplier wise stop###############################

--#####################client wise start#####################################
declare @grantotal_tcl2d3 float=0
declare @finaltable_tcl2d3 as table(id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
declare @clienttable_tcl2d3 as table (id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
insert into @clienttable_tcl2d3(ClientID,clieentname,complete)
SELECT c.ClientID ,(SELECT c2.Company FROM dbo.Clients c2 WHERE ClientID=c.ClientID) Name,count(c.ClientID) Completes  FROM dbo.Clients c JOIN dbo.Projects p ON c.ClientID=p.ClientID
JOIN dbo.ProjectMapping pm ON p.ProjectID=pm.ProjectID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY c.ClientID
SET @grantotal_tcl2d3=(SELECT sum(complete) FROM @clienttable_tcl2d3)
INSERT INTO @finaltable_tcl2d3(ClientID,clieentname,complete,percentage)
SELECT ClientID,clieentname,complete,round (CASE WHEN(@grantotal_tcl2d3 = 0) THEN 0 ELSE ( isnull(complete,0)*100/isnull(@grantotal_tcl2d3,0))end,0) from @clienttable_tcl2d3

select top 10 * from @finaltable_tcl2d3 order by complete desc
--#####################client wise stop #################

--############country wise start#########################
declare @tblcountry_tco2d3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
declare @max_tco2d3 int,@min_tco2d3 int=1,@grandtotal_tco2d3 float=0
declare @tblCountryid_tco2d3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
insert into @tblCountryid_tco2d3(countryid , countryname , total )
SELECT tc.CountryId CountryId , (SELECT tc2.Country FROM dbo.CountryMaster tc2 WHERE CountryId=tc.CountryId ) Name , count(tc.CountryId) Complete FROM dbo.CountryMaster tc JOIN dbo.ProjectMapping pm ON tc.CountryId=pm.CountryID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
 GROUP BY tc.CountryId
SET @grandtotal_tco2d3=(SELECT sum(total) FROM @tblCountryid_tco2d3)
INSERT INTO @tblcountry_tco2d3(countryid , countryname , total,percentage)
SELECT countryid , countryname , total,round (CASE WHEN(@grandtotal_tco2d3 = 0) THEN 0 ELSE ( isnull(total,0)*100/isnull(@grandtotal_tco2d3,0))end,0) FROM @tblCountryid_tco2d3

 select top 10 * from @tblcountry_tco2d3 order by total desc

 --###############country wise stop#########################
--###############total#####################

declare @output_tprojects3d1 as table (Id int identity(1,1) , Total int , Closed int, InProgress int, Onhold int, Cancelled int)

declare @total_tprojects3d1 int=0, @closed_tprojects3d1 int=0,@inprogress_tprojects3d1 int=0,@onholed_tprojects3d1 int=0,@cancelled_tprojects3d1 int=0

--select * from ProjectMapping

declare @projecttable_tprojects3d1 as table(Id int identity(1,1), ProjectId int,CountryId int)

insert into @projecttable_tprojects3d1

select ProjectID,CountryID from ProjectMapping where SUpplierID=@supplierid

declare @max_tprojects3d1 int=(select count(*) from @projecttable_tprojects3d1),@min__tprojects3d1 int=1

while(@min__tprojects3d1<=@max_tprojects3d1)

begin

declare @pid__tprojects3d1 int

set @pid__tprojects3d1=(select ProjectId from @projecttable_tprojects3d1 where Id=@min__tprojects3d1)

set @total_tprojects3d1= @total_tprojects3d1+(select count(*) from Projects where not (status=5 or status=6) and ProjectID=@pid__tprojects3d1 and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @closed_tprojects3d1= @closed_tprojects3d1+ (select count(*) from Projects where Status=1 and ProjectID=@pid__tprojects3d1 and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @inprogress_tprojects3d1= @inprogress_tprojects3d1+ (select count(*) from Projects where Status=2 and ProjectID=@pid__tprojects3d1 and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @onholed_tprojects3d1= @onholed_tprojects3d1+ (select count(*) from Projects where Status=3 and ProjectID=@pid__tprojects3d1 and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @cancelled_tprojects3d1=@cancelled_tprojects3d1+ (select count(*) from Projects where Status=0 and ProjectID=@pid__tprojects3d1 and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @min__tprojects3d1=@min__tprojects3d1+1

end

insert into @output_tprojects3d1 (Total,Closed,InProgress,Onhold,Cancelled) values(@total_tprojects3d1,@closed_tprojects3d1,@inprogress_tprojects3d1,@onholed_tprojects3d1,@cancelled_tprojects3d1)

select * from @output_tprojects3d1 



--################total######################
end
end
--Client filter
if(@countryid=0 and @supplierid=0 and @clientid!=0)
begin
if(@startdate='' and @enddate='')
begin

--#####supplier wise start###
declare @temp3_ts3 as table(id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @temp1_ts3 as table (id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @min_ts3 int=1, @max_ts3 int,@grandtotal_ts3 float=0
insert into @temp1_ts3(completes,supplierID,suppname)
SELECT count(pm.SUpplierID) Completes , pm.SUpplierID , (SELECT s2.Name FROM dbo.Suppliers s2 WHERE id=pm.SUpplierID) SupplierName FROM dbo.Suppliers s
JOIN dbo.ProjectMapping pm ON s.id=pm.SUpplierID JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid JOIN dbo.Projects p ON pm.ProjectID=p.ProjectID WHERE sp.Status='Complete' AND p.ClientID=@clientid GROUP BY pm.SUpplierID
SET @grandtotal_ts3 = (SELECT sum(completes) FROM @temp1_ts3)
INSERT INTO @temp3_ts3(supplierID,suppname,completes,percentage)
SELECT supplierID,suppname,completes,round (CASE WHEN(@grandtotal_ts3 = 0) THEN 0 ELSE ( isnull(completes,0)*100/isnull(@grandtotal_ts3,0))end,0) FROM @temp1_ts3

select top 10 * from @temp3_ts3 order by completes
--########supplier wise end##############
--########client wise start######
declare @grantotal_tcl3 float=0
declare @finaltable_tcl3 as table(id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
declare @clienttable_tcl3 as table (id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
insert into @clienttable_tcl3(ClientID,clieentname,complete)
SELECT c.ClientID ,(SELECT c2.Company FROM dbo.Clients c2 WHERE ClientID=c.ClientID) Name,count(c.ClientID) Completes  FROM dbo.Clients c JOIN dbo.Projects p ON c.ClientID=p.ClientID
JOIN dbo.ProjectMapping pm ON p.ProjectID=pm.ProjectID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID
WHERE sp.Status='Complete' AND p.ClientID=@clientid
GROUP BY c.ClientID
SET @grantotal_tcl3 = (SELECT sum(complete) FROM @clienttable_tcl3)
INSERT INTO @finaltable_tcl3(ClientID,clieentname,complete,percentage)
SELECT ClientID,clieentname,complete,round (CASE WHEN(@grantotal_tcl3 = 0) THEN 0 ELSE ( isnull(complete,0)*100/isnull(@grantotal_tcl3,0))end,0) FROM @clienttable_tcl3

select top 10 * from @finaltable_tcl3 order by complete desc
--#########client wise end######################
--###########country wise start########################
declare @tblcountry_tco3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
declare @max_tco3 int,@min_tco3 int=1,@grandtotal_tco3 float=0
declare @tblCountryid_tco3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
insert into @tblCountryid_tco3(countryid,countryname,total)
SELECT tc.CountryId CountryId , (SELECT tc2.Country FROM dbo.CountryMaster tc2 WHERE CountryId=tc.CountryId) Name , count(tc.CountryId) Complete FROM dbo.CountryMaster tc JOIN dbo.ProjectMapping pm ON tc.CountryId=pm.CountryID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectID
WHERE sp.Status='Complete' AND p.ClientID=@clientid 
 GROUP BY tc.CountryId
 SET @grandtotal_tco3 = (SELECT sum(total) FROM @tblCountryid_tco3)
 INSERT INTO @tblcountry_tco3(countryid,countryname,total,percentage)
 SELECT countryid,countryname,total,round (CASE WHEN(@grandtotal_tco3 = 0) THEN 0 ELSE ( isnull(total,0)*100/isnull(@grandtotal_tco3,0))end,0) FROM @tblCountryid_tco3

 select top 10 * from @tblcountry_tco3 order by total desc

--############country wise end############################





--##############total###################

declare @output_tprojects4 as table (Id int identity(1,1) , Total int , Closed int, InProgress int, Onhold int, Cancelled int)

declare @total_tprojects4 int, @closed_tprojects4 int,@inprogress_tprojects4 int,@onholed_tprojects4 int,@cancelled_tprojects4 int

set @total_tprojects4=(select count(*) from Projects where not (status=5 or status=6) and ClientID=@clientid)

set @closed_tprojects4=(select count(*) from Projects where Status=1 and ClientID=@clientid)

set @inprogress_tprojects4=(select count(*) from Projects where Status=2 and ClientID=@clientid)

set @onholed_tprojects4=(select count(*) from Projects where Status=3 and ClientID=@clientid)

set @cancelled_tprojects4=(select count(*) from Projects where Status=0 and ClientID=@clientid)

insert into @output_tprojects4 (Total,Closed,InProgress,Onhold,Cancelled) values(@total_tprojects4,@closed_tprojects4,@inprogress_tprojects4,@onholed_tprojects4,@cancelled_tprojects4)

select * from @output_tprojects4 

--################total######################

end
else
begin

--#####supplier wise start###

declare @temp3_ts3d3 as table(id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @temp1_ts3d3 as table (id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @min_ts3d3 int=1, @max_ts3d3 int,@grandtotal_ts3d3 float=0
insert into @temp1_ts3d3(completes,supplierID,suppname)
SELECT count(pm.SUpplierID) Completes , pm.SUpplierID , (SELECT s2.Name FROM dbo.Suppliers s2 WHERE id=pm.SUpplierID) SupplierName FROM dbo.Suppliers s
JOIN dbo.ProjectMapping pm ON s.id=pm.SUpplierID JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid JOIN dbo.Projects p ON pm.ProjectID=p.ProjectID WHERE sp.Status='Complete' AND p.ClientID=@clientid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY pm.SUpplierID
SET @grandtotal_ts3d3 = (SELECT sum(completes) FROM @temp1_ts3d3)
INSERT INTO @temp3_ts3d3(completes,supplierID,suppname,percentage)
SELECT completes,supplierID,suppname,round (CASE WHEN(@grandtotal_ts3d3 = 0) THEN 0 ELSE ( isnull(completes,0)*100/isnull(@grandtotal_ts3d3,0))end,0) FROM @temp1_ts3d3
select top 10 * from @temp3_ts3d3 order by completes desc
--########supplier wise end##############

--########client wise start######
declare @grantotal_tcl3d3 float=0
declare @finaltable_tcl3d3 as table(id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
declare @clienttable_tcl3d3 as table (id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
insert into @clienttable_tcl3d3(ClientID,clieentname,complete)
SELECT c.ClientID ,(SELECT c2.Company FROM dbo.Clients c2 WHERE ClientID=c.ClientID) Name,count(c.ClientID) Completes  FROM dbo.Clients c JOIN dbo.Projects p ON c.ClientID=p.ClientID
JOIN dbo.ProjectMapping pm ON p.ProjectID=pm.ProjectID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID
WHERE sp.Status='Complete' AND p.ClientID=@clientid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY c.ClientID
SET @grantotal_tcl3d3 = (SELECT sum(complete) FROM @clienttable_tcl3d3)
INSERT INTO @finaltable_tcl3d3(ClientID,clieentname,complete,percentage)
SELECT ClientID,clieentname,complete,round (CASE WHEN(@grantotal_tcl3d3 = 0) THEN 0 ELSE ( isnull(complete,0)*100/isnull(@grantotal_tcl3d3,0))end,0) FROM @clienttable_tcl3d3

select top 10 * from @finaltable_tcl3d3 order by complete desc
--#########client wise end######################

--###########country wise start########################
declare @tblcountry_tco3d3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
declare @max_tco3d3 int,@min_tco3d3 int=1,@grandtotal_tco3d3 float=0
declare @tblCountryid_tco3d3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
insert into @tblCountryid_tco3d3(countryid,countryname,total)
SELECT tc.CountryId CountryId , (SELECT tc2.Country FROM dbo.CountryMaster tc2 WHERE CountryId=tc.CountryId ) Name , count(tc.CountryId) Complete FROM dbo.CountryMaster tc JOIN dbo.ProjectMapping pm ON tc.CountryId=pm.CountryID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
WHERE sp.Status='Complete' AND p.ClientID=@clientid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
 GROUP BY tc.CountryId
SET @grandtotal_tco3d3 = (SELECT sum(total) FROM @tblCountryid_tco3d3)
INSERT INTO @tblcountry_tco3d3(countryid,countryname,total,percentage)
SELECT countryid,countryname,total,round (CASE WHEN(@grandtotal_tco3d3 = 0) THEN 0 ELSE ( isnull(total,0)*100/isnull(@grandtotal_tco3d3,0))end,0) FROM @tblCountryid_tco3d3

select top 10 * from @tblcountry_tco3d3 order by total desc

--############country wise end############################

--##################total###########################
declare @output_tprojects4d1 as table (Id int identity(1,1) , Total int , Closed int, InProgress int, Onhold int, Cancelled int)

declare @total_tprojects4d1 int, @closed_tprojects4d1 int,@inprogress_tprojects4d1 int,@onholed_tprojects4d1 int,@cancelled_tprojects4d1 int

set @total_tprojects4d1=(select count(*) from Projects where not (status=5 or status=6) and ClientID=@clientid and (Convert(date,SDate))>=convert(date,@startdate) and (Convert(date,EDate))<=convert(date,@enddate))

set @closed_tprojects4d1=(select count(*) from Projects where Status=1 and ClientID=@clientid and (Convert(datetime,SDate))>=@startdate and (convert(datetime,EDate))<=@enddate)

set @inprogress_tprojects4d1=(select count(*) from Projects where Status=2 and ClientID=@clientid and (convert(datetime,SDate))>=@startdate and (convert(datetime,EDate))<=@enddate)

set @onholed_tprojects4d1=(select count(*) from Projects where Status=3 and ClientID=@clientid and (convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @cancelled_tprojects4d1=(select count(*) from Projects where Status=0 and ClientID=@clientid and (convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

insert into @output_tprojects4d1 (Total,Closed,InProgress,Onhold,Cancelled) values(@total_tprojects4d1,@closed_tprojects4d1,@inprogress_tprojects4d1,@onholed_tprojects4d1,@cancelled_tprojects4d1)

select * from @output_tprojects4d1  





--##################total#########################



end
end
--country and supplier filter
if(@countryid!=0 and @supplierid!=0 and @clientid=0)
begin
if(@startdate='' and @enddate='')
begin
--###########supplier wise start##################
declare @temp3_ts4 as table(id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @temp1_ts4 as table (id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @min_ts4 int=1, @max_ts4 int,@grandtotal_ts4 float=0
insert into @temp1_ts4(completes,supplierID,suppname)
SELECT count(pm.SUpplierID) Completes , pm.SUpplierID , (SELECT s2.Name FROM dbo.Suppliers s2 WHERE id=pm.SUpplierID) SupplierName FROM dbo.Suppliers s
JOIN dbo.ProjectMapping pm ON s.id=pm.SUpplierID JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND pm.CountryID=@countryid GROUP BY pm.SUpplierID
SET @grandtotal_ts4= (SELECT sum(completes) FROM @temp1_ts4)
INSERT INTO @temp3_ts4(completes,supplierID,suppname,percentage)
SELECT completes,supplierID,suppname, round (CASE WHEN(@grandtotal_ts4 = 0) THEN 0 ELSE ( isnull(completes,0)*100/isnull(@grandtotal_ts4,0))end,0) FROM @temp1_ts4
select top 10 * from @temp3_ts4 order by completes desc
--###########supplier wise end##################
--############client wise start####################
declare @grantotal_tcl4 float=0
declare @finaltable_tcl4 as table(id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
declare @clienttable_tcl4 as table (id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
insert into @clienttable_tcl4(ClientID,clieentname,complete)
SELECT c.ClientID ,(SELECT c2.Company FROM dbo.Clients c2 WHERE ClientID=c.ClientID) Name,count(c.ClientID) Completes  FROM dbo.Clients c JOIN dbo.Projects p ON c.ClientID=p.ClientID
JOIN dbo.ProjectMapping pm ON p.ProjectID=pm.ProjectID JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND pm.CountryID=@countryid 
GROUP BY c.ClientID
SET @grantotal_tcl4 = (SELECT sum(complete) FROM @clienttable_tcl4)
INSERT INTO @finaltable_tcl4(ClientID,clieentname,complete,percentage)
SELECT ClientID,clieentname,complete,round (CASE WHEN(@grantotal_tcl4 = 0) THEN 0 ELSE ( isnull(complete,0)*100/isnull(@grantotal_tcl4,0))end,0) FROM @clienttable_tcl4

select top 10 * from @finaltable_tcl4 order by complete desc
--##############client wise end###########################
--###############country and supplier filter#country wise start###################################
declare @tblcountry_tco4 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
declare @max_tco4 int,@min_tco4 int=1,@grandtotal_tco4 float=0
declare @tblCountryid_tco4 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
insert into @tblCountryid_tco4(countryid,countryname,total)
SELECT tc.CountryID CountryId , (SELECT tc2.Country FROM dbo.CountryMaster tc2 WHERE CountryID=tc.CountryID ) Name , count(tc.CountryID) Complete FROM dbo.CountryMaster tc JOIN dbo.ProjectMapping pm ON tc.CountryID=pm.CountryID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND pm.CountryID=@countryid
 GROUP BY tc.CountryID
SET @grandtotal_tco4 = (SELECT sum(total) FROM @tblCountryid_tco4)
INSERT INTO @tblcountry_tco4(countryid,countryname,total,percentage)
SELECT countryid,countryname,total,round (CASE WHEN(@grandtotal_tco4 = 0) THEN 0 ELSE ( isnull(total,0)*100/isnull(@grandtotal_tco4,0))end,0) FROM @tblCountryid_tco4

select top 10 * from @tblcountry_tco4 order by total desc
--###############country and supplier filter#country wise end###################################
--################total##################

declare @output_tprojects5 as table (Id int identity(1,1) , Total int , Closed int, InProgress int, Onhold int, Cancelled int)

declare @total_tprojects5 int=0, @closed_tprojects5 int=0,@inprogress_tprojects5 int=0,@onholed_tprojects5 int=0,@cancelled_tprojects5 int=0

--select * from ProjectMapping

declare @projecttable_tprojects5 as table(Id int identity(1,1), ProjectId int,CountryId int)

insert into @projecttable_tprojects5

select ProjectID,CountryID from ProjectMapping where SUpplierID=@supplierid and CountryID=@countryid

declare @max_tprojects5 int=(select count(*) from @projecttable_tprojects5),@min__tprojects5 int=1

while(@min__tprojects5<=@max_tprojects5)

begin

declare @pid__tprojects5 int

set @pid__tprojects5=(select ProjectId from @projecttable_tprojects5 where Id=@min__tprojects5)

set @total_tprojects5= @total_tprojects5+(select count(*) from Projects  where not (status=5 or status=6) and ProjectId=@pid__tprojects5)

set @closed_tprojects5= @closed_tprojects5+ (select count(*) from Projects where Status=1 and ProjectId=@pid__tprojects5)

set @inprogress_tprojects5= @inprogress_tprojects5+ (select count(*) from Projects where Status=2 and ProjectId=@pid__tprojects5)

set @onholed_tprojects5= @onholed_tprojects5+ (select count(*) from Projects where Status=3 and ProjectId=@pid__tprojects5)

set @cancelled_tprojects5=@cancelled_tprojects5+ (select count(*) from Projects where Status=0 and ProjectId=@pid__tprojects5)

set @min__tprojects5=@min__tprojects5+1

end

insert into @output_tprojects5 (Total,Closed,InProgress,Onhold,Cancelled) values(@total_tprojects5,@closed_tprojects5,@inprogress_tprojects5,@onholed_tprojects5,@cancelled_tprojects5)

select * from @output_tprojects5

--################total############
end
else
begin
--###########supplier wise start##################
declare @temp3_ts4d3 as table(id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @temp1_ts4d3 as table (id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @min_ts4d3 int=1, @max_ts4d3 int,@grandtotal_ts4d3 float=0
insert into @temp1_ts4d3(completes,supplierID,suppname)
SELECT count(pm.SUpplierID) Completes , pm.SUpplierID , (SELECT s2.Name FROM dbo.Suppliers s2 WHERE id=pm.SUpplierID) SupplierName FROM dbo.Suppliers s
JOIN dbo.ProjectMapping pm ON s.id=pm.SUpplierID JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND pm.CountryID=@countryid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY pm.SUpplierID
SET @grandtotal_ts4d3 = (SELECT sum(completes) FROM @temp1_ts4d3)
INSERT INTO @temp3_ts4d3(supplierID,suppname,completes,percentage)
SELECT supplierID,suppname,completes,round (CASE WHEN(@grandtotal_ts4d3 = 0) THEN 0 ELSE ( isnull(completes,0)*100/isnull(@grandtotal_ts4d3,0))end,0) FROM @temp1_ts4d3

select top 10 * from @temp3_ts4d3 order by completes desc
--###########supplier wise end##################

--############client wise start####################
declare @grantotal_tcl4d3 float=0
declare @finaltable_tcl4d3 as table(id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
declare @clienttable_tcl4d3 as table (id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
insert into @clienttable_tcl4d3(ClientID,clieentname,complete)
SELECT c.ClientID ,(SELECT c2.Company FROM dbo.Clients c2 WHERE ClientID=c.ClientID) Name,count(c.ClientID) Completes  FROM dbo.Clients c JOIN dbo.Projects p ON c.ClientID=p.ClientID
JOIN dbo.ProjectMapping pm ON p.ProjectId=pm.ProjectID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND pm.CountryID=@countryid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY c.ClientID
SET @grantotal_tcl4d3 = (SELECT sum(complete) FROM @clienttable_tcl4d3)
INSERT INTO @finaltable_tcl4d3(ClientID,clieentname,complete,percentage)
SELECT ClientID,clieentname,complete , round (CASE WHEN(@grantotal_tcl4d3 = 0) THEN 0 ELSE ( isnull(complete,0)*100/isnull(@grantotal_tcl4d3,0))end,0) FROM @clienttable_tcl4d3

select top 10 * from @finaltable_tcl4d3 order by complete desc

--##############client wise end###########################

--###############country and supplier filter#country wise start###################################
declare @tblcountry_tco4d3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
declare @max_tco4d3 int,@min_tco4d3 int=1,@grandtotal_tco4d3 float=0
declare @tblCountryid_tco4d3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
insert into @tblCountryid_tco4d3(countryid,countryname,total)
SELECT tc.CountryID CountryId , (SELECT tc2.Country FROM dbo.CountryMaster tc2 WHERE CountryID=tc.CountryID ) Name , count(tc.CountryID) Complete FROM dbo.CountryMaster tc JOIN dbo.ProjectMapping pm ON tc.CountryID=pm.CountryID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND pm.CountryID=@countryid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
 GROUP BY tc.CountryID
 SET @grandtotal_tco4d3= (SELECT sum(total) FROM @tblCountryid_tco4d3)
 INSERT INTO @tblcountry_tco4d3(countryid,countryname,total,percentage)
 SELECT countryid,countryname,total,round (CASE WHEN(@grandtotal_tco4d3 = 0) THEN 0 ELSE ( isnull(total,0)*100/isnull(@grandtotal_tco4d3,0))end,0) FROM @tblCountryid_tco4d3

 select top 10 * from @tblcountry_tco4d3 order by total desc

--###############country and supplier filter#country wise end###################################

--###################total#################################
declare @output_tprojects5d1 as table (Id int identity(1,1) , Total int , Closed int, InProgress int, Onhold int, Cancelled int)
declare @total_tprojects5d1 int=0, @closed_tprojects5d1 int=0,@inprogress_tprojects5d1 int=0,@onholed_tprojects5d1 int=0,@cancelled_tprojects5d1 int=0
--select * from ProjectMapping
declare @projecttable_tprojects5d1 as table(Id int identity(1,1), ProjectId int,CountryId int)
insert into @projecttable_tprojects5d1
select ProjectID,CountryID from ProjectMapping where SUpplierID=@supplierid and CountryID=@countryid
declare @max_tprojects5d1 int=(select count(*) from @projecttable_tprojects5d1),@min__tprojects5d1 int=1

while(@min__tprojects5d1<=@max_tprojects5d1)

begin

declare @pid__tprojects5d1 int

set @pid__tprojects5d1=(select ProjectId from @projecttable_tprojects5d1 where Id=@min__tprojects5d1)

set @total_tprojects5d1= @total_tprojects5d1+(select count(*) from Projects where not (status=5 or status=6) and ProjectId=@pid__tprojects5d1 and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @closed_tprojects5d1= @closed_tprojects5d1+ (select count(*) from Projects where Status=1 and ProjectId=@pid__tprojects5d1  and (Convert(datetime,SDate))>=@startdate and (Convert(Datetime,EDate))<=@enddate)

set @inprogress_tprojects5d1= @inprogress_tprojects5d1+ (select count(*) from Projects where Status=2 and ProjectId=@pid__tprojects5d1 and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @onholed_tprojects5d1= @onholed_tprojects5d1+ (select count(*) from Projects where Status=3 and ProjectId=@pid__tprojects5d1 and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @cancelled_tprojects5d1=@cancelled_tprojects5d1+ (select count(*) from Projects where Status=0 and ProjectId=@pid__tprojects5d1 and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @min__tprojects5d1=@min__tprojects5d1+1

end

insert into @output_tprojects5d1 (Total,Closed,InProgress,Onhold,Cancelled) values(@total_tprojects5d1,@closed_tprojects5d1,@inprogress_tprojects5d1,@onholed_tprojects5d1,@cancelled_tprojects5d1)

select * from @output_tprojects5d1 

--##############total#############################



end



end
--supplier and client filter
if(@countryid=0 and @supplierid!=0 and @clientid!=0)
begin
if(@startdate='' and @enddate='')
begin
--#############supplier wise start##################


declare @temp3_ts5 as table(id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @temp1_ts5 as table (id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @min_ts5 int=1, @max_ts5 int,@grandtotal_ts5 float=0
insert into @temp1_ts5(completes,supplierID,suppname)
SELECT count(pm.SUpplierID) Completes , pm.SUpplierID , (SELECT s2.Name FROM dbo.Suppliers s2 WHERE id=pm.SUpplierID) SupplierName FROM dbo.Suppliers s
JOIN dbo.ProjectMapping pm ON s.id=pm.SUpplierID JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectID
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND p.ClientID=@clientid
GROUP BY pm.SUpplierID
SET  @grandtotal_ts5 = (SELECT sum(completes) FROM @temp1_ts5)
INSERT INTO @temp3_ts5(supplierID,suppname, completes  , percentage)
SELECT supplierID,suppname, completes,round (CASE WHEN(@grandtotal_ts5 = 0) THEN 0 ELSE ( isnull(completes,0)*100/isnull(@grandtotal_ts5,0))end,0) FROM @temp1_ts5
select top 10 * from @temp3_ts5 order by completes desc

--#############supplier wise end##################

--##############client wise start########################

declare @grantotal_tcl5 float=0
declare @finaltable_tcl5 as table(id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
declare @clienttable_tcl5 as table (id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
insert into @clienttable_tcl5(ClientID,clieentname,complete)
SELECT c.ClientID ,(SELECT c2.Company FROM dbo.Clients c2 WHERE ClientID=c.ClientID) Name,count(c.ClientID) Completes  FROM dbo.Clients c JOIN dbo.Projects p ON c.ClientID=p.ClientID
JOIN dbo.ProjectMapping pm ON p.ProjectID=pm.ProjectID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND p.ClientID=@clientid
GROUP BY c.ClientID
SET @grantotal_tcl5 = (SELECT sum(complete) FROM @clienttable_tcl5)
INSERT INTO @finaltable_tcl5(ClientID  , clieentname , complete , percentage )
SELECT ClientID , clieentname , complete , round (CASE WHEN(@grantotal_tcl5 = 0) THEN 0 ELSE ( isnull(complete,0)*100/isnull(@grantotal_tcl5,0))end,0) FROM @clienttable_tcl5

select top 10 * from @finaltable_tcl5 order by complete desc

--##############client wise end#################################

--#####################supplier and client filter###### country wise start ###############################
declare @tblcountry_tco5 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
declare @max_tco5 int,@min_tco5 int=1,@grandtotal_tco5 float=0
declare @tblCountryid_tco5 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
insert into @tblCountryid_tco5(countryid,countryname,total)
SELECT tc.CountryID CountryId , (SELECT tc2.Country FROM dbo.CountryMaster tc2 WHERE CountryID=tc.CountryID ) Name , count(tc.CountryID) Complete FROM dbo.CountryMaster tc JOIN dbo.ProjectMapping pm ON tc.CountryID=pm.CountryID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND p.ClientID=@clientid 
GROUP BY tc.CountryID
SET @grandtotal_tco5=(SELECT sum(total) FROM @tblCountryid_tco5)
INSERT INTO @tblcountry_tco5(countryid,countryname,total,percentage)
SELECT countryid,countryname,total,round (CASE WHEN(@grandtotal_tco5 = 0) THEN 0 ELSE ( isnull(total,0)*100/isnull(@grandtotal_tco5,0))end,0) FROM @tblCountryid_tco5
 select top 10 * from @tblcountry_tco5 order by total desc



--#####################supplier and client filter###### country wise end ###############################



--###################total############

declare @output_tprojects6 as table (Id int identity(1,1) , Total int , Closed int, InProgress int, Onhold int, Cancelled int)

declare @total_tprojects6 int=0, @closed_tprojects6 int=0,@inprogress_tprojects6 int=0,@onholed_tprojects6 int=0,@cancelled_tprojects6 int=0

--select * from ProjectMapping

declare @projecttable_tprojects6 as table(Id int identity(1,1), ProjectId int,CountryId int)

insert into @projecttable_tprojects6

select ProjectID,CountryID from ProjectMapping where SUpplierID=@supplierid

declare @max_tprojects6 int=(select count(*) from @projecttable_tprojects6),@min__tprojects6 int=1

while(@min__tprojects6<=@max_tprojects6)

begin

declare @pid__tprojects6 int

set @pid__tprojects6=(select ProjectId from @projecttable_tprojects6 where Id=@min__tprojects6)

set @total_tprojects6= @total_tprojects6+(select count(*) from Projects where not (status=5 or status=6) and ProjectId=@pid__tprojects6 and ClientID=@clientid)

set @closed_tprojects6= @closed_tprojects6+ (select count(*) from Projects where Status=1 and ProjectId=@pid__tprojects6 and ClientID=@clientid)

set @inprogress_tprojects6= @inprogress_tprojects6+ (select count(*) from Projects where Status=2 and ProjectId=@pid__tprojects6 and ClientID=@clientid)

set @onholed_tprojects6= @onholed_tprojects6+ (select count(*) from Projects where Status=3 and ProjectId=@pid__tprojects6 and ClientID=@clientid)

set @cancelled_tprojects6=@cancelled_tprojects6+ (select count(*) from Projects where Status=0 and ProjectId=@pid__tprojects6 and ClientID=@clientid)

set @min__tprojects6=@min__tprojects6+1

end

insert into @output_tprojects6 (Total,Closed,InProgress,Onhold,Cancelled) values(@total_tprojects6,@closed_tprojects6,@inprogress_tprojects6,@onholed_tprojects6,@cancelled_tprojects6)

select * from @output_tprojects6 
--#################total#############################
end
else
begin
--#############supplier wise start##################

declare @temp3_ts5d3 as table(id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @temp1_ts5d3 as table (id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @min_ts5d3 int=1, @max_ts5d3 int,@grandtotal_ts5d3 float=0
insert into @temp1_ts5d3(completes,supplierID,suppname)
SELECT count(pm.SUpplierID) Completes , pm.SUpplierID , (SELECT s2.Name FROM dbo.Suppliers s2 WHERE id=pm.SUpplierID) SupplierName FROM dbo.Suppliers s
JOIN dbo.ProjectMapping pm ON s.id=pm.SUpplierID JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
JOIN dbo.Projects p ON pm.ProjectID=p.ProjectId
WHERE sp.Status='Complete' AND p.ClientID=@clientid AND pm.SUpplierID=@supplierid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY pm.SUpplierID
SET @grandtotal_ts5d3 = (SELECT sum(completes) FROM @temp1_ts5d3)
INSERT into @temp3_ts5d3(supplierID,suppname,completes,percentage)
SELECT supplierID,suppname,completes,round (CASE WHEN(@grandtotal_ts5d3 = 0) THEN 0 ELSE ( isnull(completes,0)*100/isnull(@grandtotal_ts5d3,0))end,0) FROM @temp1_ts5d3

select top 10 * from @temp3_ts5d3 order by completes desc
--#############supplier wise end##################

--##############client wise start########################
declare @grantotal_tcl5d3 float=0
declare @finaltable_tcl5d3 as table(id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
declare @clienttable_tcl5d3 as table (id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
insert into @clienttable_tcl5d3(ClientID,clieentname,complete)
SELECT c.ClientID ,(SELECT c2.Company FROM dbo.Clients c2 WHERE ClientID=c.ClientID) Name,count(c.ClientID) Completes  FROM dbo.Clients c JOIN dbo.Projects p ON c.ClientID=p.ClientID
JOIN dbo.ProjectMapping pm ON p.ProjectId=pm.ProjectID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID
WHERE sp.Status='Complete' AND p.ClientID=@clientid AND pm.SUpplierID=@supplierid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY c.ClientID
SET @grantotal_tcl5d3=(select sum(complete) from @clienttable_tcl5d3)
INSERT INTO @finaltable_tcl5d3(ClientID,clieentname,complete,percentage)
SELECT ClientID,clieentname,complete,round (CASE WHEN(@grantotal_tcl5d3 = 0) THEN 0 ELSE ( isnull(complete,0)*100/isnull(@grantotal_tcl5d3,0))end,0) FROM @clienttable_tcl5d3


select top 10 * from @finaltable_tcl5d3 order by complete desc

--##############client wise end#################################

--#####################supplier and client filter###### country wise start ###############################



declare @tblcountry_tco5d3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total int, percentage float)
declare @max_tco5d3 int,@min_tco5d3 int=1,@grandtotal_tco5d3 float=0
declare @tblCountryid_tco5d3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total int, percentage float)
insert into @tblCountryid_tco5d3(countryid,countryname,total)
SELECT tc.CountryID CountryId , (SELECT tc2.Country FROM dbo.CountryMaster tc2 WHERE CountryID=tc.CountryID ) Name , count(tc.CountryID) Complete FROM dbo.CountryMaster tc JOIN dbo.ProjectMapping pm ON tc.CountryID=pm.CountryID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
JOIN projects p ON pm.projectid=p.ProjectId
WHERE sp.Status='Complete' AND pm.SUpplierID=@supplierid AND p.clientid=@clientid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
 GROUP BY tc.CountryID
 SET @grandtotal_tco5d3=(SELECT sum(total) FROM @tblCountryid_tco5d3)  
 INSERT INTO @tblcountry_tco5d3(countryid,countryname,total,percentage)
 SELECT countryid,countryname,total, round (CASE WHEN(@grandtotal_tco = 0) THEN 0 ELSE ( isnull(total,0)*100/isnull(@grandtotal_tco,0))end,0) from @tblCountryid_tco5d3 
 select top 10 * from @tblcountry_tco5d3 order by total desc
--#####################supplier and client filter###### country wise end ###############################
--###################total###########################
declare @output_tprojects6d1 as table (Id int identity(1,1) , Total int , Closed int, InProgress int, Onhold int, Cancelled int)

declare @total_tprojects6d1 int=0, @closed_tprojects6d1 int=0,@inprogress_tprojects6d1 int=0,@onholed_tprojects6d1 int=0,@cancelled_tprojects6d1 int=0

--select * from ProjectMapping

declare @projecttable_tprojects6d1 as table(Id int identity(1,1), ProjectId int,CountryId int)

insert into @projecttable_tprojects6d1

select ProjectID,CountryID from ProjectMapping where SUpplierID=@supplierid

declare @max_tprojects6d1 int=(select count(*) from @projecttable_tprojects6d1),@min__tprojects6d1 int=1

while(@min__tprojects6d1<=@max_tprojects6d1)

begin

declare @pid__tprojects6d1 int

set @pid__tprojects6d1=(select ProjectId from @projecttable_tprojects6d1 where Id=@min__tprojects6d1)

set @total_tprojects6d1= @total_tprojects6d1+(select count(*) from Projects where not (status=5 or status=6) and ProjectId=@pid__tprojects6d1 and ClientID=@clientid and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @closed_tprojects6d1= @closed_tprojects6d1+ (select count(*) from Projects where Status=1 and ProjectId=@pid__tprojects6d1 and ClientID=@clientid and (Convert(Datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @inprogress_tprojects6d1= @inprogress_tprojects6d1+ (select count(*) from Projects where Status=2 and ProjectId=@pid__tprojects6d1 and ClientID=@clientid and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @onholed_tprojects6d1= @onholed_tprojects6d1+ (select count(*) from Projects where Status=3 and ProjectId=@pid__tprojects6d1 and ClientID=@clientid and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @cancelled_tprojects6d1=@cancelled_tprojects6d1+ (select count(*) from Projects where Status=0 and ProjectId=@pid__tprojects6d1 and ClientID=@clientid and  (convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @min__tprojects6d1=@min__tprojects6d1+1

end

insert into @output_tprojects6d1 (Total,Closed,InProgress,Onhold,Cancelled) values(@total_tprojects6d1,@closed_tprojects6d1,@inprogress_tprojects6d1,@onholed_tprojects6d1,@cancelled_tprojects6d1)

select * from @output_tprojects6d1 
--###################total###########################
end
end
--country and client filter
if(@countryid!=0 and @supplierid=0 and @clientid!=0)
begin
if(@startdate='' and @enddate='')
begin
--#############supplier wise start################
declare @temp3_ts6 as table(id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @temp1_ts6 as table (id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @min_ts6 int=1, @max_ts6 int,@grandtotal_ts6 float=0
insert into @temp1_ts6(completes,supplierID,suppname)
SELECT count(pm.SUpplierID) Completes , pm.SUpplierID , (SELECT s2.Name FROM dbo.Suppliers s2 WHERE id=pm.SUpplierID) SupplierName FROM dbo.Suppliers s
JOIN dbo.ProjectMapping pm ON s.id=pm.SUpplierID JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
JOIN dbo.Projects p ON pm.projectid=p.ProjectId
WHERE sp.Status='Complete' AND p.ClientID=@clientid AND pm.CountryID=@countryid 
GROUP BY pm.SUpplierID
SET @grandtotal_ts6=(SELECT sum(completes) FROM @temp1_ts6)
INSERT INTO @temp3_ts6(supplierID,suppname,completes,percentage)
SELECT supplierID,suppname,completes,round (CASE WHEN(@grandtotal_ts6 = 0) THEN 0 ELSE ( isnull(completes,0)*100/isnull(@grandtotal_ts6,0))end,0) FROM @temp1_ts6

select top 10 * from @temp3_ts6 order by completes desc
--###############supplier wise end#########################
--#################client wise start#####################
declare @grantotal_tcl6 float=0
declare @finaltable_tcl6 as table(id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
declare @clienttable_tcl6 as table (id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
insert into @clienttable_tcl6(ClientID,clieentname,complete)
SELECT c.ClientID ,(SELECT c2.Company FROM dbo.Clients c2 WHERE ClientID=c.ClientID) Name,count(c.ClientID) Completes  FROM dbo.Clients c JOIN dbo.Projects p ON c.ClientID=p.ClientID
JOIN dbo.ProjectMapping pm ON p.ProjectID=pm.ProjectID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID
WHERE sp.Status='Complete' AND p.ClientID=@clientid AND pm.CountryID=@countryid
GROUP BY c.ClientID
set  @grantotal_tcl6 = (SELECT sum(complete) FROM @clienttable_tcl6)
insert INTO @finaltable_tcl6(ClientID,clieentname,complete,percentage)
SELECT ClientID,clieentname,complete,round (CASE WHEN(@grantotal_tcl6 = 0) THEN 0 ELSE ( isnull(complete,0)*100/isnull(@grantotal_tcl6,0))end,0) FROM @clienttable_tcl6

select top 10 * from @finaltable_tcl6 order by complete desc

--################client wise end#####################
--#############country and client######country wise###########################
declare @tblcountry_tco6 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
declare @max_tco6 int,@min_tco6 int=1,@grandtotal_tco6 float=0
declare @tblCountryid_tco6 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
insert into @tblCountryid_tco6(countryid,countryname,total)
SELECT tc.CountryID CountryId , (SELECT tc2.Country FROM dbo.CountryMaster tc2 WHERE CountryID=tc.CountryID ) Name , count(tc.CountryID) Complete FROM dbo.CountryMaster tc JOIN dbo.ProjectMapping pm ON tc.CountryID=pm.CountryID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
JOIN projects p ON pm.projectid=p.ProjectId
WHERE sp.Status='Complete' AND p.clientid=@clientid AND pm.CountryID=@countryid 
 GROUP BY tc.CountryID
 SET @grandtotal_tco6= (SELECT sum(total) FROM @tblCountryid_tco6)
INSERT INTO @tblcountry_tco6(countryid,countryname,total,percentage)
SELECT countryid,countryname,total,round (CASE WHEN(@grandtotal_tco6 = 0) THEN 0 ELSE ( isnull(total,0)*100/isnull(@grandtotal_tco6,0))end,0) FROM @tblCountryid_tco6

 select top 10 * from @tblcountry_tco6 order by total desc
--#############country and client######country wise###########################
--###################total##################
declare @output_tprojects7 as table (Id int identity(1,1) , Total int , Closed int, InProgress int, Onhold int, Cancelled int)

declare @total_tprojects7 int, @closed_tprojects7 int,@inprogress_tprojects7 int,@onholed_tprojects7 int,@cancelled_tprojects7 int

set @total_tprojects7=(select count(*) from Projects where not (status=5 or status=6) and ClientID=@clientid and CountryId=@countryid)

set @closed_tprojects7=(select count(*) from Projects where Status=1 and ClientID=@clientid and CountryId=@countryid)

set @inprogress_tprojects7=(select count(*) from Projects where Status=2 and ClientID=@clientid and CountryId=@countryid)

set @onholed_tprojects7=(select count(*) from Projects where Status=3 and ClientID=@clientid and CountryId=@countryid)

set @cancelled_tprojects7=(select count(*) from Projects where Status=0 and ClientID=@clientid and CountryId=@countryid)

insert into @output_tprojects7 (Total,Closed,InProgress,Onhold,Cancelled) values(@total_tprojects7,@closed_tprojects7,@inprogress_tprojects7,@onholed_tprojects7,@cancelled_tprojects7)

select * from @output_tprojects7 
--########total#################
end
else
begin
--#############supplier wise start################
declare @temp3_ts6d3 as table(id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @temp1_ts6d3 as table (id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @min_ts6d3 int=1, @max_ts6d3 int,@grandtotal_ts6d3 float=0
insert into @temp1_ts6d3(completes,supplierID,suppname)
SELECT count(pm.SUpplierID) Completes , pm.SUpplierID , (SELECT s2.Name FROM dbo.Suppliers s2 WHERE id=pm.SUpplierID) SupplierName FROM dbo.Suppliers s
JOIN dbo.ProjectMapping pm ON s.id=pm.SUpplierID JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
JOIN dbo.Projects p ON pm.projectid=p.ProjectId
WHERE sp.Status='Complete' AND p.ClientID=@clientid AND pm.CountryID=@countryid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY pm.SUpplierID
SET @grandtotal_ts6d3= (SELECT sum(completes) FROM @temp1_ts6d3)
INSERT INTO @temp3_ts6d3(supplierID,suppname,completes,percentage)
SELECT supplierID,suppname,completes,round (CASE WHEN(@grandtotal_ts6d3 = 0) THEN 0 ELSE ( isnull(completes,0)*100/isnull(@grandtotal_ts6d3,0))end,0) FROM @temp1_ts6d3

select top 10 * from @temp3_ts6d3 order by completes desc

--###############supplier wise end#########################
--#################client wise start#####################
declare @grantotal_tcl6d3 float=0
declare @finaltable_tcl6d3 as table(id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
declare @clienttable_tcl6d3 as table (id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
insert into @clienttable_tcl6d3(ClientID,clieentname,complete)
SELECT c.ClientID ,(SELECT c2.Company FROM dbo.Clients c2 WHERE ClientID=c.ClientID) Name,count(c.ClientID) Completes  FROM dbo.Clients c JOIN dbo.Projects p ON c.ClientID=p.ClientID
JOIN dbo.ProjectMapping pm ON p.ProjectID=pm.ProjectID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID
WHERE sp.Status='Complete' AND p.ClientID=@clientid AND pm.CountryID=@countryid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY c.ClientID
SET @grantotal_tcl6d3= (SELECT sum(complete) FROM @clienttable_tcl6d3)
INSERT INTO @finaltable_tcl6d3 (ClientID,clieentname,complete,percentage)
SELECT ClientID,clieentname,complete,round (CASE WHEN(@grantotal_tcl6d3 = 0) THEN 0 ELSE ( isnull(complete,0)*100/isnull(@grantotal_tcl6d3,0))end,0) FROM @clienttable_tcl6d3

select top 10 * from @finaltable_tcl6d3 order by complete desc
--################client wise end#####################
--#############country and client######country wise###########################
declare @tblcountry_tco6d3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
declare @max_tco6d3 int,@min_tco6d3 int=1,@grandtotal_tco6d3 float=0
declare @tblCountryid_tco6d3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
insert into @tblCountryid_tco6d3(countryid,countryname,total)
SELECT tc.CountryID CountryId , (SELECT tc2.Country FROM dbo.CountryMaster tc2 WHERE CountryID=tc.CountryID ) Name , count(tc.CountryID) Complete FROM dbo.CountryMaster tc JOIN dbo.ProjectMapping pm ON tc.CountryID=pm.CountryID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
JOIN projects p ON pm.projectid=p.ProjectId
WHERE sp.Status='Complete' AND p.clientid=@clientid AND pm.CountryID=@countryid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
 GROUP BY tc.CountryID
 SET @grandtotal_tco6d3 = (SELECT sum(@grandtotal_tco6d3) FROM @tblCountryid_tco6d3)
 INSERT INTO @tblcountry_tco6d3(countryid,countryname,total,percentage)
 SELECT countryid,countryname,total,round (CASE WHEN(@grandtotal_tco6d3 = 0) THEN 0 ELSE ( isnull(total,0)*100/isnull(@grandtotal_tco6d3,0))end,0) FROM @tblCountryid_tco6d3

  select top 10 * from @tblcountry_tco6d3 order by total desc
--#############country and client######country wise###########################
--################total##############################
declare @output_tprojects7d1 as table (Id int identity(1,1) , Total int , Closed int, InProgress int, Onhold int, Cancelled int)

declare @total_tprojects7d1 int, @closed_tprojects7d1 int,@inprogress_tprojects7d1 int,@onholed_tprojects7d1 int,@cancelled_tprojects7d1 int

set @total_tprojects7d1=(select count(*) from Projects where not (status=5 or status=6) and ClientID=@clientid and CountryId=@countryid and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @closed_tprojects7d1=(select count(*) from Projects where Status=1 and ClientID=@clientid and CountryId=@countryid and (convert(datetime,SDate))>=@startdate and (convert(datetime,EDate))<=@enddate)

set @inprogress_tprojects7d1=(select count(*) from Projects where Status=2 and ClientID=@clientid and CountryId=@countryid and (Convert(datetime,SDate))>=@startdate and (Convert(Datetime,EDate))<=@enddate)

set @onholed_tprojects7d1=(select count(*) from Projects where Status=3 and ClientID=@clientid and CountryId=@countryid and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @cancelled_tprojects7d1=(select count(*) from Projects where Status=0 and ClientID=@clientid and CountryId=@countryid and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

insert into @output_tprojects7d1 (Total,Closed,InProgress,Onhold,Cancelled) values(@total_tprojects7d1,@closed_tprojects7d1,@inprogress_tprojects7d1,@onholed_tprojects7d1,@cancelled_tprojects7d1)

select * from @output_tprojects7d1
--################total##############################
end
end
--country supplier client filter
if(@countryid!=0 and @supplierid!=0 and @clientid!=0)
begin
if(@startdate='' and @enddate='')
begin
--################supplier wise start###############
declare @temp3_ts7 as table(id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @temp1_ts7 as table (id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @min_ts7 int=1, @max_ts7 int,@grandtotal_ts7 float=0
insert into @temp1_ts7(completes,supplierID,suppname)
SELECT count(pm.SUpplierID) Completes , pm.SUpplierID , (SELECT s2.Name FROM dbo.Suppliers s2 WHERE id=pm.SUpplierID) SupplierName FROM dbo.Suppliers s
JOIN dbo.ProjectMapping pm ON s.id=pm.SUpplierID JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
JOIN dbo.Projects p ON pm.projectid=p.projectid
WHERE sp.Status='Complete' AND p.ClientID=@clientid AND pm.CountryID=@countryid AND pm.supplierid=@supplierid --AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY pm.SUpplierID
SET @grandtotal_ts7= (SELECT sum(completes) FROM @temp1_ts7)
INSERT INTO @temp3_ts7 (supplierID,suppname,completes,percentage)
SELECT supplierID,suppname,completes,round (CASE WHEN(@grandtotal_ts7 = 0) THEN 0 ELSE ( isnull(completes,0)*100/isnull(@grandtotal_ts7,0))end,0) FROM @temp1_ts7

select top 10 * from @temp3_ts7 order by completes desc
--################supplier wise end########################
--###############client wise start#########################
declare @grantotal_tcl7 float=0
declare @finaltable_tcl7 as table(id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
declare @clienttable_tcl7 as table (id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
insert into @clienttable_tcl7(ClientID,clieentname,complete)
SELECT c.ClientID ,(SELECT c2.Company FROM dbo.Clients c2 WHERE ClientID=c.ClientID) Name,count(c.ClientID) Completes  FROM dbo.Clients c JOIN dbo.Projects p ON c.ClientID=p.ClientID
JOIN dbo.ProjectMapping pm ON p.projectid=pm.ProjectID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID
WHERE sp.Status='Complete' AND p.ClientID=@clientid AND pm.CountryID=@countryid AND pm.supplierid=@supplierid
GROUP BY c.ClientID
SET @grantotal_tcl7 = (SELECT sum(complete) FROM @clienttable_tcl7)
INSERT INTO @finaltable_tcl7 (ClientID,clieentname,complete,percentage)
SELECT ClientID,clieentname,complete, round (CASE WHEN(@grantotal_tcl7 = 0) THEN 0 ELSE ( isnull(complete,0)*100/isnull(@grantotal_tcl7,0))end,0) FROM @clienttable_tcl7

select top 10 * from @finaltable_tcl7 order by complete desc
--################client wise end############################
--###########country client supplier#######country wise###############################
declare @tblcountry_tco7 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
declare @max_tco7 int,@min_tco7 int=1,@grandtotal_tco7 float=0
declare @tblCountryid_tco7 as table(id int identity(1,1) , countryid int, countryname varchar(50), total float, percentage float)
insert into @tblCountryid_tco7(countryid,countryname,total)
SELECT tc.CountryID CountryId , (SELECT tc2.Country FROM dbo.CountryMaster tc2 WHERE CountryID=tc.CountryID ) Name , count(tc.CountryID) Complete FROM dbo.CountryMaster tc JOIN dbo.ProjectMapping pm ON tc.CountryID=pm.CountryID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
JOIN projects p ON pm.projectid=p.ProjectId
WHERE sp.Status='Complete' AND p.clientid=@clientid AND pm.CountryID=@countryid AND pm.supplierid=@supplierid 
 GROUP BY tc.CountryID
 SET @grandtotal_tco7 = (SELECT sum(total) FROM @tblCountryid_tco7)
INSERT INTO @tblcountry_tco7(countryid,countryname,total,percentage)
SELECT countryid,countryname,total,round (CASE WHEN(@grandtotal_tco7 = 0) THEN 0 ELSE ( isnull(total,0)*100/isnull(@grandtotal_tco7,0))end,0) FROM @tblCountryid_tco7

 select top 10 * from @tblcountry_tco7 order by total desc
--###########country client supplier#######country wise###############################
--#####################total####################

declare @output_tprojects8 as table (Id int identity(1,1) , Total int , Closed int, InProgress int, Onhold int, Cancelled int)

declare @total_tprojects8 int=0, @closed_tprojects8 int=0,@inprogress_tprojects8 int=0,@onholed_tprojects8 int=0,@cancelled_tprojects8 int=0

--select * from ProjectMapping

declare @projecttable_tprojects8 as table(Id int identity(1,1), ProjectId int,CountryId int)

insert into @projecttable_tprojects8

select ProjectID,CountryID from ProjectMapping where SUpplierID=@supplierid and CountryID=@countryid

declare @max_tprojects8 int=(select count(*) from @projecttable_tprojects8),@min__tprojects8 int=1

while(@min__tprojects8<=@max_tprojects8)

begin

declare @pid__tprojects8 int

set @pid__tprojects8=(select ProjectId from @projecttable_tprojects8 where Id=@min__tprojects8)

set @total_tprojects8= @total_tprojects8+(select count(*) from Projects where not (status=5 or status=6) and ProjectId=@pid__tprojects8 and ClientID=@clientid)

set @closed_tprojects8= @closed_tprojects8+ (select count(*) from Projects where Status=1 and ProjectId=@pid__tprojects8 and ClientID=@clientid)

set @inprogress_tprojects8= @inprogress_tprojects8+ (select count(*) from Projects where Status=2 and ProjectId=@pid__tprojects8 and ClientID=@clientid)

set @onholed_tprojects8= @onholed_tprojects8+ (select count(*) from Projects where Status=3 and ProjectId=@pid__tprojects8 and ClientID=@clientid)

set @cancelled_tprojects8=@cancelled_tprojects8+ (select count(*) from Projects where Status=0 and ProjectId=@pid__tprojects8 and ClientID=@clientid)

set @min__tprojects8=@min__tprojects8+1

end

insert into @output_tprojects8 (Total,Closed,InProgress,Onhold,Cancelled) values(@total_tprojects8,@closed_tprojects8,@inprogress_tprojects8,@onholed_tprojects8,@cancelled_tprojects8)

select * from @output_tprojects8 
--########total########################
end
else
begin
--################supplier wise start###############
declare @temp3_ts7d3 as table(id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @temp1_ts7d3 as table (id int identity(1,1) ,supplierID int ,  suppname varchar(100), completes float , percentage float)
declare @min_ts7d3 int=1, @max_ts7d3 int,@grandtotal_ts7d3 float=0
insert into @temp1_ts7d3(completes,supplierID,suppname)
SELECT count(pm.SUpplierID) Completes , pm.SUpplierID , (SELECT s2.Name FROM dbo.Suppliers s2 WHERE id=pm.SUpplierID) SupplierName FROM dbo.Suppliers s
JOIN dbo.ProjectMapping pm ON s.id=pm.SUpplierID JOIN dbo.SupplierProjects sp ON pm.sid=sp.sid
JOIN dbo.Projects p ON pm.projectid=p.projectid
WHERE sp.Status='Complete' AND p.ClientID=@clientid AND pm.CountryID=@countryid AND pm.supplierid=@supplierid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY pm.SUpplierID
SET @grandtotal_ts7d3= (SELECT sum(completes) FROM @temp1_ts7d3)
INSERT INTO @temp3_ts7d3(supplierID,suppname,completes,percentage)
SELECT supplierID,suppname,completes,round (CASE WHEN(@grandtotal_ts7d3 = 0) THEN 0 ELSE ( isnull(completes,0)*100/isnull(@grandtotal_ts7d3,0))end,0) FROM @temp1_ts7d3

select top 10 * from @temp3_ts7d3 order by completes desc

--################supplier wise end########################
--###############client wise start#########################
declare @grantotal_tcl7d3 float=0
declare @finaltable_tcl7d3 as table(id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
declare @clienttable_tcl7d3 as table (id int identity(1,1) , ClientID int , clieentname varchar(100), complete float, percentage float)
insert into @clienttable_tcl7d3(ClientID,clieentname,complete)
SELECT c.ClientID ,(SELECT c2.Company FROM dbo.Clients c2 WHERE ClientID=c.ClientID) Name,count(c.ClientID) Completes  FROM dbo.Clients c JOIN dbo.Projects p ON c.ClientID=p.ClientID
JOIN dbo.ProjectMapping pm ON p.ProjectID=pm.ProjectID
JOIN dbo.SupplierProjects sp ON pm.sid=sp.SID
WHERE sp.Status='Complete' AND p.ClientID=@clientid AND pm.CountryID=@countryid AND pm.supplierid=@supplierid AND convert(date,StartDate)>=convert(date,@startdate) and convert(date,EndDate)<=convert(date,@enddate)
GROUP BY c.ClientID
SET @grantotal_tcl7d3 = (SELECT sum(complete) FROM @clienttable_tcl7d3)
INSERT INTO @finaltable_tcl7d3(ClientID,clieentname,complete,percentage)
SELECT ClientID,clieentname,complete,round (CASE WHEN(@grantotal_tcl7d3 = 0) THEN 0 ELSE ( isnull(complete,0)*100/isnull(@grantotal_tcl7d3,0))end,0)  from @clienttable_tcl7d3

select top 10 * from @finaltable_tcl7d3 order by complete desc
--################client wise end############################







--###########country client supplier#######country wise###############################















declare @tblcountry_tco7d3 as table(id int identity(1,1) , countryid int, countryname varchar(50), total int, percentage float)







declare @max_tco7d3 int,@min_tco7d3 int=1,@grandtotal_tco7d3 int=0







declare @tblCountryid_tco7d3 as table(id int identity(1,1) , countryid int)







insert into @tblCountryid_tco7d3







select distinct p.CountryId from Projects p left join ProjectMapping pm on p.ProjectID=pm.ProjectID  where ClientID=@clientid and pm.SUpplierID=@supplierid and pm.CountryID=@countryid







set @max_tco7d3=(select count(*) from @tblCountryid_tco7d3)







while(@min_tco7d3<=@max_tco7d3)







begin







declare @countryid_tco7d3 int, @countryname_tco7d3 varchar(100),@total_tco7d3 int=0







set @countryid_tco7d3=(select countryid from @tblCountryid_tco7d3 where id=@min_tco7d3)







set @countryname_tco7d3=(select Country from CountryMaster where CountryID=@countryid_tco7d3)







create table #tblSID_tco7d3 (id int identity(1,1) , sid varchar(50))







insert into #tblSID_tco7d3







select [SID] from ProjectMapping where CountryID=@countryid_tco7d3







declare @max1_tco7d3 int=(select count(*) from #tblSID_tco7d3), @min1_tco7d3 int=1 







while(@min1_tco7d3<=@max1_tco7d3)







begin







declare @sid_tco7d3 varchar(50)







set @sid_tco7d3=(select sid from #tblSID_tco7d3 where id=@min1_tco7d3)







set @total_tco7d3 =@total_tco7d3+(select count(*) from SupplierProjects where SID=@sid_tco7d3 and Status='Complete' and StartDate>=@startdate and EndDate<=@enddate)







set @min1_tco7d3=@min1_tco7d3+1







end







set @grandtotal_tco7d3=@grandtotal_tco7d3+@total_tco7d3







insert into @tblcountry_tco7d3(countryid,countryname,total) values (@countryid_tco7d3,@countryname_tco7d3,@total_tco7d3)







drop table #tblSID_tco7d3







set @min_tco7d3=@min_tco7d3+1







end







 declare @max3_tco7d3 int=(select count(*) from @tblcountry_tco7d3),@min3_tco7d3 int=1







while(@min3_tco7d3<=@max3_tco7d3)







begin







declare @percentt_tco7d3 float,@completep_tco7d3 float,@percentint_tco7d3 float







set @completep_tco7d3=(select total from @tblcountry_tco7d3 where id=@min3_tco7d3)







if(@grandtotal_tco7d3=0)
begin
set @percentint_tco7d3=0
end
else
begin
set @percentt_tco7d3= @completep_tco7d3*100/@grandtotal_tco7d3
set @percentint_tco7d3=ROUND(@percentt_tco7d3,0)







end















update @tblcountry_tco7d3 set percentage=@percentint_tco7d3 where id=@min3_tco7d3







set @min3_tco7d3=@min3_tco7d3+1







end







 select top 10 * from @tblcountry_tco7d3 order by total desc



--###########country client supplier#######country wise###############################



--######################total#########################



declare @output_tprojects8d1 as table (Id int identity(1,1) , Total int , Closed int, InProgress int, Onhold int, Cancelled int)

declare @total_tprojects8d1 int=0, @closed_tprojects8d1 int=0,@inprogress_tprojects8d1 int=0,@onholed_tprojects8d1 int=0,@cancelled_tprojects8d1 int=0

--select * from ProjectMapping

declare @projecttable_tprojects8d1 as table(Id int identity(1,1), ProjectId int,CountryId int)

insert into @projecttable_tprojects8d1

select ProjectID,CountryID from ProjectMapping where SUpplierID=@supplierid and CountryID=@countryid

declare @max_tprojects8d1 int=(select count(*) from @projecttable_tprojects8d1),@min__tprojects8d1 int=1

while(@min__tprojects8d1<=@max_tprojects8d1)

begin

declare @pid__tprojects8d1 int

set @pid__tprojects8d1=(select ProjectId from @projecttable_tprojects8d1 where Id=@min__tprojects8d1)

set @total_tprojects8d1= @total_tprojects8d1+(select count(*) from Projects where not (status=5 or status=6) and ProjectID=@pid__tprojects8d1 and ClientID=@clientid and (Convert(datetime,SDate))>=@startdate and (Convert(datetime, EDate))<=@enddate)

set @closed_tprojects8d1= @closed_tprojects8d1+ (select count(*) from Projects where Status=1 and ProjectID=@pid__tprojects8d1 and ClientID=@clientid and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @inprogress_tprojects8d1= @inprogress_tprojects8d1+ (select count(*) from Projects where Status=2 and ProjectID=@pid__tprojects8d1 and ClientID=@clientid and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @onholed_tprojects8d1= @onholed_tprojects8d1+ (select count(*) from Projects where Status=3 and ProjectID=@pid__tprojects8d1 and ClientID=@clientid and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @cancelled_tprojects8d1=@cancelled_tprojects8d1+ (select count(*) from Projects where Status=0 and ProjectID=@pid__tprojects8d1 and ClientID=@clientid and (Convert(datetime,SDate))>=@startdate and (Convert(datetime,EDate))<=@enddate)

set @min__tprojects8d1=@min__tprojects8d1+1

end

insert into @output_tprojects8d1 (Total,Closed,InProgress,Onhold,Cancelled) values(@total_tprojects8d1,@closed_tprojects8d1,@inprogress_tprojects8d1,@onholed_tprojects8d1,@cancelled_tprojects8d1)

select * from @output_tprojects8d1 

--######################total#########################







end







end







end













end





