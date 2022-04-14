<%@ Page Title="" Language="C#" MasterPageFile="~/views/users/user.master" AutoEventWireup="true" Inherits="Toems_FrontEnd.views.users.CreateUser" Codebehind="create.aspx.cs" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
     <li>
        <a href="<%= ResolveUrl("~/views/users/search.aspx") %>">Users</a>
    </li>
    <li>New User</li>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
Users
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Add User" CssClass="main-action"/></li>
</asp:Content>


<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#createuser').addClass("nav-current");
        });
    </script>
    <div class="size-4 column">
        User Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtUserName" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        User Role:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddluserMembership" runat="server" CssClass="ddlist">
            <asp:ListItem>Administrator</asp:ListItem>
            <asp:ListItem>User</asp:ListItem>
        </asp:DropDownList>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Use LDAP Authentication:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkldap" runat="server" AutoPostBack="True" OnCheckedChanged="chkldap_OnCheckedChanged" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkldap"></label>
    </div>
   
    <br class="clear"/>
    <div id="passwords" runat="server">
        <div class="size-4 column">
            User Password:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtUserPwd" runat="server" CssClass="textbox" TextMode="Password"></asp:TextBox>
        </div>
        <br class="clear"/>
        <div class="size-4 column">
            Confirm Password:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtUserPwdConfirm" runat="server" CssClass="textbox" TextMode="Password"></asp:TextBox>
        </div>
        <br class="clear"/>
    </div>
    <div class="size-4 column">
        Email:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtEmail" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
     <br class="clear"/>
    <div class="size-4 column">
        User Theme:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlTheme" runat="server" CssClass="ddlist">
          
        </asp:DropDownList>
    </div>
     <br class="clear"/>

     <div class="size-4 column">
            Default Computers Page:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlComputerView" runat="server" CssClass="ddlist">
                <asp:ListItem>Default</asp:ListItem>
                <asp:ListItem>Active</asp:ListItem>
                <asp:ListItem>Image Only</asp:ListItem>
                <asp:ListItem>All</asp:ListItem>
            </asp:DropDownList>
        </div>
        <br class="clear"/>

      <div class="size-4 column">
            Default Computer Sort:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlComputerSort" runat="server" CssClass="ddlist">
                <asp:ListItem>Default</asp:ListItem>
                <asp:ListItem>Name</asp:ListItem>
                <asp:ListItem>Last Checkin</asp:ListItem>
            </asp:DropDownList>
        </div>
        <br class="clear"/>

      <div class="size-4 column">
            Default Login Page:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlLoginPage" runat="server" CssClass="ddlist">
                <asp:ListItem>Default</asp:ListItem>
                <asp:ListItem>Dashboard</asp:ListItem>
                <asp:ListItem>Active Computers</asp:ListItem>
                <asp:ListItem>Image Only Computers</asp:ListItem>
                <asp:ListItem>All Computers</asp:ListItem>
            </asp:DropDownList>
        </div>
        <br class="clear"/>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <h5><span style="color: #ff9900;">Username:</span></h5>
<p>The name the user will use to login with.</p>
<h5><span style="color: #ff9900;">User Role:</span></h5>
<p>The role of the user, administrators have full access to Theopenem.  The user role can be assigned various permissions through ACL's after they have been added.</p>
<h5><span style="color: #ff9900;">Use LDAP Authentication:</span></h5>
<p>When enabled, the user will authenticate against AD instead of the local Theopenem database.  When using this feature, the username must match the samAccountName in AD and the LDAP connection must be setup in Admin Settings-&gt;LDAP.</p>
<h5><span style="color: #ff9900;">User Password:</span></h5>
<p>The password for users that are not using LDAP Authentication.  A user can change their password from the User's navigation menu after logging in.  A password must be at least 8 characters.</p>
<h5><span style="color: #ff9900;">Email:</span></h5>
<p>An email address for the user to receive notifications.  Email must first be setup in Admin Settings-&gt;E-mail before emails will work.</p>
    <h5><span style="color: #ff9900;">Theme:</span></h5>
<p>Sets the theme for the user.  If the user is currently logged in, a logout is required to apply the theme.</p>
</asp:Content>