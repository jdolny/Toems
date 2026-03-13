using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class WieBuildAPI : BaseAPI<EntityWieBuild>
    {

        public WieBuildAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {
            
        }

        public EntityWieBuild GetLastBuild()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetLastBuild/", Resource);
            return _apiRequest.Execute<EntityWieBuild>(Request);
        }
        public void UpdateStatus()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/UpdateStatus/", Resource);
            _apiRequest.Execute<DtoApiBoolResponse>(Request);
        }

        public List<DtoReplicationProcess> GetProcess()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetProcess/", Resource);
            return _apiRequest.Execute<List<DtoReplicationProcess>>(Request);
        }

        public bool GetWinPeDriver(DtoClientFileRequest fileRequest, string outputPath, string url, string serverName, string interComKey)
        {
            Request.Method = Method.Post;
            Request.Timeout = TimeSpan.MaxValue;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(fileRequest), ParameterType.RequestBody);
            Request.Resource = "Storage/GetWinPeDriver/";
            return new ApiRequest(new Uri(url)).DownloadWinPeDriver(Request, outputPath,serverName,interComKey);
        }

        public byte[] ExportWie()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/ExportWie/", Resource);
            return _apiRequest.ExecuteRaw(Request);

        }

        public bool CheckIsoExists()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/CheckIsoExists/", Resource);
            var result = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            if (result == null)
                return false;
            return result.Value;
        }
    }
}