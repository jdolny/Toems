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
    public class CertificateInventoryController : ApiController
    {
        private readonly ServiceCertificateInventory _certificateInventoryService;

        public CertificateInventoryController()
        {
            _certificateInventoryService = new ServiceCertificateInventory();
        }

        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public EntityCertificateInventory Get(int id)
        {
            var result = _certificateInventoryService.GetCertificate(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public IEnumerable<EntityCertificateInventory> Get()
        {
            return _certificateInventoryService.Search(new DtoSearchFilter());
        }

        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        [HttpPost]
        public IEnumerable<EntityCertificateInventory> Search(DtoSearchFilter filter)
        {
            return _certificateInventoryService.Search(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.ReportRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _certificateInventoryService.TotalCount()};
        }

    
    }
}