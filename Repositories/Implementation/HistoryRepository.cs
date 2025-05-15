using Infrastructure;
using Infrastructure.Database;
using Domain;
using Domain.Entities;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Repositories.Interfaces;

namespace Repositories.Implementation;

public class HistoryRepository : IHistoryRepository
{
    private readonly ISharedRepository _sharedRepository;
    private readonly DatabaseContext2 _database;

    public HistoryRepository(IDbContextFactory<DatabaseContext2> factory, ISharedRepository sharedRepository)
    {
        _sharedRepository = sharedRepository;
        _database = factory.CreateDbContext();
    }


    public bool Add(int userId, int postId, bool isBookmark)
    {
        // ReSharper disable StringLiteralTypo
        var appUserId = new NpgsqlParameter("appuserid", NpgsqlTypes.NpgsqlDbType.Integer);
        var iPostId = new NpgsqlParameter("ipostid", NpgsqlTypes.NpgsqlDbType.Integer);
        var addBookmark = new NpgsqlParameter("addbookmark", NpgsqlTypes.NpgsqlDbType.Boolean);
        // ReSharper restore StringLiteralTypo

        appUserId.Value = userId;
        iPostId.Value = postId;
        addBookmark.Value = isBookmark;
        
        _database.Database.ExecuteSqlRaw(
            "SELECT * from add_history(@appuserid, @ipostid, @addbookmark)",
            appUserId, iPostId, addBookmark);

        return HistoryExist(userId, postId);
    }

    public bool Add(History history)
    {
        return Add(history.Userid, history.Postid, history.isBookmark);
    }

    public History Get(int historyId)
    {
        return _database.History.Find(historyId);
    }

    public History Get(int userId, int postId)
    {
        var histories = _database.History.Where(user => user.Userid == userId && user.Postid == postId).ToList();
        if (histories.Count > 0)
        {
            return histories.First();
        }
        else
        {
            return null;
        }
    }

    public List<History> GetHistoryList(int userId)
    {
        var pagingAttributes = new PagingAttributes();
        return GetHistoryList(userId, pagingAttributes);
    }

    public List<History> GetHistoryList(int userId, PagingAttributes pageAtt)
    {
        return GetListFromQuery(userId, false, pageAtt);
    }

    public List<History> GetBookmarkList(int userId)
    {
        var pagingAttributes = new PagingAttributes();
        return GetBookmarkList(userId, pagingAttributes);
    }

    public List<History> GetBookmarkList(int userId, PagingAttributes pageAtt)
    {
        return GetListFromQuery(userId, true, pageAtt);
    }

    public bool DeleteUserHistory(int userId)
    {
        var history = _database.History.Where(x =>
            x.Userid == userId &&
            x.isBookmark == false);

        foreach (History entry in history)
        {
            _database.History.Remove(entry);
        }

        return _database.SaveChanges() > 0;
    }

    public bool DeleteHistory(int historyId)
    {
        if (HistoryExist(historyId))
        {
            History history = _database.History.Find(historyId);
            _database.History.Remove(history);

            return _database.SaveChanges() > 0;
        }

        return false;
    }

    public bool DeleteBookmark(int userId, int postId)
    {
        var histories = _database.History.Where(x =>
            x.Userid == userId &&
            x.Postid == postId &&
            x.isBookmark == true);
        
        foreach (var history in histories)
        {
            _database.History.Update(history);
            history.isBookmark = false;
        }

        return _database.SaveChanges() > 0;
    }

    public bool HistoryExist(int historyId)
    {
        History result = _database.History.Find(historyId);
        return result != null;
    }

    public bool HistoryExist(int userId, int postId)
    {
        var result = _database.History.Where(history =>
                history.Userid == userId &&
                history.Postid == postId)
            .ToList();

        return result.Count > 0;
    }

    private List<History> GetListFromQuery(int userId, bool isBookmark, PagingAttributes pageAtt)
    {
        // This enforces the page upper and lower limits 
        ISharedRepository.GetPagination(GetCount(userId, isBookmark), pageAtt);

        return _database.History
            .Where(x =>
                x.Userid == userId &&
                x.isBookmark == isBookmark)
            .OrderBy(x => x.Date)
            .Skip((pageAtt.Page - 1) * pageAtt.PageSize)
            .Take(pageAtt.PageSize)
            .ToList();
    }

    public int GetCount(int userId, bool isBookmark)
    {
        return _database.History.Count(x =>
            x.Userid == userId &&
            x.isBookmark == isBookmark);
    }
}