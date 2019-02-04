using System;
using Toems_Common;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.admin.wolrelays
{
    public partial class create : Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                PopulateComServers(ddlComServer);
        }

        protected void buttonAddRelay_OnClick(object sender, EventArgs e)
        {
            var wolRelay = new EntityWolRelay()
            {
                Gateway = txtGateway.Text,
                ComServerId = Convert.ToInt32(ddlComServer.SelectedValue)
            };

            var result = Call.WolRelayApi.Post(wolRelay);
            if (result.Success)
            {
                EndUserMessage = "Successfully Created WOL Relay";
                Response.Redirect("~/views/admin/wolrelays/edit.aspx?level=2&relayId=" + result.Id);
            }
            else
            {
                EndUserMessage = result.ErrorMessage;
            }
        }
    }
}