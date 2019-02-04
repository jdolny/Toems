using System;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    /// <summary>
    ///     Summary description for User
    /// </summary>
    public class ClientComServerAPI : BaseAPI<EntityClientComServer>
    {
        public ClientComServerAPI(string resource) : base(resource)
        {
           
        }


        public bool SyncStorage(string url, string serverName, string interComKey)
        {
            Request.Method = Method.POST;
            Request.Resource = "Storage/Sync";
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public bool WakeupComputers(string url, string serverName, string interComKey, DtoWolTask task)
        {
            Request.Method = Method.POST;
            Request.Resource = "Wol/Wakeup";
            Request.AddJsonBody(task);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public string GetVersion(string url)
        {
            Request.Method = Method.GET;
            Request.Resource = "Provision/GetToecApiVersion";
            var responseData = new ApiRequest(new Uri(url)).Execute<DtoApiStringResponse>(Request);
            if (responseData == null)
                return "";
            else
                return responseData.Value;
        }


    }
}