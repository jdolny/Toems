using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Toems_Common;
using Toems_Common.Entity;

namespace Toems_FrontEnd.views.admin.pxeboot
{
    public partial class pxesettings : BasePages.Admin
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.PxeSettingsUpdate);
            if (IsPostBack) return;

            ddlPxeBootloader.SelectedValue = GetSetting(SettingStrings.PxeBootloader);
            ddlProxyDhcp.SelectedValue = GetSetting(SettingStrings.ProxyDhcpEnabled);
            ddlProxyBios.SelectedValue = GetSetting(SettingStrings.ProxyBiosBootloader);
            ddlProxyEfi32.SelectedValue = GetSetting(SettingStrings.ProxyEfi32Bootloader);
            ddlProxyEfi64.SelectedValue = GetSetting(SettingStrings.ProxyEfi64Bootloader);

            //These require pxe boot menu or client iso to be recreated
            ViewState["pxeBootloader"] = ddlPxeBootloader.Text;
            ViewState["proxyDhcp"] = ddlProxyDhcp.SelectedValue;
            ViewState["proxyBios"] = ddlProxyBios.SelectedValue;
            ViewState["proxyEfi32"] = ddlProxyEfi32.SelectedValue;
            ViewState["proxyEfi64"] = ddlProxyEfi64.SelectedValue;

            ShowProxyMode();
        }

        protected void ShowProxyMode()
        {
            if (ddlProxyDhcp.Text == "No")
            {
                divProxy.Visible = false;
                divPxe.Visible = true;
            }
            else
            {
                divProxy.Visible = true;
                divPxe.Visible = false;
            }
        }

        protected void ddlProxyDhcp_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowProxyMode();
        }

        protected void buttonUpdate_OnClick(object sender, EventArgs e)
        {
            var listSettings = new List<EntitySetting>
            {
                new EntitySetting
                {
                    Name = SettingStrings.PxeBootloader,
                    Value = ddlPxeBootloader.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.PxeBootloader).Id
                },
                  new EntitySetting
                {
                    Name = SettingStrings.ProxyDhcpEnabled,
                    Value = ddlProxyDhcp.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.ProxyDhcpEnabled).Id
                },
                    new EntitySetting
                {
                    Name = SettingStrings.ProxyBiosBootloader,
                    Value = ddlProxyBios.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.ProxyBiosBootloader).Id
                },
                  new EntitySetting
                {
                    Name = SettingStrings.ProxyEfi32Bootloader,
                    Value = ddlProxyEfi32.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.ProxyEfi32Bootloader).Id
                },
                    new EntitySetting
                {
                    Name = SettingStrings.ProxyEfi64Bootloader,
                    Value = ddlProxyEfi64.Text,
                    Id = Call.SettingApi.GetSetting(SettingStrings.ProxyEfi64Bootloader).Id
                },
            };
           
            var result = Call.SettingApi.UpdatePxeSettings(listSettings);
            EndUserMessage = result
                ? "Successfully Updated Settings"
                : "Could Not Update Settings";

            var newBootMenu = false;
            if (!Call.SettingApi.CopyPxeBinaries())
            {
                EndUserMessage = "Updated Settings Successfully, But Could Not Copy PXE Binaries On One Or More Com Servers.  Check The Logs For More Info.";
                return;
            }

            if ((string)ViewState["proxyDhcp"] != ddlProxyDhcp.Text)
                newBootMenu = true;
            if ((string)ViewState["proxyBios"] != ddlProxyBios.Text)
                newBootMenu = true;
            if ((string)ViewState["proxyEfi32"] != ddlProxyEfi32.Text)
                newBootMenu = true;
            if ((string)ViewState["proxyEfi64"] != ddlProxyEfi64.Text)
                newBootMenu = true;
            if ((string)ViewState["pxeBootloader"] != ddlPxeBootloader.Text)
            {
                newBootMenu = true;
            }

            if (newBootMenu)
            {
                lblTitle.Text =
                    "Your Settings Changes Require A New PXE Boot File Be Created.  <br>Go There Now?";

                ClientScript.RegisterStartupScript(GetType(), "modalscript",
                    "$(function() {  var menuTop = document.getElementById('confirmbox'),body = document.body;classie.toggle(menuTop, 'confirm-box-outer-open'); });",
                    true);
                Session.Remove("Message");
            }
            else
            {
                EndUserMessage = "Successfully Updated PXE Settings";
            }

            //Update the viewstate
            ViewState["pxeBootloader"] = ddlPxeBootloader.Text;
            ViewState["proxyDhcp"] = ddlProxyDhcp.SelectedValue;
            ViewState["proxyBios"] = ddlProxyBios.SelectedValue;
            ViewState["proxyEfi32"] = ddlProxyEfi32.SelectedValue;
            ViewState["proxyEfi64"] = ddlProxyEfi64.SelectedValue;

            EndUserMessage = "Successfully Updated PXE Settings";
        }

        protected void OkButton_Click(object sender, EventArgs e)
        {
            EndUserMessage = "Successfully Updated PXE Settings";
            Response.Redirect("~/views/admin/pxeboot/globalbootmenu.aspx?level=2");
        }
    }
}