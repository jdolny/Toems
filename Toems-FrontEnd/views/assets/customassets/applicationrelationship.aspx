<%@ Page Title="" Language="C#" MasterPageFile="~/views/assets/customassets/customassets.master" AutoEventWireup="true" CodeBehind="applicationrelationship.aspx.cs" Inherits="Toems_FrontEnd.views.assets.customassets.applicationrelationship" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li> <%=Asset.DisplayName %></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    <%=Asset.DisplayName %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton runat="server" Id="btnAdd" OnClick="btnAdd_OnClick" Text="Update Asset Applications" CssClass="main-action" ></asp:LinkButton></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#softwarelink').addClass("nav-current");
        });
    </script>
    
    <asp:GridView ID="gvSoftware" runat="server"  DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvSoftware_OnRowDataBound">
        <Columns>
            <asp:TemplateField>
                <HeaderStyle CssClass="chkboxwidth"></HeaderStyle>
                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelector" runat="server"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>
                    Match Type
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:DropDownList runat="server" ID="ddlMatchType" CssClass="width_200"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Version" HeaderText="Version" ItemStyle-CssClass="width_100"></asp:BoundField>
            <asp:BoundField DataField="Major" HeaderText="Major" ItemStyle-CssClass="width_100"></asp:BoundField>
            <asp:BoundField DataField="Minor" HeaderText="Minor" ItemStyle-CssClass="width_100"></asp:BoundField>
            <asp:BoundField DataField="Build" HeaderText="Build" ></asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            No Software Found
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>This page is only available when the Asset Type is set to Built-In Software.  This allows you to map many software applications / versions to a single Software Asset.  For example, let's assume we purchased 25 copies of a software application named AtoZ and the license key is only valid for major version 3.  We could create the following custom attributes for the Built-In Software type:</p>
<ul>
	<li>License Key</li>
	<li>Purchased License Count</li>
	<li>Purchase Date</li>
	<li>PO Number</li>
</ul>
<p>We would then fill in the values for those custom attributes and map the AtoZ asset to an existing software application that has been discovered by Theopenem.  In this example, the match type would be set to Name_MajorVersion.  We can now easily see how many computers were found with that match type under the Computer Usage Page.  When creating application relationships, you can select as many as you want but each is joined together by an OR statement.  The resulting computer usage will display computers that match ANY.</p>
</asp:Content>
