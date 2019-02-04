<%@ Page Title="" Language="C#" MasterPageFile="~/views/reports/user/user.master" AutoEventWireup="true" CodeBehind="chooser.aspx.cs" Inherits="Toems_FrontEnd.views.reports.user.chooser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubNavTitle_Sub2" runat="server">
    User Reports
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub2" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.actions_left').addClass("display-none");
        });

    </script>
</asp:Content>
