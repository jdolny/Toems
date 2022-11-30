using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.modules.winpemodules
{
    public partial class general : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {

            txtDisplayName.Text = WinPeModule.Name;
            txtGuid.Text = WinPeModule.Guid;
            txtGuid.ReadOnly = true;
            txtDescription.Text = WinPeModule.Description;

        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            if (WinPeModule.Archived)
            {
                EndUserMessage = "Archived Modules Cannot Be Modified";
                return;
            }

            WinPeModule.Name = txtDisplayName.Text;
            WinPeModule.Description = txtDescription.Text;
            var result = Call.WinPeModuleApi.Put(WinPeModule.Id, WinPeModule);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Module {0}", WinPeModule.Name) : result.ErrorMessage;
        }



        protected void deleteModule_OnClick(object sender, EventArgs e)
        {
            var result = Call.WinPeModuleApi.Delete(WinPeModule.Id);
            if (result.Success)
            {
                EndUserMessage = String.Format("Successfully Deleted Module {0}", WinPeModule.Name);
                Response.Redirect("~/views/modules/winpemodules/search.aspx");
            }
            else
                EndUserMessage = result.ErrorMessage;
        }
    }
}