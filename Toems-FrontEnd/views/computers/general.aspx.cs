using System;
using Toems_Common;
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

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if(ComputerEntity.ProvisionStatus != Toems_Common.Enum.EnumProvisionStatus.Status.ImageOnly && (txtName.Text != ComputerEntity.Name || txtImagingMac.Text != ComputerEntity.ImagingMac))
            {
                EndUserMessage = "Only \"Image Only\" Computers Can Have Their Name Or Mac Updated.";
                return;
            }


            ComputerEntity.Name = txtName.Text;
            ComputerEntity.Description = txtDesc.Text;
            ComputerEntity.ImagingMac = Utility.FixMac(txtImagingMac.Text);
            ComputerEntity.ImagingClientId = Utility.FixMac(txtImagingId.Text);

            var result = Call.ComputerApi.Put(ComputerEntity.Id, ComputerEntity);
            EndUserMessage = result.Success ? String.Format("Successfully Updated Computer {0}", ComputerEntity.Name) : result.ErrorMessage;

            PopulateForm();
        }

        protected void PopulateForm()
        {
            txtName.Text = ComputerEntity.Name;
            txtDesc.Text = ComputerEntity.Description;
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
            txtImagingId.Text = ComputerEntity.ImagingClientId;
            txtImagingMac.Text = ComputerEntity.ImagingMac;


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

