CREATE OR ALTER PROC [dbo].[respondants]
@countryid int=0,
@supplierid int=0,
@clientid int=0,
@startdate datetime= '2021-08-01',
@enddate datetime= '2021-08-28'
as
begin
if(@countryid=0 and @supplierid=0 and @clientid=0)
begin
if(@startdate='' and @enddate='')
begin
declare @successrate as table (id int identity(1,1), [Project Success Rate] int , [IR Success Rate] int)
insert into @successrate ([Project Success Rate],[IR Success Rate]) values(
(select ( case when 
(select COUNT(*) from Projects)>0
 then ((select COUNT(*) from Projects  where Status=1)*100/(select COUNT(*) from Projects)) 
 else 0 end) as [Project Success Rate]),
(select ( case when 
(select COUNT(*) from SupplierProjects)>0
 then ((select COUNT(*) from SupplierProjects where Status='Complete')*100/(select COUNT(*) from SupplierProjects)) 
 else 0 end) as [IR Success Rate])
)

select * from @successrate



declare @totalrespondants as table (id int identity(1,1), Total int, Complete int, Incomplete int, Screened int, Quotafull int, Terminate int)

insert into @totalrespondants (Total,Complete,Incomplete,Screened,Quotafull,Terminate) values(
(select count(*) from SupplierProjects),

(select count(*) from SupplierProjects where Status='Complete'),

(select count(*) from SupplierProjects where Status='Incomplete'),

(select count(*) from SupplierProjects where Status='Screened'),

(select COUNT(*) from SupplierProjects where Status='OverQuota'),
(select COUNT(*) from SupplierProjects where Status='Terminate')


)

select * from @totalrespondants

end

else

begin

declare @successrated1 as table (id int identity(1,1), [Project Success Rate] int , [IR Success Rate] int)

insert into @successrated1 ([Project Success Rate],[IR Success Rate]) values(

(select ( case when 

(select COUNT(*) from Projects where CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))>0

 then ((select COUNT(*) from Projects  where Status=1 and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))*100/(select COUNT(*) from Projects where CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))) 

 else 0 end) as [Project Success Rate]),

(select ( case when 

(select COUNT(*) from SupplierProjects where StartDate >= @startdate AND EndDate <= @enddate)>0

 then ((select COUNT(*) from SupplierProjects where Status='Complete' and StartDate >= @startdate AND EndDate <= @enddate)*100/(select COUNT(*) from SupplierProjects where StartDate >= @startdate AND EndDate <= @enddate)) 

 else 0 end) as [IR Success Rate])

)

select * from @successrated1

declare @totalrespondantsd1 as table (id int identity(1,1), Total int, Complete int, Incomplete int, Screened int, Quotafull int,Terminate int)

insert into @totalrespondantsd1 (Total,Complete,Incomplete,Screened,Quotafull,Terminate) values(

(select count(*) from SupplierProjects where StartDate >= @startdate AND EndDate <= @enddate),

(select count(*) from SupplierProjects where Status='Complete' and  StartDate >= @startdate AND EndDate <= @enddate),
(select count(*) from SupplierProjects where Status='Incomplete' and  StartDate >= @startdate AND EndDate <= @enddate),
(select count(*) from SupplierProjects where Status='Screened' and  StartDate >= @startdate AND EndDate <= @enddate),
(select COUNT(*) from SupplierProjects where Status='OverQuota' and StartDate >= @startdate AND EndDate <= @enddate),
(select COUNT(*) from SupplierProjects where Status='Terminate' and StartDate >= @startdate AND EndDate <= @enddate)
)

select * from @totalrespondantsd1

end

end

--country filter

if(@countryid!=0 and @supplierid=0 and @clientid=0 )

begin

if(@startdate='' and @enddate='')

begin

declare @successrate1 as table (id int identity(1,1), [Project Success Rate] int , [IR Success Rate] int)

insert into @successrate1 ([Project Success Rate],[IR Success Rate]) values(

(select ( case when 

(select COUNT(*) from Projects where CountryId=@countryid)>0

 then ((select COUNT(*) from Projects where CountryId=@countryid and Status=1)*100/(select COUNT(*) from Projects where CountryId=@countryid)) 

 else 0 end) as [Project Success Rate]),

(

select ( case when 

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid)>0

 then ((select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid and sp.Status='Complete')*100/

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid)) 

 else 0 end) as [IR Success Rate]

)

)

