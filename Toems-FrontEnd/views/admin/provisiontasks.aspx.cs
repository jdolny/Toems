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
    public partial class provisiontasks : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.Administrator);
            if (IsPostBack) return;

            txtGroup.Text = GetSetting(SettingStrings.NewProvisionDefaultGroup);
            if (GetSetting(SettingStrings.NewProvisionAdCheck) == "1")
                chkldap.Checked = true;

        }


        protected void btnUpdateSettings_OnClick(object sender, EventArgs e)
        {
            var listSettings = new List<EntitySetting>
            {
                new EntitySetting
                {
                    Name = SettingStrings.NewProvisionDefaultGroup,
                    Value = txtGroup.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.NewProvisionDefaultGroup).Id
                }
            };

            var chkValue = chkldap.Checked ? "1" : "0";
            listSettings.Add(new EntitySetting
            {
                Name = SettingStrings.NewProvisionAdCheck,
                Value = chkValue,
                Id = Call.SettingApi.GetSetting(SettingStrings.NewProvisionAdCheck).Id
            });

            EndUserMessage = Call.SettingApi.UpdateSettings(listSettings)
                ? "Successfully Updated Settings"
                : "Could Not Update Settings";
        }
    }
}