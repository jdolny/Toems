using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ComputerLogAPI : BaseAPI<EntityComputerLog>
    {
        public ComputerLogAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public IEnumerable<EntityComputerLog> GetUnregLogs(int limit = 0)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetUnregLogs/", Resource);
            Request.AddParameter("limit", limit);
            var result = _apiRequest.Execute<List<EntityComputerLog>>(Request);
            if (result == null)
                return new List<EntityComputerLog>();
            else
                return result;
        }


    }
}