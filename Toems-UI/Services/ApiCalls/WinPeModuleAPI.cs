using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class WinPeModuleAPI(string resource, ApiRequest apiRequest)
        : BaseAPI<EntityWinPeModule>(resource,apiRequest)
    {

        public async Task<string> GetArchivedCount()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetArchivedCount";
            var responseData = await _apiRequest.ExecuteAsync<DtoApiStringResponse>(Request);
            return responseData != null ? responseData.Value : string.Empty;

        }

        public async Task<List<EntityWinPeModule>> GetArchived(DtoSearchFilterCategories filter)
        {
            Request.Method = Method.Post;
            Request.Resource = $"{Resource}/GetArchived";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(filter), ParameterType.RequestBody);
            return await _apiRequest.ExecuteAsync<List<EntityWinPeModule>>(Request);
        }
      
       
    }
}