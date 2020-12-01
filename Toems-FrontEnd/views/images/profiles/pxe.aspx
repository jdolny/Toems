<%@ Page Title="" Language="C#" MasterPageFile="~/views/images/profiles/profiles.master" AutoEventWireup="true" CodeBehind="pxe.aspx.cs" Inherits="Toems_FrontEnd.views.images.profiles.pxe" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
      <li>
        <a href="<%= ResolveUrl("~/views/images/profiles/general.aspx") %>?imageId=<%= ImageEntity.Id %>&profileId=<%= ImageProfile.Id %>&sub=profiles"><%= ImageProfile.Name %></a>
    </li>
 <li>PXE Options</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
     <%= ImageProfile.Name %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="btnUpdate" runat="server" Text="Update Profile" OnClick="btnUpdate_Click" CssClass="main-action"/></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#pxe').addClass("nav-current");
           

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
        Kernel Arguments:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtKernelArgs" runat="server" CssClass="textbox">
        </asp:TextBox>
    </div>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
  <p>Specifies the kernel and boot image that are used when pxe booting computers that have this image profile assigned and an active task has been created. 
      This is only used when an active web task for the computer with this profile is created. It does not apply to on demand or any other mode. Kernel arguments can also be passed in from here. 
      This only applies to the Linux imaging environment and only if pxe booted.</p>
</asp:Content>

