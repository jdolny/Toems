<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/softwaremodules/softwaremodule.master" AutoEventWireup="true" CodeBehind="general.aspx.cs" Inherits="Toems_FrontEnd.views.modules.softwaremodules.general" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li><%= SoftwareModule.Name %></li>
    <li>General</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
<%= SoftwareModule.Name %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Update Module" CssClass="main-action"/></li>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#general').addClass("nav-current");
        });
    </script>
    
    <div class="size-4 column">
        Display Name
    </div>

    <div class="size-5 column">
        <asp:TextBox ID="txtDisplayName" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>
      <div class="size-4 column">
        GUID
    </div>

    <div class="size-5 column">
        <asp:TextBox ID="txtGuid" runat="server" CssClass="textbox" Style="font-size:14px;" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>
      <div class="size-4 column">
        Description
    </div>

    <div class="size-5 column">
        <asp:TextBox ID="txtDescription" runat="server" CssClass="descbox" TextMode="MultiLine"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Command
    </div>

     <div class="size-5 column">
        <asp:TextBox ID="txtCommand" runat="server" CssClass="textbox" Style="font-size:12px;" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Arguments
    </div>

     <div class="size-5 column">
        <asp:TextBox ID="txtArguments" runat="server" CssClass="textbox" Style="font-size:12px;" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Additional Arguments
    </div>

     <div class="size-5 column">
        <asp:TextBox ID="txtAddArguments" runat="server" CssClass="textbox" Style="font-size:12px;" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Timeout (Minutes)
    </div>

     <div class="size-5 column">
        <asp:TextBox ID="txtTimeout" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>

    </div>
    <br class="clear" />
    <div class="size-4 column">
        MSI Type
    </div>

    <div class="size-5 column">
        <asp:DropDownList ID="ddlType" runat="server" CssClass="ddlist">
          
            </asp:DropDownList>
    </div>
    <br class="clear"/>
      <div class="size-4 column">
        Log Standard Output
    </div>

     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkStdOut" runat="server" ClientIDMode="Static"></asp:CheckBox>
           <label for="chkStdOut">Toggle</label>
        </div>
     <br class="clear"/>

      <div class="size-4 column">
        Log Standard Error
    </div>

     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkStdError" runat="server" ClientIDMode="Static"></asp:CheckBox>
           <label for="chkStdError">Toggle</label>
        </div>
     <br class="clear"/>
     <div class="size-4 column">
        Success Codes
    </div>

    <div class="size-5 column">
        <asp:TextBox ID="txtSuccessCodes" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>
        <div class="size-4 column">
        Run As
    </div>

    <div class="size-5 column">
        <asp:DropDownList ID="ddlRunAs" runat="server" CssClass="ddlist"/>
    </div>

    <br class="clear"/>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubSubHelp">
    Software Modules can only be assigned to Policies.
    <h5><span style="color: #ff9900;"><strong>Display Name:</strong></span></h5>
<p>The name of the module, module names must be unique and contain only alphanumeric characters, space, underscore, or dash.</p>
<h5><span style="color: #ff9900;"><strong>Guid:</strong></span></h5>
<p>Each module is automatically assigned a guid for an id.  This is read only and only for reference.</p>
<h5><span style="color: #ff9900;"><strong>Description:</strong></span></h5>
<p>The description field is optional for you to give a short description for what the module does.</p>
<h5><span style="color: #ff9900;"><strong>Command:</strong></span></h5>
<p>The command field is automatically populated after files are added to the module, and should not be modified.</p>
<h5><span style="color: #ff9900;"><strong>Arguments:</strong></span></h5>
<p>The arguments field is also automatically populated after files are added to the module, and should not be modified.</p>
<h5><span style="color: #ff9900;"><strong>Additional Arguments:</strong></span></h5>
<p>Additional Arguments are where the user can define any extra parameters the msi should run with.  This defaults to /q /norestart.</p>
<h5><span style="color: #ff9900;"><strong>Timeout:</strong></span></h5>
<p>A timeout value where the installation should give up, specified in minutes.  The default value is 0 or unlimited.</p>
<h5><span style="color: #ff9900;"><strong>MSI Type:</strong></span></h5>
<p>Specifies the action the software module should do.  Install, Uninstall, or Patch.</p>
<div class="alert alert-info">When using the Patch msi type, it is expected that there is no msi file, only msp.  If you are installing an msi together with a patch, you should use the Install option.</div>
<div class="alert alert-error">Not all msi's adhere to the install, uninstall guidelines.  For example, the Winzip msi will always uninstall the application if it is already installed, even if the msi type is set to install.  You must be careful when deploying msi's, because it also depends on how that msi was created.</div>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Log Standard Output:</strong></span></h5>
<p class="alert alert-error">If this is enabled, any output from the installation will try to be written to the log file on the client.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Log Standard Error:</strong></span></h5>
<p class="alert alert-error">If this is enabled, any errors from the installation will try to be written to the log file on the client.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Success Codes:</strong></span></h5>
<p class="alert alert-error">Success codes are very important.  The only way Theopenem can recognize if an installation was successful is by the exit code that is returned to it.  If this exit code matches any number in this field, it is marked as successful. The default value is 0,1641,3010 which covers most applications.  You should modify this field only if you know that application uses a different exit code success value.  Each value must be separated by only a single comma.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Run As:</strong></span></h5>
<p class="alert alert-error">By default, msi installation run as the local system account.  If you need to modify this behavior, you can create an impersonation account under Admin-&gt;Impersonation Accounts, and assign it here.</p>
</asp:Content>