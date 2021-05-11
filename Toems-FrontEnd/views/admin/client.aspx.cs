using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Toems_Common;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.admin
{
    public partial class client : BasePages.Admin
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.Administrator);
            if (!IsPostBack)
            {
                PopulateForm();
                FillStartupDelays();
            }
        }

        private void PopulateForm()
        {
            PopulateStartupDelay(ddlStartupDelay);
            var startupDelaySetting = Call.SettingApi.GetSetting(SettingStrings.StartupDelayType);
            var delayType = 0;
            if (startupDelaySetting != null)
            {
                if (!string.IsNullOrEmpty(startupDelaySetting.Value))
                    delayType = Convert.ToInt16(startupDelaySetting.Value);
            }
            ddlStartupDelay.SelectedValue = ((EnumPolicy.StartupDelayType)delayType).ToString();
            var delay = (EnumPolicy.StartupDelayType)Enum.Parse(typeof(EnumPolicy.StartupDelayType), ddlStartupDelay.SelectedValue);
            if (delay == EnumPolicy.StartupDelayType.ForXseconds)
            {
                txtDelayTime.Text = Call.SettingApi.GetSetting(SettingStrings.StartupDelaySub).Value;
            }
            else
            {
                txtStartupFilePath.Text = Call.SettingApi.GetSetting(SettingStrings.StartupDelaySub).Value;
            }
            txtThreshold.Text = Call.SettingApi.GetSetting(SettingStrings.ThresholdWindow).Value;
            txtCheckin.Text = Call.SettingApi.GetSetting(SettingStrings.CheckinInterval).Value;

            txtArguments.Text = Call.SettingApi.GetClientInstallArgs();
            txtShutdownDelay.Text = Call.SettingApi.GetSetting(SettingStrings.ShutdownDelay).Value;
            txtDomainUsername.Text = Call.SettingApi.GetSetting(SettingStrings.DomainJoinUser).Value;
            txtDomainName.Text = Call.SettingApi.GetSetting(SettingStrings.DomainJoinName).Value;
        }
      

        protected void btnUpdateSettings_OnClick(object sender, EventArgs e)
        {
            int value;
            if (!int.TryParse(txtThreshold.Text, out value))
            {
                EndUserMessage = "Threshold Window Has An Invalid Value ";
                return;
            }
            if (!int.TryParse(txtCheckin.Text, out value))
            {
                EndUserMessage = "Checkin Interval Has An Invalid Value ";
                return;
            }
            if (!int.TryParse(txtShutdownDelay.Text, out value))
            {
                EndUserMessage = "Shutdown Delay Has An Invalid Value ";
                return;
            }

            var subValue = string.Empty;
            var delayType = (EnumPolicy.StartupDelayType)Enum.Parse(typeof(EnumPolicy.StartupDelayType), ddlStartupDelay.SelectedValue);
            if (delayType == EnumPolicy.StartupDelayType.ForXseconds)
                subValue = txtDelayTime.Text;
            else if (delayType == EnumPolicy.StartupDelayType.FileCondition)
                subValue = txtStartupFilePath.Text;

            var listSettings = new List<EntitySetting>
            {
              
                new EntitySetting
                {
                    Name = SettingStrings.StartupDelayType,
                    Value = ((int)delayType).ToString(),
                    Id = Call.SettingApi.GetSetting(SettingStrings.StartupDelayType).Id
                },
                new EntitySetting
                {
                    Name = SettingStrings.ThresholdWindow,
                    Value = txtThreshold.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.ThresholdWindow).Id
                },
                new EntitySetting
                {
                    Name = SettingStrings.StartupDelaySub,
                    Value = subValue,
                    Id = Call.SettingApi.GetSetting(SettingStrings.StartupDelaySub).Id
                },
                new EntitySetting
                {
                    Name = SettingStrings.CheckinInterval,
                    Value = txtCheckin.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.CheckinInterval).Id
                },
                new EntitySetting
                {
                    Name = SettingStrings.ShutdownDelay,
                    Value = txtShutdownDelay.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.ShutdownDelay).Id
                },
                 new EntitySetting
                {
                    Name = SettingStrings.DomainJoinUser,
                    Value = txtDomainUsername.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.DomainJoinUser).Id
                },
                  new EntitySetting
                {
                    Name = SettingStrings.DomainJoinPasswordEncrypted,
                    Value = txtDomainPassword.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.DomainJoinPasswordEncrypted).Id
                },
                   new EntitySetting
                {
                    Name = SettingStrings.DomainJoinName,
                    Value = txtDomainName.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.DomainJoinName).Id
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

        protected void ddlStartupDelay_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            FillStartupDelays();
        }

        private void FillStartupDelays()
        {
          
            var delayType = (EnumPolicy.StartupDelayType)Enum.Parse(typeof(EnumPolicy.StartupDelayType), ddlStartupDelay.SelectedValue);
            if (delayType == EnumPolicy.StartupDelayType.ForXseconds)
            {
                divDelayTime.Visible = true;
                divDelayFilePath.Visible = false;
            }
            else if (delayType == EnumPolicy.StartupDelayType.FileCondition)
            {
                divDelayFilePath.Visible = true;
                divDelayTime.Visible = false;
            }
            else
            {
                divDelayTime.Visible = false;
                divDelayFilePath.Visible = false;
            }
        }

        protected void btnExportMsi64_Click(object sender, EventArgs e)
        {
            var msi = Call.SettingApi.ExportMsi(true);
            var fileName = Call.SettingApi.GetMsiFileName(true);
            Response.Clear();
            var ms = new MemoryStream(msi);
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("content-disposition", $"attachment;filename={fileName}");


            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();
        }

        protected void btnExportMsi32_Click(object sender, EventArgs e)
        {
            var msi = Call.SettingApi.ExportMsi(false);
            var fileName = Call.SettingApi.GetMsiFileName(false);
            Response.Clear();
            var ms = new MemoryStream(msi);
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("content-disposition", $"attachment;filename={fileName}");


            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();
        }

        protected void btnCopyToec_Click(object sender, EventArgs e)
        {
            var result = Call.SettingApi.CopyToecUpgrade();
            if (result)
            {
                EndUserMessage = "Success.  Clients Will Auto Update At Next Checkin.";
            }
            else
                EndUserMessage = "Failed";
        }
    }
}