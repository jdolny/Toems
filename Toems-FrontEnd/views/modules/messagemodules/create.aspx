<%@ Page Title="" Language="C#" MasterPageFile="~/views/modules/messagemodules/messagemodule.master" AutoEventWireup="true" CodeBehind="create.aspx.cs" Inherits="Toems_FrontEnd.views.modules.messagemodules.create" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li>New</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
 Message Modules
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="btnAdd" runat="server" OnClick="btnAdd_OnClick" Text="Add Module" CssClass="main-action" /></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#create').addClass("nav-current");
        });
    </script>
    <div class="size-4 column">
        Display Name
    </div>

    <div class="size-5 column">
        <asp:TextBox ID="txtDisplayName" runat="server" CssClass="textbox" ClientIDMode="Static"></asp:TextBox>
    </div>
    <br class="clear"/>
      <div class="size-4 column">
        Description
    </div>

    <div class="size-5 column">
        <asp:TextBox ID="txtDescription" runat="server" CssClass="descbox" TextMode="MultiLine"></asp:TextBox>
    </div>
    
     <br class="clear"/>
     <div class="size-4 column">
        Title
    </div>

    <br class="clear"/>
    <div class="size-5 column">
        <asp:TextBox ID="txtTitle" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>
     <div class="size-4 column">
        Timeout
    </div>
     <br class="clear"/>
    <div class="size-5 column">
        <asp:TextBox ID="txtTimeout" runat="server" CssClass="textbox" Text="0"></asp:TextBox>
    </div>
    <br class="clear"/>
      <div class="size-4 column">
        Message
    </div>
     <br class="clear"/>
      <div class="size-5 column">
     <asp:TextBox ID="txtMessage" CssClass="descbox" runat="server" TextMode="MultiLine"  ></asp:TextBox>
    </div>
     
    
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    Message Modules can only be assigned to Policies
    <h5><span style="color: #ff9900;"><strong>Display Name:</strong></span></h5>
<p>The name of the module, module names must be unique and contain only alphanumeric characters, space, underscore, or dash.</p>

<h5><span style="color: #ff9900;"><strong>Description:</strong></span></h5>
<p>The description field is optional for you to give a short description for what the module does.</p>
<h5><span style="color: #ff9900;"><strong>Title:</strong></span></h5>
<p>The title of the message that appears in the title bar.</p>
<h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Timeout</strong></span></h5>
<p class="alert alert-error">The time in seconds that the window should stay open before auto closing.  0 specifies until the user clicks ok.</p>
    <h5 class="alert alert-error"><span style="color: #ff9900;"><strong>Message</strong></span></h5>
<p class="alert alert-error">The content of the message.</p>
</asp:Content>

