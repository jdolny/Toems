using System.Collections.Generic;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class ResetRequestAPI : BaseAPI<EntityResetRequest>
    {

        public ResetRequestAPI(string resource) : base(resource)
        {
            
        }

        public DtoActionResult Approve(int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/Approve/{1}", Resource, id);
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

       
    }
}