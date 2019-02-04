<%@ Page Title="" Language="C#" MasterPageFile="~/views/groups/groups.master" AutoEventWireup="true" CodeBehind="oubrowser.aspx.cs" Inherits="Toems_FrontEnd.views.groups.oubrowser" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>OU Browser</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
Groups
</asp:Content>





<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#ou').addClass("nav-current");
            $('.actions_left').addClass("display-none");
        });
    </script>
    <div class="tree">
        <asp:TreeView ID="treeOus" runat="server" ImageSet="Arrows" ExpandDepth="0" RootNodeStyle-CssClass="rootNode" LeafNodeStyle-CssClass="leafNode" NodeStyle-CssClass="treeNode" NodeIndent="10">
        </asp:TreeView>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page allows you to view OU's that have been synced from Active Directory.  Active Directory OU's behave like static groups but are on a separate page to make it easier to browse.  Each OU can be clicked on to assign policies or view group members.  Clicking on a shaded arrow next to the OU displays any available sub OU's.  If the arrow is an empty outline it does not contain any sub OU's.  The ldap sync can be setup from admin settings->LDAP.</p>
</asp:Content>
