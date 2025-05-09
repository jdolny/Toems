using System;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;

namespace Toems_ApiCalls
{
    public class ClientAPI
    {
        protected readonly RestRequest Request;
        protected readonly string Resource;

        public ClientAPI(string resource)
        {
            Request = new RestRequest();
            Resource = resource;
        }

        public string GetLoggedInUsers(string url, X509Certificate2 cert)
        {
            Request.Method = Method.Get;
            Request.Resource = "toec/Push/GetLoggedInUsers";
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMAC<DtoApiStringResponse>(Request, cert);
            return responseData != null ? responseData.Value : string.Empty;
        }

        public bool GetStatus(string url, X509Certificate2 cert)
        {
            Request.Method = Method.Get;
            Request.Resource = "toec/Push/GetStatus";
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMAC<DtoApiBoolResponse>(Request, cert);
            return responseData != null && responseData.Value;
        }

        public void SendWolTask(string url, X509Certificate2 cert, DtoWolTask wolTask)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(wolTask), ParameterType.RequestBody);
            Request.Resource = "toec/Push/WolTask";
#pragma warning disable CS4014
            new ApiRequest(new Uri(url)).ExecuteHMACAsync<DtoApiBoolResponse>(Request, cert);
#pragma warning restore CS4014
        }

        public void Reboot(string url, X509Certificate2 cert, string delay)
        {
            Request.Method = Method.Post;

            Request.AddParameter("application/json", JsonConvert.SerializeObject(new DtoApiStringResponse() { Value = delay }), ParameterType.RequestBody);
            Request.Resource = "toec/Push/Reboot";
#pragma warning disable CS4014
            new ApiRequest(new Uri(url)).ExecuteHMACAsync<DtoApiBoolResponse>(Request, cert);
#pragma warning restore CS4014
        }

        public void Shutdown(string url, X509Certificate2 cert,string delay)
        {
            Request.Method = Method.Post;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(new DtoApiStringResponse() { Value = delay }), ParameterType.RequestBody);
            Request.Resource = "toec/Push/Shutdown";
#pragma warning disable CS4014
            new ApiRequest(new Uri(url)).ExecuteHMACAsync<DtoApiBoolResponse>(Request, cert);
#pragma warning restore CS4014

        }

        public void SendMessage(string url, X509Certificate2 cert,DtoMessage message)
        {
            Request.Method = Method.Post;
            Request.Resource = "toec/Push/Message";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(message), ParameterType.RequestBody);
#pragma warning disable CS4014
            new ApiRequest(new Uri(url)).ExecuteHMACAsync<DtoApiBoolResponse>(Request,cert);
#pragma warning restore CS4014

        }

        public void ForceCheckin(string url, X509Certificate2 cert)
        {
            Request.Method = Method.Get;
            Request.Resource = "toec/Push/Checkin";
#pragma warning disable CS4014
            new ApiRequest(new Uri(url)).ExecuteHMACAsync<DtoApiBoolResponse>(Request, cert);
#pragma warning restore CS4014
        }

        public void RunInventory(string url, X509Certificate2 cert)
        {
            Request.Method = Method.Get;
            Request.Resource = "toec/Push/Inventory";
#pragma warning disable CS4014
            new ApiRequest(new Uri(url)).ExecuteHMACAsync<DtoApiBoolResponse>(Request, cert);
#pragma warning restore CS4014
        }


    }
}