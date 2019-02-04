<%@ Page Title="" Language="C#" MasterPageFile="~/views/assets/assetgroups/assetgroups.master" AutoEventWireup="true" CodeBehind="currentmembers.aspx.cs" Inherits="Toems_FrontEnd.views.assets.assetgroups.currentmembers" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li> <%=AssetGroup.Name %></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    <%=AssetGroup.Name %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Remove Group Members" CssClass="main-action"/></li>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#currentmembers').addClass("nav-current");
        });
    </script>
    <asp:GridView ID="gvAssets" runat="server"  DataKeyNames="AssetId"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:TemplateField>
                <HeaderStyle CssClass="chkboxwidth"></HeaderStyle>
                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <HeaderTemplate>
                    <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="True" OnCheckedChanged="chkSelectAll_CheckedChanged"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelector" runat="server"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:HyperLinkField DataNavigateUrlFields="AssetId" DataNavigateUrlFormatString="~/views/assets/customassets/edit.aspx?level=2&assetId={0}" Target="_default"  Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:BoundField DataField="AssetId" HeaderText="Id" SortExpression="Id" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Display Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="AssetType" HeaderText="Asset Type" ></asp:BoundField>
         

        </Columns>
        <EmptyDataTemplate>
            No Assets Found
        </EmptyDataTemplate>
    </asp:GridView>
  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>Displays all assets that are currently part of the group.  Assets can also be removed from the group on this page.</p>
</asp:Content>
