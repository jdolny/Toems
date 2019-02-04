<%@ Page Title="" Language="C#" MasterPageFile="~/views/computers/computers.master" AutoEventWireup="true" CodeBehind="comments.aspx.cs" Inherits="Toems_FrontEnd.views.computers.comments" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li> <%= ComputerEntity.Name %></li>
    <li>General</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
        <%= ComputerEntity.Name %>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="DropDownActionsSub">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Add Comment" CssClass="main-action" /></li>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#comments').addClass("nav-current");
          });
         
    </script>

    <div class="size-4 column">
        Comment:
    </div>
    <br class="clear"/>
   
    <asp:TextBox ID="txtComment" runat="server" CssClass="descbox textbox-small" ClientIDMode="Static" TextMode="MultiLine"></asp:TextBox>

    <br class="clear"/>

    <br/>
    <asp:PlaceHolder runat="server" Id="placeholder"></asp:PlaceHolder>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>Allows you to add any comments to the computer.  Once a comment is added, it cannot be deleted or modified.  An example usage, could be to track repair history.</p>
</asp:Content>
