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
    public class CustomAttributeController : ApiController
    {
        private readonly ServiceCustomAttribute _serviceCustomAttribute;

        public CustomAttributeController()
        {
           _serviceCustomAttribute = new ServiceCustomAttribute();
          
        }

        [CustomAuth(Permission = AuthorizationStrings.CustomAttributeDelete)]
        public DtoActionResult Delete(int id)
        {
            var result = _serviceCustomAttribute.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.CustomAttributeRead)]
        public EntityCustomAttribute Get(int id)
        {
            var result = _serviceCustomAttribute.GetCustomAttribute(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [CustomAuth(Permission = AuthorizationStrings.CustomAttributeRead)]
        public IEnumerable<EntityCustomAttribute> Get()
        {
            return _serviceCustomAttribute.GetAll();
        }

        [Authorize]
        public IEnumerable<EntityCustomAttribute> GetForBuiltInComputers()
        {
            return _serviceCustomAttribute.GetForBuiltInComputers();
        }

        [Authorize]
        public IEnumerable<EntityCustomAttribute> GetForAssetType(int id)
        {
            return _serviceCustomAttribute.GetForAssetType(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.CustomAttributeRead)]
        [HttpPost]
        public IEnumerable<DtoCustomAttributeWithType> Search(DtoSearchFilter filter)
        {
            return _serviceCustomAttribute.Search(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.CustomAttributeRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _serviceCustomAttribute.TotalCount()};
        }

        [CustomAuth(Permission = AuthorizationStrings.CustomAttributeUpdate)]
        public DtoActionResult Post(EntityCustomAttribute customAttribute)
        {
            return _serviceCustomAttribute.Add(customAttribute);
        }

        [CustomAuth(Permission = AuthorizationStrings.CustomAttributeUpdate)]
        public DtoActionResult Put(int id, EntityCustomAttribute customAttribute)
        {
            customAttribute.Id = id;
            var result = _serviceCustomAttribute.Update(customAttribute);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }
    }
}