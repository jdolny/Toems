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
            }
            EndUserMessage = result.Success ? "Successfully Initialized Remote Access Server" : result.ErrorMessage;
        }

        protected void chkRemote_CheckedChanged(object sender, EventArgs e)
        {
            if(chkRemote.Checked)
            {
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
    }
}