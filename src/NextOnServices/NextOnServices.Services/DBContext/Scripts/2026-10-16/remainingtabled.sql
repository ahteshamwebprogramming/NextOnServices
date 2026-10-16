CREATE OR ALTER PROC [dbo].[remainingtabled]
@countryid int=0,
@supplierid int=0,
@clientid int=0,
@startdate datetime='',
@enddate datetime=''
as
begin
if(@countryid=0 and @supplierid=0 and @clientid=0)
begin
if(@startdate='' and @enddate='')
begin
declare @tblprojects as table (id int identity(1,1) , PID int, Pname varchar(100), ClientId int, LOI float, IRate float, CPI float, CountryId int, status int, sdate varchar(50), edate varchar(50),ltype int)
insert into @tblprojects
select ProjectId,Pname,clientid,loi,irate,cpi,CountryId,Status,SDate,Edate,LType from Projects
select (case when (MAX(IRate)) is null then 0 else (MAX(IRate)) end) as [Max Irate],(case when (Min(IRate)) is null then 0 else (Min(IRate)) end ) as [Min Irate],(case when (avg(Irate)) is null then 0 else (avg(Irate)) end) as [Average Irate] from @tblprojects
select (case when (MAX(LOI)) is null then 0 else (MAX(LOI))end) as [Max LOI],(case when (Min(LOI)) is null then 0 else (Min(LOI))end) as [Min LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average LOI] from @tblprojects
select (case when (MAX(CPI)) is null then 0 else (MAX(CPI)) end) as [Max CPI],(case when (Min(CPI)) is null then 0 else (Min(CPI))end) as [Min CPI], (case when (avg(CPI)) is null then 0 else (avg(CPI))end) as [Average CPI] from @tblprojects
declare @tblprojectsacloi as table (id int identity(1,1), startdate datetime, enddate datetime, LOI int )
insert into @tblprojectsacloi
select startdate,enddate,datediff(mi,startdate,enddate) from supplierprojects WHERE startdate IS NOT NULL AND enddate IS NOT NULL AND Status='Complete' AND datediff(d,startdate,enddate)<1
--select * from @tblprojectsacloi
select (case when (Max(LOI)) is null then 0 else (Max(LOI))end) as [Max Actual LOI] ,(case when (Min(LOI)) is null then 0 else (Min(LOI)) end ) as [Min Actual LOI],(case when (avg(LOI)) is null then 0 else (avg(LOI))end) as [Average Actual LOI] from @tblprojectsacloi
end
else
begin
declare @tblprojects_d as table (id int identity(1,1) , PID int, Pname varchar(100), ClientId int, LOI float, IRate float, CPI float, CountryId int, status int, sdate varchar(50), edate varchar(50),ltype int)
insert into @tblprojects_d
select ProjectId,Pname,clientid,loi,irate,cpi,CountryId,Status,SDate,EDate,LType from Projects where CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate)
select (case when (MAX(IRate)) is null then 0 else (MAX(IRate))end) as [Max Irate],(case when (Min(IRate)) is null then 0 else (Min(IRate))end) as [Min Irate],(case when (avg(Irate)) is null then 0 else (avg(Irate))end) as [Average Irate] from @tblprojects_d
select (case when (MAX(LOI))  is null then 0 else(MAX(LOI))end)  as [Max LOI],(case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI))end) as [Average LOI] from @tblprojects_d
select (case when (MAX(CPI))  is null then 0 else(MAX(CPI))end)  as [Max CPI],(case when (Min(CPI)) is null then 0 else (Min(CPI)) end) as [Min CPI], (case when (avg(CPI)) is null then 0 else (avg(CPI))end) as [Average CPI] from @tblprojects_d
declare @tblprojectsacloi_d as table (id int identity(1,1), startdate datetime, enddate datetime, LOI float )
insert into @tblprojectsacloi_d
select startdate,enddate,datediff(mi,startdate,enddate) from supplierprojects where StartDate >= @startdate AND EndDate <= @enddate AND status='Complete' AND StartDate IS NOT NULL AND enddate IS NOT NULL AND datediff(d,startdate,enddate)<1
--select * from @tblprojectsacloi
select (case when (Max(LOI)) is null then 0 else (Max(LOI))end) as [Max Actual LOI] ,(case when(Min(LOI)) is null then 0 else (Min(LOI))end) as [Min Actual LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI))end) as [Average Actual LOI] from @tblprojectsacloi_d
end
end
if(@countryid!=0 and @supplierid=0 and @clientid=0)
begin
if(@startdate='' and @enddate='')
begin
declare @tblprojects_WCID as table (id int identity(1,1) , PID int, Pname varchar(100), ClientId int, LOI float, IRate float, CPI float, CountryId int)
insert into @tblprojects_WCID
select distinct p.ProjectID, p.pname,p.clientid,p.loi,p.irate,p.CPI,p.CountryId from projectmapping pm left join projects p on pm.projectid=p.ProjectId where p.CountryID=@countryid
--select * from @tblprojects_WCID
select (case when(MAX(IRate)) is null then 0 else (MAX(IRate))end) as [Max Irate],(case when(Min(IRate)) is null then 0 else (Min(IRate)) end)  as [Min Irate], (case when (avg(Irate)) is null then 0 else (avg(Irate)) end ) as [Average Irate] from @tblprojects_WCID
select (case when(MAX(LOI)) is null then 0 else (MAX(LOI)) end ) as [Max LOI],(case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average LOI] from @tblprojects_WCID
select (case when (MAX(CPI)) is null then 0 else (MAX(CPI)) end) as [Max CPI],(case when (Min(CPI)) is null then 0 else (Min(CPI)) end) as [Min CPI], (case when (avg(CPI)) is null then 0 else (avg(CPI))end) as [Average CPI] from @tblprojects_WCID
declare @tblprojectsacloi_WCID as table (id int identity(1,1), startdate datetime, enddate datetime, LOI float )
insert into @tblprojectsacloi_WCID
select startdate,enddate,datediff(mi,startdate,enddate) from supplierprojects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid AND sp.Status='Complete' AND sp.StartDate IS NOT NULL AND sp.EndDate IS NOT NULL AND datediff(d,sp.startdate,sp.EndDate)<1
--select * from @tblprojectsacloi
select (case when (Max(LOI)) is null then 0 else (Max(LOI)) end) as [Max Actual LOI] , (case when(Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min Actual LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end ) as [Average Actual LOI] from @tblprojectsacloi_WCID
end
else 
begin
declare @tblprojects_WCID_d as table (id int identity(1,1) , PID int, Pname varchar(100), ClientId int, LOI float, IRate float, CPI float, CountryId int)
insert into @tblprojects_WCID_d
select distinct p.ProjectID, p.pname,p.clientid,p.loi,p.irate,p.CPI,p.CountryId from projectmapping pm left join projects p on pm.projectid=p.ProjectId where p.CountryID=@countryid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate)
--select * from @tblprojects_WCID_d
select (case when (MAX(IRate)) is null then 0 else (MAX(IRate))end) as [Max Irate],(case when (Min(IRate)) is null then 0 else (Min(IRate)) end) as [Min Irate],(case when (avg(Irate)) is null then 0 else (avg(Irate))end) as [Average Irate] from @tblprojects_WCID_d
select (case when(MAX(LOI)) is null then 0 else (MAX(LOI)) end) as [Max LOI],(case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI))end) as [Average LOI] from @tblprojects_WCID_d
select (case when (MAX(CPI)) is null then 0 else (MAX(CPI))end) as [Max CPI],(case when (Min(CPI)) is null then 0 else (Min(CPI))end) as [Min CPI], (case when (avg(CPI)) is null then 0 else (avg(CPI))end) as [Average CPI] from @tblprojects_WCID_d
declare @tblprojectsacloi_WCID_d as table (id int identity(1,1), startdate datetime, enddate datetime, LOI float )
insert into @tblprojectsacloi_WCID_d
select startdate,enddate,datediff(mi,startdate,enddate) from supplierprojects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid and StartDate >= @startdate AND EndDate <= @enddate AND sp.status='Complete' AND sp.startdate IS NOT NULL AND sp.enddate IS NOT NULL AND datediff(d,sp.startdate,sp.enddate)<1
--select * from @tblprojectsacloi
select (case when (Max(LOI)) is null then 0 else (Max(LOI))end) as [Max Actual LOI] ,(case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min Actual LOI],( case when (avg(LOI)) is null then 0 else (avg(LOI))end ) as [Average Actual LOI] from @tblprojectsacloi_WCID_d
end
end
if(@countryid=0 and @supplierid!=0 and @clientid=0)
begin
if(@startdate='' and @enddate='')
begin
declare @tblprojects_WSID as table (id int identity(1,1) , PID int, Pname varchar(100), ClientId int, LOI float, IRate float, CPI float, CountryId int)
insert into @tblprojects_WSID
select distinct p.ProjectID, p.pname,p.clientid,p.loi,p.irate,p.CPI,p.CountryId from projectmapping pm left join projects p on pm.projectid=p.ProjectId where supplierid=@supplierid
--select * from @tblprojects_WSID
select (case when (MAX(IRate)) is null then 0 else (MAX(IRate)) end) as [Max Irate],(case when (Min(IRate)) is null then 0 else (Min(IRate)) end) as [Min Irate],(case when (avg(Irate)) is null then 0 else (avg(Irate)) end) as [Average Irate] from @tblprojects_WSID
select (case when(MAX(LOI)) is null then 0 else (MAX(LOI)) end ) as [Max LOI],(case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min LOI],(case when (avg(LOI)) is null then 0 else (avg(LOI))end) as [Average LOI] from @tblprojects_WSID
select (case when(MAX(CPI)) is null then 0 else (MAX(CPI)) end) as [Max CPI],(case when (Min(CPI)) is null then 0 else (Min(CPI)) end) as [Min CPI],(case when (avg(CPI)) is null then 0 else (avg(CPI)) end) as [Average CPI] from @tblprojects_WSID
declare @tblprojectsacloi_WSID as table (id int identity(1,1), startdate datetime, enddate datetime, LOI float )
insert into @tblprojectsacloi_WSID
select startdate,enddate,datediff(mi,startdate,enddate) from supplierprojects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid AND sp.Status='Complete' AND sp.StartDate IS NOT NULL AND sp.enddate IS NOT NULL AND datediff(d,sp.startdate,sp.enddate)<1
--select * from @tblprojectsacloi
select (case when (Max(LOI)) is null then 0 else (Max(LOI)) end) as [Max Actual LOI] , (case when (Min(LOI)) is null then 0 else(Min(LOI)) end) as [Min Actual LOI],(case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average Actual LOI] from @tblprojectsacloi_WSID
end
else
begin
declare @tblprojects_WSID_d as table (id int identity(1,1) , PID int, Pname varchar(100), ClientId int, LOI float, IRate float, CPI float, CountryId int)
insert into @tblprojects_WSID_d
select distinct p.ProjectID, p.pname,p.clientid,p.loi,p.irate,p.CPI,p.CountryId from projectmapping pm left join projects p on pm.projectid=p.ProjectId where supplierid=@supplierid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate)
--select * from @tblprojects_WSID_d
select (case when(MAX(IRate)) is null then 0 else (MAX(IRate)) end) as [Max Irate],(case when (Min(IRate)) is null then 0 else (Min(IRate)) end) as [Min Irate],(case when (avg(Irate)) is null then 0 else (avg(Irate)) end) as [Average Irate] from @tblprojects_WSID_d
select (case when (MAX(LOI)) is null then 0 else (MAX(LOI)) end) as [Max LOI],(case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average LOI] from @tblprojects_WSID_d
select (case when(MAX(CPI)) is null then 0 else (MAX(CPI)) end) as [Max CPI],( case when (Min(CPI)) is null then 0 else (Min(CPI)) end) as [Min CPI], (case when (avg(CPI)) is null then 0 else (avg(CPI)) end) as [Average CPI] from @tblprojects_WSID_d
declare @tblprojectsacloi_WSID_d as table (id int identity(1,1), startdate datetime, enddate datetime, LOI float )
insert into @tblprojectsacloi_WSID_d
select startdate,enddate,datediff(mi,startdate,enddate) from supplierprojects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid  and StartDate >= @startdate AND EndDate <= @enddate AND sp.Status='Complete' AND sp.StartDate IS NOT NULL AND sp.enddate IS NOT NULL AND datediff(d,sp.startdate,sp.enddate)<1
--select * from @tblprojectsacloi
select (case when (Max(LOI)) is null then 0 else (Max(LOI)) end) as [Max Actual LOI] , (case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min Actual LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average Actual LOI] from @tblprojectsacloi_WSID_d
end

