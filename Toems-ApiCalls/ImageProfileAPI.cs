using System.Collections.Generic;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ImageProfileAPI : BaseAPI<ImageProfileWithImage>
    {
        public ImageProfileAPI(string resource) : base(resource)
        {

        }

        public bool Clone(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/Clone/{1}", Resource, id);
            var response = new ApiRequest().Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }
    }
}