using AutoMapper;
using DatabaseService;
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
    public class QuestionsController : ControllerBase
    {
        private IDataService _dataService;
        private IMapper _mapper;

        public QuestionsController(
            IDataService dataService,
            IMapper mapper)
        {
            _dataService = dataService;
            _mapper = mapper;
        }

        [HttpGet(Name = nameof(BrowseQuestions))]
        //examples http://localhost:5001/api/questions
        // http://localhost:5001/api/questions?page=10&pageSize=5
        public ActionResult BrowseQuestions([FromQuery] PagingAttributes pagingAttributes)
        {
            var categories = _dataService.GetQuestions(pagingAttributes);

            var result = CreateResult(categories, pagingAttributes);

            return Ok(result);
        }

       [HttpGet("{questionId}", Name = nameof(GetQuestion))]
        //example http://localhost:5001/api/questions/19
        public ActionResult GetQuestion(int questionId)
        {
            var question = _dataService.GetQuestion(questionId);
            if (question == null)
            {
                return NotFound();
            }
            return Ok(CreateQuestionDto(question));
        }

        [HttpGet("thread/{questionId}", Name = nameof(GetThread))]
        //example http://localhost:5001/api/questions/thread/19
        //get the whole thread of question+asnswers
        public ActionResult GetThread(int questionId)
        {

            if (questionId > 0) //dont know proper way to do this
            {
                var t = _dataService.GetThread(questionId);
                if (t != null)
                {
                    return Ok(t);
                } else return NotFound();
            }
            else
            {
                return NotFound();
            }

        }


               ///////////////////
               //
               // Helpers
               //
               //////////////////////

        private QuestionDto CreateQuestionDto(Questions question)
        {

            //var dto = _mapper.Map<QuestionDto>(question);
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
            var totalItems = _dataService.NumberOfQuestions();
            var numberOfPages = Math.Ceiling((double)totalItems / attr.PageSize);

            var prev = attr.Page > 1
                ? CreatePagingLink(attr.Page - 1, attr.PageSize)
                : null;
            var next = attr.Page < numberOfPages - 1
                ? CreatePagingLink(attr.Page + 1, attr.PageSize)
                : null;

            return new
            {
                totalItems,
                numberOfPages,
                prev,
                next,
                items = questions.Select(CreateQuestionDto)
                //items = questions
            };
        }

        private string CreatePagingLink(int page, int pageSize)
        {
            return Url.Link(nameof(BrowseQuestions), new { page, pageSize });
        }
    }
}