end
if(@countryid=0 and @supplierid=0 and @clientid!=0)
begin
if(@startdate='' and @enddate='')
begin
declare @tblprojects_WCLID as table (id int identity(1,1) , PID int, Pname varchar(100), ClientId int, LOI float, IRate float, CPI float, CountryId int)
insert into @tblprojects_WCLID
select ProjectId,Pname,clientid,loi,irate,cpi,CountryId from Projects where clientid=@clientid
--select * from @tblprojects_WCLID
select (case when (MAX(IRate)) is null then 0 else (MAX(IRate)) end) as [Max Irate],(case when (Min(IRate)) is null then 0 else (Min(IRate)) end) as [Min Irate], (case when (avg(Irate)) is null then 0 else (avg(Irate)) end) as [Average Irate] from @tblprojects_WCLID
select (Case when (MAX(LOI)) is null then 0 else (MAX(LOI)) end) as [Max LOI],(case when (Min(LOI)) is null then 0 else(Min(LOI)) end) as [Min LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average LOI] from @tblprojects_WCLID
select (case when (MAX(CPI)) is null then 0 else (MAX(CPI)) end) as [Max CPI],(case when (Min(CPI)) is null then 0 else(Min(CPI)) end) as [Min CPI], (case when (avg(CPI)) is null then 0 else (avg(CPI)) end) as [Average CPI] from @tblprojects_WCLID
declare @tblprojectsacloi_WCLID as table (id int identity(1,1), startdate datetime, enddate datetime, LOI float )
insert into @tblprojectsacloi_WCLID
select startdate,enddate,datediff(mi,startdate,enddate) from supplierprojects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid  AND sp.Status='Complete' AND sp.StartDate IS NOT NULL AND sp.enddate IS NOT NULL AND datediff(d,sp.startdate,sp.enddate)<1
--select * from @tblprojectsacloi
select (Case when (Max(LOI)) is null then 0 else Max(LOI) end) as [Max Actual LOI] ,(case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min Actual LOI],(case when (avg(LOI)) is null then 0 else (avg(LOI))end) as [Average Actual LOI] from @tblprojectsacloi_WCLID
end
else
begin
declare @tblprojects_WCLID_D as table (id int identity(1,1) , PID int, Pname varchar(100), ClientId int, LOI float, IRate float, CPI float, CountryId int)
insert into @tblprojects_WCLID_D
select ProjectId,Pname,clientid,loi,irate,cpi,CountryId from Projects where clientid=@clientid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate)
--select * from @tblprojects_WCLID_D
select (case when (MAX(IRate)) is null then 0 else (MAX(IRate)) end) as [Max Irate],(case when (Min(IRate)) is null then 0 else (Min(IRate)) end) as [Min Irate], (case when(avg(Irate)) is null then 0 else (avg(Irate)) end) as [Average Irate] from @tblprojects_WCLID_D
select (case when(MAX(LOI)) is null then 0 else (MAX(LOI)) end) as [Max LOI],( case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average LOI] from @tblprojects_WCLID_D
select (case when(MAX(CPI)) is null then 0 else (MAX(CPI)) end) as [Max CPI],(case when (Min(CPI)) is null then 0 else (Min(CPI)) end) as [Min CPI], (case when (avg(CPI)) is null then 0 else (avg(CPI)) end) as [Average CPI] from @tblprojects_WCLID_D
declare @tblprojectsacloi_WCLID_d as table (id int identity(1,1), startdate datetime, enddate datetime, LOI float )
insert into @tblprojectsacloi_WCLID_d
select startdate,enddate,datediff(mi,startdate,enddate) from supplierprojects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and StartDate >= @startdate AND EndDate <= @enddate  AND sp.Status='Complete' AND sp.StartDate IS NOT NULL AND sp.enddate IS NOT NULL AND datediff(d,sp.startdate,sp.enddate)<1
--select * from @tblprojectsacloi
select (case when (Max(LOI)) is null then 0 else(Max(LOI)) end) as [Max Actual LOI] , (case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min Actual LOI], (case when (avg(LOI)) is null then 0 else(avg(LOI)) end) as [Average Actual LOI] from @tblprojectsacloi_WCLID_d
end
end
if(@countryid!=0 and @supplierid!=0 and @clientid=0)
begin
if(@startdate='' and @enddate='')
begin
declare @tblprojects_WCOSL as table (id int identity(1,1) , PID int, Pname varchar(100), ClientId int, LOI float, IRate float, CPI float, CountryId int)
insert into @tblprojects_WCOSL
select distinct p.ProjectID, p.pname,p.clientid,p.loi,p.irate,p.CPI,p.CountryId from projectmapping pm left join projects p on pm.projectid=p.ProjectId where supplierid=@supplierid and p.countryid=@countryid
--select * from @tblprojects_WCOSL
select (case when (MAX(IRate)) is null then 0 else (MAX(IRate)) end) as [Max Irate],(case when (Min(IRate)) is null then 0 else(Min(IRate)) end) as [Min Irate], (case when (avg(Irate))is null then 0 else (avg(Irate)) end) as [Average Irate] from @tblprojects_WCOSL
select (case when (MAX(LOI)) is null then 0 else (MAX(LOI)) end) as [Max LOI],(case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min LOI],(case when (avg(LOI)) is null then 0 else (avg(LOI))end) as [Average LOI] from @tblprojects_WCOSL
select (case when (MAX(CPI)) is null then 0 else(MAX(CPI)) end) as [Max CPI],(case when (Min(CPI)) is null then 0 else (Min(CPI)) end) as [Min CPI], (case  when (avg(CPI)) is null then 0 else (avg(CPI)) end) as [Average CPI] from @tblprojects_WCOSL
declare @tblprojectsacloi_WCOSL as table (id int identity(1,1), startdate datetime, enddate datetime, LOI float )
insert into @tblprojectsacloi_WCOSL
select startdate,enddate,datediff(mi,startdate,enddate) from supplierprojects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and CountryID=@countryid  AND sp.Status='Complete' AND sp.StartDate IS NOT NULL AND sp.enddate IS NOT NULL AND datediff(d,sp.startdate,sp.enddate)<1
--select * from @tblprojectsacloi
select (case when (Max(LOI)) is null then 0 else (Max(LOI)) end ) as [Max Actual LOI] , ( case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min Actual LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average Actual LOI] from @tblprojectsacloi_WCOSL
end
else
begin
declare @tblprojects_WCOSL_D as table (id int identity(1,1) , PID int, Pname varchar(100), ClientId int, LOI float, IRate float, CPI float, CountryId int)
insert into @tblprojects_WCOSL_D
select distinct p.ProjectID, p.pname,p.clientid,p.loi,p.irate,p.CPI,p.CountryId from projectmapping pm left join projects p on pm.projectid=p.ProjectId where supplierid=@supplierid and p.countryid=@countryid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate)
--select * from @tblprojects_WCOSL_D
select (case when (MAX(IRate)) is null then 0 else (MAX(IRate)) end) as [Max Irate],(case when (Min(IRate)) is null then 0 else (Min(IRate))end) as [Min Irate],(case when (avg(Irate))is null then 0 else (avg(Irate)) end) as [Average Irate] from @tblprojects_WCOSL_D
select (case when (MAX(LOI)) is null then 0 else (MAX(LOI)) end) as [Max LOI],(case when (Min(LOI)) is null then 0 else(Min(LOI)) end) as [Min LOI],(case when  (avg(LOI))is null then 0 else(avg(LOI)) end) as [Average LOI] from @tblprojects_WCOSL_D
select (case when(MAX(CPI)) is null then 0 else (MAX(CPI)) end) as [Max CPI],(case when (Min(CPI)) is null then 0 else (Min(CPI)) end) as [Min CPI], (case when(avg(CPI)) is null then 0 else (avg(CPI)) end) as [Average CPI] from @tblprojects_WCOSL_D
declare @tblprojectsacloi_WCOSL_D as table (id int identity(1,1), startdate datetime, enddate datetime, LOI float )
insert into @tblprojectsacloi_WCOSL_D
select startdate,enddate,datediff(mi,startdate,enddate) from supplierprojects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and CountryID=@countryid and StartDate >= @startdate AND EndDate <= @enddate  AND sp.Status='Complete' AND sp.StartDate IS NOT NULL AND sp.enddate IS NOT NULL AND datediff(d,sp.startdate,sp.enddate)<1
--select * from @tblprojectsacloi
select (case when (Max(LOI))is null then 0 else(Max(LOI)) end) as [Max Actual LOI] , (case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min Actual LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average Actual LOI] from @tblprojectsacloi_WCOSL_D
end
end
if(@countryid=0 and @supplierid!=0 and @clientid!=0)
begin
if(@startdate='' and @enddate='')
begin
declare @tblprojects_WCLSL as table (id int identity(1,1) , PID int, Pname varchar(100), ClientId int, LOI float, IRate float, CPI float, CountryId int)
insert into @tblprojects_WCLSL
select distinct p.ProjectID, p.pname,p.clientid,p.loi,p.irate,p.CPI,p.CountryId from projectmapping pm left join projects p on pm.projectid=p.ProjectId where supplierid=@supplierid and p.clientid=@clientid
--select * from @tblprojects_WCLSL
select (case when (MAX(IRate)) is null then 0 else(MAX(IRate)) end) as [Max Irate],( case when (Min(IRate)) is null then 0 else (Min(IRate))end) as [Min Irate], (case when (avg(Irate)) is null then 0 else (avg(Irate)) end) as [Average Irate] from @tblprojects_WCLSL
select (case when (MAX(LOI))is null then 0 else(MAX(LOI)) end) as [Max LOI],(case when (Min(LOI))is null then 0 else (Min(LOI)) end) as [Min LOI], (case when (avg(LOI)) is null then 0 else(avg(LOI)) end) as [Average LOI] from @tblprojects_WCLSL
select (case when (MAX(CPI)) is null then 0 else (MAX(CPI)) end) as [Max CPI],(case when (Min(CPI)) is null then 0 else(Min(CPI)) end) as [Min CPI], (case when (avg(CPI)) is null then 0 else (avg(CPI))end)as [Average CPI] from @tblprojects_WCLSL
declare @tblprojectsacloi_WCLSL as table (id int identity(1,1), startdate datetime, enddate datetime, LOI float )
insert into @tblprojectsacloi_WCLSL
select startdate,enddate,datediff(mi,startdate,enddate) from supplierprojects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.SUpplierID=@supplierid  AND sp.Status='Complete' AND sp.StartDate IS NOT NULL AND sp.enddate IS NOT NULL AND datediff(d,sp.startdate,sp.enddate)<1
--select * from @tblprojectsacloi
select (case when (Max(LOI)) is null then 0 else(Max(LOI)) end) as [Max Actual LOI] , (case when (Min(LOI))is  null then 0 else (Min(LOI)) end) as [Min Actual LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average Actual LOI] from @tblprojectsacloi_WCLSL
end
else
begin
declare @tblprojects_WCLSL_D as table (id int identity(1,1) , PID int, Pname varchar(100), ClientId int, LOI float, IRate float, CPI float, CountryId int)
insert into @tblprojects_WCLSL_D
select distinct p.ProjectID, p.pname,p.clientid,p.loi,p.irate,p.CPI,p.CountryId from projectmapping pm left join projects p on pm.projectid=p.ProjectId where supplierid=@supplierid and p.clientid=@clientid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate)
--select * from @tblprojects_WCLSL_D
select (case when (MAX(IRate)) is null then 0 else(MAX(IRate)) end) as [Max Irate],(case when (Min(IRate)) is null then 0 else(Min(IRate)) end) as [Min Irate], (case when (avg(Irate)) is null then 0 else (avg(Irate)) end) as [Average Irate] from @tblprojects_WCLSL_D
select (case when (MAX(LOI)) is null then 0 else(MAX(LOI)) end) as [Max LOI],(case when (Min(LOI)) is null then 0 else (Min(LOI))  end) as [Min LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average LOI] from @tblprojects_WCLSL_D
select (case when (MAX(CPI)) is null then 0 else (MAX(CPI)) end) as [Max CPI],(case when (Min(CPI)) is null then 0 else(Min(CPI)) end) as [Min CPI], (case when (avg(CPI))is null then 0 else (avg(CPI)) end) as [Average CPI] from @tblprojects_WCLSL_D
declare @tblprojectsacloi_WCLSL_D as table (id int identity(1,1), startdate datetime, enddate datetime, LOI float )
insert into @tblprojectsacloi_WCLSL_D
select startdate,enddate,datediff(mi,startdate,enddate) from supplierprojects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.SUpplierID=@supplierid and StartDate >= @startdate AND EndDate <= @enddate  AND sp.Status='Complete' AND sp.StartDate IS NOT NULL AND sp.enddate IS NOT NULL AND datediff(d,sp.startdate,sp.enddate)<1
--select * from @tblprojectsacloi
select (case when (Max(LOI)) is null then 0 else (Max(LOI)) end) as [Max Actual LOI] , (case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min Actual LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average Actual LOI] from @tblprojectsacloi_WCLSL_D
end

