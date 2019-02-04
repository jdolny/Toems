using System;
using System.Collections.Generic;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{

    public class ExternalDownloadAPI : BaseAPI<EntityExternalDownload>
    {
        public ExternalDownloadAPI(string resource) : base(resource)
        {

        }

        public List<EntityExternalDownload> GetForModule(string moduleGuid)
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetForModule", Resource);
            Request.AddParameter("moduleGuid", moduleGuid);
            return new ApiRequest().Execute<List<EntityExternalDownload>>(Request);
        }

        public void DownloadFile(DtoFileDownload download)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/DownloadFile", Resource);
            Request.AddJsonBody(download);
            Request.Timeout = Int32.MaxValue;
            new ApiRequest().ExecuteAsync<DtoApiBoolResponse>(Request);
           
        }

        public void BatchDownload(List<DtoFileDownload> downloads)
        {
            Request.Method = Method.POST;
            Request.Resource = string.Format("{0}/BatchDownload", Resource);
            Request.AddJsonBody(downloads);
            Request.Timeout = Int32.MaxValue;
            new ApiRequest().ExecuteAsync<DtoApiBoolResponse>(Request);

        }



    }
}