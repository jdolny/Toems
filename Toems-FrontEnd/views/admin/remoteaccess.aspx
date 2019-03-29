<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" CodeBehind="remoteaccess.aspx.cs" Inherits="Toems_FrontEnd.views.admin.remoteaccess" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Remote Access</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Remote Access
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnUpdateSettings" runat="server" Text="Update Settings" OnClick="btnUpdateSettings_OnClick" CssClass="main-action"/></li>
     
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" Runat="Server">
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
    <script keeplink="1" type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/charts.js") %>"></script>
    <script keeplink="1" type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/filesaver.1.1.20151003.js") %>"></script>
    <script keeplink="1" type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/ol.js") %>"></script>
    <script keeplink="1" type="text/javascript" src="<%= ResolveUrl("~/content/js/mesh/ol3-contextmenu.js") %>"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            $('#remoteaccess').addClass("nav-current");
            meshserver = MeshServerCreateControl(meshServer, authCookie);
            meshserver.onStateChanged = onStateChanged;
            meshserver.onMessage = onMessage;
            meshserver.Start();
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
        var meshServer = "<%= MeshServer %>";
        var autoReconnect = true;
        var authCookieRenewTimer = null;
        var meshes = null;
        function onStateChanged(server, state, prevState, errorCode) {
            if (state == 0) {
                if (errorCode == 'noauth') { console.log('Unable to perform authentication'); return; }
                if (prevState == 2) { if (autoReconnect) { setTimeout(serverPoll, 5000); } } else { console.log('Unable to connect web socket'); }
                if (authCookieRenewTimer != null) { clearInterval(authCookieRenewTimer); authCookieRenewTimer = null; }
            } else if (state == 2) {
                // Fetch list of meshes, nodes, files
             
                authCookieRenewTimer = setInterval(function () { meshserver.send({ action: 'authcookie' }); }, 1800000); // Request a cookie refresh every 30 minutes.
                meshserver.send({ action: 'meshes' });
                meshserver.send({ action: 'users' }); 
                meshserver.send({ action: 'wssessioncount' }); 
            }
        }

        // Poll the server, if it responds, refresh the page.
        function serverPoll() {
            var xdr = null;
            try { xdr = new XDomainRequest(); } catch (e) { }
            if (!xdr) xdr = new XMLHttpRequest();
            xdr.open("HEAD", window.location.href);
            xdr.timeout = 15000;
            xdr.onload = function () { reload(); };
            xdr.onerror = xdr.ontimeout = function () { setTimeout(serverPoll, 10000); };
            xdr.send();
        }

        function onMessage(server, message) {
            switch (message.action) {
                case 'authcookie': {
                    authCookie = message.cookie;
                    break;
                }

                case 'meshes': {
                    meshes = {};
                    for (var m in message.meshes) { meshes[message.meshes[m]._id] = message.meshes[m]; }
                    for (var i in meshes) {
                        document.getElementById("lblMeshGroups").innerHTML += meshes[i].name + " : " + meshes[i]._id + "<br/>";
                        if (meshes[i].name == 'All Mesh Clients') {
                            var currentMesh = meshes[i];
                            $('#<%= meshID.ClientID %>').val(currentMesh._id);
                            meshserver.send({ action: 'addmeshuser', meshid: currentMesh._id, meshname: currentMesh.name, username: 'meshcontrol', meshadmin: 8 });
                            meshserver.send({ action: 'addmeshuser', meshid: currentMesh._id, meshname: currentMesh.name, username: 'meshview', meshadmin: 264 });

                        }
                    }
                    ;
                    break;
                }

                case 'users': {
                    users = {};
                    for (var m in message.users) {
                        users[message.users[m]._id] = message.users[m];
                        document.getElementById("lblMeshUsers").innerHTML += message.users[m].name + "<br/>";
                    }
                    break;
                }
                case 'wssessioncount': {
                    wssessions = message.wssessions;

                    for (var i in wssessions) {
                        document.getElementById("lblMeshConnections").innerHTML += i + " : " + wssessions[i] + "<br/>";
                        
                    }
                    break;
                }

                default:
                    console.log('Unknown message.action', message.action);
                    break;
            }
        }

        function createMesh() {
            if (meshes == null) return;
            for (var i in meshes) {
                if (meshes[i].name == 'All Mesh Clients') {
                    return;
                }
            }
            meshserver.send({ action: 'createmesh', meshname: 'All Mesh Clients', meshtype: '2', desc: 'Contains all mesh clients' });
            meshserver.send({ action: 'meshes' });
           
        }

           
    </script>
      <asp:HiddenField ID="meshID" runat="server" ClientIDMode="Static"/>
    <div class="size-4 column">
        Remote Access Server:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtRemoteAccessServer" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
     <asp:LinkButton runat="server" ID="SetupUsers" OnClick="SetupUsers_Click" Text="Setup Users"></asp:LinkButton> <br />
    <asp:LinkButton ID="btnSend" runat="server" OnClick="btnCreateSession_Click" Text="Create Mesh Session" CssClass="main-action"/><br />
   
    <asp:LinkButton runat="server" Text="Create Mesh" OnClientClick="createMesh();"></asp:LinkButton><br />
    <asp:LinkButton runat="server" Text="Download Client" OnClick="Unnamed_Click" ></asp:LinkButton>
    
    <br /><br />
    <h4 style="margin-left:0;">Mesh Groups</h4>
    <asp:Label runat="server" ID="lblMeshGroups" ClientIDMode="Static"></asp:Label>    
    <br /><br />
      <h4 style="margin-left:0;">Mesh Users</h4>
    <asp:Label runat="server" ID="lblMeshUsers" ClientIDMode="Static"></asp:Label>    
    <br /><br />
      <h4 style="margin-left:0;">Mesh Connections</h4>
    <asp:Label runat="server" ID="lblMeshConnections" ClientIDMode="Static"></asp:Label>    


</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    
</asp:Content>