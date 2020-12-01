<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/comservers/comservers.master" AutoEventWireup="true" CodeBehind="imagingsettings.aspx.cs" Inherits="Toems_FrontEnd.views.admin.comservers.imagingsettings" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li><a href="<%= ResolveUrl("~/views/admin/comservers/imagingsettings.aspx") %>?level=2">Imaging Settings</a></li>
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
            $('#imaging').addClass("nav-current");
        });
    </script>
   
          <div class="size-4 column">
        Imaging Server:
    </div>
    <div class="size-5 column hidden-check">
        <asp:CheckBox runat="server" id="chkImagingServer" ClientIDMode="Static"/>
        <label for="chkImagingServer"></label>
    </div>
    <br class="clear"/>
         <div class="size-4 column">
       Max Bitrate Kbps(1000Kbps = 1Mbps):
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtMaxBitrate" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
     <br class="clear"/>
     <div class="size-4 column">
       Max Simultaneous Client Connections:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtMaxClients" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
     <br class="clear"/>

       <div class="size-4 column">
       Upload Interface IP:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtImagingIp" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
     <br class="clear"/>
    

  
  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <h5><span style="color: #ff9900;">Imaging Server:</span></h5>
<p>Determines if this com server can be used as an imaging server.</p>
<h5><span style="color: #ff9900;">Max Bitrate:</span></h5>
<p>The max bitrate per client connection.  0 = no limit.</p>
<h5><span style="color: #ff9900;">Max Simultaneous Client Connection:</span></h5>
<p>The max number of imaging connections that can occur at the same time.  After this number is reached others will be placed in a queue.  0 = none.  The minimum value should be 1.</p>
    <h5><span style="color: #ff9900;">Upload Interface IP:</span></h5>
    <p>The ip address of the nic that will be used for image uploads.  This must be populated before an image can be uploaded.</p>
</asp:Content>

