using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;


namespace DatabaseService
{
    public class DataService : IDataService
    {

        public IList<Questions> GetQuestions(PagingAttributes pagingAttributes)
        {
            // AuthUser()
            // if ok do browse q-list
            using var db = new StackoverflowContext();
            return db.Questions
                .Skip(pagingAttributes.Page * pagingAttributes.PageSize)
                .Take(pagingAttributes.PageSize)
                .ToList();
        }

        public IList<Posts> Search(string searchstring, int? searchtypecode, PagingAttributes pagingAttributes)
        {
            ////// for performing searches with appsearch on the db
            ///
            // AuthUser()
            // 
            // do actual search using appsearch in db and build results

            //need db context and searchtype lookuptable
            using var db = new StackoverflowContext();
            Modules.SearchTypeLookupTable st = new Modules.SearchTypeLookupTable();

            ////get params for db.func
            ///
            //build searchstring
            var search = new NpgsqlParameter("search", NpgsqlTypes.NpgsqlDbType.Text);
            search.Value = BuildSearchString(searchstring);

            //lookup searchtype string
            var searchtype = new NpgsqlParameter("searchtype", NpgsqlTypes.NpgsqlDbType.Text);
            if (searchtypecode >= 0 && searchtypecode <= 3)
            {
                searchtype.Value = st.searchType[searchtypecode.Value];
            }
            else searchtype.Value = st.searchType[3];

            //userid is hardcoded for now; should be returned from auth or sth
            var appuserid = new NpgsqlParameter("appuserid", NpgsqlTypes.NpgsqlDbType.Integer);
            appuserid.Value = 2;

            //count all matches
            var matchcount = db.Search
                .FromSqlRaw("select appsearch(@appuserid, @searchtype, @search)", appuserid, searchtype, search)
                .Count();
            System.Console.WriteLine($"{matchcount} results.");

            //calc max pages and set requested page to last page if out of bounds
            var calculatedNumberOfPages = (int)Math.Ceiling((double)matchcount / pagingAttributes.PageSize)-1;
            System.Console.WriteLine($"{calculatedNumberOfPages} calculated pages.");
            int page;
            if (pagingAttributes.Page > calculatedNumberOfPages) 
            {
                page = calculatedNumberOfPages;
            }

            if (pagingAttributes.Page <= 0)
            {
                page = 0;
            }
            else page=pagingAttributes.Page-1;

            //get subset of results according to pagesize etc
            var resultlist = db.Search
                .FromSqlRaw("SELECT * from appsearch(@appuserid, @searchtype, @search)", appuserid, searchtype, search)
                .Skip(page * pagingAttributes.PageSize)
                .Take(pagingAttributes.PageSize)
                .ToList();

            //build and map results to posts
            var resultposts = new List<Posts>();

            foreach (Search s in resultlist) 
            {
               //different mapping for results that are questions and answers
                string tablename = GetPostType(s.postid);
                if (tablename == "answers")
                {
                    Posts p = new Posts();
                    p.Parentid = GetAnswer(s.postid).Parentid;
                    p.Id = s.postid;

                    var endpos = 100;
                    if (GetAnswer(s.postid).Body.Length < 100)
                    { endpos = GetAnswer(s.postid).Body.Length; }
                    p.Body = GetAnswer(s.postid).Body.Substring(0, endpos);

                    p.Title = GetQuestion(p.Parentid).Title;
                    p.Totalresults = matchcount;
                    p.Rank = s.rank;
                    resultposts.Add(p);
                }
                else 
                {
                    Posts p = new Posts();
                    p.Id = s.postid;

                    var endpos = 100;
                    if (GetQuestion(s.postid).Body.Length < 100)
                    { endpos = GetQuestion(s.postid).Body.Length; }
                    p.Body = GetQuestion(s.postid).Body.Substring(0, endpos);

                    p.Title = GetQuestion(s.postid).Title;
                    p.Totalresults = matchcount;
                    p.Rank = s.rank;
                    resultposts.Add(p);
                }
            }
            return resultposts;
        }


