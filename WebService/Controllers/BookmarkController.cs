using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Entities;
using Domain.Services;
using Repositories.Interfaces;
using WebDTOs;

namespace WebService.Controllers;

[ApiController]
[Route("api/bookmark")]
[Authorize]
public class BookmarkController : SharedController
{
    private IHistoryRepository _historyRepositoryService;
    private ISharedRepository _sharedRepositoryService;

    public BookmarkController(IHistoryRepository historyRepositoryService, ISharedRepository sharedRepositoryService)
    {
        _historyRepositoryService = historyRepositoryService;
        _sharedRepositoryService = sharedRepositoryService;
    }

    [HttpGet(Name = nameof(GetBookmarkList))]
    //example http://localhost:5001/api/bookmark
    //example http://localhost:5001/api/bookmark?Page=1&PageSize=5
    public ActionResult GetBookmarkList([FromQuery] PagingAttributes pagingAttributes)
    {
        if (pagingAttributes.Page < 1 || pagingAttributes.PageSize < 1) return NotFound();
        var userId = GetAuthUserId().Item1;
        var bookmarks = _historyRepositoryService.GetBookmarkList(userId, pagingAttributes);

        if (bookmarks == null)
        {
            return NotFound();
        }

        var count = _historyRepositoryService.GetCount(userId, true);
        return Ok(CreateResult(bookmarks, count, pagingAttributes));
    }

    [HttpPost("add/{postId}", Name = nameof(AddBookmark))]
    //example http://localhost:5001/api/bookmark/add/1760
    public ActionResult AddBookmark(int postId)
    {
        var userId = GetAuthUserId().Item1;
        var result = _historyRepositoryService.Add(userId, postId, true);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("delete/{postId}", Name = nameof(DeleteBookmark))]
    //example http://localhost:5001/api/bookmark/delete/1760
    public ActionResult DeleteBookmark(int postId)
    {
        var userId = GetAuthUserId().Item1;
        var result = _historyRepositoryService.DeleteBookmark(userId, postId);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("delete/all", Name = nameof(DeleteAllBookmarks))]
    //example http://localhost:5001/api/bookmark/delete/all
    public ActionResult DeleteAllBookmarks()
    {
        var userId = GetAuthUserId().Item1;
        var bookmarks = _historyRepositoryService.GetBookmarkList(userId);

        foreach (var bookmark in bookmarks)
        {
            _historyRepositoryService.DeleteBookmark(bookmark.Userid, bookmark.Postid);
        }

        var result = _historyRepositoryService.GetBookmarkList(userId).Count == 0;
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }


    private object CreateResult(IEnumerable<History> list, int count, PagingAttributes attr)
    {
        if (list.FirstOrDefault() != null)
        {
            var totalResults = count;
            var numberOfPages = Math.Ceiling((double)totalResults / attr.PageSize);

            var prev = attr.Page > 1
                ? CreatePagingLink(nameof(GetBookmarkList), attr.Page - 1, attr.PageSize)
                : null;
            var next = attr.Page < numberOfPages
                ? CreatePagingLink(nameof(GetBookmarkList), attr.Page + 1, attr.PageSize)
                : null;

            return new
            {
                totalResults,
                numberOfPages,
                prev,
                next,
                items = list.Select(CreateBookmarkResultDto) //Select() is like a foreach loop
            };
        }
        else
        {
            return null;
        }
    }

    private BookmarkDTO CreateBookmarkResultDto(History hist)
    {
        var post = _sharedRepositoryService.GetPost(hist.Postid);
        var dto = new BookmarkDTO
        {
            Title = post.Title,
            Body = post.Body,
            Date = hist.Date,
            ThreadUrl = Url.Link(
                nameof(QuestionsController.GetThread),
                new { questionId = hist.Postid }
            ),
            PostId = hist.Postid
        };

        return dto;
    }


    public string CreatePagingLink(string nameof, int page, int pageSize)
    {
        return Url.Link(nameof, new { page, pageSize });
    }
}