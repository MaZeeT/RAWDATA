using System.Security.Claims;
using Application.Common;

namespace Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Result<int> GetUserId(this ClaimsPrincipal user)
    {
        var claimsIdentity = user.Identity as ClaimsIdentity;
        
        return int.TryParse(claimsIdentity?.FindFirst(ClaimTypes.Name)?.Value, out var userId)
            ? Result<int>.Success(userId) 
            : Result<int>.Failure("User is not authenticated");
    }
}