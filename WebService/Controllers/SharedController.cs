using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    public abstract class SharedController : ControllerBase
    {
        protected (int,bool) GetAuthUserId()
        {
            bool useridok = false;
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            if (Int32.TryParse(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value, out int userId))
            {
                useridok = true; //becomes true when we get an int in userId
            }
            return (userId,useridok);
        }

    }
}
