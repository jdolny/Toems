<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/filecopymodules/filecopymodule.master" AutoEventWireup="true" CodeBehind="general.aspx.cs" Inherits="Toems_FrontEnd.views.modules.filecopymodules.general" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li><%= FileCopyModule.Name %></li>
    <li>General</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
<%= FileCopyModule.Name %>
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
    <h5><span style="color: #ff9900;"><strong>Display Name:</strong></span></h5>
<p>The name of the module, module names must be unique and contain only alphanumeric characters, space, underscore, or dash.</p>
<h5><span style="color: #ff9900;"><strong>Guid:</strong></span></h5>
<p>Each module is automatically assigned a guid for an id. This is read only and only for reference.</p>
<h5><span style="color: #ff9900;"><strong>Description:</strong></span></h5>
<p>The description field is optional for you to give a short description for what the module does.</p>
<h5><span style="color: #ff9900;"><strong>Destination:</strong></span></h5>
<p>The full path of the destination directory for the files, such as c:\temp.  There is one special option for this field <strong>[toec-appdata]</strong> will cache the files in the toec appdata directory to be referenced by other modules.  When using the file copy module to install software not in MSI format, the [toec-appdata] method is the recommended approach.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Unzip After Copy:</strong></span></h5>
<p class="alert alert-error">When this option is enabled, any zip files that are part of the module will be unzipped to the destination directory, otherwise the zip file itself will be copied.</p>
</asp:Content>
