--
-- PostgreSQL database dump
--

-- Dumped from database version 11.5
-- Dumped by pg_dump version 11.5

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: comments_universal; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.comments_universal (
    commentid integer,
    postid integer,
    commentscore integer,
    commenttext text,
    commentcreatedate timestamp without time zone,
    authorid integer,
    authordisplayname text,
    authorcreationdate timestamp without time zone,
    authorlocation text,
    authorage integer
);


ALTER TABLE public.comments_universal OWNER TO postgres;

--
-- Name: posts_universal; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.posts_universal (
    id integer,
    posttypeid integer,
    parentid integer,
    acceptedanswerid integer,
    creationdate timestamp without time zone,
    score integer,
    body text,
    closeddate timestamp without time zone,
    title text,
    tags text,
    ownerid integer,
    ownerdisplayname text,
    ownercreationdate timestamp without time zone,
    ownerlocation text,
    ownerage integer,
    linkpostid integer
);


ALTER TABLE public.posts_universal OWNER TO postgres;

--
-- Name: stopwords; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.stopwords (
    word character varying(18) DEFAULT NULL::character varying
);


ALTER TABLE public.stopwords OWNER TO postgres;

--
-- Name: words; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.words (
    id integer,
    tablename character varying(100),
    what character varying(100),
    sen integer,
    idx integer,
    word character varying(100),
    pos character varying(100),
    lemma character varying(100)
);


ALTER TABLE public.words OWNER TO postgres;

--
-- Data for Name: comments_universal; Type: TABLE DATA; Schema: public; Owner: postgres
--