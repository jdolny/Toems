using System;
using System.Collections.Generic;
using System.Linq;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.admin
{
    public partial class server : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                PopulateForm();
            }

        }

        private void PopulateForm()
        {
            txtOrganization.Text = Call.SettingApi.GetSetting(SettingStrings.CertificateOrganization).Value;
            txtComputerArchive.Text = Call.SettingApi.GetSetting(SettingStrings.ComputerAutoArchiveDays).Value;
            txtAuditLogDelete.Text = Call.SettingApi.GetSetting(SettingStrings.AuditLogAutoDelete).Value;
            txtComputerProcess.Text = Call.SettingApi.GetSetting(SettingStrings.ComputerProcessAutoDelete).Value;
            txtDeleteComputers.Text = Call.SettingApi.GetSetting(SettingStrings.ComputerAutoDelete).Value;
            txtPolicyHistory.Text = Call.SettingApi.GetSetting(SettingStrings.PolicyHistoryAutoDelete).Value;
            txtUserLogin.Text = Call.SettingApi.GetSetting(SettingStrings.UserLoginHistoryAutoDelete).Value;



        }

        protected void btnUpdateSettings_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtOrganization.Text))
            {
                EndUserMessage = "Organization Must Not Be Empty";
                return;
            }
            if (!txtOrganization.Text.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' ' || c == '.'))
            {
                EndUserMessage = "Organization Can Only Contain Alphanumeric Characters And _ - space .";
                return;
            }
                var listSettings = new List<EntitySetting>
            {
              
                new EntitySetting
                {
                    Name = SettingStrings.CertificateOrganization,
                    Value = txtOrganization.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.CertificateOrganization).Id
                },
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