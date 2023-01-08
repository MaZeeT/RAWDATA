using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DatabaseService;
using System.Collections.Generic;
using System.Linq;
using System;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/history/searches")]
    [Authorize]
    public class SearchHistoryController : SharedController
    {
        private readonly ISearchHistoryService _searchHistoryService;
        private readonly ISearchDataService _dataService;

        public SearchHistoryController(
            ISearchHistoryService searchHistoryService,
            ISearchDataService dataService)
        {
            _searchHistoryService = searchHistoryService;
            _dataService = dataService;
        }

        [HttpGet(Name = nameof(GetSearchHistory))]
        //example http://localhost:5001/api/history/searches 
        public ActionResult GetSearchHistory([FromQuery] PagingAttributes pagingAttributes)
        {
            (int userId, bool useridok) = GetAuthUserId();
            if (!useridok)
            {
                return Unauthorized();
            }

            (var shistory, int count) = _searchHistoryService.GetSearchesList(userId, pagingAttributes);
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

            var result = _searchHistoryService.DeleteUserSearchHistory(userId);
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
                s = _dataService.BuildSearchString(searches.SearchString, true);
            }

            var stype = _dataService.SearchTypeLookup(searches.SearchType);

            dto.SearchLink = Url.Link(
                nameof(SearchController.Search),
                new
                {
                    s,
                    stype
                });

            dto.SearchMethod = searches.SearchType;
            dto.SearchString = searches.SearchString;
            dto.Date = searches.Date;

            return dto;
        }

        private object CreateResult(IEnumerable<Searches> searches, int count, PagingAttributes attr)
        {
            if (searches.FirstOrDefault() != null)
            {
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
            else
            {
                return null;
            }
        }

        private string CreatePagingLink(int page, int pageSize)
        {
            return Url.Link(nameof(GetSearchHistory), new { page, pageSize });
        }
    }
}