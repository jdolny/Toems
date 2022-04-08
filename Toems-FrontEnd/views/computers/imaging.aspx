<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="imaging.aspx.cs" Inherits="Toems_FrontEnd.views.computers.imaging" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li> <%= ComputerEntity.Name %></li>
    <li>Image Settings</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
        <%= ComputerEntity.Name %>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="DropDownActionsSub">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="btnUpdate_Click" Text="Update Computer" CssClass="main-action" /></li>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#imaging').addClass("nav-current");
          });
         
    </script>

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
        Effective Image:
    </div>
    <div class="size-5 column">
        <asp:Label ID="lblImage" runat="server"/>
    </div>
    <br class="clear"/>

     <div class="size-4 column">
        Effective Image Profile:
    </div>
    <div class="size-5 column">
        <asp:Label ID="lblImageProfile" runat="server"/>
    </div>
    <br class="clear"/>
    <br class="clear" />
     <br class="clear" />
    <h2>LIE PXE Static Ip Settings</h2>
    <div class="size-4 column">
        Ip Address:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtIpAddress" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Netmask:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtNetMask" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Gateway:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtGateway" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Dns:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtDns" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>Allows you to set an image for the computer to deploy or upload.  The effective image takes group membership into account.  An image does not need
        to be assigned directly to a computer, it can be from a group.
    </p>
</asp:Content>