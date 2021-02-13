<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" CodeBehind="provisiontasks.aspx.cs" Inherits="Toems_FrontEnd.views.admin.provisiontasks" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Provision Tasks</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Admin Settings
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" runat="Server">
    <li><asp:LinkButton ID="btnUpdateSettings" runat="server" Text="Update Client Settings" OnClick="btnUpdateSettings_OnClick" CssClass="main-action"/></li>
</asp:Content>





<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#provision').addClass("nav-current");
        });
    </script>
     <div class="size-4 column">
            Check Active Directory OU Membership:
    </div>
   
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkldap" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkldap"></label>
    </div>
    <br class="clear"/>

      <div class="size-4 column">
       Default Group Membership:
     </div>
     <div class="size-5 column">
        <asp:TextBox ID="txtGroup" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
       
    

   

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>These tasks are run immediately after an endpoint provisions for the first time, before it's first checkin.</p>
    <h5><span style="color: #ff9900;">Check Active Directory OU Membership:</span></h5>
<p>If this is enabled, the endpoint will be added to the corresponding OU group for Theopenem if LDAP synchronization is enabled.  This feature is 
    useful to immediately get the computer into an OU group to have the policies for that group run on the endpoint, instead of waiting
    for the next LDAP sync.</p>
     <h5><span style="color: #ff9900;">Default Group Membership:</span></h5>
    <p>If an existing Theopenem group name is entered here, the endpoint will be added to the group after it is provisioned for the first time.
        This will allow you to run policies immediately after the endpoint provisions.  This is similar to the setting above but is useful if you are not
        using LDAP synchronization.
    </p>
</asp:Content>
