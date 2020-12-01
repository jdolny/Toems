<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/pxeboot/pxeboot.master" AutoEventWireup="true" CodeBehind="pxesettings.aspx.cs" Inherits="Toems_FrontEnd.views.admin.pxeboot.pxesettings" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
     <li><a href="<%= ResolveUrl("~/views/admin/pxeboot/pxesettings.aspx") %>?level=2">PXE Boot Settings</a></li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubNavTitle_Sub2" runat="server">

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub2" runat="server">

     <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Update PXE Settings" CssClass="main-action" /></li>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" runat="server">
     <script type="text/javascript">
        $(document).ready(function() {
            $('#pxesettings').addClass("nav-current");
        });
    </script>

      <div class="size-4 column">
        Using Proxy DHCP:
    </div>
    <div class="size-5 column">
       
        <asp:DropDownList ID="ddlProxyDhcp" runat="server" CssClass="ddlist" AutoPostBack="true" OnSelectedIndexChanged="ddlProxyDhcp_SelectedIndexChanged">

            <asp:ListItem>Yes</asp:ListItem>
            <asp:ListItem>No</asp:ListItem>
        </asp:DropDownList>
  
            </div>
    <br class="clear"/>
    <div id="divPxe" runat="server">
      <div class="size-4 column">
        PXE Bootloader:
    </div>
    <div class="size-5 column">
       
        <asp:DropDownList ID="ddlPxeBootloader" runat="server" CssClass="ddlist">

            <asp:ListItem>pxelinux</asp:ListItem>
            <asp:ListItem>ipxe</asp:ListItem>
            <asp:ListItem>syslinux_efi32</asp:ListItem>
            <asp:ListItem>syslinux_efi64</asp:ListItem>
            <asp:ListItem>ipxe_efi32</asp:ListItem>
            <asp:ListItem>ipxe_efi64</asp:ListItem>
            <asp:ListItem>ipxe_efi_snp32</asp:ListItem>
            <asp:ListItem>ipxe_efi_snp64</asp:ListItem>
            <asp:ListItem>ipxe_efi_snponly32</asp:ListItem>
            <asp:ListItem>ipxe_efi_snponly64</asp:ListItem>
            <asp:ListItem>grub</asp:ListItem>
            <asp:ListItem>winpe_bios32</asp:ListItem>
            <asp:ListItem>winpe_bios64</asp:ListItem>
            <asp:ListItem>winpe_efi32</asp:ListItem>
            <asp:ListItem>winpe_efi64</asp:ListItem>
        </asp:DropDownList>
  
            </div>
    <br class="clear"/>
        </div>

    <div id="divProxy" runat="server">
      <div class="size-4 column">
        Proxy Bios Bootloader:
    </div>
    <div class="size-5 column">
       
        <asp:DropDownList ID="ddlProxyBios" runat="server" CssClass="ddlist">
            <asp:ListItem>pxelinux</asp:ListItem>
            <asp:ListItem>ipxe</asp:ListItem>
            <asp:ListItem>winpe</asp:ListItem>
        </asp:DropDownList>
  
            </div>
    <br class="clear"/>


      <div class="size-4 column">
        Proxy Efi32 Bootloader:
    </div>
    <div class="size-5 column">
       
        <asp:DropDownList ID="ddlProxyEfi32" runat="server" CssClass="ddlist">
            <asp:ListItem>syslinux</asp:ListItem>
            <asp:ListItem>ipxe_efi</asp:ListItem>
            <asp:ListItem>ipxe_snp</asp:ListItem>
            <asp:ListItem>ipxe_snponly</asp:ListItem>
            <asp:ListItem>winpe_efi</asp:ListItem>
        </asp:DropDownList>
  
            </div>
    <br class="clear"/>

      <div class="size-4 column">
        Proxy Efi64 Bootloader:
    </div>
    <div class="size-5 column">
       
        <asp:DropDownList ID="ddlProxyEfi64" runat="server" CssClass="ddlist">
 <asp:ListItem>syslinux</asp:ListItem>
            <asp:ListItem>ipxe_efi</asp:ListItem>
            <asp:ListItem>ipxe_snp</asp:ListItem>
            <asp:ListItem>ipxe_snponly</asp:ListItem>
            <asp:ListItem>grub</asp:ListItem>
            <asp:ListItem>winpe_efi</asp:ListItem>
        </asp:DropDownList>
  
            </div>
    <br class="clear"/>
        </div>

     <div id="confirmbox" class="confirm-box-outer">
        <div class="confirm-box-inner">
            <h4>
                <asp:Label ID="lblTitle" runat="server" CssClass="modaltitle"></asp:Label>
            </h4>
            <div class="confirm-box-btns">
                <asp:LinkButton ID="OkButton" OnClick="OkButton_Click" runat="server" Text="Yes" CssClass="confirm_yes"/>
                <asp:LinkButton ID="CancelButton" runat="server" Text="No" CssClass="confirm_no"/>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="subsubHelp" runat="server">
    <p;>The pxe settings control which bootloader is globally applied to computers when pxe booting. If you are booting a mix of legacy bios and EFI computers, it is recommended to use ProxyDHCP. 
        When using ProxyDHCP, up to 3 different boot loaders can be utilized simultaneously to match the correct boot architecture. If not using ProxyDHCP, only one can be enabled at a time.</p>
</asp:Content>
