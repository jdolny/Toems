using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Dto;

namespace Toems_ApiCalls
{
    public class OnlineKernelAPI : BaseAPI<DtoOnlineKernel>
    {
        private readonly ApiRequest _apiRequest;

        public OnlineKernelAPI(string resource) : base(resource)
        {
            _apiRequest = new ApiRequest();
        }

        public bool DownloadKernel(DtoOnlineKernel onlineKernel)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/DownloadKernel/", Resource);
            Request.AddJsonBody(onlineKernel);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        new public List<DtoOnlineKernel> Get()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/Get", Resource);

            var result = _apiRequest.Execute<List<DtoOnlineKernel>>(Request);
            if (result == null)
                return new List<DtoOnlineKernel>();
            else
                return result;
        }
    }
}
