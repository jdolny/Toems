using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.modules.winpemodules
{
    public partial class create : BasePages.Modules
    {
        protected void btnAdd_OnClick(object sender, EventArgs e)
        {
            var module = new EntityWinPeModule()
            {
                Name = txtDisplayName.Text,
                Description = txtDescription.Text,
            };


            var result = Call.WinPeModuleApi.Post(module);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created WinPE Module";
                Response.Redirect("~/views/modules/winpemodules/general.aspx?winPeModuleId=" + result.Id);
            }
        }
    }
}