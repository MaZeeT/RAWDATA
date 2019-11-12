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
        //example http://localhost:5001/api/ //todo make an example
        public ActionResult GetHistory(int userId)
        {
            /*
            var history = _historyService.Get(userId);
            if (history == null)
            {
                return NotFound();
            }
            */
            return Ok();
        }
        

        
        [HttpGet("{userId}", Name = nameof(AddBookmark))]
        //example http://localhost:5001/api/ //todo make an example
        public ActionResult AddBookmark(int userId)
        {
            /*
            var history = _historyService.Get(userId);
            if (history == null)
            {
                return NotFound();
            }
            */
            return Ok();
        }
        
        [HttpGet("{userId}", Name = nameof(GetBookmarks))]
        //example http://localhost:5001/api/ //todo make an example
        public ActionResult GetBookmarks(int userId)
        {
            /*
            var history = _historyService.Get(userId);
            if (history == null)
            {
                return NotFound();
            }
            */
            return Ok();
            
        }
        
        [HttpGet("{userId}", Name = nameof(DeleteBookmark))]
        //example http://localhost:5001/api/ //todo make an example
        public ActionResult DeleteBookmark(int userId)
        {
            /*
            var history = _historyService.Get(userId);
            if (history == null)
            {
                return NotFound();
            }
            */
            return Ok();
            
        }
    }
}