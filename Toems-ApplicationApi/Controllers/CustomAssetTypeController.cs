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
    public class CustomAssetTypeController : ApiController
    {
        private readonly ServiceCustomAssetType _serviceCustomAssetType;

        public CustomAssetTypeController()
        {
           _serviceCustomAssetType = new ServiceCustomAssetType();
          
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetTypeDelete)]
        public DtoActionResult Delete(int id)
        {
            var result = _serviceCustomAssetType.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetTypeRead)]
        public EntityCustomAssetType Get(int id)
        {
            var result = _serviceCustomAssetType.GetCustomAssetType(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [Authorize]
        public IEnumerable<EntityCustomAssetType> Get()
        {
            return _serviceCustomAssetType.GetAll();
        }

         [CustomAuth(Permission = AuthorizationStrings.AssetTypeRead)]
        [HttpPost]
        public IEnumerable<EntityCustomAssetType> Search(DtoSearchFilter filter)
        {
            return _serviceCustomAssetType.Search(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetTypeRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _serviceCustomAssetType.TotalCount()};
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetTypeUpdate)]
        public DtoActionResult Post(EntityCustomAssetType customAssetType)
        {
            return _serviceCustomAssetType.Add(customAssetType);
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetTypeUpdate)]
        public DtoActionResult Put(int id, EntityCustomAssetType customAssetType)
        {
            customAssetType.Id = id;
            var result = _serviceCustomAssetType.Update(customAssetType);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }
    }
}