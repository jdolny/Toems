using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class AssetAPI : BaseAPI<EntityAsset>
    {
        public AssetAPI(string resource, ProtectedSessionStorage protectedSessionStorage) : base(resource, protectedSessionStorage)
        {

        }

        public DtoActionResult Restore(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Restore/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoActionResult>(Request);
            if (response != null)
            {
                if (response.Id == 0)
                    response.Success = false;
            }
            else
            {
                return new DtoActionResult()
                {
                    ErrorMessage = "Unknown Exception.  Check The Exception Logs For More Info.",
                    Success = false
                };
            }
            return response;
        }

        public DtoActionResult Archive(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Archive/{1}", Resource, id);
            var response = _apiRequest.Execute<DtoActionResult>(Request);
            if (response != null)
            {
                if (response.Id == 0)
                    response.Success = false;
            }
            else
            {
                return new DtoActionResult()
                {
                    ErrorMessage = "Unknown Exception.  Check The Exception Logs For More Info.",
                    Success = false
                };
            }
            return response;
        }

        public IEnumerable<EntityAssetAttribute> GetAttributes(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetAttributes/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityAssetAttribute>>(Request);
        }

        new public IEnumerable<DtoAssetWithType> Search(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/Search", Resource);
            return _apiRequest.Execute<List<DtoAssetWithType>>(Request);
        }

        public string GetArchivedCount()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetArchivedCount", Resource);
            var responseData = _apiRequest.Execute<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public IEnumerable<DtoAssetWithType> SearchArchived(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/SearchArchived", Resource);
            return _apiRequest.Execute<List<DtoAssetWithType>>(Request);
        }

        public IEnumerable<EntityAttachment> GetAttachments(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetAttachments/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityAttachment>>(Request);
        }

        public IEnumerable<EntityComputer> GetAssetSoftwareComputers(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetAssetSoftwareComputers/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityComputer>>(Request);
        }

        public IEnumerable<DtoAssetComment> GetComments(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetComments/{1}", Resource, id);
            return _apiRequest.Execute<List<DtoAssetComment>>(Request);
        }

        public IEnumerable<DtoAssetSoftware> GetSoftware(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetSoftware/{1}", Resource, id);
            return _apiRequest.Execute<List<DtoAssetSoftware>>(Request);
        }

        public DtoActionResult AddComment(DtoAssetComment comment)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/AddComment/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(comment), ParameterType.RequestBody);
            return _apiRequest.Execute<DtoActionResult>(Request);
        }

        public IEnumerable<EntityAssetCategory> GetAssetCategories(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetAssetCategories/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityAssetCategory>>(Request);
        }
    }
}