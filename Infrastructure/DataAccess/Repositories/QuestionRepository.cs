using System.Collections.Generic;
using System.Linq;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.DataAccess.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly DatabaseContext _dbContext;

    public QuestionRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Questions GetQuestion(int questionId)
    {
        return _dbContext.Questions.Find(questionId) ?? throw new KeyNotFoundException($"Question with id {questionId} not found");
    }
    
    public IList<Questions> GetQuestions(PagingAttributes pagingAttributes)
    {
        //convert back from 1-based pages + check/fix page
        var page = ISharedRepository.GetPagination(NumberOfQuestions(), pagingAttributes);

        return _dbContext.Questions
            .OrderBy(u => u.Id)
            .Skip(page * pagingAttributes.PageSize)
            .Take(pagingAttributes.PageSize)
            .ToList();
    }
    
    public int NumberOfQuestions()
    {
        return _dbContext.Questions
            .Count();
    }
}