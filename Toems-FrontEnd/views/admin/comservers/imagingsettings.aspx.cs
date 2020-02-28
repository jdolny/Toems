using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.admin.comservers
{
    public partial class imagingsettings : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateForm();
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            ComServer.IsImagingServer = chkImagingServer.Checked;

            var maxBps = 0;
            if (!int.TryParse(txtMaxBitrate.Text, out maxBps))
            {
                EndUserMessage = "Max Bitrate Not Valid";
                return;
            }

            var maxClient = 0;
            if (!int.TryParse(txtMaxClients.Text, out maxClient))
            {
                EndUserMessage = "Max Client Connections Not Valid";
                return;
            }

            ComServer.ImagingMaxBps = Convert.ToInt32(txtMaxBitrate.Text);
            ComServer.ImagingMaxBps = (ComServer.ImagingMaxBps * 125);
            ComServer.ImagingMaxClients = Convert.ToInt32(txtMaxClients.Text);

            var result = Call.ClientComServerApi.Put(ComServer.Id, ComServer);
            EndUserMessage = result.Success ? "Successfully Updated Server" : result.ErrorMessage;
        }

        private void PopulateForm()
        {
            chkImagingServer.Checked = ComServer.IsImagingServer;
            if (ComServer.ImagingMaxBps != 0)
                txtMaxBitrate.Text = (ComServer.ImagingMaxBps / 125).ToString();
            else
                txtMaxBitrate.Text = "0";
            txtMaxClients.Text = ComServer.ImagingMaxClients.ToString();
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