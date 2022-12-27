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
drop function if exists search_exactmatch;
drop function if exists search_bestmatch;
drop function if exists search_tfidf;
drop function if exists search_simple;
drop function if exists appsearch;
drop function if exists wordrank;
drop function if exists resolveid(integer);
drop function if exists resolveid(integer, varchar);
drop function if exists add_history(integer, integer);
drop function if exists add_history(integer, integer, boolean);
drop function if exists exists_bookmark;
drop function if exists exists_appuser;
drop function if exists annotate;

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


-- overloaded function to resolve postid into tablename (questions, answers, comments)
create or replace function resolveid(postid int)
returns varchar as 
$$
declare
   kind varchar;
   typeid int;
begin
	select posttypeid from q_and_a where q_and_a.id=postid
	into typeid;
	if typeid=1 then
		kind='questions';
	elsif typeid=2 then
		kind='answers';
	elsif typeid is null then
		kind='unknown';
	end if;
	RAISE NOTICE 'Looking up id, id is part of -- %', kind;
	return kind;
end;
$$ 
language plpgsql;

create or replace function resolveid(postid int, what varchar)
returns varchar as 
$$
declare
   kind varchar;
   didcall boolean=false;
   idcheck integer;
begin
	if what='text'
	then 
		select id from comments where id=postid into idcheck;
		if idcheck is not null -- check id actually exists
			then kind='comments';
		else kind='unknown';
		end if;
	else
		select resolveid(postid) into kind;
		didcall=true;
	end if;
	if didcall=false --suppress repeat notice
		then RAISE NOTICE 'Looking up id, id is part of -- %', kind;
	end if;
	return kind;
end;
$$ 
language plpgsql;



-- added: function that takes appuserid, postid and adds an entry into history as 1) history and 2) a bookmark
-- added: check user exists
-- todo: support comments
create or replace function add_history(appuserid integer, ipostid integer, addbookmark boolean )
returns boolean as 
$$
declare
	tabl varchar;
	checkz boolean;
	existsuser boolean;
begin
	select exists_appuser(appuserid) into existsuser;
	if existsuser=false then
		RAISE NOTICE 'ERROR: Unknown user -- %', appuserid;
		return false;
	end if;
	select resolveid(ipostid) into tabl; -- get table name
	if tabl='questions' or tabl='answers'
	then
		if addbookmark=true then 
			select exists_bookmark(appuserid, ipostid, tabl into checkz); 
			if checkz=false
			then RAISE NOTICE 'Adding bookmark for post -- %', ipostid;
			insert into history (userid, postid, posttablename, date, isbookmark) values (appuserid, ipostid, tabl, CURRENT_TIMESTAMP(3), addbookmark);
			else
				RAISE NOTICE 'Bookmark for post exists, not added -- %', ipostid;
			end if;
		else
			insert into history (userid, postid, posttablename, date, isbookmark) values (appuserid, ipostid, tabl,CURRENT_TIMESTAMP(3), addbookmark);
			RAISE NOTICE 'Adding browse history for post -- %', ipostid;
		end if;
		return true;
	else 
		RAISE NOTICE 'Unable to add browse history, unknown post -- %', ipostid;	
		return false;
	end if;
end;
$$ 
language plpgsql;

create or replace function add_history(appuserid integer, postid integer)
returns void as 
$$
begin
	perform add_history(appuserid, postid, false);
end;
$$ 
language plpgsql;


-- check bookmark
-- todo: exists_user (not really needed? this only called from other function, also doesnt insert)
create or replace function exists_bookmark(appuserid integer, ipostid integer, tabl varchar)
returns boolean as 
$$
declare
	checkz boolean;
begin
			select isbookmark from history where userid=appuserid and postid=ipostid and posttablename=tabl and isbookmark=true into checkz; --hmm dont really like this part
			if checkz=false or checkz is null then
				RAISE NOTICE 'No bookmark found for post -- %', ipostid;
				return false;
			else 
				RAISE NOTICE 'Bookmark exists for post -- %', ipostid;
				return true;
			end if;
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


-- added: annotations function
create or replace function annotate(appuserid integer, ipostid integer, note text)
returns integer as 
$$
declare
	bid integer;
	tabl varchar;
	existsuser boolean;
	newAnnotId integer;
begin
	select exists_appuser(appuserid) into existsuser;
	if existsuser=false then
		RAISE NOTICE 'ERROR: Unknown user -- %', appuserid;
		return null;
	end if;
	perform add_history(appuserid, ipostid, true);
	select resolveid(ipostid) into tabl; -- get table name
	if tabl!='unknown'
	then
		select id from history 
		where userid=appuserid and postid=ipostid and posttablename=tabl and isbookmark=true into bid;
		
		insert into annotations (userid, historyid, body, date) 
		values (appuserid, bid, note,CURRENT_TIMESTAMP(3)) RETURNING id into newAnnotId;
		
		return newAnnotId;
		
	else
		RAISE NOTICE 'ERROR: Unknown error.';
		return null;
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


-- D1 Simple search function ---------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION search_simple(keyword text[])
RETURNS text as  
$$
declare
            q text := '';
