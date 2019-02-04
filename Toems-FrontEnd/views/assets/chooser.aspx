<%@ Page Title="" Language="C#" MasterPageFile="~/views/assets/assets.master" AutoEventWireup="true" CodeBehind="chooser.aspx.cs" Inherits="Toems_FrontEnd.views.assets.chooser" %>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Assets
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubContent">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.actions_left').addClass("display-none");
        });

    </script>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>The Assets landing page is where you can create new Asset Types, Custom Assets, Asset Groups, and Custom Attributes.</p>
</asp:Content>
