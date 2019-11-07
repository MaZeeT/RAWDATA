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

        [HttpGet("{appUserId}", Name = nameof(GetAppUser))]
        public ActionResult GetAppUser(int appUserId)
        {
            var appUser = _appUsersService.GetAppUser(appUserId);
            if (appUser == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(appUser);    
            }
            
        }
        
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


    }
}