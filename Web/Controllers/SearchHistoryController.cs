using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Web.DTOs;

namespace Web.Controllers;

[ApiController]
[Route("api/history/searches")]
[Authorize]
public class SearchHistoryController : SharedController
{
    private readonly ISearchService _searchService;

    public SearchHistoryController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet(Name = nameof(GetSearchHistory))]
    //example http://localhost:5001/api/history/searches 
    public ActionResult GetSearchHistory([FromQuery] PagingAttributes pagingAttributes)
    {
        var (userId, userIdOk) = GetAuthUserId();
        if (!userIdOk)
        {
            return Unauthorized();
        }

        var (searchHistory, count) = _searchService.GetSearchesList(userId, pagingAttributes);
        if (searchHistory == null || count == 0)
        {
            searchHistory = new List<Searches>();
            var dummyitem = new Searches(); //TODO why a dummy item?
            searchHistory.Add(dummyitem);
            count = 0;
        }

        var result = CreateResult(searchHistory, count, pagingAttributes);
        if (result is null)
        {
            return NoContent();
        }
        return Ok(result);
    }

    [HttpDelete("delete/all", Name = nameof(ClearSearchHistory))]
    //example http://localhost:5001/api/history/searches/delete/all
    public ActionResult ClearSearchHistory()
    {
        var (userId, userIdOk) = GetAuthUserId();
        if (!userIdOk)
        {
            return Unauthorized();
        }

        var result = _searchService.DeleteUserSearchHistory(userId);
        if (!result)
        {
            return NotFound();
        }

        return Ok(result);
    }

    private SearchHistoryListDto CreateSearchHistoryResultDto(Searches searches)
    {
        var searchString = "";
        if (searches.SearchString != null)
        {
            searchString = _searchService.BuildSearchString(searches.SearchString, true);
        }

        var url = Url.Link(
            nameof(SearchController.Search),
            new{
                s = searchString, stype = searches.SearchType
            });

        return new SearchHistoryListDto{
            SearchLink = url,
            SearchMethod = searches.SearchType,
            SearchString = searches.SearchString,
            Date = searches.Date
        };
    }

    private object CreateResult(IEnumerable<Searches> searches, int count, PagingAttributes attr)
    {
        if (searches.FirstOrDefault() is null)
            return null;
        
        var totalResults = count;
        var numberOfPages = Math.Ceiling((double)totalResults / attr.PageSize);

        var prev = attr.Page > 1
            ? CreatePagingLink(attr.Page - 1, attr.PageSize)
            : null;
        var next = attr.Page < numberOfPages
            ? CreatePagingLink(attr.Page + 1, attr.PageSize)
            : null;

        return new
        {
            totalResults,
            numberOfPages,
            prev,
            next,
            items = searches.Select(CreateSearchHistoryResultDto)
        };
    
    }

    private string CreatePagingLink(int page, int pageSize)
    {
        return Url.Link(nameof(GetSearchHistory), new { page, pageSize });
    }
}