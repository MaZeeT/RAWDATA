using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Domain.Enums;
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
    public ActionResult WordRank([FromQuery] SearchQuery searchQuery, [FromQuery] int? maxResults)
    {
        var (userId, userIdOk) = GetAuthUserId();

        Console.WriteLine("Got user: " + userId);

        if (searchQuery.SearchTerms == null || !userIdOk)
        {
            return BadRequest();
        }
            
        Console.WriteLine("Got searchparams: " + searchQuery.SearchTerms);
        Console.WriteLine("Got maxresults: " + maxResults);

        switch (searchQuery.SearchType)
        {
            //checking of params
            case SearchType.Tfidf or SearchType.ExactMatch or SearchType.Simple or SearchType.BestMatch:
                //wrong search type, redirect
                return RedirectToAction("Search", new { s = searchQuery.SearchTerms, stype = searchQuery.SearchType });
            case SearchType.WordsTfidf or SearchType.WordsBest:
            {
                var search = _searchService.WordRank(userId, searchQuery.SearchTerms, searchQuery.SearchType, maxResults);
                return Ok(search);
            }
            default:
            {
                var search = _searchService.WordRank(userId, searchQuery.SearchTerms, SearchType.WordsBest, maxResults);
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

        if (searchparams.SearchTerms == null || !userIdOk)
        {
            return BadRequest();
        }
        
        Console.WriteLine("Got searchparams: " + searchparams.SearchTerms);

        switch (searchparams.SearchType)
        {
            //checking of params
            case SearchType.Tfidf or SearchType.ExactMatch or SearchType.Simple or SearchType.BestMatch:
            {
                //do search, fix page also if needed as a bonus
                var search = _searchService.Search(userId, searchparams.SearchTerms, searchparams.SearchType, pagingAttributes);

                // try to fix searchsting for link generation if it seems useable but ugly
                searchparams.SearchTerms = _searchService.BuildSearchString(searchparams.SearchTerms, true);

                var result = CreateResult(search, searchparams, pagingAttributes);
                
                if (result is null)
                {
                    return NoContent();
                }
                
                return Ok(result);
            }
            case SearchType.WordsTfidf or SearchType.WordsBest:
                //wrong search type, redirect
                return RedirectToAction("WordRank", new { s = searchparams.SearchTerms, stype = searchparams.SearchType });
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
        if (posts.FirstOrDefault() == null)
        {
            return null;
        }
         
        var totalResults = posts[0].TotalResults;
        var numberOfPages = Math.Ceiling((double)totalResults / attr.PageSize);

        var prev = attr.Page > 1
            ? CreatePagingLink(searchparams.SearchTerms, searchparams.SearchType, attr.Page - 1, attr.PageSize)
            : null;
        var next = attr.Page < numberOfPages
            ? CreatePagingLink(searchparams.SearchTerms, searchparams.SearchType, attr.Page + 1, attr.PageSize)
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

    private string CreatePagingLink(string s, SearchType searchType, int page, int pageSize)
    {
        return Url.Link(nameof(Search), new { s, searchType, page, pageSize });
    }
}