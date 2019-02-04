<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/wolrelays/wolrelays.master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="Toems_FrontEnd.views.admin.wolrelays.edit" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li> <%=WolRelay.Gateway %></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    <%=WolRelay.Gateway %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
   <li><asp:LinkButton ID="buttonUpdateRelay" runat="server" OnClick="buttonUpdateRelay_OnClick" Text="Update Relay" CssClass="main-action"/></li>
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#general').addClass("nav-current");
        });
    </script>
     <div class="size-4 column">
        Gateway:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtGateway" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>

    </div>
       <br class="clear"/>
      <div class="size-4 column">
        Com Server:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlComServer" runat="server" CssClass="ddlist"/>
    </div>
  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <h5><span style="color: #ff9900;">Gateway:</span></h5>
<p>The gateway address for the subnet this relay is responsible for.</p>
<h5><span style="color: #ff9900;">Com Server:</span></h5>
<p>The com server that will be used as the relay.  It must be on the same subnet as the gateway address.</p>
</asp:Content>