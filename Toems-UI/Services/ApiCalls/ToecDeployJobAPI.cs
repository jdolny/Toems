using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class ToecDeployJobAPI : BaseAPI<EntityToecDeployJob>
    {
        public ToecDeployJobAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }
        public List<EntityToecTargetListComputer> GetTargetComputers(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetTargetComputers/{1}", Resource,id);
            return _apiRequest.Execute<List<EntityToecTargetListComputer>>(Request);
        }

        public bool RestartDeployJobService()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/RestartDeployJobService/", Resource);
            var result = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            if (result != null)
                return result.Value;
            return false;
        }

        public bool ResetComputerStatus(int computerId)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/ResetComputerStatus/{1}", Resource,computerId);
            var result = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            if (result != null)
                return result.Value;
            return false;
        }

        public bool RunToecDeploySingle(DtoSingleToecDeploy job)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/RunToecDeploySingle/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(job), ParameterType.RequestBody);
            var result = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            if (result != null)
                return result.Value;
            return false;
        }
    }
}