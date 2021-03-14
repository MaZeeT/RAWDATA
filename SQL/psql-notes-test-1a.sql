-- GROUP: raw6, MEMBERS: Mads Zeuch Ethelberg, Monica Toader, Stefan Dimitriu, Tue Brisson Mosich

-- ___________              __  .__                
-- \__    ___/___   _______/  |_|__| ____    ____  
--   |    |_/ __ \ /  ___/\   __\  |/    \  / ___\ 
--   |    |\  ___/ \___ \  |  | |  |   |  \/ /_/  >
--   |____| \___  >____  > |__| |__|___|  /\___  / 
--              \/     \/               \//_____/ 
-- 

-- +-+-+-+-+-+-+-+-+-+
-- |S|t|a|c|k|D|a|t|a|
-- +-+-+-+-+-+-+-+-+-+

-- Small test to see that linkpostid were inserted correctly

select id, linkpostid from posts_universal where id=19;

select questions.id, linkposts.id linkpostid
from questions
join linkposts on linkposts.questionid = questions.id
where questions.id=19;


-- Small test of tags to see that they were inserted correctly

select parentid, tag from tags inner join taglabels on taglabels.id=tags.tagid where tags.parentid=19;

select id, tags from posts_universal where id=19;


-- +-+-+-+-+-+-+-+-+-+
-- |F|r|a|m|e|w|o|r|k|
-- +-+-+-+-+-+-+-+-+-+

-- Insert some users for testing
-- Note: there is no createuser() function or similar yet
insert into appusers (username, password, salt) values ('Huey','password','salt'),('Dewey','password','salt'),('Louie','password','salt');


-- Before we start, display search history for all users 

 select username, searchtype, searchstring, date from appusers
 INNER JOIN searches ON appusers.id=searches.userid
 order by date desc;

-- It is empty.
-- Search history is inserted when a user makes a search.

--   _   _  
--  / \ / \ 
-- ( D | 1 )
--  \_/ \_/ 
-- 

-- Simple search
-- Using appsearch() and the keyword 'simple', we can perform a simple search.
-- Note: appsearch returns matching postid(s) and weights/ranks (if any).
-- Note: simple search takes one search term, others are discarded.

-- User 1 now makes some searches using simple search.

select appsearch(1, 'simple', 'chocolate');
select appsearch(1, 'simple', 'chocolate explosion');

--   _   _  
--  / \ / \ 
-- ( D | 3 )
--  \_/ \_/ 
-- 

-- Exact-match querying
-- Using appsearch() and the keyword 'exactmatch', we can perform an exact-match search.
-- Note: appsearch() takes the arguments appuserid, searchmode and string.

-- User 3 now makes some searches using exact-match.

select appsearch(3, 'exactmatch', '');
select appsearch(3, 'exactmatch', 'constructors') limit 10; -- Note: using limit for the sake of this test-script
select appsearch(3, 'exactmatch', 'constructors regions');
select appsearch(3, 'exactmatch', 'constructors regions blocks');

--   _   _  
--  / \ / \ 
-- ( D | 4 )
--  \_/ \_/ 
-- 

-- Best-match querying
-- Using appsearch() and the keyword 'bestmatch', we can perform a best-match search.
-- Note: appserach() tries to check that the appuser exists before searching and committing the search to the search history.
-- Best-match search is also the fallthrough search; unknown search keywords will use best-match.

-- Several users now makes some searches using exact-match.

select appsearch(2, 'bestmatch', 'delete sql database') limit 10; -- Note: using limit for the sake of this test-script

-- Note: unknown searchtype uses best-match
select appsearch(2, 'wrong', 'format drive c') limit 10; -- Note: using limit for the sake of this test-script

-- Nonexistant user tries to search also
select appsearch(5, 'bestmatch', 'no such user');


--   _   _  
--  / \ / \ 
-- ( D | 6 )
--  \_/ \_/ 
-- 

-- Ranked weighted querying using TFIDF
-- Using appsearch() and the keyword 'tfidf', we can perform this kind of search.

-- A user makes some searches

select appsearch(1, 'tfidf', 'chocolate');
select appsearch(1, 'tfidf', 'chocolate explosion');
select appsearch(1, 'tfidf', 'chocolate explosion caramel');


