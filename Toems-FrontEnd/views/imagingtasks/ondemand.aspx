<%@ Page Title="" Language="C#" MasterPageFile="~/views/imagingtasks/imagingtasks.master" AutoEventWireup="true" CodeBehind="ondemand.aspx.cs" Inherits="Toems_FrontEnd.views.imagingtasks.ondemand" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>On Demand Multicasts</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" runat="Server">
      <li><asp:LinkButton ID="btnMulticast" runat="server" Text="Start Multicast" OnClick="btnMulticast_Click" CssClass="main-action"/></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" runat="Server">
  <script type="text/javascript">
        $(document).ready(function() {
            $('#ond').addClass("nav-current");
        
        });

    </script>
  
         <div class="size-4 column">
        Session Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox runat="server" ID="txtSessionName" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear" />

    <div class="size-4 column">
        Image:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlComputerImage" runat="server" CssClass="ddlist" AutoPostBack="true" OnSelectedIndexChanged="ddlComputerImage_OnSelectedIndexChanged"/>
    </div>
    <br class="clear"/>

    <div class="size-4 column">
        Image Profile:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlImageProfile" runat="server" CssClass="ddlist"/>
    </div>

    <br class="clear"/>

    <div class="size-4 column">
        Client Count:
    </div>
    <div class="size-5 column">
        <asp:TextBox runat="server" ID="txtClientCount" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear" />
        <div class="size-4 column">
        Com Server:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlComServer" runat="server" CssClass="ddlist"/>
    </div>

    <br class="clear"/>
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
