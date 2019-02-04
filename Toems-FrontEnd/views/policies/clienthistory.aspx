<%@ Page Title="" Language="C#" MasterPageFile="~/views/policies/policies.master" AutoEventWireup="true" CodeBehind="clienthistory.aspx.cs" Inherits="Toems_FrontEnd.views.policies.clienthistory" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= Policy.Name %></li>
    <li>Client History</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
        <%= Policy.Name %>
</asp:Content>


<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#clienthistory').addClass("nav-current");

            $("[id*=gvHistory] td").hover(function () {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function () {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });

    </script>

    <div class="size-7 column">
        <asp:TextBox ID="txtSearch" runat="server" CssClass="rounded-search" OnTextChanged="search_Changed"></asp:TextBox>
    </div>

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
    <br class="clear"/>

    <asp:GridView ID="gvHistory" runat="server" AutoGenerateColumns="False" AllowSorting="True" OnSorting="gvHistory_OnSorting" CssClass="Gridview extraPad" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:TemplateField HeaderText="Computer" SortExpression="ComputerName">
                <ItemTemplate>
                    <asp:HyperLink ID="computerName" runat="server" NavigateUrl='<%# Eval("ComputerId", "~/views/computers/general.aspx?computerId={0}") %>' Text='<%# Bind("ComputerName") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="User" HeaderText="User" ItemStyle-CssClass="width_200" />
            <asp:BoundField DataField="Result" HeaderText="Result" ItemStyle-CssClass="width_200" SortExpression="Result"/>
              <asp:TemplateField HeaderText="Last Run Time" SortExpression="LastRunTime" >
                    <ItemTemplate>
                        <asp:Label ID="lblLocalTime" runat="server" Text='<%# Convert.ToDateTime(Eval("LastRunTime")).ToLocalTime() %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
        
              <asp:TemplateField HeaderText="Policy Hash" SortExpression="Hash">
                <ItemTemplate>
                    <asp:HyperLink ID="policyHash" runat="server" NavigateUrl='<%# String.Format("~/views/policies/hashview.aspx?hash={0}&policyId={1}", Eval("Hash"), Eval("PolicyId")) %>' Text='<%# Bind("Hash") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            No Client History Found
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page displays every endpoint the policy was run as well the user that policy ran under.  It also shows the result of the policy.  Results include, success, failed, and skipped if a script module condition was not met.  The policy hash link will show you the JSON representation of the policy at the time it was run on the endpoint.  Every time the policy is changed, the hash value is updated and new endpoints would show the new hash value.</p>
</asp:Content>
