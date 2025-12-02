
drop function if exists wordrank;

-- returns ranked list of words found in matching posts
create or replace function wordrank(searchtype text, searchstr text)
returns table (term text, rank decimal) as
$$
declare
	wordz text[];
	w text;
    q text :='';
begin
    select regexp_split_to_array(searchstr, '\s+')
	into wordz;
	if searchtype='wordstfidf' then
		q:='select word, sum(rank) from wi, (select id, round(sum(tfidf), 4) rank from (';
		foreach w in array wordz
		loop
			q := q || 'select distinct on (id, what, word) id, what, word, tfidf from wi_weighted where word = ''';
			q := q || w;
			q := q || '''  and (what=''title'' or what=''body'') union all ';
		end loop;
		select regexp_replace(q, '\sunion all\s$', '') --remove last union all
		into q;
		q := q || ') t1 group by id) t2 where wi.id=t2.id group by word order by sum desc;';
	elsif searchtype='wordsbest' then -- adapted from slides
		q:='select word, sum(rank)::decimal from wi, (select id, sum(relevance) rank from (';
		foreach w in array wordz
		loop
			q := q || 'select distinct id, 1 relevance from wi where word = ''';
			q := q || w;
			q := q || ''' union all ';
		end loop;
		select regexp_replace(q, '\sunion all\s$', '') --remove last union all
		into q;
		q := q || ') t1 group by id) t2 where wi.id=t2.id group by word order by sum desc;';
	raise notice 'Building query -- %', q;
	else 
		raise notice 'Unknown searchtype -- %', searchtype;
		return;
	end if;
	return query execute q;
end;
$$ 
language plpgsql;
