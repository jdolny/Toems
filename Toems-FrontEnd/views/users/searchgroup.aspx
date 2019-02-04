<%@ Page Title="" Language="C#" MasterPageFile="~/views/users/user.master" AutoEventWireup="true" Inherits="Toems_FrontEnd.views.users.views_users_searchgroup" Codebehind="searchgroup.aspx.cs" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
     <li>
        <a href="<%= ResolveUrl("~/views/users/searchgroup.aspx") %>">User Groups</a>
    </li>
    <li>Search User Groups</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
User Groups
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
     <li><asp:LinkButton runat="server" ID="btnDelete" Text="Delete Selected" CssClass="main-action" OnClick="btnDelete_OnClick"></asp:LinkButton></li>
</asp:Content>



<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#searchusergroup').addClass("nav-current");
            $("[id*=gvGroups] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });
    </script>
   
    <div class="size-7 column">
        <asp:TextBox ID="txtSearch" runat="server" CssClass="rounded-search" OnTextChanged="search_Changed"></asp:TextBox>
    </div>
     <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>

    <asp:GridView ID="gvGroups" runat="server" DataKeyNames="Id" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
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
            <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/users/editgroup.aspx?groupid={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:BoundField DataField="Id" HeaderText="userID" SortExpression="Id" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"/>
            <asp:BoundField DataField="IsLdapGroup" HeaderText="LDAP" SortExpression="Name" ItemStyle-CssClass="width_200"/>
            <asp:BoundField DataField="Membership" HeaderText="Role" SortExpression="Membership" ItemStyle-CssClass="width_200 mobi-hide-smallest" HeaderStyle-CssClass="mobi-hide-smallest"/>
            <asp:TemplateField ShowHeader="True" HeaderText="Members">
                <ItemTemplate>
                    <asp:Label ID="lblCount" runat="server" CausesValidation="false" CssClass="lbl_file "></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
        <EmptyDataTemplate>
            No User Groups Found
        </EmptyDataTemplate>
    </asp:GridView>

    <div id="confirmbox" class="confirm-box-outer">
        <div class="confirm-box-inner">
            <h4>
                <asp:Label ID="lblTitle" runat="server" Text="Delete The Selected Groups?" CssClass="modaltitle"></asp:Label>
            </h4>
            <div class="confirm-box-btns">
                <asp:LinkButton ID="ConfirmButton" OnClick="btnSubmit_Click" runat="server" Text="Yes" CssClass="confirm_yes"/>
                <asp:LinkButton ID="CancelButton" runat="server" Text="No" CssClass="confirm_no"/>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <h3><strong>The Search User Groups Page</strong></h3>
<p>The search page allows you to view and delete existing user groups. Filtering options include:</p>
<ul>
	<li><strong>Search by group name<br />
</strong></li>
</ul>
</asp:Content>