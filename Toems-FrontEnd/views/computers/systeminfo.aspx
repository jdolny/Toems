<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="systeminfo.aspx.cs" Inherits="Toems_FrontEnd.views.computers.systeminfo" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li> <%= ComputerEntity.Name %></li>
    <li>System Info</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
<%= ComputerEntity.Name %>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="DropDownActionsSub">

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#system').addClass("nav-current");
          });
         
    </script>
     <div class="size-lbl column">
        Name:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblName" ></asp:Label>
    </div>
    <br class="clear"/>
     <div class="size-lbl column">
        Domain:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblDomain" ></asp:Label>
    </div>
    <br class="clear"/>
     <div class="size-lbl column">
        Workgroup:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblWorkgroup" ></asp:Label>
    </div>
    <br class="clear"/>
    <div class="size-lbl column">
        Manufacturer:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblManufacturer" ></asp:Label>
    </div>
    <br class="clear"/>
    <div class="size-lbl column">
        Model:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblModel" ></asp:Label>
    </div>
    <br class="clear"/>
    <div class="size-lbl column">
        Memory(MB):
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblMemory" ></asp:Label>
    </div>
     <br class="clear"/>
    <div class="size-lbl column">
        GPU:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblGpu" ></asp:Label>
    </div>
    <br class="clear"/>
    <br /><br/>
    <div class="size-lbl column">
        OS Name:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblOsName" ></asp:Label>
    </div>
    <br class="clear"/>
    <div class="size-lbl column">
        OS Version:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblOsVersion" ></asp:Label>
    </div>
    <br class="clear"/>
    <div class="size-lbl column">
        OS Build:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblOsBuild" ></asp:Label>
    </div>
<br class="clear"/>
<div class="size-lbl column">
    OS Release Id:
</div>
<div class="size-lbl2 column">
    <asp:Label runat="server" ID="lblReleaseId" ></asp:Label>
</div>
    <br class="clear"/>
    <div class="size-lbl column">
        OS Architecture:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblOsArch" ></asp:Label>
    </div>
    <br class="clear"/>
    <div class="size-lbl column">
        OS Service Pack Major:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblOsSpMajor" ></asp:Label>
    </div>
    <br class="clear"/>
    <div class="size-lbl column">
        OS Service Pack Minor:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblOsSpMinor" ></asp:Label>
    </div>
<br class="clear"/>
<div class="size-lbl column">
    Time Zone:
</div>
<div class="size-lbl2 column">
    <asp:Label runat="server" ID="lblTimeZone" ></asp:Label>
</div>
    <br class="clear"/>
    <br /><br/>
    <div class="size-lbl column">
        BIOS Serial Number:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblBiosSerial" ></asp:Label>
    </div>
    <br class="clear"/>
    <div class="size-lbl column">
        Bios Version:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblBiosVersion" ></asp:Label>
    </div>
    <br class="clear"/>
    <div class="size-lbl column">
        SMBIOS Version:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblSmBiosVersion" ></asp:Label>
    </div>
    <br class="clear"/>
    <br/><br />
     <div class="size-lbl column">
        Processor:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblProcessor" ></asp:Label>
    </div>
    <br class="clear"/>
     <div class="size-lbl column">
        Processor Clock Speed:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblSpeed" ></asp:Label>
    </div>
    <br class="clear"/>
     <div class="size-lbl column">
        Processor Cores
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblCores" ></asp:Label>
    </div>
    <br class="clear"/>
        <br />
    <hr/>
    <br/>
<hr/>
<br/>
<h2>Security</h2>
<div class="size-lbl column">
    UAC Status
</div>
<div class="size-lbl2 column">
    <asp:Label runat="server" ID="lblUac" ></asp:Label>
</div>
<br class="clear"/>
<div class="size-lbl column">
    Location Services Enabled
</div>
<div class="size-lbl2 column">
    <asp:Label runat="server" ID="lblLocation" ></asp:Label>
</div>
<br class="clear"/>
<div class="size-lbl column">
    Domain Firewall Enabled
</div>
<div class="size-lbl2 column">
    <asp:Label runat="server" ID="lblDomainFirewall" ></asp:Label>
</div>
<br class="clear"/>
<div class="size-lbl column">
    Private Firewall Enabled
</div>
<div class="size-lbl2 column">
    <asp:Label runat="server" ID="lblPrivateFirewall" ></asp:Label>
</div>
<br class="clear"/>
<div class="size-lbl column">
    Public Firewall Enabled
</div>
<div class="size-lbl2 column">
    <asp:Label runat="server" ID="lblPublicFirewall" ></asp:Label>
</div>
<br class="clear"/>
<div class="size-lbl column">
    Update Server
</div>
<div class="size-lbl2 column">
    <asp:Label runat="server" ID="lblUpdateServer" ></asp:Label>
</div>
<br class="clear"/>
<br class="clear"/>
<div class="size-lbl column">
    Update Server Target Group
</div>
<div class="size-lbl2 column">
    <asp:Label runat="server" ID="lblTargetGroup" ></asp:Label>
</div>
<br class="clear"/>
<br />
<hr/>
<br/>
<hr/>
<br/>
<h2>AntiVirus</h2>

