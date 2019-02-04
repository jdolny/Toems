<%@ Page Title="" Language="C#" MasterPageFile="~/views/assets/assetgroups/assetgroups.master" AutoEventWireup="true" CodeBehind="create.aspx.cs" Inherits="Toems_FrontEnd.views.assets.assetgroups.create" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>Create</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    Asset Groups
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="buttonAdd" runat="server" OnClick="buttonAdd_OnClick" Text="Add Asset Group" CssClass="main-action" /></li>
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#create').addClass("nav-current");
        });
    </script>
    <div class="size-4 column">
        Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtName" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>

    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Description:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtDescription" runat="server" CssClass="descbox" TextMode="MultiLine" ClientIDMode="Static"></asp:TextBox>
    </div>
  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>The new page is used to create a new Asset Group.  Give the group a name and description.  After the group is added, you will be able to add assets to the group.</p>
</asp:Content>
