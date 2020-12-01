<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/filecopymodules/filecopymodule.master" AutoEventWireup="true" CodeBehind="create.aspx.cs" Inherits="Toems_FrontEnd.views.modules.filecopymodules.create" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>New</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
 File Copy Modules
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
        Destination
    </div>

     <div class="size-5 column">
        <asp:TextBox ID="txtDestination" runat="server" CssClass="textbox" Style="font-size:12px;" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>
    
   
    <div class="size-4 column">
        Unzip After Copy
    </div>

     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkUnzip" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkUnzip"></label>
        </div>
       <br class="clear"/>
     
    
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    File Copy Modules can be assigned to both Policies and Image Profiles.
    <h5><span style="color: #ff9900;"><strong>Display Name:</strong></span></h5>
<p>The name of the module, module names must be unique and contain only alphanumeric characters, space, underscore, or dash.</p>

<h5><span style="color: #ff9900;"><strong>Description:</strong></span></h5>
<p>The description field is optional for you to give a short description for what the module does.</p>
<h5><span style="color: #ff9900;"><strong>Destination:</strong></span></h5>
<p>The full path of the destination directory for the files, such as c:\temp.  There is one special option for this field <strong>[toec-appdata]</strong> 
    will cache the files in the toec appdata directory to be referenced by other modules.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Unzip After Copy:</strong></span></h5>
<p class="alert alert-error">When this option is enabled, any zip files that are part of the module will be unzipped to the destination directory, 
    otherwise the zip file itself will be copied.</p>
</asp:Content>
