<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="software.aspx.cs" Inherits="Toems_FrontEnd.views.computers.software" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li> <%= ComputerEntity.Name %></li>
    <li>Software</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">

    <%= ComputerEntity.Name %>
</asp:Content>

<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnExport" runat="server" OnClick="btnExport_OnClick" Text="Export To CSV" CssClass="main-action"/></li>

</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#software').addClass("nav-current");

              $("[id*=gvSoftware] td").hover(function () {
                  $("td", $(this).closest("tr")).addClass("hover_row");
              }, function () {
                  $("td", $(this).closest("tr")).removeClass("hover_row");
              });
          });

    </script>

    <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>
    <div class="size-7 column">

        <asp:TextBox ID="txtSearch" runat="server" CssClass="rounded-search" OnTextChanged="txtSearch_OnTextChanged"></asp:TextBox>
    </div>

    <br class="clear"/>

    <asp:GridView ID="gvSoftware" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gvSoftware_OnSorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Version" HeaderText="Version" SortExpression="Version" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="UninstallString" HeaderText="Uninstall String" ></asp:BoundField>
           
        </Columns>
        <EmptyDataTemplate>
            No Software Found
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page displays all of the software that is currently installed on the computer.  This info is also collected during an inventory scan.</p>
</asp:Content>
