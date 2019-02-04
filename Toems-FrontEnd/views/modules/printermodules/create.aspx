<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/printermodules/printermodule.master" AutoEventWireup="true" CodeBehind="create.aspx.cs" Inherits="Toems_FrontEnd.views.modules.printermodules.create" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>New</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
 Printer Modules
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
        Display Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtDisplayName" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>
      <div class="size-4 column">
        Description:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtDescription" runat="server" CssClass="descbox" TextMode="MultiLine"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Network Path:
    </div>
     <div class="size-5 column">
        <asp:TextBox ID="txtPath" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>

    </div>
    
    <br class="clear" />
    <div class="size-4 column">
        Action:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlType" runat="server" CssClass="ddlist">
           
            </asp:DropDownList>
    </div>
    
     <br class="clear"/>
         <div class="size-4 column">
            Set As Default:
        </div>
        <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkDefault" runat="server" ClientIDMode="Static"></asp:CheckBox>
            <label for="chkDefault"></label>
        </div>
    
     <br class="clear"/>
         <div class="size-4 column">
            Restart Print Spooler:
        </div>
        <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkRestartSpooler" runat="server" ClientIDMode="Static"></asp:CheckBox>
            <label for="chkRestartSpooler"></label>
        </div>
    
        
     <br class="clear"/>
         <div class="size-4 column">
            Wait For Enumeration:
        </div>
        <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkEnumeration" runat="server" Checked="True" ClientIDMode="Static"></asp:CheckBox>
            <label for="chkEnumeration"></label>
        </div>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <h5><span style="color: #ff6600;"><strong>Display Name:</strong></span></h5>
<p>The name of the module, module names must be unique and contain only alphanumeric characters, space, underscore, or dash.</p>
<h5><span style="color: #ff6600;"><strong>Description:</strong></span></h5>
<p>The description field is optional for you to give a short description for what the module does.</p>
<h5><span style="color: #ff6600;"><strong>Network Path:</strong></span></h5>
<p>The UNC path of the printer.  Ex: <strong>\\myprintserver\printer1</strong></p>
<h5><span style="color: #ff6600;"><strong>Action:</strong></span></h5>
<p>The action that should be take with the printer.  Install and Delete are self explanatory.  None means nothing with the printer will happen and is really only used as a way to restart the printer spooler.  InstallPowershell is just an alternative way to install the printer in case the default install option has problems.</p>
<h5><span style="color: #ff6600;"><strong>Set As Default:</strong></span></h5>
<p>Sets the printer as default.  A policy may contain multiple printer modules.  The last printer module to run with set as default, will become the default printer.</p>
<h5><span style="color: #ff6600;"><strong>Restart Print Spooler:</strong></span></h5>
<p>If this is enabled the printer spooler service will be restarted after the printer is installed.</p>
<h5><span style="color: #ff6600;"><strong>Wait For Enumeration:</strong></span></h5>
<p>If this is enabled, the policy won't continue until it verifies the printer was installed.</p>
<div class="alert alert-error">Be careful with this setting, you can easily prevent additional policies from running while waiting.</div>
</asp:Content>