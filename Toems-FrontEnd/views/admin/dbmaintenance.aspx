<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" CodeBehind="dbmaintenance.aspx.cs" Inherits="Toems_FrontEnd.views.admin.dbmaintenance" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>DB Maintenance</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Admin Settings
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnUpdateSettings" runat="server" Text="Update Server Settings" OnClick="btnUpdateSettings_OnClick" CssClass="main-action"/></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#dbmaintenance').addClass("nav-current");
        });
    </script>
 
      <div class="size-4 column">
        Computers Auto Archive:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtComputerArchive" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Archived Computers Auto Delete:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtDeleteComputers" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Audit Log Auto Delete:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtAuditLogDelete" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Computer Process Auto Delete:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtComputerProcess" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Policy History Auto Delete:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtPolicyHistory" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        User Login History Auto Delete:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtUserLogin" runat="server" CssClass="textbox"></asp:TextBox>
    </div>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    
<p><strong>The remaining fields are used for the data cleanup task.  All values are specified in days.  0 is used to disable cleanup on that item.</strong></p>
<p>&nbsp;</p>
<h5><span style="color: #ff9900;">Computers Auto Archive:</span></h5>
<p>Automatically archives computers that have not checked in for X days.</p>
<h5><span style="color: #ff9900;">Archived Computers Auto Delete:</span></h5>
<p>Permanently deletes archived computers that have been archived longer than X days.</p>
<h5><span style="color: #ff9900;">Audit Log Auto Delete:<br />
</span></h5>
<p><strong>Deletes a user's WebUI history for any items that occurred longer than X days ago.<br />
</strong></p>
<h5><span style="color: #ff9900;">Computer Process Delete:</span></h5>
<p><strong>Deletes all computers user process history where the process was started longer than X days ago.<br />
</strong></p>
<h5><span style="color: #ff9900;">Policy History Delete:</span></h5>
<p><strong>Deletes the records of all computers policy history where the policy ran longer than X days ago.<br />
</strong></p>
<h5><span style="color: #ff9900;">User Login History Auto Delete:</span></h5>
<p><strong>Deletes the user login history data for all computers where the login was longer than X days ago.<br />
</strong></p>
</asp:Content>
