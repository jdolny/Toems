<%@ Page Title="" Language="C#" MasterPageFile="~/views/assets/customassets/customassets.master" AutoEventWireup="true" CodeBehind="create.aspx.cs" Inherits="Toems_FrontEnd.views.assets.customassets.create" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>Create</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    Custom Assets
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="buttonAdd" runat="server" OnClick="buttonAdd_OnClick" Text="Add Asset" CssClass="main-action" /></li>
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#create').addClass("nav-current");
        });
    </script>
    <div class="size-4 column">
        Display Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtName" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>

    </div>
    <br class="clear"/>
    <div class="size-4 column">
        Asset Type:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlAssetType" runat="server" CssClass="ddlist" />
    </div>
  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>The new page allows you to create a new Custom Asset. When creating a new Custom Asset, give the Asset a name and select the Asset Type.  The Asset Type cannot be modified after the Asset is created.  When the Asset is added, you can then provide values to the corresponding custom attributes for that Asset Type.</p>
</asp:Content>
