using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using Domain.Services;
using DomainServices.Interfaces;
using WebService.DTOs;

namespace WebService.Controllers;

[ApiController]
[Route("api/history")]
[Authorize]
public class HistoryController : SharedController
{
    private readonly IHistoryService _historyService;
    private readonly IThreadService _threadService;

    public HistoryController(IHistoryService historyService, IThreadService threadService)
    {
        _historyService = historyService;
        _threadService = threadService;
    }

    [HttpGet(Name = nameof(GetHistory))]
    //example http://localhost:5001/api/history
    //example http://localhost:5001/api/history?Page=1&PageSize=5 
    public ActionResult GetHistory([FromQuery] PagingAttributes pagingAttributes)
    {
        if (pagingAttributes.Page < 1 || pagingAttributes.PageSize < 1) return NotFound();
        var userId = GetAuthUserId().Item1;

        var history = _historyService.GetHistoryList(userId, pagingAttributes);

        if (history == null)
        {
            return NotFound();
        }

        var count = _historyService.GetCount(userId);

        return Ok(CreateResult(history, count, pagingAttributes));
    }

    [HttpDelete("delete/all", Name = nameof(ClearHistory))]
    //example http://localhost:5001/api/history/delete/all
    public ActionResult ClearHistory()
    {
        var userId = GetAuthUserId().Item1;
        var result = _historyService.DeleteUserHistory(userId);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }

    private object CreateResult(IList<History> list, int count, PagingAttributes attr)
    {
        if (list.FirstOrDefault() is null){ return null; }
            
        var totalResults = count;
        var numberOfPages = Math.Ceiling((double)totalResults / attr.PageSize);

        var prev = attr.Page > 1
            ? CreatePagingLink(nameof(GetHistory), attr.Page - 1, attr.PageSize)
            : null;
        var next = attr.Page < numberOfPages
            ? CreatePagingLink(nameof(GetHistory), attr.Page + 1, attr.PageSize)
            : null;

        return new
        {
            totalResults,
            numberOfPages,
            prev,
            next,
            items = list.Select(CreateHistoryResultDto) //Select() is like a foreach loop
        };
    }

    private HistoryDto CreateHistoryResultDto(History hist)
    {
        var post = _threadService.GetPost(hist.PostId);
        var dto = new HistoryDto
        {
            Title = post.Title,
            Body = post.Body,
            Date = hist.Date,
            ThreadUrl = Url.Link(
                nameof(QuestionsController.GetThread),
                new { questionId = hist.PostId }
            )
        };

        return dto;
    }

    private string CreatePagingLink(string nameof, int page, int pageSize)
    {
        return Url.Link(nameof, new { page, pageSize });
    }
}