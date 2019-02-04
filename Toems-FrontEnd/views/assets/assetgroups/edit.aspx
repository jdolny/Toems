<%@ Page Title="" Language="C#" MasterPageFile="~/views/assets/assetgroups/assetgroups.master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="Toems_FrontEnd.views.assets.assetgroups.edit" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li> <%=AssetGroup.Name %></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    <%=AssetGroup.Name %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Update Asset Group" CssClass="main-action"/></li>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#general').addClass("nav-current");
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
    <p>The general page display the name and description of the Asset Group.</p>
</asp:Content>
