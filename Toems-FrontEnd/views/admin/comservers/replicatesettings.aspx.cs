using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.admin.comservers
{
    public partial class replicatesettings : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateForm();
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            var maxMbps = 0;
            if (!int.TryParse(txtMaxBitrate.Text, out maxMbps))
            {
                EndUserMessage = "Max Bitrate Not Valid";
                return;
            }
            ComServer.ReplicationRateIpg = Convert.ToInt32(txtMaxBitrate.Text);
            ComServer.ReplicationRateIpg = (ComServer.ReplicationRateIpg * 750);
            ComServer.ReplicateStorage = chkReplicateStorage.Checked;
            var result = Call.ClientComServerApi.Put(ComServer.Id, ComServer);
            EndUserMessage = result.Success ? "Successfully Updated Server" : result.ErrorMessage;
        }

        private void PopulateForm()
        {
            chkReplicateStorage.Checked = ComServer.ReplicateStorage;
            if (ComServer.ReplicationRateIpg != 0)
                txtMaxBitrate.Text = (ComServer.ReplicationRateIpg / 750).ToString();
            else
                txtMaxBitrate.Text = "0";
        }

        protected void btnCert_Click(object sender, EventArgs e)
        {
            var cert = Call.ClientComServerApi.GenerateCert(ComServer.Id);

            Response.Clear();
            var ms = new MemoryStream(cert);
            Response.ContentType = "application/x-x509-ca-cert";
            Response.AddHeader("content-disposition", "attachment;filename=ComServerWebCert.pfx");
            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();
        }
    }
}