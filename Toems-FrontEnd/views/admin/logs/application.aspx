<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/logs/logs.master" AutoEventWireup="true" CodeBehind="application.aspx.cs" Inherits="Toems_FrontEnd.views.admin.logs.application" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>Application</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    Logs
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="btnExportLog" runat="server" Text="Export Log" OnClick="btnExportLog_Click" CssClass="main-action"></asp:LinkButton></li>
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#app').addClass("nav-current");
        });
    </script>
    <div class="size-7 column">
        <asp:DropDownList ID="ddlLog" runat="server" CssClass="ddlist" AutoPostBack="True">
        </asp:DropDownList>
    </div>

    <div class="size-4 column" style="float: right; margin: 0;">
        <asp:DropDownList ID="ddlLimit" runat="server" CssClass="ddlist" Style="float: right; width: 75px;" AutoPostBack="true" OnSelectedIndexChanged="ddlLimit_SelectedIndexChanged">
            <asp:ListItem>10</asp:ListItem>
            <asp:ListItem>25</asp:ListItem>
            <asp:ListItem>50</asp:ListItem>
            <asp:ListItem>100</asp:ListItem>
            <asp:ListItem>All</asp:ListItem>
        </asp:DropDownList>
        <br class="clear"/>

    </div>
    <br class="clear"/>
    <asp:GridView ID="gvLog" runat="server" CssClass="Gridview log" ShowHeader="false">
    </asp:GridView>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>Logs display some of the available logs through the WebUI.  The Application logs and Front End Logs are available for viewing, but the Toec Api log must be viewed from the filesystem.</p>
</asp:Content>