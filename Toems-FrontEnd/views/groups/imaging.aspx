<%@ Page Title="" Language="C#" MasterPageFile="~/views/groups/groups.master" AutoEventWireup="true" CodeBehind="imaging.aspx.cs" Inherits="Toems_FrontEnd.views.groups.imaging" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= GroupEntity.Name %></li>
    <li>Image Settings</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
<%= GroupEntity.Name %>
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnUpdate" runat="server" OnClick="btnUpdate_Click" Text="Update Settings" CssClass="main-action"/></li>
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
    Proxy Bootloader:
</div>

<div class="size-5 column">
    <asp:DropDownList ID="ddlBootFile" runat="server" CssClass="ddlist">
        <asp:ListItem>bios_pxelinux</asp:ListItem>
        <asp:ListItem>bios_ipxe</asp:ListItem>
        <asp:ListItem>bios_x86_winpe</asp:ListItem>
        <asp:ListItem>bios_x64_winpe</asp:ListItem>
        <asp:ListItem>efi_x86_syslinux</asp:ListItem>
        <asp:ListItem>efi_x86_ipxe</asp:ListItem>
        <asp:ListItem>efi_x86_winpe</asp:ListItem>
        <asp:ListItem>efi_x64_syslinux</asp:ListItem>
        <asp:ListItem>efi_x64_ipxe</asp:ListItem>
        <asp:ListItem>efi_x64_grub</asp:ListItem>
        <asp:ListItem>efi_x64_winpe</asp:ListItem>

    </asp:DropDownList>
</div>
    <br class="clear" />
    <div class="size-4 column">
        Imaging Priority
    </div>

    <div class="size-5 column">
        <asp:TextBox ID="txtPriority" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    
   
    
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page can be used to send a message to all the computers in the group.  A message will only display if a user is logged into the computer.  The message just needs a Title and the message itself.  The timeout value is used to automatically close the message after a certain time period.  The default is 0, meaning it will not automatically close.  This value is in seconds.</p>
</asp:Content>
