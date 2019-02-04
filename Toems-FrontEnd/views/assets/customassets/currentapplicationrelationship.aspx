<%@ Page Title="" Language="C#" MasterPageFile="~/views/assets/customassets/customassets.master" AutoEventWireup="true" CodeBehind="currentapplicationrelationship.aspx.cs" Inherits="Toems_FrontEnd.views.assets.customassets.currentapplicationrelationship" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li> <%=Asset.DisplayName %></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    <%=Asset.DisplayName %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton runat="server" Id="btnRemove" OnClick="btnRemove_OnClick" Text="Remove Asset Applications" CssClass="main-action" ></asp:LinkButton></li>
    <li><asp:LinkButton runat="server" Id="btnUpdate" OnClick="btnUpdate_OnClick" Text="Update Match Types" ></asp:LinkButton></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#currentsoftwarelink').addClass("nav-current");
        });
    </script>
    
    <asp:GridView ID="gvSoftware" runat="server"  DataKeyNames="SoftwareId"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt" OnRowDataBound="gvSoftware_OnRowDataBound">
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
            <asp:BoundField DataField="SoftwareId" HeaderText="Id" SortExpression="Id" Visible="False"/>
            <asp:BoundField DataField="SoftwareAssetSoftwareId" HeaderText="Sas Id" Visible="True"/>
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
    <p>This page is only available when the Asset Type is set to Built-In Software.  It allows you to modify the existing application relationships.</p>
</asp:Content>
