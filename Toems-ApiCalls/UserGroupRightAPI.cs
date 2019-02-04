using System.Collections.Generic;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class UserGroupRightAPI : BaseAPI<EntityUserGroupRight>
    {
        private readonly ApiRequest _apiRequest;

        public UserGroupRightAPI(string resource) : base(resource)
        {
            _apiRequest = new ApiRequest();
        }

        public DtoActionResult Post(List<EntityUserGroupRight> listOfRights)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/Post/", Resource);
            Request.AddJsonBody(listOfRights);
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