select * from @successrate1



declare @totalrespondants1 as table (id int identity(1,1), Total int, Complete int, Incomplete int, Screened int, Quotafull int,Terminate int)

insert into @totalrespondants1(Total,Complete,Incomplete,Screened,Quotafull,Terminate) values(

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid),

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid and sp.Status='Complete'),

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid and sp.Status='Incomplete'),

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid and sp.Status='Screened'),

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid and sp.Status='OverQuota'),
(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid and sp.Status='Terminate')

)

select * from @totalrespondants1

end

else

begin

declare @successrate1d1 as table (id int identity(1,1), [Project Success Rate] int , [IR Success Rate] int)

insert into @successrate1d1 ([Project Success Rate],[IR Success Rate]) values(

(select ( case when 

(select COUNT(*) from Projects where CountryId=@countryid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))>0

 then ((select COUNT(*) from Projects where CountryId=@countryid and Status=1 and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))*100/(select COUNT(*) from Projects where CountryId=@countryid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))) 

 else 0 end) as [Project Success Rate]),

(

select ( case when 

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid and StartDate >= @startdate AND EndDate <= @enddate)>0

 then ((select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid and sp.Status='Complete' and StartDate >= @startdate AND EndDate <= @enddate)*100/

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid and StartDate >= @startdate AND EndDate <= @enddate)) 

 else 0 end) as [IR Success Rate]

)

)

select * from @successrate1d1



declare @totalrespondants1d1 as table (id int identity(1,1), Total int, Complete int, Incomplete int, Screened int, Quotafull int,Terminate int)

insert into @totalrespondants1d1(Total,Complete,Incomplete,Screened,Quotafull,Terminate) values(

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid  and StartDate >= @startdate AND EndDate <= @enddate),

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid and sp.Status='Complete' and StartDate >= @startdate AND EndDate <= @enddate),

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid and sp.Status='Incomplete' and StartDate >= @startdate AND EndDate <= @enddate),

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid and sp.Status='Screened' and StartDate >= @startdate AND EndDate <= @enddate),

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid and sp.Status='OverQuota' and StartDate >= @startdate AND EndDate <= @enddate),
(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.CountryID=@countryid and sp.Status='Terminate' and StartDate >= @startdate AND EndDate <= @enddate)

)

select * from @totalrespondants1d1

end

end

--supplierfilter

if(@countryid=0 and @supplierid!=0 and @clientid=0)

begin

if(@startdate='' and @enddate='')

begin

declare @successrate2 as table (id int identity(1,1), [Project Success Rate] int , [IR Success Rate] int)

insert into @successrate2 ([Project Success Rate],[IR Success Rate]) values(

(

select ( case when 

(select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId=pm.ProjectID where SUpplierID=@supplierid)>0

 then ((select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId=pm.ProjectID where SUpplierID=@supplierid and p.Status=1)

*100/(select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId=pm.ProjectID where SUpplierID=@supplierid)) 

 else 0 end) as [Project Success Rate]

),

(

 select ( case when 

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid)>0

 then ((select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and sp.Status='Complete')

*100/(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid)) 

 else 0 end) as [IR Success Rate]

)

)

select * from @successrate2



declare @totalrespondants2 as table (id int identity(1,1), Total int, Complete int, Incomplete int, Screened int, Quotafull int,Terminate int)

insert into @totalrespondants2(Total,Complete,Incomplete,Screened,Quotafull,Terminate) values(

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and sp.Status='Complete'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and sp.Status='Incomplete'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and sp.Status='Screened'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and sp.Status='OverQuota'),
(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and sp.Status='Terminate')

)

select * from @totalrespondants2

end

ELSE

BEGIN

declare @successrate2d1 as table (id int identity(1,1), [Project Success Rate] int , [IR Success Rate] int)

insert into @successrate2d1 ([Project Success Rate],[IR Success Rate]) values(

(

select ( case when 

(select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId=pm.ProjectID where SUpplierID=@supplierid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))>0

 then ((select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId=pm.ProjectID where SUpplierID=@supplierid and p.Status=1 and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))

*100/(select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId=pm.ProjectID where SUpplierID=@supplierid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))) 

 else 0 end) as [Project Success Rate]

),

