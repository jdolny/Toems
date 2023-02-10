<%@ Page Title="" Language="C#" MasterPageFile="~/theopenem/views/users/user.master" AutoEventWireup="true" CodeBehind="groupcomputergroupacls.aspx.cs" Inherits="Toems_FrontEnd.views.users.groupcomputergroupacls" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
     <li>
        <a href="<%= ResolveUrl("~/views/users/searchgroup.aspx") %>">User Groups</a>
    </li>
    <li><%= ToemsUserGroup.Name %></li>
    <li>Computer Group Management</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
<%= ToemsUserGroup.Name %>
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_Click" Text="Update Computer Group Management" CssClass="main-action"/></li>
</asp:Content>


<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
<script type="text/javascript">
    $(document).ready(function () {
        $('#computeracl').addClass("nav-current");
    });
</script>


<div class="size-4 column">
    Enable Computer Group Management
</div>

<div class="size-10 column hidden-check">
    <asp:CheckBox runat="server" Id="chkEnableGroup" ClientIDMode="Static"/>
     <label for="chkEnableGroup"></label>
</div>
    <br class="clear" />

     
    <br/>
  

    <asp:GridView ID="gvGroups" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gridView_Sorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
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
            <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/groups/general.aspx?groupId={0}" Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:BoundField DataField="Id" HeaderText="computerID" SortExpression="computerID" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
              <asp:BoundField DataField="Type" HeaderText="Type" SortExpression="Type" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="MemberCount" HeaderText="Member Count" SortExpression="MemberCount" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Description" HeaderText="Description"  ></asp:BoundField>
        
        
        </Columns>
        <EmptyDataTemplate>
            No Groups Found
        </EmptyDataTemplate>
    </asp:GridView>


</asp:Content>