<asp:GridView ID="gvAntivirus" runat="server" DataKeyNames="Id" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
    <Columns>
        <asp:BoundField DataField="DisplayName" HeaderText="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
        <asp:BoundField DataField="Provider" HeaderText="Provider" ItemStyle-CssClass="width_200"></asp:BoundField>
        <asp:BoundField DataField="RealTimeScanner" HeaderText="Real Time Scanner Status" ItemStyle-CssClass="width_200"></asp:BoundField>
        <asp:BoundField DataField="DefinitionStatus" HeaderText="Definition Status" ItemStyle-CssClass="width_200"></asp:BoundField>
        <asp:BoundField DataField="ProductState" HeaderText="Product State"></asp:BoundField>
    </Columns>
    <EmptyDataTemplate>
        No AntiVirus Information Found
    </EmptyDataTemplate>
</asp:GridView>
<br class="clear"/>
<br />
<hr/>
<br/>
<hr/>
<br/>
<h2>Bitlocker</h2>

<asp:GridView ID="gvBitlocker" runat="server" DataKeyNames="Id" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
    <Columns>
        <asp:BoundField DataField="DriveLetter" HeaderText="Drive" ItemStyle-CssClass="width_200"></asp:BoundField>
        <asp:BoundField DataField="Status" HeaderText="Status"></asp:BoundField>
    </Columns>
    <EmptyDataTemplate>
        No Bitlocker Information Found
    </EmptyDataTemplate>
</asp:GridView>
<br class="clear"/>
<br />
<hr/>
<br/>
<hr/>
<br/>
    <h2>Hard Drives</h2>

    <asp:GridView ID="gvHds" runat="server" DataKeyNames="Id" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="Model" HeaderText="Model" ItemStyle-CssClass="width_200"></asp:BoundField>
           <asp:BoundField DataField="SerialNumber" HeaderText="Serial" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="FirmwareRevision" HeaderText="Firmware" ItemStyle-CssClass="width_200"></asp:BoundField>
          <asp:BoundField DataField="SizeMb" HeaderText="Size(MB)" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Status" HeaderText="Smart Status"></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No Hard Drives Found
        </EmptyDataTemplate>
    </asp:GridView>
<br class="clear"/>
<br />
<hr/>
<br/>
<hr/>
<br/>
    <h2>Logical Volumes</h2>

    <asp:GridView ID="gvLogicalVolumes" runat="server" DataKeyNames="Id" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="Drive" HeaderText="Drive" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="SizeGB" HeaderText="Size (GB)" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="FreeSpacePercent" HeaderText="Free (%)" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="FreeSpaceGB" HeaderText="Free (GB)"></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No Logical Volumes Found
        </EmptyDataTemplate>
    </asp:GridView>
<br class="clear"/>
<br />
<hr/>
<br/>
<hr/>
<br/>
     <h2>Network Adapters</h2>

    <asp:GridView ID="gvNics" runat="server" DataKeyNames="Id" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
           <asp:BoundField DataField="Description" HeaderText="Description" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="Type" HeaderText="Type" ItemStyle-CssClass="width_200"></asp:BoundField>
          <asp:BoundField DataField="Mac" HeaderText="MAC" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="Speed" HeaderText="Speed" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Ips" HeaderText="Ips" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Gateways" HeaderText="Gateways"/>
        </Columns>
        <EmptyDataTemplate>
            No Network Adapters Found
        </EmptyDataTemplate>
    </asp:GridView>
<br class="clear"/>
<br />
<hr/>
<br/>
<hr/>
<br/>
    <h2>Printers</h2>
    <asp:GridView ID="gvPrinters" runat="server" DataKeyNames="Id" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
              <asp:BoundField DataField="SystemName" HeaderText="System" ItemStyle-CssClass="width_200"></asp:BoundField>
           <asp:BoundField DataField="ShareName" HeaderText="Share" ItemStyle-CssClass="width_200"></asp:BoundField>
           
          <asp:BoundField DataField="DriverName" HeaderText="Driver" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Local" HeaderText="Local" ItemStyle-CssClass="width_200"></asp:BoundField>
              <asp:BoundField DataField="Network" HeaderText="Network"></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No Printers Found
        </EmptyDataTemplate>
    </asp:GridView>
<br class="clear"/>
<br />
<hr/>
<br/>
<hr/>
<br/>
<h2>Location</h2>
<div class="size-lbl column">
    Latitude:
</div>
<div class="size-lbl2 column">
    <asp:Label runat="server" ID="lblLatitude" ></asp:Label>
</div>
<br class="clear"/>
<div class="size-lbl column">
    Longitude:
</div>
<div class="size-lbl2 column">
    <asp:Label runat="server" ID="lblLongitude" ></asp:Label>
</div>
<br class="clear"/>
<div class="size-lbl column">
    Last Location Update:
</div>
<div class="size-lbl2 column">
    <asp:Label runat="server" ID="lblLastLocationUpdate" ></asp:Label>
</div>
<br class="clear"/>
<div class="size-lbl2 column">
    <a class="color_dark" href="https://www.google.com/maps/search/?api=1&query=<%= Latitude %>,<%= Longitude %>" target="_blank">View On Google Maps</a>

</div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page displays all of the inventory collected from an inventory scan, enabled in a policy.</p>
</asp:Content>

