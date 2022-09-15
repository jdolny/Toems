using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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
        public string GetDefaultBootFilePath(string type, int id)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetDefaultBootFilePath/", Resource);
            Request.AddParameter("type", type);
            Request.AddParameter("id", id);
            var responseData = new ApiRequest().Execute<DtoApiStringResponse>(Request);
            if (responseData == null)
                return "";
            else
                return responseData.Value;
        }
        public List<DtoReplicationProcess> GetReplicationProcesses(int id)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/GetReplicationProcesses/{1}", Resource, id);
            return new ApiRequest().Execute<List<DtoReplicationProcess>>(Request);
        }

        public bool KillReplicationProcess(int comServerId, int pid)
        {
            var comPid = new DtoComPid();
            comPid.ComServerId = comServerId;
            comPid.Pid = pid;
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/KillReplicationProcess/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(comPid), ParameterType.RequestBody);
            var response = new ApiRequest().Execute<DtoApiBoolResponse>(Request);
            if (response == null)
                return false;
            else
                return response.Value;
        }

        public void SendAction(string url, string serverName, string interComKey, DtoSocketRequest socketRequest)
        {
            Request.Method = Method.POST;
            Request.Resource = "Socket/SendAction";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(socketRequest), ParameterType.RequestBody);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
        }

        public bool SyncStorage(string url, string serverName, string interComKey)
        {
            Request.Method = Method.POST;
            Request.Resource = "Storage/Sync";
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public bool CheckImageExists(string url, string serverName, string interComKey,int imageId)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/CheckImageExists/" + imageId;
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public bool SyncComToSmb(string url, string serverName, string interComKey, List<int> imageIds)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/SyncComToSmb/";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(imageIds), ParameterType.RequestBody);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public bool SyncSmbToCom(string url, string serverName, string interComKey, List<int> imageIds)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/SyncSmbToCom/";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(imageIds), ParameterType.RequestBody);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public bool WakeupComputers(string url, string serverName, string interComKey, DtoWolTask task)
        {
            Request.Method = Method.POST;
            Request.Resource = "Wol/Wakeup";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(task), ParameterType.RequestBody);
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

        public byte[] GenerateRemoteAccessCert(int id)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/GenerateRemoteAccessCert/{1}", Resource, id);
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
            Request.AddParameter("application/json", JsonConvert.SerializeObject(computer), ParameterType.RequestBody);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public bool KillUdpReceiver(string url, string serverName, string interComKey, List<int> pids)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/KillUdpReceiver";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(pids), ParameterType.RequestBody);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public bool TerminateMulticast(string url, string serverName, string interComKey, EntityActiveMulticastSession multicast)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/TerminateMulticast";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(multicast), ParameterType.RequestBody);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public bool CreateDefaultBootMenu(string url, string serverName, string interComKey,DtoBootMenuGenOptions bootOptions)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/CreateDefaultBootMenu";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(bootOptions), ParameterType.RequestBody);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public bool CreateTaskBootFiles(string url, string serverName, string interComKey, DtoTaskBootFile bootFile)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/CreateTaskBootFiles";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(bootFile), ParameterType.RequestBody);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public int StartUdpSender(string url, string serverName, string interComKey, DtoMulticastArgs mArgs)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/StartUdpSender";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(mArgs), ParameterType.RequestBody);
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
            Request.AddParameter("application/json", JsonConvert.SerializeObject(onlineKernel), ParameterType.RequestBody);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public List<string> GetComServerLogs(string url, string serverName, string interComKey)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/GetComServerLogs";
            return new ApiRequest(new Uri(url)).ExecuteHMACInterCom<List<string>>(Request, serverName, interComKey);
        }

        public List<string> GetComServerLogContents(string url, string serverName, string interComKey, string name, int limit)
        {
            var dtorequest = new DtoLogContentRequest();
            dtorequest.name = name;
            dtorequest.limit = limit;
            Request.Method = Method.POST;
            Request.Resource = "Imaging/GetComServerLogContents";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(dtorequest),ParameterType.RequestBody);
            return new ApiRequest(new Uri(url)).ExecuteHMACInterCom<List<string>>(Request, serverName, interComKey);
        }

        public List<string> GetKernels(string url, string serverName, string interComKey)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/GetKernels";
            return new ApiRequest(new Uri(url)).ExecuteHMACInterCom<List<string>>(Request, serverName, interComKey);
        }

        public List<DtoReplicationProcess> GetReplicationProcesses(string url, string serverName, string interComKey)
        {
            Request.Method = Method.POST;
            Request.Resource = "Storage/GetReplicationProcesses";
            return new ApiRequest(new Uri(url)).ExecuteHMACInterCom<List<DtoReplicationProcess>>(Request, serverName, interComKey);
        }

        public bool KillProcess(string url, string serverName, string interComKey, int pid)
        {
            Request.Method = Method.POST;
            Request.Resource = "Storage/KillProcess/" + pid;
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
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
            Request.AddParameter("application/json", JsonConvert.SerializeObject(isoOptions), ParameterType.RequestBody);
            Request.Resource = string.Format("Imaging/GenerateISO");
            return new ApiRequest(new Uri(url)).ExecuteRaw(Request);
        }

        public DtoFreeSpace GetFreeSpace(string url, string serverName, string interComKey)
        {
            Request.Method = Method.POST;
            Request.Resource = "Storage/GetFreeSpace";
            return new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoFreeSpace>(Request, serverName, interComKey);
        }

        public bool VerifyRemoteAccessInstalled(string url, string serverName, string interComKey)
        {
            Request.Method = Method.POST;
            Request.Resource = "RemoteAccess/VerifyRemoteAccessInstalled";
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public string ReadBootFileText(string url, string serverName, string interComKey, string path)
        {
            var dto = new DtoReadFileText();
            dto.Path = path;
            Request.Method = Method.POST;
            Request.Resource = "Imaging/ReadFileText";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(dto), ParameterType.RequestBody);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiStringResponse>(Request, serverName, interComKey);

            return responseData.Value;
        }

        public bool EditBootFileText(string url, string serverName, string interComKey, DtoCoreScript script)
        {
            Request.Method = Method.POST;
            Request.Resource = "Imaging/EditDefaultBootMenu";
            Request.AddParameter("application/json", JsonConvert.SerializeObject(script), ParameterType.RequestBody);
            var responseData = new ApiRequest(new Uri(url)).ExecuteHMACInterCom<DtoApiBoolResponse>(Request, serverName, interComKey);
            return responseData != null && responseData.Value;
        }

        public string ReadFileText(string path, int comServerId)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/ReadFileText/", Resource);
            Request.AddParameter("path", path);
            Request.AddParameter("comServerId", comServerId);
            var response = new ApiRequest().Execute<DtoApiStringResponse>(Request);

            return response.Value;
        }

        public bool EditDefaultBootMenu(DtoCoreScript script)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/EditDefaultBootMenu/", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(script), ParameterType.RequestBody);
            var response = new ApiRequest().Execute<DtoApiBoolResponse>(Request);

            return response.Value;
        }

    }
}