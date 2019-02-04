using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class VersionAPI : BaseAPI<EntityVersion>
    {
        private readonly ApiRequest _apiRequest;

        public VersionAPI(string resource) : base(resource)
        {
            _apiRequest = new ApiRequest();
        }

        public bool IsFirstRunCompleted()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/IsFirstRunCompleted", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

    }
}