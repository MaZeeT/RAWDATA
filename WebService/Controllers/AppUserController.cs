using DatabaseService.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WebService.Controllers;

[ApiController]
[Route("api/appuser")]
public class AppUserController : ControllerBase
{
    private IUser _appUserService;

    public AppUserController(IUser appUserService)
    {
        _appUserService = appUserService;
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