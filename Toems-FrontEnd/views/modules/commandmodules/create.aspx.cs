using System;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.modules.commandmodules
{
    public partial class create : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                PopulateImpersonationDdl(ddlRunAs);
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

            var module = new EntityCommandModule()
            {
                Name = txtDisplayName.Text,
                Description = txtDescription.Text,
                Command = txtCommand.Text,
                WorkingDirectory = txtWorkingDirectory.Text,
                Timeout = Convert.ToInt32(txtTimeout.Text),
                Arguments = txtArguments.Text,
                RedirectStdOut = chkStdOut.Checked,
                RedirectStdError = chkStdError.Checked,
                SuccessCodes = txtSuccessCodes.Text,
                ImpersonationId = Convert.ToInt32(ddlRunAs.SelectedValue)

            };


            var result = Call.CommandModuleApi.Post(module);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created Command Module";
                Response.Redirect("~/views/modules/commandmodules/general.aspx?commandModuleId=" + result.Id);
            }
        }
    }
}