<%@ Page Title="" Language="C#" MasterPageFile="~/views/site.master" AutoEventWireup="true" Inherits="Toems_FrontEnd.views.dashboard.Mfa" Codebehind="mfa.aspx.cs" %>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle">
MFA
</asp:Content>

<asp:Content runat="server" ID="Breadcrumb" ContentPlaceHolderID="DropDownActions">
     <li><asp:LinkButton ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Verify MFA" CssClass="main-action"/></li>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="Server">

    <p>Your account is required to use Multi Factor Authentication.</p>  
    <p>Scan the QR Code below into your Authenticator app.</p>
    <p>Enter the code from your app into the Verification Code box below.  Then Select <b>Actions-> Verify MFA</b>.</p>
    <br />
    <p>
        <img runat="server" id="q"/></p>

    <br />
     <div class="size-4 column">
        Verification Code:
    </div>
    <div class="size-5 column">
         <asp:TextBox runat="server" ID="txtVerify" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
  

</asp:Content>
