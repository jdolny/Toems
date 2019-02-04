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
    public class AssetController : ApiController
    {
        private readonly ServiceAsset _serviceAsset;
        private readonly int _userId;

        public AssetController()
        {
           _serviceAsset = new ServiceAsset();
            _userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault());

        }

        [CustomAuth(Permission = AuthorizationStrings.AssetDelete)]
        public DtoActionResult Delete(int id)
        {
            var result = _serviceAsset.Delete(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetArchive)]
        [HttpGet]
        public DtoActionResult Archive(int id)
        {
            var result = _serviceAsset.Archive(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetArchive)]
        [HttpGet]
        public DtoActionResult Restore(int id)
        {
            var result = _serviceAsset.Restore(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetRead)]
        public EntityAsset Get(int id)
        {
            var result = _serviceAsset.GetAsset(id);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }


        [CustomAuth(Permission = AuthorizationStrings.AssetRead)]
        public IEnumerable<EntityAsset> Get()
        {
            return _serviceAsset.GetAll();
        }

        [CustomAuth(Permission = AuthorizationStrings.AttachmentRead)]
        public IEnumerable<EntityAttachment> GetAttachments(int id)
        {
            return _serviceAsset.GetAttachments(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetRead)]
        public IEnumerable<EntityComputer> GetAssetSoftwareComputers(int id)
        {
            return _serviceAsset.GetAssetSoftwareComputers(id);
        }


        [CustomAuth(Permission = AuthorizationStrings.AssetRead)]
        [HttpPost]
        public IEnumerable<DtoAssetWithType> Search(DtoSearchFilterCategories filter)
        {
            return _serviceAsset.Search(filter);
        }


        [CustomAuth(Permission = AuthorizationStrings.AssetRead)]
        [HttpPost]
        public IEnumerable<DtoAssetWithType> SearchArchived(DtoSearchFilterCategories filter)
        {
            return _serviceAsset.SearchArchived(filter);
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetRead)]
        public DtoApiStringResponse GetCount()
        {
            return new DtoApiStringResponse {Value = _serviceAsset.TotalCount()};
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetRead)]
        public DtoApiStringResponse GetArchivedCount()
        {
            return new DtoApiStringResponse { Value = _serviceAsset.GetArchivedCount() };
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetUpdate)]
        public DtoActionResult Post(EntityAsset asset)
        {
            return _serviceAsset.Add(asset);
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetUpdate)]
        public DtoActionResult Put(int id, EntityAsset asset)
        {
            asset.Id = id;
            var result = _serviceAsset.Update(asset);
            if (result == null) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            return result;
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetRead)]
        public IEnumerable<EntityAssetAttribute> GetAttributes(int id)
        {
            return _serviceAsset.GetAttributes(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.CommentAdd)]
        [HttpPost]
        public DtoActionResult AddComment(DtoAssetComment comment)
        {
            return _serviceAsset.AddComment(comment, _userId);
        }

        [CustomAuth(Permission = AuthorizationStrings.CommentRead)]
        public List<DtoAssetComment> GetComments(int id)
        {
            return _serviceAsset.GetComments(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetRead)]
        public List<DtoAssetSoftware> GetSoftware(int id)
        {
            return _serviceAsset.GetAssetSoftware(id);
        }

        [CustomAuth(Permission = AuthorizationStrings.AssetRead)]
        public IEnumerable<EntityAssetCategory> GetAssetCategories(int id)
        {
            return _serviceAsset.GetAssetCategories(id);
        }


    }
}