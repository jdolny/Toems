using System.Collections.Generic;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Service;
using Toems_Service.Entity;
using Toems_Service.Workflows;

namespace Toems_ApplicationApi.Controllers
{
    public class FileSystemController : ApiController
    {
      
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public List<string> GetLogContents(string name, int limit)
        {
            return new FilesystemServices().GetLogContents(name, limit);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public List<string> GetComServerLogContents(string name, int limit, int comServerId)
        {
            return new FilesystemServices().GetComServerLogContents(name, limit, comServerId);
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public List<string> GetLogs()
        {
            return FilesystemServices.GetLogs();
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public List<string> GetComServerLogs(int id)
        {
            return FilesystemServices.GetComServerLogs(id);
        }
      
        [HttpGet]
        [Authorize]
        public List<string> GetKernels()
        {
            return new GetKernels().Run();
        }

        [HttpGet]
        [Authorize]
        public List<string> GetBootImages()
        {
            return new GetBootImages().Run();
        }


         [Authorize]
         public DtoFreeSpace GetSMBFreeSpace()
         {
             return new FilesystemServices().GetSMBFreeSpace();
         }

        [Authorize]
        public List<DtoFreeSpace> GetComFreeSpace()
        {
            return new ComServerFreeSpace().RunAllServers();
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleUpdate)]
        [HttpGet]
        public DtoApiBoolResponse SyncStorageServers()
        {
            var response = new FolderSync().RunAllServers();
            return new DtoApiBoolResponse() {Value = response};
        }

      


    }
}