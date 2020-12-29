<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/comservers/comservers.master" AutoEventWireup="true" CodeBehind="remoteaccess.aspx.cs" Inherits="Toems_FrontEnd.views.admin.comservers.remoteaccess" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
     <li><a href="<%= ResolveUrl("~/views/admin/comservers/comservers.aspx") %>?level=2">Com Servers</a></li>
    <li><%=ComServer.DisplayName %></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
<%=ComServer.DisplayName %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
   <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Update Server" CssClass="main-action" /></li>
     <li><asp:LinkButton ID="btnInitialize" runat="server" OnClick="btnInitialize_Click" Text="Initialize Remote Access" OnClientClick="initialize();" /></li>
     <li><asp:LinkButton ID="btnCopyFiles" runat="server" OnClick="btnCopyFiles_Click" Text="Copy Remote Access Files" OnClientClick="copy();"/></li>
         <li><asp:LinkButton ID="btnCert" runat="server" OnClick="btnCert_Click" Text="Create Remote Access Certificate" /></li>
     <li><asp:LinkButton ID="btnHealthCheck" runat="server" OnClick="btnHealthCheck_Click" Text="Run Health Check" /></li>
</asp:Content>
         
<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
         function initialize() {
              Swal.fire({
                  title: 'Please Wait',
                  text: 'Initializing Remote Access Server',
                  footer: 'This box will close automatically when ready.'
              })
        }

          function copy() {
              Swal.fire({
                  title: 'Please Wait',
                  text: 'Copying Remote Access File To Storage Path',
                  footer: 'This box will close automatically when ready.'
              })
        }

        $(document).ready(function() {
            $('#remoteaccess').addClass("nav-current");
        });
    </script>
   
     <div class="size-4 column">
        Is Remote Access Server:
    </div>
    <div class="size-5 column hidden-check">
        <asp:CheckBox runat="server" id="chkRemote" ClientIDMode="Static" AutoPostBack="true" OnCheckedChanged="chkRemote_CheckedChanged"/>
        <label for="chkRemote"></label>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Remote Access URL:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtUrl" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>
  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <h5><span style="color: #ff9900;">Remote Access Server:</span></h5>

</asp:Content>
