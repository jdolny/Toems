using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Toems_ApplicationApiCore;
using Toems_Common;
using Toems_Service;
using Toems_Service.Entity;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly AuthenticationServices _auth;
    private readonly IConfiguration _config;
    

    [HttpGet("VerifyDb")]
    public string VerifyDb()
    {
        return "60";
    }
    
}