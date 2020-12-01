<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/logs/logs.master" AutoEventWireup="true" CodeBehind="unregimaging.aspx.cs" Inherits="Toems_FrontEnd.views.admin.logs.unregimaging" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>Unregistered Imaging Logs</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    Logs
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">

</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#unreg').addClass("nav-current");
                        $("[id*=gvLogs] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });
    </script>

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
    <asp:GridView ID="gvLogs" runat="server" DataKeyNames="Id" AutoGenerateColumns="False" CssClass="Gridview extraPad" AlternatingRowStyle-CssClass="alt">
        <Columns>

            <asp:TemplateField>
                <HeaderStyle CssClass="chkboxwidth"></HeaderStyle>
                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <ItemTemplate>
                    <asp:LinkButton ID="btnView" runat="server" OnClick="btnView_OnClick" Text="View"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <ItemTemplate>
                    <asp:LinkButton ID="btnExport" runat="server" OnClick="btnExport_OnClick" Text="Export"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Id" Visible="False"/>
            <asp:BoundField DataField="Mac" HeaderText="Mac" SortExpression="Mac" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="LogTime" HeaderText="Time" SortExpression="LogTime" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="SubType" HeaderText="Type" SortExpression="SubType"/>


        </Columns>
        <EmptyDataTemplate>
            No Logs Found
        </EmptyDataTemplate>
    </asp:GridView>

    <div id="ViewLog" runat="server" Visible="False">
        <asp:GridView ID="gvLogView" runat="server" CssClass="Gridview log" ShowHeader="false">
        </asp:GridView>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>Displays imaging logs for unregistered computers.</p>
</asp:Content>
