<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/softwaremodules/softwaremodule.master" AutoEventWireup="true" CodeBehind="create.aspx.cs" Inherits="Toems_FrontEnd.views.modules.softwaremodules.create" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>New</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
 Software Modules
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="btnAdd" runat="server" OnClick="btnAdd_OnClick" Text="Add Module" CssClass="main-action"/></li>
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
    <br class="clear"/>
    
     <div class="size-4 column">
        Timeout (Minutes)
    </div>

     <div class="size-5 column">
        <asp:TextBox ID="txtTimeout" runat="server" CssClass="textbox" ClientIDMode="Static" Text="0"></asp:TextBox>

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
        <asp:TextBox ID="txtSuccessCodes" runat="server" CssClass="textbox" ClientIDMode="Static" Text="0,1641,3010"></asp:TextBox>
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
The name of the module, module names must be unique and contain only alphanumeric characters, space, underscore, or dash.

<h5><span style="color: #ff9900;"><strong>Description:</strong></span></h5>
The description field is optional for you to give a short description for what the module does.
<h5><span style="color: #ff9900;"><strong>Timeout:</strong></span></h5>
A timeout value where the installation should give up, specified in minutes.  The default value is 0 or unlimited.
<h5><span style="color: #ff9900;"><strong>MSI Type:</strong></span></h5>
Specifies the action the software module should do.  Install, Uninstall, or Patch.
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
&nbsp;

</asp:Content>