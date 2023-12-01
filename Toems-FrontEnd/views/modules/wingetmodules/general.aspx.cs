using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.modules.wingetmodules
{
    public partial class general : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
            PopulateImpersonationDdl(ddlRunAs);
            PopulateWingetType(ddlInstallType);
            txtDisplayName.Text = WingetModule.Name;
            txtGuid.Text = WingetModule.Guid;
            txtGuid.ReadOnly = true;
            txtDescription.Text = WingetModule.Description;
            txtTimeout.Text = WingetModule.Timeout.ToString();
            txtArguments.Text = WingetModule.Arguments;
            chkAutoUpdate.Checked = WingetModule.KeepUpdated;
            chkLatest.Checked = WingetModule.InstallLatest;
            ddlInstallType.SelectedValue = WingetModule.InstallType.ToString();
            chkStdOut.Checked = WingetModule.RedirectStdOut;
            chkStdError.Checked = WingetModule.RedirectStdError;
            txtOverride.Text = WingetModule.Override;
            ddlRunAs.SelectedValue = WingetModule.ImpersonationId.ToString();
            txtPackageIdentifier.Text = WingetModule.PackageId;
            txtPackageVersion.Text = WingetModule.PackageVersion;

        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            if (WingetModule.Archived)
            {
                EndUserMessage = "Archived Modules Cannot Be Modified";
                return;
            }
            int timeout = 0;
            var parseResult = Int32.TryParse(txtTimeout.Text, out timeout);
            if (!parseResult)
            {
                EndUserMessage = "Timeout Is Not Valid";
                return;
            }

            WingetModule.Name = txtDisplayName.Text;
            WingetModule.Description = txtDescription.Text;
            WingetModule.Timeout = Convert.ToInt32(txtTimeout.Text);
            WingetModule.PackageVersion = txtPackageVersion.Text;
            WingetModule.Arguments = txtArguments.Text;
            WingetModule.Override = txtOverride.Text;
            WingetModule.KeepUpdated = chkAutoUpdate.Checked;
            WingetModule.InstallLatest = chkLatest.Checked;
            WingetModule.InstallType = (EnumWingetInstallType.WingetInstallType)Enum.Parse(typeof(EnumWingetInstallType.WingetInstallType), ddlInstallType.SelectedValue);
            WingetModule.RedirectStdOut = chkStdOut.Checked;
            WingetModule.RedirectStdError = chkStdError.Checked;
            WingetModule.ImpersonationId = Convert.ToInt32(ddlRunAs.SelectedValue);
            
          
            var result = Call.WingetModuleApi.Put(WingetModule.Id, WingetModule);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Module {0}", WingetModule.Name) : result.ErrorMessage;
        }



        protected void deleteModule_OnClick(object sender, EventArgs e)
        {
            var result = Call.WingetModuleApi.Delete(WingetModule.Id);
            if (result.Success)
            {
                EndUserMessage = String.Format("Successfully Deleted Module {0}", WingetModule.Name);
                Response.Redirect("~/views/modules/wingetmodules/search.aspx");
            }
            else
                EndUserMessage = result.ErrorMessage;
        }
    }
}