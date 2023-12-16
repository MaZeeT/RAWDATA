using DatabaseService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseService.Modules;
using DatabaseService.Services;
using WebService.DTOs;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/history")]
    [Authorize]
    public class HistoryController : SharedController
    {
        private IHistoryService _historyService;
        private ISharedService _sharedService;

        public HistoryController(IHistoryService historyService, ISharedService sharedService)
        {
            _historyService = historyService;
            _sharedService = sharedService;
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

            var count = _historyService.GetCount(userId, false);

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

        private object CreateResult(IEnumerable<History> list, int count, PagingAttributes attr)
        {
            if (list.FirstOrDefault() != null)
            {
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
            else
            {
                return null;
            }
        }

        private HistoryDTO CreateHistoryResultDto(History hist)
        {
            var post = _sharedService.GetPost(hist.Postid);
            var dto = new HistoryDTO
            {
                Title = post.Title,
                Body = post.Body,
                Date = hist.Date,
                ThreadUrl = Url.Link(
                    nameof(QuestionsController.GetThread),
                    new { questionId = hist.Postid }
                )
            };

            return dto;
        }

        private string CreatePagingLink(string nameof, int page, int pageSize)
        {
            return Url.Link(nameof, new { page, pageSize });
        }
    }
}