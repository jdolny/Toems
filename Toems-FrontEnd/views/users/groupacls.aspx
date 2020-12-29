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
<div class="size-10 column">&nbsp;</div>
<div class="size-10 column"><h2>Read</h2></div>
<div class="size-10 column"><h2>Update</h2></div>
<div class="size-10 column"><h2>Archive</h2></div>
<div class="size-10 column"><h2>Delete</h2></div>
        </div>
<div class="clear"></div>

<div class="size-10-2 column">
    Modules
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="moduleRead" ClientIDMode="Static"/>
     <label for="moduleRead"></label>
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="moduleUpdate" ClientIDMode="Static"/>
     <label for="moduleUpdate"></label>
</div>
    <div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="moduleArchive" ClientIDMode="Static"/>
         <label for="moduleArchive"></label>
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="moduleDelete" ClientIDMode="Static"/>
     <label for="moduleDelete"></label>
</div>
<br class="clear"/>
<br/>
<div class="size-10-2 column">
    Policies
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="policyRead" ClientIDMode="Static"/>
     <label for="policyRead"></label>
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="policyUpdate" ClientIDMode="Static"/>
     <label for="policyUpdate"></label>
</div>
        <div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="policyArchive" ClientIDMode="Static"/>
             <label for="policyArchive"></label>
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="policyDelete" ClientIDMode="Static"/>
     <label for="policyDelete"></label>
</div>

    <br class="clear"/>
<br/>
<div class="size-10-2 column">
    Group Policies
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="groupPolicyRead" ClientIDMode="Static"/>
     <label for="groupPolicyRead"></label>
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="groupPolicyUpdate" ClientIDMode="Static"/>
     <label for="groupPolicyUpdate"></label>
</div>

<br class="clear"/>
<br/>

<div class="size-10-2 column">
    Groups
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="groupRead" ClientIDMode="Static"/>
     <label for="groupRead"></label>
</div>
<div class="size-10 column hidden-check">
   <asp:CheckBox runat="server" Id="groupUpdate" ClientIDMode="Static"/>
     <label for="groupUpdate"></label>
</div>
        <div class="size-10 column">
    &nbsp;
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="groupDelete" ClientIDMode="Static"/>
     <label for="groupDelete"></label>
</div>

<br class="clear"/>
<br/>
<div class="size-10-2 column">
    Computers
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="computerRead" ClientIDMode="Static"/>
     <label for="computerRead"></label>
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="computerUpdate" ClientIDMode="Static"/>
     <label for="computerUpdate"></label>
</div>
    <div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="computerArchive" ClientIDMode="Static"/>
         <label for="computerArchive"></label>
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="computerDelete" ClientIDMode="Static"/>
     <label for="computerDelete"></label>
</div>
<br class="clear"/>
<br/>
<div class="size-10-2 column">
    Reports
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="reportRead" ClientIDMode="Static"/>
     <label for="reportRead"></label>
</div>

<br class="clear"/>
<br/>

    
<div class="size-10-2 column hidden-check">
    Schedules
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="scheduleRead" ClientIDMode="Static"/>
     <label for="scheduleRead"></label>
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="scheduleUpdate" ClientIDMode="Static"/>
     <label for="scheduleUpdate"></label>
</div>
    <div class="size-10 column">
    &nbsp;
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="scheduleDelete" ClientIDMode="Static"/>
     <label for="scheduleDelete"></label>
</div>

    <br class="clear"/>
<br/>

<div class="size-10-2 column">
    Categories
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="categoryRead" ClientIDMode="Static"/>
     <label for="categoryRead"></label>
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="categoryUpdate" ClientIDMode="Static"/>
     <label for="categoryUpdate"></label>
</div>
<div class="size-10 column hidden-check">
    &nbsp;
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="categoryDelete" ClientIDMode="Static"/>
     <label for="categoryDelete"></label>
</div>

<br class="clear"/>
<br/>

<div class="size-10-2 column">
    Custom Attributes
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="customAttributeRead" ClientIDMode="Static"/>
     <label for="customAttributeRead"></label>
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="customAttributeUpdate" ClientIDMode="Static"/>
     <label for="customAttributeUpdate"></label>
