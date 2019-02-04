using System.Net;
using System.Net.Http;
using System.Web.Http;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class VersionController : ApiController
    {
        private readonly ServiceVersion _versionServices;

        public VersionController()
        {
            _versionServices = new ServiceVersion();
        }

        [Authorize]
        public EntityVersion Get(int id)
        {
            var result = _versionServices.Get(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [HttpGet]
        [Authorize]
        public DtoApiBoolResponse IsFirstRunCompleted()
        {
            return new DtoApiBoolResponse {Value = _versionServices.FirstRunCompleted()};
        }

        [Authorize]
        public DtoActionResult Put(int id, EntityVersion version)
        {
            version.Id = id;
            var result = _versionServices.Update(version);
            if (result.Id == 0)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, result));
          
            return result;
        }
    }
}