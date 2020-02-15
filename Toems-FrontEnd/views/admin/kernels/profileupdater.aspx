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


</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="subsubHelp" runat="server">
</asp:Content>
