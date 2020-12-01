using System;
using System.Collections.Generic;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.admin
{
    public partial class storage : BasePages.Admin
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
            ddlType.SelectedValue = Call.SettingApi.GetSetting(SettingStrings.StorageType).Value;
            txtPath.Text = Call.SettingApi.GetSetting(SettingStrings.StoragePath).Value;
            if (ddlType.Text != "Local")
            {
                txtUsername.Text = Call.SettingApi.GetSetting(SettingStrings.StorageUsername).Value;
              
                txtDomain.Text = Call.SettingApi.GetSetting(SettingStrings.StorageDomain).Value;
            }
            ShowHideDiv();
        }

        protected void ddlType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ShowHideDiv();
        }

        protected void btnUpdateSettings_OnClick(object sender, EventArgs e)
        {
            //this should be moved to back end
            if(txtPath.Text.Contains(" "))
            {
                EndUserMessage = "Storage Path Cannot Contain Any Spaces";
                return;
            }
            var listSettings = new List<EntitySetting>
            {
              
                new EntitySetting
                {
                    Name = SettingStrings.StorageType,
                    Value = ddlType.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.StorageType).Id
                },
                new EntitySetting
                {
                    Name = SettingStrings.StoragePath,
                    Value = txtPath.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.StoragePath).Id
                },
                new EntitySetting
                {
                    Name = SettingStrings.StorageUsername,
                    Value = txtUsername.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.StorageUsername).Id
                },
             
                 new EntitySetting
                {
                    Name = SettingStrings.StorageDomain,
                    Value = txtDomain.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.StorageDomain).Id
                }
              
            };

            if (!string.IsNullOrEmpty(txtPassword.Text))
            {
                listSettings.Add(new EntitySetting()
                {
                    Name = SettingStrings.StoragePassword,
                    Value = txtPassword.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.StoragePassword).Id
                });
            }

            EndUserMessage = Call.SettingApi.UpdateSettings(listSettings) ? "Successfully Updated Settings" : "Could Not Update Settings";
        }

        private void ShowHideDiv()
        {
            divSmb.Visible = ddlType.Text != "Local";
        }
    }
}