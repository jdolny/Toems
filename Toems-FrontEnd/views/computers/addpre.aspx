<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="addpre.aspx.cs" Inherits="Toems_FrontEnd.views.computers.addpre" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Add Pre-Provisions</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
 Computers
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="DropDownActionsSub">
      <li><asp:LinkButton ID="btnAddPre" runat="server" OnClick="btnAddPre_OnClick" Text="Add PreProvisions" CssClass="main-action" /></li>
   
</asp:Content>





<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#preprovision').addClass("nav-current");
            $('#create').addClass("nav-current");

        });

    </script>

  <asp:TextBox ID="txtPre" runat="server" TextMode="MultiLine" Height="500" Width="400" CssClass="descbox border" ></asp:TextBox>
  
   
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page is used to create pre-provisioned computers.  Enter the computer names, one per line, in the textbox and select add PreProvisions.  Preprovisioned computers serve two purposes.  First, it allows you to add computers before they are provisioned.  This allows you to place computers in the proper groups ahead of time.  Second, is for security.  A security option is available that will not let computers provision unless they have been pre-provisioned.  This option is available in Admin Settings->Security->PreProvision Required.  If AD sync is used, all synced computers are added as pre-provisioned computers.</p>
</asp:Content>