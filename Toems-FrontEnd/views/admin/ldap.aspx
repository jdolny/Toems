<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" CodeBehind="ldap.aspx.cs" Inherits="Toems_FrontEnd.views.admin.ldap" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>LDAP</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Admin Settings
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" runat="Server">
    <li><asp:LinkButton ID="btnUpdateSettings" runat="server" Text="Update LDAP Settings" OnClick="btnUpdateSettings_OnClick" CssClass="main-action"/></li>
    <li><asp:LinkButton ID="btnTestBind" runat="server" Text="Test AD Bind" OnClick="btnTestBind_Click" /></li>
</asp:Content>





<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#ldap').addClass("nav-current");
        });
    </script>

    <asp:Label runat="server" ID="txtLdap"></asp:Label>
    <div id="ad" runat="server" Visible="False">
        <div class="size-4 column">
            LDAP Server:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtldapServer" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
        <br class="clear"/>
        <div class="size-4 column">
            LDAP Port:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtldapPort" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
        <br class="clear"/>
        <div class="size-4 column">
            LDAP Authentication Attribute:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtldapAuthAttribute" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
        <br class="clear"/>
        <div class="size-4 column">
            LDAP Base DN:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtldapbasedn" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
        <br class="clear"/>
        <div class="size-4 column">
            LDAP Sync OU:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtldapgroupfilter" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
        <br class="clear"/>
         <div class="size-4 column">
            LDAP Bind Username:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtLdapUsername" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
        <br class="clear"/>
         <div class="size-4 column">
            LDAP Bind Password:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtLdapPassword" runat="server" CssClass="textbox password" TextMode="Password"></asp:TextBox>
        </div>
        <br class="clear"/>
        <div class="size-4 column">
            LDAP Authentication Type:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlldapAuthType" runat="server" CssClass="ddlist">
                <asp:ListItem>Basic</asp:ListItem>
                <asp:ListItem>Secure</asp:ListItem>
            </asp:DropDownList>
        </div>
        <br class="clear"/>

    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <h5><span style="color: #ff9900;">LDAP Server:</span></h5>
<p>The ip address or fqdn of your LDAP server.</p>
<h5><span style="color: #ff9900;">LDAP Port:</span></h5>
<p>The port of your LDAP server, typically <strong>389</strong> or <strong>636 for SSL</strong>.</p>
<h5><span style="color: #ff9900;">LDAP Authentication Attribute:</span></h5>
<p>The attribute to authenticate against, should most of the time be set to <strong>samaccountname</strong></p>
<h5><span style="color: #ff9900;">LDAP Base DN:</span></h5>
<p>The root of your domain, it must be the root in order to work, such as <strong>dc=theopenem,dc=com</strong></p>
<h5><span style="color: #ff9900;">LDAP Sync OU:</span></h5>
<p>Limit the LDAP sync to this OU and all child elements, needs to be in the following format <strong>ou=parttime,ou=openemstaff</strong></p>
<h5><span style="color: #ff9900;">LDAP Bind Username:</span></h5>
<p>The username of the account used to bind to AD.  Domain should not be included.</p>
<h5><span style="color: #ff9900;">LDAP Bind Password:</span></h5>
<p>The password of the account used to bind to AD.</p>
<h5><span style="color: #ff9900;">LDAP Authentication Type:<br />
</span></h5>
<p>Secure should be used for both LDAP and LDAPS</p>
</asp:Content>