(

 select ( case when 

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and StartDate >= @startdate AND EndDate <= @enddate)>0

 then ((select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and sp.Status='Complete' and StartDate >= @startdate AND EndDate <= @enddate)

*100/(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and StartDate >= @startdate AND EndDate <= @enddate)) 

 else 0 end) as [IR Success Rate]

)

)

select * from @successrate2d1



declare @totalrespondants2d1 as table (id int identity(1,1), Total int, Complete int, Incomplete int, Screened int, Quotafull int,Terminate int)

insert into @totalrespondants2d1(Total,Complete,Incomplete,Screened,Quotafull,Terminate) values(

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and sp.Status='Complete'   and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and sp.Status='Incomplete'  and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and sp.Status='Screened'  and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and sp.Status='OverQuota'  and StartDate >= @startdate AND EndDate <= @enddate),
(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and sp.Status='Terminate'  and StartDate >= @startdate AND EndDate <= @enddate)

)

select * from @totalrespondants2d1

end

end

--clientfilter

if(@countryid=0 and @supplierid=0 and @clientid!=0)

begin

if(@startdate='' and @enddate='')

begin

declare @successrate3 as table (id int identity(1,1), [Project Success Rate] int , [IR Success Rate] int)

insert into @successrate3 ([Project Success Rate],[IR Success Rate]) values(

(select ( case when 

(select COUNT(*) from Projects where ClientID=@clientid)>0

 then ((select COUNT(*) from Projects where ClientID=ClientID and Status=1)*100/(select COUNT(*) from Projects where ClientID=@clientid)) 

 else 0 end) as [Project Success Rate]

),

( select ( case when 

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId where p.ClientID=@clientid)>0

 then ((select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId where p.ClientID=@clientid and sp.Status='Complete')*100

/(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId where p.ClientID=@clientid)) 

 else 0 end) as [IR Success Rate]))

select * from @successrate3



declare @totalrespondants3 as table (id int identity(1,1), Total int, Complete int, Incomplete int, Screened int, Quotafull int,Terminate int)

insert into @totalrespondants3(Total,Complete,Incomplete,Screened,Quotafull,Terminate) values(

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and sp.Status='Complete'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and sp.Status='Incomplete'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and sp.Status='Screened'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and sp.Status='OverQuota'),
(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and sp.Status='Terminate')

)

select * from @totalrespondants3

end

else

begin

declare @successrate3d1 as table (id int identity(1,1), [Project Success Rate] int , [IR Success Rate] int)

insert into @successrate3d1 ([Project Success Rate],[IR Success Rate]) values(

(select ( case when 

(select COUNT(*) from Projects where ClientID=@clientid  and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))>0

 then ((select COUNT(*) from Projects where ClientID=ClientID and Status=1 and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))*100/(select COUNT(*) from Projects where ClientID=@clientid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))) 

 else 0 end) as [Project Success Rate]

),

( select ( case when 

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId where p.ClientID=@clientid and StartDate >= @startdate AND EndDate <= @enddate )>0

 then ((select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId where p.ClientID=@clientid and sp.Status='Complete' and StartDate >= @startdate AND EndDate <= @enddate)*100

/(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId where p.ClientID=@clientid and StartDate >= @startdate AND EndDate <= @enddate)) 

 else 0 end) as [IR Success Rate])

)

select * from @successrate3d1



declare @totalrespondants3d1 as table (id int identity(1,1), Total int, Complete int, Incomplete int, Screened int, Quotafull int,Terminate int)

insert into @totalrespondants3d1(Total,Complete,Incomplete,Screened,Quotafull,Terminate) values(

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and sp.Status='Complete' and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and sp.Status='Incomplete' and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and sp.Status='Screened' and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and sp.Status='OverQuota' and StartDate >= @startdate AND EndDate <= @enddate),
(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and sp.Status='Terminate' and StartDate >= @startdate AND EndDate <= @enddate)


)

select * from @totalrespondants3d1

end

end

--country and supplier

if(@countryid!=0 and @supplierid!=0 and @clientid=0)

begin

if(@startdate='' and @enddate='')

begin

