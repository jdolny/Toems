using System.Collections.Generic;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class ComServerClusterAPI : BaseAPI<EntityComServerCluster>
    {
        public ComServerClusterAPI(string resource) : base(resource)
        {

        }

        public IEnumerable<EntityComServerClusterServer> GetClustServers(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetClusterServers/{1}", Resource, id);
            return new ApiRequest().Execute<List<EntityComServerClusterServer>>(Request);
        }
    }
}