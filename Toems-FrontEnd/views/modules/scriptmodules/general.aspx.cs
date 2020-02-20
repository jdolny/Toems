using System;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.modules.scriptmodules
{
    public partial class general : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
            ddlType.DataSource = Enum.GetNames(typeof(EnumScriptModule.ScriptType));
            ddlType.DataBind();
            PopulateImpersonationDdl(ddlRunAs);
            txtDisplayName.Text = ScriptModule.Name;
            txtGuid.Text = ScriptModule.Guid;
            txtGuid.ReadOnly = true;
            txtDescription.Text = ScriptModule.Description;
            ddlType.SelectedValue = ScriptModule.ScriptType.ToString();
            txtArguments.Text = ScriptModule.Arguments;
            txtTimeout.Text = ScriptModule.Timeout.ToString();
            txtWorkingDirectory.Text = ScriptModule.WorkingDirectory;
            scriptEditor.Value = ScriptModule.ScriptContents;
            txtSuccessCodes.Text = ScriptModule.SuccessCodes;
            chkStdOut.Checked = ScriptModule.RedirectStdOut;
            chkStdError.Checked = ScriptModule.RedirectStdError;
            chkInventory.Checked = ScriptModule.AddInventoryCollection;
            chkCondition.Checked = ScriptModule.IsCondition;
            ddlRunAs.SelectedValue = ScriptModule.ImpersonationId.ToString();
            ShowDivs();
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            if (ScriptModule.Archived)
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

            ScriptModule.Name = txtDisplayName.Text;
            ScriptModule.Description = txtDescription.Text;
            ScriptModule.Arguments = txtArguments.Text;
            ScriptModule.ScriptType =
                (EnumScriptModule.ScriptType) Enum.Parse(typeof (EnumScriptModule.ScriptType), ddlType.SelectedValue);
            ScriptModule.WorkingDirectory = txtWorkingDirectory.Text;
            ScriptModule.Timeout = Convert.ToInt32(txtTimeout.Text);
            ScriptModule.ScriptContents = scriptEditor.Value;
            ScriptModule.RedirectStdOut = chkStdOut.Checked;
            ScriptModule.RedirectStdError = chkStdError.Checked;
            ScriptModule.SuccessCodes = txtSuccessCodes.Text;
            ScriptModule.AddInventoryCollection = chkInventory.Checked;
            ScriptModule.IsCondition = chkCondition.Checked;
            ScriptModule.ImpersonationId = Convert.ToInt32(ddlRunAs.SelectedValue);
            var result = Call.ScriptModuleApi.Put(ScriptModule.Id, ScriptModule);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Module {0}", ScriptModule.Name) : result.ErrorMessage;
        }



        protected void deleteModule_OnClick(object sender, EventArgs e)
        {
            var result = Call.ScriptModuleApi.Delete(ScriptModule.Id);
            if (result.Success)
            {
                EndUserMessage = String.Format("Successfully Deleted Module {0}", ScriptModule.Name);
                Response.Redirect("~/views/modules/scriptmodules/search.aspx");
            }
            else
                EndUserMessage = result.ErrorMessage;
        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDivs();
        }

        private void ShowDivs()
        {
            if (ddlType.Text == "ImagingClient_Bash" || ddlType.Text == "ImagingClient_PowerShell")
                divNotBash.Visible = false;
            else
                divNotBash.Visible = true;
        }

    }
}