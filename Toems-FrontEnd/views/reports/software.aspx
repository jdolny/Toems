<%@ Page Title="" Language="C#" MasterPageFile="~/views/reports/reports.master" AutoEventWireup="true" CodeBehind="software.aspx.cs" Inherits="Toems_FrontEnd.views.reports.software" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Software List</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
 Software List
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnExport" runat="server" OnClick="btnExport_OnClick" Text="Export To CSV" CssClass="main-action"/></li>
</asp:Content>


<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
     <script type="text/javascript">
        $(document).ready(function() {
            $('#search').addClass("nav-current");
            $("[id*=gvSoftware] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });
    </script>
    <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>
    <div class="size-7 column">
        <asp:TextBox ID="txtSearch" runat="server" CssClass="rounded-search" OnTextChanged="search_Changed"></asp:TextBox>
    </div>
    <br class="clear"/>

    <asp:GridView ID="gvSoftware" runat="server"  DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
           
            <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="Version" HeaderText="Version" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="UninstallString" HeaderText="Uninstall String"></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No Software Found
        </EmptyDataTemplate>
    </asp:GridView>

  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This is a simple report that displays every software application that Theopenem is aware of.</p>
</asp:Content>