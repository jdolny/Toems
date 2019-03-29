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
var webPageFullScreen = getstore('webPageFullScreen', true);
if (webPageFullScreen == 'false') { webPageFullScreen = false; }
if (webPageFullScreen == 'true') { webPageFullScreen = true; }
var currentNode;
var xxdialogMode;
var xxdialogFunc;
var xxdialogButtons;
var xxdialogTag;
var xxcurrentView = -1;
var fullscreen;




function getNodeFromId(id) {
    if (nodes != null) { for (var i in nodes) { if (nodes[i]._id == id) return nodes[i]; } } return null;
}
function reload() { window.location.href = window.location.href; }

function onStateChanged(server, state, prevState, errorCode) {
    if (state == 0) {
        if (errorCode == 'noauth') { QH('p0span', 'Unable to perform authentication'); return; }
        if (prevState == 2) { if (autoReconnect) { setTimeout(serverPoll, 5000); } } else { QH('p0span', 'Unable to connect web socket'); }
        if (authCookieRenewTimer != null) { clearInterval(authCookieRenewTimer); authCookieRenewTimer = null; }
    } else if (state == 2) {
        // Fetch list of meshes, nodes, files
        meshserver.send({ action: 'meshes' });
        meshserver.send({ action: 'nodes', id: nodeId });
        authCookieRenewTimer = setInterval(function () { meshserver.send({ action: 'authcookie' }); }, 1800000); // Request a cookie refresh every 30 minutes.
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
            if (message.tag == 'clickonce') {
                var basicPort = serverRedirPort == "" ? serverPublicPort : serverRedirPort;
                var rdpurl = "http://" + window.location.hostname + ":" + basicPort + "/clickonce/minirouter/MeshMiniRouter.application?WS=wss%3A%2F%2F" + window.location.hostname + "%2Fmeshrelay.ashx%3Fauth=" + message.cookie + "&CH=" + webCertHash + "&AP=" + message.protocol + ((debugmode == 1) ? "" : "&HOL=1");
                var newWindow = window.open(rdpurl, '_blank');
                newWindow.opener = null;
            }
            break;
        }

        default:
            console.log('Unknown message.action', message.action);
            break;
    }
}

function ondockeypress(e) {
    if (desktop) {

        return desktop.m.handleKeys(e);
    }

}

function ondockeydown(e) {
    if (desktop) {
        return desktop.m.handleKeyDown(e);
    }
}

function ondockeyup(e) {
    if (desktop) {

        return desktop.m.handleKeyUp(e);
    }

}

function clickOnceRDP() {

    meshserver.send({ action: 'getcookie', nodeid: nodeId, tcpport: 3389, tag: 'clickonce', protocol: 'RDP2' });
}

var desktopNode;
function setupDesktop() {
    // Setup the remote desktop
    if ((desktopNode != currentNode) && (desktop != null)) { desktop.Stop(); desktopNode = null; desktop = null; }

    // If the device desktop is already connected in multi-desktop, use that.
    if ((desktopNode != currentNode) || (desktop == null)) {
        var xdesk = multiDesktop[currentNode._id];
        if (xdesk != null) {
            // This device already has a canvas, use it.
            QH('DeskParent', '');
            var c = xdesk.m.CanvasId;
            c.setAttribute('id', 'Desk');
            c.setAttribute('style', 'width:100%;-ms-touch-action:none;margin-left:0px');
            c.setAttribute('onmousedown', 'dmousedown(event)');
            c.setAttribute('onmouseup', 'dmouseup(event)');
            c.setAttribute('onmousemove', 'dmousemove(event)');
            c.removeAttribute('onclick');
            Q('DeskParent').appendChild(c);
            desktop = xdesk;
            if (desktop.m.SendCompressionLevel) { desktop.m.SendCompressionLevel(1, desktopsettings.quality, desktopsettings.scaling, desktopsettings.framerate); }
            desktop.onStateChanged = onDesktopStateChange;
            desktopNode = currentNode;
            onDesktopStateChange(desktop, desktop.State);
            delete multiDesktop[currentNode._id];
        } else {
            // Device is not already connected, just setup a blank canvas
            QH('DeskParent', '<canvas id=Desk width=640 height=480 style="width:100%;-ms-touch-action:none;margin-left:0px" oncontextmenu="return false" onmousedown=dmousedown(event) onmouseup=dmouseup(event) onmousemove=dmousemove(event)></canvas>');
            desktopNode = currentNode;
        }
        // Setup the mouse wheel
        Q('Desk').addEventListener('DOMMouseScroll', function (e) { return dmousewheel(e); });
        Q('Desk').addEventListener('mousewheel', function (e) { return dmousewheel(e); });
    }
    desktopNode = currentNode;
    deskAdjust();

}

