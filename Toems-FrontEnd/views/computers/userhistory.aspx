<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="userhistory.aspx.cs" Inherits="Toems_FrontEnd.views.computers.userhistory" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li> <%= ComputerEntity.Name %></li>
    <li>User History</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    <%= ComputerEntity.Name %>
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnExport" runat="server" OnClick="btnExport_OnClick" Text="Export To CSV" CssClass="main-action" /></li>

</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#userhistory').addClass("nav-current");

              $("[id*=gvUserHistory] td").hover(function () {
                  $("td", $(this).closest("tr")).addClass("hover_row");
              }, function () {
                  $("td", $(this).closest("tr")).removeClass("hover_row");
              });
          });

    </script>

    <div class="size-7 column">

        <asp:TextBox ID="txtSearch" runat="server" CssClass="rounded-search" OnTextChanged="txtSearch_OnTextChanged"></asp:TextBox>
    </div>

    <br class="clear"/>

    <asp:GridView ID="gvUserHistory" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gvUserHistory_OnSorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="Username" HeaderText="Name" SortExpression="Username" ItemStyle-CssClass="width_200"></asp:BoundField>
              <asp:TemplateField HeaderText="Login Date" SortExpression="LoginDateTime" ItemStyle-CssClass="width_200">
                    <ItemTemplate>
                        <asp:Label ID="lblLocalTime" runat="server" Text='<%# Convert.ToDateTime(Eval("LoginDateTime")).ToLocalTime() %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
              <asp:TemplateField HeaderText="Logout Date" SortExpression="LogoutDateTime">
                    <ItemTemplate>
                        <asp:Label ID="lblLocalTimeOut" runat="server" Text='<%# Convert.ToDateTime(Eval("LogoutDateTime")).ToLocalTime() %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
           
        </Columns>
        <EmptyDataTemplate>
            No User History Found
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>Displays the login / logout time of all users of the computer.</p>
</asp:Content>