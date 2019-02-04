<%@ Page Title="" Language="C#" MasterPageFile="~/views/users/user.master" AutoEventWireup="true" CodeBehind="history.aspx.cs" Inherits="Toems_FrontEnd.views.users.history" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= ToemsUser.Name %></li>
    <li>History</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
<%= ToemsUser.Name %>
</asp:Content>


<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
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
            </asp:DropDownList>
        </div>
    </div>
    <br class="clear"/>
    <br />
    <asp:GridView ID="gvHistory" runat="server" DataKeyNames="Id" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="AuditId" Visible="False"/>
            <asp:BoundField DataField="ObjectType" HeaderText="Object Type" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="ObjectName" HeaderText="Object Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="AuditType" HeaderText="Type" SortExpression="Mac" ItemStyle-CssClass="width_200"/>
            <asp:BoundField DataField="DateTime" HeaderText="Date" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="ObjectJson" HeaderText="Json" ItemStyle-CssClass="width_200 mobi-hide-smallest" HeaderStyle-CssClass="mobi-hide-smallest"></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No User History Found
        </EmptyDataTemplate>
    </asp:GridView>


</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>Displays actions that the user has performed in Theopenem.  Currently not everything is tracked, but most are and will continue to be expanded in future versions.</p>
</asp:Content>