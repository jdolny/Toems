<%@ Page Title="" Language="C#" MasterPageFile="~/views/imagingtasks/imagingtasks.master" AutoEventWireup="true" CodeBehind="ondemand.aspx.cs" Inherits="Toems_FrontEnd.views.imagingtasks.ondemand" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>On Demand Multicast</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    On Demand Multicasts
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
  On Demand multicast is a way to quickly start a multicast without needing to put computers into a group. To use this feature you first start the task in the WebUI, then boot each 
    client computer into On Demand Mode and select multicast. Finally select the corresponding session name to join. After you have connected all the clients simply press 
    Enter on any client to start the multicast. Alternatively, you can specify a client count in the WebUI in which case the multicast will automatically start after that number of clients have connected. 
    The client count field is optional.
</asp:Content>