end
if(@countryid!=0 and @supplierid=0 and @clientid!=0)
begin
if(@startdate='' and @enddate='')
begin
declare @tblprojects_WCOCL as table (id int identity(1,1) , PID int, Pname varchar(100), ClientId int, LOI float, IRate float, CPI float, CountryId int)
insert into @tblprojects_WCOCL
select distinct p.ProjectID, p.pname,p.clientid,p.loi,p.irate,p.CPI,p.CountryId from projectmapping pm left join projects p on pm.projectid=p.ProjectId where pm.countryid=@countryid and p.clientid=@clientid
--select * from @tblprojects_WCOCL
select (case when (MAX(IRate)) is null then 0 else(MAX(IRate)) end) as [Max Irate],(case when (Min(IRate)) is null then 0 else (Min(IRate)) end) as [Min Irate], (case when (avg(Irate)) is null then 0 else (avg(Irate)) end) as [Average Irate] from @tblprojects_WCOCL
select (case when (MAX(LOI)) is null then 0 else (MAX(LOI)) end) as [Max LOI],(case when (Min(LOI)) is null then 0 else(Min(LOI)) end) as [Min LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average LOI] from @tblprojects_WCOCL
select (case when (MAX(CPI)) is null then 0 else(MAX(CPI)) end) as [Max CPI],(case when (Min(CPI)) is null then 0 else (Min(CPI)) end) as [Min CPI], (case when (avg(CPI)) is null then 0 else(avg(CPI)) end) as [Average CPI] from @tblprojects_WCOCL
declare @tblprojectsacloi_WCOCL as table (id int identity(1,1), startdate datetime, enddate datetime, LOI float )
insert into @tblprojectsacloi_WCOCL
select startdate,enddate,datediff(mi,startdate,enddate) from supplierprojects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.CountryID=@countryid  AND sp.Status='Complete' AND sp.StartDate IS NOT NULL AND sp.enddate IS NOT NULL AND datediff(d,sp.startdate,sp.enddate)<1
--select * from @tblprojectsacloi
select (case when (Max(LOI)) is null then 0 else (Max(LOI)) end) as [Max Actual LOI] , (case when (Min(LOI)) is null then 0 else(Min(LOI)) end) as [Min Actual LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average Actual LOI] from @tblprojectsacloi_WCOCL
end
else
begin
declare @tblprojects_WCOCL_D as table (id int identity(1,1) , PID int, Pname varchar(100), ClientId int, LOI float, IRate float, CPI float, CountryId int)
insert into @tblprojects_WCOCL_D
select distinct p.ProjectID, p.pname,p.clientid,p.loi,p.irate,p.CPI,p.CountryId from projectmapping pm left join projects p on pm.projectid=p.ProjectId where pm.countryid=@countryid and p.clientid=@clientid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate)
--select * from @tblprojects_WCOCL_D
select (case when (MAX(IRate)) is null then 0 else (MAX(IRate)) end) as [Max Irate],(case when (Min(IRate)) is null then 0 else (Min(IRate)) end) as [Min Irate], (case when (avg(Irate)) is null then 0 else (avg(Irate)) end) as [Average Irate] from @tblprojects_WCOCL_D
select (case when (MAX(LOI)) is null then 0 else (MAX(LOI)) end) as [Max LOI],(case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average LOI] from @tblprojects_WCOCL_D
select (case when (MAX(CPI)) is null then 0 else (MAX(CPI)) end) as [Max CPI],(case when (Min(CPI)) is null then 0 else (Min(CPI)) end) as [Min CPI], avg(CPI) as [Average CPI] from @tblprojects_WCOCL_D
declare @tblprojectsacloi_WCOCL_d as table (id int identity(1,1), startdate datetime, enddate datetime, LOI float )
insert into @tblprojectsacloi_WCOCL_d
select startdate,enddate,datediff(mi,startdate,enddate) from supplierprojects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.CountryID=@countryid and StartDate >= @startdate AND EndDate <= @enddate  AND sp.Status='Complete' AND sp.StartDate IS NOT NULL AND sp.enddate IS NOT NULL AND datediff(d,sp.startdate,sp.enddate)<1
--select * from @tblprojectsacloi
select (case when (Max(LOI)) is null then 0 else (Max(LOI)) end) as [Max Actual LOI] , (case when (Min(LOI)) is null then 0 else(Min(LOI))  end) as [Min Actual LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average Actual LOI] from @tblprojectsacloi_WCOCL_d
end
end
if(@countryid!=0 and @supplierid!=0 and @clientid!=0)
begin
if(@startdate='' and @enddate='')
begin
declare @tblprojects_WCOCLSL as table (id int identity(1,1) , PID int, Pname varchar(100), ClientId int, LOI float, IRate float, CPI float, CountryId int)
insert into @tblprojects_WCOCLSL
select distinct p.ProjectID, p.pname,p.clientid,p.loi,p.irate,p.CPI,p.CountryId from projectmapping pm left join projects p on pm.projectid=p.ProjectId where pm.supplierid=@supplierid and  pm.countryid=@countryid and p.clientid=@clientid
--select * from @tblprojects_WCOCLSL
select (case when (MAX(IRate)) is null then 0 else (MAX(IRate)) end) as [Max Irate],(case when (Min(IRate)) is null then 0 else (Min(IRate)) end) as [Min Irate], (case when(avg(Irate)) is null then 0 else (avg(Irate)) end) as [Average Irate] from @tblprojects_WCOCLSL
select (case when (MAX(LOI)) is null then 0 else (MAX(LOI)) end) as [Max LOI],(case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average LOI] from @tblprojects_WCOCLSL
select (case when (MAX(CPI)) is null then 0 else (MAX(CPI)) end) as [Max CPI],(case when(Min(CPI)) is null then 0 else (Min(CPI)) end) as [Min CPI], (case when(avg(CPI)) is null then 0 else (avg(CPI)) end) as [Average CPI] from @tblprojects_WCOCLSL
declare @tblprojectsacloi_WCOCLSL as table (id int identity(1,1), startdate datetime, enddate datetime, LOI float )
insert into @tblprojectsacloi_WCOCLSL
select startdate,enddate,datediff(mi,startdate,enddate) from supplierprojects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where p.ClientID=@clientid and pm.CountryID=@countryid and pm.SUpplierID=@supplierid  AND sp.Status='Complete' AND sp.StartDate IS NOT NULL AND sp.enddate IS NOT NULL AND datediff(d,sp.startdate,sp.enddate)<1
--select * from @tblprojectsacloi
select (case when (Max(LOI)) is null then 0 else (Max(LOI)) end) as [Max Actual LOI] , (case when(Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min Actual LOI], (case when(avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average Actual LOI] from @tblprojectsacloi_WCOCLSL
end
else
begin
declare @tblprojects_WCOCLSL_D as table (id int identity(1,1) , PID int, Pname varchar(100), ClientId int, LOI float, IRate float, CPI float, CountryId int)
insert into @tblprojects_WCOCLSL_D
select distinct p.ProjectID, p.pname,p.clientid,p.loi,p.irate,p.CPI,p.CountryId from projectmapping pm left join projects p on pm.projectid=p.ProjectId where pm.supplierid=@supplierid and  pm.countryid=@countryid and p.clientid=@clientid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate)
--select * from @tblprojects_WCOCLSL_D
select (case when (MAX(IRate)) is null then 0 else (MAX(IRate)) end) as [Max Irate],(case when (Min(IRate)) is null then 0 else (Min(IRate)) end) as [Min Irate], (case when (avg(Irate)) is null then 0 else (avg(Irate)) end) as [Average Irate] from @tblprojects_WCOCLSL_D
select (case when (MAX(LOI)) is null then 0 else(MAX(LOI)) end) as [Max LOI],(case when(Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average LOI] from @tblprojects_WCOCLSL_D
select (case when(MAX(CPI)) is null then 0 else (MAX(CPI)) end) as [Max CPI],(case when (Min(CPI)) is null then 0 else(Min(CPI)) end) as [Min CPI], (case when (avg(CPI)) is null then 0 else (avg(CPI)) end) as [Average CPI] from @tblprojects_WCOCLSL_D
declare @tblprojectsacloi_WCOCLSL_D as table (id int identity(1,1), startdate datetime, enddate datetime, LOI float )
insert into @tblprojectsacloi_WCOCLSL_D
select startdate,enddate,datediff(mi,startdate,enddate) from supplierprojects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where p.ClientID=@clientid and pm.CountryID=@countryid and pm.SUpplierID=@supplierid and StartDate >= @startdate AND EndDate <= @enddate  AND sp.Status='Complete' AND sp.StartDate IS NOT NULL AND sp.enddate IS NOT NULL AND datediff(d,sp.startdate,sp.enddate)<1
--select * from @tblprojectsacloi
select (case when (Max(LOI)) is null then 0 else (Max(LOI)) end) as [Max Actual LOI] , (case when (Min(LOI)) is null then 0 else (Min(LOI)) end) as [Min Actual LOI], (case when (avg(LOI)) is null then 0 else (avg(LOI)) end) as [Average Actual LOI] from @tblprojectsacloi_WCOCLSL_D
end

end
exec [usp_getvalues] @countryid,@supplierid,@clientid,@startdate,@enddate
end
