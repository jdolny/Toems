<%@ Page Title="" Language="C#" MasterPageFile="~/views/images/profiles/profiles.master" AutoEventWireup="true" CodeBehind="multicast.aspx.cs" Inherits="Toems_FrontEnd.views.images.profiles.multicast" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
      <li>
        <a href="<%= ResolveUrl("~/views/images/profiles/general.aspx") %>?imageId=<%= ImageEntity.Id %>&profileId=<%= ImageProfile.Id %>&sub=profiles"><%= ImageProfile.Name %></a>
    </li>
 <li>Multicast Options</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
     <%= ImageProfile.Name %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="btnUpdate" runat="server" Text="Update Profile" OnClick="btnUpdate_Click" CssClass="main-action"/></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#multicast').addClass("nav-current");
           

        });
    </script>

      <div class="size-4 column">
        Sender Arguments:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtSender" runat="server" CssClass="textbox">
        </asp:TextBox>
    </div>
    <br class="clear"/>

    <div class="size-4 column">
        Receiver Arguments:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtReceiver" runat="server" CssClass="textbox">
        </asp:TextBox>
    </div>
    <br class="clear"/>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    Allows you to set custom arguments on the server and client to control multicast behavior.  Argument list can be found at <a href="http://www.udpcast.linux.lu/cmd.html">http://www.udpcast.linux.lu/cmd.html</a>
    <h5><span style="color: #ff9900;">Sender Arguments:</span></h5>
    <p>Arguments that are passed to the multicast sender.</p>

     <h5><span style="color: #ff9900;">Receiver Arguments:</span></h5>
     <p>Arguments that are passed to the multicast receiver.</p>
</asp:Content>