function connectDesktop(e, contype) {
    if (desktop == null) {
        desktopNode = currentNode;

        // Setup the Mesh Agent remote desktop
        desktop = CreateAgentRedirect(meshserver, CreateAgentRemoteDesktop('Desk'), serverPublicNamePort, authCookie);
        desktop.debugmode = debugmode;
        desktop.m.debugmode = debugmode;
        desktop.onStateChanged = onDesktopStateChange;
        desktop.m.CompressionLevel = desktopsettings.quality; // Number from 1 to 100. 50 or less is best.
        desktop.m.ScalingLevel = desktopsettings.scaling;
        desktop.m.FrameRateTimer = desktopsettings.framerate;
        //desktop.m.onDisplayinfo = deskDisplayInfo;
        desktop.m.onScreenSizeChange = deskAdjust;
        desktop.Start(desktopNode._id);
        desktop.contype = 1;

    } else {
        // Disconnect and clean up the remote desktop
        desktop.Stop();
        desktopNode = desktop = null;
    }
}


function onDesktopStateChange(xdesktop, state) {
    var xstate = state;
    if ((xstate == 3) && (xdesktop.contype == 2)) { xstate++; }
    var str = StatusStrs[xstate];
    if ((desktop != null) && (desktop.webRtcActive == true)) { str += ', WebRTC'; }
    //if (desktop.m.stopInput == true) { str += ', Loopback'; }
    QH('deskstatus', str);
    switch (state) {
        case 0:
            // Disconnect and clean up the remote desktop
            desktop.Stop();
            desktopNode = desktop = null;
            break;
        case 2:
            break;
        default:
            //console.log('Unknown onDesktopStateChange state', state);
            break;
    }

    deskAdjust();
    setTimeout(deskAdjust, 50);
}


function deskAdjust() {
    var x = (Math.max(document.documentElement.clientHeight, window.innerHeight || 0) - (Q('deskarea1').clientHeight + Q('deskarea2').clientHeight + Q('Desk').clientHeight + Q('deskarea4').clientHeight + 2)) / 2;
    if (fullscreen) {
        document.documentElement.style.overflow = 'hidden';
        QS('deskarea3x').height = null;
        if (x < 0) {
            var mh = (Math.max(document.documentElement.clientHeight, window.innerHeight || 0) - (Q('deskarea1').clientHeight + Q('deskarea2').clientHeight + Q('deskarea4').clientHeight));
            var mw = 9999;
            if (desktop) { mw = (desktop.m.width / desktop.m.height) * mh; }

            QS('Desk')['max-height'] = mh + 'px';
            QS('Desk')['max-width'] = mw + 'px';
            x = 0;
        } else {
            QS('Desk')['max-height'] = null;
            QS('Desk')['max-width'] = null;
        }
        QS('Desk')['margin-top'] = x + 'px';
        QS('Desk')['margin-bottom'] = x + 'px';
    } else {
        var mw = 9999, mh = (Math.max(document.documentElement.clientHeight, window.innerHeight || 0) - (webPageFullScreen ? 276 : 290));
        if (desktop) { mw = (desktop.m.width / desktop.m.height) * mh; }

        document.documentElement.style.overflow = 'auto';
        QS('Desk')['max-height'] = mh + 'px';
        QS('Desk')['max-width'] = mw + 'px';
        QS('Desk')['margin-top'] = '0';
        QS('Desk')['margin-bottom'] = '0';
    }
}


