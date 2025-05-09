using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class CustomBootMenuAPI : BaseAPI<EntityCustomBootMenu>
    {
        public CustomBootMenuAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

      
    }
}