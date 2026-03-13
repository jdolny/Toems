using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Dto;

namespace Toems_ApiCalls
{
    public class AuthorizationAPI : BaseAPI<DtoPlaceHolder>
    {


        public AuthorizationAPI(string resource, ProtectedSessionStorage protectedSessionStorage) : base(resource, protectedSessionStorage)
        {

        }

        public bool IsAuthorized(string requiredRight)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/IsAuthorized/", Resource);
            Request.AddParameter("requiredRight", requiredRight);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }
    }
}