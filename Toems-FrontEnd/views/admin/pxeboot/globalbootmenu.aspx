<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/pxeboot/pxeboot.master" AutoEventWireup="true" CodeBehind="globalbootmenu.aspx.cs" Inherits="Toems_FrontEnd.views.admin.pxeboot.globalbootmenu" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
    <li><a href="<%= ResolveUrl("~/views/admin/pxeboot/globalbootmenu.aspx") %>?level=2">Global Boot Menu</a></li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubNavTitle_Sub2" runat="server">
  Global Boot Menu
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub2" runat="server">
         <li><asp:LinkButton ID="btnSubmitDefault" runat="server" Text="Create Boot Files " OnClick="btnSubmit_Click" OnClientClick="get_shas();" CssClass="main-action"/></li>
    <li><asp:LinkButton ID="btnSubmitDefaultProxy" runat="server" Text="Create Boot Files " OnClick="btnSubmit_Click" OnClientClick="get_shas_proxy();" CssClass="main-action"/></li>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" runat="server">
      <script type="text/javascript">
        $(document).ready(function() {
            $('#bootmenu').addClass("nav-current");
          });

        function get_shas_proxy() {
        $('#<%= consoleShaProxy.ClientID %>').val(syslinux_sha512(document.getElementById('<%= txtProxDebugPwd.ClientID %>').value));
        $('#<%= ondshaProxy.ClientID %>').val(syslinux_sha512(document.getElementById('<%= txtProxOndPwd.ClientID %>').value));
    }

    function get_shas() {
        $('#<%= consoleSha.ClientID %>').val(syslinux_sha512(document.getElementById('<%= txtDebugPwd.ClientID %>').value));  
        $('#<%= ondsha.ClientID %>').val(syslinux_sha512(document.getElementById('<%= txtOndPwd.ClientID %>').value));
    }
    </script>
<asp:HiddenField ID="consoleSha" runat="server"/>
<asp:HiddenField ID="addcomputerSha" runat="server"/>
<asp:HiddenField ID="ondsha" runat="server"/>
<asp:HiddenField ID="diagsha" runat="server"/>
<asp:HiddenField ID="consoleShaProxy" runat="server"/>
<asp:HiddenField ID="addcomputerShaProxy" runat="server"/>
<asp:HiddenField ID="ondshaProxy" runat="server"/>
<asp:HiddenField ID="diagshaProxy" runat="server"/>



<asp:Label ID="lblNoMenu" runat="server" Visible="False" Text="Boot Menus Are Not Used When Proxy DHCP Is Set To No And The PXE Mode Is Set To WinPE"></asp:Label>
<div id="divStandardMode" runat="server" visible="false">
    <div id="bootPasswords" runat="server" visible="false" style="margin-top: 0;">
        <div class="size-4 column">
            <h2>PXE Mode - <%= noProxyLbl %></h2>
        </div>
        <br class="clear"/>
        <div class="size-4 column">
            Kernel:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlComputerKernel" runat="server" CssClass="ddlist">
            </asp:DropDownList>
        </div>
        <br class="clear"/>
        <div class="size-4 column">
            Boot Image:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlComputerBootImage" runat="server" CssClass="ddlist">
            </asp:DropDownList>
        </div>
        <br class="clear"/>
        <div id="passboxes" runat="server">
            <br/>
            <h2>Boot Menu Passwords - Optional</h2>
              <div class="size-4 column">
                Theopenem Password:
            </div>
            <div class="size-5 column">
                <asp:TextBox ID="txtOndPwd" runat="server" CssClass="textbox" type="password"></asp:TextBox>
            </div>
            <br class="clear"/>

            <div class="size-4 column">
                Client Console Password:
            </div>
            <div class="size-5 column">
                <asp:TextBox ID="txtDebugPwd" runat="server" CssClass="textbox" type="password"></asp:TextBox>
            </div>
            <br class="clear"/>
           
          
          
        </div>
        <div id="ipxePassBoxes" runat="server" visible="false" style="margin-top: 0;">
            <br/>
            <h2>Boot Menu Passwords - Optional</h2>
            <div class="size-4 column">
                iPXE Requires Login?:
            </div>
            <div class="size-5 column">
                <asp:CheckBox ID="chkIpxeLogin" runat="server"></asp:CheckBox>
            </div>
            <br class="clear"/>

        </div>
        <div id="grubPassBoxes" runat="server" visible="false" style="margin-top: 0;">
            <br/>
            <h2>Boot Menu Passwords - Optional</h2>
            <div class="size-4 column">
                Username:
            </div>
            <div class="size-5 column">
                <asp:TextBox ID="txtGrubUsername" runat="server" CssClass="textbox"></asp:TextBox>
            </div>
            <br class="clear"/>
            <div class="size-4 column">
                Password:
            </div>
            <div class="size-5 column">
                <asp:TextBox ID="txtGrubPassword" runat="server" CssClass="textbox" type="password"></asp:TextBox>
            </div>
            <br class="clear"/>
        </div>

    </div>

