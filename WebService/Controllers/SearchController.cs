using DatabaseService;
using DatabaseService.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebService.Controllers;

[ApiController]
[Route("api/search")]
[Authorize]
///
/// when accessing with tokens, the header needs a key Authorization with a value of Bearer [space] and then the token (no quotes)
///
public class SearchController : SharedController
{
    private readonly ISearch _dataService;

    public SearchController(
        ISearch dataService)
    {
        _dataService = dataService;
    }

    [HttpGet("wordrank", Name = nameof(WordRank))]
    public ActionResult WordRank([FromQuery] SearchQuery searchparams, [FromQuery] int? maxresults) //
        // http://localhost:5001/api/search/wordrank?s=code&stype=5&maxresults=5
        // http://localhost:5001/api/search/wordrank?s=code,app,program
    {
        (int userId, bool useridok) = GetAuthUserId();

        Console.WriteLine("Got user: " + userId);

        if (searchparams.s != null && useridok)
        {
            Console.WriteLine("Got searchparams: " + searchparams.s);
            Console.WriteLine("Got maxresults: " + maxresults);

            //checking of params
            if (searchparams.stype >= 0 && searchparams.stype <= 3)
            {
                //wrong search type, redirect
                return RedirectToAction("Search", new { searchparams.s, searchparams.stype });
            }
            else if (searchparams.stype >= 4 && searchparams.stype <= 5)
            {
                var search = _dataService.WordRank(userId, searchparams.s, searchparams.stype, maxresults);
                return Ok(search);
            }
            else
            {
                var search = _dataService.WordRank(userId, searchparams.s, 5, maxresults);
                return Ok(search);
            }
        }

        return BadRequest();
    }

    [HttpGet(Name = nameof(Search))]
    //examples
    // http://localhost:5001/api/search?s=code&stype=0&page=10&pageSize=5
    // http://localhost:5001/api/search?s=code,app,program
    public ActionResult Search([FromQuery] SearchQuery searchparams, [FromQuery] PagingAttributes pagingAttributes)
    {
        (int userId, bool useridok) = GetAuthUserId();

        Console.WriteLine("Got user: " + userId);

        if (searchparams.s != null && useridok)
        {
            Console.WriteLine("Got searchparams: " + searchparams.s);

            //checking of params
            if (searchparams.stype >= 0 && searchparams.stype <= 3)
            {
                //do search, fix page also if needed as a bonus
                var search = _dataService.Search(userId, searchparams.s, searchparams.stype, pagingAttributes);

                // try to fix searchsting for link generation if it seems useable but ugly
                searchparams.s = _dataService.BuildSearchString(searchparams.s, true);

                var result = CreateResult(search, searchparams, pagingAttributes);
                if (result != null)
                {
                    return Ok(result);
                }
                else return NoContent();
            }
            else if (searchparams.stype >= 4 && searchparams.stype <= 5)
            {
                //wrong search type, redirect
                return RedirectToAction("WordRank", new { searchparams.s, searchparams.stype });
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
        return posts.Parentid == 0
            ? GetPostQuestionDto(posts)
            : GetPostAnswerDto(posts);
    }

    private PostsSearchListDto GetPostQuestionDto(Posts posts){
        return new PostsSearchListDto{
            Rank = posts.Rank,
            QuestionTitle = posts.Title,
            PostBody = posts.Body,
            PostId = posts.Id,
            ThreadLink = Url.Link(
                nameof(QuestionsController.GetThread),
                new
                {
                    questionId = posts.Id
                })
        };
    }

    private PostsSearchListDto GetPostAnswerDto(Posts posts){
        return new PostsSearchListDto{
            Rank = posts.Rank,
            QuestionTitle = posts.Title,
            PostBody = posts.Body,
            PostId = posts.Id,
            ThreadLink = Url.Link(
                nameof(QuestionsController.GetThread),
                new
                {
                    questionId = posts.Parentid,
                    postId = posts.Id
                })
        };
    }


    private object CreateResult(IEnumerable<Posts> posts, SearchQuery searchparams, PagingAttributes attr)
    {
        if (posts.FirstOrDefault() != null){ return null; }
         
        var totalResults = posts.First().Totalresults;
        var numberOfPages = Math.Ceiling((double)totalResults / attr.PageSize);

        var prev = attr.Page > 1
            ? CreatePagingLink(searchparams.s, searchparams.stype, attr.Page - 1, attr.PageSize)
            : null;
        var next = attr.Page < numberOfPages
            ? CreatePagingLink(searchparams.s, searchparams.stype, attr.Page + 1, attr.PageSize)
            : null;

        return new
        {
            totalResults,
            numberOfPages,
            prev,
            next,
            items = posts.Select(CreateSearchResultDto)
        };
    }

    private string CreatePagingLink(string s, int stype, int page, int pageSize)
    {
        return Url.Link(nameof(Search), new { s, stype, page, pageSize });
    }
}