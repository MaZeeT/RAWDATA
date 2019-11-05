using AutoMapper;
using DatabaseService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web.Http;
//using WebService.Models;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        private IDataService _dataService;
        private IMapper _mapper;

        public SearchController(
            IDataService dataService,
            IMapper mapper)
        {
            _dataService = dataService;
            _mapper = mapper;
        }

        //[HttpGet, Route("{s=searchstring}/{stype=searchtype}")]
        [HttpGet] //dont quite understand how this part functions
        public ActionResult Search([FromQuery] SearchQuery searchparams)
        {
            if (searchparams.s != null)
            {
                Console.WriteLine("hola " + searchparams.s);


                if (searchparams.stype >= 0 && searchparams.stype <= 3 || searchparams.stype == null)
                {
                    var search = _dataService.Search(searchparams.s, searchparams.stype);
                    return Ok(search);
                }
                else if (searchparams.stype >= 4 && searchparams.stype <= 5)
                {
                    var search = _dataService.WordRank(searchparams.s, searchparams.stype);
                    return Ok(search);
                }
            }
            return BadRequest();
        }

        // [HttpGet("{searchstring}")]
        // public ActionResult Search(string searchstring)
        //  {
        //     var search = _dataService.Search(searchstring);

        //     //var result = CreateResult(categories);

        //      return Ok(search);
        // }

        public class SearchQuery
        {
            public string s { get; set; }
            public int? stype { get; set; }
        }


        ///////////////////
        //
        // Helpers
        //
        //////////////////////



    }
}
