/*using AutoMapper;
using DatabaseService;
using Microsoft.AspNetCore.Mvc;

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


        [HttpGet("{searchstring}")]
        public ActionResult Search(string searchstring)
        {
            var search = _dataService.Search(searchstring);

            //var result = CreateResult(categories);

            return Ok(search);
        }


        ///////////////////
        //
        // Helpers
        //
        //////////////////////



    }
}
*/