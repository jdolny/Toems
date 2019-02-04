using System.Web.Http;
using log4net;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common.Dto;
using Toems_Service.Workflows;

namespace Toems_ClientApi.Controllers
{
    public class InventoryController : ApiController
    {
        private static readonly ILog Logger =
         LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [CertificateAuth]
        [HttpPost]
        public DtoApiBoolResponse SubmitInventory(DtoInventoryCollection collection)
        {
            var clientId = RequestContext.Principal.Identity.Name;
            var result = new SubmitInventory().Run(collection,clientId);
            return new DtoApiBoolResponse() {Value = result};

        }
    
    }
}