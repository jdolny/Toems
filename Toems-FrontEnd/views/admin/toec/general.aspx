<%@ Page Title="" Language="C#" MasterPageFile="~/theopenem/views/admin/toec/toec.master" AutoEventWireup="true" CodeBehind="general.aspx.cs" Inherits="Toems_FrontEnd.views.admin.toec.general" %>
<asp:Content ID="Content3" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
    <li><a href="<%= ResolveUrl("~/views/admin/toec/general.aspx") %>?level=2">General Toec Settings</a></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    General Toec Settings
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
      <li><asp:LinkButton ID="btnUpdateSettings" runat="server" Text="Update Client Settings" OnClick="btnUpdateSettings_OnClick" CssClass="main-action"/></li>
    <li><asp:LinkButton ID="btnExportMsi" runat="server" Text="Export Client Msi 64-Bit" OnClick="btnExportMsi64_Click" CssClass="main-action"/></li>
      <li><asp:LinkButton ID="btnExportMsi32" runat="server" Text="Export Client Msi 32-Bit" OnClick="btnExportMsi32_Click" CssClass="main-action"/></li>
          <li><asp:LinkButton ID="btnCopyToec" runat="server" Text="Prepare Toec Updates" OnClick="btnCopyToec_Click" CssClass="main-action"/></li>

</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#general').addClass("nav-current");
        });
    </script>
     <div class="size-4 column">
            Startup Delay Type:
        </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlStartupDelay" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlStartupDelay_OnSelectedIndexChanged"/>
        </div>
        <br class="clear"/>
     <div id="divDelayFilePath" runat="server">
            <div class="size-4 column">
                File Path:
            </div>
            <div class="size-5 column">
                <asp:TextBox ID="txtStartupFilePath" runat="server" CssClass="textbox"></asp:TextBox>
            </div>
            <br class="clear" />
        </div>
        <div id="divDelayTime" runat="server">
            <div class="size-4 column">
                Time (Seconds):
            </div>
            <div class="size-5 column">
                <asp:TextBox ID="txtDelayTime" runat="server" CssClass="textbox"></asp:TextBox>
            </div>
            <br class="clear" />
        </div>
 
        <div class="size-4 column">
            Threshold Window (Seconds):
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtThreshold" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
        <br class="clear"/>
    
          <div class="size-4 column">
            Checkin Interval (Minutes):
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtCheckin" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
    
           <br class="clear"/>

          <div class="size-4 column">
            Toec Remote Install Max Works:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtMaxWorkers" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
        <br class="clear"/>
    
          <div class="size-4 column">
            Shutdown Delay (Seconds):
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtShutdownDelay" runat="server" CssClass="textbox"></asp:TextBox>
        </div>

        <br class="clear"/>
       
    <div class="size-4 column">
            Domain Name:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtDomainName" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
        <br class="clear"/>

          <div class="size-4 column">
            Domain Join Username (NetBIOS Format):
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtDomainUsername" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
        <br class="clear"/>
     <div class="size-4 column">
            Domain Join Password:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtDomainPassword" runat="server" CssClass="textbox password" TextMode="Password"></asp:TextBox>
        </div>
        <br class="clear"/>
    
        <div class="size-4 column">
            Client MSI Arguments:
        </div>
   
    <br class="clear" />
     
        <div class="size-5 column">
            <asp:TextBox ID="txtArguments" runat="server" CssClass="descbox" TextMode="MultiLine" Style="font-size:12px;"></asp:TextBox>
        </div>
        <br class="clear"/>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
        <h5><span style="color: #ff9900;">Startup Delay Type:</span></h5>
<p>When the Toec service starts, an optional delay can be configured to delay the initial checkin time.  The delay can be specified in seconds or by a file condition.  A file condition means that Toec will not checkin until the specified File Path exists.  An example usage for the File Condition might be a long Sysprep workflow.  You might not want Toec to checkin until everything else is completed.  One of the last steps in your Sysprep workflow would be to create this file and the specified File Path Condition.</p>
<h5><span style="color: #ff9900;">Threshold Window:</span></h5>
<p>The threshold window is used to try and prevent all of computers from checking in at the same time.  If the Window value is greater than 0, each client will wait to checkin by selecting a random number b/w 0 and the value specified in seconds before checking in.</p>
<h5><span style="color: #ff9900;">Checkin Interval:</span></h5>
<p>This specifies how often computers should Checkin.  The recommended value is 60 minutes.</p>
<h5><span style="color: #ff9900;">Shutdown Delay:</span></h5>
<p>When a computer or group is issued a reboot or shutdown command via the actions menu, this will delay the shutdown by the provided value on the computer, giving a user time to save work if they are still logged in.  This does not apply to a Policy's reboot option.</p>
<h5><span style="color: #ff9900;">Upload New Client MSI Version:</span></h5>
<p>When updates to Toec are available, the are uploaded here.  Existing Toec endpoints will automatically update themselves at the next checkin.</p>
<h5><span style="color: #ff9900;">Client MSI Arguments:</span></h5>
<p>Displays the arguments that must be passed to the Toec MSI installer.</p>
</asp:Content>
