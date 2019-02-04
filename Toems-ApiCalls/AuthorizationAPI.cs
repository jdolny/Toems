using RestSharp;
using Toems_Common.Dto;

namespace Toems_ApiCalls
{
    public class AuthorizationAPI
    {
        private readonly RestRequest _request;
        private readonly string _resource;

        public AuthorizationAPI(string resource)
        {
            _request = new RestRequest();
            _resource = resource;
        }

        public bool IsAuthorized(string requiredRight)
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/IsAuthorized/", _resource);
            _request.AddParameter("requiredRight", requiredRight);
            var response = new ApiRequest().Execute<DtoApiBoolResponse>(_request);
            return response != null && response.Value;
        }
    }
}