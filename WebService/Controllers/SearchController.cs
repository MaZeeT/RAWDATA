using AutoMapper;
using DatabaseService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web.Http;
//using WebService.Models;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        private IDataService _dataService;
        private IMapper _mapper;

        public SearchController(
            IDataService dataService,
            IMapper mapper)
        {
            _dataService = dataService;
            _mapper = mapper;
        }

        [HttpGet(Name = nameof(Search)), Route("{s=}/{stype=3}/{page=0}/{pageSize=10}")]
        //[HttpGet] put defalut values here for optional parameters. in this case only s is not optional
        //examples
        // http://localhost:5001/api/search?s=code&stype=0&page=10&pageSize=5
        // http://localhost:5001/api/search?s=code,app,program
        public ActionResult Search([FromQuery] SearchQuery searchparams, [FromQuery] PagingAttributes pagingAttributes)
        {
            if (searchparams.s != null)
            {
                Console.WriteLine("Got searchparams: " + searchparams.s);

                //rudimentary checking of params
                if (searchparams.stype >= 0 && searchparams.stype <= 3)
                {
                    var search = _dataService.Search(searchparams.s, searchparams.stype, pagingAttributes);

                    var result = CreateResult(search, searchparams, pagingAttributes);
                    return Ok(result);
                }
                else if (searchparams.stype >= 4 && searchparams.stype <= 5)
                {
                    var search = _dataService.WordRank(searchparams.s, searchparams.stype, pagingAttributes);
                    return Ok(search);
                }
            }
            return BadRequest();
        }




        ///////////////////
        //
        // Helpers
        //
        //////////////////////

        private PostsSearchListDto CreateSearchResultDto(Posts posts)
        {

            //var dto = _mapper.Map<QuestionDto>(question);
            var dto = new PostsSearchListDto();
            if (posts.Parentid != 0)
            {
                dto.ThreadLink = Url.Link(
                         nameof(QuestionsController.GetThread),
                        new { questionId = posts.Parentid });
            }
            else
            {
                dto.ThreadLink = Url.Link(
         nameof(QuestionsController.GetThread),
        new { questionId = posts.Id });
            }

            dto.Rank = posts.Rank;
            dto.QuestionTitle = posts.Title;
            dto.PostBody = posts.Body;
            dto.PostId = posts.Id;

            return dto;
        }

        private object CreateResult(IEnumerable<Posts> posts, SearchQuery searchparams, PagingAttributes attr)
        {
            var totalResults = posts.First().Totalresults;
            var numberOfPages = Math.Ceiling((double)totalResults / attr.PageSize);

            var prev = attr.Page > 0
                ? CreatePagingLink(searchparams.s, searchparams.stype, attr.Page - 1, attr.PageSize)
                : null;
            var next = attr.Page < numberOfPages - 1
                ? CreatePagingLink(searchparams.s, searchparams.stype, attr.Page + 1, attr.PageSize)
                : null;

            return new
            {
                totalResults,
                numberOfPages,
                prev,
                next,
                items = posts.Select(CreateSearchResultDto)
                //items = posts
            };
        }

        private string CreatePagingLink(string s, int stype, int page, int pageSize)
        {
            return Url.Link(nameof(Search), new { s, stype, page, pageSize });
        }

    }
}