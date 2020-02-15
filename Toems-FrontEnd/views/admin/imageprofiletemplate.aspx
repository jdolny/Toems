<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" CodeBehind="imageprofiletemplate.aspx.cs" Inherits="Toems_FrontEnd.views.admin.imageprofiletemplate" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Image Profile Templates</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Image Profile Templates
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnUpdateSettings" runat="server" Text="Update Templates" OnClick="btnUpdateSettings_OnClick" CssClass="main-action"/></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#imageprofiletemplates').addClass("nav-current");
        });
    </script>

     <div class="size-4 column">
        Image Type:
    </div>
    <div class="size-setting column">
        <asp:DropDownList ID="ddlImageType" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlImageType_OnSelectedIndexChanged">
           
        </asp:DropDownList>
    </div>
    <br class="clear"/>
    <br/><br /><br />
    <br />
  
    <h2>General</h2>
     <div class="size-4 column">
        Profile Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtProfileName" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Profile Description:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtProfileDesc" runat="server" CssClass="descbox" TextMode="MultiLine"></asp:TextBox>
    </div>
    <br class="clear"/>
    <br/>
  
    
    <div id="LinuxAll1" runat="server">
        <br />
    <h2>PXE Boot Options</h2>
     <div class="size-4 column">
        Kernel:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlKernel" runat="server" CssClass="ddlist">
        </asp:DropDownList>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Boot Image:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlBootImage" runat="server" CssClass="ddlist">
        </asp:DropDownList>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Kernel Arguments:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtKernelArgs" runat="server" CssClass="textbox">
        </asp:TextBox>
    </div>
    <br class="clear"/>
    <br/>
    </div>
    
    <br />
    <h2>Task Options</h2>
      <div class="size-4-3 column">
        Web Cancel
    </div>
       <div class="size-5-2 column hidden-check">
        <asp:CheckBox runat="server" id="chkWebCancel" ClientIDMode="Static"/>
        <label for="chkWebCancel"></label>
    </div>
  

    <br class="clear"/>

    <div class="size-4 column">
        Task Completed Action
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlTaskComplete" runat="server" CssClass="ddlist">
            <asp:ListItem>Reboot</asp:ListItem>
            <asp:ListItem>Power Off</asp:ListItem>
            <asp:ListItem>Exit To Shell</asp:ListItem>
        </asp:DropDownList>
    </div>
   
     <br class="clear"/>
    


    <br/>
  
    
    <br />
    <h2>Deploy Options</h2>
    <div class="size-4-3 column">
    Change Computer Name
</div>
 <div class="size-5-2 column hidden-check">
        <asp:CheckBox runat="server" id="chkChangeName" ClientIDMode="Static"/>
        <label for="chkChangeName"></label>
    </div>
