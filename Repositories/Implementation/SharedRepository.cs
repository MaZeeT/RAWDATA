using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Implementation;

public class SharedRepository : ISharedRepository
{
    private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;
    private readonly IQuestionRepository _questionRepository;

    public SharedRepository(IDbContextFactory<DatabaseContext> factory, IQuestionRepository questionRepository)
    {
        _dbContextFactory = factory;
        _questionRepository = questionRepository;
    }
    
    public string GetPostType(int postId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var postTypeId = db.QAndA.Find(postId);
        var tablename = postTypeId?.PostTypeId switch
        {
            1 => "questions",
            2 => "answers",
            _ => "unknown"  //catches postTypeId when null
        };
        
        Console.WriteLine($"Post is part of -- {tablename}");
        return tablename;
    }

    private Answers GetAnswer(int answerId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return db.Answers.Find(answerId) ?? throw new KeyNotFoundException($"Answer with id {answerId} not found");
    }

    public SinglePost GetPost(int postId)
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
                returnPost.QuestionId = GetAnswer(postId).ParentId; //get parent q of answer
                returnPost.Title = _questionRepository.GetQuestion(returnPost.QuestionId).Title; //get title of parent q
                return returnPost;
            }
            default:
                throw new ArgumentOutOfRangeException($"type {type} is unknown");
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
            .Where(e => e.ParentId == questionId)
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
                    ParentId = answer.ParentId,
                    Body = answer.Body
                });
        }

        return posts;
    }


}