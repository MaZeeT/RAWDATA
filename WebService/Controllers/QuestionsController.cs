using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using DomainServices.Interfaces;
using WebService.DTOs;

namespace WebService.Controllers;

[ApiController]
[Route("api/questions")]
[Authorize]
public class QuestionsController : SharedController
{
    private readonly IThreadService _threadService;
    private readonly IAnnotationService _annotationService;
    private readonly IHistoryService _historyService;

    public QuestionsController(
        IThreadService threadService,
        IAnnotationService annotationService,
        IHistoryService historyService)
    {
        _threadService = threadService;
        _historyService = historyService;
        _annotationService = annotationService;
    }

    [HttpGet(Name = nameof(BrowseQuestions))]
    // examples http://localhost:5001/api/questions
    // http://localhost:5001/api/questions?page=10&pageSize=5
    // for browsing all the questions; with links to the thread
    public ActionResult BrowseQuestions([FromQuery] PagingAttributes pagingAttributes)
    {
        var categories = _threadService.GetQuestions(pagingAttributes);
        var result = CreateResult(categories, pagingAttributes);
        return Ok(result);
    }

    [HttpGet("thread/{questionId:int}/{postId:int?}", Name = nameof(GetThread))]
    //example http://localhost:5001/api/questions/thread/19
    //get the whole thread of question+answers
    public ActionResult GetThread(int questionId, int? postId)
    {
        var (userId, userIdOk) = GetAuthUserId();

        var checkThatPost = _threadService.GetPostType(questionId);
        if (checkThatPost == "answers")
        {
            questionId = _threadService.GetPost(questionId).QuestionId;
            if (postId != null)
            {
                postId = questionId;
            }
        }
        else if (checkThatPost == null)
        {
            return NotFound();
        }

        var t = _threadService.GetThread(questionId);
        if (t == null || !userIdOk) // then we got a thread!
        {
            return NotFound();
        }
        
        // call to add browse history here
        var browseHistory = new History
        {
            UserId = userId
        };
        
        if (postId != null)
        {
            browseHistory.PostId = (int)postId;
        }
        else
        {
            browseHistory.PostId = questionId;
        }

        var result = _historyService.Add(browseHistory);
        if (!result)
        {
            throw new ArgumentException("Could not add question");
        }

        // create thread dto
        var thread = new List<PostsThreadDto>();
        foreach (var posts in t)
        {
            var pt = new PostsThreadDto
            {
                Id = posts.Id,
                ParentId = posts.ParentId,
                Title = posts.Title,
                Body = posts.Body
            };
            var pagingAttributes = new PagingAttributes();
            var tempAnnotations = _annotationService.GetUserAnnotationsMadeOnAPost(userId, posts.Id, pagingAttributes);
            pt.Annotations = tempAnnotations;
            pt.CreateBookmarkLink = Url.Link(nameof(BookmarkController.AddBookmark), new { postId = posts.Id });
            var anno = new AnnotationsDto
            {
                Body = "form_or_similar_would_be_here_to_POST_a_new_annotation",
                PostId = posts.Id
            };
            pt.CreateAnnotationLink = Url.Link(nameof(AnnotationsController.AddAnnotation), anno);
            // i know its supposed to be a form/post. just thought it'd be neat to have a link mockup. 
            thread.Add(pt);
        }

        return Ok(thread);
        
    }

    ///////////////////
    //
    // Helpers
    //
    //////////////////////

    private QuestionDto CreateQuestionDto(Questions question)
    {
        var dto = new QuestionDto
        {
            Link = Url.Link(
                nameof(GetThread),
                new { questionId = question.Id }),
            Id = question.Id,
            Title = question.Title,
            Body = question.Body
        };
        return dto;
    }

    private object CreateResult(IEnumerable<Questions> questions, PagingAttributes attr)
    {
        var totalItems = _threadService.NumberOfQuestions();
        var numberOfPages = Math.Ceiling((double)totalItems / attr.PageSize);

        var prev = attr.Page > 1
            ? CreatePagingLink(attr.Page - 1, attr.PageSize)
            : null;
        var next = attr.Page < numberOfPages
            ? CreatePagingLink(attr.Page + 1, attr.PageSize)
            : null;

        return new
        {
            totalItems,
            numberOfPages,
            prev,
            next,
            items = questions.Select(CreateQuestionDto)
        };
    }

    private string CreatePagingLink(int page, int pageSize)
    {
        return Url.Link(nameof(BrowseQuestions), new { page, pageSize });
    }
}