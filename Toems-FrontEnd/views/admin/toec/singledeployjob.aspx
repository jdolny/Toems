<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/toec/toec.master" AutoEventWireup="true" CodeBehind="singledeployjob.aspx.cs" Inherits="Toems_FrontEnd.views.admin.toec.singledeployjob" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TopBreadCrumbSub2" runat="server">
     <li><a href="<%= ResolveUrl("~/views/admin/toec/singledeployjob.aspx") %>?level=2">Single Deploy Job</a></li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="SubNavTitle_Sub2" runat="server">
    Single Deploy Job
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DropDownActionsSub2" runat="server">
    <li><asp:LinkButton ID="buttonDeploy" runat="server" OnClick="buttonDeploy_Click"  Text="Deploy Toec To Target" CssClass="main-action" /></li> 
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" runat="server">
     <script type="text/javascript">
        $(document).ready(function() {
            $('#singledeployjob').addClass("nav-current");
        });
     </script>

      <div class="size-4 column">
        Computer Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtName" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>

     <div class="size-4 column">
        User Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtUsername" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Password:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtPassword" runat="server" CssClass="textbox" TextMode="Password"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Domain (NetBIOS Format):
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtDomain" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Job Type:
    </div>
    <div class="size-5 column">
       
        <asp:DropDownList ID="ddlJobType" runat="server" CssClass="ddlist"></asp:DropDownList>
  
            </div>
    <br class="clear"/>
   
    

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="subsubHelp" runat="server">
   
</asp:Content>