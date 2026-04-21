using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementation;

public class QuestionRepository : IQuestionRepository
{
    private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

    public QuestionRepository(IDbContextFactory<DatabaseContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public Questions GetQuestion(int questionId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return db.Questions.Find(questionId) ?? throw new KeyNotFoundException($"Question with id {questionId} not found");
    }
    
    public IList<Questions> GetQuestions(PagingAttributes pagingAttributes)
    {
        //// for browsing the full list of questions
        using var db = _dbContextFactory.CreateDbContext();

        //convert back from 1-based pages + check/fix page
        var page = ISharedRepository.GetPagination(NumberOfQuestions(), pagingAttributes);

        return db.Questions
            .OrderBy(u => u.Id)
            .Skip(page * pagingAttributes.PageSize)
            .Take(pagingAttributes.PageSize)
            .ToList();
    }
    
    public int NumberOfQuestions()
    {
        using var db = _dbContextFactory.CreateDbContext();
        return db.Questions
            .Count();
    }
}