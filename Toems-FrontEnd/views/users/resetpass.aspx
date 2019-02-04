<%@ Page Title="" Language="C#" MasterPageFile="~/views/users/user.master" AutoEventWireup="true" Inherits="Toems_FrontEnd.views.users.ResetPass" Codebehind="resetpass.aspx.cs" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">

    <li>Reset Password</li>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
Users
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Update User" CssClass="main-action"/></li>
</asp:Content>



<asp:Content ID="Content" ContentPlaceHolderID="SubContent" runat="Server">
    <div class="size-4 column">
        Use LDAP Authentication:
    </div>
    <div class="size-5 column">
        <asp:Label runat="server" ID="lblLdap"></asp:Label>
    </div>
    <br class="clear"/>
    <br class="clear"/>
    <div id="passwords" runat="server">
        <div class="size-4 column">
            New Password:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtUserPwd" runat="server" CssClass="textbox" TextMode="Password"></asp:TextBox>
        </div>
        <br class="clear"/>
        <div class="size-4 column">
            Confirm Password:
        </div>
        <div class="size-5 column">
            <asp:TextBox ID="txtUserPwdConfirm" runat="server" CssClass="textbox" TextMode="Password"></asp:TextBox>
        </div>
        <br class="clear"/>
    </div>
    <div class="size-4 column">
        Email:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtEmail" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
     <br class="clear"/>
    <div class="size-4 column">
        User Theme:
    </div>
    <div class="size-5 column">
        <asp:DropDownList ID="ddlTheme" runat="server" CssClass="ddlist">
          
        </asp:DropDownList>
    </div>

   


</asp:Content>