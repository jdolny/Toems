using System.Threading;
using System.Web.Http;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common.Dto;
using Toems_Service.Workflows;

namespace Toems_ClientApi.Controllers
{

    public class WolController : ApiController
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
