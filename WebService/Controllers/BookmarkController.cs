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
    [Route("api/bookmark")]
    [Authorize]
    public class BookmarkController : SharedController
    {
        private IHistoryService _historyService;
        private ISharedService _sharedService;
        private IMapper _mapper;

        public BookmarkController(IHistoryService historyService, ISharedService sharedService, IMapper mapper)
        {
            _historyService = historyService;
            _sharedService = sharedService;
            _mapper = mapper;
        }

        [HttpGet(Name = nameof(GetBookmarkList))]
        //example http://localhost:5001/api/bookmark
        //example http://localhost:5001/api/bookmark?Page=1&PageSize=5
        public ActionResult GetBookmarkList([FromQuery]int page = 1, [FromQuery]int pageSize = 10)
        {
            if (page < 1 || pageSize < 1) return NotFound();
            
            var userId = GetAuthUserId().Item1;
            PagingAttributes pagingAttributes = new PagingAttributes
            {
                Page = page,
                PageSize = pageSize
            };
            var bookmarks = _historyService.GetBookmarkList(userId,pagingAttributes);

            if (bookmarks == null)
            {
                return NotFound();
            }

            return Ok(ConvertToDto(bookmarks));
        }

        [HttpPost("add/{postId}", Name = nameof(AddBookmark))]
        //example http://localhost:5001/api/bookmark/add/1760
        public ActionResult AddBookmark(int postId)
        {
            var userId = GetAuthUserId().Item1;
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
            var userId = GetAuthUserId().Item1;
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
            var userId = GetAuthUserId().Item1;
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

        private List<BookmarkDTO> ConvertToDto(IEnumerable<History> bookmarks)
        {
            List<BookmarkDTO> list = new List<BookmarkDTO>();
            foreach (var mark in bookmarks)
            {
                list.Add(new BookmarkDTO
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
        }
    }
}