--   _   _  
--  / \ / \ 
-- ( D | 7 )
--  \_/ \_/ 
-- 

-- Word-to-words querying
-- A different kind of search than the ones above, and uses a different function, wordrank().
-- Note: This function does not have a fallthrough mode to avoid confusion in the search history.

-- Users making wordrank searches

select wordrank(1, 'wordsbest', 'program') limit 10;
select wordrank(1, 'wordstfidf', 'program') limit 10;
select wordrank(1, 'wordstfidf', 'program programming') limit 10;

--Note: unknown searchtype not allowed.
select wordrank(2, 'best', 'chocolate') limit 10;

--   _   _   _   _   _   _     _   _   _   _   _   _   _  
--  / \ / \ / \ / \ / \ / \   / \ / \ / \ / \ / \ / \ / \ 
-- ( S | e | a | r | c | h ) ( H | i | s | t | o | r | y )
--  \_/ \_/ \_/ \_/ \_/ \_/   \_/ \_/ \_/ \_/ \_/ \_/ \_/ 
-- 

-- After all the searches, display search history for all users
 
select username, searchtype, searchstring, date from appusers
INNER JOIN searches ON appusers.id=searches.userid
order by date desc;


--   _   _   _   _   _   _     _   _   _   _   _   _   _  
--  / \ / \ / \ / \ / \ / \   / \ / \ / \ / \ / \ / \ / \ 
-- ( B | r | o | w | s | e ) ( H | i | s | t | o | r | y )
--  \_/ \_/ \_/ \_/ \_/ \_/   \_/ \_/ \_/ \_/ \_/ \_/ \_/ 
--

-- History should be empty:
select * from history;

-- Add browse history for a user with the function add_history(userid, postid);
-- Note: Only existing appusers allowed.
-- Note: Only existing posts allowed.

select add_history(1, 71);
select add_history(2, 19);
select add_history(1, 120);
select add_history(6, 19);

--   _   _   _   _   _   _   _   _   _  
--  / \ / \ / \ / \ / \ / \ / \ / \ / \ 
-- ( B | o | o | k | m | a | r | k | s )
--  \_/ \_/ \_/ \_/ \_/ \_/ \_/ \_/ \_/ 
-- 

-- Add bookmark: add_history(appuserid integer, ipostid integer, addbookmark boolean)
-- Note: can only add one bookmark per userid, post

select add_history(2, 71, true);
select add_history(2, 71, true);
select add_history(1, 71, true);
select add_history(1, 71, false);
select add_history(7, 71, true);

-- Show users' history and bookmarks
select username, postid, posttablename, date, isbookmark from history INNER JOIN appusers ON history.userid=appusers.id
order by date desc;


--   _   _   _   _   _   _   _   _   _   _   _  
--  / \ / \ / \ / \ / \ / \ / \ / \ / \ / \ / \ 
-- ( A | n | n | o | t | a | t | i | o | n | s )
--  \_/ \_/ \_/ \_/ \_/ \_/ \_/ \_/ \_/ \_/ \_/ 
--

-- Show users' annotations are empty:
select username, postid, posttablename, body, annotations.date from annotations, history, appusers where historyid=history.id and history.userid=appusers.id;


-- Add note: annotate(appuserid integer, ipostid integer, note text)
-- Note: Adds bookmark automagically if it doesnt exist
-- Note: Only existing appusers allowed.
-- Note: Only existing posts allowed.

select annotate(2, 71, 'my note for post 71: this post is very relevant');
select annotate(1, 19, 'a note: remember to wash the dog');

select annotate(5, 19, 'wrong userid');
select annotate(1, 666, 'wrong postid');

-- Show users' annotations
select username, postid, posttablename, body, annotations.date from annotations, history, appusers where historyid=history.id and history.userid=appusers.id;

-- Show users' history and bookmarks
select username, postid, posttablename, date, isbookmark from history INNER JOIN appusers ON history.userid=appusers.id
order by date desc;

--  ____  _  _  ____  
-- ( ___)( \( )(  _ \ 
--  )__)  )  (  )(_) )
-- (____)(_)\_)(____/ 
-- 