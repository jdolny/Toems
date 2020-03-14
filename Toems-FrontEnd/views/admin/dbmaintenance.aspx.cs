using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.admin
{
    public partial class dbmaintenance : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.Administrator);
            if (!IsPostBack)
            {
                PopulateForm();
            }

        }

        private void PopulateForm()
        {
            txtComputerArchive.Text = Call.SettingApi.GetSetting(SettingStrings.ComputerAutoArchiveDays).Value;
            txtAuditLogDelete.Text = Call.SettingApi.GetSetting(SettingStrings.AuditLogAutoDelete).Value;
            txtComputerProcess.Text = Call.SettingApi.GetSetting(SettingStrings.ComputerProcessAutoDelete).Value;
            txtDeleteComputers.Text = Call.SettingApi.GetSetting(SettingStrings.ComputerAutoDelete).Value;
            txtPolicyHistory.Text = Call.SettingApi.GetSetting(SettingStrings.PolicyHistoryAutoDelete).Value;
            txtUserLogin.Text = Call.SettingApi.GetSetting(SettingStrings.UserLoginHistoryAutoDelete).Value;
            txtImagingLogs.Text = GetSetting(SettingStrings.ImagingLogsAutoDeleteDays);
        }

        protected void btnUpdateSettings_OnClick(object sender, EventArgs e)
        {
           
            var listSettings = new List<EntitySetting>
            {

                 new EntitySetting
                {
                    Name = SettingStrings.ComputerAutoArchiveDays,
                    Value = txtComputerArchive.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.ComputerAutoArchiveDays).Id
                },
                new EntitySetting
                {
                    Name = SettingStrings.AuditLogAutoDelete,
                    Value = txtAuditLogDelete.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.AuditLogAutoDelete).Id
                },
                new EntitySetting
                {
                    Name = SettingStrings.ComputerProcessAutoDelete,
                    Value = txtComputerProcess.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.ComputerProcessAutoDelete).Id
                },
                new EntitySetting
                {
                    Name = SettingStrings.ComputerAutoDelete,
                    Value = txtDeleteComputers.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.ComputerAutoDelete).Id
                },
                new EntitySetting
                {
                    Name = SettingStrings.PolicyHistoryAutoDelete,
                    Value = txtPolicyHistory.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.PolicyHistoryAutoDelete).Id
                },
                new EntitySetting
                {
                    Name = SettingStrings.UserLoginHistoryAutoDelete,
                    Value = txtUserLogin.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.UserLoginHistoryAutoDelete).Id
                },
                  new EntitySetting
                {
                    Name = SettingStrings.ImagingLogsAutoDeleteDays,
                    Value = txtImagingLogs.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.ImagingLogsAutoDeleteDays).Id
                },


            };


            if (Call.SettingApi.UpdateSettings(listSettings))
            {
                EndUserMessage = "Successfully Updated Settings";
            }
            else
            {
                EndUserMessage = "Could Not Update Settings";
            }
        }
    }
}