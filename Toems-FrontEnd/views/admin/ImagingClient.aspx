<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" CodeBehind="imagingclient.aspx.cs" Inherits="Toems_FrontEnd.views.admin.ImagingClient" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>Server</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Admin Settings
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnUpdateSettings" runat="server" Text="Update Server Settings" OnClick="btnUpdateSettings_OnClick" CssClass="main-action"/></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#imagingclient').addClass("nav-current");
        });
    </script>
    <div class="size-4 column">
        Global Imaging Arguments:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtArguments" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Enable iPXE SSL:
    </div>
    <div class="size-setting column hidden-check">
        <asp:CheckBox ID="chkIpxeSsl" runat="server" ClientIDMode="Static"></asp:CheckBox>
        <label for="chkIpxeSsl"></label>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        iPXE SSL Disabled Port:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtPort" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Imaging Task Timeout:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtImagingTimeout" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
  
  

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">


</asp:Content>