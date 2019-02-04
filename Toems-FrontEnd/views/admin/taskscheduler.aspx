<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" CodeBehind="taskscheduler.aspx.cs" Inherits="Toems_FrontEnd.views.admin.taskscheduler" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Task Scheduler</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Admin Settings
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" runat="Server">
    <li><asp:LinkButton ID="btnUpdateSettings" runat="server" Text="Update Tasks" OnClick="btnUpdateSettings_OnClick" CssClass="main-action"/></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#tasks').addClass("nav-current");
           
        });
    </script>
    
      <div id="settings">
          <div class="size-cron2-header column">
           &nbsp;
        </div>
        <div class="size-cron2-header column">
          
            <asp:Label runat="server" ID="Label4" Text="Cron Expression"></asp:Label>
        </div>
              <div class="size-cron2-header column">
              <asp:Label runat="server" ID="Label1" Text="Last Run Time"></asp:Label>
              </div>
          <div class="size-cron2-header column">
              <asp:Label runat="server" ID="Label2" Text = "Last Run Status"></asp:Label>
              </div>
         <div class="size-cron2-header column">
              <asp:Label runat="server" ID="Label3" Text ="Next Run Time"></asp:Label>
              </div>
        <br class="clear"/>
           <div class="size-cron2 column">
            Storage Sync:
        </div>

        <div class="size-cron column">
            <asp:TextBox ID="txtFolderSync" runat="server" CssClass="textbox margin-top-10" ></asp:TextBox>
              
        </div>
              <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblLastRun"></asp:Label>
              </div>
          <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblStatus"></asp:Label>
              </div>
         <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblNextRun"></asp:Label>
              </div>
          <div class="size-cron column">
           <asp:LinkButton ID="btnFolderSync" runat="server" Text="Run Now" OnClick="btnFolderSync_OnClick" CssClass="btn white margin-top-5"/>
        </div>
        
         <br class="clear"/>
           <div class="size-cron2 column">
            LDAP Sync:
        </div>
        <div class="size-cron column">
            <asp:TextBox ID="txtLdap" CssClass="textbox margin-top-10" runat="server" ></asp:TextBox>
              
        </div>
              <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblLdapLastRun"></asp:Label>
              </div>
          <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblLdapStatus"></asp:Label>
              </div>
         <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblLdapNextRun"></asp:Label>
              </div>
          <div class="size-cron column">
           <asp:LinkButton ID="btnLdapSync" runat="server" Text="Run Now" OnClick="btnLdapSync_OnClick" CssClass="btn white margin-top-5"/>
        </div>
        
          <br class="clear"/>
           <div class="size-cron2 column">
            Dynamic Group Update:
        </div>
        <div class="size-cron column">
            <asp:TextBox ID="txtGroup" runat="server" CssClass="textbox margin-top-10"></asp:TextBox>
              
        </div>
              <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblGroupLastRun"></asp:Label>
              </div>
          <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblGroupStatus"></asp:Label>
              </div>
         <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblGroupNextRun"></asp:Label>
              </div>
          <div class="size-cron column">
           <asp:LinkButton ID="btnGroup" runat="server" Text="Run Now" OnClick="btnGroup_OnClick" CssClass="btn white margin-top-5"/>
        </div>
          
          
               <br class="clear"/>
           <div class="size-cron2 column">
            Reset Request Report:
        </div>
        <div class="size-cron column">
            <asp:TextBox ID="txtReset" runat="server" CssClass="textbox margin-top-10"></asp:TextBox>
              
        </div>
              <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblResetLastRun"></asp:Label>
              </div>
          <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblResetStatus"></asp:Label>
              </div>
         <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblResetNextRun"></asp:Label>
              </div>
          <div class="size-cron column">
           <asp:LinkButton ID="btnReset" runat="server" Text="Run Now" OnClick="btnReset_OnClick" CssClass="btn white margin-top-5"/>
        </div>
          
                 <br class="clear"/>
           <div class="size-cron2 column">
            Approval Request Report:
        </div>
        <div class="size-cron column">
            <asp:TextBox ID="txtApproval" runat="server" CssClass="textbox margin-top-10"></asp:TextBox>
              
        </div>
              <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblApprovalLastRun"></asp:Label>
              </div>
          <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblApprovalStatus"></asp:Label>
              </div>
         <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblApprovalNextRun"></asp:Label>
              </div>
          <div class="size-cron column">
           <asp:LinkButton ID="btnApproval" runat="server" Text="Run Now" OnClick="btnApproval_OnClick" CssClass="btn white margin-top-5"/>
        </div>
          
              <br class="clear"/>
           <div class="size-cron2 column">
            S.M.A.R.T. Report:
        </div>
        <div class="size-cron column">
            <asp:TextBox ID="txtSmart" runat="server" CssClass="textbox margin-top-10"></asp:TextBox>
              
        </div>
              <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblSmartLastRun"></asp:Label>
              </div>
          <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblSmartStatus"></asp:Label>
              </div>
         <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblSmartNextRun"></asp:Label>
              </div>
          <div class="size-cron column">
           <asp:LinkButton ID="btnSmart" runat="server" Text="Run Now" OnClick="btnSmart_OnClick" CssClass="btn white margin-top-5"/>
        </div>
          
            <br class="clear"/>
           <div class="size-cron2 column">
            Data Cleanup:
        </div>
        <div class="size-cron column">
            <asp:TextBox ID="txtDataCleanup" runat="server" CssClass="textbox margin-top-10 "></asp:TextBox>
              
        </div>
              <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblDataCleanupLastRun"></asp:Label>
              </div>
          <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblDataCleanupStatus"></asp:Label>
              </div>
         <div class="size-cron2 column">
              <asp:Label runat="server" ID="lblDataCleanupNextRun"></asp:Label>
              </div>
          <div class="size-cron column">
           <asp:LinkButton ID="btnDataCleanup" runat="server" Text="Run Now" OnClick="btnDataCleanup_OnClick" CssClass="btn white margin-top-5"/>
        </div>
          
       
          
            <br class="clear"/>
     
          

    </div>

       
        
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>The task scheduler displays a series of tasks Theopenem will run on a recurring schedule.  If you are not familiar with Cron Expressions, there is an easy expression tester at <a href="https://crontab.guru/" target="_blank" rel="noopener">https://crontab.guru/</a> to test your expressions.  Any of the tasks can be modified to run at the times you want.</p>
<p>&nbsp;</p>
<h5><span style="color: #ff9900;">Storage Sync:</span></h5>
<p>The storage sync replicates the remote SMB storage with all Com Servers.  The default value is <strong>30 */4 * * *</strong> which runs every 4 hours at half past the hour.</p>
<h5><span style="color: #ff9900;">LDAP Sync:</span></h5>
<p>LDAP sync synchronizes computers from your AD to Theopenem.  The default value is <strong>0 1 * * *</strong> which runs every day at 1:00 AM.</p>
<h5><span style="color: #ff9900;">Dynamic Group Update:</span></h5>
<p>Scans all dynamic groups adding and removing group members.  The default value is <strong>15 */3 * * *</strong> which runs every 3 hours at 15 minutes past the hour.  If this task runs too frequently it could severely degrade performance on the server if you have a lot of dynamic groups.</p>
<h5><span style="color: #ff9900;">Reset Request Report:<br />
</span></h5>
<p>This sends out an email if any computers are waiting for a reset approval.  The default value is <strong>0 7,15 * * * which means a report will sent out twice per day at 7:00 AM and 3:00 PM.</strong></p>
<h5><span style="color: #ff9900;">Approval Request Report:</span></h5>
<p>This sends out an email if any computers are waiting for provision approval.  The default value is <strong>0 7,15 * * * which means a report will sent out twice per day at 7:00 AM and 3:00 PM.</strong></p>
<h5><span style="color: #ff9900;">S.M.A.R.T Report:</span></h5>
<p>This sends out an email if any computers have a hard drive with a failed SMART status.  The default value is <strong>0 2 * * * which means a report will be sent daily at 2:00 AM.</strong></p>
<h5><span style="color: #ff9900;">Data Cleanup:</span></h5>
<p>The data cleanup is a process that tries to keep the database clean, deleting entries that are old, specified by values you define in Admin Settings-&gt;Server.  The default value is <strong>0 4 * * * which means it will run daily at 4:00 AM.</strong></p>
</asp:Content>