<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" CodeBehind="imagingclient.aspx.cs" Inherits="Toems_FrontEnd.views.admin.ImagingClient" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Server</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Admin Settings
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnUpdateSettings" runat="server" Text="Update Server Settings" OnClick="btnUpdateSettings_OnClick" CssClass="main-action"/></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#imagingclient').addClass("nav-current");
        });
    </script>
    <div class="size-4 column">
        Global Imaging Arguments:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtArguments" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Enable iPXE SSL:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkIpxeSsl" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkIpxeSsl"></label>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        iPXE SSL Disabled Port:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtPort" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Imaging Task Timeout:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtImagingTimeout" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Registration Enabled:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlRegistration" runat="server" CssClass="ddlist" >
            <asp:ListItem>Yes</asp:ListItem>
            <asp:ListItem>No</asp:ListItem>
        </asp:DropDownList>
    </div>
      <br class="clear"/>
    

     <div class="size-4 column">
        Registration Disabled Keep Name Prompt:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlKeepNamePrompt" runat="server" CssClass="ddlist" >
            <asp:ListItem>Yes</asp:ListItem>
            <asp:ListItem>No</asp:ListItem>
        </asp:DropDownList>
    </div>

     <div class="size-4 column">
        Upload / Deploy Direct to SMB:
    </div>
      <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkDirectSMB" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkDirectSMB"></label>
    </div>
  
  

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <h5><span style="color: #ff9900;">Global Imaging Arguments:</span></h5>
    <p>Arguments that will be passed to every imaging client.</p>

    <h5><span style="color: #ff9900;">Enable iPXE SSL:</span></h5>
    <p>Enable if iPXE should connect over SSL.  Requires you to correctly setup certs.  By default iPXE doesn't trust certs unless they are embedded.  Typically this option should be off.</p>

     <h5><span style="color: #ff9900;">iPXE SSL Disabled Port:</span></h5>
    <p>If a com server has SSL enabled for everything except iPXE.  A port is needed in order to communicate with the com server that is not Https.</p>

     <h5><span style="color: #ff9900;">Imaging Task Timeout:</span></h5>
    <p>The time in minutes that an imaging task will auto close if not completed.</p>

     <h5><span style="color: #ff9900;">Registration Enabled:</span></h5>
    <p>When imaging a computer, it will check to see if a matching computer already exists in Theopenem.  If not, it will ask you to register the computer and will by added to the imaging only computers page.
        When a computer is registered, it will keep track of all imaging logs and auto rename it using the registered name.  If you don't want these features, then set this to no.
    </p>

     <h5><span style="color: #ff9900;">Registration Disabled Keep Name Prompt:</span></h5>
    <p>If registration is disabled but you still want a prompt to rename the computer during imaging then enable this option.</p>
</asp:Content>