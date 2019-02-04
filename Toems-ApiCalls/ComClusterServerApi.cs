using System.Collections.Generic;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class ComClusterServerAPI : BaseAPI<EntityComServerClusterServer>
    {
        public ComClusterServerAPI(string resource) : base(resource)
        {

        }

        public DtoActionResult Post(List<EntityComServerClusterServer> listOfServers)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/Post/", Resource);
            Request.AddJsonBody(listOfServers);
            return new ApiRequest().Execute<DtoActionResult>(Request);
        }
    }
}