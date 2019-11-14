using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseService
{
    public class SharedService : ISharedService
    {
        public string GetPostType(int postId)
        // try to get the tablename of post -- answers or questions
        //using varchar resolveid(postid int) in db
        {
            System.Console.WriteLine($"Postid -- {postId}");
            var postid = new NpgsqlParameter("postid", NpgsqlTypes.NpgsqlDbType.Integer);
            postid.Value = postId;
            using var db = new DatabaseContext();
            string tablename = db.PostsTable
                .FromSqlRaw("SELECT * from resolveid(@postid)", postid).First().resolveid;

            System.Console.WriteLine($"Post is part of -- {tablename}");

            return tablename;
        }

        public int NumberOfQuestions()
        {
            using var db = new DatabaseContext();
            return db.Questions
                .Count();
        }

        public Questions GetQuestion(int questionId)
        {
            using var db = new DatabaseContext();
            return db.Questions.Find(questionId);
        }

        public Answers GetAnswer(int answerId)
        {
            using var db = new DatabaseContext();
            return db.Answers.Find(answerId);
        }

        public SinglePost GetPost(int postId)
        //try to get a particular post, q or a
        //returns null if post not found
        //use SinglePost.Id for annotations
        //use SinglePost.QuestionId to get the thread the post belongs to
        {
            SinglePost returnPost = new SinglePost();

            var type = GetPostType(postId);
            if (type == "questions") //then its a question
            {
                var q = GetQuestion(postId);
                returnPost.Body = q.Body;
                returnPost.Id = postId;
                returnPost.QuestionId = q.Id;
                returnPost.Title = q.Title;
                return returnPost;
            }
            else if (type == "answers") //then its an answer
            {
                var a = GetAnswer(postId);
                returnPost.Body = a.Body;
                returnPost.Id = postId;
                returnPost.QuestionId = GetAnswer(postId).Parentid; //get parent q of answer
                returnPost.Title = GetQuestion(returnPost.QuestionId).Title; //get title of parent q
                return returnPost;
            }
            else return null; //else its unknown!
        }
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
        public IList<Posts> GetThread(int questionId)
        //returns question and all child answers
        {
            using var db = new DatabaseContext();
            //get the question
            var q = GetQuestion(questionId);
            if (q != null)
            {
                //find answers to the specified question
                var ans = db.Answers
                            .Where(e => e.Parentid == questionId)
                            .ToList();
                //manual mapping
                List<Posts> posts = new List<Posts>
                {
                    new Posts
                    {
                        Id = q.Id,
                        Title = q.Title,
                        Body = q.Body

                    }
                };
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
        }

        public int GetPagination(int matchcount, PagingAttributes pagingAttributes)
        {
            //calc max pages and set requested page to last page if out of bounds
            var calculatedNumberOfPages = (int)Math.Ceiling((double)matchcount / pagingAttributes.PageSize);
            System.Console.WriteLine($"{calculatedNumberOfPages} calculated pages.");
            int page;
            if (pagingAttributes.Page > calculatedNumberOfPages)
            {
                page = calculatedNumberOfPages;
            }
            else if (pagingAttributes.Page <= 0)
            {
                page = 0;
            }
            else page = pagingAttributes.Page - 1;
            return page;
        }

    }
}
