using Microsoft.AspNetCore.Identity;

namespace Toems_ServiceCore.Data;

public class AppUser : IdentityUser
{
    public string? DisplayName { get; set; }
}