<%@ Page Title="" Language="C#" MasterPageFile="~/views/users/user.master" AutoEventWireup="true" CodeBehind="groupacls.aspx.cs" Inherits="Toems_FrontEnd.views.users.groupacls" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
     <li>
        <a href="<%= ResolveUrl("~/views/users/searchgroup.aspx") %>">User Groups</a>
    </li>
    <li><%= ToemsUserGroup.Name %></li>
    <li>ACL's</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
<%= ToemsUserGroup.Name %>
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Update Access Control" CssClass="main-action"/></li>
</asp:Content>


<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
<script type="text/javascript">
    $(document).ready(function () {
        $('#usergroupacl').addClass("nav-current");
    });
</script>
    <div class="acl_header">
<div class="size-4 column">&nbsp;</div>
<div class="size-10 column">Read</div>
<div class="size-10 column">Update</div>
<div class="size-10 column">Archive</div>
<div class="size-10 column">Delete</div>
        </div>
<div class="clear"></div>

<div class="size-4 column">
    Modules
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="moduleRead"/>
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="moduleUpdate"/>
</div>
    <div class="size-10 column">
    <asp:CheckBox runat="server" Id="moduleArchive"/>
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="moduleDelete"/>
</div>
<br class="clear"/>
<br/>
<div class="size-4 column">
    Policies
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="policyRead"/>
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="policyUpdate"/>
</div>
        <div class="size-10 column">
    <asp:CheckBox runat="server" Id="policyArchive"/>
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="policyDelete"/>
</div>
<br class="clear"/>
<br/>
    <div class="size-4 column">
    Group Policies
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="groupPolicyRead"/>
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="groupPolicyUpdate"/>
</div>

<br class="clear"/>
<br/>
<div class="size-4 column">
    Groups
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="groupRead"/>
</div>
<div class="size-10 column">
   <asp:CheckBox runat="server" Id="groupUpdate"/>
</div>
        <div class="size-10 column">
    &nbsp;
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="groupDelete"/>
</div>

<br class="clear"/>
<br/>
<div class="size-4 column">
    Computers
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="computerRead"/>
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="computerUpdate"/>
</div>
    <div class="size-10 column">
    <asp:CheckBox runat="server" Id="computerArchive"/>
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="computerDelete"/>
</div>
<br class="clear"/>
<br/>
<div class="size-4 column">
    Reports
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="reportRead"/>
</div>

<br class="clear"/>
<br/>
    
<div class="size-4 column">
    Schedules
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="scheduleRead"/>
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="scheduleUpdate"/>
</div>
    <div class="size-10 column">
    &nbsp;
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="scheduleDelete"/>
</div>

    <br class="clear"/>
<br/>
    
<div class="size-4 column">
    Categories
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="categoryRead"/>
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="categoryUpdate"/>
</div>
<div class="size-10 column">
    &nbsp;
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="categoryDelete"/>
</div>

<br class="clear"/>
<br/>

<div class="size-4 column">
    Custom Attributes
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="customAttributeRead"/>
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="customAttributeUpdate"/>
</div>
<div class="size-10 column">
    &nbsp;
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="customAttributeDelete"/>
</div>

<br class="clear"/>
<br/>

<div class="size-4 column">
    Asset Types
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="assetTypeRead"/>
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="assetTypeUpdate"/>
</div>
<div class="size-10 column">
    &nbsp;
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="assetTypeDelete"/>
</div>

<br class="clear"/>
<br/>

<div class="size-4 column">
    Assets
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="assetRead"/>
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="assetUpdate"/>
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="assetArchive"/>
</div>
<div class="size-10 column">
    <asp:CheckBox runat="server" Id="assetDelete"/>
</div>

<br class="clear"/>
<br/>
<hr/>
<br/>

<br/>
     <div class="size-4 column">
    Activate / Deactivate Policies
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="policyActivate"/>
</div>

<br class="clear"/>
<br/>
<div class="size-4 column">
    Approve Provision Requests
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="approveProvision"/>
</div>

<br class="clear"/>
<br/>
<div class="size-4 column">
    Approve Reset Requests
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="approveReset"/>
</div>

<br class="clear"/>
<br/>
    <div class="size-4 column">
    Send Messages
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="computerSendMessage"/>
</div>

<br class="clear"/>
<br/>
    <div class="size-4 column">
    Force Checkins
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="computerForceCheckin"/>
</div>
    <br class="clear"/>
<br/>
      <div class="size-4 column">
    Computer Reboot
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="computerReboot"/>
</div>

<br class="clear"/>
    <br/>
    
 <div class="size-4 column">
    Computer Shutdown
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="computerShutdown"/>
</div>
      <br class="clear"/>
    <br/>
    
 <div class="size-4 column">
    Computer Wake Up
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="computerWakeup"/>
</div>
<br class="clear"/>
    <br/>

 <div class="size-4 column">
    Group Reboot
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="groupReboot"/>
</div>

<br class="clear"/>
    <br/>

     <div class="size-4 column">
    Group Shutdown
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="groupShutdown"/>
</div>
     <br class="clear"/>
    <br/>
    
 <div class="size-4 column">
    Group Wake Up
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="groupWakeup"/>
</div>
<br class="clear"/>
    <br/>

<div class="size-4 column">
    Upload Attachments
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="attachmentAdd"/>
</div>
<br class="clear"/>
<br/>

<div class="size-4 column">
    Download Attachments
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="attachmentRead"/>
</div>
<br class="clear"/>
<br/>

<div class="size-4 column">
    Add Comments
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="commentAdd"/>
</div>
<br class="clear"/>
<br/>

<div class="size-4 column">
    Read Comments
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="commentRead"/>
</div>
<br class="clear"/>
<br/>

<div class="size-4 column">
    Import Policy
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="policyImport"/>
</div>
<br class="clear"/>
<br/>

<div class="size-4 column">
    Export Policy
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="policyExport"/>
</div>
<br class="clear"/>
<br/>

<div class="size-4 column">
    Module Upload Files
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="moduleUploadFiles"/>
</div>
<br class="clear"/>
<br/>

<div class="size-4 column">
    Module Download External Files
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="moduleExternalFiles"/>
</div>
<br class="clear"/>
<br/>
<div class="size-4 column">
    Receive Pending Approval Report Email
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="emailApproval"/>
</div>
    
    <br class="clear"/>
<br/>
<div class="size-4 column">
    Receive Pending Reset Report Email
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="emailReset"/>
</div>
<br class="clear"/>
<br/>
<div class="size-4 column">
    Receive SMART Failure Report Email
</div>

<div class="size-10 column">
    <asp:CheckBox runat="server" Id="emailSmart"/>
</div>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page is used to set the permissions for all members of the group. It has no effect if the group is in the administrator role. The permissions are mostly self explanatory, following the general navigational layout of Theopenem. There are also some more specific options below the first section.</p>
</asp:Content>