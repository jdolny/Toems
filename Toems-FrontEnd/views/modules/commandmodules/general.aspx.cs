using System;

namespace Toems_FrontEnd.views.modules.commandmodules
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
            txtDisplayName.Text = CommandModule.Name;
            txtGuid.Text = CommandModule.Guid;
            txtGuid.ReadOnly = true;
            txtDescription.Text = CommandModule.Description;
            txtCommand.Text = CommandModule.Command;
            txtTimeout.Text = CommandModule.Timeout.ToString();
            txtWorkingDirectory.Text = CommandModule.WorkingDirectory;
            txtArguments.Text = CommandModule.Arguments;
            chkStdOut.Checked = CommandModule.RedirectStdOut;
            chkStdError.Checked = CommandModule.RedirectStdError;
            txtSuccessCodes.Text = CommandModule.SuccessCodes;
            ddlRunAs.SelectedValue = CommandModule.ImpersonationId.ToString();
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            if (CommandModule.Archived)
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

            CommandModule.Name = txtDisplayName.Text;
            CommandModule.Description = txtDescription.Text;
            CommandModule.Command = txtCommand.Text;
            CommandModule.WorkingDirectory = txtWorkingDirectory.Text;
            CommandModule.Timeout = Convert.ToInt32(txtTimeout.Text);
            CommandModule.Arguments = txtArguments.Text;
            CommandModule.RedirectStdOut = chkStdOut.Checked;
            CommandModule.RedirectStdError = chkStdError.Checked;
            CommandModule.SuccessCodes = txtSuccessCodes.Text;
            CommandModule.ImpersonationId = Convert.ToInt32(ddlRunAs.SelectedValue);
            var result = Call.CommandModuleApi.Put(CommandModule.Id, CommandModule);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Module {0}", CommandModule.Name) : result.ErrorMessage;
        }



        protected void deleteModule_OnClick(object sender, EventArgs e)
        {
            var result = Call.CommandModuleApi.Delete(CommandModule.Id);
            if (result.Success)
            {
                EndUserMessage = String.Format("Successfully Deleted Module {0}", CommandModule.Name);
                Response.Redirect("~/views/modules/commandmodules/search.aspx");
            }
            else
                EndUserMessage = result.ErrorMessage;
        }
    }
}