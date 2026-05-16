select *
from weather_conditions wc
where wc.cloud_cover > 10
    and wc.cloud_cover < 40

select c.column_name, * from information_schema.columns c
where c.table_schema = 'public'
where table_name = 'match_details'

select * from weather_conditions

select * from match_details

select * from stadiums

--teams with 2 matches
select md.home_team_id, t.name, count(*) from match_details md
inner join teams t on md.home_team_id = t.id
group by md.home_team_id, t.name
having count(*) = 2

select m.*, l.name from league_external_maps m
inner join leagues l on m.league_id = l.id
where m.data_source = 'apifootball'

select * from leagues l
inner join countries c on l.country_id = c.id

select l.name, COUNT(*) from leagues l
group by l.name

select * from countries c
left join leagues l on c.id = l.country_id

select * from leagues l
left join countries c on l.country_id = c.id

select 
    c.id,
    c.name,
    count(*) as league_count
from leagues l
left join countries c
    on c.id = l.country_id
--where c.name = 'England'
group by 
    c.id,
    c.name
having count(*) > 5;

select l.name, c.name, count(*) from leagues l
inner join countries c on l.country_id = c.id
GROUP BY l.name, c.name
having COUNT(*) > 1


select * from match_details md
where md.match_date = current_date
order by md.match_time desc

select * from teams
where stadium_id is null

