using AutoMapper;
using DatabaseService.Modules;
using DatabaseService.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/appuser")]
    public class AppUserController : ControllerBase
    {
        private IAppUsersService _appUsersService;
        private IMapper _mapper;

        public AppUserController(IAppUsersService appUsersService, IMapper mapper)
        {
            _appUsersService = appUsersService;
            _mapper = mapper;
        }

        /*
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
                    var search = _dataService.Search(searchparams.s, searchparams.stype, pagingAttributes);
                    return Ok(search);
                }
                else if (searchparams.stype >= 4 && searchparams.stype <= 5)
                {
                    var search = _dataService.WordRank(searchparams.s, searchparams.stype, pagingAttributes);
                    return Ok(search);
                }
            }

            return BadRequest();
        }
        
        
        */
        
    }
}