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
        private IAppUserService _appUserService;
        private IMapper _mapper;

        public AppUserController(IAppUserService appUserService, IMapper mapper)
        {
            _appUserService = appUserService;
            _mapper = mapper;
        }

        [HttpGet, Route("{id=}")]
        // http://localhost:5001/api/appuser?id=2
        public ActionResult GetAppUser([FromQuery] int id)
        {
            //todo Need to query db to check if user exist instead of this hack
            Console.WriteLine("input: " + id);
            try
            {
                var appUser = _appUserService.GetAppUserName(id);
                Console.WriteLine("ds: " + appUser);
                return Ok(appUser);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}