<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" CodeBehind="storage.aspx.cs" Inherits="Toems_FrontEnd.views.admin.storage" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Storage</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Admin Settings
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" runat="Server">
    <li><asp:LinkButton ID="btnUpdateSettings" runat="server" Text="Update Storage Settings" OnClick="btnUpdateSettings_OnClick" CssClass="main-action"/></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#storage').addClass("nav-current");
        });
    </script>

     <div class="size-4 column">
            Storage Type:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlType" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlType_OnSelectedIndexChanged">
                <asp:ListItem>Local</asp:ListItem>
                <asp:ListItem>SMB</asp:ListItem>
            </asp:DropDownList>
        </div>
        <br class="clear"/>
        <div class="size-4 column">
            Storage Path:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtPath" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
        <br class="clear"/>
        <div id="divSmb" runat="server">
       
              <div class="size-4 column">
            Username:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtUsername" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
        <br class="clear"/>
                  <div class="size-4 column">
            Password:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtPassword" runat="server" CssClass="textbox password" TextMode="Password"></asp:TextBox>
        </div>
        <br class="clear"/>
              <div class="size-4 column">
            Domain:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtDomain" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
        <br class="clear"/>
        </div>
       
       

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>Specifies if the storage path is local to this server or on a remote SMB share.  The local type can only be used if only a single server is used for all Toec Api's, Toems Api's, and Toems Front Ends.</p>
<h5><span style="color: #ff9900;">Storage Path:</span></h5>
<p>The path to the storage directory.  If the storage type is local, the path should be a local drive, such as c:\toems-local-storage.  If the storage type is SMB, the path must be a UNC path, such as \\server\toems-remote-storage.</p>
<h5><span style="color: #ff9900;">Username:</span></h5>
<p>Only available for a storage type of SMB.  This is the username used to connect to the share.</p>
<h5><span style="color: #ff9900;">Password:</span></h5>
<p>Only available for a storage type of SMB.  This is the password used to connect to the share.</p>
<h5><span style="color: #ff9900;">Domain:</span></h5>
<p>Only available for a storage type of SMB.  This is the domain used to connect to the share.</p>
</asp:Content>
