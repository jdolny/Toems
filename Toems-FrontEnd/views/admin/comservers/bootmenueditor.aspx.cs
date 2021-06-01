using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Toems_FrontEnd.views.admin.comservers
{
    public partial class bootmenueditor : BasePages.Admin
    {
       /* protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateForm();
        }

        protected void PopulateForm()
        {
            var path = Call.FilesystemApi.GetDefaultBootFilePath(ddlEditProxyType.Text);

            var proxyDhcp = GetSetting(SettingStrings.ProxyDhcp);

            if (proxyDhcp == "Yes")
            {
                btnSaveEditor.Visible = true;

                srvEdit.Visible = true;

                var biosFile = GetSetting(SettingStrings.ProxyBiosFile);
                var efi32File = GetSetting(SettingStrings.ProxyEfi32File);
                var efi64File = GetSetting(SettingStrings.ProxyEfi64File);
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
                var mode = GetSetting(SettingStrings.PxeMode);
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
                if (path != null) scriptEditorText.Value = Call.FilesystemApi.ReadFileText(path);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        protected void EditProxy_Changed(object sender, EventArgs e)
        {
            var path = Call.FilesystemApi.GetDefaultBootFilePath(ddlEditProxyType.Text);
            var proxyDhcp = GetSetting(SettingStrings.ProxyDhcp);

            if (proxyDhcp == "Yes")
            {
                btnSaveEditor.Visible = true;

                srvEdit.Visible = true;

                var biosFile = GetSetting(SettingStrings.ProxyBiosFile);
                var efi32File = GetSetting(SettingStrings.ProxyEfi32File);
                var efi64File = GetSetting(SettingStrings.ProxyEfi64File);
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
                if (path != null) scriptEditorText.Value = Call.FilesystemApi.ReadFileText(path);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }*/
    }
}