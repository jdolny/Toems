using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class AttachmentAPI : BaseAPI<EntityAttachment>
    {
        public AttachmentAPI(string resource, ProtectedSessionStorage protectedSessionStorage) : base(resource, protectedSessionStorage)
        {

        }

        public byte[] GetAttachment(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetAttachment/{1}", Resource, id);
            return _apiRequest.ExecuteRaw(Request);
        }

        

        
    }
}