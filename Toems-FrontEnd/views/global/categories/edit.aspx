<%@ Page Title="" Language="C#" MasterPageFile="~/views/global/categories/categories.master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="Toems_FrontEnd.views.global.categories.edit" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li><% =Category.Name %></li>
     <li>General</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    <% =Category.Name %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="buttonUpdateCategory" runat="server" OnClick="buttonUpdateCategory_OnClick" Text="Update Category" CssClass="main-action"/></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
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
        <asp:TextBox ID="txtDescription" runat="server" CssClass="descbox" TextMode="MultiLine"></asp:TextBox>
    </div>
    <br class="clear"/>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>A category is mostly straight forward. Give the category a name and description, after it's added, it can be used by Groups, Policies, and Modules.</p>
</asp:Content>
