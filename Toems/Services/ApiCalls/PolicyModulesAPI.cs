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
    public class PolicyModulesAPI : BaseAPI<EntityPolicyModules>
    {

        public PolicyModulesAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public DtoActionResult PostList(List<EntityPolicyModules> listOfPolicyModules)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/PostList/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(listOfPolicyModules), ParameterType.RequestBody);
            var response = _apiRequest.Execute<DtoActionResult>(Request);
            if (response == null)
            {
                return new DtoActionResult()
                {
                    ErrorMessage = "Unknown Exception.  Check The Exception Logs For More Info.",
                    Success = false
                };
            }
        
            return response;
        }

        public new List<EntityPolicyModules> Get()
        {
            throw new NotImplementedException();
        }

        public new string GetCount()
        {
            throw new NotImplementedException();
        }

        public List<EntityPolicyModules> Search(int limit, string searchstring)
        {
            throw new NotImplementedException();
        }
    }
}