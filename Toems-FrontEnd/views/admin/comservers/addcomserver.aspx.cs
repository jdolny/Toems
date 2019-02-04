using System;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.admin.comservers
{
    public partial class addcomserver : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

         protected void buttonAdd_OnClick(object sender, EventArgs e)
        {
            var server = new EntityClientComServer()
            {
                DisplayName = txtName.Text,
                Url = txtUrl.Text,
                Description = txtDescription.Text,
                ReplicateStorage = true,
            };

            var result = Call.ClientComServerApi.Post(server);
            if (result.Success)
            {
                EndUserMessage = "Successfully Created Server";
                Response.Redirect("~/views/admin/comservers/editcomserver.aspx?level=2&serverId=" + result.Id);
            }
            else
            {
                EndUserMessage = result.ErrorMessage;
            }
        }
    }
    
}