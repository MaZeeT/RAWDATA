using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.AnnotationsDTOs;
using Domain.Models;
using Domain.Services;
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
    //examples http://localhost:5001/api/questions
    // http://localhost:5001/api/questions?page=10&pageSize=5
    // for browsing all the questions; with links to the thread
    public ActionResult BrowseQuestions([FromQuery] PagingAttributes pagingAttributes)
    {
        var categories = _threadService.GetQuestions(pagingAttributes);
        var result = CreateResult(categories, pagingAttributes);
        return Ok(result);
    }

    [HttpGet("thread/{questionId}/{postId?}", Name = nameof(GetThread))]
    //example http://localhost:5001/api/questions/thread/19
    //get the whole thread of question+asnswers
    public ActionResult GetThread(int questionId, int? postId)
    {
        (int userId, bool useridok) = GetAuthUserId();

        var checkthatpost = _threadService.GetPostType(questionId);
        if (checkthatpost == "answers")
        {
            questionId = _threadService.GetPost(questionId).QuestionId;
            if (postId != null)
            {
                postId = questionId;
            }
        }
        else if (checkthatpost == null)
        {
            return NotFound();
        }

        var t = _threadService.GetThread(questionId);
        if (t != null && useridok) // then we got a thread!
        {
            ///call to add browse history here
            History browsehist = new History
            {
                UserId = userId
            };
            if (postId != null)
            {
                browsehist.PostId = (int)postId;
            }
            else browsehist.PostId = questionId;

            var result = _historyService.Add(browsehist);
            if (!result)
            {
                throw new Exception("Could not add question");
            }

            //createthreaddto
            List<PostsThreadDto> thread = new List<PostsThreadDto>();
            foreach (Posts p in t)
            {
                PostsThreadDto pt = new PostsThreadDto
                {
                    Id = p.Id,
                    Parentid = p.ParentId,
                    Title = p.Title,
                    Body = p.Body
                };
                PagingAttributes pagingAttributes = new PagingAttributes();
                List<SimpleAnnotationDto> tempanno = new List<SimpleAnnotationDto>();
                tempanno = _annotationService.GetUserAnnotationsMadeOnAPost(userId, p.Id, pagingAttributes);
                pt.Annotations = tempanno;
                pt.createBookmarkLink = Url.Link(nameof(BookmarkController.AddBookmark), new { postId = p.Id });
                AnnotationsDto anno = new AnnotationsDto
                {
                    Body = "form_or_similar_would_be_here_to_POST_a_new_annotation",
                    PostId = p.Id
                };
                pt.createAnnotationLink = Url.Link(nameof(AnnotationsController.AddAnnotation), anno);
                // i know its supposed to be a form/post. just thought it'd be neat to have a link mockup. 
                thread.Add(pt);
            }

            return Ok(thread);
        }
        else return NotFound();
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