<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/comservers/comservers.master" AutoEventWireup="true" CodeBehind="editcomservercluster.aspx.cs" Inherits="Toems_FrontEnd.views.admin.comservers.editcomservercluster" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li><a href="<%= ResolveUrl("~/views/admin/comservers/comservercluster.aspx") %>?level=2">Com Server Clusters</a></li>
    <li><%=ComServerCluster.Name %></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
<%=ComServerCluster.Name %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
  <li><asp:LinkButton ID="buttonEdit" runat="server" OnClick="buttonEdit_OnClick" Text="Update Cluster" CssClass="main-action" /></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#general').addClass("nav-current");
        });
    </script>
    <div class="size-4 column">
        Display Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtName" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>

    </div>
    <br class="clear"/>
  
      <div class="size-4 column">
        Description:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtDescription" runat="server" CssClass="descbox" TextMode="MultiLine"></asp:TextBox>
    </div>
     <br class="clear" />
    <div class="size-4 column">
        Default Cluster
    </div>
    <div class="size-5 column hidden-check">
        <asp:CheckBox ID="chkDefault" runat="server" ClientIDMode="Static" />
        <label for="chkDefault"></label>
    </div>
    <br class="clear"/>
     <asp:GridView ID="gvServers" runat="server"  DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:TemplateField>
                <HeaderStyle CssClass="chkboxwidth"></HeaderStyle>
                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <HeaderTemplate>
                    <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="True" OnCheckedChanged="chkSelectAll_CheckedChanged"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelector" runat="server"/>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="False"/>
            <asp:BoundField DataField="DisplayName" HeaderText="Display Name" ItemStyle-CssClass="width_200"></asp:BoundField>
              <asp:BoundField DataField="Url" HeaderText="URL" ItemStyle-CssClass="width_200"></asp:BoundField>
           <asp:TemplateField ItemStyle-CssClass="width_100" HeaderText="Role">
                <ItemTemplate>
                    <asp:DropDownList runat="server" Id="ddlRole" CssClass="ddlist order">
                        <asp:ListItem>Active</asp:ListItem>
                         <asp:ListItem>Passive</asp:ListItem>

                        </asp:DropDownList>
                </ItemTemplate>

            </asp:TemplateField>
              <asp:TemplateField>
                <ItemTemplate>
                   &nbsp;
                </ItemTemplate>

            </asp:TemplateField>


        </Columns>
        <EmptyDataTemplate>
            No Client Communication Servers Found
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>This page is used to define a new com server cluster.</p>
<h5><span style="color: #ff9900;">Display Name:</span></h5>
<p>A name used to identify the com server cluster, it has not effect on the functionality.</p>
<h5><span style="color: #ff9900;">Description:</span></h5>
<p>An optional description for the com server.</p>
<h5><span style="color: #ff9900;">Default Cluster:</span></h5>
<p>When this option is enabled, it marks the cluster as the default cluster.  Each group of computers can have a statically defined com server cluster that forces all computers in that group to use the assigned cluster.  If a group does not have a com server cluster assigned, then the default com server cluster is used.</p>
<h5><span style="color: #ff9900;">Com Server List:</span></h5>
<p>Below the default cluster option, is a list of all available com servers, check each com server that should be part of the cluster.  Each com server can have an active or passive role.  All com servers marked as active will be used by the client computers in a load balanced manner.  If a connection cannot be made with any active com server, the client will attempt to connect to any passive com servers.  When using a com server in your DMZ it should almost always be marked as passive.  This way the clients will connect to the internal active com servers when on site and will fail back to the passive com server in your DMZ when offsite.</p>
</asp:Content>