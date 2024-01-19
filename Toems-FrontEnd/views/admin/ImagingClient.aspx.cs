using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.admin
{
    public partial class ImagingClient : Admin
    {


        protected void btnUpdateSettings_OnClick(object sender, EventArgs e)
        {
            if(chkDirectSMB.Checked)
            {
                var storageType = GetSetting(SettingStrings.StorageType);
                if(storageType.Equals("Local"))
                {
                    EndUserMessage = "Direct SMB Imaging Cannot Be Used When The Global Storage Type Is Set To Local.  You Must First Change Your Storage Type.";
                    return;
                }

            }

            var listSettings = new List<EntitySetting>
            {
                new EntitySetting
                {
                    Name = SettingStrings.GlobalImagingArguments,
                    Value = txtArguments.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.GlobalImagingArguments).Id
                },
                new EntitySetting
                {
                    Name = SettingStrings.IpxeSSL,
                    Value = chkIpxeSsl.Checked.ToString(),
                    Id = Call.SettingApi.GetSetting(SettingStrings.IpxeSSL).Id
                },
                  new EntitySetting
                {
                    Name = SettingStrings.IpxeHttpPort,
                    Value = txtPort.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.IpxeHttpPort).Id
                },
                      new EntitySetting
                {
                    Name = SettingStrings.ImageTaskTimeoutMinutes,
                    Value = txtImagingTimeout.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.ImageTaskTimeoutMinutes).Id
                },
                    new EntitySetting
            {
                Name = SettingStrings.RegistrationEnabled,
                Value = ddlRegistration.Text,
                Id = Call.SettingApi.GetSetting(SettingStrings.RegistrationEnabled).Id
            },
                    new EntitySetting
            {
                Name = SettingStrings.DisabledRegNamePrompt,
                Value = ddlKeepNamePrompt.Text,
                Id = Call.SettingApi.GetSetting(SettingStrings.DisabledRegNamePrompt).Id
                    },

                new EntitySetting
            {
                Name = SettingStrings.ImageDirectSmb,
                Value = chkDirectSMB.Checked.ToString(),
                Id = Call.SettingApi.GetSetting(SettingStrings.ImageDirectSmb).Id
                    },
                   new EntitySetting
            {
                Name = SettingStrings.DefaultWieImageType,
                Value = ddlWieImageType.Text,
                Id = Call.SettingApi.GetSetting(SettingStrings.DefaultWieImageType).Id
                    },
              new EntitySetting
            {
                Name = SettingStrings.LieSleepTime,
                Value = txtImagingSleep.Text,
                Id = Call.SettingApi.GetSetting(SettingStrings.LieSleepTime).Id
                    },

            };

            EndUserMessage = Call.SettingApi.UpdateSettings(listSettings)
                ? "Successfully Updated Settings"
                : "Could Not Update Settings";
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            RequiresAuthorization(AuthorizationStrings.Administrator);
            if (IsPostBack) return;
            txtArguments.Text = GetSetting(SettingStrings.GlobalImagingArguments);
            txtPort.Text = GetSetting(SettingStrings.IpxeHttpPort);
            txtImagingTimeout.Text = GetSetting(SettingStrings.ImageTaskTimeoutMinutes);
            if (GetSetting(SettingStrings.IpxeSSL) == "True")
                chkIpxeSsl.Checked = true;
            ddlRegistration.Text = GetSetting(SettingStrings.RegistrationEnabled);
            ddlKeepNamePrompt.Text = GetSetting(SettingStrings.DisabledRegNamePrompt);
            ddlWieImageType.Text = GetSetting(SettingStrings.DefaultWieImageType);
            txtImagingSleep.Text = GetSetting(SettingStrings.LieSleepTime);
            if (GetSetting(SettingStrings.ImageDirectSmb) == "True")
                chkDirectSMB.Checked = true;
        }
    }
}