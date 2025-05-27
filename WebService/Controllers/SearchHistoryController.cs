using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using Domain.Models;
using Domain.Services;
using DomainServices.Interfaces;
using WebService.DTOs;

namespace WebService.Controllers;

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
        (int userId, bool useridok) = GetAuthUserId();
        if (!useridok){ return Unauthorized(); }

        (var shistory, int count) = _searchService.GetSearchesList(userId, pagingAttributes);
        if (shistory == null || count == 0)
        {
            //return NotFound();
            shistory = new List<Searches>();
            var dummyitem = new Searches();
            shistory.Add(dummyitem);
            count = 0;
        }

        var result = CreateResult(shistory, count, pagingAttributes);
        if (result != null)
        {
            return Ok(result);
        }
        else return NoContent();
    }

    [HttpDelete("delete/all", Name = nameof(ClearSearchHistory))]
    //example http://localhost:5001/api/history/searches/delete/all
    public ActionResult ClearSearchHistory()
    {
        (int userId, bool useridok) = GetAuthUserId();
        if (!useridok)
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

    ///////////////////
    //
    // Helpers
    //
    //////////////////////

    private SearchHistoryListDto CreateSearchHistoryResultDto(Searches searches)
    {
        var dto = new SearchHistoryListDto();

        var s = "";
        if (searches.SearchString != null)
        {
            s = _searchService.BuildSearchString(searches.SearchString, true);
        }

        var stype = _searchService.SearchTypeLookup(searches.SearchType);

        var url = Url.Link(
            nameof(SearchController.Search),
            new{
                s,
                stype
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