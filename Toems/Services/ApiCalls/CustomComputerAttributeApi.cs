using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class CustomComputerAttributeAPI(string resource, ApiRequest apiRequest)
        : BaseAPI<EntityCustomComputerAttribute>(resource,apiRequest)
    {
        public async Task<DtoActionResult> Post(List<EntityCustomComputerAttribute> attributes)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/Post/";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(attributes), ParameterType.RequestBody);
            return await _apiRequest.ExecuteAsync<DtoActionResult>(Request);
        }
    }
}