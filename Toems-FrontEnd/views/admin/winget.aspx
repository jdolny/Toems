<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" CodeBehind="winget.aspx.cs" Inherits="Toems_FrontEnd.views.admin.winget" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>LDAP</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Admin Settings
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" runat="Server">
    <li><asp:LinkButton ID="btnRunWinGet" runat="server" Text="Run Winget Importer" OnClick="btnRunWinGet_Click" CssClass="main-action"/></li>
</asp:Content>





<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#winget').addClass("nav-current");
        });
    </script>


</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">

</asp:Content>
