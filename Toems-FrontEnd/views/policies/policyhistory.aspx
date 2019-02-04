<%@ Page Title="" Language="C#" MasterPageFile="~/views/policies/policies.master" AutoEventWireup="true" CodeBehind="policyhistory.aspx.cs" Inherits="Toems_FrontEnd.views.policies.policyhistory" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= Policy.Name %></li>
    <li>History</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
        <%= Policy.Name %>
</asp:Content>


<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
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

   

    <asp:GridView ID="gvHistory" runat="server" AutoGenerateColumns="False" CssClass="Gridview extraPad" AlternatingRowStyle-CssClass="alt">
        <Columns>
               <asp:TemplateField>
                <ItemTemplate>
                    <asp:HyperLink ID="view" runat="server" NavigateUrl='<%# String.Format("~/views/policies/hashview.aspx?hash={0}&policyId={1}", Eval("Hash"), Eval("PolicyId")) %>' Text="View" />
                </ItemTemplate>
            </asp:TemplateField>
           <asp:BoundField DataField="Hash" HeaderText="Hash" ItemStyle-CssClass="width_200" />
                 <asp:TemplateField HeaderText="Modify Time" >
                    <ItemTemplate>
                        <asp:Label ID="lblModifyTime" runat="server" Text='<%# Convert.ToDateTime(Eval("ModifyTime")).ToLocalTime() %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
              <asp:BoundField DataField="Json" HeaderText="Json" ItemStyle-CssClass="width_200" />

        </Columns>
        <EmptyDataTemplate>
            No Policy History Found
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>Displays the JSON representation of the policy as it has changed over time.</p>
</asp:Content>