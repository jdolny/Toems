<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/comservers/comservers.master" AutoEventWireup="true" CodeBehind="tftpsettings.aspx.cs" Inherits="Toems_FrontEnd.views.admin.comservers.tftpsettings" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
     <li><a href="<%= ResolveUrl("~/views/admin/comservers/comservers.aspx") %>?level=2">Com Servers</a></li>
    <li><%=ComServer.DisplayName %></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
<%=ComServer.DisplayName %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
   <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Update Server" CssClass="main-action" /></li>
     <li><asp:LinkButton ID="btnCert" runat="server" OnClick="btnCert_Click" Text="Create Certificate" /></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#tftp').addClass("nav-current");
        });
    </script>
   

      <div class="size-4 column">
        TFTP Server:
    </div>
    <div class="size-5 column hidden-check">
        <asp:CheckBox runat="server" id="chkTftp" ClientIDMode="Static"/>
        <label for="chkTftp"></label>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Interface IP Address:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtIp" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Tftp Local path:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtPath" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>
      <div class="size-4 column">
        TFTP Information Server:
    </div>
    <div class="size-5 column hidden-check">
        <asp:CheckBox runat="server" id="chkTftpInformation" ClientIDMode="Static"/>
        <label for="chkTftpInformation"></label>
    </div>
    <br class="clear"/>
  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <h5><span style="color: #ff9900;">TFTP Server:</span></h5>
<p>Determines if this com server can be used as a tftp / pxe boot server for imaging computers.</p>
        <h5><span style="color: #ff9900;">Interface IP Address:</span></h5>
    <p>The ip address of the nic that will be used for tftp transfers.  This must be populated before pxe booting can work.</p>

       <h5><span style="color: #ff9900;">Tftp Local Path:</span></h5>
    <p>The location of Theopenem tftpboot directory.  Default value should be used in most cases.</p>

       <h5><span style="color: #ff9900;">Tftp Information Server:</span></h5>
    <p>Every cluster that will be used for imaging must have a tftp information server to provide the front end with info about kernels, images, and boot files.</p>
</asp:Content>