// Remote desktop special key combos for Windows
function deskSendKeys() {
    if (xxdialogMode || desktop == null || desktop.State != 3) return;
    var ks = Q('deskkeys').value;
    if (ks == 0) { // WIN+Down arrow
        if (desktop.contype == 2) {
            desktop.m.sendkey([[0xffe7, 1], [0xff54, 1], [0xff54, 0], [0xffe7, 0]]); // Intel AMT: Meta-left down, Down arrow press, Down arrow release, Meta-left release
        } else {
            desktop.m.SendKeyMsgKC([[desktop.m.KeyAction.EXDOWN, 0x5B], [desktop.m.KeyAction.DOWN, 40], [desktop.m.KeyAction.UP, 40], [desktop.m.KeyAction.EXUP, 0x5B]]); // Agent: L-Winkey press, Down arrow press, Down arrow release, L-Winkey release
        }
    } else if (ks == 1) { // WIN+Up arrow
        if (desktop.contype == 2) {
            desktop.m.sendkey([[0xffe7, 1], [0xff52, 1], [0xff52, 0], [0xffe7, 0]]); // Intel AMT: Meta-left down, Up arrow press, Up arrow release, Meta-left release
        } else {
            desktop.m.SendKeyMsgKC([[desktop.m.KeyAction.EXDOWN, 0x5B], [desktop.m.KeyAction.DOWN, 38], [desktop.m.KeyAction.UP, 38], [desktop.m.KeyAction.EXUP, 0x5B]]); // MeshAgent: L-Winkey press, Up arrow press, Up arrow release, L-Winkey release
        }
    } else if (ks == 2) { // WIN+L arrow
        if (desktop.contype == 2) {
            desktop.m.sendkey([[0xffe7, 1], [0x6c, 1], [0x6c, 0], [0xffe7, 0]]); // Intel AMT: Meta-left down, 'l' press, 'l' release, Meta-left release
        } else {
            desktop.sendCtrlMsg('{"action":"lock"}');
            //desktop.m.SendKeyMsgKC([[desktop.m.KeyAction.EXDOWN,0x5B],[desktop.m.KeyAction.DOWN,76],[desktop.m.KeyAction.UP,76],[desktop.m.KeyAction.EXUP,0x5B]]); // MeshAgent: L-Winkey press, 'L' press, 'L' release, L-Winkey release
            //desktop.m.SendKeyMsgKC(desktop.m.KeyAction.EXDOWN, 0x5B);
            //desktop.m.SendKeyMsgKC(desktop.m.KeyAction.DOWN, 76);
            //desktop.m.SendKeyMsgKC(desktop.m.KeyAction.UP, 76);
            //desktop.m.SendKeyMsgKC(desktop.m.KeyAction.EXUP, 0x5B);
        }
    } else if (ks == 3) { // WIN+M arrow
        if (desktop.contype == 2) {
            desktop.m.sendkey([[0xffe7, 1], [0x6d, 1], [0x6d, 0], [0xffe7, 0]]); // Intel AMT: Meta-left down, 'm' press, 'm' release, Meta-left release
        } else {
            desktop.m.SendKeyMsgKC([[desktop.m.KeyAction.EXDOWN, 0x5B], [desktop.m.KeyAction.DOWN, 77], [desktop.m.KeyAction.UP, 77], [desktop.m.KeyAction.EXUP, 0x5B]]); // MeshAgent: L-Winkey press, 'M' press, 'M' release, L-Winkey release
        }
    } else if (ks == 4) { // Shift+WIN+M arrow
        if (desktop.contype == 2) {
            desktop.m.sendkey([[0xffe1, 1], [0xffe7, 1], [0x6d, 1], [0x6d, 0], [0xffe7, 0], [0xffe1, 0]]); // Intel AMT: Shift-left down, Meta-left down, 'm' press, 'm' release, Meta-left release, Shift-left release
        } else {
            desktop.m.SendKeyMsgKC([[desktop.m.KeyAction.DOWN, 16], [desktop.m.KeyAction.EXDOWN, 0x5B], [desktop.m.KeyAction.DOWN, 77], [desktop.m.KeyAction.UP, 77], [desktop.m.KeyAction.EXUP, 0x5B], [desktop.m.KeyAction.UP, 16]]);     // MeshAgent: L-shift press, L-Winkey press, 'M' press, 'M' release, L-Winkey release, L-shift release
        }
    } else if (ks == 5) { // WIN
        if (desktop.contype == 2) {
            desktop.m.sendkey([[0xffe7, 1], [0xffe7, 0]]); // Intel AMT: Meta-left down, Meta-left release
        } else {
            desktop.m.SendKeyMsgKC([[desktop.m.KeyAction.EXDOWN, 0x5B], [desktop.m.KeyAction.EXUP, 0x5B]]); // MeshAgent: L-Winkey press, L-Winkey release
        }
    } else if (ks == 6) { // WIN+R
        if (desktop.contype == 2) {
            desktop.m.sendkey([[0xffe7, 1], [0x72, 1], [0x72, 0], [0xffe7, 0]]); // Intel AMT: Meta-left down, 'l' press, 'l' release, Meta-left release
        } else {
            desktop.m.SendKeyMsgKC([[desktop.m.KeyAction.EXDOWN, 0x5B], [desktop.m.KeyAction.DOWN, 82], [desktop.m.KeyAction.UP, 82], [desktop.m.KeyAction.EXUP, 0x5B]]); // MeshAgent: L-Winkey press, 'R' press, 'R' release, L-Winkey release
        }
    }
}

