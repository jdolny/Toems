<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/comservers/comservers.master" AutoEventWireup="true" CodeBehind="editcomserver.aspx.cs" Inherits="Toems_FrontEnd.views.admin.comservers.editcomserver" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li><a href="<%= ResolveUrl("~/views/admin/comservers/comservers.aspx") %>?level=2">Com Servers</a></li>
    <li><%=ComServer.DisplayName %></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
<%=ComServer.DisplayName %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
   <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Update Server" CssClass="main-action" /></li>
     <li><asp:LinkButton ID="btnCert" runat="server" OnClick="btnCert_Click" Text="Create Certificate" /></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#general').addClass("nav-current");
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
       Unique ID:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtUniqueId" runat="server" CssClass="textbox"  ReadOnly="true"></asp:TextBox>
    </div>
     <br class="clear"/>
      <div class="size-4 column">
        Description:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtDescription" runat="server" CssClass="descbox" TextMode="MultiLine"></asp:TextBox>
    </div>
     <br class="clear" />
     <div class="size-4 column">
       Local Storage Path:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtLocalStorage" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
     <br class="clear"/>
  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <h5><span style="color: #ff9900;">Display Name:</span></h5>
<p>A name used to identify the com server, it has not effect on the functionality.</p>
<h5><span style="color: #ff9900;">URL:</span></h5>
<p>The url used to access the com server, this must match the url that is set in IIS.</p>
<h5><span style="color: #ff9900;">Description:</span></h5>
<p>An optional description for the com server.</p>
    <h5><span style="color: #ff9900;">Local Storage Path:</span></h5>
    <p>The local path for this com server of where all images and file uploads are stored.</p>
</asp:Content>
