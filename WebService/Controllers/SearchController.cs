using AutoMapper;
using DatabaseService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web.Http;
//using WebService.Models;

namespace webservice.controllers
{
    [apicontroller]
    [route("api/search")]
    public class searchcontroller : controllerbase
    {
        private idataservice _dataservice;
        private imapper _mapper;

        public searchcontroller(
            idataservice dataservice,
            imapper mapper)
        {
            _dataservice = dataservice;
            _mapper = mapper;
        }

        [HttpGet, Route("{s=}/{stype=3}/{page=0}/{pageSize=10}")]
        //[HttpGet] put defalut values here for optional parameters. in this case only s is not optional
        public ActionResult Search([FromQuery] SearchQuery searchparams, [FromQuery] PagingAttributes pagingAttributes)
        {
            if (searchparams.s != null)
            {
                Console.WriteLine("Got searchparams: " + searchparams.s);

                //rudimentary checking of params
                if (searchparams.stype >= 0 && searchparams.stype <= 3 || searchparams.stype == null)
                {
                    return Ok(search);
                    var search = _dataService.Search(searchparams.s, searchparams.stype, pagingAttributes);
                }
                else if (searchparams.stype >= 4 && searchparams.stype <= 5)
                    return Ok(search);
                    var search = _dataService.WordRank(searchparams.s, searchparams.stype, pagingAttributes);
                {
                }
            }
            return BadRequest();
        }




        ///////////////////
        //
        // helpers
        //
        //////////////////////
/*
        private QuestionDto CreateCategoryDto(Questions category)
        {
            var dto = _mapper.Map<QuestionDto>(category);
            dto.Link = Url.Link(
                    nameof(GetQuestion),
                    new { categoryId = category.Id });
            return dto;
        }

        private object CreateResult(IEnumerable<Questions> categories, PagingAttributes attr)
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
                items = categories.Select(CreateCategoryDto)
            };
        }
        private string CreatePagingLink(int page, int pageSize)
        {
            return Url.Link(nameof(GetCategories), new { page, pageSize });
        } 
        */
    }
}
