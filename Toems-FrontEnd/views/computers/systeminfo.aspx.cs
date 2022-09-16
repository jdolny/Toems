using System;
using System.Globalization;

namespace Toems_FrontEnd.views.computers
{
    public partial class systeminfo : BasePages.Computers
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateForm();
            }
        }

        protected void PopulateForm()
        {
            var sysInfo = Call.ComputerApi.GetSystemInfo(ComputerEntity.Id);
            if (sysInfo == null) return;

            if (sysInfo.ComputerSystem != null)
            {
                lblManufacturer.Text = sysInfo.ComputerSystem.Manufacturer;
                lblModel.Text = sysInfo.ComputerSystem.Model;
                lblDomain.Text = sysInfo.ComputerSystem.Domain;
                lblWorkgroup.Text = sysInfo.ComputerSystem.Workgroup;
                lblMemory.Text = sysInfo.ComputerSystem.Memory.ToString();
                lblGpu.Text = sysInfo.ComputerSystem.Gpu;
                lblName.Text = sysInfo.ComputerSystem.Name;
            }
            if (sysInfo.Os != null)
            {
                lblOsName.Text = sysInfo.Os.Caption;
                lblOsVersion.Text = sysInfo.Os.Version;
                lblOsBuild.Text = sysInfo.Os.BuildNumber;
                lblOsArch.Text = sysInfo.Os.OSArchitecture;
                lblOsSpMajor.Text = sysInfo.Os.SpMajor.ToString();
                lblOsSpMinor.Text = sysInfo.Os.SpMinor.ToString();
                lblReleaseId.Text = sysInfo.Os.ReleaseId;
                lblTimeZone.Text = sysInfo.Os.LocalTimeZone;
                lblUac.Text = sysInfo.Os.UacStatus;
                lblLocation.Text = sysInfo.Os.LocationEnabled.ToString();
                lblLatitude.Text = sysInfo.Os.Latitude;
                lblLongitude.Text = sysInfo.Os.Longitude;
                Latitude = sysInfo.Os.Latitude;
                Longitude = sysInfo.Os.Longitude;
                lblUpdateServer.Text = sysInfo.Os.UpdateServer;
                lblTargetGroup.Text = sysInfo.Os.SUStargetGroup;
                lblLastLocationUpdate.Text = sysInfo.Os.LastLocationUpdateUtc.ToString(CultureInfo.InvariantCulture);
            }

            if (sysInfo.Bios != null)
            {
                lblBiosSerial.Text = sysInfo.Bios.SerialNumber;
                lblBiosVersion.Text = sysInfo.Bios.Version;
                lblSmBiosVersion.Text = sysInfo.Bios.SMBIOSBIOSVersion;
            }
            if (sysInfo.Processor != null)
            {
                lblProcessor.Text = sysInfo.Processor.Name;
                lblSpeed.Text = sysInfo.Processor.Speed.ToString();
                lblCores.Text = sysInfo.Processor.Cores.ToString();
            }

            if (sysInfo.Firewall != null)
            {
                lblDomainFirewall.Text = sysInfo.Firewall.DomainEnabled.ToString();
                lblPrivateFirewall.Text = sysInfo.Firewall.PrivateEnabled.ToString();
                lblPublicFirewall.Text = sysInfo.Firewall.PublicEnabled.ToString();
            }

            gvHds.DataSource = sysInfo.HardDrives;
            gvHds.DataBind();

            gvLogicalVolumes.DataSource = sysInfo.LogicalVolume;
            gvLogicalVolumes.DataBind();

            gvNics.DataSource = sysInfo.Nics;
            gvNics.DataBind();

            gvPrinters.DataSource = sysInfo.Printers;
            gvPrinters.DataBind();

            gvAntivirus.DataSource = sysInfo.AntiVirus;
            gvAntivirus.DataBind();

            gvBitlocker.DataSource = sysInfo.Bitlocker;
            gvBitlocker.DataBind();

        }

       

    }
}