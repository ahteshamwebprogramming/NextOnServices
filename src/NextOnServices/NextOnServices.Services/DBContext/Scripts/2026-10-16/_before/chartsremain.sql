CREATE proc [dbo].[chartsremain]
as
begin
declare @monts datetime=getdate(), @onemonth int, @twomonth int, @thridmonth int, @currentmonth datetime, @prevmonth datetime, @prevprevmonth datetime,@thirdyear int, @twoyear int, @oneyear int
SET @currentmonth=getdate()
SET @prevmonth=(SELECT DATEADD(month, -1, @currentmonth))
SET @prevprevmonth=(SELECT DATEADD(month,-1,@prevmonth))
SET @thridmonth=month(@currentmonth)
SET @thirdyear=year(@currentmonth)
SET @twomonth=month(@prevmonth)
SET @twoyear=year(@prevmonth)
SET @onemonth=month(@prevprevmonth)
SET @oneyear=year(@prevprevmonth)
--print @onemonth
--print @twomonth
--print @thridmonth
declare @bartable as table (id int identity(1,1), Months varchar(20),Completes int)

insert into @bartable(months,completes) values((Select left(DateName( month , DateAdd( month , @onemonth , 0 ) -1),3)),(select Count(*) from supplierprojects WHERE month(convert(date,startdate))=@onemonth and month((Convert(date,enddate)))=@onemonth AND year((Convert(date,enddate)))=@oneyear and status='Complete'))

insert into @bartable(months,completes) values((Select left(DateName( month , DateAdd( month , @twomonth , 0 ) - 1 ),3)),(select Count(*) from supplierprojects WHERE month(convert(date,startdate))=@twomonth and month((Convert(date,enddate)))=@twomonth AND year((Convert(date,enddate)))=@twoyear and status='Complete'))

insert into @bartable(months,completes) values((Select left(DateName( month , DateAdd( month , @thridmonth , 0 ) - 1 ),3)),(select Count(*) from supplierprojects WHERE month(convert(date,startdate))=@thridmonth and month((Convert(date,enddate)))=@thridmonth AND year((Convert(date,enddate)))=@thirdyear and status='Complete'))



declare @completedata as table (id int identity(1,1) , CPI int)

insert into @completedata
select CPI from projects where month((Convert(date,creationdate)))=@onemonth and status=1

insert into @completedata
select CPI from projects where month((Convert(date,creationdate)))=@twomonth and status=1

insert into @completedata
select CPI from projects where month((Convert(date,creationdate)))=@thridmonth and status=1

--select * from @completedata

select * from @bartable

declare @pietable as table (id int identity(1,1), Label varchar(20), Minn int , Maxx int ,Mean int)

declare @min int=(select (case when (Min(CPI)) is null then 0 else (Min(CPI)) end) from @completedata), @max int=(select (case when (MAX(CPI)) is null then 0 else (MAX(CPI)) end) from @completedata),@avg int=(select (case when (avg(CPI)) is null then 0 else (avg(CPI))end) from @completedata)

declare @total int=(@min+@max+@avg)

insert into @pietable (Label,Minn,Maxx,Mean) values ('Percent',((case when (@total)=0 then 0 else (@min*100/@total) end)),(case when (@total)=0 then 0 else (@max*100/@total) end),(case when (@total)=0 then 0 else (@avg*100/@total)end))

insert into @pietable (Label,Minn,Maxx,Mean) values ('CPI', @min, @max,@avg)



select * from @pietable



end