declare @successrate4 as table (id int identity(1,1), [Project Success Rate] int , [IR Success Rate] int)

insert into @successrate4 ([Project Success Rate],[IR Success Rate]) values(

(

select ( case when 

(select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId=pm.ProjectID where SUpplierID=@supplierid and p.CountryId=@countryid)>0

 then ((select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId=pm.ProjectID where SUpplierID=@supplierid and p.CountryId=@countryid and p.Status=1)

*100/(select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId=pm.ProjectID where SUpplierID=@supplierid and p.CountryId=@countryid)) 

 else 0 end) as [Project Success Rate]

),

(

 select ( case when 

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and pm.CountryID=@countryid)>0

 then ((select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and pm.CountryID=@countryid and sp.Status='Complete')

*100/(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and pm.CountryID=@countryid)) 

 else 0 end) as [IR Success Rate]

)

)

select * from @successrate4



declare @totalrespondants4 as table (id int identity(1,1), Total int, Complete int, Incomplete int, Screened int, Quotafull int,Terminate int)

insert into @totalrespondants4(Total,Complete,Incomplete,Screened,Quotafull, Terminate) values(

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and CountryID=@countryid),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and CountryID=@countryid and sp.Status='Complete'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and CountryID=@countryid and sp.Status='Incomplete'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and CountryID=@countryid and sp.Status='Screened'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and CountryID=@countryid and sp.Status='OverQuota'),
(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and CountryID=@countryid and sp.Status='Terminate')

)

select * from @totalrespondants4

end

else

begin

declare @successrate4d1 as table (id int identity(1,1), [Project Success Rate] int , [IR Success Rate] int)

insert into @successrate4d1 ([Project Success Rate],[IR Success Rate]) values(

(

select ( case when 

(select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId=pm.ProjectID where SUpplierID=@supplierid and p.CountryId=@countryid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))>0

 then ((select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId=pm.ProjectID where SUpplierID=@supplierid and p.CountryId=@countryid and p.Status=1 and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))

*100/(select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId=pm.ProjectID where SUpplierID=@supplierid and p.CountryId=@countryid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate)))

 else 0 end) as [Project Success Rate]

),

(

 select ( case when 

(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and pm.CountryID=@countryid and StartDate >= @startdate AND EndDate <= @enddate)>0

 then ((select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and pm.CountryID=@countryid and sp.Status='Complete' and StartDate >= @startdate AND EndDate <= @enddate)

*100/(select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and pm.CountryID=@countryid and StartDate >= @startdate AND EndDate <= @enddate)) 

 else 0 end) as [IR Success Rate]

)

)



select * from @successrate4d1



declare @totalrespondants4d1 as table (id int identity(1,1), Total int, Complete int, Incomplete int, Screened int, Quotafull int,Terminate int)

insert into @totalrespondants4d1(Total,Complete,Incomplete,Screened,Quotafull,Terminate) values(

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and CountryID=@countryid and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and CountryID=@countryid and sp.Status='Complete' and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and CountryID=@countryid and sp.Status='Incomplete' and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and CountryID=@countryid and sp.Status='Screened' and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and CountryID=@countryid and sp.Status='OverQuota' and StartDate >= @startdate AND EndDate <= @enddate),
(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID where pm.SUpplierID=@supplierid and CountryID=@countryid and sp.Status='Terminate' and StartDate >= @startdate AND EndDate <= @enddate)

)

select * from @totalrespondants4d1

end



end

--supplier client

if(@countryid=0 and @supplierid!=0 and @clientid!=0)

begin

if(@startdate='' and @enddate='')

begin

declare @successrate5 as table (id int identity(1,1), [Project Success Rate] int , [IR Success Rate] int)

insert into @successrate5 ([Project Success Rate],[IR Success Rate]) values(

(

select ( case when 

(select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId = pm.ProjectID where p.ClientID=@clientid and pm.SUpplierID=@supplierid)>0

 then ((select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId = pm.ProjectID where p.ClientID=@clientid and pm.SUpplierID=@supplierid and p.Status=1)

*100/(select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId = pm.ProjectID where p.ClientID=@clientid and pm.SUpplierID=@supplierid)) 

 else 0 end) as [Project Success Rate]

),

(

select ( case when 

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId where

  pm.SUpplierID=@supplierid and p.ClientID=@clientid)>0

 then ((select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId where pm.SUpplierID=@supplierid and p.ClientID=@clientid

 and sp.Status='Complete')*100/(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId where

  pm.SUpplierID=@supplierid and p.ClientID=@clientid)) 

 else 0 end) as [IR Success Rate]

)

)

