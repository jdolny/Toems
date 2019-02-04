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
    public class SoftwareInventoryController : ApiController
    {
        private readonly ServiceSoftwareInventory _softwareInventoryService;

        public SoftwareInventoryController()
        {
            _softwareInventoryService = new ServiceSoftwareInventory();
        }

        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public EntitySoftwareInventory Get(int id)
        {
            var result = _softwareInventoryService.GetSoftware(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public IEnumerable<EntitySoftwareInventory> Get()
        {
            return _softwareInventoryService.Search(new DtoSearchFilter());
        }

        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        [HttpPost]
        public IEnumerable<EntitySoftwareInventory> Search(DtoSearchFilter filter)
        {
            return _softwareInventoryService.Search(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _softwareInventoryService.TotalCount()};
        }

    
    }
}