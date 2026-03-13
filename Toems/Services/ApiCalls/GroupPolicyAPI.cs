using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class GroupPolicyAPI : BaseAPI<EntityGroupPolicy>
    {

        public GroupPolicyAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public DtoActionResult PostList(List<EntityGroupPolicy> listOfGroupPolicies)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/PostList/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(listOfGroupPolicies), ParameterType.RequestBody);
            var response = _apiRequest.Execute<DtoActionResult>(Request);
            if (response != null)
            {
                if (response.Id == 0)
                    response.Success = false;
            }
            else
            {
                return new DtoActionResult()
                {
                    ErrorMessage = "Unknown Exception.  Check The Exception Logs For More Info.",
                    Success = false
                };
            }
            return response;
        }

        public new List<EntityGroupPolicy> Get()
        {
            throw new NotImplementedException();
        }

        public new string GetCount()
        {
            throw new NotImplementedException();
        }

        public List<EntityGroupPolicy> Search(int limit, string searchstring)
        {
            throw new NotImplementedException();
        }
    }
}