select * from @successrate5



declare @totalrespondants5 as table (id int identity(1,1), Total int, Complete int, Incomplete int, Screened int, Quotafull int,Terminate int)

insert into @totalrespondants5(Total,Complete,Incomplete,Screened,Quotafull,Terminate) values(

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.SUpplierID=@supplierid),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.SUpplierID=@supplierid and sp.Status='Complete'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.SUpplierID=@supplierid and sp.Status='Incomplete'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.SUpplierID=@supplierid and sp.Status='Screened'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.SUpplierID=@supplierid and sp.Status='OverQuota'),
(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.SUpplierID=@supplierid and sp.Status='Terminate')

)

select * from @totalrespondants5

end

else

begin

declare @successrate5d1 as table (id int identity(1,1), [Project Success Rate] int , [IR Success Rate] int)

insert into @successrate5d1 ([Project Success Rate],[IR Success Rate]) values(

(

select ( case when(select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId = pm.ProjectID where p.ClientID=@clientid and pm.SUpplierID=@supplierid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))>0

 then ((select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId = pm.ProjectID where p.ClientID=@clientid and pm.SUpplierID=@supplierid and p.Status=1 and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))

*100/(select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId = pm.ProjectID where p.ClientID=@clientid and pm.SUpplierID=@supplierid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate)))

 else 0 end) as [Project Success Rate]

),

(

select ( case when 

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId where

  pm.SUpplierID=@supplierid and p.ClientID=@clientid and StartDate >= @startdate AND EndDate <= @enddate)>0

 then ((select count(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId where pm.SUpplierID=@supplierid and p.ClientID=@clientid

 and sp.Status='Complete' and StartDate >= @startdate AND EndDate <= @enddate)*100/(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId where

  pm.SUpplierID=@supplierid and p.ClientID=@clientid and StartDate >= @startdate AND EndDate <= @enddate)) 

 else 0 end) as [IR Success Rate]

)

)

select * from @successrate5d1





declare @totalrespondants5d1 as table (id int identity(1,1), Total int, Complete int, Incomplete int, Screened int, Quotafull int,Terminate int)

insert into @totalrespondants5d1(Total,Complete,Incomplete,Screened,Quotafull,Terminate) values(

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.SUpplierID=@supplierid and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.SUpplierID=@supplierid and sp.Status='Complete' and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.SUpplierID=@supplierid and sp.Status='Incomplete' and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.SUpplierID=@supplierid and sp.Status='Screened' and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.SUpplierID=@supplierid and sp.Status='OverQuota' and StartDate >= @startdate AND EndDate <= @enddate),
(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.SUpplierID=@supplierid and sp.Status='Terminate' and StartDate >= @startdate AND EndDate <= @enddate)

)

select * from @totalrespondants5d1

end

end

--country and client

if(@countryid!=0 and @supplierid=0 and @clientid!=0)

begin

if(@startdate='' and @enddate='')

begin

declare @successrate6 as table (id int identity(1,1), [Project Success Rate] int , [IR Success Rate] int)

insert into @successrate6 ([Project Success Rate],[IR Success Rate]) values(

(

select ( case when 

(select COUNT(*) from Projects  where ClientID=@clientid and CountryId=@countryid)>0

 then ((select COUNT(*) from Projects where ClientID=@clientid and CountryId=@countryid and Status=1)

*100/(select COUNT(*) from Projects  where ClientID=@clientid and CountryId=@countryid)) 

 else 0 end) as [Project Success Rate]

),

(

select ( case when 

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on

  pm.ProjectID=p.ProjectId where pm.CountryID=@countryid and p.ClientID=@clientid)>0

 then ((select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId where pm.CountryID=@countryid

 and p.ClientID=@clientid and sp.Status='Complete')*100/(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on

  pm.ProjectID=p.ProjectId where pm.CountryID=@countryid and p.ClientID=@clientid)) 

 else 0 end) as [IR Success Rate]

)

)

