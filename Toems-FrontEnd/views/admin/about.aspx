<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/admin.master" AutoEventWireup="true" CodeBehind="about.aspx.cs" Inherits="Toems_FrontEnd.views.admin.about" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li>About</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
    Admin Settings
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="SubContent" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#about').addClass("nav-current");
        });
    </script>
     <div class="size-lbl column">
        Toems UI Version:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="tomesUiVersion" Text="1.4.7.0" ></asp:Label>
    </div>
    <br class="clear"/>

     <div class="size-lbl column">
        Toems API Version:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblToemsApi"></asp:Label>
    </div>
    <br class="clear"/>

    <div class="size-lbl column">
        Database Version:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblDatabaseVersion" ></asp:Label>
    </div>
    <br class="clear"/>
    <div class="size-lbl column">
        Expected Toec API Version:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblExpectedToecApi" ></asp:Label>
    </div>
    <br class="clear"/>
    <div class="size-lbl column">
        Expected Toems API Version:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblExpectedToemsApi" ></asp:Label>
    </div>
    <br class="clear"/>
    <div class="size-lbl column">
        Expected Toec Agent Version:
    </div>
    <div class="size-lbl2 column">
        <asp:Label runat="server" ID="lblExpectedToec" ></asp:Label>
    </div>
    <br class="clear"/>

    <h1>Toec API Com Servers</h1>
    <br class="clear" />
     <asp:PlaceHolder runat="server" Id="comServerPlaceholder"></asp:PlaceHolder>
   

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page displays the versions of Theopenem servers.</p>
</asp:Content>
