<%@ Page Title="" Language="C#" MasterPageFile="~/views/groups/groups.master" AutoEventWireup="true" CodeBehind="message.aspx.cs" Inherits="Toems_FrontEnd.views.groups.message" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= GroupEntity.Name %></li>
    <li>Send Message</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
<%= GroupEntity.Name %>
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
    <li><asp:LinkButton ID="btnSend" runat="server" OnClick="btnSend_OnClick" Text="Send Message" CssClass="main-action"/></li>
</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#message').addClass("nav-current");
          });

    </script>
    
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
     <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" CssClass="descbox" ></asp:TextBox>
    
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>This page can be used to send a message to all the computers in the group.  A message will only display if a user is logged into the computer.  The message just needs a Title and the message itself.  The timeout value is used to automatically close the message after a certain time period.  The default is 0, meaning it will not automatically close.  This value is in seconds.</p>
</asp:Content>
