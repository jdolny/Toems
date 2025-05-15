using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class BrowserTokenAPI(string resource, ApiRequest apiRequest)
        : BaseAPI<EntityAttachment>(resource, apiRequest)
    {
        public async Task<EntityBrowserToken> GetToken()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetToken/";
            return await _apiRequest.ExecuteAsync<EntityBrowserToken>(Request);
        }

        

        
    }
}