<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/pxeboot/pxeboot.master" AutoEventWireup="true" CodeBehind="createbootentry.aspx.cs" Inherits="Toems_FrontEnd.views.admin.pxeboot.createbootentry" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
     <li><a href="<%= ResolveUrl("~/views/admin/pxeboot/createbootentry.aspx") %>?level=2">Create Custom Boot Entry</a></li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubNavTitle_Sub2" runat="server">
    Create Boot Entry
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub2" runat="server">

     <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_Click" Text="Create Boot Entry" CssClass="main-action" /></li>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" runat="server">
     <script type="text/javascript">
        $(document).ready(function() {
            $('#createbootentry').addClass("nav-current");
        });
    </script>

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
        <asp:TextBox ID="txtOrder" runat="server" CssClass="textbox" Text="0"></asp:TextBox>
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
       <div class="size-5 column">
    <asp:TextBox ID="txtContents" runat="server" CssClass="sysprepcontent" TextMode="MultiLine"></asp:TextBox>
           </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="subsubHelp" runat="server">
    <p>Boot Menu Entries are a way to add custom boot menu options to the existing Global Default Boot Menu. The entries are inserted every time you generate the Global Default 
        Boot Menu. They do not get inserted when you create the entry, only after you generate the new boot files in Admin Settings->Global Boot Menu</p>
</asp:Content>

