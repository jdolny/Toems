<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" CodeBehind="server.aspx.cs" Inherits="Toems_FrontEnd.views.admin.server" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Server</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Admin Settings
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnUpdateSettings" runat="server" Text="Update Server Settings" OnClick="btnUpdateSettings_OnClick" CssClass="main-action"/></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#server').addClass("nav-current");
        });
    </script>
    <div class="size-4 column">
        Organization Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtOrganization" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
      <br class="clear"/>
        <div class="size-4 column">
            Web UI Timeout (Minutes):
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtWebTimeout" runat="server" CssClass="textbox"></asp:TextBox>
        </div>
    <br class="clear"/>

    <div class="size-4 column">
            Default Computers Page:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlComputerView" runat="server" CssClass="ddlist">
                <asp:ListItem>Active</asp:ListItem>
                <asp:ListItem>Image Only</asp:ListItem>
                <asp:ListItem>All</asp:ListItem>
            </asp:DropDownList>
        </div>

    <br class="clear" />
         <div class="size-4 column">
            Default Computer Sort:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlComputerSort" runat="server" CssClass="ddlist">
                <asp:ListItem>Name</asp:ListItem>
                <asp:ListItem>Last Checkin</asp:ListItem>
            </asp:DropDownList>
        </div>
        <br class="clear"/>

      <div class="size-4 column">
            Default Login Page:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlLoginPage" runat="server" CssClass="ddlist">
                <asp:ListItem>Dashboard</asp:ListItem>
                <asp:ListItem>Active Computers</asp:ListItem>
                <asp:ListItem>Image Only Computers</asp:ListItem>
                <asp:ListItem>All Computers</asp:ListItem>
            </asp:DropDownList>
        </div>
        <br class="clear"/>

     <div class="size-4 column">
            Default Image Replication Mode:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlImageReplication" runat="server" CssClass="ddlist" OnSelectedIndexChanged="ddlImageReplication_SelectedIndexChanged" AutoPostBack="true">
                <asp:ListItem>All</asp:ListItem>
                <asp:ListItem>None</asp:ListItem>
                <asp:ListItem>Selective</asp:ListItem>
            </asp:DropDownList>
        </div>
        <br class="clear"/>

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
     <br class="clear"/>
     <div class="size-4 column">
            Image Replication Time:
        </div>
        <div class="size-5 column">
            <asp:DropDownList ID="ddlReplicationTime" runat="server" CssClass="ddlist">
                <asp:ListItem>Immediately</asp:ListItem>
                <asp:ListItem>Scheduler</asp:ListItem>
            </asp:DropDownList>
        </div>
        <br class="clear"/>
  
  

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <h5><span style="color: #ff9900;">Organization Name:</span></h5>
<p>The name of your organization, this will be displayed in all certificates generated by Theopenem.</p>
<p>&nbsp;</p>

</asp:Content>
