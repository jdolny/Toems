using System.Collections.Generic;
using RestSharp;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_ApiCalls
{
    public class HangfireTriggerAPI : BaseAPI<EntityVersion>
    {
        private readonly ApiRequest _apiRequest;

        public HangfireTriggerAPI(string resource) : base(resource)
        {
            _apiRequest = new ApiRequest();
        }

        public bool StartFolderSync()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/StartFolderSync", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool StartLdapSync()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/StartLdapSync", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool StartDynamicGroupSync()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/StartDynamicGroupSync", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool StartResetReport()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/StartResetReport", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool StartApproveReport()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/StartApproveReport", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool StartSmartReport()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/StartSmartReport", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool StartDataCleanup()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/StartDataCleanup", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public bool StartLowDiskReport()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/StartLowDiskReport", Resource);
            var response = _apiRequest.Execute<DtoApiBoolResponse>(Request);
            return response != null && response.Value;
        }

        public List<DtoRecurringJobStatus> GetJobStatus()
        {
            Request.Method = Method.GET;
            Request.Resource = string.Format("{0}/GetJobStatus", Resource);
            return new ApiRequest().Execute<List<DtoRecurringJobStatus>>(Request);
        }

    }
}