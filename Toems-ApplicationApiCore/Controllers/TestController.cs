using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Toems_ApplicationApiCore;
using Toems_Common;
using Toems_ServiceCore.Data;
using Toems_ServiceCore.Infrastructure;

[ApiController]
[Route("api/[controller]")]
public class TestController(ServiceContext ictx, IToemsDbFactory toemsDbFactory) : ControllerBase
{
    private readonly AuthenticationService _auth;
    private readonly IConfiguration _config;
    

    [HttpGet("VerifyDb")]
    public async Task<string> VerifyDb()
    {
        await using var sparcDb = await toemsDbFactory.CreateDbContextAsync();
        var comp = sparcDb.Computers.ToList();
        return comp.FirstOrDefault()?.Name;

        return "60";
    }
    
}