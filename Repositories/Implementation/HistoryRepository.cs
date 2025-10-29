using Infrastructure.Database;
using Domain.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementation;

public class HistoryRepository : IHistoryRepository
{
    private readonly DatabaseContext2 _database;

    public HistoryRepository(IDbContextFactory<DatabaseContext2> factory)
    {
        _database = factory.CreateDbContext();
    }
    

    public bool Add(History history)
    {
        _database.History.Add(history);
        var result = _database.SaveChanges();
        return result > 0;
    }

    public History Get(int historyId)
    {
        return _database.History.Find(historyId);
    }

    public History Get(int userId, int postId)
    {
        var histories = _database.History.Where(user => user.UserId == userId && user.PostId == postId).ToList();
        if (histories.Count > 0)
        {
            return histories[0];
        }
        
        return null;
    }

    public List<History> GetHistoryList(int userId)
    {
        var pagingAttributes = new PagingAttributes();
        return GetHistoryList(userId, pagingAttributes);
    }

    public List<History> GetHistoryList(int userId, PagingAttributes pagingAttributes)
    {
        return GetListFromQuery(userId, false, pagingAttributes);
    }

    public List<History> GetBookmarkList(int userId)
    {
        var pagingAttributes = new PagingAttributes();
        return GetBookmarkList(userId, pagingAttributes);
    }

    public List<History> GetBookmarkList(int userId, PagingAttributes pagingAttributes)
    {
        return GetListFromQuery(userId, true, pagingAttributes);
    }

    public bool DeleteUserHistory(int userId)
    {
        var history = _database.History.Where(x =>
            x.UserId == userId &&
            x.IsBookmark == false);

        foreach (var entry in history)
        {
            _database.History.Remove(entry);
        }

        return _database.SaveChanges() > 0;
    }

    public bool DeleteHistory(int historyId)
    {
        if (!HistoryExist(historyId))
        {
            return false;
        }

        History history = _database.History.Find(historyId);
        _database.History.Remove(history);

        return _database.SaveChanges() > 0;
    }

    public bool DeleteBookmark(int userId, int postId)
    {
        var histories = _database.History.Where(x =>
            x.UserId == userId &&
            x.PostId == postId &&
            x.IsBookmark == true);
        
        foreach (var history in histories)
        {
            _database.History.Update(history);
            history.IsBookmark = false;
        }

        return _database.SaveChanges() > 0;
    }

    public bool HistoryExist(int historyId)
    {
        History result = _database.History.Find(historyId);
        return result != null;
    }

    private bool HistoryExist(int userId, int postId)
    {
        var result = _database.History.Where(history =>
                history.UserId == userId &&
                history.PostId == postId)
            .ToList();

        return result.Count > 0;
    }

    private List<History> GetListFromQuery(int userId, bool isBookmark, PagingAttributes pageAtt)
    {
        // This enforces the page upper and lower limits 
        ISharedRepository.GetPagination(GetCount(userId, isBookmark), pageAtt);

        return _database.History
            .Where(x =>
                x.UserId == userId &&
                x.IsBookmark == isBookmark)
            .OrderBy(x => x.Date)
            .Skip((pageAtt.Page - 1) * pageAtt.PageSize)
            .Take(pageAtt.PageSize)
            .ToList();
    }

    public int GetCount(int userId, bool isBookmark)
    {
        return _database.History.Count(x =>
            x.UserId == userId &&
            x.IsBookmark == isBookmark);
    }
}