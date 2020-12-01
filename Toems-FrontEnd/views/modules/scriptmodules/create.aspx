<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/scriptmodules/scriptmodule.master" AutoEventWireup="true" CodeBehind="create.aspx.cs" ValidateRequest="False" Inherits="Toems_FrontEnd.views.modules.scriptmodules.create" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>New</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
 Script Modules
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="btnAdd" runat="server" OnClick="btnAdd_OnClick" Text="Add Module" CssClass="main-action" OnClientClick="update_click()" /></li>
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#create').addClass("nav-current");
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
        Description
    </div>

    <div class="size-5 column">
        <asp:TextBox ID="txtDescription" runat="server" CssClass="descbox" TextMode="MultiLine"></asp:TextBox>
    </div>
     <br class="clear" />
    <div class="size-4 column">
        Script Type
    </div>

    <div class="size-5 column">
        <asp:DropDownList ID="ddlType" runat="server" CssClass="ddlist" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
            </asp:DropDownList>
    </div>
   <br class="clear"/>

    <div id="divNotBash" runat="server">
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
        Add To Inventory Collection
    </div>

     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkInventory" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkInventory"></label>
        </div>
     <br class="clear"/>
    <div class="size-4 column">
        Use As Condition
    </div>

     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkCondition" runat="server" ClientIDMode="Static"></asp:CheckBox>
            <label for="chkCondition"></label>
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
        </div>
    <br class="clear"/>
    <div class="size-4 column">
        Script Contents
    </div>
    <br class="clear"/>
    <div id="aceEditor" runat="server">
        <br class="clear"/>
        <pre id="editor" class="editor height_1200"></pre>
        <asp:HiddenField ID="scriptEditor" runat="server"/>


        <script>

            var editor = ace.edit("editor");
            editor.session.setValue($('#<%= scriptEditor.ClientID %>').val());

            editor.setTheme("ace/theme/idle_fingers");
            editor.getSession().setMode("ace/mode/sh");
            editor.setOption("showPrintMargin", false);
            editor.session.setUseWrapMode(true);
            editor.session.setWrapLimitRange(120, 120);


            function update_click() {
                var editor = ace.edit("editor");
                $('#<%= scriptEditor.ClientID %>').val(editor.session.getValue());
            }

        </script>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    Script Modules can be assigned to both Policies and Image Profiles.
    <h5><span style="color: #ff9900;"><strong>Display Name:</strong></span></h5>
<p>The name of the module, module names must be unique and contain only alphanumeric characters, space, underscore, or dash.</p>

<h5><span style="color: #ff9900;"><strong>Description:</strong></span></h5>
<p>The description field is optional for you to give a short description for what the module does.</p>
<h5><span style="color: #ff9900;"><strong>Script Type:</strong></span></h5>
<p>There are 5 script type options, Powershell, VbScript, Batch, ImagingClientBash, and ImagingClientPowershell.  The first 3 are used with Toec and can be assigned to Policies.  The last 2 are
    specific to Client Imaging.  If you want to run a custom script while using the Linux Imaging Environment, select ImagingClientBash.  If you want to run a custom script while using the WinPE Imaging
    Environment, select ImagingClientPowershell.  The script contents must be for the appropriate script type.</p>
<h5><span style="color: #ff9900;"><strong>Arguments:</strong></span></h5>
<p>The arguments field is used to pass any arguments to the script.</p>
<h5><span style="color: #ff9900;"><strong>Working Directory:</strong></span></h5>
<p>The directory the script is run from. If this is left blank it default to %SYSTEMROOT%\system32.</p>
<h5><span style="color: #ff9900;"><strong>Timeout:</strong></span></h5>
<p>A timeout value where the script should give up, specified in minutes. The default value is 0 or unlimited.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Log Standard Output:</strong></span></h5>
<p class="alert alert-error">If this is enabled, any output from the installation will try to be written to the log file on the client.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Log Standard Error:</strong></span></h5>
<p class="alert alert-error">If this is enabled, any errors from the installation will try to be written to the log file on the client.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Add To Inventory Collection:</strong></span></h5>
<p>When this is enabled, the result of the script will be added as a custom inventory attribute.  It will then be viewable under the computer's custom inventory page.  This data is also usable in the custom reporting page.  When using this option, whatever is written on screen as the last line is what is collected.  The following example collects a registry value that specifies if the computer requires Ctrl+Alt+Delete before login.</p>
<pre>Write-Host (Get-ItemProperty -Path "HKLM:SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon" -Name DisableCAD).DisableCAD</pre>
<p>This example script is only a single line, but your script can be as long as needed to obtain the required information, then just do a Write-Host with the value you need as the last line.</p>
<h5><span style="color: #ff9900;">Use As Condition:</span></h5>
<p>When this option is enabled, the script result can be used as a condition for a policy to continue or exit.  Policy module's can be set to run in a specific order, the script module condition can be ordered to test the condition at any order, but is typically set to run first.  To use this option, you must set the success code to be a non standard exit code, that will match the condition met exit code in your script.  The value of -1 is typically recommended.  Leaving the default value of 0 could result in false positives.  The following example checks if the system is 64-bit, if so it exits with code -1, signifying success and the policy should continue.  If the system is not 64-bit, the policy would exit skipping any remaining assigned modules.  Since the script is using -1 as success, the success code for this module must be set to -1.</p>
<pre>$arch = (Get-WmiObject Win32_OperatingSystem).OSArchitecture
if($arch -eq "64-bit")
{
Exit -1
}</pre>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Success Codes:</strong></span></h5>
<p class="alert alert-error">Success codes are very important. The only way Theopenem can recognize if an installation was successful is by the exit code that is returned to it. If this exit code matches any number in this field, it is marked as successful. The default value for a command module is 0. You should modify this field only if you know that command uses a different exit code success value. Each value must be separated by only a single comma.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Run As:</strong></span></h5>
<p class="alert alert-error">By default, scripts run as the local system account. If you need to modify this behavior, you can create an impersonation account under Admin-&gt;Impersonation Accounts, and assign it here.</p>
<h5><span style="color: #ff9900;">Script Contents:</span></h5>
<p>Enter the script in this box.  Scripts are not uploaded as files, instead they entered in the Script Contents box and stored in the database.</p>
</asp:Content>
