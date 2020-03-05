using System;
using System.Collections.Generic;
using NCrontab;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.admin
{
    public partial class taskscheduler : BasePages.Admin
    {
        protected void btnUpdateSettings_OnClick(object sender, EventArgs e)
        {
            var listSettings = new List<EntitySetting>
            {
              
                new EntitySetting
                {
                    Name = SettingStrings.FolderSyncSchedule,
                    Value = txtFolderSync.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.FolderSyncSchedule).Id
                },
                 new EntitySetting
                {
                    Name = SettingStrings.LdapSyncSchedule,
                    Value = txtLdap.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.LdapSyncSchedule).Id
                },
                 new EntitySetting
                {
                    Name = SettingStrings.DynamicGroupSchedule,
                    Value = txtGroup.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.DynamicGroupSchedule).Id
                },
                  new EntitySetting
                {
                    Name = SettingStrings.ResetReportSchedule,
                    Value = txtReset.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.ResetReportSchedule).Id
                },
                    new EntitySetting
                {
                    Name = SettingStrings.ApproveReportSchedule,
                    Value = txtApproval.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.ApproveReportSchedule).Id
                },
                 new EntitySetting
                {
                    Name = SettingStrings.SmartReportSchedule,
                    Value = txtSmart.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.SmartReportSchedule).Id
                },
                 new EntitySetting
                {
                    Name = SettingStrings.DataCleanupSchedule,
                    Value = txtDataCleanup.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.DataCleanupSchedule).Id
                },
                  new EntitySetting
                {
                    Name = SettingStrings.LowDiskSchedule,
                    Value = txtLowDiskSpace.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.LowDiskSchedule).Id
                },


            };

            foreach (var setting in listSettings)
            {
                if (string.IsNullOrEmpty(setting.Value)) continue; //disables job
                if (CrontabSchedule.TryParse(setting.Value) == null)
                {
                    EndUserMessage = setting.Name + " Is Not A Valid CRON Expression.";
                    return;
                }
            }

            if (Call.SettingApi.UpdateSettings(listSettings))
            {
                EndUserMessage = "Successfully Updated Settings";
            }
            else
            {
                EndUserMessage = "Could Not Update Settings";
            }
        }



        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                PopulateForm();
            }

        }

        private void PopulateForm()
        {

            txtFolderSync.Text = Call.SettingApi.GetSetting(SettingStrings.FolderSyncSchedule).Value;
            txtLdap.Text = Call.SettingApi.GetSetting(SettingStrings.LdapSyncSchedule).Value;
            txtGroup.Text = Call.SettingApi.GetSetting(SettingStrings.DynamicGroupSchedule).Value;
            txtReset.Text = Call.SettingApi.GetSetting(SettingStrings.ResetReportSchedule).Value;
            txtApproval.Text = Call.SettingApi.GetSetting(SettingStrings.ApproveReportSchedule).Value;
            txtSmart.Text = Call.SettingApi.GetSetting(SettingStrings.SmartReportSchedule).Value;
            txtDataCleanup.Text = Call.SettingApi.GetSetting(SettingStrings.DataCleanupSchedule).Value;
            txtLowDiskSpace.Text = Call.SettingApi.GetSetting(SettingStrings.LowDiskSchedule).Value;

            var jobs = Call.HangfireTriggerApi.GetJobStatus();
            foreach (var job in jobs)
            {
                if (string.IsNullOrEmpty(job.Name)) continue;
                if (job.Name.Equals("StorageSync-Job"))
                {
                    lblLastRun.Text = string.IsNullOrEmpty(job.LastRun) ? "N/A" : job.LastRun;
                    lblStatus.Text = string.IsNullOrEmpty(job.Status) ? "N/A" : job.Status;
                    lblNextRun.Text = string.IsNullOrEmpty(job.NextRun) ? "N/A" : job.NextRun;

                }
                else if (job.Name.Equals("DynamicGroupUpdate-Job"))
                {
                    lblGroupLastRun.Text = string.IsNullOrEmpty(job.LastRun) ? "N/A" : job.LastRun;
                    lblGroupStatus.Text = string.IsNullOrEmpty(job.Status) ? "N/A" : job.Status;
                    lblGroupNextRun.Text = string.IsNullOrEmpty(job.NextRun) ? "N/A" : job.NextRun;
                }
                else if (job.Name.Equals("LDAPSync-Job"))
                {
                    lblLdapLastRun.Text = string.IsNullOrEmpty(job.LastRun) ? "N/A" : job.LastRun;
                    lblLdapStatus.Text = string.IsNullOrEmpty(job.Status) ? "N/A" : job.Status;
                    lblLdapNextRun.Text = string.IsNullOrEmpty(job.NextRun) ? "N/A" : job.NextRun;
                }
                else if (job.Name.Equals("ResetRequestReport-Job"))
                {
                    lblResetLastRun.Text = string.IsNullOrEmpty(job.LastRun) ? "N/A" : job.LastRun;
                    lblResetStatus.Text = string.IsNullOrEmpty(job.Status) ? "N/A" : job.Status;
                    lblResetNextRun.Text = string.IsNullOrEmpty(job.NextRun) ? "N/A" : job.NextRun;
                }
                else if (job.Name.Equals("ApprovalRequestReport-Job"))
                {
                    lblApprovalLastRun.Text = string.IsNullOrEmpty(job.LastRun) ? "N/A" : job.LastRun;
                    lblApprovalStatus.Text = string.IsNullOrEmpty(job.Status) ? "N/A" : job.Status;
                    lblApprovalNextRun.Text = string.IsNullOrEmpty(job.NextRun) ? "N/A" : job.NextRun;
                }
                else if (job.Name.Equals("SmartReport-Job"))
                {
                    lblSmartLastRun.Text = string.IsNullOrEmpty(job.LastRun) ? "N/A" : job.LastRun;
                    lblSmartStatus.Text = string.IsNullOrEmpty(job.Status) ? "N/A" : job.Status;
                    lblSmartNextRun.Text = string.IsNullOrEmpty(job.NextRun) ? "N/A" : job.NextRun;
                }
                else if (job.Name.Equals("DataCleanup-Job"))
                {
                    lblDataCleanupLastRun.Text = string.IsNullOrEmpty(job.LastRun) ? "N/A" : job.LastRun;
                    lblDataCleanupStatus.Text = string.IsNullOrEmpty(job.Status) ? "N/A" : job.Status;
                    lblDataCleanupNextRun.Text = string.IsNullOrEmpty(job.NextRun) ? "N/A" : job.NextRun;
                }
                else if (job.Name.Equals("LowDiskReport-Job"))
                {
                    lblLowDiskLastRun.Text = string.IsNullOrEmpty(job.LastRun) ? "N/A" : job.LastRun;
                    lblLowDiskStatus.Text = string.IsNullOrEmpty(job.Status) ? "N/A" : job.Status;
                    lblLowDiskNextRun.Text = string.IsNullOrEmpty(job.NextRun) ? "N/A" : job.NextRun;
                }

            }
        }

        protected void btnFolderSync_OnClick(object sender, EventArgs e)
        {
            Call.HangfireTriggerApi.StartFolderSync();
        }

        protected void btnLdapSync_OnClick(object sender, EventArgs e)
        {
            Call.HangfireTriggerApi.StartLdapSync();
        }

        protected void btnGroup_OnClick(object sender, EventArgs e)
        {
            Call.HangfireTriggerApi.StartDynamicGroupSync();
        }

        protected void btnReset_OnClick(object sender, EventArgs e)
        {
            Call.HangfireTriggerApi.StartResetReport();
        }

        protected void btnApproval_OnClick(object sender, EventArgs e)
        {
            Call.HangfireTriggerApi.StartApproveReport();
        }

        protected void btnSmart_OnClick(object sender, EventArgs e)
        {
            Call.HangfireTriggerApi.StartSmartReport();
        }

        protected void btnDataCleanup_OnClick(object sender, EventArgs e)
        {
            Call.HangfireTriggerApi.StartDataCleanup();
        }

        protected void btnLowDisk_OnClick(object sender, EventArgs e)
        {
            Call.HangfireTriggerApi.StartDataCleanup();
        }



    }
}