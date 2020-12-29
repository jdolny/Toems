'use strict';
var args;
var autoReconnect = true;
var StatusStrs = ['Disconnected', 'Connecting...', 'Setup...', 'Connected', 'Intel&reg; AMT Connected'];
var meshserver = null;
var meshes = {};
var meshcount = 0;
var nodes = null;
var serverinfo = null;
var events = [];
var users = null;
var wssessions = null;
var nodeShortIdent = 0;
var desktop;
var desktopsettings = { encoding: 2, showfocus: false, showmouse: true, showcad: true, quality: 100, scaling: 1024, framerate: 100, localkeymap: false };
var multiDesktop = {};
var multiDesktopFilter = null;
var amtScanResults = null;
var authCookieRenewTimer = null;
var debugmode = 0;
var currentNode;
var xxdialogMode;
var xxdialogFunc;
var xxdialogButtons;
var xxdialogTag;
var xxcurrentView = -1;
var fullscreen;
var urlargs = parseUriArgs();
var features = parseInt('{{{features}}}');
var p11DeskConsoleMsgTimer = null;
var attemptWebRTC = ((features & 128) != 0);
var agentPresent = true;
var intelAmtPresent = false;
var pluginHandler;





function getNodeFromId(id) { if (nodes != null) { for (var i in nodes) { if (nodes[i]._id == id) return nodes[i]; } } return null; }
function reload() {
    var x = window.location.href;
    if (x.endsWith('/#')) { x = x.substring(0, x.length - 2); }
    window.location.href = x;
}


function onStateChanged(server, state, prevState, errorCode) {
    if (state == 0) {

        QV('logoutControl', false);
        if (errorCode == 'noauth') { QH('p0span', "Unable to perform authentication"); return; }
        if (prevState == 2) { if (autoReconnect) { setTimeout(serverPoll, 5000); } } else { QH('p0span', "Unable to connect web socket"); }
        if (authCookieRenewTimer != null) { clearInterval(authCookieRenewTimer); authCookieRenewTimer = null; }
    } else if (state == 2) {
        // Fetch list of meshes, nodes, files
        meshserver.send({ action: 'usergroups' });
        meshserver.send({ action: 'meshes' });
        meshserver.send({ action: 'nodes', id: '{{currentNode}}' });
        if (pluginHandler != null) { meshserver.send({ action: 'plugins' }); }
        if ('{{currentNode}}'.toLowerCase() == '') { meshserver.send({ action: 'files' }); }
        if ('{{viewmode}}'.toLowerCase() == '') { go(1); }
        authCookieRenewTimer = setInterval(function () { meshserver.send({ action: 'authcookie' }); }, 1800000); // Request a cookie refresh every 30 minutes.
    }
}

// Poll the server, if it responds, refresh the page.
function serverPoll() {
    var xdr = null;
    try { xdr = new XDomainRequest(); } catch (e) { }
    if (!xdr) xdr = new XMLHttpRequest();
    xdr.open('HEAD', window.location.href);
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
            
            break;
        }
        case 'serverinfo': {
            serverinfo = message.serverinfo;
            if (serverinfo.timeout) { setInterval(checkIdleSessionTimeout, 10000); checkIdleSessionTimeout(); }
            if (debugmode == 1) { console.log('Server time: ', printDateTime(new Date(serverinfo.serverTime))); }
            break;
        }
        case 'nodes': {
            nodes = [];

            for (var m in message.nodes) {
                if (!meshes[m]) { console.log('Invalid mesh (1): ' + m); continue; }
                for (var n in message.nodes[m]) {
                    if (message.nodes[m][n]._id == null) { console.log('Invalid node (' + n + '): ' + JSON.stringify(message.nodes)); continue; }
                    message.nodes[m][n].namel = message.nodes[m][n].name.toLowerCase();
                    if (message.nodes[m][n].rname) { message.nodes[m][n].rnamel = message.nodes[m][n].rname.toLowerCase(); } else { message.nodes[m][n].rnamel = message.nodes[m][n].namel; }
                    message.nodes[m][n].meshnamel = meshes[m].name.toLowerCase();
                    message.nodes[m][n].meshid = m;
                    message.nodes[m][n].state = (message.nodes[m][n].state) ? (message.nodes[m][n].state) : 0;
                    message.nodes[m][n].desc = message.nodes[m][n].desc;
                    message.nodes[m][n].ip = message.nodes[m][n].ip;
                    if (!message.nodes[m][n].icon) message.nodes[m][n].icon = 1;
                    message.nodes[m][n].ident = ++nodeShortIdent;
                    nodes.push(message.nodes[m][n]);
                }
            }
            currentNode = getNodeFromId(nodeId);
            break;
        }

        case 'getcookie': {
            if (message.tag == 'MCRouter') {
                var servername = serverinfo.name;
                if ((servername.indexOf('.') == -1) || ((features & 2) != 0)) { servername = window.location.hostname; } // If the server name is not set or it's in LAN-only mode, use the URL hostname as server name.
                var domainUrlNoSlash = domainUrl.substring(0, domainUrl.length - 1);
                var portStr = (serverinfo.port == 443) ? '' : (':' + serverinfo.port);
                var url = 'mcrouter://' + servername + portStr + domainUrl + 'control.ashx?c=' + authCookie + '&t=' + serverinfo.tlshash + '&l={{{lang}}}' + (urlargs.key ? ('&key=' + urlargs.key) : '');
                if (message.nodeid != null) { url += ('&nodeid=' + message.nodeid); }
                if (message.tcpport != null) { url += ('&protocol=1&remoteport=' + message.tcpport); }
                if (message.protocol == 'HTTP') { url += ('&appid=1'); } // HTTP
                if (message.protocol == 'HTTPS') { url += ('&appid=2'); } // HTTPS
                if (message.protocol == 'RDP2') { url += ('&appid=3'); } // RDP
                if (message.protocol == 'PSSH') { url += ('&appid=4'); } // Putty
                if (message.protocol == 'WSCP') { url += ('&appid=5'); } // WinSCP
                url += '&autoexit=1';
                console.log(url);
                downloadFile(url, '');
            }

            break;
        }

        default:
            console.log('Unknown message.action', message.action);
            break;
    }
}



function clickOnceRDP() {
    alert(nodeId);
    meshserver.send({ action: 'getcookie', nodeid: nodeId, tcpport: 3389, tag: 'MCRouter', protocol: 'RDP2' });
}










// Webkit seems to have a problem with "download" tag causing "network error", but openning the download in a hidden frame fixes it.
// So we do that for all browsers except FireFox
function downloadFile(link, name, closeDialog) {
    var element = document.createElement('a');
    element.setAttribute('href', link);
    element.setAttribute('rel', 'noreferrer noopener');
    element.setAttribute('target', 'fileDownloadFrame');
    if (navigator.userAgent.indexOf('Firefox') >= 0) { element.setAttribute('download', decodeURIComponent(name ? name : '')); }
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);

}





