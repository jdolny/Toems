using System.Collections.Generic;
using System.Web.Http;
using log4net;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common.Dto;
using Toems_Service;
using Toems_Service.Workflows;

namespace Toems_ClientApi.Controllers
{

    public class StorageController : ApiController
    {
        private static readonly ILog Logger =
         LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

       [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse Sync()
        {
            return new DtoApiBoolResponse(){Value = new FolderSync().Sync()};
        }

        [InterComAuth]
        [HttpPost]
        public DtoFreeSpace GetFreeSpace()
        {
            return new Toems_Service.Workflows.ComServerFreeSpace().GetFreeSpace();
        }

        [InterComAuth]
        [HttpPost]
        public List<DtoReplicationProcess> GetReplicationProcesses()
        {
            return new FilesystemServices().GetRunningSyncProcess();
        }

        [InterComAuth]
        [HttpPost]
        public DtoApiBoolResponse KillProcess(int id)
        {
            return new DtoApiBoolResponse() { Value = new FilesystemServices().KillRoboProcess(id) };
        }


    }

    
}
