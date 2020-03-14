<%@ Page Title="" Language="C#" MasterPageFile="~/views/images/images.master" AutoEventWireup="true" CodeBehind="history.aspx.cs" Inherits="Toems_FrontEnd.views.images.history" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
     <li>
        <a href="<%= ResolveUrl("~/views/images/general.aspx") %>?imageId=<%= ImageEntity.Id %>"><%= ImageEntity.Name %></a>
    </li>
    <li>History</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
     <%= ImageEntity.Name %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" runat="Server">
   
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" runat="Server">
  <script type="text/javascript">
        $(document).ready(function() {
            $('#history').addClass("nav-current");
   $("[id*=gvHistory] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });

    </script>

     

 
    <div class="size-11 column">
        <div class="custom-select">
        <asp:DropDownList runat="server" ID="ddlLimit" AutoPostBack="True" OnSelectedIndexChanged="ddl_OnSelectedIndexChanged" CssClass="ddlist">
            <asp:ListItem></asp:ListItem>
            <asp:ListItem>25</asp:ListItem>
            <asp:ListItem>100</asp:ListItem>
            <asp:ListItem Selected="True">250</asp:ListItem>
            <asp:ListItem >500</asp:ListItem>
            <asp:ListItem>1000</asp:ListItem>
            <asp:ListItem>5000</asp:ListItem>
            <asp:ListItem>All</asp:ListItem>
        </asp:DropDownList>
            </div>
    </div>
    <div class="size-11 column">
        <div class="custom-select">
            <asp:DropDownList ID="ddlType" runat="server" CssClass="ddlist" AutoPostBack="True" OnSelectedIndexChanged="ddlType_OnSelectedIndexChanged">
             <asp:ListItem>All</asp:ListItem>
            <asp:ListItem>Create</asp:ListItem>
            <asp:ListItem>Update</asp:ListItem>
            <asp:ListItem>Deploy</asp:ListItem>
            <asp:ListItem>Upload</asp:ListItem>
                </asp:DropDownList>
        </div>
    </div>
    <br class="clear"/>
    <br />
    <asp:GridView ID="gvHistory" runat="server" DataKeyNames="Id" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
             <asp:BoundField DataField="Id" HeaderText="AuditId" Visible="False"/>
           
           
            <asp:BoundField DataField="Id" HeaderText="AuditId" Visible="False"/>
            <asp:BoundField DataField="UserName" HeaderText="User" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="AuditType" HeaderText="Type" SortExpression="Mac" ItemStyle-CssClass="width_200"/>
            <asp:BoundField DataField="DateTime" HeaderText="Date" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="ObjectJson" HeaderText="Json" ItemStyle-CssClass="width_200 mobi-hide-smallest" HeaderStyle-CssClass="mobi-hide-smallest"></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No Image History Found
        </EmptyDataTemplate>
    </asp:GridView>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>Specifies if the storage path is local to this server or on a remote SMB share.  The local type can only be used if only a single server is used for all Toec Api's, Toems Api's, and Toems Front Ends.</p>
<h5><span style="color: #ff9900;">Storage Path:</span></h5>
<p>The path to the storage directory.  If the storage type is local, the path should be a local drive, such as c:\toems-local-storage.  If the storage type is SMB, the path must be a UNC path, such as \\server\toems-remote-storage.</p>
<h5><span style="color: #ff9900;">Username:</span></h5>
<p>Only available for a storage type of SMB.  This is the username used to connect to the share.</p>
<h5><span style="color: #ff9900;">Password:</span></h5>
<p>Only available for a storage type of SMB.  This is the password used to connect to the share.</p>
<h5><span style="color: #ff9900;">Domain:</span></h5>
<p>Only available for a storage type of SMB.  This is the domain used to connect to the share.</p>
</asp:Content>
