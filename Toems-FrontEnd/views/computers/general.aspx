﻿<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="general.aspx.cs" Inherits="Toems_FrontEnd.views.computers.general" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li> <%= ComputerEntity.Name %></li>
    <li>General</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
        <%= ComputerEntity.Name %>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="DropDownActionsSub">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="btnUpdate_Click" Text="Update Computer" CssClass="main-action" /></li>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#general').addClass("nav-current");
          });
         
    </script>
        <div class="size-lbl column">
        Name:
    </div>
    <div class="size-lbl2 column">
        <asp:Textbox runat="server" ID="txtName" CssClass="textbox" ></asp:Textbox>
    </div>
    <br class="clear"/>
     <div class="size-lbl column">
        Description:
    </div>
    <div class="size-lbl2 column">
        <asp:TextBox ID="txtDesc" runat="server" TextMode="MultiLine" CssClass="descbox"></asp:TextBox>
    </div>
    <br class="clear"/>
       <div class="size-lbl column">
        Identifier:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblIdentifier" ></asp:Label>
    </div>
      <br class="clear"/>
       <div class="size-lbl column">
        Synced From Active Directory:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblAdSync"></asp:Label>
    </div>
      <br class="clear"/>
       <div class="size-lbl column">
        Active Directory GUID:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblAdGuid" ></asp:Label>
    </div>
     <br class="clear"/>
       <div class="size-lbl column">
        Active Directory Disabled:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblAdDisabled" ></asp:Label>
    </div>
      <br class="clear"/>
       <div class="size-lbl column">
        Installation Id:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblInstallId" ></asp:Label>
    </div>
    <br class="clear"/>
           <div class="size-lbl column">
        Hardware UUID:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblHardwareUuid" ></asp:Label>
    </div>
    <br class="clear"/>

       <div class="size-lbl column">
        Provision Status:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblStatus" ></asp:Label>
    </div>
      <br class="clear"/>
       <div class="size-lbl column">
        Provision Date:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblProvisionDate" ></asp:Label>
    </div>
      <br class="clear"/>
       <div class="size-lbl column">
        Last Checkin:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblLastCheckin" ></asp:Label>
    </div>
      <br class="clear"/>
       <div class="size-lbl column">
        Last Known IP:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblLastIp" ></asp:Label>
    </div>
      <br class="clear"/>
       <div class="size-lbl column">
        Client Version:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblClientVersion" ></asp:Label>
    </div>
      <br class="clear"/>
       <div class="size-lbl column">
        Last Inventory Time:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblInventoryTime" ></asp:Label>
    </div>
      <br class="clear"/>
      
       <div class="size-lbl column">
        Remote Access ID:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblRemoteAccessId" ></asp:Label>
    </div>
    <br class="clear" />
      <div class="size-lbl column">
        Remote Access WebRtcStatus:
    </div>
    <div class="size-lbl2 column hidden-check">
           <asp:CheckBox runat="server" id="chkWebRtc" ClientIDMode="Static" AutoPostBack="true" OnCheckedChanged="chkWebRtc_CheckedChanged"/>
        <label for="chkWebRtc"></label>
    </div>

      <br class="clear"/>
       <div class="size-lbl column">
        Primary Imaging Mac:
    </div>
    <div class="size-lbl2 column">
        <asp:Textbox runat="server" ID="txtImagingMac" CssClass="textbox"></asp:Textbox>
    </div>

     <br class="clear"/>
       <div class="size-lbl column">
        Imaging Client Id:
    </div>
    <div class="size-5 column">
        <asp:Textbox runat="server" ID="txtImagingId" CssClass="textbox"></asp:Textbox>
    </div>

    

    
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <h1>Actions</h1>
     <h5><span style="color: #ff9900;">Deploy Image:</span></h5>
    <p>Starts an image deployment task for the computer using it's effective image, defined in Image Settings.</p>
     <h5><span style="color: #ff9900;">Upload Image:</span></h5>
    <p>Starts an image upload task for the computer using it's effective image, defined in Image Settings.</p>
    <h5><span style="color: #ff9900;">Force Checkin:</span></h5>
<p>Forces the computer to checkin immediately, instead of waiting for the next checkin interval.</p>
<h5><span style="color: #ff9900;">Current Users:</span></h5>
<p>Displays any users that are currently logged into the computer.  The computer must be on the internal network for this to work.</p>
<h5><span style="color: #ff9900;">Status:</span></h5>
<p>Displays true if the client computer can be reached through the client(toec), otherwise false.  The computer must be on the interal network for this to work.</p>
<h5><span style="color: #ff9900;">Reboot:</span></h5>
<p>Reboots the computer.</p>
<h5><span style="color: #ff9900;">Shutdown:</span></h5>
<p>Powers off the computer.</p>
<h5><span style="color: #ff9900;">Wake Up:</span></h5>
<p>Powers on the computer, using WOL.</p>
<h5><span style="color: #ff9900;">Collect Inventory:</span></h5>
<p>Immediately runs an inventory collection on the computer without needing to wait for an inventory policy to run.</p>
     <h5><span style="color: #ff9900;">Clear Imaging Id:</span></h5>
<p>Resets the ImagingId for this computer.  An imaging id is based on a computers physical hardware.  If imaging a computer does not correctly identify itself
, it probably needs it's id reset.
</p>
    <h1>General</h1>
    <p>The general page displays information specific to the functionality of Theopenem.  This information cannot be modified.</p>
</asp:Content>