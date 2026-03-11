using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toems_ServiceCore.Data;
using Toems_UI.Services.ControllerService;

namespace Toems_UI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController(DebugService debugService) : ControllerBase
{
    [HttpGet("VerifyDb")]
    public async Task<string?> VerifyDb()
    {
        return await debugService.VerifyDb();
    }
    
}