<br class="clear"/>

    
    <div id="LinuxBlock1" runat="server">
    <div class="size-4-3 column">
        Don't Expand Volumes
    </div>
         <div class="size-5-2 column hidden-check">
        <asp:CheckBox runat="server" id="chkDownNoExpand" ClientIDMode="Static"/>
        <label for="chkDownNoExpand"></label>
    </div>
  
    </div>

    <br class="clear"/>


    <div id="LinuxAll2" runat="server">
    <div class="size-4-3 column">
        Update BCD
    </div>
     <div class="size-5-2 column hidden-check">
        <asp:CheckBox runat="server" id="chkAlignBCD" ClientIDMode="Static"/>
        <label for="chkAlignBCD"></label>
    </div>
    
    <br class="clear"/>
    
     <div class="size-4-3 column">
        Randomize GUIDs
    </div>
     <div class="size-5-2 column hidden-check">
        <asp:CheckBox runat="server" id="chkRandomize" ClientIDMode="Static"/>
        <label for="chkRandomize"></label>
    </div>
    <br class="clear"/>

    <div class="size-4-3 column">
        Fix Boot Sector
    </div>
    <div class="size-5-2 column hidden-check">
        <asp:CheckBox runat="server" id="chkRunFixBoot" ClientIDMode="Static"/>
        <label for="chkRunFixBoot"></label>
    </div>
    <br class="clear"/>
       <div class="size-4-3 column">
       Don't Update NVRAM
    </div>
     <div class="size-5-2 column hidden-check">
        <asp:CheckBox runat="server" id="chkNvram" ClientIDMode="Static"/>
        <label for="chkNvram"></label>
    </div>
    <br class="clear"/>
    </div>
   

     
    
    <div id="LinuxAll3" runat="server">
    <div class="size-4-3 column">
        Create Partitions Method
    </div>
    <div class="size-5 column">
        <asp:dropdownlist ID="ddlPartitionMethodLin" runat="server" CssClass="ddlist"  AutoPostBack="True">
            <asp:ListItem>Use Original MBR / GPT</asp:ListItem>
            <asp:ListItem>Standard</asp:ListItem>
            <asp:ListItem>Dynamic</asp:ListItem>
            <asp:ListItem>Custom Script</asp:ListItem>
        </asp:dropdownlist>
    </div>
    <br class="clear"/>
    </div>

    
    <div id="winpe1" runat="server">
    <div class="size-4-3 column">
        Create Partitions Method
    </div>
    <div class="size-5 column">
        <asp:dropdownlist ID="ddlPartitionMethodWin" runat="server" CssClass="ddlist"  AutoPostBack="True">
            <asp:ListItem>Standard</asp:ListItem>
            <asp:ListItem>Dynamic</asp:ListItem>
            <asp:ListItem>Custom Script</asp:ListItem>
        </asp:dropdownlist>
    </div>
    <br class="clear"/>
        </div>

   
    
    <div id="LinuxAll4" runat="server">
     <div class="size-4-3 column">
        Force Standard EFI Partitions
    </div>
    <div class="size-5-2 column hidden-check">
        <asp:CheckBox runat="server" id="chkForceEfi" ClientIDMode="Static"/>
        <label for="chkForceEfi"></label>
    </div>
    <br class="clear"/>
     <div class="size-4-3 column">
        Force Standard Legacy Partitions
    </div>
     <div class="size-5-2 column hidden-check">
        <asp:CheckBox runat="server" id="chkForceLegacy" ClientIDMode="Static"/>
        <label for="chkForceLegacy"></label>
    </div>
    <br class="clear"/>


    <div class="size-4-3 column">
        Force Dynamic Partition For Exact Hdd Match
    </div>
     <div class="size-5-2 column hidden-check">
        <asp:CheckBox runat="server" id="chkDownForceDynamic" ClientIDMode="Static" AutoPostBack="True" OnCheckedChanged="chkForceDynamic_OnCheckedChanged"/>
        <label for="chkDownForceDynamic"></label>
    </div>
  

    <br class="clear"/>
    <br/>
    </div>
    <br /><br />
    <h2>Upload Options</h2>
    
    <div id="LinuxAll5" runat="server">
    <div class="size-4-3 column">
        Remove GPT Structures
    </div>
   <div class="size-5-2 column hidden-check">
        <asp:CheckBox runat="server" id="chkRemoveGpt" ClientIDMode="Static"/>
        <label for="chkRemoveGpt"></label>
    </div>
    <br class="clear"/>
        
      <div class="size-4-3 column">
        Skip Hibernation Check
    </div>
   <div class="size-5-2 column hidden-check">
        <asp:CheckBox runat="server" id="chkSkipHibernation" ClientIDMode="Static"/>
        <label for="chkSkipHibernation"></label>
    </div>
    <br class="clear"/>

      <div class="size-4-3 column">
        Skip Bitlocker Check
    </div>
   <div class="size-5-2 column hidden-check">
        <asp:CheckBox runat="server" id="chkSkipBitlocker" ClientIDMode="Static"/>
        <label for="chkSkipBitlocker"></label>
    </div>
    <br class="clear"/>
        </div>
    
    <div id="LinuxBlock2" runat="server">
    <div class="size-4-3 column">
        Don't Shrink Volumes
    </div>
   <div class="size-5-2 column hidden-check">
        <asp:CheckBox runat="server" id="chkUpNoShrink" ClientIDMode="Static"/>
        <label for="chkUpNoShrink"></label>
    </div>
    <br class="clear"/>

    <div class="size-4-3 column">
        Don't Shrink LVM Volumes
    </div>
    <div class="size-5-2 column hidden-check">
        <asp:CheckBox runat="server" id="chkUpNoShrinkLVM" ClientIDMode="Static"/>
        <label for="chkUpNoShrinkLVM"></label>
    </div>
    <br class="clear"/>

    <div class="size-4 column">
        Compression Algorithm:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlCompAlg" runat="server" CssClass="ddlist">
            <asp:ListItem>gzip</asp:ListItem>
            <asp:ListItem>lz4</asp:ListItem>
            <asp:ListItem>none</asp:ListItem>
        </asp:DropDownList>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Compression Level:
    </div>

    <div class="size-5 column">
        <asp:DropDownList ID="ddlCompLevel" runat="server" CssClass="ddlist">
            <asp:ListItem>1</asp:ListItem>
            <asp:ListItem>2</asp:ListItem>
            <asp:ListItem>3</asp:ListItem>
            <asp:ListItem>4</asp:ListItem>
            <asp:ListItem>5</asp:ListItem>
            <asp:ListItem>6</asp:ListItem>
            <asp:ListItem>7</asp:ListItem>
            <asp:ListItem>8</asp:ListItem>
            <asp:ListItem>9</asp:ListItem>
        </asp:DropDownList>
    </div>

<br class="clear"/>
        </div>
    
    <div id="LinuxFileWinpe1" runat="server">
   
        </div>

<div class="size-4-3 column">
    Only Upload Schema
</div>
 <div class="size-5-2 column hidden-check">
        <asp:CheckBox runat="server" id="chkSchemaOnly" ClientIDMode="Static"/>
        <label for="chkSchemaOnly"></label>
    </div>
    <br class="clear"/>
    <br/>
  
    
    <div id="LinuxAll6" runat="server">
        <br /><br />
    <h2>Multicast Options</h2>
     <div class="size-4 column">
        Sender Arguments:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtSender" runat="server" CssClass="textbox">
        </asp:TextBox>
    </div>
    <br class="clear"/>

    <div class="size-4 column">
        Receiver Arguments:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtReceiver" runat="server" CssClass="textbox">
        </asp:TextBox>
    </div>
    <br class="clear"/>
        </div>
  
  

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <h5><span style="color: #ff9900;">Organization Name:</span></h5>
<p>The name of your organization, this will be displayed in all certificates generated by Theopenem.</p>
<p>&nbsp;</p>

</asp:Content>
