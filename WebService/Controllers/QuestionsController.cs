using AutoMapper;
using DatabaseService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
//using WebService.Models;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/questions")]
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
        public ActionResult BrowseQuestions([FromQuery] PagingAttributes pagingAttributes)
        {
            var categories = _dataService.GetQuestions(pagingAttributes);

            var result = CreateResult(categories, pagingAttributes);

            return Ok(result);
        }

       [HttpGet("{questionId}", Name = nameof(GetQuestion))]
        public ActionResult GetQuestion(int questionId)
        {
            var question = _dataService.GetQuestion(questionId);
            if (question == null)
            {
                return NotFound();
            }
            return Ok(CreateQuestionDto(question));
        }


        /*
               [HttpPost]
               public ActionResult CreateCategory(CategoryForCreation categoryDto)
               {
                   var category = _mapper.Map<Questions>(categoryDto);
                   _dataService.CreateCategory(category);
                   return CreatedAtRoute(
                       nameof(GetCategory),
                       new { categoryId = category.Id},
                       CreateCategoryDto(category));
               }

               [HttpPut("{categoryId}")]
               public ActionResult UpdateCategory(
                   int categoryId, Questions category)
               {
                   if (!_dataService.CategoryExcist(categoryId))
                   {
                       return NotFound();
                   }
                   category.Id = categoryId;
                   _dataService.UpdateCategory(category);
                   return NoContent();
               }

               [HttpDelete("{categoryId}")]
               public ActionResult DeleteCategory(int categoryId)
               {
                   if (_dataService.DeleteCategory(categoryId))
                   {
                       return NoContent();
                   }
                   return NotFound();
               }

               ///////////////////
               //
               // Helpers
               //
               //////////////////////
       */
        private QuestionDto CreateQuestionDto(Questions question)
        {

            //var dto = _mapper.Map<QuestionDto>(question);
            var dto = new QuestionDto();
            dto.Link = Url.Link(
                    nameof(GetQuestion),
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

            var prev = attr.Page > 0
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
