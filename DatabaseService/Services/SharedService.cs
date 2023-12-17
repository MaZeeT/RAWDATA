using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseService.Interfaces.Repositories;

namespace DatabaseService;

public class SharedService : IShared
{
    public string GetPostType(int postId)
        // try to get the tablename of post -- answers or questions
        //using varchar resolveid(postid int) in db
    {
        System.Console.WriteLine($"Postid -- {postId}");
        var postid = new NpgsqlParameter("postid", NpgsqlTypes.NpgsqlDbType.Integer)
        {
            Value = postId
        };
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

    public IList<Posts> GetThread(int questionId)
        //returns question and all child answers
    {
        using var db = new DatabaseContext();
        //get the question
        var q = GetQuestion(questionId);
        if (q != null)
        {
            //find answers to the specified question
            var answers = db.Answers
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
            foreach (Answers a in answers)
            {
                posts.Add(
                    new Posts
                    {
                        Id = a.Id,
                        Parentid = a.Parentid,
                        Body = a.Body
                    });
            }

            ;
            return posts;
        }
        else return null;
    }

    public int GetPagination(int matchcount, PagingAttributes pagingAttributes)
    {
        //calc max pages and set requested page to last page if out of bounds
        var maxPages = (int)Math.Ceiling((double)matchcount / pagingAttributes.PageSize);
        var minPages = 1;

        System.Console.WriteLine($"{maxPages} calculated pages.");

        if (pagingAttributes.Page > maxPages)
        {
            pagingAttributes.Page = maxPages;
        }
        else if (pagingAttributes.Page < minPages)
        {
            pagingAttributes.Page = minPages;
        }

        return pagingAttributes.Page - 1; // return 0 indexed
    }
}