using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class CustomAttributeAPI : BaseAPI<EntityCustomAttribute>
    {
        public CustomAttributeAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public List<EntityCustomAttribute> GetForBuiltInComputers()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetForBuiltInComputers", Resource);
            return _apiRequest.Execute<List<EntityCustomAttribute>>(Request);
        }

        public List<EntityCustomAttribute> GetForAssetType(int assetTypeId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetForAssetType/{1}", Resource,assetTypeId);
            return _apiRequest.Execute<List<EntityCustomAttribute>>(Request);
        }

        new public IEnumerable<DtoCustomAttributeWithType> Search(DtoSearchFilter filter)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            Request.Resource = string.Format("{0}/Search", Resource);
            return _apiRequest.Execute<List<DtoCustomAttributeWithType>>(Request);
        }

    }
}