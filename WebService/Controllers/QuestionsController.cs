using DatabaseService;
using DatabaseService.Modules;
using DatabaseService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/questions")]
    [Authorize]
    public class QuestionsController : SharedController
    {
        private readonly ISearchDataService _dataService;
        private readonly ISharedService _sharedService;
        private readonly IAnnotationService _annotationService;
        private readonly IHistoryService _historyService;

        public QuestionsController(
            ISearchDataService dataService,
            ISharedService sharedService,
            IAnnotationService annotationService,
            IHistoryService historyService)
        {
            _dataService = dataService;
            _sharedService = sharedService;
            _historyService = historyService;
            _annotationService = annotationService;
        }

        [HttpGet(Name = nameof(BrowseQuestions))]
        //examples http://localhost:5001/api/questions
        // http://localhost:5001/api/questions?page=10&pageSize=5
        // for browsing all the questions; with links to the thread
        public ActionResult BrowseQuestions([FromQuery] PagingAttributes pagingAttributes)
        {
            var categories = _dataService.GetQuestions(pagingAttributes);
            var result = CreateResult(categories, pagingAttributes);
            return Ok(result);
        }

        [HttpGet("thread/{questionId}/{postId?}", Name = nameof(GetThread))]
        //example http://localhost:5001/api/questions/thread/19
        //get the whole thread of question+asnswers
        public ActionResult GetThread(int questionId, int? postId)
        {
            (int userId, bool useridok) = GetAuthUserId();

            var checkthatpost = _sharedService.GetPostType(questionId);
            if (checkthatpost == "answers")
            {
                questionId = _sharedService.GetPost(questionId).QuestionId;
                if (postId != null)
                {
                    postId = questionId;
                }
            }
            else if (checkthatpost == null) 
            { 
                return NotFound(); 
            }


            var t = _sharedService.GetThread(questionId);
            if (t != null && useridok) // then we got a thread!
            {
                ///call to add browse history here
                History browsehist = new History();
                browsehist.Userid = userId;
                if (postId != null)
                {
                    browsehist.Postid = (int)postId;
                }
                else browsehist.Postid = questionId;
                _historyService.Add(browsehist);

                //createthreaddto
                List<PostsThreadDto> thread = new List<PostsThreadDto>();
                foreach (Posts p in t)
                {
                    PostsThreadDto pt = new PostsThreadDto();
                    pt.Id = p.Id;
                    pt.Parentid = p.Parentid;
                    pt.Title = p.Title;
                    pt.Body = p.Body;
                    PagingAttributes pagingAttributes = new PagingAttributes();
                    List<SimpleAnnotationDto> tempanno = new List<SimpleAnnotationDto>();
                        tempanno = _annotationService.GetUserAnnotationsMadeOnAPost(userId, p.Id, pagingAttributes);
                    pt.Annotations = tempanno;
                    pt.createBookmarkLink = Url.Link(  nameof(BookmarkController.AddBookmark),  new { postId = p.Id });
                    AnnotationsDto anno = new AnnotationsDto();
                    anno.Body = "form/similar would be here to POST a new annotation";
                    anno.PostId = p.Id;
                    pt.createAnnotationLink = Url.Link(nameof(AnnotationsController.AddAnnotation), anno);
                    // i know its supposed to be a form/post. just thought it'd be neat to have a link mockup. oh well maybe its more confusing this way :(
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
            var dto = new QuestionDto();
            dto.Link = Url.Link(
                    nameof(GetThread),
                    new { questionId = question.Id });
            dto.Id = question.Id;
            dto.Title = question.Title;
            dto.Body = question.Body;
            return dto;
        }

        private object CreateResult(IEnumerable<Questions> questions, PagingAttributes attr)
        {
            var totalItems = _sharedService.NumberOfQuestions();
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
}
