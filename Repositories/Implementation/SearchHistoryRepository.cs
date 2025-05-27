using Infrastructure.Database;
using Domain.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementation;

public class SearchHistoryRepository : ISearchHistoryRepository
{
    private readonly IDbContextFactory<DatabaseContext2> _dbContextFactory;

    public SearchHistoryRepository(IDbContextFactory<DatabaseContext2> factory)
    {
        _dbContextFactory = factory;
    }

    public (List<Searches>, int) GetSearchesList(int userId, PagingAttributes pagingAttributes)
    {
        using var db = _dbContextFactory.CreateDbContext();

        var count = db.Searches
            .Where(x => x.UserId == userId)
            .Count();

        //try to convert back from 1-based pages
        int page = ISharedRepository.GetPagination(count, pagingAttributes);

        var list = db.Searches
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Date)
            .Skip(page * pagingAttributes.PageSize)
            .Take(pagingAttributes.PageSize)
            .ToList();

        return (list, count);
    }

    public bool DeleteUserSearchHistory(int userId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var history = db.Searches.Where(x =>
            x.UserId == userId);

        foreach (var entry in history)
        {
            db.Searches.Remove(entry);
        }

        return db.SaveChanges() > 0;
    }

    public bool DeleteSearchHistory(int searchId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        if (SearchExist(searchId))
        {
            var history = db.History.Find(searchId);
            db.History.Remove(history);

            return db.SaveChanges() > 0;
        }

        return false;
    }

    public bool SearchExist(int searchId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var result = db.Searches.Find(searchId);
        return result != null;
    }
}