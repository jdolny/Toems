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
    public class ProcessInventoryController : ApiController
    {
        private readonly ServiceProcessInventory _processInventoryService;

        public ProcessInventoryController()
        {
            _processInventoryService = new ServiceProcessInventory();
        }

        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public EntityProcessInventory Get(int id)
        {
            var result = _processInventoryService.GetProcess(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public IEnumerable<EntityProcessInventory> Get()
        {
            return _processInventoryService.Search(new DtoSearchFilter());
        }

        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        [HttpPost]
        public IEnumerable<EntityProcessInventory> Search(DtoSearchFilter filter)
        {
            return _processInventoryService.Search(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _processInventoryService.TotalCount()};
        }

      

    
    }
}