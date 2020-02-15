<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/pxeboot/pxeboot.master" AutoEventWireup="true" CodeBehind="editbootentry.aspx.cs" Inherits="Toems_FrontEnd.views.admin.pxeboot.editbootentry" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
     <li><a href="<%= ResolveUrl("~/views/admin/pxeboot/createbootentry.aspx") %>?level=2">Create Custom Boot Entry</a></li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubNavTitle_Sub2" runat="server">

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub2" runat="server">

     <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_Click" Text="Update Boot Entry" CssClass="main-action" /></li>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" runat="server">
     
     <div class="size-4 column">
        Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtName" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>


    <div class="size-4 column">
        Description:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" CssClass="descbox"></asp:TextBox>
    </div>
    <br class="clear"/>

    <div class="size-4 column">
        Type:
    </div>
    <div class="size-5 column">
        <asp:DropDownList runat="server" id="ddlType" CssClass="ddlist">
            <asp:ListItem>syslinux/pxelinux</asp:ListItem>
            <asp:ListItem>ipxe</asp:ListItem>
            <asp:ListItem>grub</asp:ListItem>
        </asp:DropDownList>
    </div>
    <br class="clear"/>

    <div class="size-4 column">
        Order:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtOrder" runat="server" CssClass="textbox" ></asp:TextBox>
    </div>
    <br class="clear"/>

    <div class="size-4 column">
        Active:
    </div>
    <div class="size-5 column">
        <asp:CheckBox runat="server" id="chkActive" CssClass=""/>
    </div>
    <br class="clear"/>

    <div class="size-4 column">
        Default:
    </div>
    <div class="size-5 column">
        <asp:CheckBox runat="server" id="chkDefault" CssClass=""/>
    </div>
    <br class="clear"/>

    <div class="size-4 column">
        Contents:
    </div>
    <br class="clear"/>


    <asp:TextBox ID="txtContents" runat="server" CssClass="sysprepcontent" TextMode="MultiLine"></asp:TextBox>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="subsubHelp" runat="server">
</asp:Content>