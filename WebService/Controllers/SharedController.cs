using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    public abstract class SharedController : ControllerBase
    {
        protected (int,bool) GetAuthUserId()
        {
            bool useridok = false;
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            int userId;
            if (Int32.TryParse(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value, out userId))
            {
                useridok = true; //becomes true when we get an int in userId
            }
            return (userId,useridok);
        }

    }
}
