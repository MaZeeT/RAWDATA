using Microsoft.EntityFrameworkCore;
using Npgsql;
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

        public IList<Search> Search(string searchstring, int? searchtypecode, PagingAttributes pagingAttributes)
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

            //get subset of results according to pagesize etc
            var resultlist = db.Search
                .FromSqlRaw("SELECT * from appsearch(@appuserid, @searchtype, @search)", appuserid, searchtype, search)
                .Skip(pagingAttributes.Page * pagingAttributes.PageSize)
                .Take(pagingAttributes.PageSize)
                .ToList();

            foreach (Search s in resultlist) { GetPost(s.postid); }

            return resultlist;
        }

        private static string BuildSearchString(string searchstring)
        {
            string[] separators = { ",", ".", "..." };

            string[] words = searchstring.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
            System.Console.WriteLine($"{words.Length} tokens in search");

            string finalstring = string.Join(" ", words);
            System.Console.WriteLine("Built search string: " + finalstring);
            return finalstring;
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

        public void GetPost(int postId)
        // try to get the tablename of post -- answers or questions
        //using varchar resolveid(postid int) in db
        {
            System.Console.WriteLine($"Postid -- {postId}");
            var postid = new NpgsqlParameter("postid", NpgsqlTypes.NpgsqlDbType.Integer);
            postid.Value = postId;
            using var db = new StackoverflowContext();
           var tablename = db.PostsTable
                .FromSqlRaw("SELECT * from resolveid(@postid)", postid).ToList();

            foreach (PostsTable t in tablename)
            {
                System.Console.WriteLine($"Post is part of -- {t.resolveid}");
            }


        }

        //public (Questions, IList<Answers>) GetThread(int questionId)
        public IList<Posts> GetThread(int questionId)
        {
            using var db = new StackoverflowContext();

            var q = GetQuestion(questionId);
            if (q != null)
            {
                var ans = db.Answers
                    .Where(e => e.Parentid == questionId)
                    .ToList();
                //manual mapping
                List<Posts> posts = new List<Posts>();
                posts.Add(
                    new Posts { Id = q.Id, Title = q.Title, Body = q.Body });
                foreach (Answers a in ans)
                {
                    var endpos = 100;
                    if (a.Body.Length<100)
                    { endpos = a.Body.Length; }
                    posts.Add(
        new Posts { Id = a.Id, Parentid = a.Parentid, Body = a.Body.Substring(0, endpos) });
                };


                return posts;
            }
            else return null;
            //return (q, ans); //not sure how this works, multiple return values
        }
    }
}
