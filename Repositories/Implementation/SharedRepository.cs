using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Repositories.Interfaces;

namespace Repositories.Implementation;

public class SharedRepository : ISharedRepository
{
    private readonly IDbContextFactory<DatabaseContext2> _dbContextFactory;
    private readonly IQuestionRepository _questionRepository;

    public SharedRepository(IDbContextFactory<DatabaseContext2> factory, IQuestionRepository questionRepository)
    {
        _dbContextFactory = factory;
        _questionRepository = questionRepository;
    }
    
    public string GetPostType(int postId)
        // try to get the tablename of post -- answers or questions
        //using varchar resolveid(postid int) in db
    {
        System.Console.WriteLine($"Postid -- {postId}");
        var postid = new NpgsqlParameter("postid", NpgsqlTypes.NpgsqlDbType.Integer)
        {
            Value = postId
        };
        using var db = _dbContextFactory.CreateDbContext();
        var tablename = db.PostsTable
            .FromSqlRaw("SELECT * from resolveid(@postid)", postid).First().resolveid;

        System.Console.WriteLine($"Post is part of -- {tablename}");

        return tablename;
    }

    private Answers GetAnswer(int answerId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return db.Answers.Find(answerId);
    }

    public SinglePost GetPost(int postId)
        //try to get a particular post, q or a
        //returns null if post not found
        //use SinglePost.Id for annotations
        //use SinglePost.QuestionId to get the thread the post belongs to
    {
        var returnPost = new SinglePost();

        var type = GetPostType(postId);
        switch (type)
        {
            //then its a question
            case "questions":
            {
                var q = _questionRepository.GetQuestion(postId);
                returnPost.Body = q.Body;
                returnPost.Id = postId;
                returnPost.QuestionId = q.Id;
                returnPost.Title = q.Title;
                return returnPost;
            }
            //then its an answer
            case "answers":
            {
                var a = GetAnswer(postId);
                returnPost.Body = a.Body;
                returnPost.Id = postId;
                returnPost.QuestionId = GetAnswer(postId).Parentid; //get parent q of answer
                returnPost.Title = _questionRepository.GetQuestion(returnPost.QuestionId).Title; //get title of parent q
                return returnPost;
            }
            default:
                return null; //else its unknown!
        }
    }

    public IList<Posts> GetThread(int questionId)
        //returns question and all child answers
    {
        using var db = _dbContextFactory.CreateDbContext();
        //get the question
        var q = _questionRepository.GetQuestion(questionId);
        if (q == null)
        {
            return null;
        }
        
        //find answers to the specified question
        var answers = db.Answers
            .Where(e => e.Parentid == questionId)
            .ToList();
        //manual mapping
        var posts = new List<Posts>
        {
            new Posts
            {
                Id = q.Id,
                Title = q.Title,
                Body = q.Body
            }
        };
        foreach (var answer in answers)
        {
            posts.Add(
                new Posts
                {
                    Id = answer.Id,
                    Parentid = answer.Parentid,
                    Body = answer.Body
                });
        }

        return posts;
    }


}