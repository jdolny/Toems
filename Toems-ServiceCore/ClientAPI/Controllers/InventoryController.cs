using log4net;
using Microsoft.AspNetCore.Mvc;
using Toems_ClientApi.Controllers.Authorization;
using Toems_Common.Dto;
using Toems_ServiceCore.Infrastructure;
using Toems_ServiceCore.Workflows;

namespace Toems_ClientApi.Controllers
{
    [ApiController]
    [Route("api/client/[controller]")]
    public class InventoryController(ServiceContext ctx) : ControllerBase
    {
        [CertificateAuth]
        [HttpPost]
        public DtoApiBoolResponse SubmitInventory(DtoInventoryCollection collection)
        {
            var clientId = HttpContext.User.Identity?.Name;
            var result = ctx.SubmitInventory.Run(collection,clientId);
            return new DtoApiBoolResponse() {Value = result};
        }
    }
}