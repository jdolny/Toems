using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ProcessInventoryAPI : BaseAPI<EntityProcessInventory>
    {
        public ProcessInventoryAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public List<DtoComputerProcessTime> GetProcessTimeComputer(int processId, int dayFilter)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetProcessTimeComputer", Resource);
            Request.AddParameter("processId", processId);
            Request.AddParameter("dayFilter", dayFilter);
            return _apiRequest.Execute<List<DtoComputerProcessTime>>(Request);
        }
    }
}