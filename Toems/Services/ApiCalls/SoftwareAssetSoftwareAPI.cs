using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class SoftwareAssetSoftwareAPI : BaseAPI<EntitySoftwareAssetSoftware>
    {
        public SoftwareAssetSoftwareAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public DtoActionResult Post(List<EntitySoftwareAssetSoftware> software)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/Post/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(software), ParameterType.RequestBody);
            return _apiRequest.Execute<DtoActionResult>(Request);
        }
    }
}