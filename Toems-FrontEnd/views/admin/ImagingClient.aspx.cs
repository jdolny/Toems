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

            };
           
            EndUserMessage = Call.SettingApi.UpdateSettings(listSettings)
                ? "Successfully Updated Settings"
                : "Could Not Update Settings";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            txtArguments.Text = GetSetting(SettingStrings.GlobalImagingArguments);
            txtPort.Text = GetSetting(SettingStrings.IpxeHttpPort);
            txtImagingTimeout.Text = GetSetting(SettingStrings.ImageTaskTimeoutMinutes);
            if (GetSetting(SettingStrings.IpxeSSL) == "True")
                chkIpxeSsl.Checked = true;
        }
    }
}