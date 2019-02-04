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
    public class AssetCategoryController : ApiController
    {
        private readonly ServiceAssetCategory _serviceAssetCategory;

        public AssetCategoryController()
        {
            _serviceAssetCategory = new ServiceAssetCategory();
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetUpdate)]
        public DtoActionResult Post(List<EntityAssetCategory> assetCategories)
        {
            return _serviceAssetCategory.AddOrUpdate(assetCategories);
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetUpdate)]
        public DtoActionResult Delete(int id)
        {
            var result = _serviceAssetCategory.DeleteAllForAsset(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


    }
}