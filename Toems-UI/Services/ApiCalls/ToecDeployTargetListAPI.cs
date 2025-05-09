using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto.exports;
using Toems_Common.Dto;
using Toems_Common.Entity;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;

namespace Toems_ApiCalls
{
    public class ToecDeployTargetListAPI : BaseAPI<EntityToecTargetList>
    {
        public ToecDeployTargetListAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {


        }

        public List<EntityToecTargetListComputer> GetMembers(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetMembers/{1}", Resource, id);
            return _apiRequest.Execute<List<EntityToecTargetListComputer>>(Request);
        }
    }
}