using System;
using System.Collections.Generic;
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

        public void SendAction(string url, string serverName, string interComKey, DtoSocketRequest socketRequest)
        {
            Request.Method = Method.POST;
            Request.Resource = "Socket/SendAction";
            Request.AddJsonBody(socketRequest);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
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

        public byte[] GenerateCert(int id)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/GenerateCert/{1}", Resource, id);
            return new ApiRequest().ExecuteRaw(Request);
        }

        public bool CopyPxeBinaries(string url, string serverName, string interComKey)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/CopyPxeBinaries";
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public bool CancelAllImagingTasks(string url, string serverName, string interComKey)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/CancelAllImagingTasks";
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public bool CleanTaskBootFiles(string url, string serverName, string interComKey, EntityComputer computer)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/CleanTaskBootFiles";
            Request.AddJsonBody(computer);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public bool TerminateMulticast(string url, string serverName, string interComKey, EntityActiveMulticastSession multicast)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/TerminateMulticast";
            Request.AddJsonBody(multicast);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public bool CreateDefaultBootMenu(string url, string serverName, string interComKey,DtoBootMenuGenOptions bootOptions)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/CreateDefaultBootMenu";
            Request.AddJsonBody(bootOptions);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public bool CreateTaskBootFiles(string url, string serverName, string interComKey, DtoTaskBootFile bootFile)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/CreateTaskBootFiles";
            Request.AddJsonBody(bootFile);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public int StartUdpSender(string url, string serverName, string interComKey, DtoMulticastArgs mArgs)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/StartUdpSender";
            Request.AddJsonBody(mArgs);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiIntResponse>(Request, serverName, interComKey);
            if (responseData != null)
                return responseData.Value;
            else
                return 0;
        }


        public bool DownloadKernel(string url, string serverName, string interComKey, DtoOnlineKernel onlineKernel)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/DownloadKernel";
            Request.AddJsonBody(onlineKernel);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public List<string> GetKernels(string url, string serverName, string interComKey)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/GetKernels";
            return new ApiRequest(new Uri(url)).ExecuteHMACInterCom<List<string>>(Request, serverName, interComKey);
        }

        public List<string> GetBootImages(string url, string serverName, string interComKey)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/GetBootImages";
            return new ApiRequest(new Uri(url)).ExecuteHMACInterCom<List<string>>(Request, serverName, interComKey);
        }

        public byte[] GenerateISO(string url, string serverName, string interComKey,DtoIsoGenOptions isoOptions)
        {
            Request.Method = Method.POST;
            Request.AddJsonBody(isoOptions);
            Request.Resource = string.Format("Imaging/GenerateISO");
            return new ApiRequest(new Uri(url)).ExecuteRaw(Request);
        }

        public DtoFreeSpace GetFreeSpace(string url, string serverName, string interComKey)
        {
            Request.Method = Method.POST;
            Request.Resource = "Storage/GetFreeSpace";
            return new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoFreeSpace>(Request, serverName, interComKey);
        }


    }
}