</div>

<div id="divProxyDHCP" runat="server" visible="false">
    <div class="size-4 column">
        <h2>BIOS - <%= biosLbl %></h2>
    </div>
    <br class="clear"/>
    <asp:Label runat="server" id="lblBiosHidden" Visible="False"></asp:Label>
    <div id="divProxyBios" runat="server">
        <div class="size-4 column">
            Kernel:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlBiosKernel" runat="server" CssClass="ddlist">
            </asp:DropDownList>
        </div>

        <br class="clear"/>
        <div class="size-4 column">
            Boot Image:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlBiosBootImage" runat="server" CssClass="ddlist">
            </asp:DropDownList>
        </div>
    </div>
    <br class="clear"/>

    <div class="size-4 column">
        <h2>EFI32 - <%= efi32Lbl %></h2>
    </div>
    <br class="clear"/>
    <asp:Label runat="server" id="lblEfi32Hidden" Visible="False"></asp:Label>
    <div id="divProxyEfi32" runat="server">
        <div class="size-4 column">
            Kernel:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlEfi32Kernel" runat="server" CssClass="ddlist">
            </asp:DropDownList>
        </div>
        <br class="clear"/>
        <div class="size-4 column">
            Boot Image:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlEfi32BootImage" runat="server" CssClass="ddlist">
            </asp:DropDownList>
        </div>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        <h2>EFI64 - <%= efi64Lbl %></h2>
    </div>
    <br class="clear"/>
    <asp:Label runat="server" id="lblEfi64Hidden" Visible="False"></asp:Label>
    <div id="divProxyEfi64" runat="server">
        <div class="size-4 column">
            Kernel:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlEfi64Kernel" runat="server" CssClass="ddlist">
            </asp:DropDownList>
        </div>

        <br class="clear"/>
        <div class="size-4 column">
            Boot Image:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlEfi64BootImage" runat="server" CssClass="ddlist">
            </asp:DropDownList>
        </div>
    </div>
    <br class="clear"/>
    <br/>
    <div id="proxyPassBoxes" runat="server" visible="false" style="margin-top: 20px;">
        <div class="size-1 column">
            Optional Boot Menu Passwords Can Be Assigned Below For Each Active Bootloader.
        </div>
        <br class="clear"/>
        <br/>
        <h2>PXELinux Boot Menu Passwords</h2>
        
          <div class="size-4 column">
            Theopenem Password:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtProxOndPwd" runat="server" CssClass="textbox" type="password"></asp:TextBox>
        </div>
        <br class="clear"/>

        <div class="size-4 column">
            Client Console Password:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtProxDebugPwd" runat="server" CssClass="textbox" type="password"></asp:TextBox>
        </div>
        <br class="clear"/>
      
      
       
    </div>
    <div id="ipxeProxyPasses" runat="server" visible="false" style="margin-top: 0;">
        <br/>
        <h2>iPXE Boot Menu Passwords</h2>
        <div class="size-4 column">
            iPXE Requires Login?
        </div>
        <div class="size-5 column">
            <asp:CheckBox ID="chkIpxeProxy" runat="server"></asp:CheckBox>
        </div>
        <br class="clear"/>

    </div>
    <div id="grubProxyPasses" runat="server" visible="false" style="margin-top: 0;">
        <br/>
        <h2>Grub Boot Menu Passwords</h2>
        <div class="size-4 column">
            Username:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtGrubProxyUsername" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
        <br class="clear"/>
        <div class="size-4 column">
            Password:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtGrubProxyPassword" runat="server" CssClass="textbox" type="password"></asp:TextBox>
        </div>
        <br class="clear"/>
    </div>
</div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="subsubHelp" runat="server">
    This is where you create the Global Default Boot Menu used for PXE booting. This the menu that client computers will see when pxe booted. It only applies to the Linux Imaging Environment. 
    Also remember this menu is loaded only when an active task has not been created for a computer. Additionally you can set boot menu passwords here.
</asp:Content>
