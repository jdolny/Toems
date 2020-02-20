<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/kernels/kernels.master" AutoEventWireup="true" CodeBehind="profileupdater.aspx.cs" Inherits="Toems_FrontEnd.views.admin.kernels.profileupdater" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
     <li><a href="<%= ResolveUrl("~/views/admin/kernels/profileupdater.aspx") %>?level=2">Image Profile Kernel Update</a></li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubNavTitle_Sub2" runat="server">

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub2" runat="server">

     <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_Click" Text="Update Profile Kernels" CssClass="main-action" /></li>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" runat="server">
     <script type="text/javascript">
        $(document).ready(function() {
            $('#profileupdater').addClass("nav-current");
        });
    </script>

      <div class="size-4 column">
        New Kernel:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlKernel" runat="server" CssClass="ddlist">
        </asp:DropDownList>
    </div>
    <br class="clear"/>
    <br/>
    <asp:GridView ID="gvProfiles" runat="server" DataKeyNames="Id" AutoGenerateColumns="False" CssClass="Gridview" AlternatingRowStyle-CssClass="alt">
        <Columns>

            <asp:TemplateField>
                <HeaderStyle CssClass="chkboxwidth"></HeaderStyle>
                <ItemStyle CssClass="chkboxwidth"></ItemStyle>
                <HeaderTemplate>
                    <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="True" OnCheckedChanged="chkSelectAll_CheckedChanged"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelector" runat="server"/>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:HyperLinkField DataNavigateUrlFields="Id,ImageId" DataNavigateUrlFormatString="~/views/images/profiles/general.aspx?imageid={1}&profileid={0}&sub=profiles" Text="View" ItemStyle-CssClass="chkboxwidth" Target="_blank"/>

            <asp:BoundField DataField="Id" HeaderText="ID" SortExpression="ID" Visible="False"/>
            <asp:BoundField DataField="ImageId" HeaderText="ImageID" SortExpression="ImageID" Visible="False"/>

            <asp:TemplateField ItemStyle-CssClass="mobi-hide-smaller" HeaderStyle-CssClass="width_200 mobi-hide-smaller" HeaderText="Image">
                <ItemTemplate>
                    <asp:Label ID="lblImage" runat="server" Text='<%# Bind("Image.Name") %>'/>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField DataField="Name" HeaderText="Profile Name" SortExpression="Name" ItemStyle-CssClass="width_200"></asp:BoundField>
            <asp:BoundField DataField="Kernel" HeaderText="Current Kernel" SortExpression="Name" ItemStyle-CssClass=""></asp:BoundField>

        </Columns>
        <EmptyDataTemplate>
            No Profiles Found
        </EmptyDataTemplate>
    </asp:GridView>

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="subsubHelp" runat="server">
</asp:Content>
