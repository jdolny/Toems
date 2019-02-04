<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="policyhistory.aspx.cs" Inherits="Toems_FrontEnd.views.computers.policyhistory" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li> <%= ComputerEntity.Name %></li>
    <li>Policy History</li>
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
              $('#policyhistory').addClass("nav-current");

              $("[id*=gvHistory] td").hover(function () {
                  $("td", $(this).closest("tr")).addClass("hover_row");
              }, function () {
                  $("td", $(this).closest("tr")).removeClass("hover_row");
              });
          });

    </script>

    <asp:GridView ID="gvHistory" runat="server"  DataKeyNames="PolicyId" AllowSorting="True" OnSorting="gvHistory_OnSorting" AutoGenerateColumns="False" CssClass="Gridview extraPad" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:TemplateField HeaderText="Policy" SortExpression="PolicyName">
                <ItemTemplate>
                    <asp:HyperLink ID="policyName" runat="server" NavigateUrl='<%# Eval("PolicyId", "~/views/policies/general.aspx?policyId={0}") %>' Text='<%# Bind("PolicyName") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Result" HeaderText="Result" ItemStyle-CssClass="width_200" SortExpression="Result"></asp:BoundField>
              <asp:TemplateField HeaderText="Date" SortExpression="RunTime">
                    <ItemTemplate>
                        <asp:Label ID="lblLocalTime" runat="server" Text='<%# Convert.ToDateTime(Eval("RunTime")).ToLocalTime() %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
               <asp:TemplateField HeaderText="Policy Hash" SortExpression="PolicyHash">
                <ItemTemplate>
                    <asp:HyperLink ID="policyHash" runat="server" NavigateUrl='<%# String.Format("~/views/policies/hashview.aspx?hash={0}&policyId={1}", Eval("PolicyHash"), Eval("PolicyId")) %>' Text='<%# Bind("PolicyHash") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            No History Found
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page displays the results of all policies that have ran on the computer.</p>
</asp:Content>
