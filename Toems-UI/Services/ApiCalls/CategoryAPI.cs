using System.Collections.Generic;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class CategoryAPI : BaseAPI<EntityCategory>
    {
        public CategoryAPI(string resource, ILocalStorageService protectedSessionStorage) : base(resource, protectedSessionStorage)
        {

        }

      
    }
}