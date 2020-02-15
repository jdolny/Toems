using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.admin.comservers
{
    public partial class tftpsettings : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateForm();
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            ComServer.IsTftpServer = chkTftp.Checked;
            ComServer.TftpInterfaceIp = txtIp.Text;
            ComServer.TftpPath = txtPath.Text;
            ComServer.IsTftpInfoServer = chkTftpInformation.Checked;
            var result = Call.ClientComServerApi.Put(ComServer.Id, ComServer);
            EndUserMessage = result.Success ? "Successfully Updated Server" : result.ErrorMessage;
        }

        private void PopulateForm()
        {
            chkTftp.Checked = ComServer.IsTftpServer;
            txtIp.Text = ComServer.TftpInterfaceIp;
            txtPath.Text = ComServer.TftpPath;
            chkTftpInformation.Checked = ComServer.IsTftpInfoServer;
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