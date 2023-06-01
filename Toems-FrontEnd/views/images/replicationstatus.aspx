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
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Update Image" CssClass="main-action"></asp:LinkButton></li>
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

     <div class="size-4 column">
        Image Replication Mode:
    </div>
    <div class="size-5 column">
       
        <asp:DropDownList ID="ddlReplicationMode" runat="server" CssClass="ddlist" AutoPostBack="true" OnSelectedIndexChanged="ddlReplicationMode_SelectedIndexChanged"></asp:DropDownList>
  
            </div>
   <br class="clear" />
<div id="divComServers" runat="server">
<asp:GridView ID="gvServers" runat="server"  DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
    <Columns>
        <asp:TemplateField>
            <HeaderStyle CssClass="chkboxwidth"></HeaderStyle>
            <ItemStyle CssClass="chkboxwidth"></ItemStyle>
      
            <ItemTemplate>
                <asp:CheckBox ID="chkSelector" runat="server"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/admin/comservers/editcomserver.aspx?level=2&serverId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
        <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="False"/>
        <asp:BoundField DataField="DisplayName" HeaderText="Display Name" ItemStyle-CssClass="width_200"></asp:BoundField>
        <asp:BoundField DataField="Url" HeaderText="URL"></asp:BoundField>
         

    </Columns>
    <EmptyDataTemplate>
        No Client Communication Servers Found
    </EmptyDataTemplate>
</asp:GridView>
</div>
       
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
If you are using more than a single server, images will need to replicate among them all.  This page displays the status.
</asp:Content>

