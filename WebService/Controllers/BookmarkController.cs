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
    [Route("api/bookmark")]
    [Authorize]
    public class BookmarkController : ControllerBase
    {
        private IHistoryService _historyService;
        private IMapper _mapper;

        public BookmarkController(IHistoryService historyService, IMapper mapper)
        {
            _historyService = historyService;
            _mapper = mapper;
        }

        [HttpGet(Name = nameof(GetBookmarkList))]
        //example http://localhost:5001/api/bookmark
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
        
        [HttpPost("add/{postId}", Name = nameof(AddBookmark))]
        //example http://localhost:5001/api/bookmark/add/1760
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

        [HttpDelete("delete/{postId}", Name = nameof(DeleteBookmark))]
        //example http://localhost:5001/api/bookmark/delete/1760
        public ActionResult DeleteBookmark(int postId)
        {
            var userId = GetAuthUserId();
            var result = _historyService.DeleteBookmark(userId, postId);
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
            var userId = GetAuthUserId();
            var bookmarks = _historyService.GetBookmarkList(userId);

            foreach (var bookmark in bookmarks)
            {
                _historyService.DeleteBookmark(bookmark.Userid, bookmark.Postid);
            }

            var result = _historyService.GetBookmarkList(userId).Count == 0;
            if (!result)
            {
                return NotFound();
            }

            return Ok(result);
            
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