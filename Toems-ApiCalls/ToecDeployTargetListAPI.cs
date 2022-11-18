using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto.exports;
using Toems_Common.Dto;
using Toems_Common.Entity;
using System.Collections.Generic;

namespace Toems_ApiCalls
{
    public class ToecDeployTargetListAPI : BaseAPI<EntityToecTargetList>
    {
        public ToecDeployTargetListAPI(string resource) : base(resource)
        {


        }

        public List<EntityToecTargetListComputer> GetMembers(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetMembers/{1}", Resource, id);
            return new ApiRequest().Execute<List<EntityToecTargetListComputer>>(Request);
        }
    }
}