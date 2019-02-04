using System.Collections.Generic;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class SoftwareAssetSoftwareController : ApiController
    {
        private readonly ServiceSoftwareAssetSoftware _serviceSoftwareAssetSoftware;

        public SoftwareAssetSoftwareController()
        {
          _serviceSoftwareAssetSoftware = new ServiceSoftwareAssetSoftware();
          
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetUpdate)]
        public DtoActionResult Post(List<EntitySoftwareAssetSoftware> software)
        {
            return _serviceSoftwareAssetSoftware.AddOrUpdate(software);
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetUpdate)]
        public DtoActionResult Delete(int id)
        {
            return _serviceSoftwareAssetSoftware.Delete(id);
        }
    }
}