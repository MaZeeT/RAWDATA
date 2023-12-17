using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WebService.Controllers;

[ApiController]
[Route("api/appuser")]
public class AppUserController : ControllerBase
{
    private IUserHandler _userHandler;

    public AppUserController(IUserHandler userHandler)
    {
        _userHandler = userHandler;
    }

    // http://localhost:5001/api/appuser?id=2
    [HttpGet, Route("{id=}")]
    public ActionResult GetAppUser([FromQuery] int id)
    {
        //todo Need to query db to check if user exist instead of this hack
        try
        {
            var appUser = _userHandler.UserName(id);
            return Ok(appUser);
        }
        catch (Exception)
        {
            return NotFound("User not found");
        }
    }
}