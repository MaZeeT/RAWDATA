using AutoMapper;
using DatabaseService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebService.Models;

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

        [HttpGet(Name = nameof(GetCategories))]
        public ActionResult GetCategories([FromQuery] PagingAttributes pagingAttributes)
        {
            var categories = _dataService.Getquestions(pagingAttributes);

            var result = CreateResult(categories, pagingAttributes);

            return Ok(result);
        }

        [HttpGet("{categoryId}", Name = nameof(GetCategory))]
        public ActionResult GetCategory(int categoryId)
        {
            var category = _dataService.GetCategory(categoryId);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(CreateCategoryDto(category));
        }

               

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

        private CategoryDto CreateCategoryDto(Questions category)
        {
            var dto = _mapper.Map<CategoryDto>(category);
            dto.Link = Url.Link(
                    nameof(GetCategory),
                    new { categoryId = category.Id });
            return dto;
        }

        private object CreateResult(IEnumerable<Questions> categories, PagingAttributes attr)
        {
            var totalItems = _dataService.NumberOfCategories();
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
                items = categories.Select(CreateCategoryDto)
            };
        }

        private string CreatePagingLink(int page, int pageSize)
        {
            return Url.Link(nameof(GetCategories), new { page, pageSize });
        }

    }
}
