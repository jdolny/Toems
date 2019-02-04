using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_FrontEnd.views.modules.wumodules
{
    public partial class create : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

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

            var module = new EntityWuModule()
            {
                Name = txtDisplayName.Text,
                Description = txtDescription.Text,             
                Timeout = Convert.ToInt32(txtTimeout.Text),
                RedirectStdOut = chkStdOut.Checked,
                RedirectStdError = chkStdError.Checked,
                SuccessCodes = txtSuccessCodes.Text,
            };


            var result = Call.WuModuleApi.Post(module);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created Windows Update Module";
                Response.Redirect("~/views/modules/wumodules/general.aspx?wuModuleId=" + result.Id);
            }
        }
    }
}