<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" CodeBehind="winget.aspx.cs" Inherits="Toems_FrontEnd.views.admin.winget" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>LDAP</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Admin Settings
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" runat="Server">
    <li><asp:LinkButton ID="btnUpdate" runat="server" Text="Update Settings" OnClick="btnUpdate_Click" CssClass="main-action"/></li>
</asp:Content>





<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#winget').addClass("nav-current");
        });
    </script>
     <div class="size-4 column">
        Winget Packages URL:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtUrl" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
      <br class="clear"/>

     <div class="size-4 column">
        Last Packages Import:
    </div>
    <div class="size-5 column">
        <asp:Label ID="lblLastImport" runat="server" CssClass="textbox"></asp:Label>
    </div>
      <br class="clear"/>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">

</asp:Content>
