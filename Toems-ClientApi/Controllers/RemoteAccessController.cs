using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common.Dto;

namespace Toems_ClientApi.Controllers
{
    public class RemoteAccessController : ApiController
    {

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse VerifyRemoteAccessInstalled()
        {
            return new DtoApiBoolResponse() { Value = new Toems_Service.FilesystemServices().FileExists(string.Format(@"{0}\Theopenem\Remotely\Remotely_Server.exe",
                            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles))) };
        }


    }
}