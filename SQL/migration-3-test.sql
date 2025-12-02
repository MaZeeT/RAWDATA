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

-- Add browse history for a user

insert into history (userid, postid, posttablename, date, isbookmark) values (1, 71, 'answers',CURRENT_TIMESTAMP(3), false);
insert into history (userid, postid, posttablename, date, isbookmark) values (2, 19, 'questions',CURRENT_TIMESTAMP(3), false);
insert into history (userid, postid, posttablename, date, isbookmark) values (1, 120, 'unknown',CURRENT_TIMESTAMP(3), false);
insert into history (userid, postid, posttablename, date, isbookmark) values (6, 19, 'questions',CURRENT_TIMESTAMP(3), false);

--   _   _   _   _   _   _   _   _   _  
--  / \ / \ / \ / \ / \ / \ / \ / \ / \ 
-- ( B | o | o | k | m | a | r | k | s )
--  \_/ \_/ \_/ \_/ \_/ \_/ \_/ \_/ \_/ 
-- 

-- Add bookmark
insert into history (userid, postid, posttablename, date, isbookmark) values (2, 71, 'answers',CURRENT_TIMESTAMP(3), true);
insert into history (userid, postid, posttablename, date, isbookmark) values (1, 71, 'answers',CURRENT_TIMESTAMP(3), false);
insert into history (userid, postid, posttablename, date, isbookmark) values (7, 71, 'answers',CURRENT_TIMESTAMP(3), true);

-- Show users' history and bookmarks
select username, postid, posttablename, date, isbookmark from history INNER JOIN appusers ON history.userid=appusers.id
order by date desc;


--  ____  _  _  ____  
-- ( ___)( \( )(  _ \ 
--  )__)  )  (  )(_) )
-- (____)(_)\_)(____/ 
-- 