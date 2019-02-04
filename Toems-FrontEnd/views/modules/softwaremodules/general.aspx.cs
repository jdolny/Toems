using System;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.modules.softwaremodules
{
    public partial class general : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
            ddlType.DataSource = Enum.GetNames(typeof(EnumSoftwareModule.MsiInstallType));
            ddlType.DataBind();
            PopulateImpersonationDdl(ddlRunAs);
            txtDisplayName.Text = SoftwareModule.Name;
            txtGuid.Text = SoftwareModule.Guid;
            txtGuid.ReadOnly = true;
            txtDescription.Text = SoftwareModule.Description;
            ddlType.SelectedValue = SoftwareModule.InstallType.ToString();
            txtArguments.Text = SoftwareModule.Arguments;
            txtTimeout.Text = SoftwareModule.Timeout.ToString();
            txtCommand.Text = SoftwareModule.Command;
            txtAddArguments.Text = SoftwareModule.AdditionalArguments;
            txtSuccessCodes.Text = SoftwareModule.SuccessCodes;
            chkStdOut.Checked = SoftwareModule.RedirectStdOut;
            chkStdError.Checked = SoftwareModule.RedirectStdError;
            ddlRunAs.SelectedValue = SoftwareModule.ImpersonationId.ToString();
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            if (SoftwareModule.Archived)
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

            SoftwareModule.Name = txtDisplayName.Text;
            SoftwareModule.Description = txtDescription.Text;
            SoftwareModule.InstallType =
                (EnumSoftwareModule.MsiInstallType) Enum.Parse(typeof (EnumSoftwareModule.MsiInstallType), ddlType.SelectedValue);
            SoftwareModule.Arguments = txtArguments.Text;
            SoftwareModule.Timeout = Convert.ToInt32(txtTimeout.Text);
            SoftwareModule.Command = txtCommand.Text;
            SoftwareModule.AdditionalArguments = txtAddArguments.Text;
            SoftwareModule.RedirectStdOut = chkStdOut.Checked;
            SoftwareModule.RedirectStdError = chkStdError.Checked;
            SoftwareModule.SuccessCodes = txtSuccessCodes.Text;
            SoftwareModule.ImpersonationId = Convert.ToInt32(ddlRunAs.SelectedValue);
            var result = Call.SoftwareModuleApi.Put(SoftwareModule.Id, SoftwareModule);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Module {0}", SoftwareModule.Name) : result.ErrorMessage;
        }

        protected void deleteModule_OnClick(object sender, EventArgs e)
        {
            var result = Call.SoftwareModuleApi.Delete(SoftwareModule.Id);
            if (result.Success)
            {
                EndUserMessage = String.Format("Successfully Deleted Module {0}", SoftwareModule.Name);
                Response.Redirect("~/views/modules/softwaremodules/search.aspx");
            }
            else
                EndUserMessage = result.ErrorMessage;
        }
    }
}