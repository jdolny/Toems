using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common.Dto;
using Toems_ServiceCore.Infrastructure;

namespace Toems_ClientApi.Controllers
{
    [ApiController]
    [Route("api/client/[controller]")]
    public class RemoteAccessController(ServiceContext ctx) : ControllerBase
    {

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse VerifyRemoteAccessInstalled()
        {
            return new DtoApiBoolResponse() { Value = ctx.Filessystem.FileExists(string.Format(@"{0}\Theopenem\Remotely\Remotely_Server.exe",
                            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles))) };
        }


    }
}