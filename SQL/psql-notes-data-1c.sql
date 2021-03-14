-- GROUP: raw6, MEMBERS: Mads Zeuch Ethelberg, Monica Toader, Stefan Dimitriu, Tue Brisson Mosich

--
--   _________ __                 __    ________          __          
--  /   _____//  |______    ____ |  | __\______ \ _____ _/  |______   
--  \_____  \\   __\__  \ _/ ___\|  |/ / |    |  \\__  \\   __\__  \  
--  /        \|  |  / __ \\  \___|    <  |    `   \/ __ \|  |  / __ \_
-- /_______  /|__| (____  /\___  >__|_ \/_______  (____  /__| (____  /
--         \/           \/     \/     \/        \/     \/          \/ 
-- 

--
--  ____   __    ____  __    ____  ___ 
-- (_  _) /__\  (  _ \(  )  ( ___)/ __)
--   )(  /(__)\  ) _ < )(__  )__) \__ \
--  (__)(__)(__)(____/(____)(____)(___/
-- 

drop table if exists tags;
drop table if exists taglabels;
drop table if exists tagtemp;
drop table if exists comments;
drop table if exists linkposts;
drop table if exists answers;
drop table if exists questions;
drop table if exists stackusers;
DROP MATERIALIZED VIEW IF EXISTS q_and_a;

CREATE TABLE stackusers (
  id INTEGER NOT NULL,   
  displayname TEXT,
  creationdate DATE,
  location TEXT,
  age INTEGER,
  PRIMARY KEY (id)
);

CREATE TABLE questions (
  id INTEGER NOT NULL,   
  posttypeid INTEGER,
  acceptedanswerid INTEGER,
  --acceptedanswerid is not referencing answer.id cuz a) circular referencing, b) can be null here. can possibly be handled another way keeping some kind of relation
  creationdate DATE,
  score INTEGER,
  body TEXT,
  closeddate DATE,
  title TEXT,
  tags text, --temporary column to make it easier to extract tags
  ownerid INTEGER REFERENCES stackusers(id),
  PRIMARY KEY (id)
);

CREATE TABLE answers (
  id INTEGER NOT NULL, 
  parentid INTEGER REFERENCES questions(id) ON DELETE CASCADE,  
  posttypeid INTEGER,
  creationdate DATE,
  score INTEGER,
  body TEXT,
  ownerid INTEGER REFERENCES stackusers(id),
  PRIMARY KEY (id)
);

CREATE TABLE linkposts
(
  id INTEGER NOT NULL,
  questionid INTEGER NOT NULL,
  CONSTRAINT questionid_fk FOREIGN KEY (questionid) REFERENCES questions (id) ON DELETE CASCADE,
  PRIMARY KEY (id, questionid)
);

CREATE TABLE comments (
  id INTEGER NOT NULL, 
  parentid INTEGER,  
  parentposttypeid INTEGER,
  creationdate DATE,
  score INTEGER,
  body TEXT,
  ownerid INTEGER REFERENCES stackusers(id),
  PRIMARY KEY (id)
);

CREATE TABLE tagtemp (
  parentid INTEGER REFERENCES questions(id) ON DELETE CASCADE,  
  tag TEXT,
  PRIMARY KEY (parentid, tag)
);

CREATE TABLE taglabels (
  id SERIAL NOT NULL, 
  tag TEXT,
  PRIMARY KEY (id)
);

CREATE TABLE tags (
  parentid INTEGER REFERENCES questions(id) ON DELETE CASCADE,  
  tagid INTEGER REFERENCES taglabels(id) ON DELETE CASCADE,
  PRIMARY KEY (parentid, tagid)
);

--lookup table 
CREATE MATERIALIZED VIEW q_and_a
AS
SELECT DISTINCT ON (id) id, posttypeid FROM posts_universal;
 

--  ____  _  _  ___  ____  ____  ____    ___  ____  __  __  ____  ____ 
-- (_  _)( \( )/ __)( ___)(  _ \(_  _)  / __)(_  _)(  )(  )( ___)( ___)
--  _)(_  )  ( \__ \ )__)  )   /  )(    \__ \  )(   )(__)(  )__)  )__) 
-- (____)(_)\_)(___/(____)(_)\_) (__)   (___/ (__) (______)(__)  (__)
--

-- insert stackusers
insert into stackusers (id, displayname, creationdate, location, age)
SELECT DISTINCT ON (ownerid) * FROM
(SELECT ownerid, ownerdisplayname, ownercreationdate, ownerlocation, ownerage FROM posts_universal 
UNION ALL  
SELECT authorid, authordisplayname, authorcreationdate, authorlocation, authorage FROM comments_universal)
as dummy;

-- insert questions
insert into questions 
SELECT DISTINCT ON (id) id, posttypeid, acceptedanswerid, creationdate, score, body, closeddate, title, tags, ownerid FROM posts_universal where posttypeid=1;

-- insert linkposts
insert into linkposts (id, questionid) 
SELECT DISTINCT ON (linkpostid, id) linkpostid, id FROM posts_universal where posttypeid=1 and linkpostid is not null;

-- ins answers
insert into answers 
SELECT DISTINCT ON (id) id, parentid, posttypeid, creationdate, score, body, ownerid FROM posts_universal where posttypeid=2;

-- insert comments
insert into comments (id, parentid, parentposttypeid, creationdate, score, body, ownerid) 
SELECT DISTINCT ON (commentid) commentid, postid, posts_universal.posttypeid, commentcreatedate, commentscore, commenttext, authorid FROM comments_universal INNER JOIN posts_universal ON postid=posts_universal.id;

-- insert tags
insert into tagtemp 
select id, unnest(regexp_split_to_array(tags, '::')) tagg FROM questions;

insert into taglabels (tag)
select DISTINCT ON (tagg) unnest(regexp_split_to_array(tags, '::')) tagg FROM questions;

insert into tags (parentid, tagid)
select tagtemp.parentid, taglabels.id FROM tagtemp inner join taglabels on taglabels.tag = tagtemp.tag;

--ok tags should be dealt with, we can drop the temp stuff
-- drop temp tag table
drop table tagtemp;
--drop column tags from questions
ALTER TABLE questions
DROP COLUMN tags;

--
-- ___________                                                __    
-- \_   _____/___________    _____   ______  _  _____________|  | __
--  |    __) \_  __ \__  \  /     \_/ __ \ \/ \/ /  _ \_  __ \  |/ /
--  |     \   |  | \// __ \|  Y Y  \  ___/\     (  <_> )  | \/    < 
--  \___  /   |__|  (____  /__|_|  /\___  >\/\_/ \____/|__|  |__|_ \
--      \/               \/      \/     \/                        \/
-- 

--
--  ____   __    ____  __    ____  ___ 
-- (_  _) /__\  (  _ \(  )  ( ___)/ __)
--   )(  /(__)\  ) _ < )(__  )__) \__ \
--  (__)(__)(__)(____/(____)(____)(___/
-- 

drop table if exists wi;
drop table if exists wi_weighted;
drop table if exists searches;
drop table if exists annotations;
drop table if exists history;
drop table if exists appusers;

-- D2 unweighted index, from slides
create table wi as
select id, tablename, lower(word) word from words
where word ~* '^[a-z][a-z0-9_]+$'
and tablename = 'posts' and (what='title' or what='body')
group by id,word,tablename;


-- D5 weighted index using tfidf
-- indexing all alphanumeric words from words table; from posts (q and a; title, body) and comments (text)
-- trim is used to remove spaces around the words (again, better regex could probably replace this); this means that eg. 'this' and 'this ' are counted as the same word 
-- did not do additional filtering to remove 'noise' as they put it. how to fix their 'imperfect tokenization'?? hmm..
-- dont care about stopwords. they take up a little space, sure, but get a low relevance due to tfidf, so feel it's safe to ignore them 
-- could have surrogate key instead of composite key

CREATE TABLE wi_weighted (
  id INTEGER NOT NULL,   
  what VARCHAR(255) NOT NULL,
  word VARCHAR(255) NOT NULL,
  tfidf DECIMAL,
  PRIMARY KEY (id,what,word)
);

-- caculate and insert tfidf and words into wi_weighted
insert into wi_weighted (id, what, word, tfidf)
select tf.id, what, tf.word, tf*LOG(idf) tfidf from
	(select p.id, what, p.word, p.ct::decimal/t.ct tf from
		(select id, what, trim(word) word, count(*) as ct
		from
			(select id, what, lower(word) word
			from words  
			where word ~* '^[a-z][a-z0-9_]+$') tr
		group by id, what, word) p, 
		(select id, count(*) as ct
		from 
			(select id, trim(word) word, count(*) as ct
			from
				(select id, lower(word) word
				from words  
				where word ~* '^[a-z][a-z0-9_]+$') tr
			group by id,word) wc
		group by id) t
	where p.id=t.id
	group by p.id, what, p.word, p.ct, t.ct) tf,
(select n.word, t.ct::decimal/n.ct idf 
from
	(select count (distinct id) ct
	from  posts_universal) t,
	(select p.word, count(*) as ct 
	from
		(select distinct(trim(word)) word, id
		from
			(select id, lower(word) word
			from words  
			where word ~* '^[a-z][a-z0-9_]+$') tr 
		group by id,word) p
	group by p.word) n
group by n.word, t.ct, n.ct) idf
where tf.word=idf.word
order by tf.id asc;


CREATE TABLE appusers (
  id SERIAL NOT NULL,   
  username VARCHAR(255) UNIQUE NOT NULL,
  password VARCHAR (500) NOT NULL,
  salt VARCHAR (500) NOT NULL,
  PRIMARY KEY (id)
);


CREATE TABLE searches (
  id SERIAL NOT NULL,   
  userid INTEGER REFERENCES appusers(id) ON DELETE CASCADE,
  searchtype TEXT,
  searchstring TEXT,
  date TIMESTAMP,
  PRIMARY KEY (id)
);


CREATE TABLE history (
  id SERIAL NOT NULL,  
  userid INTEGER REFERENCES appusers(id) ON DELETE CASCADE,
  postid INTEGER,
  posttablename VARCHAR(100),
  date TIMESTAMP,
  isbookmark BOOLEAN DEFAULT FALSE,
  PRIMARY KEY (id)
);


CREATE TABLE annotations (
  id SERIAL NOT NULL,   
  userid INTEGER REFERENCES appusers(id) ON DELETE CASCADE,
  historyid INTEGER REFERENCES history(id) ON DELETE CASCADE,
  body TEXT,
  date TIMESTAMP,
  PRIMARY KEY (id)
);

--  ____  _  _  ____  
-- ( ___)( \( )(  _ \ 
--  )__)  )  (  )(_) )
-- (____)(_)\_)(____/ 
-- 
