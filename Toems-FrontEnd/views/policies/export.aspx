<%@ Page Title="" Language="C#" MasterPageFile="~/views/policies/policies.master" AutoEventWireup="true" CodeBehind="export.aspx.cs" Inherits="Toems_FrontEnd.views.policies.export" %>
<asp:Content runat="server" ContentPlaceHolderID="TopBreadCrumbSub1">
    <li><%= Policy.Name %></li>
    <li>Export Policy</li>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="SubNavTitle_Sub1">
        <%= Policy.Name %>
</asp:Content>
<asp:Content ID="Breadcrumb" ContentPlaceHolderID="DropDownActionsSub" Runat="Server">
  <li><asp:LinkButton ID="buttonExport" runat="server" OnClick="buttonExport_OnClick" Text="Export Policy" CssClass="main-action"></asp:LinkButton></li>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" Runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {
              $('#export').addClass("nav-current");
          });
          
    </script>
     <div class="size-4 column">
        Name:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtName" runat="server" CssClass="textbox"></asp:TextBox>
    </div>
    <br class="clear"/>

    <div class="size-4 column">
        Description:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtDescription" runat="server" CssClass="descbox" TextMode="MultiLine" Height="150px"></asp:TextBox>
    </div>
    <br class="clear"/>
    
    <div class="size-4 column">
        Instructions:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtInstructions" runat="server" CssClass="descbox" TextMode="MultiLine" Height="150px"></asp:TextBox>
    </div>
    <br class="clear"/>
    
    <div class="size-4 column">
        Requirements:
    </div>
    <div class="size-5 column">
        <asp:TextBox ID="txtRequirements" runat="server" CssClass="descbox" TextMode="MultiLine" Height="150px"></asp:TextBox>
    </div>
    <br class="clear"/>
    
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="subHelp">
    <p>The export page allows the policy to be exported and shared with other people.  When exporting a policy, a name and description must be provided.  Any special instructions should also be included as well as any other requirements for the policy.  Exporting a policy, exports all policy settings and all modules assigned to that policy.  Uploaded files are not exported, if special files are required, they should be noted in the requirements.  When possible a module should use external files, so after it's imported the necessary files can automatically be downloaded.</p>
</asp:Content>