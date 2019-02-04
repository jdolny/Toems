<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" Inherits="Toems_FrontEnd.views.admin.views_admin_email" Codebehind="email.aspx.cs" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>E-Mail</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Admin Settings
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnUpdateSettings" runat="server" Text="Update E-Mail Settings" OnClick="btnUpdateSettings_OnClick" CssClass="main-action"/></li>
     <li><asp:LinkButton ID="btnTestEmail" runat="server" Text="Send Test Message" OnClick="btnTestMessage_Click" /></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#email').addClass("nav-current");
        });
    </script>
    <div class="size-4 column">
        Mail Enabled:
    </div>
    <div class="size-5 column hidden-check">
        <asp:CheckBox runat="server" id="chkEnabled" ClientIDMode="Static"/>
        <label for="chkEnabled"></label>
    </div>
    <br class="clear"/>
    <br/>
    <div class="size-4 column">
        Smtp Server:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtSmtpServer" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>

    <div class="size-4 column">
        Smtp Port:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtSmtpPort" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Smtp SSL:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlSmtpSsl" runat="server" CssClass="ddlist">
            <asp:ListItem>Yes</asp:ListItem>
            <asp:ListItem>No</asp:ListItem>
        </asp:DropDownList>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Smtp Mail From:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtSmtpFrom" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Smtp Mail To:
        <p style="font-size: 12px;">Only For Test Message</p>
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtSmtpTo" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Smtp Username:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtSmtpUsername" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Smtp Password:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtSmtpPassword" runat="server" CssClass="textbox password" TextMode="Password"></asp:TextBox>
    </div>
    <br class="clear"/>


 

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>The E-mail page is where your SMTP server is defined.  This is required to receive notifications / reports from your Theopenem server.  The actions button has a send test message option, you must first update the email settings before sending the test message.</p>
<h5><span style="color: #ff9900;">Mail Enabled:</span></h5>
<p>Enables or disables the email functionality in Theopenem.</p>
<h5><span style="color: #ff9900;">Smtp Server:</span></h5>
<p>The ip or fqdn of your smtp server.</p>
<h5><span style="color: #ff9900;">Smtp Port:</span></h5>
<p>The port of your smtp server, usually 25 or 587.</p>
<h5><span style="color: #ff9900;">Smtp SSL:</span></h5>
<p>If your smtp server supports / requires SSL, then set this to yes.</p>
<h5><span style="color: #ff9900;">Smtp Mail From:</span></h5>
<p>The address that email notifications should come from.  Something like <strong>theopenem@yourdomain</strong> is a good choice.</p>
<h5><span style="color: #ff9900;">Smtp Mail To:</span></h5>
<p>This is the mail to address that is used for sending a test message.</p>
<h5><span style="color: #ff9900;">Smtp Username:</span></h5>
<p>If your smtp server requires authentication, enter the username here.</p>
<h5><span style="color: #ff9900;">Smtp Password:</span></h5>
<p>If your smtp server requires authentication, enter the password here.</p>
</asp:Content>