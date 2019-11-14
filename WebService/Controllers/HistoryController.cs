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

        [HttpGet("{userId}", Name = nameof(GetHistory))]
        //example http://localhost:5001/api/history/0 
        public ActionResult GetHistory(int userId)
        {
            var history = _historyService.GetHistoryList(userId);
            if (history == null)
            {
                return NotFound();
            }

            return Ok(history);
        }


        [HttpPost("addbookmark/{userId}/{postId}", Name = nameof(AddBookmark))]
        //example http://localhost:5001/api/ //todo make an example
        public ActionResult AddBookmark(int userId, int postId)
        {
            var history = _historyService.Add(userId, postId, true);
            if (!history)
            {
                return NotFound();
            }

            return Ok(history);
        }

        [HttpGet("getbookmarklist/{userId}", Name = nameof(GetBookmarkList))]
        //example http://localhost:5001/api/ //todo make an example
        public ActionResult GetBookmarkList(int userId)
        {
            var userId2 = GetAuthUserId();
            var bookmarks = _historyService.GetBookmarkList(userId);
            if (bookmarks == null)
            {
                return NotFound();
            }

            return Ok(bookmarks);
        }

        [HttpGet("deletebookmark/{historyId}", Name = nameof(DeleteBookmark))]
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


        //todo move the function below into an abstract class or something to remove duplicated code
        /// <summary>
        /// Get the authenticated user's id from token claim
        /// </summary>
        /// <returns>integer authenticated user's id from token</returns>
        private int GetAuthUserId()
        {
            var userIdFromToken = int.Parse(this.User.Identity.Name);
            return userIdFromToken;
        }
    }
}