</div>
<div class="size-10 column hidden-check">
    &nbsp;
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="customAttributeDelete" ClientIDMode="Static"/>
     <label for="customAttributeDelete"></label>
</div>

<br class="clear"/>
<br/>

<div class="size-10-2 column">
    Asset Types
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="assetTypeRead" ClientIDMode="Static"/>
     <label for="assetTypeRead"></label>
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="assetTypeUpdate" ClientIDMode="Static"/>
     <label for="assetTypeUpdate"></label>
</div>
<div class="size-10 column hidden-check">
    &nbsp;
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="assetTypeDelete" ClientIDMode="Static"/>
     <label for="assetTypeDelete"></label>
</div>

<br class="clear"/>
<br/>

<div class="size-10-2 column">
    Assets
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="assetRead" ClientIDMode="Static"/>
     <label for="assetRead"></label>
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="assetUpdate" ClientIDMode="Static"/>
     <label for="assetUpdate"></label>
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="assetArchive" ClientIDMode="Static"/>
     <label for="assetArchive"></label>
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="assetDelete" ClientIDMode="Static"/>
     <label for="assetDelete"></label>
</div>

<br class="clear"/>
<br/>

<div class="size-10-2 column">
    Images
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="imageRead" ClientIDMode="Static"/>
     <label for="imageRead"></label>
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="imageUpdate" ClientIDMode="Static"/>
     <label for="imageUpdate"></label>
</div>
<div class="size-10 column hidden-check">
    &nbsp;
</div>
<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="imageDelete" ClientIDMode="Static"/>
     <label for="imageDelete"></label>
</div>

<br class="clear"/>
<br/>

<hr/>
<br/>

    <div class="acl_header">
<div class="size-10 column">&nbsp;</div>
<div class="size-10 column"><h2>Upload</h2></div>
<div class="size-10 column"><h2>Deploy</h2></div>
<div class="size-10 column"><h2>Multicast</h2></div>

        </div>
    <br class="clear" />
    <div class="size-10-2 column">
    Image Tasks
</div>

<div class="size-10 column hidden-check">
        <asp:CheckBox runat="server" id="imageUploadTask" ClientIDMode="Static"/>

        <label for="imageUploadTask"></label>
</div>
<div class="size-10 column  hidden-check">
    <asp:CheckBox runat="server" Id="imageDeployTask" ClientIDMode="Static"/>
    <label for="imageDeployTask"></label>
</div>
<div class="size-10 column  hidden-check">
    <asp:CheckBox runat="server" Id="imageMulticastTask" ClientIDMode="Static"/>
    <label for="imageMulticastTask"></label>
</div>

<br class="clear"/>
<br/>

<hr/>
<br/>

    <div class="size-4 column">
    Activate / Deactivate Policies
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="policyActivate" ClientIDMode="Static"/>
     <label for="policyActivate"></label>
</div>

<br class="clear"/>
<br/>

<div class="size-4 column">
    Approve Provision Requests
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="approveProvision" ClientIDMode="Static"/>
     <label for="approveProvision"></label>
</div>

<br class="clear"/>
<br/>
<div class="size-4 column">
    Approve Reset Requests
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="approveReset" ClientIDMode="Static"/>
     <label for="approveReset"></label>
</div>

<br class="clear"/>
<br/>
    
<div class="size-4 column ">
    Send Messages
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="computerSendMessage" ClientIDMode="Static"/>
     <label for="computerSendMessage"></label>
</div>

<br class="clear"/>
<br/>
    <div class="size-4 column">
    Force Checkins
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="computerForceCheckin" ClientIDMode="Static"/>
     <label for="computerForceCheckin"></label>
</div>

<br class="clear"/>
<br/>
    
    <div class="size-4 column">
    Computer Reboot
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="computerReboot" ClientIDMode="Static"/>
     <label for="computerReboot"></label>
</div>

<br class="clear"/>
    <br/>
    
 <div class="size-4 column">
    Computer Shutdown
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="computerShutdown" ClientIDMode="Static"/>
     <label for="computerShutdown"></label>
</div>
    
    <br class="clear"/>
    <br/>
    
 <div class="size-4 column">
    Computer Wake Up
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="computerWakeup" ClientIDMode="Static"/>
     <label for="computerWakeup"></label>
</div>

