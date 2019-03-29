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

   
    <div id="p11" class="noselect">
        <table id="deskarea0" cellpadding="0" cellspacing="0" style="width: 100%; padding: 0px; padding: 0px; margin-top: 0px">
            <tr id="deskarea1">
                <td style="padding-top: 2px; padding-bottom: 2px; background: #C0C0C0">

                    <div>
                        <div id="idx_deskFullBtn2" onclick="deskToggleFull(event)" style="float: left; font-size: large; cursor: pointer; display: none">&nbsp;&#x2716;</div>

                        <span id="connectbutton1span">&nbsp;<input type="button" id="connectbutton1" value="Connect HTML5" onclick="connectDesktop(event, 1)" onkeypress="return false" onkeydown="return false"></span>
                        <asp:LinkButton runat="server" OnClientClick="clickOnceRDP();">Connect RDP</asp:LinkButton>
                        <span id="disconnectbutton1span">&nbsp;<input type="button" id="disconnectbutton1" value="Disconnect" onclick="connectDesktop(event, 0)" onkeypress="return false" onkeydown="return false"></span>
                        &nbsp;<span id="deskstatus">Disconnected</span>
                    </div>
                        <div class="fullscreen" style="float:right;">Click for full screen</div>
                </td>
             
            </tr>
            <tr id="deskarea2">
                <td>
                    <div style="background-color: gray">
                        <div id="progressbar" style="height: 2px; width: 0%; background-color: red"></div>
                    </div>
                </td>
            </tr>
            <tr id="deskarea3">
                <td id="deskarea3x" style="background: black; text-align: center; position: relative; overflow: hidden">
                    <div id="DeskFocus" style="overflow: hidden; color: transparent; border: 3px dotted rgba(255,0,0,.2); position: absolute; border-radius: 5px" oncontextmenu="return false" onmousedown="dmousedown(event)" onmouseup="dmouseup(event)" onmousemove="dmousemove(event)"></div>
                    <div id="DeskParent" style="overflow: hidden">
                        <canvas id="Desk" width="640" height="480" style="overflow: hidden; width: 100%; -ms-touch-action: none; margin-left: 0px" oncontextmenu="return false" onmousedown="dmousedown(event)" onmouseup="dmouseup(event)" onmousemove="dmousemove(event)" onmousewheel="dmousewheel(event)"></canvas>
                    </div>

                </td>
            </tr>
            <tr id="deskarea4">
                <td style="padding-top: 2px; padding-bottom: 2px; background: #C0C0C0">
                    <div style="float: right; text-align: right">
                        <select id="termdisplays" style="display: none" onchange="deskSetDisplay(event)" onclick="deskGetDisplayNumbers(event)"></select>&nbsp;
                                  
                    </div>
                    <div style="float: left;">
                        <select style="margin-left: 6px; width: 150px; height: 30px;" id="deskkeys">
                            <option value="6" selected="selected">Win+R</option>
                            <option value="5">Win</option>
                            <option value="0">Win+Down</option>
                            <option value="1">Win+Up</option>
                            <option value="2">Win+L</option>
                            <option value="3">Win+M</option>
                            <option value="4">Shift+Win+M</option>
                        </select>
                    </div>
                    <div style="float: left; margin-left: 10px;">

                        <input id="DeskWD" type="button" value="Send" onkeypress="return false" onkeydown="return false" onclick="deskSendKeys()">
                        <input id="DeskClip" style="margin-left: 6px; display: none" type="button" value="Clipboard" onkeypress="return false" onkeydown="return false" onclick="showDeskClip()">
                        <input id="DeskCAD" type="button" value="Ctrl-Alt-Del" onkeypress="return false" onkeydown="return false" onclick="sendCAD()">
                    </div>

                </td>
            </tr>
        </table>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            document.onkeypress = ondockeypress;
            document.onkeydown = ondockeydown;
            document.onkeyup = ondockeyup;
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
      
        $(".fullscreen").click(function () {
            fullscreen = true;
            $("#leftnav").hide();
            $("#secondary-nav-container").hide();
            $(".content-wrapper").css("width", "100%");
            $(".content-header").hide();
            $("#content-main").css("height", "100%");
            deskAdjust();
        });
       
        </script>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
</asp:Content>