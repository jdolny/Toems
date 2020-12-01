<%@ Page Title="" Language="C#" MasterPageFile="~/views/images/images.master" AutoEventWireup="true" CodeBehind="categories.aspx.cs" Inherits="Toems_FrontEnd.views.images.categories" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
     <li>
        <a href="<%= ResolveUrl("~/views/images/general.aspx") %>?imageId=<%= ImageEntity.Id %>"><%= ImageEntity.Name %></a>
    </li>
    <li>Categories</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    <%= ImageEntity.Name %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" runat="Server">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Update Image" CssClass="main-action"></asp:LinkButton></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" runat="Server">
  <script type="text/javascript">
        $(document).ready(function() {
            $('#categories').addClass("nav-current");

        });

    </script>
    <asp:GridView ID="gvCategories" runat="server"  DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
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
        <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/global/categories/edit.aspx?level=2&categoryId={0}" Target="_default" Text="View" ItemStyle-CssClass="chkboxwidth"/>
        <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="False"/>
        <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
        <asp:BoundField DataField="Description" HeaderText="Description"></asp:BoundField>
         

    </Columns>
    <EmptyDataTemplate>
        No Categories Defined
    </EmptyDataTemplate>
</asp:GridView>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
  <p>Categories operate like tags and allow you to organize your groups and filter by them on the search page. Categories can be defined in Global Properties->Categories</p>
</asp:Content>
