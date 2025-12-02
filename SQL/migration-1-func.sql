-- GROUP: raw6, MEMBERS: Mads Zeuch Ethelberg, Monica Toader, Stefan Dimitriu, Tue Brisson Mosich

--
-- ___________                                                __    
-- \_   _____/___________    _____   ______  _  _____________|  | __
--  |    __) \_  __ \__  \  /     \_/ __ \ \/ \/ /  _ \_  __ \  |/ /
--  |     \   |  | \// __ \|  Y Y  \  ___/\     (  <_> )  | \/    < 
--  \___  /   |__|  (____  /__|_|  /\___  >\/\_/ \____/|__|  |__|_ \
--      \/               \/      \/     \/                        \/
-- 

--  _____ _     _      ____ _____ _  ____  _      ____ 
-- /    // \ /\/ \  /|/   _Y__ __Y \/  _ \/ \  /|/ ___\
-- |  __\| | ||| |\ |||  /   / \ | || / \|| |\ |||    \
-- | |   | \_/|| | \|||  \_  | | | || \_/|| | \||\___ |
-- \_/   \____/\_/  \|\____/ \_/ \_/\____/\_/  \|\____/


drop function if exists addsearchhistory;
drop function if exists tokenizer;
drop function if exists wordrank;
drop function if exists exists_appuser;

--

-- very simple function to add a searchstring to users history of searches
-- added: also store searchtype
create or replace function addsearchhistory(appuserid int, stype text, search text)
returns void as 
$$
begin
	insert into searches (userid, searchtype, searchstring, date) values (appuserid, stype, search, CURRENT_TIMESTAMP(3));
	RAISE NOTICE 'Adding search history -- %', search;
end;
$$ 
language plpgsql;

-- check user exists
create or replace function exists_appuser(appuserid integer)
returns boolean as 
$$
declare
	checkz integer;
begin
			select id from appusers where appusers.id=appuserid into checkz; --hmm dont really like this part
			if checkz is null then
				RAISE NOTICE 'No user found for id -- %', appuserid;
				return false;
			else 
				RAISE NOTICE 'User exists with id -- %', appuserid;
				return true;
			end if;
end;
$$ 
language plpgsql;

-- tokenizer function to split search string
-- todo: remove non-alphanumeric characters from search string
create or replace function tokenizer(searchstr text)
returns text[] as 
$$
declare
   _wordz text[];
begin
	select regexp_split_to_array(searchstr, '\s+')
	into _wordz;
	RAISE NOTICE 'Splitting into tokens -- %', _wordz;
	return _wordz;
end;
$$ 
language plpgsql;

-- D7 word-to-word
-- returns ranked list of words found in matching posts
create or replace function wordrank(appuserid int, searchtype text, searchstr text)
returns table (term text, rank decimal) as
$$
declare
	wordz text[];
	w text;
    q text :='';
	existsuser boolean;
begin
	select exists_appuser(appuserid) into existsuser;
	if existsuser=false then
		RAISE NOTICE 'ERROR: Unknown user -- %', appuserid;
		return;
	end if;
	select tokenizer(searchstr)
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
	perform addsearchhistory(appuserid, searchtype, searchstr);
	return query execute q;
end;
$$ 
language plpgsql;

--  ____  _  _  ____  
-- ( ___)( \( )(  _ \ 
--  )__)  )  (  )(_) )
-- (____)(_)\_)(____/ 
-- 