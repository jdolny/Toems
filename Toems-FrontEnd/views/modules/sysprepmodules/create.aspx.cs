using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.modules.sysprepmodules
{
    public partial class create : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnAdd_OnClick(object sender, EventArgs e)
        {

            var module = new EntitySysprepModule()
            {
                Name = txtDisplayName.Text,
                Description = txtDescription.Text,
                OpeningTag = txtOpen.Text,
                ClosingTag = txtClose.Text,
                Contents = scriptEditor.Value,

            };


            var result = Call.SysprepModuleApi.Post(module);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created Sysprep Module";
                Response.Redirect("~/views/modules/sysprepmodules/general.aspx?sysprepModuleId=" + result.Id);
            }
        }

     
    }
}