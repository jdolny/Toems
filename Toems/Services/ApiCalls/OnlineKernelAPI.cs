using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Toems_Common.Dto;

namespace Toems_ApiCalls
{
    public class OnlineKernelAPI : BaseAPI<DtoOnlineKernel>
    {


        public OnlineKernelAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public bool DownloadKernel(DtoOnlineKernel onlineKernel)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/DownloadKernel/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(onlineKernel), ParameterType.RequestBody);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        new public List<DtoOnlineKernel> Get()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/Get", Resource);

            var result = _apiRequest.Execute<List<DtoOnlineKernel>>(Request);
            if (result == null)
                return new List<DtoOnlineKernel>();
            else
                return result;
        }
    }
}
