using System.Collections.Generic;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class CustomComputerAttributeController : ApiController
    {
        private readonly ServiceComputerCustomAttributes _serviceComputerCustomAttributes;

        public CustomComputerAttributeController()
        {
           _serviceComputerCustomAttributes = new ServiceComputerCustomAttributes();
          
        }

        [CustomAuth(Permission = AuthorizationStrings.ComputerUpdate)]
        public DtoActionResult Post(List<EntityCustomComputerAttribute> attributes)
        {
            return _serviceComputerCustomAttributes.AddOrUpdate(attributes);
        }
    }
}