select * from @successrate6



declare @totalrespondants6 as table (id int identity(1,1), Total int, Complete int, Incomplete int, Screened int, Quotafull int,Terminate int)

insert into @totalrespondants6(Total,Complete,Incomplete,Screened,Quotafull,Terminate) values(

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.CountryID=@countryid),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.CountryID=@countryid and sp.Status='Complete'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.CountryID=@countryid and sp.Status='Incomplete'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.CountryID=@countryid and sp.Status='Screened'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.CountryID=@countryid and sp.Status='OverQuota'),
(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.CountryID=@countryid and sp.Status='Terminate')

)

select * from @totalrespondants6

end

else

begin

declare @successrate6d1 as table (id int identity(1,1), [Project Success Rate] int , [IR Success Rate] int)

insert into @successrate6d1 ([Project Success Rate],[IR Success Rate]) values(

(

select ( case when 

(select COUNT(*) from Projects  where ClientID=@clientid and CountryId=@countryid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))>0

 then ((select COUNT(*) from Projects where ClientID=@clientid and CountryId=@countryid and Status=1 and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))

*100/(select COUNT(*) from Projects  where ClientID=@clientid and CountryId=@countryid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))) 

 else 0 end) as [Project Success Rate]

),

(

select ( case when 

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on

  pm.ProjectID=p.ProjectId where pm.CountryID=@countryid and p.ClientID=@clientid and StartDate >= @startdate AND EndDate <= @enddate)>0

 then ((select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId where pm.CountryID=@countryid

 and p.ClientID=@clientid and sp.Status='Complete' and StartDate >= @startdate AND EndDate <= @enddate)*100/(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on

  pm.ProjectID=p.ProjectId where pm.CountryID=@countryid and p.ClientID=@clientid and StartDate >= @startdate AND EndDate <= @enddate)) 

 else 0 end) as [IR Success Rate]

)

)

select * from @successrate6d1



declare @totalrespondants6d1 as table (id int identity(1,1), Total int, Complete int, Incomplete int, Screened int, Quotafull int,Terminate int)

insert into @totalrespondants6d1(Total,Complete,Incomplete,Screened,Quotafull,Terminate) values(

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.CountryID=@countryid and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.CountryID=@countryid and sp.Status='Complete' and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.CountryID=@countryid and sp.Status='Incomplete' and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.CountryID=@countryid and sp.Status='Screened' and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.CountryID=@countryid and sp.Status='OverQuota' and StartDate >= @startdate AND EndDate <= @enddate),
(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where ClientID=@clientid and pm.CountryID=@countryid and sp.Status='Terminate' and StartDate >= @startdate AND EndDate <= @enddate)

)

select * from @totalrespondants6d1

end

end

--country supplier client

if(@countryid!=0 and @supplierid!=0 and @clientid!=0)

begin

if(@startdate='' and @enddate='')

begin

declare @successrate7 as table (id int identity(1,1), [Project Success Rate] int , [IR Success Rate] int)

insert into @successrate7 ([Project Success Rate],[IR Success Rate]) values(

(

select ( case when 

(select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId = pm.ProjectID where p.ClientID=@clientid and pm.SUpplierID=@supplierid and p.CountryId=@countryid)>0

 then ((select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId = pm.ProjectID where p.ClientID=@clientid and pm.SUpplierID=@supplierid and p.CountryId=@countryid and p.Status=1)

*100/(select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId = pm.ProjectID where p.ClientID=@clientid and pm.SUpplierID=@supplierid and p.CountryId=@countryid)) 

 else 0 end) as [Project Success Rate]

 ),

(

select ( case when 

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId

  where pm.CountryID=@countryid and p.ClientID=@clientid and pm.SUpplierID=@supplierid)>0

 then ((select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId where pm.CountryID=@countryid and p.ClientID=@clientid

 and pm.SUpplierID=@supplierid and sp.Status='Complete')*100/(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId

  where pm.CountryID=@countryid and p.ClientID=@clientid and pm.SUpplierID=@supplierid)) 

 else 0 end) as [IR Success Rate]

 )

)

select * from @successrate7



declare @totalrespondants7 as table (id int identity(1,1), Total int, Complete int, Incomplete int, Screened int, Quotafull int,Terminate int)

