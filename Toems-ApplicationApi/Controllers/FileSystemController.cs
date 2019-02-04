using System.Collections.Generic;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Service;
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

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public List<string> GetLogs()
        {
            return FilesystemServices.GetLogs();
        }

         [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiStringResponse GetServerPaths(string type, string subType)
        {
            return new DtoApiStringResponse { Value = new FilesystemServices().GetServerPaths(type, subType) };
        }

         [Authorize]
         public DtoFreeSpace GetFreeSpace(bool isRemote)
         {
             return new FilesystemServices().GetStorageFreeSpace(isRemote);
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