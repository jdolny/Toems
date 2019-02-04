<%@ Page Title="" Language="C#" MasterPageFile="~/views/users/user.master" AutoEventWireup="true" Inherits="Toems_FrontEnd.views.users.views_users_creategroup" Codebehind="creategroup.aspx.cs" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
     <li>
        <a href="<%= ResolveUrl("~/views/users/searchgroup.aspx") %>">User Groups</a>
    </li>
    <li>New User Group</li>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
User Groups
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnSubmit" runat="server" OnClick="btnSubmit_OnClick" Text="Add User Group" CssClass="main-action"/></li>
</asp:Content>


<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#createusergroup').addClass("nav-current");
        });
    </script>
    <div class="size-4 column">
        Group Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtGroupName" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Group Role:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlGroupMembership" runat="server" CssClass="ddlist">
            <asp:ListItem>Administrator</asp:ListItem>
            <asp:ListItem>User</asp:ListItem>
        </asp:DropDownList>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Use LDAP Group:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkldap" runat="server" AutoPostBack="True" OnCheckedChanged="chkldap_OnCheckedChanged" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkldap"></label>
    </div>
    <br class="clear"/>
    <div id="divldapgroup" runat="server" Visible="False">
        <div class="size-4 column">
            LDAP Group Name:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtLdapGroupName" runat="server" CssClass="textbox"></asp:TextBox>
        </div>


        <br class="clear"/>
    </div>

    <br/>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <h5><span style="color: #ff9900;">Group Name:</span></h5>
<p>The display name of the user group.</p>
<h5><span style="color: #ff9900;">Group Role:</span></h5>
<p>The role that will be assigned to any users added to the group, administrators have full access to Theopenem. The user role can be assigned various permissions through ACL's after they have been added.</p>
<h5><span style="color: #ff9900;">Use LDAP Group:</span></h5>
<p>When enabled, users from the specified AD security group are automatically added to the system.  This is not a sync.  The user will be added when they successfully authenticate. The user will authenticate against AD instead of the local Theopenem database. When using this feature, the LDAP connection must be setup in Admin Settings-&gt;LDAP.</p>
<h5><span style="color: #ff9900;">LDAP Group Name:</span></h5>
<p>This field is only available when Use LDAP Group is enabled.  This is used to specify the name of the AD Security Group to match against.</p>
</asp:Content>