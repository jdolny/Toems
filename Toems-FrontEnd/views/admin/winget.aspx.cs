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
    public partial class winget : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.Administrator);
            if (IsPostBack) return;

            txtUrl.Text = GetSetting(SettingStrings.WingetPackageSource);
            lblLastImport.Text = Call.WingetModuleApi.GetLastWingetImportTime();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            var listSettings = new List<EntitySetting>
            {

                new EntitySetting
                {
                    Name = SettingStrings.WingetPackageSource,
                    Value = txtUrl.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.WingetPackageSource).Id
                }
            };

            EndUserMessage = Call.SettingApi.UpdateSettings(listSettings) ? "Successfully Updated Settings" : "Could Not Update Settings";
        }
    }
}