insert into @totalrespondants7(Total,Complete,Incomplete,Screened,Quotafull,Terminate) values(

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where p.ClientID=@clientid and pm.CountryID=@countryid and pm.SUpplierID=@supplierid),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where p.ClientID=@clientid and pm.CountryID=@countryid and pm.SUpplierID=@supplierid and sp.Status='Complete'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where p.ClientID=@clientid and pm.CountryID=@countryid and pm.SUpplierID=@supplierid and sp.Status='Incomplete'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where p.ClientID=@clientid and pm.CountryID=@countryid and pm.SUpplierID=@supplierid and sp.Status='Screened'),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where p.ClientID=@clientid and pm.CountryID=@countryid and pm.SUpplierID=@supplierid and sp.Status='OverQuota'),
(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where p.ClientID=@clientid and pm.CountryID=@countryid and pm.SUpplierID=@supplierid and sp.Status='Terminate')

)

select * from @totalrespondants7

end

else

begin

declare @successrate7d1 as table (id int identity(1,1), [Project Success Rate] int , [IR Success Rate] int)

insert into @successrate7d1 ([Project Success Rate],[IR Success Rate]) values(

(

select ( case when 

(select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId = pm.ProjectID where p.ClientID=@clientid and pm.SUpplierID=@supplierid and p.CountryId=@countryid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))>0

 then ((select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId = pm.ProjectID where p.ClientID=@clientid and pm.SUpplierID=@supplierid and p.CountryId=@countryid and p.Status=1 and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate))

*100/(select COUNT(*) from Projects p left join ProjectMapping pm on p.ProjectId = pm.ProjectID where p.ClientID=@clientid and pm.SUpplierID=@supplierid and p.CountryId=@countryid and CONVERT(date, TRY_CONVERT(datetime, SDate)) >= CONVERT(date, @startdate) AND CONVERT(date, TRY_CONVERT(datetime, EDate)) <= CONVERT(date, @enddate)))

 else 0 end) as [Project Success Rate]

 ),

(

select ( case when 

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId

  where pm.CountryID=@countryid and p.ClientID=@clientid and pm.SUpplierID=@supplierid and StartDate >= @startdate AND EndDate <= @enddate)>0

 then ((select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId where pm.CountryID=@countryid and p.ClientID=@clientid

 and pm.SUpplierID=@supplierid and sp.Status='Complete' and StartDate >= @startdate AND EndDate <= @enddate)*100/(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID=pm.SID left join Projects p on pm.ProjectID=p.ProjectId

  where pm.CountryID=@countryid and p.ClientID=@clientid and pm.SUpplierID=@supplierid and StartDate >= @startdate AND EndDate <= @enddate)) 

 else 0 end) as [IR Success Rate]

 )

)

select * from @successrate7d1



declare @totalrespondants7d1 as table (id int identity(1,1), Total int, Complete int, Incomplete int, Screened int, Quotafull int,Terminate int)

insert into @totalrespondants7d1(Total,Complete,Incomplete,Screened,Quotafull,Terminate) values(

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where p.ClientID=@clientid and pm.CountryID=@countryid and pm.SUpplierID=@supplierid and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where p.ClientID=@clientid and pm.CountryID=@countryid and pm.SUpplierID=@supplierid and sp.Status='Complete' and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where p.ClientID=@clientid and pm.CountryID=@countryid and pm.SUpplierID=@supplierid and sp.Status='Incomplete' and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where p.ClientID=@clientid and pm.CountryID=@countryid and pm.SUpplierID=@supplierid and sp.Status='Screened' and StartDate >= @startdate AND EndDate <= @enddate),

(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where p.ClientID=@clientid and pm.CountryID=@countryid and pm.SUpplierID=@supplierid and sp.Status='OverQuota' and StartDate >= @startdate AND EndDate <= @enddate),
(select COUNT(*) from SupplierProjects sp left join ProjectMapping pm on sp.SID = pm.SID left join Projects p on pm.ProjectID =p.ProjectId where p.ClientID=@clientid and pm.CountryID=@countryid and pm.SUpplierID=@supplierid and sp.Status='Terminate' and StartDate >= @startdate AND EndDate <= @enddate)

)

select * from @totalrespondants7d1

end

end

end
