using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;
using Hangfire;
using Hangfire.Storage;
using Toems_ApplicationApi.Controllers.Authorization;
using Toems_Common;
using Toems_Common.Dto;

namespace Toems_ApplicationApi.Controllers
{
    public class HangfireTriggerController : ApiController
    {
   
        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartFolderSync()
        {
            Hangfire.RecurringJob.Trigger("StorageSync-Job");
            return new DtoApiBoolResponse() {Value = true};
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartLdapSync()
        {
            Hangfire.RecurringJob.Trigger("LDAPSync-Job");
            return new DtoApiBoolResponse() { Value = true };
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartDynamicGroupSync()
        {
            Hangfire.RecurringJob.Trigger("DynamicGroupUpdate-Job");
            return new DtoApiBoolResponse() { Value = true };
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartResetReport()
        {
            Hangfire.RecurringJob.Trigger("ResetRequestReport-Job");
            return new DtoApiBoolResponse() { Value = true };
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartApproveReport()
        {
            Hangfire.RecurringJob.Trigger("ApprovalRequestReport-Job");
            return new DtoApiBoolResponse() { Value = true };
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartSmartReport()
        {
            Hangfire.RecurringJob.Trigger("SmartReport-Job");
            return new DtoApiBoolResponse() { Value = true };
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartDataCleanup()
        {
            Hangfire.RecurringJob.Trigger("DataCleanup-Job");
            return new DtoApiBoolResponse() { Value = true };
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartLowDiskReport()
        {
            Hangfire.RecurringJob.Trigger("LowDiskReport-Job");
            return new DtoApiBoolResponse() { Value = true };
        }


        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartShutdown()
        {
            Hangfire.RecurringJob.Trigger("Shutdown-Job");
            return new DtoApiBoolResponse() { Value = true };
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public List<DtoRecurringJobStatus> GetJobStatus()
        {
            var list = new List<DtoRecurringJobStatus>();
            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var job in connection.GetRecurringJobs())
                {
                    var jobStatus = new DtoRecurringJobStatus();
                    jobStatus.Name = job.Id;
                    jobStatus.LastRun = Convert.ToDateTime(job.LastExecution).ToLocalTime().ToString(CultureInfo.InvariantCulture);
                    jobStatus.NextRun = Convert.ToDateTime(job.NextExecution).ToLocalTime().ToString(CultureInfo.InvariantCulture);
                    jobStatus.Status = job.LastJobState;
                    list.Add(jobStatus);
                }
            }
            return list;
        }

    
    }
}