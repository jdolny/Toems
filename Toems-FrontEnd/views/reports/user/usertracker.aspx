<%@ Page Title="" Language="C#" MasterPageFile="~/views/reports/user/user.master" AutoEventWireup="true" CodeBehind="usertracker.aspx.cs" Inherits="Toems_FrontEnd.views.reports.user.usertracker" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
    <li>Login History</li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubNavTitle_Sub2" runat="server">
    User Reports
    </asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub2" runat="server">
    <li><asp:LinkButton ID="btnExport" runat="server" OnClick="btnExport_OnClick" Text="Export To CSV" CssClass="main-action"/></li>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" runat="server">
        <script type="text/javascript">
        $(document).ready(function() {
            $('#usertracker').addClass("nav-current");

            $("[id*=gvComputers] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });

    </script>

   
    <div class="size-7 column">

        <asp:TextBox ID="txtSearch" runat="server" CssClass="rounded-search" OnTextChanged="txtSearch_OnTextChanged"></asp:TextBox>
    </div>

    <br class="clear"/>

    <asp:GridView ID="gvComputers" runat="server" AllowSorting="True" OnSorting="gvComputers_OnSorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="ComputerName" HeaderText="Computer Name" SortExpression="ComputerName" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="UserName" HeaderText="User Name" SortExpression="UserName" ItemStyle-CssClass="width_200"></asp:BoundField>
          
               <asp:TemplateField HeaderText="Login Date" SortExpression="LoginTime" ItemStyle-CssClass="width_200">
                    <ItemTemplate>
                        <asp:Label ID="lblLocalTime" runat="server" Text='<%# Convert.ToDateTime(Eval("LoginTime")).ToLocalTime() %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
              <asp:TemplateField HeaderText="Logout Date" SortExpression="LogoutTime">
                    <ItemTemplate>
                        <asp:Label ID="lblLocalTimeOut" runat="server" Text='<%# Convert.ToDateTime(Eval("LogoutTime")).ToLocalTime() %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            No User Logins Found
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>This page allows you to search for a specific user to determine every computer they have logged into and when.</p>
</asp:Content>
