using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ImpersonationAccountAPI : BaseAPI<EntityImpersonationAccount>
    {
        public ImpersonationAccountAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public List<EntityImpersonationAccount> GetForDropDown()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetForDropDown", Resource);
            return _apiRequest.Execute<List<EntityImpersonationAccount>>(Request);
        }
    }
}