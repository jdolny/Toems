using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.modules.wumodules
{
    public partial class general : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
          
            txtDisplayName.Text = WuModule.Name;
            txtGuid.Text = WuModule.Guid;
            txtGuid.ReadOnly = true;
            txtDescription.Text = WuModule.Description;  
            txtTimeout.Text = WuModule.Timeout.ToString();
            txtAddArguments.Text = WuModule.AdditionalArguments;
            txtSuccessCodes.Text = WuModule.SuccessCodes;
            chkStdOut.Checked = WuModule.RedirectStdOut;
            chkStdError.Checked = WuModule.RedirectStdError;

        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            if (WuModule.Archived)
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

            WuModule.Name = txtDisplayName.Text;
            WuModule.Description = txtDescription.Text;
            WuModule.Timeout = Convert.ToInt32(txtTimeout.Text);

            WuModule.AdditionalArguments = txtAddArguments.Text;
            WuModule.RedirectStdOut = chkStdOut.Checked;
            WuModule.RedirectStdError = chkStdError.Checked;
            WuModule.SuccessCodes = txtSuccessCodes.Text;
            var result = Call.WuModuleApi.Put(WuModule.Id, WuModule);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Module {0}", WuModule.Name) : result.ErrorMessage;
        }

        protected void deleteModule_OnClick(object sender, EventArgs e)
        {
            var result = Call.WuModuleApi.Delete(SoftwareModule.Id);
            if (result.Success)
            {
                EndUserMessage = String.Format("Successfully Deleted Module {0}", WuModule.Name);
                Response.Redirect("~/views/modules/wumodules/search.aspx");
            }
            else
                EndUserMessage = result.ErrorMessage;
        }
    }
}