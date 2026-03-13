using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class ApprovalRequestAPI : BaseAPI<EntityApprovalRequest>
    {

        public ApprovalRequestAPI(string resource, ProtectedSessionStorage protectedSessionStorage) : base(resource, protectedSessionStorage)
        {
            
        }

        public DtoActionResult Approve(int id)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Approve/{1}", Resource, id);
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