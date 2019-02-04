using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class UploadedFileController : ApiController
    {
        private readonly ServiceUploadedFile _uploadedFileServices;

        public UploadedFileController()
        {
            _uploadedFileServices = new ServiceUploadedFile();
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleUpdate)]
        public DtoActionResult Delete(int id)
        {
            var result = _uploadedFileServices.DeleteFile(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public EntityUploadedFile Get(int id)
        {
            var result = _uploadedFileServices.GetFile(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public IEnumerable<EntityUploadedFile> Get()
        {
            return _uploadedFileServices.SearchFiles(new DtoSearchFilter());
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        [HttpPost]
        public IEnumerable<EntityUploadedFile> Search(DtoSearchFilter filter)
        {
            return _uploadedFileServices.SearchFiles(filter);
        }

       [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse { Value = _uploadedFileServices.TotalCount() };
        }

       [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
       public IEnumerable<EntityUploadedFile> GetModuleFiles(string moduleGuid)
       {
           return _uploadedFileServices.GetFilesForModule(moduleGuid);
       }

       
    }
}