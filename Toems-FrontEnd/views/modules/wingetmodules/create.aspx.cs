using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.modules.wingetmodules
{
    public partial class create : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateImpersonationDdl(ddlRunAs);
                PopulateWingetType(ddlInstallType);
            }
        }

        protected void btnAdd_OnClick(object sender, EventArgs e)
        {
            int timeout = 0;
            var parseResult = Int32.TryParse(txtTimeout.Text, out timeout);
            if (!parseResult)
            {
                EndUserMessage = "Timeout Is Not Valid";
                return;
            }

            var module = new EntityWingetModule()
            {
                Name = txtDisplayName.Text,
                Description = txtDescription.Text,
                Timeout = Convert.ToInt32(txtTimeout.Text),
                Arguments = "-e --accept-package-agreements --accept-source-agreements -h",
                Override = txtOverride.Text,
                KeepUpdated = chkAutoUpdate.Checked,
                InstallType = (EnumWingetInstallType.WingetInstallType)Enum.Parse(typeof(EnumWingetInstallType.WingetInstallType), ddlInstallType.SelectedValue),
                RedirectStdOut = chkStdOut.Checked,
                RedirectStdError = chkStdError.Checked,
                ImpersonationId = Convert.ToInt32(ddlRunAs.SelectedValue)

            };


            var result = Call.WingetModuleApi.Post(module);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created Winget Module";
                Response.Redirect("~/views/modules/wingetmodules/general.aspx?wingetModuleId=" + result.Id);
            }
        }
    }
}