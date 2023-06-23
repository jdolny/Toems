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
            new RecurringJobManager().TriggerJob("StorageSync-Job");

            return new DtoApiBoolResponse() {Value = true};
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartLdapSync()
        {
            new RecurringJobManager().TriggerJob("LDAPSync-Job");
            return new DtoApiBoolResponse() { Value = true };
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartDynamicGroupSync()
        {
            new RecurringJobManager().TriggerJob("DynamicGroupUpdate-Job");
            return new DtoApiBoolResponse() { Value = true };
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartResetReport()
        {
            new RecurringJobManager().TriggerJob("ResetRequestReport-Job");
            return new DtoApiBoolResponse() { Value = true };
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartApproveReport()
        {
            new RecurringJobManager().TriggerJob("ApprovalRequestReport-Job");
            return new DtoApiBoolResponse() { Value = true };
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartSmartReport()
        {
            new RecurringJobManager().TriggerJob("SmartReport-Job");
            return new DtoApiBoolResponse() { Value = true };
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartDataCleanup()
        {
            new RecurringJobManager().TriggerJob("DataCleanup-Job");
            return new DtoApiBoolResponse() { Value = true };
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartLowDiskReport()
        {
            new RecurringJobManager().TriggerJob("LowDiskReport-Job");
            return new DtoApiBoolResponse() { Value = true };
        }

        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartManifestImport()
        {
            new RecurringJobManager().TriggerJob("WingetManifest-Job");
            return new DtoApiBoolResponse() { Value = true };
        }


        [HttpGet]
        [CustomAuth(Permission = AuthorizationStrings.Administrator)]
        public DtoApiBoolResponse StartShutdown()
        {
            new RecurringJobManager().TriggerJob("Shutdown-Job");
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