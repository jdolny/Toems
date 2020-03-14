<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/pxeboot/pxeboot.master" AutoEventWireup="true" CodeBehind="isogen.aspx.cs" Inherits="Toems_FrontEnd.views.admin.pxeboot.isogen" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
     <li><a href="<%= ResolveUrl("~/views/admin/pxeboot/isogen.aspx") %>?level=2">ISO / USB Generator</a></li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubNavTitle_Sub2" runat="server">

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub2" runat="server">

     <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_Click" Text="Generate ISO" CssClass="main-action" /></li>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" runat="server">
     <script type="text/javascript">
        $(document).ready(function() {
            $('#iso').addClass("nav-current");
        });
    </script>

       <div class="size-4 column">
        Kernel:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlKernel" runat="server" CssClass="ddlist">
        </asp:DropDownList>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Boot Image:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlBootImage" runat="server" CssClass="ddlist">
        </asp:DropDownList>
    </div>
     <br class="clear"/>
      <div class="size-4 column">
        Communication Server Cluster
    </div>

    <div class="size-5 column">
        <asp:DropDownList ID="ddlCluster" runat="server" CssClass="ddlist"/>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Kernel Arguments:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtKernelArgs" runat="server" CssClass="textbox">
        </asp:TextBox>
    </div>
    </asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="subsubHelp" runat="server">
</asp:Content>
