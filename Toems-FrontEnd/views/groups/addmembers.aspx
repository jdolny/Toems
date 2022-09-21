<%@ Page Title="" Language="C#" MasterPageFile="~/views/groups/groups.master" AutoEventWireup="true" CodeBehind="addmembers.aspx.cs" Inherits="Toems_FrontEnd.views.groups.addmembers" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= GroupEntity.Name %></li>
    <li>Add Group Members</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
<%= GroupEntity.Name %>
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnAddSelected" runat="server" OnClick="btnAddSelected_OnClick" Text="Add Selected Computers" CssClass="main-action"/></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#addmembers').addClass("nav-current");

            $("[id*=gvComputers] td").hover(function() {
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
        <asp:DropDownList runat="server" ID="ddlLimit" AutoPostBack="True" OnSelectedIndexChanged="ddlLimit_OnSelectedIndexChanged" CssClass="ddlist">
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
    <br />
    
    <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>
    <asp:GridView ID="gvComputers" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gridView_Sorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:TemplateField>

                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <HeaderTemplate>
                    <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="True" OnCheckedChanged="chkSelectAll_CheckedChanged"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelector" runat="server"/>
                </ItemTemplate>
            </asp:TemplateField>
                <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/computers/general.aspx?computerId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:BoundField DataField="Id" HeaderText="computerID" SortExpression="computerID" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="LastCheckinTime" HeaderText="Last Checkin" SortExpression="LastCheckinTime" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="LastIp" HeaderText="Last Known Ip" SortExpression="LastIp" ItemStyle-CssClass="width_200" ></asp:BoundField>
               <asp:BoundField DataField="ClientVersion" HeaderText="Client Version" SortExpression="ClientVersion" ItemStyle-CssClass="width_200"></asp:BoundField> 
            <asp:BoundField DataField="ProvisionedTime" HeaderText="Provision Date" SortExpression="ProvisionedTime" ></asp:BoundField>
         


        </Columns>
        <EmptyDataTemplate>
            No Computers Found
        </EmptyDataTemplate>
    </asp:GridView>


</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page is only available when group type is set to static.  This allows you to manually add computers to the group.</p>
</asp:Content>
