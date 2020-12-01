<%@ Page Title="" Language="C#" MasterPageFile="~/views/images/images.master" AutoEventWireup="true" CodeBehind="replicationstatus.aspx.cs" Inherits="Toems_FrontEnd.views.images.replicationstatus" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
     <li>
        <a href="<%= ResolveUrl("~/views/images/general.aspx") %>?imageId=<%= ImageEntity.Id %>"><%= ImageEntity.Name %></a>
    </li>
    <li>Replication Status</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
     <%= ImageEntity.Name %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" runat="Server">

</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" runat="Server">
  <script type="text/javascript">
        $(document).ready(function() {
            $('#repstatus').addClass("nav-current");

        });

    </script>
    <asp:Label id="lblLocal" runat="server"></asp:Label>

      <asp:GridView ID="gvCom" runat="server"   AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
    <Columns>

     
        <asp:BoundField DataField="Servername" HeaderText="Server" ItemStyle-CssClass="width_200"></asp:BoundField>
        <asp:BoundField DataField="Status" HeaderText="Status"></asp:BoundField>
         

    </Columns>

</asp:GridView>
       
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
If you are using more than a single server, images will need to replicate among them all.  This page displays the status.
</asp:Content>

