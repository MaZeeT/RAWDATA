using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Application.Interfaces.Services;
using Domain.Entities;
using Web.DTOs;

namespace Web.Controllers;

[ApiController]
[Route("api/bookmark")]
[Authorize]
public class BookmarkController : SharedController
{
    private readonly IBookmarkService _bookmarkService;
    private readonly IThreadService _threadService;

    public BookmarkController(IBookmarkService bookmarkService, IThreadService threadService)
    {
        _bookmarkService = bookmarkService;
        _threadService = threadService;
    }

    [HttpGet(Name = nameof(GetBookmarkList))]
    //example http://localhost:5001/api/bookmark
    //example http://localhost:5001/api/bookmark?Page=1&PageSize=5
    public ActionResult GetBookmarkList([FromQuery] PagingAttributes pagingAttributes)
    {
        if (pagingAttributes.Page < 1 || pagingAttributes.PageSize < 1) return NotFound();
        var userId = GetAuthUserId().Item1;
        var bookmarks = _bookmarkService.GetBookmarkList(userId, pagingAttributes);

        if (bookmarks == null)
        {
            return NotFound();
        }

        var count = _bookmarkService.GetCount(userId);
        return Ok(CreateResult(bookmarks, count, pagingAttributes));
    }

    [HttpPost("add/{postId:int}", Name = nameof(AddBookmark))]
    //example http://localhost:5001/api/bookmark/add/1760
    public ActionResult AddBookmark(int postId)
    {
        var userId = GetAuthUserId().Item1;
        var result = _bookmarkService.Add(userId, postId);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("delete/{postId:int}", Name = nameof(DeleteBookmark))]
    //example http://localhost:5001/api/bookmark/delete/1760
    public ActionResult DeleteBookmark(int postId)
    {
        var userId = GetAuthUserId().Item1;
        var result = _bookmarkService.DeleteBookmark(userId, postId);
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
        var bookmarks = _bookmarkService.GetBookmarkList(userId);

        foreach (var bookmark in bookmarks)
        {
            _bookmarkService.DeleteBookmark(bookmark.UserId, bookmark.PostId);
        }

        var result = _bookmarkService.GetBookmarkList(userId).Count == 0;
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }


    private object CreateResult(IList<History> list, int count, PagingAttributes attr)
    {
        if (list.FirstOrDefault() == null)
        {
            return null;
        }
    
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

    private BookmarkDto CreateBookmarkResultDto(History hist)
    {
        var post = _threadService.GetPost(hist.PostId);
        var dto = new BookmarkDto
        {
            Title = post.Title,
            Body = post.Body,
            Date = hist.Date,
            ThreadUrl = Url.Link(
                nameof(QuestionsController.GetThread),
                new { questionId = hist.PostId }
            ),
            PostId = hist.PostId
        };

        return dto;
    }


    private string CreatePagingLink(string nameof, int page, int pageSize)
    {
        return Url.Link(nameof, new { page, pageSize });
    }
}