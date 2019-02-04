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
    public class WolRelayController : ApiController
    {
        private readonly ServiceWolRelay _wolRelayService;

        public WolRelayController()
        {
            _wolRelayService = new ServiceWolRelay();
          
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Delete(int id)
        {
            var result = _wolRelayService.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public EntityWolRelay Get(int id)
        {
            var result = _wolRelayService.GetWolRelay(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public IEnumerable<EntityWolRelay> Get()
        {
            return _wolRelayService.Search(new DtoSearchFilter());
        }

         [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        [HttpPost]
        public IEnumerable<EntityWolRelay> Search(DtoSearchFilter filter)
        {
            return _wolRelayService.Search(filter);
        }

         [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _wolRelayService.TotalCount()};
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Post(EntityWolRelay relay)
        {
            return _wolRelayService.Add(relay);
        }

        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoActionResult Put(int id, EntityWolRelay relay)
        {
            relay.Id = id;
            var result = _wolRelayService.Update(relay);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }
    }
}