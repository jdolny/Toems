using System;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.modules.scriptmodules
{
    public partial class create : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlType.DataSource = Enum.GetNames(typeof(EnumScriptModule.ScriptType));
                ddlType.DataBind();
                PopulateImpersonationDdl(ddlRunAs);
             
            }
            ShowDivs();
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

            var module = new EntityScriptModule()
            {
                Name = txtDisplayName.Text,
                Description = txtDescription.Text,
                Arguments = txtArguments.Text,
                ScriptType = (EnumScriptModule.ScriptType)Enum.Parse(typeof(EnumScriptModule.ScriptType), ddlType.SelectedValue),
                WorkingDirectory = txtWorkingDirectory.Text,
                Timeout = Convert.ToInt32(txtTimeout.Text),
                ScriptContents = scriptEditor.Value,
                RedirectStdOut = chkStdOut.Checked,
                RedirectStdError = chkStdError.Checked,
                SuccessCodes = txtSuccessCodes.Text,
                AddInventoryCollection = chkInventory.Checked,
                IsCondition = chkCondition.Checked,
                ImpersonationId = Convert.ToInt32(ddlRunAs.SelectedValue)
            };


            var result = Call.ScriptModuleApi.Post(module);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created Script Module";
                Response.Redirect("~/views/modules/scriptmodules/general.aspx?scriptModuleId=" + result.Id);
            }
        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDivs();
        }

        private void ShowDivs()
        {
            if (ddlType.Text == "ImagingClient_Bash")
                divNotBash.Visible = false;
            else
                divNotBash.Visible = true;
        }
    }
}