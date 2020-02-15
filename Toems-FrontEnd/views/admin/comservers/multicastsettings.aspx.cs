using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.admin.comservers
{
    public partial class multicastsettings : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateForm();
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            ComServer.IsMulticastServer = chkMulticast.Checked;
            ComServer.MulticastInterfaceIp = txtIp.Text;
            ComServer.MulticastSenderArguments = txtSendArgs.Text;
            ComServer.MulticastReceiverArguments = txtRecArgs.Text;
            var startPort = 0;
            if (!int.TryParse(txtStartPort.Text, out startPort))
            {
                EndUserMessage = "Start Port Is Not Valid";
                return;
            }
            var endPort = 0;
            if (!int.TryParse(txtEndPort.Text, out endPort))
            {
                EndUserMessage = "End Port Not Valid";
                return;
            }
            if(startPort == 0)
            {
                EndUserMessage = "Start Port Is Not Valid";
                return;
            }
            if (endPort == 0)
            {
                EndUserMessage = "End Port Is Not Valid";
                return;
            }

            ComServer.MulticastStartPort = startPort;
            ComServer.MulticastEndPort = endPort;
            ComServer.DecompressImageOn = ddlDecompress.Text;
            var result = Call.ClientComServerApi.Put(ComServer.Id, ComServer);
            EndUserMessage = result.Success ? "Successfully Updated Server" : result.ErrorMessage;
        }

        private void PopulateForm()
        {
            chkMulticast.Checked = ComServer.IsMulticastServer;
            txtIp.Text = ComServer.MulticastInterfaceIp;
            txtSendArgs.Text = ComServer.MulticastSenderArguments;
            txtRecArgs.Text = ComServer.MulticastReceiverArguments;
            txtStartPort.Text = ComServer.MulticastStartPort.ToString();
            txtEndPort.Text = ComServer.MulticastEndPort.ToString();
            ddlDecompress.Text = ComServer.DecompressImageOn;
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