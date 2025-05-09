using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class PinnedPolicyAPI : BaseAPI<EntityPinnedPolicy>
    {
        public PinnedPolicyAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public DtoActionResult Delete(int policyId, int userId)
        {
            Request.Method = Method.Delete;
            Request.Resource = string.Format("{0}/Delete/", Resource);
            Request.AddParameter("policyId", policyId);
            Request.AddParameter("userId", userId);
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