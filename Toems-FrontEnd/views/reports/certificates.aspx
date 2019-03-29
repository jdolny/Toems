<%@ Page Title="" Language="C#" MasterPageFile="~/views/reports/reports.master" AutoEventWireup="true" CodeBehind="certificates.aspx.cs" Inherits="Toems_FrontEnd.views.reports.certificates" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Certificate List</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
 Certificate List
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnExport" runat="server" OnClick="btnExport_OnClick" Text="Export To CSV" CssClass="main-action"/></li>
</asp:Content>


<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
     <script type="text/javascript">
        $(document).ready(function() {
            $('#search').addClass("nav-current");
            $("[id*=gvSoftware] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });
    </script>
    <p class="total">
        <asp:Label ID="lblTotal" runat="server"></asp:Label>
    </p>
    <div class="size-7 column">
        <asp:TextBox ID="txtSearch" runat="server" CssClass="rounded-search" OnTextChanged="search_Changed"></asp:TextBox>
    </div>
    <br class="clear"/>

   <asp:GridView ID="gvCertificates" runat="server" AllowSorting="True" DataKeyNames="Id" OnSorting="gvSoftware_OnSorting" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="Store" HeaderText="Store" SortExpression="Store" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="FriendlyName" HeaderText="Friendly Name" SortExpression="FriendlyName" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Subject" HeaderText="Subject" SortExpression="Subject" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="NotBefore" HeaderText="Not Before" SortExpression="NotBefore" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="NotAfter" HeaderText="Not After" SortExpression="NotAfter" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="Subject" HeaderText="Subject" SortExpression="Subject" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="Issuer" HeaderText="Issuer" SortExpression="Issuer" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="Serial" HeaderText="Serial" SortExpression="Serial" ItemStyle-CssClass="width_200"></asp:BoundField>
             <asp:BoundField DataField="Thumbprint" HeaderText="Thumbprint" SortExpression="Thumbprint" ></asp:BoundField>
           
        </Columns>
        <EmptyDataTemplate>
            No Certificates Found
        </EmptyDataTemplate>
    </asp:GridView>

  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This is a simple report that displays every certificate that Theopenem is aware of.</p>
</asp:Content>
