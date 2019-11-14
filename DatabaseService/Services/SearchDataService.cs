using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DatabaseService
{
    public class SearchDataService : ISearchDataService
    {
        private readonly ISharedService _sharedService; //shared stuff by injection
        public SearchDataService(
            ISharedService sharedService)
        {
            _sharedService = sharedService;
        }

        public IList<Questions> GetQuestions(PagingAttributes pagingAttributes)
        {
            //// for browsing the full list of questions
            using var db = new AppContext();

            //try to convert back from 1-based pages
            int page;
            if (pagingAttributes.Page <= 0)
            {
                page = 0;
            }
            else page = pagingAttributes.Page - 1;

            return db.Questions
                .OrderBy(u => u.Id)
                .Skip(page * pagingAttributes.PageSize)
                .Take(pagingAttributes.PageSize)
                .ToList();
        }

        public IList<Posts> Search(int userid, string searchstring, int? searchtypecode, PagingAttributes pagingAttributes)
        {
            ////// for performing searches with appsearch on the db
            ///
            // do actual search using appsearch in db and build results

            //need db context and searchtype lookuptable
            using var db = new AppContext();
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

            //userid 
            var appuserid = new NpgsqlParameter("appuserid", NpgsqlTypes.NpgsqlDbType.Integer);
            appuserid.Value = userid;

            //if internal call is specified, stored function appsearch won't add to searches/searchhistory
            var internalcall = new NpgsqlParameter("internalcall", NpgsqlTypes.NpgsqlDbType.Boolean);
            internalcall.Value = true;

            //count all matches
            var matchcount = db.Search
                .FromSqlRaw("select appsearch(@appuserid, @searchtype, @search, @internalcall)", appuserid, searchtype, search, internalcall)
                .Count();
            System.Console.WriteLine($"{matchcount} results.");

            int page = _sharedService.GetPagination(matchcount, pagingAttributes);

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

                Posts p = new Posts();
                SinglePost sp = new SinglePost();
                sp = _sharedService.GetPost(s.postid);

                p.Parentid = sp.QuestionId;
                p.Id = sp.Id;
                var endpos = 100;
                if (sp.Body.Length < 100)
                { endpos =sp.Body.Length; }
                p.Body = sp.Body.Substring(0, endpos);

                p.Title = _sharedService.GetQuestion(p.Parentid).Title;
                p.Totalresults = matchcount;
                p.Rank = s.rank;
                resultposts.Add(p);

                /*string tablename = _sharedService.GetPostType(s.postid);
                if (tablename == "answers")
                {
                    Posts p = new Posts();
                    p.Parentid = _sharedService.GetAnswer(s.postid).Parentid;
                    p.Id = s.postid;

                    var endpos = 100;
                    if (_sharedService.GetAnswer(s.postid).Body.Length < 100)
                    { endpos = _sharedService.GetAnswer(s.postid).Body.Length; }
                    p.Body = _sharedService.GetAnswer(s.postid).Body.Substring(0, endpos);

                    p.Title = _sharedService.GetQuestion(p.Parentid).Title;
                    p.Totalresults = matchcount;
                    p.Rank = s.rank;
                    resultposts.Add(p);
                }
                else 
                {
                    Posts p = new Posts();
                    p.Id = s.postid;

                    var endpos = 100;
                    if (_sharedService.GetQuestion(s.postid).Body.Length < 100)
                    { endpos = _sharedService.GetQuestion(s.postid).Body.Length; }
                    p.Body = _sharedService.GetQuestion(s.postid).Body.Substring(0, endpos);

                    p.Title = _sharedService.GetQuestion(s.postid).Title;
                    p.Totalresults = matchcount;
                    p.Rank = s.rank;
                    resultposts.Add(p);
                }*/
            }
            return resultposts;
        }


        public IList<WordRank> WordRank(int userid, string searchstring, int searchtypecode, int? maxresults)
        {
            ////// for performing searches with wordrank on the db
            ///
            // do actual search using appsearch in db and build results

            //need db context and searchtype lookuptable
            using var db = new AppContext();
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
                searchtype.Value = st.searchType[searchtypecode];
            }
            else searchtype.Value = st.searchType[5];

            //userid 
            var appuserid = new NpgsqlParameter("appuserid", NpgsqlTypes.NpgsqlDbType.Integer);
            appuserid.Value = userid;

            //if internal call is specified, stored function appsearch won't add to searches/searchhistory
            var internalcall = new NpgsqlParameter("internalcall", NpgsqlTypes.NpgsqlDbType.Boolean);
            internalcall.Value = true;

            //count all matches
            var matchcount = db.Search
                .FromSqlRaw("select appsearch(@appuserid, @searchtype, @search, @internalcall)", appuserid, searchtype, search, internalcall)
                .Count();
            System.Console.WriteLine($"{matchcount} results.");

            var limit = new NpgsqlParameter("limit", NpgsqlTypes.NpgsqlDbType.Integer);
            limit.Value = 10;
            if (maxresults!=null)
            {
                limit.Value = maxresults;
            }

            //call db.func wordrank
            return db.WordRank
                .FromSqlRaw("SELECT * from wordrank(@appuserid, @searchtype, @search) limit @limit", appuserid, searchtype, search, limit)
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
    }
}
