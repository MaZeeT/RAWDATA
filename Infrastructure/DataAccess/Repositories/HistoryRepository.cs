using System;
using System.Collections.Generic;
using System.Linq;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.DataAccess.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess.Repositories;

public class HistoryRepository : IHistoryRepository
{
    private readonly DatabaseContext _dbContext;

    public HistoryRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }


    public bool Add(History history)
    {
        _dbContext.History.Add(history);
        var result = _dbContext.SaveChanges();
        return result > 0;
    }

    public History Fetch(int historyId)
    {
        return _dbContext.History.Find(historyId)
               ?? throw new ArgumentException("HistoryId not found");
    }

    public History Fetch(int userId, int postId)
    {
        var histories = _dbContext.History.Where(user => user.UserId == userId && user.PostId == postId).ToList();
        if (histories.Count > 0)
        {
            return histories[0];
        }

        throw new ArgumentException("HistoryEntity not found");
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
        var history = _dbContext.History.Where(x =>
            x.UserId == userId &&
            x.IsBookmark == false);

        foreach (var entry in history)
        {
            _dbContext.History.Remove(entry);
        }

        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteHistory(int historyId)
    {
        if (!HistoryExist(historyId))
        {
            return false;
        }

        var history = _dbContext.History.Find(historyId);
        _dbContext.History.Remove(history);

        return _dbContext.SaveChanges() > 0;
    }

    public bool DeleteBookmark(int userId, int postId)
    {
        var histories = _dbContext.History.Where(x =>
            x.UserId == userId &&
            x.PostId == postId &&
            x.IsBookmark == true);

        foreach (var history in histories)
        {
            _dbContext.History.Update(history);
            history.IsBookmark = false;
        }

        return _dbContext.SaveChanges() > 0;
    }

    public bool HistoryExist(int historyId)
    {
        var result = _dbContext.History.Find(historyId);
        return result != null;
    }

    private bool HistoryExist(int userId, int postId)
    {
        var result = _dbContext.History.Where(history =>
                history.UserId == userId &&
                history.PostId == postId)
            .ToList();

        return result.Count > 0;
    }

    private List<History> GetListFromQuery(int userId, bool isBookmark, PagingAttributes pageAtt)
    {
        // This enforces the page upper and lower limits 
        ISharedRepository.GetPagination(GetCount(userId, isBookmark), pageAtt);

        return _dbContext.History
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
        return _dbContext.History.Count(x =>
            x.UserId == userId &&
            x.IsBookmark == isBookmark);
    }
}