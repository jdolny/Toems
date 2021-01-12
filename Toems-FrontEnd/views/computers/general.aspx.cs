using System;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.computers
{
    public partial class general : BasePages.Computers
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateForm();
            }
        }

        protected void PopulateForm()
        {
            lblName.Text = ComputerEntity.Name;
            lblAdSync.Text = ComputerEntity.IsAdSync.ToString();
            lblClientVersion.Text = ComputerEntity.ClientVersion;
            lblIdentifier.Text = ComputerEntity.Guid;
            lblAdGuid.Text = ComputerEntity.AdGuid;
            lblInstallId.Text = ComputerEntity.InstallationId;
            lblInventoryTime.Text = ComputerEntity.LastInventoryTime.ToString();
            lblLastCheckin.Text = ComputerEntity.LastCheckinTime.ToString();
            lblLastIp.Text = ComputerEntity.LastIp;
            lblProvisionDate.Text = ComputerEntity.ProvisionedTime.ToString();
            lblStatus.Text = ComputerEntity.ProvisionStatus.ToString();
            lblAdDisabled.Text = ComputerEntity.AdDisabled.ToString();
            lblRemoteAccessId.Text = ComputerEntity.RemoteAccessId;
            lblHardwareUuid.Text = ComputerEntity.UUID;
            if (!string.IsNullOrEmpty(ComputerEntity.RemoteAccessId))
            {
                var webRtc = Call.RemoteAccessApi.IsWebRtcEnabled(ComputerEntity.RemoteAccessId);
                if (webRtc != null)
                {
                    if (webRtc.Equals("true"))
                        chkWebRtc.Checked = true;
                    else
                        chkWebRtc.Checked = false;
                }
            }

        }

        protected void chkWebRtc_CheckedChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ComputerEntity.RemoteAccessId))
            {
                var webRtc = new DtoWebRtc();
                webRtc.DeviceId = ComputerEntity.RemoteAccessId;
                if (chkWebRtc.Checked)
                    webRtc.Mode = "1";
                else
                    webRtc.Mode = "2";
                var updateWebRtc = Call.RemoteAccessApi.UpdateWebRtc(webRtc);
                PopulateForm();
            }
           
        }
    }
}

