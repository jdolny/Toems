<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="custominventory.aspx.cs" Inherits="Toems_FrontEnd.views.computers.custominventory" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li> <%= ComputerEntity.Name %></li>
    <li>Custom Inventory</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    <%= ComputerEntity.Name %>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="DropDownActionsSub">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#custominventory').addClass("nav-current");

              $("[id*=gvSoftware] td").hover(function () {
                  $("td", $(this).closest("tr")).addClass("hover_row");
              }, function () {
                  $("td", $(this).closest("tr")).removeClass("hover_row");
              });
          });

    </script>

   

    <br class="clear"/>

    <asp:GridView ID="gvSoftware" runat="server" AllowSorting="True" DataKeyNames="ComputerId" OnSorting="gvSoftware_OnSorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="ModuleName" HeaderText="Name" SortExpression="ModuleName" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Value" HeaderText="Value" SortExpression="Value"></asp:BoundField>
           
        </Columns>
        <EmptyDataTemplate>
            No Custom Inventory Found
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>Theopenem can be used to collect additional inventory information that is not part of the standard inventory collection.  These inventory script modules results will appear on this page.</p>
</asp:Content>
