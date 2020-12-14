<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/comservers/comservers.master" AutoEventWireup="true" CodeBehind="multicastsettings.aspx.cs" Inherits="Toems_FrontEnd.views.admin.comservers.multicastsettings" %>
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
     <h5><span style="color: #ff9900;">Multicast Server:</span></h5>
<p>Determines if this com server can be used as a multicast server for deploying images to a group of computers simultaneously.</p>
        <h5><span style="color: #ff9900;">Interface IP Address:</span></h5>
    <p>The ip address of the nic that will be used for multicasting.  This must be populated before an image can be multicasted.</p>
    <h5><span style="color: #ff9900;">Multicast Sender Arguments:</span></h5>
    <p>Additional arguments that are passed to the multicast sender.</p>

     <h5><span style="color: #ff9900;">Multicast Receiver Arguments:</span></h5>
     <p>Additional arguments that are passed to the multicast receiver.</p>

     <h5><span style="color: #ff9900;">Multicast Start Port:</span></h5>
     <p>The starting port of the range used for multicasting.</p>

         <h5><span style="color: #ff9900;">Multicast End Port:</span></h5>
     <p>The ending port of the range used for multicasting.</p>

         <h5><span style="color: #ff9900;">Decompress Image On:</span></h5>
     <p>Specifies if the image should be decompressed on the server or client when multicasting.  Selecting server will result in transferring a larger amount of data, but could be more reliable.</p>

</asp:Content>
