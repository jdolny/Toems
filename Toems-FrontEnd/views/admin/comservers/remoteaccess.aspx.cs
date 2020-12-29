using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.admin.comservers
{
    public partial class remoteaccess : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateForm();
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            ComServer.RemoteAccessUrl = txtUrl.Text;
            var result = Call.ClientComServerApi.Put(ComServer.Id, ComServer);
            EndUserMessage = result.Success ? "Successfully Updated Server" : result.ErrorMessage;
        }

        private void PopulateForm()
        {
            chkRemote.Checked = ComServer.IsRemoteAccessServer;
            txtUrl.Text = ComServer.RemoteAccessUrl;
        }

        protected void btnInitialize_Click(object sender, EventArgs e)
        {
            var result = Call.RemoteAccessApi.InitializeRemotelyServer(ComServer.Id);
            if(result == null)
            {
                EndUserMessage = "Could Not Initialize Remote Access Server.  Check Logs For More Info.";
                return;
            }
            EndUserMessage = result.Success ? "Successfully Initialized Remote Access Server" : result.ErrorMessage;
        }

        protected void btnCopyFiles_Click(object sender, EventArgs e)
        {
            var result = Call.RemoteAccessApi.CopyRemotelyInstallerToStorage();
            if (result == null)
            {
                EndUserMessage = "Could Not Copy Files.  Check Logs For More Info.";
                return;
            }
            EndUserMessage = result.Success ? "Successfully Copied Files" : result.ErrorMessage;
        }

        protected void chkRemote_CheckedChanged(object sender, EventArgs e)
        {
            if(chkRemote.Checked)
            {
                var currentCount = Call.RemoteAccessApi.GetRemoteAccessCount();
                if(currentCount > 0)
                {
                    EndUserMessage = "There Can Only Be One Remote Access Server.";
                    return;
                }

                var result = Call.RemoteAccessApi.VerifyRemoteAccessInstalled(ComServer.Id);
                if (!result)
                    EndUserMessage = "Could Not Enable Remote Access For This Com Server.  The Remotely Module Has Not Been Installed.";
                else
                {
                    ComServer.IsRemoteAccessServer = true;
                    Call.ClientComServerApi.Put(ComServer.Id, ComServer);
                }
            }
            else
            {
                ComServer.IsRemoteAccessServer = false;
                Call.ClientComServerApi.Put(ComServer.Id, ComServer);
            }
        }

        protected void btnCert_Click(object sender, EventArgs e)
        {
            var cert = Call.ClientComServerApi.GenerateRemoteAccessCert(ComServer.Id);

            Response.Clear();
            var ms = new MemoryStream(cert);
            Response.ContentType = "application/x-x509-ca-cert";
            Response.AddHeader("content-disposition", "attachment;filename=RemoteAccessWebCert.pfx");
            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();
        }

        protected void btnHealthCheck_Click(object sender, EventArgs e)
        {
            var result = Call.RemoteAccessApi.HealthCheck();
            if (result == null)
            {
                EndUserMessage = "Could Not Run Health Check.  Check The Exception Logs.";
                return;
            }
            EndUserMessage = result.Success ? "All Checks Passed" : result.ErrorMessage;
        }
    }
}