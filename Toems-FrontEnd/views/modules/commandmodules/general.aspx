<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/commandmodules/commandmodule.master" AutoEventWireup="true" CodeBehind="general.aspx.cs" Inherits="Toems_FrontEnd.views.modules.commandmodules.general" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li><%= CommandModule.Name %></li>
    <li>General</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
<%= CommandModule.Name %>
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
        Working Directory
    </div>

     <div class="size-5 column">
        <asp:TextBox ID="txtWorkingDirectory" runat="server" CssClass="textbox" Style="font-size:12px;" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Timeout (Minutes)
    </div>

     <div class="size-5 column">
        <asp:TextBox ID="txtTimeout" runat="server" CssClass="textbox" ClientIDMode="Static" Text="0"></asp:TextBox>

    </div>
    <br class="clear" />
      <div class="size-4 column">
        Log Standard Output
    </div>

     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkStdOut" runat="server" ClientIDMode="Static"></asp:CheckBox>
            <label for="chkStdOut"></label>   
     </div>
     <br class="clear"/>

      <div class="size-4 column">
        Log Standard Error
    </div>

     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkStdError" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkStdError"></label>
        </div>
     <br class="clear"/>
     <div class="size-4 column">
        Success Codes
    </div>

    <div class="size-5 column">
        <asp:TextBox ID="txtSuccessCodes" runat="server" CssClass="textbox" ClientIDMode="Static" Text="0"></asp:TextBox>
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
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <h5><span style="color: #ff9900;"><strong>Display Name:</strong></span></h5>
<p>The name of the module, module names must be unique and contain only alphanumeric characters, space, underscore, or dash.</p>
<h5><span style="color: #ff9900;"><strong>Guid:</strong></span></h5>
<p>Each module is automatically assigned a guid for an id. This is read only and only for reference.</p>
<h5><span style="color: #ff9900;"><strong>Description:</strong></span></h5>
<p>The description field is optional for you to give a short description for what the module does.</p>
<h5><span style="color: #ff9900;"><strong>Command:</strong></span></h5>
<p>The command to execute.  No arguments should be included in this field.  If the command's path is included in the system environmental variables path, then only the command name is needed.  Otherwise, the full path to the command is needed.  The command field has one special option.  A file copy module's cache path can be specified with [module-{module-guid}].  For example, if you needed to run a command called test.exe that first needs to be copied to the endpoint, you would create a file copy module with a blank destination, then reference that module with something like [module-36d65c9e-6f73-4a84-a54d-2ba8e3ef349c]test.exe.</p>
<h5><span style="color: #ff9900;"><strong>Arguments:</strong></span></h5>
<p>The arguments field is used to pass any arguments to the command.  For example if you wanted to disable hibernation, the command would be powercfg.exe and the arguments would be <strong>/h off</strong>.</p>
<h5><span style="color: #ff9900;"><strong>Working Directory:</strong></span></h5>
<p>The directory the command is run from.  If this is left blank it default to %SYSTEMROOT%\system32.</p>
<h5><span style="color: #ff9900;"><strong>Timeout:</strong></span></h5>
<p>A timeout value where the command should give up, specified in minutes.  The default value is 0 or unlimited.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Log Standard Output:</strong></span></h5>
<p class="alert alert-error">If this is enabled, any output from the installation will try to be written to the log file on the client.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Log Standard Error:</strong></span></h5>
<p class="alert alert-error">If this is enabled, any errors from the installation will try to be written to the log file on the client.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Success Codes:</strong></span></h5>
<p class="alert alert-error">Success codes are very important.  The only way Theopenem can recognize if an installation was successful is by the exit code that is returned to it.  If this exit code matches any number in this field, it is marked as successful. The default value for a command module is 0.  You should modify this field only if you know that command uses a different exit code success value.  Each value must be separated by only a single comma.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Run As:</strong></span></h5>
<p class="alert alert-error">By default, commands run as the local system account.  If you need to modify this behavior, you can create an impersonation account under Admin-&gt;Impersonation Accounts, and assign it here.</p>
</asp:Content>
