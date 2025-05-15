using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebService.Attributes;

public class RequireAuthenticationAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        
        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        var userClaim = user.FindFirst(ClaimTypes.Name);
        if (userClaim == null || !int.TryParse(userClaim.Value, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        context.HttpContext.Items["UserId"] = userId;
        await next();
    }
}