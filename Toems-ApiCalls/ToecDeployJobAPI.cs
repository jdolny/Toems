using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class ToecDeployJobAPI : BaseAPI<EntityToecDeployJob>
    {
        public ToecDeployJobAPI(string resource) : base(resource)
        {

        }
        public List<EntityToecTargetListComputer> GetTargetComputers(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetTargetComputers/{1}", Resource,id);
            return new ApiRequest().Execute<List<EntityToecTargetListComputer>>(Request);
        }

        public bool RestartDeployJobService()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/RestartDeployJobService/", Resource);
            var result = new ApiRequest().Execute<DtoApiBoolResponse>(Request);
            if (result != null)
                return result.Value;
            return false;
        }

        public bool ResetComputerStatus(int computerId)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/ResetComputerStatus/{1}", Resource,computerId);
            var result = new ApiRequest().Execute<DtoApiBoolResponse>(Request);
            if (result != null)
                return result.Value;
            return false;
        }
    }
}