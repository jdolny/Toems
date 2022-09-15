using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class PolicyModulesAPI : BaseAPI<EntityPolicyModules>
    {

        public PolicyModulesAPI(string resource) : base(resource)
        {

        }

        public DtoActionResult PostList(List<EntityPolicyModules> listOfPolicyModules)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/PostList/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(listOfPolicyModules), ParameterType.RequestBody);
            var response = new ApiRequest().Execute<DtoActionResult>(Request);
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

        public new List<EntityPolicyModules> Search(int limit, string searchstring)
        {
            throw new NotImplementedException();
        }
    }
}