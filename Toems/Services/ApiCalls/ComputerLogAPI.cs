using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ComputerLogAPI (string resource, ApiRequest apiRequest)
        : BaseAPI<EntityComputerLog>(resource,apiRequest)
    {

        public async Task<IEnumerable<EntityComputerLog>> GetUnregLogs(int limit = 0)
        {
            Request.Method = Method.Get;
            Request.Resource = $"{Resource}/GetUnregLogs/";
            Request.AddParameter("limit", limit);
            var result = await _apiRequest.ExecuteAsync<List<EntityComputerLog>>(Request);
            return result ?? new List<EntityComputerLog>();
        }


    }
}