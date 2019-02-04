<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="customattributes.aspx.cs" Inherits="Toems_FrontEnd.views.computers.customattributes" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li> <%= ComputerEntity.Name %></li>
    <li>Custom Attributes</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    <%= ComputerEntity.Name %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub" runat="server">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Update Attributes" CssClass="main-action"></asp:LinkButton></li>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent" runat="server">
<script type="text/javascript">
    $(document).ready(function () {
        $('#customattributes').addClass("nav-current");
    });
    </script>

    <asp:PlaceHolder runat="server" Id="placeholder"></asp:PlaceHolder>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    Allows you to define custom attributes that can be used to track additional information.  Examples might include, asset tag number, windows product key, computer building name or room, etc.  Custom attributes can be defined in Global Properties->Custom Attributes.
</asp:Content>