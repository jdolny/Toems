using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.modules.messagemodules
{
    public partial class create : BasePages.Modules
    {
        protected void Page_Load(object sender, EventArgs e)
        {


        }

        protected void btnAdd_OnClick(object sender, EventArgs e)
        {
            var module = new EntityMessageModule()
            {
                Name = txtDisplayName.Text,
                Description = txtDescription.Text,
                Title = txtTitle.Text,
                Message = txtMessage.Text,
                Timeout = Convert.ToInt32(txtTimeout.Text),
            };


            var result = Call.MessageModuleApi.Post(module);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created Message Module";
                Response.Redirect("~/views/modules/messagemodules/general.aspx?messageModuleId=" + result.Id);
            }
        }
    }
}