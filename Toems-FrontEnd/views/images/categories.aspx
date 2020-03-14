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
    <p>Specifies if the storage path is local to this server or on a remote SMB share.  The local type can only be used if only a single server is used for all Toec Api's, Toems Api's, and Toems Front Ends.</p>
<h5><span style="color: #ff9900;">Storage Path:</span></h5>
<p>The path to the storage directory.  If the storage type is local, the path should be a local drive, such as c:\toems-local-storage.  If the storage type is SMB, the path must be a UNC path, such as \\server\toems-remote-storage.</p>
<h5><span style="color: #ff9900;">Username:</span></h5>
<p>Only available for a storage type of SMB.  This is the username used to connect to the share.</p>
<h5><span style="color: #ff9900;">Password:</span></h5>
<p>Only available for a storage type of SMB.  This is the password used to connect to the share.</p>
<h5><span style="color: #ff9900;">Domain:</span></h5>
<p>Only available for a storage type of SMB.  This is the domain used to connect to the share.</p>
</asp:Content>
