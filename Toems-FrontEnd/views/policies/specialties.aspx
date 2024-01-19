<%@ Page Title="" Language="C#" MasterPageFile="~/views/policies/policies.master" AutoEventWireup="true" CodeBehind="specialties.aspx.cs" Inherits="Toems_FrontEnd.views.policies.specialties" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= Policy.Name %></li>
    <li>Specialties</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
        <%= Policy.Name %>
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Update Policy" CssClass="main-action"></asp:LinkButton></li>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#specialties').addClass("nav-current");
          });
      </script>
   
    <div class="size-4 column">
        Collect Inventory:
    </div>
     <div class="size-5 column">
              <asp:DropDownList ID="ddlInventory" runat="server" CssClass="ddlist"></asp:DropDownList>
        </div>
     <br class="clear"/>
    
     <div class="size-4 column">
        Login Tracker:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkLoginTracker" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkLoginTracker"></label>
        </div>
     <br class="clear"/>
     <div class="size-4 column">
        Application Monitor:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkApplicationMonitor" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkApplicationMonitor"></label>
        </div>
    <br class="clear"/>
     <div class="size-4 column">
        Run Winget Updates:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkWingetUpdates" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkWingetUpdates"></label>
        </div>
    <br class="clear"/>
     <div class="size-4 column">
        Winget Use Download Connections:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkWingetDownloadConnections" runat="server" ClientIDMode="Static"></asp:CheckBox>
         <label for="chkWingetDownloadConnections"></label>
        </div>
    <br class="clear"/>
  
         <div class="size-4 column">
        Remote Access:
    </div>
     <div class="size-5 column">
            <asp:DropDownList ID="ddlRemoteAccess" runat="server" CssClass="ddlist"></asp:DropDownList>
        </div>
      <br class="clear"/>
       <div class="size-4 column">
        Join Domain:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkJoinDomain" runat="server" ClientIDMode="Static"></asp:CheckBox>
          <label for="chkJoinDomain">Toggle</label>
        </div>
     <br class="clear"/>
      <div class="size-4 column">
        Domain OU:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtDomainOU" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
      <div class="size-4 column">
        Image Prep Cleanup:
    </div>
     <div class="size-setting column hidden-check">
            <asp:CheckBox ID="chkImagePrepCleanup" runat="server" ClientIDMode="Static"></asp:CheckBox>
          <label for="chkImagePrepCleanup">Toggle</label>
        </div>
     <br class="clear"/>
    <div class="size-4 column">
        Install Available Windows Updates:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlWinUpdates" runat="server" CssClass="ddlist"></asp:DropDownList>
    </div>
     <br class="clear"/>

   
</asp:Content>