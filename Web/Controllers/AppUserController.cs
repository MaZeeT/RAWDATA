using Microsoft.AspNetCore.Mvc;
using System;
using Application.UseCases.Users.GetUser;

namespace Web.Controllers;

[ApiController]
[Route("api/appuser")]
public class AppUserController : ControllerBase
{
    private readonly IGetUser _getUser;

    public AppUserController(IGetUser getUser)
    {
        _getUser = getUser;
    }

    // http://localhost:5001/api/appuser?id=2
    [HttpGet, Route("{id=}")]
    public ActionResult GetAppUser([FromQuery] int id)
    {
        //todo Need to query db to check if user exist instead of this hack
        try
        {
            var query = new GetUserQuery { UserId = id };
            var user = _getUser.Execute(query);
            return Ok(user);
        }
        catch (Exception)
        {
            return NotFound("User not found");
        }
    }
}