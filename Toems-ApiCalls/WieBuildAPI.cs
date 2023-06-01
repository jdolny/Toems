using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Dto.client;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class WieBuildAPI : BaseAPI<EntityWieBuild>
    {

        public WieBuildAPI(string resource) : base(resource)
        {
            
        }

        public EntityWieBuild GetLastBuild()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetLastBuild/", Resource);
            return new ApiRequest().Execute<EntityWieBuild>(Request);
        }
        public void UpdateStatus()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/UpdateStatus/", Resource);
            new ApiRequest().Execute<DtoApiBoolResponse>(Request);
        }

        public List<DtoReplicationProcess> GetProcess()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetProcess/", Resource);
            return new ApiRequest().Execute<List<DtoReplicationProcess>>(Request);
        }

        public bool GetWinPeDriver(DtoClientFileRequest fileRequest, string outputPath, string url, string serverName, string interComKey)
        {
            Request.Method = Method.POST;
            Request.Timeout = 14400000;
            Request.AddParameter("application/json", JsonConvert.SerializeObject(fileRequest), ParameterType.RequestBody);
            Request.Resource = "Storage/GetWinPeDriver/";
            return new ApiRequest(new Uri(url)).DownloadWinPeDriver(Request, outputPath,serverName,interComKey);
        }


    }
}