using System;

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
            lblPushUrl.Text = ComputerEntity.PushUrl;
            lblAdDisabled.Text = ComputerEntity.AdDisabled.ToString();
        }

      


     
    }
}

