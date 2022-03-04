using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Dto;

namespace Toems_FrontEnd.views.admin.pxeboot
{
    public partial class bootmenueditor : BasePages.Admin
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateForm();
        }

        protected void ddlComServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateForm();
        }
        protected void PopulateForm()
        {
            if (!IsPostBack)
                PopulateComServers(ddlComServer);

            var comServerId = Convert.ToInt32(ddlComServer.SelectedValue);
            var path = Call.ClientComServerApi.GetDefaultBootFilePath(ddlEditProxyType.Text, comServerId);

            var proxyDhcp = GetSetting(SettingStrings.ProxyDhcpEnabled);

            if (proxyDhcp == "Yes")
            {
                btnSaveEditor.Visible = true;

                srvEdit.Visible = true;

                var biosFile = GetSetting(SettingStrings.ProxyBiosBootloader);
                var efi32File = GetSetting(SettingStrings.ProxyEfi32Bootloader);
                var efi64File = GetSetting(SettingStrings.ProxyEfi64Bootloader);
                proxyEditor.Visible = true;
                if (ddlEditProxyType.Text == "bios")
                {
                    if (biosFile.Contains("winpe"))
                    {
                        btnSaveEditor.Visible = false;
                        lblFileName1.Text = "Bios Boot Menus Are Not Used With WinPE";

                        srvEdit.Visible = false;
                        return;
                    }
                }

                if (ddlEditProxyType.Text == "efi32")
                {
                    if (efi32File.Contains("winpe"))
                    {
                        btnSaveEditor.Visible = false;
                        lblFileName1.Text = "Efi32 Boot Menus Are Not Used With WinPE";

                        srvEdit.Visible = false;
                        return;
                    }
                }

                if (ddlEditProxyType.Text == "efi64")
                {
                    if (efi64File.Contains("winpe"))
                    {
                        btnSaveEditor.Visible = false;
                        lblFileName1.Text = "Efi64 Boot Menus Are Not Used With WinPE";

                        srvEdit.Visible = false;
                        return;
                    }
                }
            }
            else
            {
                var mode = GetSetting(SettingStrings.PxeBootloader);
                proxyEditor.Visible = false;
                if (mode.Contains("winpe"))
                {
                    btnSaveEditor.Visible = false;
                    lblFileName1.Text = "Boot Menus Are Not Used With WinPE";

                    srvEdit.Visible = false;
                    return;
                }

            }
            lblFileName1.Text = path;
            try
            {
                if (path != null) scriptEditorText.Value = Call.ClientComServerApi.ReadFileText(path, comServerId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        protected void EditProxy_Changed(object sender, EventArgs e)
        {
            var comServerId = Convert.ToInt32(ddlComServer.SelectedValue);
            var path = Call.ClientComServerApi.GetDefaultBootFilePath(ddlEditProxyType.Text, comServerId);
            var proxyDhcp = GetSetting(SettingStrings.ProxyDhcpEnabled);

            if (proxyDhcp == "Yes")
            {
                btnSaveEditor.Visible = true;

                srvEdit.Visible = true;

                var biosFile = GetSetting(SettingStrings.ProxyBiosBootloader);
                var efi32File = GetSetting(SettingStrings.ProxyEfi32Bootloader);
                var efi64File = GetSetting(SettingStrings.ProxyEfi64Bootloader);
                proxyEditor.Visible = true;
                if (ddlEditProxyType.Text == "bios")
                {
                    if (biosFile.Contains("winpe"))
                    {
                        btnSaveEditor.Visible = false;
                        lblFileName1.Text = "Bios Boot Menus Are Not Used With WinPE";

                        srvEdit.Visible = false;
                        return;
                    }
                }

                if (ddlEditProxyType.Text == "efi32")
                {
                    if (efi32File.Contains("winpe"))
                    {
                        btnSaveEditor.Visible = false;
                        lblFileName1.Text = "Efi32 Boot Menus Are Not Used With WinPE";

                        srvEdit.Visible = false;
                        return;
                    }
                }

                if (ddlEditProxyType.Text == "efi64")
                {
                    if (efi64File.Contains("winpe"))
                    {
                        btnSaveEditor.Visible = false;
                        lblFileName1.Text = "Efi64 Boot Menus Are Not Used With WinPE";

                        srvEdit.Visible = false;
                        return;
                    }
                }
            }
            else
            {
                proxyEditor.Visible = false;
            }
            lblFileName1.Text = path;
            try
            {
                if (path != null) scriptEditorText.Value = Call.ClientComServerApi.ReadFileText(path, comServerId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        protected void saveEditor_Click(object sender, EventArgs e)
        {
            var menu = new DtoCoreScript();
            menu.Name = ddlEditProxyType.Text;
            menu.Contents = scriptEditorText.Value;
            menu.ComServerId = Convert.ToInt32(ddlComServer.SelectedValue);
            EndUserMessage = Call.ClientComServerApi.EditDefaultBootMenu(menu) ? "Success" : "Could Not Save Boot Menu";
        }
    }
}
