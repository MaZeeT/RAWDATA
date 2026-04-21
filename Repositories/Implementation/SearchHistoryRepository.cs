using Infrastructure.Database;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementation;

public class SearchHistoryRepository : ISearchHistoryRepository
{
    private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

    public SearchHistoryRepository(IDbContextFactory<DatabaseContext> factory)
    {
        _dbContextFactory = factory;
    }

    public (List<Searches>, int) GetSearchesList(int userId, PagingAttributes pagingAttributes)
    {
        using var db = _dbContextFactory.CreateDbContext();

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
        using var db = _dbContextFactory.CreateDbContext();
        var rowsDeleted = db.Searches
            .Where(x => x.UserId == userId)
            .ExecuteDelete();
        
        db.SaveChanges();
        
        return rowsDeleted > 0;
    }
}