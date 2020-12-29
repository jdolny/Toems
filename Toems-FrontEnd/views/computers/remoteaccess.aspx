<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="remoteaccess.aspx.cs" Inherits="Toems_FrontEnd.views.computers.remoteaccess" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= ComputerEntity.Name %></li>
    <li>Remote Access</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    <%= ComputerEntity.Name %>
</asp:Content>

<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" runat="Server">
</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/common-0.0.1.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/u2f-api.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/meshcentral.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/amt-0.2.0.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/amt-wsman-0.2.0.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/amt-desktop-0.0.2.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/amt-terminal-0.0.2.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/zlib.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/zlib-inflate.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/zlib-adler32.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/zlib-crc32.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/amt-redir-ws-0.1.0.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/amt-wsman-ws-0.2.0.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/agent-redir-ws-0.1.0.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/agent-redir-rtc-0.1.0.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/agent-desktop-0.0.2.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/qrcode.min.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/content/js/meshdesktop.js") %>"></script>
    <script keeplink="1" type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/charts.js") %>"></script>
    <script keeplink="1" type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/filesaver.1.1.20151003.js") %>"></script>
    <script keeplink="1" type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/ol.js") %>"></script>
    <script keeplink="1" type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/ol3-contextmenu.js") %>"></script>

         <asp:LinkButton runat="server" OnClientClick="clickOnceRDP();">Connect RDP</asp:LinkButton>
        <asp:LinkButton runat="server" OnClick="Unnamed_Click">Remote Control</asp:LinkButton>
    <iframe runat="server" id="frame" style="width:100%;height:600px;"></iframe>
  <script type="text/javascript">
        $(document).ready(function () {
          
            meshserver = MeshServerCreateControl(domainUrl, authCookie);
            meshserver.onStateChanged = onStateChanged;
            meshserver.onMessage = onMessage;
            meshserver.Start();
            meshserver.send({ action: 'meshes' });
            meshserver.send({ action: 'nodes', id: nodeId });
        });
   
        var debugLevel = parseInt("<%= DebugLevel %>");
        var domain = "<%= Domain %>";
        var domainUrl = "<%= DomainUrl %>";
        var authCookie = "<%= AuthCookie %>";
        var serverPublicNamePort = "<%=ServerDnsName %>:<%= ServerPublicPort %>";
        var passRequirements = "<%= PassRequirements %>";
        if (passRequirements != "") { passRequirements = JSON.parse(decodeURIComponent(passRequirements)); }
        var serverRedirPort = "<%= ServerRedirectPort %>";
        var serverPublicPort = "<%= ServerPublicPort %>";
        var webCertHash = "<%= WebCertHash %>";
      var nodeId = "<%= NodeId %>";
      var meshUrl = "<%= MeshServer %>";
      
       
        </script>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
</asp:Content>