// Double click detection. This is important for MacOS.
var dblClickDetectArgs = { t: 0, x: 0, y: 0 };
function dblClickDetect(e) {
    if (e.buttons != 1) return;
    var t = Date.now();
    if (((t - dblClickDetectArgs.t) < 250) && (Math.abs(e.clientX - dblClickDetectArgs.x) < 2) && (Math.abs(e.clientY - dblClickDetectArgs.y) < 2)) {
        if (!xxdialogMode && desktop != null) { desktop.m.mousedblclick(e); }
    }
    dblClickDetectArgs.t = t;
    dblClickDetectArgs.x = e.clientX;
    dblClickDetectArgs.y = e.clientY;
}

function sendCAD() {

    desktop.m.sendcad();
}

function dmousedown(e) { if (!xxdialogMode && desktop != null) { desktop.m.mousedown(e); } dblClickDetect(e); }
function dmouseup(e) { if (!xxdialogMode && desktop != null) { desktop.m.mouseup(e); } dblClickDetect(e); }
function dmousemove(e) { if (!xxdialogMode && desktop != null) { desktop.m.mousemove(e); } }
function dmousewheel(e) { if (!xxdialogMode && desktop != null) { if (desktop.m.mousewheel) { desktop.m.mousewheel(e); } haltEvent(e); return true; } return false; }
function joinPaths() { var x = []; for (var i in arguments) { var w = arguments[i]; if ((w != null) && (w != '')) { while (w.endsWith('/') || w.endsWith('\\')) { w = w.substring(0, w.length - 1); } while (w.startsWith('/') || w.startsWith('\\')) { w = w.substring(1); } x.push(w); } } return x.join('/'); }
function putstore(name, val) { try { if (typeof (localStorage) === 'undefined') return; localStorage.setItem(name, val); } catch (e) { } }
function getstore(name, val) { try { if (typeof (localStorage) === 'undefined') return val; var v = localStorage.getItem(name); if ((v == null) || (v == null)) return val; return v; } catch (e) { return val; } }
function haltEvent(e) { if (e.preventDefault) e.preventDefault(); if (e.stopPropagation) e.stopPropagation(); return false; }
