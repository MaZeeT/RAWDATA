using AutoMapper;
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
        private IMapper _mapper;

        public HistoryController(IHistoryService historyService, ISharedService sharedService, IMapper mapper)
        {
            _historyService = historyService;
            _sharedService = sharedService;
            _mapper = mapper;
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

       /* private List<HistoryDTO> ConvertToDto(IEnumerable<History> history)
        {
            List<HistoryDTO> list = new List<HistoryDTO>();
            foreach (var mark in history)
            {
                list.Add(new HistoryDTO
                {
                    Title = _sharedService.GetPost(mark.Postid).Title,
                    Date = mark.Date,
                    ThreadUrl = Url.Link(
                        nameof(QuestionsController.GetThread),
                        new {questionId = mark.Postid}
                    )
                });
            }

            return list;
        }*/

        private HistoryDTO CreateHistoryResultDto(History hist)
        {
            var dto = new HistoryDTO                
            {
                Title = _sharedService.GetPost(hist.Postid).Title,
                Date = hist.Date,
                ThreadUrl = Url.Link(
                        nameof(QuestionsController.GetThread),
                        new { questionId = hist.Postid }
                    )
            };

            return dto;
        }

        private object CreateResult(IEnumerable<History> searches, int count, PagingAttributes attr)
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
                    items = searches.Select(CreateHistoryResultDto)
                };
            }
            else
            {
                return null;
            }
        }

        private string CreatePagingLink(int page, int pageSize)
        {
            return Url.Link(nameof(GetHistory), new { page, pageSize });
        }

    }
}