<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="imaging.aspx.cs" Inherits="Toems_FrontEnd.views.computers.imaging" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li> <%= ComputerEntity.Name %></li>
    <li>Image Settings</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
        <%= ComputerEntity.Name %>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="DropDownActionsSub">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="btnUpdate_Click" Text="Update Image" CssClass="main-action" /></li>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#imaging').addClass("nav-current");
          });
         
    </script>

      <div class="size-4 column">
        Image:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlComputerImage" runat="server" CssClass="ddlist" AutoPostBack="true" OnSelectedIndexChanged="ddlComputerImage_OnSelectedIndexChanged"/>
    </div>
    <br class="clear"/>

    <div class="size-4 column">
        Image Profile:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlImageProfile" runat="server" CssClass="ddlist"/>
    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Effective Image:
    </div>
    <div class="size-5 column">
        <asp:Label ID="lblImage" runat="server"/>
    </div>
    <br class="clear"/>

     <div class="size-4 column">
        Effective Image Profile:
    </div>
    <div class="size-5 column">
        <asp:Label ID="lblImageProfile" runat="server"/>
    </div>
    <br class="clear"/>
    
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>Allows you to set an image for the computer to deploy or upload.  The effective image takes group membership into account.  An image does not need
        to be assigned directly to a computer, it can be from a group.
    </p>
</asp:Content>