using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class AssetAttributeAPI : BaseAPI<EntityAssetAttribute>
    {
        public AssetAttributeAPI(string resource, ProtectedSessionStorage protectedSessionStorage) : base(resource, protectedSessionStorage)
        {

        }

        public DtoActionResult Post(List<EntityAssetAttribute> attributes)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/Post/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(attributes), ParameterType.RequestBody);
            return _apiRequest.Execute<DtoActionResult>(Request);
        }
    }
}