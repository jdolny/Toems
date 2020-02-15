<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/comservers/comservers.master" AutoEventWireup="true" CodeBehind="multicastsettings.aspx.cs" Inherits="Toems_FrontEnd.views.admin.comservers.multicastsettings" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li><a href="<%= ResolveUrl("~/views/admin/comservers/multicastsettings.aspx") %>?level=2">Multicast Settings</a></li>
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
            $('#multicast').addClass("nav-current");
        });
    </script>
  

      <div class="size-4 column">
        Multicast Server:
    </div>
    <div class="size-5 column hidden-check">
        <asp:CheckBox runat="server" id="chkMulticast" ClientIDMode="Static"/>
        <label for="chkMulticast"></label>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Interface IP Address:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtIp" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Multicast Sender Arguments:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtSendArgs" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>

    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Multicast Receiver Arguments:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtRecArgs" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>

    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Multicast Start Port:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtStartPort" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>

    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Multicast End Port:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtEndPort" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>

    </div>
    <br class="clear"/>
      <div class="size-4 column">
        Decompress Image On:
    </div>
    <div class="size-5 column">
       
        <asp:DropDownList ID="ddlDecompress" runat="server" CssClass="ddlist">

            <asp:ListItem>client</asp:ListItem>
            <asp:ListItem>server</asp:ListItem>
        </asp:DropDownList>
  
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
<h5><span style="color: #ff9900;">Replicate Storage:</span></h5>
<p>This option is only available after the com server is added.  When multiple com servers are defined, files for your modules must be replicated across all com servers.  If you want to disable replication to a com server, then disable this option.  If this option is disabled, you must manually replicate the files.</p>
</asp:Content>
