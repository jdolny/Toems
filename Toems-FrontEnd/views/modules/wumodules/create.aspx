<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/wumodules/wumodule.master" AutoEventWireup="true" CodeBehind="create.aspx.cs" Inherits="Toems_FrontEnd.views.modules.wumodules.create" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>New</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
 Windows Update Modules
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="btnAdd" runat="server" OnClick="btnAdd_OnClick" Text="Add Module" CssClass="main-action" /></li>
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
    
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    Windows Update Modules can only be assigned to Policies
    <h5><span style="color: #ff9900;"><strong>Display Name:</strong></span></h5>
<p>The name of the module, module names must be unique and contain only alphanumeric characters, space, underscore, or dash.</p>

<h5><span style="color: #ff9900;"><strong>Description:</strong></span></h5>
<p>The description field is optional for you to give a short description for what the module does.</p>
<h5><span style="color: #ff9900;"><strong>Timeout:</strong></span></h5>
<p>A timeout value where the update should give up, specified in minutes. The default value is 0 or unlimited.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Log Standard Output:</strong></span></h5>
<p class="alert alert-error">If this is enabled, any output from the installation will try to be written to the log file on the client.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Log Standard Error:</strong></span></h5>
<p class="alert alert-error">If this is enabled, any errors from the installation will try to be written to the log file on the client.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Success Codes:</strong></span></h5>
<p class="alert alert-error">Success codes are very important. The only way Theopenem can recognize if an installation was successful is by the exit code that is returned to it. If this exit code matches any number in this field, it is marked as successful. The default value is 0,1641,3010 which covers most applications. You should modify this field only if you know that application uses a different exit code success value. Each value must be separated by only a single comma.</p>
<p>&nbsp;</p>


</asp:Content>

