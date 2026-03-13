using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class HangfireTriggerAPI : BaseAPI<EntityVersion>
    {


        public HangfireTriggerAPI(string resource, ProtectedLocalStorage protectedLocalStorage) : base(resource, protectedLocalStorage)
        {

        }

        public bool StartFolderSync()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/StartFolderSync", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool StartLdapSync()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/StartLdapSync", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool StartDynamicGroupSync()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/StartDynamicGroupSync", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool StartResetReport()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/StartResetReport", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool StartApproveReport()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/StartApproveReport", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool StartSmartReport()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/StartSmartReport", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool StartDataCleanup()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/StartDataCleanup", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool StartLowDiskReport()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/StartLowDiskReport", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool StartManifestImport()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/StartManifestImport", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public List<DtoRecurringJobStatus> GetJobStatus()
        {
            Request.Method = Method.Get;
            Request.Resource = string.Format("{0}/GetJobStatus", Resource);
            return _apiRequest.Execute<List<DtoRecurringJobStatus>>(Request);
        }

    }
}