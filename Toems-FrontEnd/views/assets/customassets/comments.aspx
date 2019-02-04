<%@ Page Title="" Language="C#" MasterPageFile="~/views/assets/customassets/customassets.master" AutoEventWireup="true" CodeBehind="comments.aspx.cs" Inherits="Toems_FrontEnd.views.assets.customassets.comments" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub2">
    <li> <%=Asset.DisplayName %></li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub2">
    <%=Asset.DisplayName %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="DropDownActionsSub2" Runat="Server">
    <li><asp:LinkButton ID="buttonUpdate" runat="server" OnClick="buttonUpdate_OnClick" Text="Add Comment" CssClass="main-action" /></li>
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="SubContent2" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $('#comments').addClass("nav-current");
        });
    </script>
    <div class="size-4 column">
        Comment:
    </div>
    <br class="clear"/>
   
        <asp:TextBox ID="txtComment" runat="server" CssClass="textbox-small descbox" ClientIDMode="Static" TextMode="MultiLine"></asp:TextBox>

    <br class="clear"/>

    <br/>
    <asp:PlaceHolder runat="server" Id="placeholder"></asp:PlaceHolder>
  
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subsubHelp">
    <p>Allows you to add any comments to the Asset.  Once a comment is added, it cannot be deleted or modified.  An example usage, could be to track repair history.</p>
</asp:Content>