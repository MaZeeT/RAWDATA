using AutoMapper;
using DatabaseService.Modules;
using DatabaseService.Services;
using Microsoft.AspNetCore.Mvc;
using System;

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

        [HttpGet, Route("{id=}")]
       // http://localhost:5001/api/appuser?id=2
       //routing stuff is annoying to debug
        public ActionResult GetAppUser([FromQuery] int id)
        {
            Console.WriteLine("input: " + id);

            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var appUser = _appUsersService.GetAppUser(id);
                Console.WriteLine("ds: " + appUser);
                return Ok(appUser);    
            }
            
        }
        /*
        [HttpGet("{appUserName}", Name = nameof(GetAppUser))]
        public ActionResult GetAppUser(string appUserName)
        {
            var appUser = _appUsersService.GetAppUser(appUserName);
            if (appUser == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(appUser);    
            }
            
        }
*/

    }
}