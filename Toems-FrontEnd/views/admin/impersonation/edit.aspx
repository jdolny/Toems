<%@ Page Title="" Language="C#" MasterPageFile="~/views/admin/impersonation/impersonation.master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="Toems_FrontEnd.views.admin.impersonation.edit" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li><%=ImpersonationAccount.Username %></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    <%=ImpersonationAccount.Username %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
   <li><asp:LinkButton ID="buttonUpdateAccount" runat="server" OnClick="buttonUpdateAccount_OnClick" Text="Update Account" CssClass="main-action" /></li>
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#general').addClass("nav-current");
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