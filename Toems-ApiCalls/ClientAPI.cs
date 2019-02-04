using System;
using System.Security.Cryptography.X509Certificates;
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
            Request.Method = Method.GET;
            Request.Resource = "toec/Push/GetLoggedInUsers";
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMAC<DtoApiStringResponse>(Request, cert);
            return responseData != null ? responseData.Value : string.Empty;
        }

        public bool GetStatus(string url, X509Certificate2 cert)
        {
            Request.Method = Method.GET;
            Request.Resource = "toec/Push/GetStatus";
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMAC<DtoApiBoolResponse>(Request, cert);
            return responseData != null && responseData.Value;
        }

        public void SendWolTask(string url, X509Certificate2 cert, DtoWolTask wolTask)
        {
            Request.Method = Method.POST;
            Request.AddJsonBody(wolTask);
            Request.Resource = "toec/Push/WolTask";
            new ApiRequest(new Uri(url)).ExecuteHMACAsync<DtoApiBoolResponse>(Request, cert);
        }

        public void Reboot(string url, X509Certificate2 cert, string delay)
        {
            Request.Method = Method.POST;
            Request.AddJsonBody(new DtoApiStringResponse() { Value = delay });
            Request.Resource = "toec/Push/Reboot";
            new ApiRequest(new Uri(url)).ExecuteHMACAsync<DtoApiBoolResponse>(Request, cert);
        }

        public void Shutdown(string url, X509Certificate2 cert,string delay)
        {
            Request.Method = Method.POST;
            Request.AddJsonBody(new DtoApiStringResponse() {Value = delay});
            Request.Resource = "toec/Push/Shutdown";
            new ApiRequest(new Uri(url)).ExecuteHMACAsync<DtoApiBoolResponse>(Request, cert);

        }

        public void SendMessage(string url, X509Certificate2 cert,DtoMessage message)
        {
            Request.Method = Method.POST;
            Request.Resource = "toec/Push/Message";      
            Request.AddJsonBody(message);
            new ApiRequest(new Uri(url)).ExecuteHMACAsync<DtoApiBoolResponse>(Request,cert);

        }

        public void ForceCheckin(string url, X509Certificate2 cert)
        {
            Request.Method = Method.GET;
            Request.Resource = "toec/Push/Checkin";
            new ApiRequest(new Uri(url)).ExecuteHMACAsync<DtoApiBoolResponse>(Request, cert);
        }

        public void RunInventory(string url, X509Certificate2 cert)
        {
            Request.Method = Method.GET;
            Request.Resource = "toec/Push/Inventory";
            new ApiRequest(new Uri(url)).ExecuteHMACAsync<DtoApiBoolResponse>(Request, cert);
        }

     
    }
}