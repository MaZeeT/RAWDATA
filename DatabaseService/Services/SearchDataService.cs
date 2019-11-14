using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DatabaseService
{
    public class SearchDataService : ISearchDataService
    {
        private ISharedService _sharedService; //shared stuff by injection
        public SearchDataService(
            ISharedService sharedService)
        {
            _sharedService = sharedService;
        }

        public IList<Questions> GetQuestions(PagingAttributes pagingAttributes)
        {
            // AuthUser()
            // if ok do browse q-list
            using var db = new AppContext();

            //try to make pages 1-based
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

            /*     //calc max pages and set requested page to last page if out of bounds
                 var calculatedNumberOfPages = (int)Math.Ceiling((double)matchcount / pagingAttributes.PageSize);
                 System.Console.WriteLine($"{calculatedNumberOfPages} calculated pages.");
                 int page;
                 if (pagingAttributes.Page > calculatedNumberOfPages) 
                 {
                     page = calculatedNumberOfPages;
                 } else if (pagingAttributes.Page <= 0)
                 {
                     page = 0;
                 }
                 else page=pagingAttributes.Page-1;*/

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
                string tablename = _sharedService.GetPostType(s.postid);
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
                }
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

     /*   public int NumberOfQuestions()
        {
            using var db = new AppContext();
            return db.Questions
                .Count();
        }

        public Questions GetQuestion(int questionId)
        {
            using var db = new AppContext();

            return db.Questions.Find(questionId);
        }

        public Answers GetAnswer(int answerId)
        {
            using var db = new AppContext();

            return db.Answers.Find(answerId);
        }*/

  /*      public string GetPostType(int postId)
        // try to get the tablename of post -- answers or questions
        //using varchar resolveid(postid int) in db
        {
            System.Console.WriteLine($"Postid -- {postId}");
            var postid = new NpgsqlParameter("postid", NpgsqlTypes.NpgsqlDbType.Integer);
            postid.Value = postId;
            using var db = new AppContext();
            string tablename = db.PostsTable
                .FromSqlRaw("SELECT * from resolveid(@postid)", postid).First().resolveid;

                System.Console.WriteLine($"Post is part of -- {tablename}");

            return tablename;
        }
        */
   /*     public SinglePost GetPost(int postId)
        //try to get a particular post, q or a
        //returns null if post not found
        //use SinglePost.Id for annotations
        //use SinglePost.QuestionId to get the thread the post belongs to
        {
            SinglePost returnPost = new SinglePost();

            var type = GetPostType(postId);
            if (type=="questions") //then its a question
            {
                var q = GetQuestion(postId);
                returnPost.Body = q.Body;
                returnPost.Id = postId;
                returnPost.QuestionId = q.Id;
                returnPost.Title = q.Title;
                return returnPost;
            }
            else if (type=="answers") //then its an answer
            {
                var a = GetAnswer(postId);
                returnPost.Body = a.Body;
                returnPost.Id = postId;
                returnPost.QuestionId = GetAnswer(postId).Parentid; //get parent q of answer
                returnPost.Title = GetQuestion(returnPost.QuestionId).Title; //get title of parent q
                return returnPost;
            }
            else return null; //else its unknown!
        }*/

/*
        public int GetParentId(int answerID) //maybe not needed?
        {
            System.Console.WriteLine($"Answerid -- {answerID}");
            using var db = new AppContext();
            int parentid = db.Answers
                .Where(e => e.Id == answerID)
                .FirstOrDefault()
                .Parentid;

            System.Console.WriteLine($"Parentid -- {parentid}");

            return parentid;

        }
*/
    
/*
        public IList<Posts> GetThread(int questionId)
            //returns question and all child answers
        {
            using var db = new AppContext();
            //get the question
            var q = GetQuestion(questionId);
            if (q != null)
            {
                //find answers to the specified question
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

                    }) ;
                foreach (Answers a in ans)
                {
                    //below is for limiting body size, disabled rn
                   // var endpos = 100;
                   // if (a.Body.Length<100)
                  //  { 
                  //      endpos = a.Body.Length; //limit body size for now
                  //  }
                    posts.Add(
                    new Posts 
                    {
                        Id = a.Id,
                        Parentid = a.Parentid,
                        Body = a.Body
                        
                        // Body = a.Body.Substring(0, endpos) 
                    });
                };
                return posts;
            }
            else return null;
        }*/
    }
}
