using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class ExternalDownloadController : ApiController
    {
        private readonly ServiceExternalDownload _downloadService;

        public ExternalDownloadController()
        {
         _downloadService = new ServiceExternalDownload();
          
        }

         [CustomAuth(Permission = AuthorizationStrings.ModuleUpdate)]
        public DtoActionResult Delete(int id)
        {
            var result = _downloadService.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public EntityExternalDownload Get(int id)
        {
            var result = _downloadService.GetDownload(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public IEnumerable<EntityExternalDownload> GetForModule(string moduleGuid)
        {
            return _downloadService.GetForModule(moduleGuid);
        }


        [CustomAuth(Permission = AuthorizationStrings.ModuleExternalFiles)]
        [HttpPost]
        public async Task BatchDownload(List<DtoFileDownload> downloads)
        {
            await _downloadService.BatchDownload(downloads);
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleExternalFiles)]
        [HttpPost]
        public void DownloadFile(DtoFileDownload download)
        {
            _downloadService.DownloadFile(download);
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _downloadService.TotalCount()};
        }


        [CustomAuth(Permission = AuthorizationStrings.ModuleUpdate)]
        public DtoActionResult Post(EntityExternalDownload download)
        {
            return _downloadService.Add(download);
        }

        [CustomAuth(Permission = AuthorizationStrings.ModuleUpdate)]
        public DtoActionResult Put(int id, EntityExternalDownload download)
        {
            download.Id = id;
            var result = _downloadService.Update(download);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }
    }
}