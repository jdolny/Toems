using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class ComServerClusterAPI : BaseAPI<EntityComServerCluster>
    {
        public ComServerClusterAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public IEnumerable<EntityComServerClusterServer> GetClustServers(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetClusterServers/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityComServerClusterServer>>(Request);
        }
    }
}