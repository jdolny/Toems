using Blazored.LocalStorage;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class BrowserTokenAPI : BaseAPI<EntityAttachment>
    {
        public BrowserTokenAPI(string resource, ILocalStorageService protectedSessionStorage) : base(resource, protectedSessionStorage)
        {

        }

        public async Task<EntityBrowserToken> GetToken()
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetToken/";
            return await _apiRequest.ExecuteAsync<EntityBrowserToken>(Request);
        }

        

        
    }
}