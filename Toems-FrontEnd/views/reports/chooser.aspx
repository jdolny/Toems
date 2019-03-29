<%@ Page Title="" Language="C#" MasterPageFile="~/views/reports/reports.master" AutoEventWireup="true" CodeBehind="chooser.aspx.cs" Inherits="Toems_FrontEnd.views.reports.chooser" %>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Reports
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavLevel1">

        <ul class="ul-secondary-nav">
            <li id="computer">
                <a href="<%= ResolveUrl("~/views/reports/computer/chooser.aspx") %>">
                    <span class="sub-nav-text">Computer Reports</span></a>
            </li>
            <li id="asset">
                <a href="<%= ResolveUrl("~/views/reports/asset/chooser.aspx") %>">
                    <span class="sub-nav-text">Asset Reports</span></a>
            </li>
            <li id="process">
                <a href="<%= ResolveUrl("~/views/reports/process/chooser.aspx") %>">
                    <span class="sub-nav-text">Process Reports</span></a>
            </li>
            <li id="user">
                <a href="<%= ResolveUrl("~/views/reports/user/chooser.aspx") %>">
                    <span class="sub-nav-text">User Reports</span></a>
            </li>
            <li id="software">
                <a href="<%= ResolveUrl("~/views/reports/software.aspx") %>">
                    <span class="sub-nav-text">Software List</span></a>
            </li>
               <li id="certificates">
                <a href="<%= ResolveUrl("~/views/reports/certificates.aspx") %>">
                    <span class="sub-nav-text">Certificate List</span></a>
            </li>
        </ul>


</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubContent">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.actions_left').addClass("display-none");
        });
    </script>
</asp:Content>