        public IList<WordRank> WordRank(string searchstring, int? searchtypecode, PagingAttributes pagingAttributes)
        {
            ////// for performing searches with wordrank on the db
            ///
            // AuthUser()
            // do actual search using appsearch in db and build results

            //need db context and searchtype lookuptable
            using var db = new StackoverflowContext();
            Modules.SearchTypeLookupTable st = new Modules.SearchTypeLookupTable();

            ////get params for db.func
            ///
            //build searchstring
            var search = new NpgsqlParameter("search", NpgsqlTypes.NpgsqlDbType.Text);
            search.Value = BuildSearchString(searchstring);

            //lookup searchtype string
            var searchtype = new NpgsqlParameter("searchtype", NpgsqlTypes.NpgsqlDbType.Text);
            if (searchtypecode >= 4 && searchtypecode <= 5)
            {
                searchtype.Value = st.searchType[searchtypecode.Value];
            }
            else searchtype.Value = st.searchType[5];

            //userid is hardcoded for now; should be returned from auth or sth
            var appuserid = new NpgsqlParameter("appuserid", NpgsqlTypes.NpgsqlDbType.Integer);
            appuserid.Value = 2;

            //count matches
            var matchcount = db.Search
                .FromSqlRaw("select wordrank(@appuserid, @searchtype, @search)", appuserid, searchtype, search)
                .Count();
            System.Console.WriteLine($"{matchcount} results.");

            //call db.func wordrank
            return db.WordRank
                .FromSqlRaw("SELECT * from wordrank(@appuserid, @searchtype, @search) limit 10", appuserid, searchtype, search)
                .Skip(pagingAttributes.Page * pagingAttributes.PageSize)
                .Take(pagingAttributes.PageSize)
                .ToList();
        }

        private static string BuildSearchString(string searchstring)
        {
            //convert query search string to appsearch db func search string
            string[] separators = { ",", ".", "..." };

            string[] words = searchstring.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
            System.Console.WriteLine($"{words.Length} tokens in search");

            string finalstring = string.Join(" ", words);
            System.Console.WriteLine("Built search string: " + finalstring);
            return finalstring;
        }

        public int NumberOfQuestions()
        {
            using var db = new StackoverflowContext();
            return db.Questions
                .Count();
        }

        public Questions GetQuestion(int questionId)
        {
            using var db = new StackoverflowContext();

            return db.Questions.Find(questionId);
        }

        public Answers GetAnswer(int answerId)
        {
            using var db = new StackoverflowContext();

            return db.Answers.Find(answerId);
        }

        public string GetPostType(int postId)
        // try to get the tablename of post -- answers or questions
        //using varchar resolveid(postid int) in db
        {
            System.Console.WriteLine($"Postid -- {postId}");
            var postid = new NpgsqlParameter("postid", NpgsqlTypes.NpgsqlDbType.Integer);
            postid.Value = postId;
            using var db = new StackoverflowContext();
            string tablename = db.PostsTable
                .FromSqlRaw("SELECT * from resolveid(@postid)", postid).First().resolveid;

                System.Console.WriteLine($"Post is part of -- {tablename}");

            return tablename;
        }

/*
        public int GetParentId(int answerID) //not needed possibly?
        {
            System.Console.WriteLine($"Answerid -- {answerID}");
            using var db = new StackoverflowContext();
            int parentid = db.Answers
                .Where(e => e.Id == answerID)
                .FirstOrDefault()
                .Parentid;

            System.Console.WriteLine($"Parentid -- {parentid}");

            return parentid;

        }

    */

        //public (Questions, IList<Answers>) GetThread(int questionId)
        public IList<Posts> GetThread(int questionId)
            //returns question and all child answers
        {
            using var db = new StackoverflowContext();
            //get the questionid
            var q = GetQuestion(questionId);
            if (q != null)
            {
                //find answers to the specified questions
                var ans = db.Answers
                            .Where(e => e.Parentid == questionId)
                            .ToList();
                //manual mapping
                List<Posts> posts = new List<Posts>();
                posts.Add(
                    new Posts 
                    { 
                        Id = q.Id, 
                        Title = q.Title, 
                        Body = q.Body 
                    });
                foreach (Answers a in ans)
                {
                    var endpos = 100;
                    if (a.Body.Length<100)
                    { 
                        endpos = a.Body.Length; //limit body size for now
                    }
                    posts.Add(
                    new Posts 
                    {
                        Id = a.Id,
                        Parentid = a.Parentid, 
                        Body = a.Body.Substring(0, endpos) 
                    });
                };
                return posts;
            }
            else return null;
            //return (q, ans); //not sure how this works, multiple return values
        }
    }
}
