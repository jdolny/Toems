<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/impersonation/impersonation.master" AutoEventWireup="true" CodeBehind="create.aspx.cs" Inherits="Toems_FrontEnd.views.admin.impersonation.create" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>Create</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    Impersonation Accounts
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="buttonAddAccount" runat="server" OnClick="buttonAddAccount_OnClick" Text="Add Account" CssClass="main-action"/></li>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#create').addClass("nav-current");
        });
    </script>
    <div class="size-4 column">
        User Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtUsername" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>

    </div>
    <br class="clear"/>
    <div class="size-4 column">
       Password:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtPassword" runat="server" CssClass="textbox password" TextMode="Password"></asp:TextBox>
    </div>
    <br class="clear"/>
  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <h5><span style="color: #ff9900;">Username:</span></h5>
<p>The username of the impersonation account.  If the user is a domain user, it should be in the format of domain\username</p>
<h5><span style="color: #ff9900;">Password:</span></h5>
<p>The password for the impersonation account.</p>
</asp:Content>