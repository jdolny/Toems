<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/imageprep/imageprep.master" AutoEventWireup="true" CodeBehind="searchsetupcomplete.aspx.cs" Inherits="Toems_FrontEnd.views.admin.imageprep.searchsetupcomplete" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
     <li><a href="<%= ResolveUrl("~/views/admin/imageprep/searchsetupcomplete.aspx") %>?level=2">Search SetupComplete Files</a></li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubNavTitle_Sub2" runat="server">

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub2" runat="server">

     <li><asp:LinkButton runat="server" ID="btnDelete" Text="Delete Selected" OnClick="btnDelete_OnClick" CssClass="main-action"></asp:LinkButton></li>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" runat="server">
      <script type="text/javascript">
        $(document).ready(function() {
            $('#searchsetupcomplete').addClass("nav-current");
            $("[id*=gvEntries] td").hover(function() {
                $("td", $(this).closest("tr")).addClass("hover_row");
            }, function() {
                $("td", $(this).closest("tr")).removeClass("hover_row");
            });
        });
      </script>

    <asp:GridView ID="gvEntries" runat="server" AllowSorting="True" DataKeyNames="Id" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:TemplateField>
                <HeaderStyle CssClass="chkboxwidth"></HeaderStyle>
                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <HeaderTemplate>
                    <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="True" OnCheckedChanged="chkSelectAll_CheckedChanged1"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelector" runat="server"/>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/views/admin/imageprep/editsetupcomplete.aspx?setupCompleteId={0}&level=2" Text="View" ItemStyle-CssClass="chkboxwidth"/>
            <asp:BoundField DataField="Id" Visible="False"/>
            <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Description" HeaderText="Description" ></asp:BoundField>

        </Columns>
        <EmptyDataTemplate>
            No Entries Found
        </EmptyDataTemplate>
    </asp:GridView>

    <div id="confirmbox" class="confirm-box-outer">
        <div class="confirm-box-inner">
            <h4>
                <asp:Label ID="lblTitle" runat="server" Text="Delete The Selected SetupComplete Files?"></asp:Label>
            </h4>
            <div class="confirm-box-btns">
                <asp:LinkButton ID="ConfirmButton" OnClick="ButtonConfirmDelete_Click" runat="server" Text="Yes" CssClass="confirm_yes"/>
                <asp:LinkButton ID="CancelButton" runat="server" Text="No" CssClass="confirm_no"/>
            </div>
        </div>
    </div>
    

  
</asp:Content>

