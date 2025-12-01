-- helper functions
create or alter function dbo.modulo
(
    @dividend bigint,
    @divisor bigint
)
returns bigint
with schemabinding
as
begin
    declare @remainder bigint = @dividend % @divisor;
    return case
               when @remainder < 0 then @remainder + @divisor
               else @remainder
           end;
end;
go
-------------------

declare @is_example bit = 1;
declare @debug bit = 0;

drop table if exists #input;
create table #input (
    id int primary key,
    line nvarchar(max)
);

if @is_example = 1
begin
    declare @example nvarchar(max) = N'L68
L30
R48
L5
R60
L55
L1
L99
R14
L82';
    insert into #input (id, line)
    select ordinal, value
    from string_split(@example, char(10), 1);
end
else
begin
    bulk insert #input
    from 'C:\development\adventofcode\2025\input\1.txt'
    with (
        formatfile = 'C:\development\adventofcode\2025\format.fmt'
    );
end;

drop table if exists #parsed;
create table #parsed (
    id int primary key,
    direction char(1),
    distance int
);

if (@debug = 1) select * from #input;

-- solution
insert into #parsed (id, direction, distance)
select
    id,
    left(ltrim(line), 1) as direction,
    cast(replace(replace(substring(line, 2, 8000), char(13), ''), char(10), '') as int) as distance
from #input

union

select 0 as id,
    'R' as direction,
    50 as distance;

if (@debug = 1) select * from #parsed;

with running_total as (
    select sum(
        case
            when x.direction = 'L' then -1 * x.distance
            else x.distance
        end) over(order by id) as running_total,
        x.distance,
        x.direction
    from #parsed as x
)
select *, (
            (case
                when t.direction = 'L' then dbo.modulo(t.running_total, 100)
                else 100 - (dbo.modulo(t.running_total, 100))
            end)
            + t.distance
        ) / 100 from running_total as t;
/*
select 'password', count(case when (t.running_total + @start) % 100 = 0 then 1 else null end),
    '0x434C49434B password',
    sum(
        (
            (case
                when t.direction = 'R' then t.running_total % 100
                else 100 - (t.running_total % 100)
            end)
            + t.distance
        ) / 100
    )
from running_total as t;
*/

drop table #input;
drop table #parsed;