using Microsoft.AspNetCore.Mvc;
using System;
using Application.Interfaces.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/appuser")]
public class AppUserController : ControllerBase
{
    private readonly IUserService _userService;

    public AppUserController(IUserService userService)
    {
        _userService = userService;
    }

    // http://localhost:5001/api/appuser?id=2
    [HttpGet, Route("{id=}")]
    public ActionResult GetAppUser([FromQuery] int id)
    {
        //todo Need to query db to check if user exist instead of this hack
        try
        {
            var appUser = _userService.GetUserName(id);
            return Ok(appUser);
        }
        catch (Exception)
        {
            return NotFound("User not found");
        }
    }
}