BEGIN
                q:= q||' SELECT questions.id, 0::decimal FROM questions WHERE questions.title ~* ''';
                q:= q|| keyword[1];
                q:= q|| ''' ';
                q:= q|| 'or questions.body ~* ''';
                q:= q|| keyword[1];
                q:= q|| ''' ';
                q:= q||' UNION SELECT answers.id, 0::decimal FROM answers WHERE answers.body ~* ''';
                q:= q|| keyword[1];
                q:= q|| ''' ';
                RAISE NOTICE 'SimpleSearch: Using only first keymord -- %', keyword[1];
              RETURN q;
END;
$$
language plpgsql;


-- D3 Exact-match querying
create or replace FUNCTION search_exactmatch(keywordsArray text[]) 
returns text AS
$$
declare 
    keyword text;
        a_final text :='';
        lengthArray integer;
        a_count integer;
        q_count integer;
        w text;
begin
        lengthArray := array_length(keywordsArray, 1);
        RAISE NOTICE 'Exactmatch found lengthArray of -- % ', lengthArray;
        a_count:= array_length(keywordsArray, 1);
        q_count:= array_length(keywordsArray, 1);
        if(lengthArray <> 0) then
            if (lengthArray = 1) then
                    a_final:= a_final || ' SELECT answers.id, 0::decimal FROM answers, (';
                    a_final:= a_final || ' SELECT wi.id from wi where word = ''';
                    a_final:= a_final || keywordsArray[1] ;
                    a_final:= a_final || ''' ';
                    a_final := a_final || ' ) t WHERE answers.id=t.id UNION SELECT questions.id, 0::decimal FROM questions, ( ';
                    a_final := a_final || ' SELECT wi.id from wi where  word = ''';
                    a_final := a_final || keywordsArray[1] ;
                    a_final := a_final || ''' ';
                    a_final := a_final || ' ) t WHERE questions.id=t.id';
            else
                    a_final:= a_final || ' SELECT answers.id, 0::decimal FROM answers, ( ';
                    foreach w in array keywordsArray 
                    loop
                        a_count := a_count - 1;
                        a_final := a_final || ' SELECT wi.id from wi where  word = ''';
                        a_final := a_final || w ;
                        a_final := a_final || ''' ';
                        if(a_count > 0) then
                            a_final := a_final || ' INTERSECT ';
                        end if;
                    end loop;
                    a_final := a_final || ' ) t WHERE answers.id=t.id UNION SELECT questions.id, 0::decimal FROM questions, ( ';
                    foreach w in array keywordsArray 
                    loop
                        q_count := q_count - 1;
                        a_final := a_final || ' SELECT wi.id from wi where  word = ''';
                        a_final := a_final || w ;
                        a_final := a_final || ''' ';
                        if(q_count > 0) then
                            a_final := a_final || ' INTERSECT ';
                        end if;
                    end loop;
                    a_final := a_final || ' ) t WHERE questions.id=t.id';
            END IF;
            RETURN a_final;
        END IF;
end; 
$$ 
language plpgsql;


-- D4 search, best match, adapted from slides
-- takes array of search words and builds query string
-- currently searches q and a, not comments
create or replace function search_bestmatch(searchwords text[])
returns text as 
$$
declare
	w text;
	q text :='';
begin
	q:='select p.id, sum(relevance)::decimal rank from (SELECT id, body FROM questions UNION ALL SELECT id, body FROM answers) p, (';
	foreach w in array searchwords
loop
	q := q || 'select distinct id, 1 relevance from wi where word = ''';
	q := q || w;
	q := q || ''' union all ';
end loop;
	select regexp_replace(q, '\sunion all\s$', '') --remove last union all
	into q;
	q := q || ') t where p.id=t.id group by p.id, body order by rank desc;';
	return q;
end;
$$ 
language plpgsql;


-- D6 search, tfidf
-- TF = (Number of time the word occurs in the text) / (Total number of words in text)
-- IDF = (Total number of documents / Number of documents with word t in it)
-- TF-IDF = TF * LOG(IDF)
-- takes array of search words and builds query string
-- currently searches q and a, not comments
create or replace function search_tfidf(searchwords text[])
returns text as 
$$
declare
	w text;
	q text :='';
begin
	q:='select t.id, round(sum(tfidf), 4) rank from (';
	foreach w in array searchwords
loop
	q := q || 'select distinct on (id, what, word) id, what, word, tfidf from wi_weighted where word = ''';
	q := q || w;
	q := q || '''  and (what=''title'' or what=''body'') union all ';
end loop;
	select regexp_replace(q, '\sunion all\s$', '') --remove last union all
	into q;
	q := q || ') t group by t.id order by rank desc;';
	return q;
end;
$$ 
language plpgsql;


-- D7 word-to-word
-- returns ranked list of words found in matching posts
-- can't immediately be called from appsearch, as returns something different?
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


-- mother function for searching
-- added: third parameter: searchtype
-- added: check user exists
create or replace function appsearch(appuserid int, searchtype text, searchstr text, internalcaller boolean DEFAULT false)
returns table (postid integer, rank decimal) as 
$$
declare
	wordz text[];
	w text;
    q text :='';
	existsuser boolean;
begin
if internalcaller=false then
	select exists_appuser(appuserid) into existsuser;
	if existsuser=false then
		RAISE NOTICE 'ERROR: Unknown user -- %', appuserid;
		return;
	end if;
	perform addsearchhistory(appuserid, searchtype, searchstr);
end if;
	select tokenizer(searchstr)
	into wordz;
	if searchtype='tfidf' then
		select search_tfidf(wordz)
		into q;
	elsif searchtype='exactmatch' then
		select search_exactmatch(wordz)
		into q;	
	elsif searchtype='simple' then
		select search_simple(wordz)
		into q;	
	else
		select search_bestmatch(wordz)
		into q;
	end if;
raise notice 'Building query -- %', q;
return query execute q;
end;
$$ 
language plpgsql;

--  ____  _  _  ____  
-- ( ___)( \( )(  _ \ 
--  )__)  )  (  )(_) )
-- (____)(_)\_)(____/ 
-- 