<br class="clear"/>
    <br/>

 <div class="size-4 column ">
    Group Reboot
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="groupReboot" ClientIDMode="Static"/>
     <label for="groupReboot"></label>
</div>

<br class="clear"/>
    <br/>

     <div class="size-4 column">
    Group Shutdown
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="groupShutdown" ClientIDMode="Static"/>
     <label for="groupShutdown"></label>
</div>
    
     <br class="clear"/>
    <br/>
    
 <div class="size-4 column">
    Group Wake Up
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="groupWakeup" ClientIDMode="Static"/>
     <label for="groupWakeup"></label>
</div>
<br class="clear"/>
    <br/>

<div class="size-4 column">
    Upload Attachments
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="attachmentAdd" ClientIDMode="Static"/>
     <label for="attachmentAdd"></label>
</div>
<br class="clear"/>
<br/>

<div class="size-4 column">
    Download Attachments
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="attachmentRead" ClientIDMode="Static"/>
     <label for="attachmentRead"></label>
</div>
<br class="clear"/>
<br/>

<div class="size-4 column">
    Add Comments
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="commentAdd" ClientIDMode="Static"/>
     <label for="commentAdd"></label>
</div>
<br class="clear"/>
<br/>

<div class="size-4 column">
    Read Comments
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="commentRead" ClientIDMode="Static"/>
     <label for="commentRead"></label>
</div>
<br class="clear"/>
<br/>

<div class="size-4 column">
    Import Policy
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="policyImport" ClientIDMode="Static"/>
     <label for="policyImport"></label>
</div>
<br class="clear"/>
<br/>

<div class="size-4 column">
    Export Policy
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="policyExport" ClientIDMode="Static"/>
     <label for="policyExport"></label>
</div>
<br class="clear"/>
<br/>

<div class="size-4 column">
Module Upload Files
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="moduleUploadFiles" ClientIDMode="Static"/>
     <label for="moduleUploadFiles"></label>
</div>
<br class="clear"/>
<br/>

<div class="size-4 column">
Module Download External Files
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="moduleExternalFiles" ClientIDMode="Static"/>
     <label for="moduleExternalFiles"></label>
</div>
<br class="clear"/>
<br/>

<div class="size-4 column">
    Receive Pending Approval Report Email
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="emailApproval" ClientIDMode="Static"/>
     <label for="emailApproval"></label>
</div>
    
    <br class="clear"/>
<br/>
<div class="size-4 column">
    Receive Pending Reset Report Email
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="emailReset" ClientIDMode="Static"/>
     <label for="emailReset"></label>
</div>
<br class="clear"/>
<br/>
<div class="size-4 column">
    Receive SMART Failure Report Email
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="emailSmart" ClientIDMode="Static"/>
     <label for="emailSmart"></label>
</div>

<br class="clear"/>
<br/>
<div class="size-4 column">
    Receive Com Server Low Disk Space Report Email
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="emailLowDiskSpace" ClientIDMode="Static"/>
     <label for="emailLowDiskSpace"></label>
</div>

    <br class="clear"/>
<br/>
<div class="size-4 column">
    Receive Image Task Failed Email
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="emailImagingTaskFailed" ClientIDMode="Static"/>
     <label for="emailImagingTaskFailed"></label>
</div>

<br class="clear"/>
<br/>
<div class="size-4 column">
    Receive Image Task Completed Email
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="emailImagingTaskCompleted" ClientIDMode="Static"/>
     <label for="emailImagingTaskCompleted"></label>
</div>

<br class="clear"/>
<br/>
<div class="size-4 column">
    Update PXE Settings
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="pxeSettingsUpdate" ClientIDMode="Static"/>
     <label for="pxeSettingsUpdate"></label>
</div>

<br class="clear"/>
<br/>
<div class="size-4 column">
    Generate Client Boot ISO
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="pxeIsoGen" ClientIDMode="Static"/>
     <label for="pxeIsoGen"></label>
</div>

    <div class="size-4 column">
    Allow Remote Control
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="allowRemoteControl" ClientIDMode="Static"/>
     <label for="allowRemoteControl"></label>
</div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page is used to set the permissions for all members of the group. It has no effect if the group is in the administrator role. The permissions are mostly self explanatory, following the general navigational layout of Theopenem. There are also some more specific options below the first section.</p>
</asp:Content>