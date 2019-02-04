using System;
using System.Collections.Generic;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class GroupPolicyAPI : BaseAPI<EntityGroupPolicy>
    {

        public GroupPolicyAPI(string resource) : base(resource)
        {

        }

        public DtoActionResult PostList(List<EntityGroupPolicy> listOfGroupPolicies)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/PostList/", Resource);
            Request.AddJsonBody(listOfGroupPolicies);
            var response = new ApiRequest().Execute<DtoActionResult>(Request);
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

        public new List<EntityGroupPolicy> Search(int limit, string searchstring)
        {
            throw new NotImplementedException();
        }
    }
}