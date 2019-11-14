using AutoMapper;
using DatabaseService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseService.Services;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/history")]
    public class HistoryController : ControllerBase
    {
        private IHistoryService _historyService;
        private IMapper _mapper;

        public HistoryController(IHistoryService historyService, IMapper mapper)
        {
            _historyService = historyService;
            _mapper = mapper;
        }

        [HttpGet("/{userId}", Name = nameof(GetHistory))]
        //example http://localhost:5001/api/ //todo make an example
        public ActionResult GetHistory(int userId)
        {
            var history = _historyService.GetHistoryList(userId);
            if (history == null)
            {
                return NotFound();
            }

            return Ok(history);
        }

        [HttpGet("/addbookmark/{userId}/{PostId}", Name = nameof(AddBookmark))]
        //example http://localhost:5001/api/ //todo make an example
        public ActionResult AddBookmark(int userId, int PostId)
        {
            var history = _historyService.Add(userId, PostId, true);
            if (!history)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("{userId}", Name = nameof(GetBookmarks))]
        //example http://localhost:5001/api/ //todo make an example
        public ActionResult GetBookmarks(int userId)
        {
            var bookmarks = _historyService.GetBookmarks(userId);
            if (bookmarks == null)
            {
                return NotFound();
            }

            return Ok(bookmarks);
        }

        [HttpGet("/deletebookmark/{historyId}", Name = nameof(DeleteBookmark))]
        //example http://localhost:5001/api/ //todo make an example
        public ActionResult DeleteBookmark(int historyId)
        {
            var history = _historyService.Delete(historyId);
            if (!history)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}