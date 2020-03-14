using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.admin.pxeboot
{
    public partial class globalbootmenu : BasePages.Admin
    {
        public string biosLbl;
        public string efi32Lbl;
        public string efi64Lbl;
        public string noProxyLbl;

        public void btnSubmit_Click(object sender, EventArgs e)
        {
            if (GetSetting(SettingStrings.ProxyDhcpEnabled) == "Yes")
                CreateProxyMenu();
            else
                CreateStandardMenu();
            EndUserMessage = "Complete";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.PxeSettingsUpdate);
            if (!IsPostBack) PopulateForm();
            else
            {
                var biosFile = GetSetting(SettingStrings.ProxyBiosBootloader);
                biosLbl = biosFile;
                var efi32File = GetSetting(SettingStrings.ProxyEfi32Bootloader);
                efi32Lbl = efi32File;
                var efi64File = GetSetting(SettingStrings.ProxyEfi64Bootloader);
                efi64Lbl = efi64File;
                var pxeMode = GetSetting(SettingStrings.PxeBootloader);
                noProxyLbl = pxeMode;
            }
        }

        protected void CreateProxyMenu()
        {
            var listSettings = new List<EntitySetting>
            {
                new EntitySetting
                {
                    Name = SettingStrings.IpxeRequiresLogin,
                    Value = chkIpxeProxy.Checked.ToString(),
                    Id = Call.SettingApi.GetSetting(SettingStrings.IpxeRequiresLogin).Id
                }
            };

            Call.SettingApi.UpdatePxeSettings(listSettings);

            var defaultBootMenuOptions = new DtoBootMenuGenOptions()
            {
                DebugPwd = consoleShaProxy.Value,
                AddPwd = addcomputerShaProxy.Value,
                OndPwd = ondshaProxy.Value,
                DiagPwd = diagshaProxy.Value,
                GrubUserName = txtGrubProxyUsername.Text,
                GrubPassword = txtGrubProxyPassword.Text
            };

            defaultBootMenuOptions.BiosKernel = ddlBiosKernel.SelectedValue;
            defaultBootMenuOptions.BiosBootImage = ddlBiosBootImage.SelectedValue;


            defaultBootMenuOptions.Efi32Kernel = ddlEfi32Kernel.SelectedValue;
            defaultBootMenuOptions.Efi32BootImage = ddlEfi32BootImage.SelectedValue;

            defaultBootMenuOptions.Efi64Kernel = ddlEfi64Kernel.SelectedValue;
            defaultBootMenuOptions.Efi64BootImage = ddlEfi64BootImage.SelectedValue;

            Call.SettingApi.CreateDefaultBootMenu(defaultBootMenuOptions);
        }

        protected void CreateStandardMenu()
        {
            var listSettings = new List<EntitySetting>
            {
                new EntitySetting
                {
                    Name = SettingStrings.IpxeRequiresLogin,
                    Value = chkIpxeLogin.Checked.ToString(),
                    Id = Call.SettingApi.GetSetting(SettingStrings.IpxeRequiresLogin).Id
                }
            };

            Call.SettingApi.UpdatePxeSettings(listSettings);

            var defaultBootMenuOptions = new DtoBootMenuGenOptions();
            var pxeMode = GetSetting(SettingStrings.PxeBootloader);
            if (pxeMode.Contains("grub"))
            {
                defaultBootMenuOptions.GrubUserName = txtGrubUsername.Text;
                defaultBootMenuOptions.GrubPassword = txtGrubPassword.Text;
            }
            else
            {
                defaultBootMenuOptions.DebugPwd = consoleSha.Value;
                defaultBootMenuOptions.AddPwd = addcomputerSha.Value;
                defaultBootMenuOptions.OndPwd = ondsha.Value;
                defaultBootMenuOptions.DiagPwd = diagsha.Value;
            }
            defaultBootMenuOptions.Kernel = ddlComputerKernel.SelectedValue;
            defaultBootMenuOptions.BootImage = ddlComputerBootImage.SelectedValue;
            defaultBootMenuOptions.Type = "standard";
            Call.SettingApi.CreateDefaultBootMenu(defaultBootMenuOptions);
        }

        protected void PopulateForm()
        {
            var proxyDhcp = GetSetting(SettingStrings.ProxyDhcpEnabled);

            if (proxyDhcp == "Yes")
            {
                divProxyDHCP.Visible = true;
                btnSubmitDefaultProxy.Visible = true;
                btnSubmitDefault.Visible = false;
                var biosFile = GetSetting(SettingStrings.ProxyBiosBootloader);
                biosLbl = biosFile;
                var efi32File = GetSetting(SettingStrings.ProxyEfi32Bootloader);
                efi32Lbl = efi32File;
                var efi64File = GetSetting(SettingStrings.ProxyEfi64Bootloader);
                efi64Lbl = efi64File;
                if (biosFile.Contains("winpe"))
                {
                    divProxyBios.Visible = false;
                    lblBiosHidden.Text = "Bios Boot Menus Are Not Used When Proxy Bios Is Set To WinPE";
                    lblBiosHidden.Visible = true;
                }
                if (efi32File.Contains("winpe"))
                {
                    divProxyEfi32.Visible = false;
                    lblEfi32Hidden.Text = "Efi32 Boot Menus Are Not Used When Proxy Efi32 Is Set To WinPE";
                    lblEfi32Hidden.Visible = true;
                }
                if (efi64File.Contains("winpe"))
                {
                    divProxyEfi64.Visible = false;
                    lblEfi64Hidden.Text = "Efi64 Boot Menus Are Not Used When Proxy Efi64 Is Set To WinPE";
                    lblEfi64Hidden.Visible = true;
                }
                if (biosFile.Contains("linux") || efi32File.Contains("linux") || efi64File.Contains("linux"))
                    proxyPassBoxes.Visible = true;
                if (biosFile.Contains("ipxe") || efi32File.Contains("ipxe") || efi64File.Contains("ipxe"))
                {
                    ipxeProxyPasses.Visible = true;
                    chkIpxeProxy.Checked = Convert.ToBoolean(GetSetting(SettingStrings.IpxeRequiresLogin));
                }
                if (efi64File.Contains("grub"))
                    grubProxyPasses.Visible = true;

                var kernels = Call.FilesystemApi.GetKernels();
                ddlBiosKernel.DataSource = kernels;
                ddlBiosKernel.DataBind();
                ddlBiosKernel.SelectedValue = SettingStrings.DefaultKernel64;
                ddlEfi32Kernel.DataSource = kernels;
                ddlEfi32Kernel.DataBind();
                ddlEfi32Kernel.SelectedValue = SettingStrings.DefaultKernel32;
                ddlEfi64Kernel.DataSource = kernels;
                ddlEfi64Kernel.DataBind();
                ddlEfi64Kernel.SelectedValue = SettingStrings.DefaultKernel64;

                var bootImages = Call.FilesystemApi.GetBootImages();
                ddlBiosBootImage.DataSource = bootImages;
                ddlBiosBootImage.DataBind();
                ddlBiosBootImage.SelectedValue = SettingStrings.DefaultInit;
                ddlEfi32BootImage.DataSource = bootImages;
                ddlEfi32BootImage.DataBind();
                ddlEfi32BootImage.SelectedValue = SettingStrings.DefaultInit;
                ddlEfi64BootImage.DataSource = bootImages;
                ddlEfi64BootImage.DataBind();
                ddlEfi64BootImage.SelectedValue = SettingStrings.DefaultInit;
            }
            else
            {
                var pxeMode = GetSetting(SettingStrings.PxeBootloader);
                noProxyLbl = pxeMode;
                if (pxeMode.Contains("winpe"))
                {
                    lblNoMenu.Visible = true;
                    btnSubmitDefaultProxy.Visible = false;
                    btnSubmitDefault.Visible = false;
                    return;
                }

                btnSubmitDefaultProxy.Visible = false;
                btnSubmitDefault.Visible = true;
                bootPasswords.Visible = true;
                divStandardMode.Visible = true;

                if (pxeMode.Contains("ipxe"))
                {
                    passboxes.Visible = false;
                    grubPassBoxes.Visible = false;
                    ipxePassBoxes.Visible = true;
                    chkIpxeLogin.Checked = Convert.ToBoolean(GetSetting(SettingStrings.IpxeRequiresLogin));
                }
                else if (pxeMode.Contains("grub"))
                {
                    passboxes.Visible = false;
                    grubPassBoxes.Visible = true;
                    ipxePassBoxes.Visible = false;
                }

                ddlComputerKernel.DataSource = Call.FilesystemApi.GetKernels();
                ddlComputerKernel.DataBind();
                ddlComputerKernel.SelectedValue = SettingStrings.DefaultKernel64;

                ddlComputerBootImage.DataSource = Call.FilesystemApi.GetBootImages();
                ddlComputerBootImage.DataBind();
                ddlComputerBootImage.SelectedValue = SettingStrings.DefaultInit;
            }
        }
    }
}