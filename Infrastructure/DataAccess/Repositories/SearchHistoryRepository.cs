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
        var count = _dbContext.Searches
            .Count(x => x.UserId == userId);

        //try to convert back from 1-based pages
        var page = ISharedRepository.GetPagination(count, pagingAttributes);

        var list = _dbContext.Searches
            .Where(x => x.UserId == userId)
            //.OrderByDescending(x => x.Date)
            .Skip(page * pagingAttributes.PageSize)
            .Take(pagingAttributes.PageSize)
            .ToList();

        return (list, count);
    }

    public bool DeleteUserSearchHistory(int userId)
    {
        var rowsDeleted = _dbContext.Searches
            .Where(x => x.UserId == userId)
            .ExecuteDelete();
        
        _dbContext.SaveChanges();
        
        return rowsDeleted > 0;
    }
}