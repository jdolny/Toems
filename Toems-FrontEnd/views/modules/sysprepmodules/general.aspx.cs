using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.modules.sysprepmodules
{
    public partial class general : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
        
            txtDisplayName.Text = SysprepModule.Name;
            txtGuid.Text = SysprepModule.Guid;
            txtGuid.ReadOnly = true;
            txtOpen.Text = SysprepModule.OpeningTag;
            txtClose.Text = SysprepModule.ClosingTag;
            txtDescription.Text = SysprepModule.Description;
           
            scriptEditor.Value = SysprepModule.Contents;
           
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            if (SysprepModule.Archived)
            {
                EndUserMessage = "Archived Modules Cannot Be Modified";
                return;
            }
          

            SysprepModule.Name = txtDisplayName.Text;
            SysprepModule.Description = txtDescription.Text;
            SysprepModule.OpeningTag = txtOpen.Text;
            SysprepModule.ClosingTag = txtClose.Text;
            SysprepModule.Contents = scriptEditor.Value;
         
            var result = Call.SysprepModuleApi.Put(SysprepModule.Id, SysprepModule);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Module {0}", SysprepModule.Name) : result.ErrorMessage;
        }



        protected void deleteModule_OnClick(object sender, EventArgs e)
        {
            var result = Call.SysprepModuleApi.Delete(SysprepModule.Id);
            if (result.Success)
            {
                EndUserMessage = String.Format("Successfully Deleted Module {0}", SysprepModule.Name);
                Response.Redirect("~/views/modules/sysprepmodules/search.aspx");
            }
            else
                EndUserMessage = result.ErrorMessage;
        }

    

    }
}