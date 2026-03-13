using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common.Dto;
using Toems_ServiceCore.Infrastructure;
using Toems_ServiceCore.Workflows;

namespace Toems_ClientApi.Controllers
{
    [ApiController]
    [Route("api/client/[controller]")]
    public class WolController(ServiceContext ctx) : ControllerBase
    {
        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse Wakeup(DtoWolTask wolTask )
        {
            var t = new Thread(() => RunWakeup(wolTask));
            t.Start();
            return new DtoApiBoolResponse() { Value = true };
        }

        private void RunWakeup(DtoWolTask wolTask)
        {
            WolRelayTask.WakeUp(wolTask);
        }
    }

    
}
