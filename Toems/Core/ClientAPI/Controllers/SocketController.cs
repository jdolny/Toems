using Microsoft.AspNetCore.Mvc;
using Toems_ClientApi.Controllers.Authorization;
using Toems_ClientApi.Hubs;
using Toems_Common.Dto;


namespace Toems_ClientApi.Controllers
{
    [ApiController]
    [Route("api/client/[controller]")]
    public class SocketController : ControllerBase
    {
        private readonly ActionHubService _hubService;

        public SocketController(ActionHubService hubService)
        {
            _hubService = hubService;
        }

        [HttpPost]
        [InterComAuth]
        public async Task<IActionResult> Send(DtoSocketRequest request)
        {
            await _hubService.SendAction(request);
            return Ok();
        }
    }
}
