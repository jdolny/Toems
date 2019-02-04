<%@ Page Title="" Language="C#" MasterPageFile="~/views/groups/groups.master" AutoEventWireup="true" CodeBehind="assignedpolicies.aspx.cs" Inherits="Toems_FrontEnd.views.groups.assignedpolicies" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= GroupEntity.Name %></li>
    <li>Assigned Policies</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
<%= GroupEntity.Name %>
</asp:Content>


<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#assignedpolicies').addClass("nav-current");

            $("[id*=gvPolicies] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
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
    <br/>
 
    <asp:GridView ID="gvPolicies" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gridView_Sorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
                       
              <asp:TemplateField ItemStyle-CssClass="width_100" HeaderText="Order" SortExpression="PolicyOrder">
                <ItemTemplate>
                    <asp:TextBox ID="txtOrder" runat="server" CssClass="textbox height_18 order" Text='<%# Eval("PolicyOrder") %>' />
                </ItemTemplate>
            </asp:TemplateField>
           
            <asp:BoundField DataField="Id" Visible="False"/>
             <asp:BoundField DataField="Policy.Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Policy.Trigger" HeaderText="Trigger" SortExpression="Trigger" ItemStyle-CssClass="width_200"/>
              <asp:BoundField DataField="Policy.Frequency" HeaderText="Frequency" SortExpression="Frequency"/>
             <asp:HyperLinkField DataNavigateUrlFields="PolicyId" DataNavigateUrlFormatString="~/views/policies/general.aspx?policyId={0}" Target="_default" Text="View" ItemStyle-CssClass="chkboxwidth"/>
             <asp:TemplateField ItemStyle-CssClass="chkboxwidth">
                <ItemTemplate>
                    <asp:LinkButton ID="btnUpdate" runat="server" OnClick="btnUpdate_OnClick" Text="Update Order"/>
                </ItemTemplate>
            </asp:TemplateField>
                        <asp:TemplateField ItemStyle-CssClass="chkboxwidth">
                <ItemTemplate>
                    <asp:LinkButton ID="btnRemove" runat="server" OnClick="btnRemove_OnClick" Text="Remove"/>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            No Policies Found
        </EmptyDataTemplate>
    </asp:GridView>

 
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page displays all policies that are currently assigned to the group. The results can be filtered by policy name.  This page also allows you to set the order in which the policies run. Both negative and positive values can be used. Policies can also be removed from the group from this page.</p>
</asp:Content>
