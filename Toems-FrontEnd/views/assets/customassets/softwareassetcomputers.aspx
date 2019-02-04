<%@ Page Title="" Language="C#" MasterPageFile="~/views/assets/customassets/customassets.master" AutoEventWireup="true" CodeBehind="softwareassetcomputers.aspx.cs" Inherits="Toems_FrontEnd.views.assets.customassets.softwareassetcomputers" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li> <%=Asset.DisplayName %></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    <%=Asset.DisplayName %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
  
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {

            $('#softwarecomputers').addClass("nav-current");
        });
    </script>
    
    <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>
    <asp:GridView ID="gvComputers" runat="server" AllowSorting="True" DataKeyNames="Id"  AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
           
            <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/computers/general.aspx?computerId={0}" Text="View" ItemStyle-CssClass="chkboxwidth" Target="_default"/>
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
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>This page is only available when the Asset Type is set to Built-In Software.  It displays all computers that matched any of the application relationships.</p>
</asp:Content>
