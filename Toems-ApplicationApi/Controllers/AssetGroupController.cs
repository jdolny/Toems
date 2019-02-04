using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Service.Entity;

namespace Toems_ApplicationApi.Controllers
{
    public class AssetGroupController : ApiController
    {
        private readonly ServiceAssetGroup _serviceAssetGroup;
        private readonly int _userId;

        public AssetGroupController()
        {
           _serviceAssetGroup = new ServiceAssetGroup();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());

        }

        [CustomAuth(Permission = AuthorizationStrings.AssetRead)]
        [HttpPost]
        public IEnumerable<EntityAssetGroup> Search(DtoSearchFilter filter)
        {
            return _serviceAssetGroup.Search(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetDelete)]
        public DtoActionResult Delete(int id)
        {
            var result = _serviceAssetGroup.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetRead)]
        public EntityAssetGroup Get(int id)
        {
            var result = _serviceAssetGroup.GetAssetGroup(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [CustomAuth(Permission = AuthorizationStrings.AssetRead)]
        public IEnumerable<EntityAssetGroup> Get()
        {
            return _serviceAssetGroup.GetAll();
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _serviceAssetGroup.TotalCount()};
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetUpdate)]
        public DtoActionResult Post(EntityAssetGroup assetGroup)
        {
            return _serviceAssetGroup.Add(assetGroup);
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetUpdate)]
        public DtoActionResult Put(int id, EntityAssetGroup assetGroup)
        {
            assetGroup.Id = id;
            var result = _serviceAssetGroup.Update(assetGroup);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.AssetUpdate)]
        public DtoApiBoolResponse RemoveGroupMember(int id, int assetId)
        {
            return new DtoApiBoolResponse { Value = new ServiceAssetGroupMember().DeleteByIds(assetId,id) };
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetRead)]
        public IEnumerable<DtoAssetWithType> GetGroupMembers(int id)
        {
            return _serviceAssetGroup.GetGroupMembers(id);
        }



    }
}