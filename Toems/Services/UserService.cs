using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Toems_ServiceCore.Data;

namespace Toems_UI.Services;

public class CurrentUserService(IHttpContextAccessor context, UserManager<AppUser> userManager)
{
    public string? UserId => context.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public int UserIdInt => int.Parse(UserId);
    public async Task<AppUser?> GetUser()
    {
        var principal = context.HttpContext?.User;

        if (principal == null)
            return null;

        return await userManager.GetUserAsync(principal);
    }
}