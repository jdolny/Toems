using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class CategoryAPI(string resource, ApiRequest apiRequest)
        : BaseAPI<EntityCategory>(resource, apiRequest);
}