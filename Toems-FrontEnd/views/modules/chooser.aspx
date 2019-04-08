<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/modules.master" AutoEventWireup="true" CodeBehind="chooser.aspx.cs" Inherits="Toems_FrontEnd.views.modules.chooser" %>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
 Modules
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavLevel1" ID="subNavLevel1">
     <ul class="ul-secondary-nav">
            <li id="software">
                <a href="<%= ResolveUrl("~/views/modules/softwaremodules/search.aspx") %>">
                    <span class="sub-nav-text">Software Modules</span></a>
            </li>
             <li id="printer">
                <a href="<%= ResolveUrl("~/views/modules/printermodules/search.aspx") %>">
                    <span class="sub-nav-text">Printer Modules</span></a>
            </li>
             <li id="command">
                <a href="<%= ResolveUrl("~/views/modules/commandmodules/search.aspx") %>">
                    <span class="sub-nav-text">Command Modules</span></a>
            </li>
             <li id="script">
                <a href="<%= ResolveUrl("~/views/modules/scriptmodules/search.aspx") %>">
                    <span class="sub-nav-text">Script Modules</span></a>
            </li>
             <li id="file">
                <a href="<%= ResolveUrl("~/views/modules/filecopymodules/search.aspx") %>">
                    <span class="sub-nav-text">File Copy Modules</span></a>
            </li>
         <li id="windowsupdate">
             <a href="<%= ResolveUrl("~/views/modules/wumodules/search.aspx") %>">
                 <span class="sub-nav-text">Windows Update Modules</span></a>
         </li>
          <li id="message">
             <a href="<%= ResolveUrl("~/views/modules/messagemodules/search.aspx") %>">
                 <span class="sub-nav-text">Message Modules</span></a>
         </li>
        </ul>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="SubContent" runat="server">
    <script>
    $(document).ready(function () {
    $('.actions_left').addClass("display-none");
        });
    </script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SubHelp">
    <p>Modules serve as the building blocks for the actions you will perform on your endpoints.  Most of the custom actions to perform are accomplished through modules.  Options such as installing software, running scripts, deploying printers, installing updates, running commands, are just a few of the things that modules can accomplish.  Modules provide you with the flexibility to perform almost anything on your endpoints, with a controlled, monitored, and reliable state.</p>
    </asp:Content>
