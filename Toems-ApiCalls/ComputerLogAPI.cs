using System.Collections.Generic;
using RestSharp;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ComputerLogAPI : BaseAPI<EntityComputerLog>
    {
        public ComputerLogAPI(string resource) : base(resource)
        {

        }

        public IEnumerable<EntityComputerLog> GetUnregLogs(int limit = 0)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetUnregLogs/", Resource);
            Request.AddParameter("limit", limit);
            var result = new ApiRequest().Execute<List<EntityComputerLog>>(Request);
            if (result == null)
                return new List<EntityComputerLog>();
            else
                return result;
        }


    }
}