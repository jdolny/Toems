using System.Collections.Generic;
using System.Net.Http;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class AttachmentAPI : BaseAPI<EntityAttachment>
    {
        public AttachmentAPI(string resource) : base(resource)
        {

        }

        public byte[] GetAttachment(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetAttachment/{1}", Resource, id);
            return new ApiRequest().ExecuteRaw(Request);
        }

        

        
    }
}