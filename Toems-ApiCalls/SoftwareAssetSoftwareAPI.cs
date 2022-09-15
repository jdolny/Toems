using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class SoftwareAssetSoftwareAPI : BaseAPI<EntitySoftwareAssetSoftware>
    {
        public SoftwareAssetSoftwareAPI(string resource) : base(resource)
        {

        }

        public DtoActionResult Post(List<EntitySoftwareAssetSoftware> software)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/Post/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(software), ParameterType.RequestBody);
            return new ApiRequest().Execute<DtoActionResult>(Request);
        }
    }
}