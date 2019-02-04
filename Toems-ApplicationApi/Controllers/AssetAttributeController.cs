using System.Collections.Generic;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class AssetAttributeController : ApiController
    {
        private readonly ServiceAssetAttributes _serviceAssetAttributes;

        public AssetAttributeController()
        {
          _serviceAssetAttributes = new ServiceAssetAttributes();
          
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetUpdate)]
        public DtoActionResult Post(List<EntityAssetAttribute> attributes)
        {
            return _serviceAssetAttributes.AddOrUpdate(attributes);
        }
    }
}