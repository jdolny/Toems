using System;
using Toems_Common;
using Toems_Common.Entity;
using Toems_FrontEnd.BasePages;

namespace Toems_FrontEnd.views.admin.wolrelays
{
    public partial class edit : Admin
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateForm();
        }

        protected void buttonUpdateRelay_OnClick(object sender, EventArgs e)
        {
            WolRelay.Gateway = txtGateway.Text;
            WolRelay.ComServerId = Convert.ToInt32(ddlComServer.SelectedValue);

            var result = Call.WolRelayApi.Put(WolRelay.Id, WolRelay);
            EndUserMessage = result.Success ? "Successfully Updated Relay" : result.ErrorMessage;
        }

        private void PopulateForm()
        {
            PopulateComServers(ddlComServer);

            txtGateway.Text = WolRelay.Gateway;
            ddlComServer.SelectedValue = WolRelay.ComServerId.ToString();
        }
    }
}