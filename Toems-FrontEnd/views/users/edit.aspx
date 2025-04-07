<%@ Page Title="" Language="C#" MasterPageFile="~/views/users/user.master" AutoEventWireup="true" Inherits="Toems_FrontEnd.views.users.EditUser" Codebehind="edit.aspx.cs" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= ToemsUser.Name %></li>
    <li>General</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
<%= ToemsUser.Name %>
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Update User" CssClass="main-action"/></li>
     <li><asp:LinkButton ID="btnResetMfa" runat="server" OnClick="btnResetMfa_Click" Text="Reset Mfa Data" /></li>
    <li><asp:LinkButton ID="btnRemoveLegacy" runat="server" OnClick="btnRemoveLegacy_Click" Text="Remove Legacy Group Data" /></li>
</asp:Content>


<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">

    <script type="text/javascript">
        $(document).ready(function() {
            $('#editoption').addClass("nav-current");
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
    <div class="size-5 column">
       <asp:Label runat="server" ID="lblLdap"></asp:Label>
    </div>
    <br class="clear"/>

    <div id="passwords" runat="server">
        <div class="size-4 column">
            User Password:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtUserPwd" runat="server" CssClass="textbox password" TextMode="Password"></asp:TextBox>
        </div>
        <br class="clear"/>
        <div class="size-4 column">
            Confirm Password:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtUserPwdConfirm" runat="server" CssClass="textbox password" TextMode="Password"></asp:TextBox>
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
     <div class="size-4 column">
        Enable Web MFA:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkWebMfa" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkWebMfa"></label>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Enable Imaging MFA:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkImagingMfa" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkImagingMfa"></label>
    </div>
    <br class="clear"/>
    <h2>Active Computer Search Fields</h2>
    <hr />
        <div class="acl_header">
<div class="size-10-3 column"><h2>Field</h2></div>
<div class="size-10-3 column"><h2>Enabled</h2></div>
<div class="size-10-3 column"><h2>Order</h2></div>

        </div>
    <br class="clear" />
      <div class="size-10-3 column">
        Description:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkComputerDesc" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkComputerDesc"></label>
    </div>
       <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderDesc" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
    <br />

     <br class="clear" />
      <div class="size-10-3 column">
        Last Checkin:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkLastCheckin" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkLastCheckin"></label>
    </div>
     <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderLastCheckin" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
     <br />

     <br class="clear" />
      <div class="size-10-3 column">
        Last Known IP:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkLastKnownIp" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkLastKnownIp"></label>
    </div>
      <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderLastKnownIp" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
     <br class="clear" />
       <div class="size-10-3 column">
        Client Version:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkClientVersion" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkClientVersion"></label>
    </div>
      <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderClientVersion" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
    <br class="clear" />

       <div class="size-10-3 column">
        Last User:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkLastUser" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkLastUser"></label>
    </div>
      <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderLastUser" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
     <br class="clear" />
    <div class="size-10-3 column">
        Provision Date:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkProvisionDate" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkProvisionDate"></label>
    </div>
      <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderProvisionDate" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
     <br class="clear" />
    <div class="size-10-3 column">
        Status:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkStatus" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkStatus"></label>
    </div>
      <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderStatus" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
     <br class="clear" />
    <div class="size-10-3 column">
        Current Image:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkCurrentImage" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkCurrentImage"></label>
    </div>
      <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderCurrentImage" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
    <br class="clear" />
     <div class="size-10-3 column">
        Manufacturer:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkManufacturer" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkManufacturer"></label>
    </div>
      <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderManufacturer" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
     <br class="clear" />
     <div class="size-10-3 column">
        Model:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkModel" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkModel"></label>
    </div>
      <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderModel" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
     <br class="clear" />
     <div class="size-10-3 column">
        OS Name:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkOs" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkOs"></label>
    </div>
      <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderOs" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
     <br class="clear" />
     <div class="size-10-3 column">
        OS Version:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkOsVersion" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkOsVersion"></label>
    </div>
      <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderOsVersion" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
    <br class="clear" />
     <div class="size-10-3 column">
        OS Build:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkOsBuild" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkOsBuild"></label>

    </div>
      <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderOsBuild" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
     <br class="clear" />
    <div class="size-10-3 column">
        Domain:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkDomain" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkDomain"></label>

    </div>
      <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderDomain" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
     <br class="clear" />
     <div class="size-10-3 column">
        Force Checkin:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkForceCheckin" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkForceCheckin"></label>

    </div>
      <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderForceCheckin" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
     <br class="clear" />
     <div class="size-10-3 column">
        Collect Inventory:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkCollectInventory" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkCollectInventory"></label>

    </div>
      <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderCollectInventory" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
    <br class="clear" />
     <div class="size-10-3 column">
        Remote Control:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkRemoteControl" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkRemoteControl"></label>
    </div>
      <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderRemoteControl" runat="server" CssClass="orderbox">0</asp:TextBox>
    </div>
     <br class="clear" />
     <div class="size-10-3 column">
        Get Service Log:
    </div>
    <div class="size-10-3 column hidden-check">
        <asp:CheckBox ID="chkGetServiceLog" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkGetServiceLog"></label>

    </div>
      <div class="size-10-4 column">
        <asp:TextBox ID="txtOrderGetServiceLog" runat="server" CssClass="orderbox">0</asp:TextBox>
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