using System.Collections.Generic;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ProcessInventoryAPI : BaseAPI<EntityProcessInventory>
    {
        public ProcessInventoryAPI(string resource) : base(resource)
        {

        }

        public List<DtoComputerProcessTime> GetProcessTimeComputer(int processId, int dayFilter)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetProcessTimeComputer", Resource);
            Request.AddParameter("processId", processId);
            Request.AddParameter("dayFilter", dayFilter);
            return new ApiRequest().Execute<List<DtoComputerProcessTime>>(Request);
        }
    }
}