using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Toems_Common;
using Toems_Service;
using Toems_Service.Entity;

namespace Toems_ApplicationApiCore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthenticationServices auth, IConfiguration config) : ControllerBase
{
    //private readonly AuthenticationServices _auth = service.AuthenticationServices;


    [HttpPost("token")]
    public IActionResult Token([FromForm] string username, [FromForm] string password, [FromForm] string verification_code)
    {
        var validationResult = auth.GlobalLogin(username, password, "Web", verification_code);

        if (!validationResult.Success)
            return Unauthorized(new { error = validationResult.ErrorMessage });

        //var user = service..GetUser(context.UserName);
        //oAuthIdentity.AddClaim(new Claim("user_id", user.Id.ToString()));
        var claims = new List<Claim>
        {
            new Claim("user_id", "123"), // replace with your user.Id
        };

        if (validationResult.ErrorMessage == "Mfa setup is required")
            claims.Add(new Claim("mfa_setup_required", "true"));

        //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var webTimeout = ServiceSetting.GetSettingValue(SettingStrings.WebUiTimeout);
        var expireMinutes = 60; // default to 60 minutes
        if(!string.IsNullOrEmpty(webTimeout))
        {
            bool result = int.TryParse(webTimeout, out expireMinutes);
            if (result)
            {
                if (expireMinutes == 0) expireMinutes = 99999;
            }
        }
        

        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            claims: claims);
        //signingCredentials: creds);

        return Ok(new
        {
            access_token = new JwtSecurityTokenHandler().WriteToken(token),
            expires_in = expireMinutes * 60
        });
    }
}