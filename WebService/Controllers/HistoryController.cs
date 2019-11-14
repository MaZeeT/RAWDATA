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
    [Authorize]
    public class HistoryController : ControllerBase
    {
        private IHistoryService _historyService;
        private IMapper _mapper;

        public HistoryController(IHistoryService historyService, IMapper mapper)
        {
            _historyService = historyService;
            _mapper = mapper;
        }

        [HttpGet(Name = nameof(GetHistory))]
        //example http://localhost:5001/api/history 
        public ActionResult GetHistory()
        {
            var userId = GetAuthUserId();
            var history = _historyService.GetHistoryList(userId);
            if (history == null)
            {
                return NotFound();
            }

            return Ok(history);
        }


        [HttpPost("addbookmark/{postId}", Name = nameof(AddBookmark))]
        //example http://localhost:5001/api/history/addbookmark/1760
        public ActionResult AddBookmark(int postId)
        {
            var userId = GetAuthUserId();
            var result = _historyService.Add(userId, postId, true);
            if (!result)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("getbookmarklist", Name = nameof(GetBookmarkList))]
        //example http://localhost:5001/api/history/getbookmarklist
        public ActionResult GetBookmarkList()
        {
            var userId = GetAuthUserId();
            var bookmarks = _historyService.GetBookmarkList(userId);
            if (bookmarks == null)
            {
                return NotFound();
            }

            return Ok(bookmarks);
        }

        [HttpDelete("deletebookmark/{postId}", Name = nameof(DeleteBookmark))]
        //example http://localhost:5001/api/history/deletebookmark/1760
        public ActionResult DeleteBookmark(int postId)
        {
            var userId = GetAuthUserId();
            var result = _historyService.DeleteBookmark(userId, postId);
            if (!result)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("clearhistory", Name = nameof(ClearHistory))]
        //example http://localhost:5001/api/history/clearhistory
        public ActionResult ClearHistory()
        {
            var userId = GetAuthUserId();
            var result = _historyService.DeleteUserHistory(userId);
            if (!result)
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