using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class GroupMembershipAPI : BaseAPI<EntityGroupMembership>
    {

        public GroupMembershipAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {
            
        }

        public DtoActionResult Post(List<EntityGroupMembership> groupMemberships)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/Post/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(groupMemberships), ParameterType.RequestBody);
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
    }
}