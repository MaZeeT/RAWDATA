using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using Domain.Services;
using DomainServices.Interfaces;
using WebService.DTOs;

namespace WebService.Controllers;

[ApiController]
[Route("api/search")]
[Authorize]
// when accessing with tokens, the header needs a key Authorization with a value of Bearer [space] and then the token (no quotes)
public class SearchController : SharedController
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("wordrank", Name = nameof(WordRank))]
    // http://localhost:5001/api/search/wordrank?s=code&stype=5&maxresults=5
    // http://localhost:5001/api/search/wordrank?s=code,app,program
    public ActionResult WordRank([FromQuery] SearchQuery searchparams, [FromQuery] int? maxresults)
    {
        var (userId, userIdOk) = GetAuthUserId();

        Console.WriteLine("Got user: " + userId);

        if (searchparams.s == null || !userIdOk)
        {
            return BadRequest();
        }
            
        Console.WriteLine("Got searchparams: " + searchparams.s);
        Console.WriteLine("Got maxresults: " + maxresults);

        switch (searchparams.stype)
        {
            //checking of params
            case >= 0 and <= 3:
                //wrong search type, redirect
                return RedirectToAction("Search", new { searchparams.s, searchparams.stype });
            case >= 4 and <= 5:
            {
                var search = _searchService.WordRank(userId, searchparams.s, searchparams.stype, maxresults);
                return Ok(search);
            }
            default:
            {
                var search = _searchService.WordRank(userId, searchparams.s, 5, maxresults);
                return Ok(search);
            }
        }

    }

    [HttpGet(Name = nameof(Search))]
    //examples
    // http://localhost:5001/api/search?s=code&stype=0&page=10&pageSize=5
    // http://localhost:5001/api/search?s=code,app,program
    public ActionResult Search([FromQuery] SearchQuery searchparams, [FromQuery] PagingAttributes pagingAttributes)
    {
        var (userId, userIdOk) = GetAuthUserId();

        Console.WriteLine("Got user: " + userId);

        if (searchparams.s == null || !userIdOk)
        {
            return BadRequest();
        }
        
        Console.WriteLine("Got searchparams: " + searchparams.s);

        switch (searchparams.stype)
        {
            //checking of params
            case >= 0 and <= 3:
            {
                //do search, fix page also if needed as a bonus
                var search = _searchService.Search(userId, searchparams.s, searchparams.stype, pagingAttributes);

                // try to fix searchsting for link generation if it seems useable but ugly
                searchparams.s = _searchService.BuildSearchString(searchparams.s, true);

                var result = CreateResult(search, searchparams, pagingAttributes);
                
                if (result is null)
                {
                    return NoContent();
                }
                
                return Ok(result);
            }
            case >= 4 and <= 5:
                //wrong search type, redirect
                return RedirectToAction("WordRank", new { searchparams.s, searchparams.stype });
            default:
                return BadRequest();
        }
    }


    ///////////////////
    //
    // Helpers
    //
    //////////////////////

    private PostsSearchListDto CreateSearchResultDto(Posts posts)
    {
        return posts.ParentId == 0
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
                    questionId = posts.ParentId,
                    postId = posts.Id
                })
        };
    }


    private object CreateResult(IList<Posts> posts, SearchQuery searchparams, PagingAttributes attr)
    { 
        if (posts.FirstOrDefault() != null)
        {
            return null;
        }
         
        var totalResults = posts[0].TotalResults;
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