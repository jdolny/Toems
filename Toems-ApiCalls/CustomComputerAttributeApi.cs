using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class CustomComputerAttributeAPI : BaseAPI<EntityCustomComputerAttribute>
    {
        public CustomComputerAttributeAPI(string resource) : base(resource)
        {

        }

        public DtoActionResult Post(List<EntityCustomComputerAttribute> attributes)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/Post/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(attributes), ParameterType.RequestBody);
            return new ApiRequest().Execute<DtoActionResult>(Request);
        }
    }
}