using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public abstract class SharedController : ControllerBase
{
    protected (int, bool) GetAuthUserId()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;

        return int.TryParse(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value, out var userId) 
            ? (userId, true) 
            : (userId, false);
    }
}