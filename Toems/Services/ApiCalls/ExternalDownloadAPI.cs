using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ExternalDownloadAPI : BaseAPI<EntityExternalDownload>
    {
        public ExternalDownloadAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public List<EntityExternalDownload> GetForModule(string moduleGuid)
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetForModule", Resource);
            Request.AddParameter("moduleGuid", moduleGuid);
            return _apiRequest.Execute<List<EntityExternalDownload>>(Request);
        }

        public void DownloadFile(DtoFileDownload download)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/DownloadFile", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(download), ParameterType.RequestBody);
            Request.Timeout = TimeSpan.MaxValue;
#pragma warning disable CS4014
            _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
#pragma warning restore CS4014

        }

        public void BatchDownload(List<DtoFileDownload> downloads)
        {
            Request.Method = Method.Post;
            Request.Resource = string.Format("{0}/BatchDownload", Resource);
            Request.AddParameter("application/json", JsonConvert.SerializeObject(downloads), ParameterType.RequestBody);
            Request.Timeout = TimeSpan.MaxValue;
#pragma warning disable CS4014
            _apiRequest.ExecuteAsync<DtoApiBoolResponse>(Request);
#pragma warning restore CS4014
        }



    }
}