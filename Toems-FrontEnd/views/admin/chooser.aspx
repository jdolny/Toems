<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" Inherits="Toems_FrontEnd.views.admin.AdminChooser" ValidateRequest="false" Codebehind="chooser.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Admin Settings
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.actions_left').addClass("display-none");
        });

    </script>

</asp:Content>