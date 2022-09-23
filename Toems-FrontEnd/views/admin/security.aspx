<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" Inherits="Toems_FrontEnd.views.admin.views_admin_security" Codebehind="security.aspx.cs" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Security</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Admin Settings
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" runat="Server">
    <li><asp:LinkButton ID="btnUpdateSettings" runat="server" Text="Update Security Settings" OnClick="btnUpdateSettings_OnClick" CssClass="main-action"/></li>
    <li><asp:LinkButton ID="btnGenKey" runat="server" Text="Generate Provison Key" OnClick="btnGenKey_OnClick" /></li>
     <li><asp:LinkButton ID="btnGenImagingToken" runat="server" Text="Generate Imaging Token" OnClick="btnGenToken_OnClick" /></li>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#security').addClass("nav-current");
        });
    </script>
    <h2>Endpoint Management Security Settings</h2>
    <div class="size-4 column">
        Provision Key:
        
    </div>
    <div class="size-5 column ">
        <asp:TextBox ID="txtKey" runat="server" Style="font-size:16px;" CssClass="textbox blank"></asp:TextBox>
    </div>
    <br class="clear"/>
    <br />
       <div class="size-4 column">
        Enable MFA:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkMfa" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkMfa"></label>
    </div>
    <br class="clear"/>
       <div class="size-4 column">
        Force MFA For Web - All Users:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkForceMfa" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkForceMfa"></label>
    </div>
    <br class="clear"/>
       <div class="size-4 column">
        Force MFA For Imaging - All Users:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkForceMfaImaging" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkForceMfaImaging"></label>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Enable LDAP Integration:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkldap" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkldap"></label>
    </div>
    <br class="clear"/>
     <div class="size-4 column hidden-check">
        PreProvision Required:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkPreProvision" runat="server" AutoPostBack="True" OnCheckedChanged="chkPreProvision_OnCheckedChanged" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkPreProvision"></label>
    </div>
    <br class="clear"/>
     <div class="size-4 column hidden-check">
        New Computer Provision Approval Required:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkProvisionApproval" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkProvisionApproval"></label>
    </div>
    <br class="clear"/>
    <br/>
     <div class="size-4 column">
        Pre Provision Approval Required:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkPreProvisionApproval" runat="server" AutoPostBack="True" OnCheckedChanged="chkPreProvisionApproval_OnCheckedChanged" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkPreProvisionApproval"></label>
    </div>
    <br class="clear"/>
     <div class="size-4 column hidden-check">
        Reset Requests Required:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkResetRequest" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkResetRequest"></label>
    </div>
    <br class="clear"/>
    <br />
    <br />
    <h2>Imaging Security Settings</h2>
     <div class="size-4 column">
        Global Imaging Token:
        
    </div>
       <div class="size-5 column ">
        <asp:TextBox ID="txtImagingToken" runat="server" Style="font-size:16px;" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <br />

      <div class="size-4 column hidden-check">
        Web Tasks Require Login:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkWebTask" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkWebTask"></label>
    </div>
    <br class="clear"/>

      <div class="size-4 column hidden-check">
        Console Tasks Require Login:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkConsoleTask" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkConsoleTask"></label>
    </div>
    <br class="clear"/>
  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <h5><span style="color: #ff9900;">Enable LDAP Integration:</span></h5>
<p>This must be enabled for any LDAP integration to work.</p>
<h5><span style="color: #ff9900;">Pre-Provision Required:</span></h5>
<p>This is a client security setting.  This prevents computers from provisioning to Theopenem unless they have been pre-provisioned.  Computers can be pre-provisioned by syncing from AD, or manually from Computers-&gt;Create Pre-Provision.</p>
<h5><span style="color: #ff9900;">New Computer Provision Approval Required:</span></h5>
<p>This is a client security setting.  When this is enabled, new computers must be manually approved before they will provision to Theopenem.  If the computer has already been pre-provisioned, a manual approval will not be required.</p>
<h5><span style="color: #ff9900;">Pre-Provision Approval Required:</span></h5>
<p>This is a client security setting.  When this is enabled, new computers must be manually approved before they will provision to Theopenem.  Even if the computer has already been pre-provisioned, a manual approval will still be required.</p>
<h5><span style="color: #ff9900;">Reset Requests Required:</span></h5>
<p>This is a client security setting.  When this is enabled, client reset requests must be manually approved before re-provisioning can occur.  If a client cannot communicate with the Toec Api, due to a bad key, or certificate or many other reasons, it will attempt to reset itself in order to fix the problem, then provision again.  If someone is trying impersonate a computer or hack your Toec Api, it should trigger this reset.</p>
</asp:Content>