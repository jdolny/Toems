using System.Collections.Generic;
using System.Web.Http;
using log4net;
using Toems_ClientApi.Controllers.Authorization;
using Toems_ClientApi.Hubs;
using Toems_Common.Dto;
using Toems_Service.Workflows;

namespace Toems_ClientApi.Controllers
{

    public class SocketController : ApiController
    {
        private static readonly ILog Logger =
         LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [InterComAuth]
        [HttpPost]
        public void SendAction(DtoSocketRequest socketRequest)
        {
            new ActionHub().SendAction(socketRequest);
        }

      
    }

    
}
