using System.Collections.Generic;
using System.Linq;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.DataAccess.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess.Repositories;

public class SearchHistoryRepository : ISearchHistoryRepository
{
    private readonly DatabaseContext _dbContext;

    public SearchHistoryRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public (List<Searches>, int) GetSearchesList(int userId, PagingAttributes pagingAttributes)
    {
        using var db = _dbContext;

        var count = db.Searches
            .Count(x => x.UserId == userId);

        //try to convert back from 1-based pages
        var page = ISharedRepository.GetPagination(count, pagingAttributes);

        var list = db.Searches
            .Where(x => x.UserId == userId)
            //.OrderByDescending(x => x.Date)
            .Skip(page * pagingAttributes.PageSize)
            .Take(pagingAttributes.PageSize)
            .ToList();

        return (list, count);
    }

    public bool DeleteUserSearchHistory(int userId)
    {
        using var db = _dbContext;
        var rowsDeleted = db.Searches
            .Where(x => x.UserId == userId)
            .ExecuteDelete();
        
        db.SaveChanges();
        
        return rowsDeleted > 0;
    }
}