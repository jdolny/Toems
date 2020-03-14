<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="imaginglogs.aspx.cs" Inherits="Toems_FrontEnd.views.computers.imaginglogs" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li> <%= ComputerEntity.Name %></li>
    <li>Imaging Logs</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">

    <%= ComputerEntity.Name %>
</asp:Content>

<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">


</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#imaginglogs').addClass("nav-current");

              $("[id*=gvLogs] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
          });

    </script>

       <div id="SearchLogs" runat="server">
        <p class="total">
            <asp:Label ID="lblTotal" runat="server"></asp:Label>
        </p>
        <asp:GridView ID="gvLogs" runat="server" DataKeyNames="Id" AutoGenerateColumns="False" CssClass="Gridview extraPad" AlternatingRowStyle-CssClass="alt">
            <Columns>
                <asp:TemplateField>

                    <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                    <ItemTemplate>

                        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="btnExport_OnClick" Text="Export"/>


                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderStyle CssClass="width_100"></HeaderStyle>
                    <ItemStyle CssClass="width_100"></ItemStyle>
                    <ItemTemplate>
                        <asp:LinkButton ID="btnView" runat="server" OnClick="btnView_OnClick" Text="View"/>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Id" Visible="False"/>
                   <asp:BoundField DataField="SubType" HeaderText="Type" SortExpression="SubType" ItemStyle-CssClass="width_200"/>
                <asp:BoundField DataField="LogTime" HeaderText="Time" SortExpression="LogTime" ></asp:BoundField>
             


            </Columns>
            <EmptyDataTemplate>
                No Logs Found
            </EmptyDataTemplate>
        </asp:GridView>
    </div>

    <div id="ViewLog" runat="server" Visible="False">
        <asp:GridView ID="gvLogView" runat="server" CssClass="Gridview log" ShowHeader="false">
        </asp:GridView>
    </div>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page displays all of the certificates from the Personal, Trusted Root, and Trusted Intermediate stores for the computer.  This info is collected during an inventory scan.</p>
</asp:Content>


