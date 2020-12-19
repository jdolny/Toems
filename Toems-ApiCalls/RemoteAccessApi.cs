using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Dto;
using Toems_Common.Entity.Remotely;

namespace Toems_ApiCalls
{
    public class RemoteAccessApi
    {
        private readonly RestRequest _request;
        private readonly string _resource;

        public RemoteAccessApi(string resource)
        {
            _request = new RestRequest();
            _resource = resource;
        }

        public bool VerifyRemoteAccessInstalled(int id)
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/VerifyRemoteAccessInstalled/{1}", _resource, id);
            var response = new ApiRequest().Execute<DtoApiBoolResponse>(_request);
            return response != null && response.Value;
        }

        public DtoActionResult InitializeRemotelyServer(int id)
        {
            _request.Method = Method.GET;
            _request.Resource = string.Format("{0}/InitializeRemotelyServer/{1}", _resource, id);
            return new ApiRequest().Execute<DtoActionResult>(_request);

        }

        public string CreateRemotelyFirstUser(string remotelyUrl, RemotelyUser user)
        {
            _request.Method = Method.POST;
            _request.AddJsonBody(user);
            _request.Resource = "api/Theopenem/CreateFirstUser";
            return new ApiRequest(new Uri(remotelyUrl)).ExecuteRemotely(_request);
        }




    }
}
