<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/comservers/comservers.master" AutoEventWireup="true" CodeBehind="addcomserver.aspx.cs" Inherits="Toems_FrontEnd.views.admin.comservers.addcomserver" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>Create Com Server</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    Communication Servers
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
   <li><asp:LinkButton ID="buttonAdd" runat="server" OnClick="buttonAdd_OnClick" Text="Add Server" CssClass="main-action"/></li>
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
        <asp:TextBox ID="txtName" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>

    </div>
    <br class="clear"/>
    <div class="size-4 column">
       Url:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtUrl" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
     <br class="clear"/>
      <div class="size-4 column">
        Description:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtDescription" runat="server" CssClass="descbox" TextMode="MultiLine"></asp:TextBox>
    </div>
     <br class="clear" />
  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <h5><span style="color: #ff9900;">Display Name:</span></h5>
<p>A name used to identify the com server, it has not effect on the functionality.</p>
<h5><span style="color: #ff9900;">URL:</span></h5>
<p>The url used to access the com server, this must match the url that is set in IIS.</p>
<h5><span style="color: #ff9900;">Description:</span></h5>
<p>An optional description for the com server.</p>
<h5><span style="color: #ff9900;">Replicate Storage:</span></h5>
<p>This option is only available after the com server is added.  When multiple com servers are defined, files for your modules must be replicated across all com servers.  If you want to disable replication to a com server, then disable this option.  If this option is disabled, you must manually replicate the files.</p>
</asp:Content>